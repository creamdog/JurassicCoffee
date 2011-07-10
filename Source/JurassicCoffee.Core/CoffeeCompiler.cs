using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using Jurassic;
using System.Linq;
using JurassicCoffee.Core.Diagnostics;
using JurassicCoffee.Core.Plugins;

namespace JurassicCoffee.Core
{
    public class CoffeeCompiler
    {
        private readonly List<Func<CompilerContext, string, string>> _preScriptLoadActions;
        public List<Func<CompilerContext, string, string>> PreScriptLoadActions
        {
            get { return _preScriptLoadActions; }
        }

        private readonly List<Func<CompilerContext, string, string>> _preScriptOutputActions;
        public List<Func<CompilerContext, string, string>> PreScriptOutputActions
        {
            get { return _preScriptOutputActions; }
        }

        private readonly List<Func<CompilerContext, string, string>> _preCompilationActions;
        public List<Func<CompilerContext, string, string>> PreCompilationActions
        {
            get { return _preCompilationActions; }
        }

        private readonly List<Func<CompilerContext, string, string>> _postCompilationActions;
        public List<Func<CompilerContext, string, string>> PostCompilationActions
        {
            get { return _postCompilationActions; }
        }

        public string CoffeeCompilerScriptPath { get; set; }

        
        public CoffeeCompiler()
        {
            _preCompilationActions = new List<Func<CompilerContext, string, string>>();
            _postCompilationActions = new List<Func<CompilerContext, string, string>>();
            _preScriptLoadActions = new List<Func<CompilerContext, string, string>>();
            _preScriptOutputActions = new List<Func<CompilerContext, string, string>>();
            PreCompilationActions.Add(RequiredCoffeeFiles.InsertRequiredFiles);
            PostCompilationActions.Add(RequiredCoffeeFiles.ReplaceJurassicCoffeeSplot);
        }

        [ThreadStatic]
        private static ScriptEngine _engine;
        private ScriptEngine Engine
        {
            get
            {
                if (_engine == null)
                {
                    _engine = new ScriptEngine();
                    _engine.Execute(CoffeeCompilerScript);
                }

                return _engine;
            }
        }

        

        private Stream GetCoffeeCompilerScriptStream()
        {
            return string.IsNullOrEmpty(CoffeeCompilerScriptPath)
            ? Assembly.GetAssembly(typeof(CoffeeCompiler)).GetManifestResourceStream("JurassicCoffee.Core.coffee-script.js")
            : File.OpenRead(CoffeeCompilerScriptPath);
        }

        private String CoffeeCompilerScript
        {
            get
            {

                using (var stream = GetCoffeeCompilerScriptStream())
                {
                    if (stream == null)
                        throw new NullReferenceException();

                    using(var reader = new StreamReader(stream))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
        }

        public CompilationRecord Compile(string coffeeScriptFile)
        {
            return Compile(coffeeScriptFile, new List<string>());
        }

        internal CompilationRecord Compile(string coffeeScriptFilePath, List<string> includedRequiredFiles)
        {
            var compilationRecorder = new CompilationRecorder();
            compilationRecorder.Start();
            var coffeeScriptFileInfo = new FileInfo(coffeeScriptFilePath);
            var context = new CompilerContext(compilationRecorder) { WorkingDirectory = coffeeScriptFileInfo.Directory.FullName };
            return Compile(context, coffeeScriptFileInfo,  includedRequiredFiles);
        }

        internal CompilationRecord Compile(CompilerContext context, FileInfo coffeeScriptFileInfo, List<string> includedRequiredFiles)
        {
            var inputFilePath = _preScriptLoadActions.Aggregate(coffeeScriptFileInfo.FullName, (current, action) => action(context, current));

            coffeeScriptFileInfo = new FileInfo(inputFilePath);

            context = new CompilerContext(context.CompilationRecorder) { WorkingDirectory = coffeeScriptFileInfo.Directory.FullName };

            var javaScriptFilePath = Regex.Replace(coffeeScriptFileInfo.FullName, coffeeScriptFileInfo.Extension + "$", ".js", RegexOptions.IgnoreCase);

            javaScriptFilePath = _preScriptOutputActions.Aggregate(javaScriptFilePath, (current, action) => action(context, current));

            using (var input = new StreamReader(File.OpenRead(coffeeScriptFileInfo.FullName)))
            {
                using (var output = new StreamWriter(File.Open(javaScriptFilePath, FileMode.Create)))
                {
                    Compile(context, input, output);
                }
            }

            context.CompilationRecorder.Stop();

            return context.CompilationRecorder.GetRecord();
        }

        public void Compile(StreamReader input, StreamWriter output)
        {
            Compile(string.Empty, input, output);
        }

        public CompilationRecord Compile(string workingDirectory, StreamReader input, StreamWriter output)
        {
            var compilationRecorder = new CompilationRecorder();
            compilationRecorder.Start();
            Compile(new CompilerContext(compilationRecorder) { WorkingDirectory = workingDirectory }, input, output);
            compilationRecorder.Stop();
            return compilationRecorder.GetRecord();
        }

        private void Compile(CompilerContext context, StreamReader input, StreamWriter output)
        {
            var coffeeScript = input.ReadToEnd();

            coffeeScript = _preCompilationActions.Aggregate(coffeeScript, (current, action) => action(context, current));
            
            string javascript;

            try {
                javascript = CompileString(coffeeScript);
            } catch(JavaScriptException ex) {
                throw new CompilerException(ex.Message, ex);
            }

            javascript = _postCompilationActions.Aggregate(javascript, (current, action) => action(context, current));

            output.Write(javascript);
        }

        public string CompileString(string coffeeScript)
        {
            Engine.SetGlobalValue("Source", coffeeScript);
            var javascript = Engine.Evaluate<string>("CoffeeScript.compile(Source, {bare: true})");
            return javascript;
        }
    }
}
