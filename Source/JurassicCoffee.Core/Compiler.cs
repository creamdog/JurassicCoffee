using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using Jurassic;
using System.Linq;

namespace JurassicCoffee.Core
{
    public class Compiler
    {
        [ThreadStatic]
        private static ScriptEngine _engine;

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

        private readonly List<Func<CompilerContext, string, string>> _precompilationActions;
        public List<Func<CompilerContext, string, string>> PrecompilationActions
        {
            get { return _precompilationActions; }
        }

        private readonly List<Func<CompilerContext, string, string>> _postcompilationActions;
        public List<Func<CompilerContext, string, string>> PostcompilationActions
        {
            get { return _postcompilationActions; }
        }

        
        public Compiler()
        {
            _precompilationActions = new List<Func<CompilerContext, string, string>>();
            _postcompilationActions = new List<Func<CompilerContext, string, string>>();
            _preScriptLoadActions = new List<Func<CompilerContext, string, string>>();
            _preScriptOutputActions = new List<Func<CompilerContext, string, string>>();
            PrecompilationActions.Add(Precompiler.InsertRequiredFiles);
            PostcompilationActions.Add(Precompiler.ReplaceJurassicCoffeeSplot);
        }

        private static ScriptEngine Engine
        {
            get
            {
                if (_engine == null)
                {
                    _engine = new ScriptEngine();
                    _engine.Execute(CoffeeScriptResource);
                }

                return _engine;
            }
        }

        private static String CoffeeScriptResource
        {
            get
            {

                using(var stream = Assembly.GetAssembly(typeof(Compiler)).GetManifestResourceStream("JurassicCoffee.Core.coffee-script.js"))
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

        public FileInfo Compile(string coffeeScriptFile)
        {
            return Compile(coffeeScriptFile, new List<string>());
        }

        internal FileInfo Compile(string coffeeScriptFilePath, List<string> includedRequiredFiles)
        {
            var context = new CompilerContext { WorkingDirectory = new FileInfo(coffeeScriptFilePath).Directory.FullName };

            var inputFilePath = _preScriptLoadActions.Aggregate(coffeeScriptFilePath, (current, action) => action(context, current));

            var coffeeScriptFileInfo = new FileInfo(inputFilePath);

            context = new CompilerContext { WorkingDirectory = coffeeScriptFileInfo.Directory.FullName };

            var javaScriptFilePath = Regex.Replace(coffeeScriptFileInfo.FullName, coffeeScriptFileInfo.Extension + "$", ".js", RegexOptions.IgnoreCase);

            javaScriptFilePath = _preScriptOutputActions.Aggregate(javaScriptFilePath, (current, action) => action(context, current));

            using (var input = new StreamReader(File.OpenRead(coffeeScriptFileInfo.FullName)))
            {
                using (var output = new StreamWriter(File.Open(javaScriptFilePath, FileMode.Create)))
                {
                    Compile(context, input, output);
                }
            }

            return new FileInfo(javaScriptFilePath);
        }

        public void Compile(StreamReader input, StreamWriter output)
        {
            Compile(string.Empty, input, output);
        }

        public void Compile(string workingDirectory, StreamReader input, StreamWriter output)
        {
            Compile(new CompilerContext {WorkingDirectory = workingDirectory}, input, output);
        }

        private void Compile(CompilerContext context, StreamReader input, StreamWriter output)
        {
            var coffeeScript = input.ReadToEnd();

            coffeeScript = _precompilationActions.Aggregate(coffeeScript, (current, action) => action(context, current));

            var javascript = CompileString(coffeeScript);

            javascript = _postcompilationActions.Aggregate(javascript, (current, action) => action(context, current));

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
