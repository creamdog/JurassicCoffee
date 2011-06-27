using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using Jurassic;

namespace JurassicCoffee.Core
{
    public class Compiler
    {
        [ThreadStatic]
        private static ScriptEngine _engine;
        private static object _o = new object();

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
            return Compile(coffeeScriptFile, new string[0]);
        }

        internal FileInfo Compile(string coffeeScriptFile, string[] compiledRequiredFiles)
        {
            try
            {
                var coffeeScriptFileInfo = new FileInfo(coffeeScriptFile);

                ValidateCoffeeScriptFile(coffeeScriptFileInfo);

                var coffeeScript = File.ReadAllText(coffeeScriptFile);

                coffeeScript = Precompiler.CompileRequiredFiles(this, coffeeScript, compiledRequiredFiles);
                
                var javascript = CompileString(coffeeScript);

                var javaScriptFile = Regex.Replace(coffeeScriptFileInfo.FullName, coffeeScriptFileInfo.Extension + "$", ".js", RegexOptions.IgnoreCase);
                File.WriteAllText(javaScriptFile, javascript);

                Precompiler.InsertRequiredFiles(javaScriptFile);

                return new FileInfo(javaScriptFile);

            } catch(Exception ex) {
                throw new Exception(coffeeScriptFile, ex);
            }
        }

        public string CompileString(string coffeeScript)
        {
            Engine.SetGlobalValue("Source", coffeeScript);
            var javascript = Engine.Evaluate<string>("CoffeeScript.compile(Source, {bare: true})");
            return javascript;
        }


        private static void ValidateCoffeeScriptFile(FileSystemInfo coffeeScriptFileInfo)
        {
            if (!coffeeScriptFileInfo.Exists)
                throw new FileNotFoundException(coffeeScriptFileInfo.FullName);

            if (coffeeScriptFileInfo.Extension != ".coffee")
                throw new IOException(string.Format("file does not end with .coffee : {0}", coffeeScriptFileInfo.FullName));
        }
    }
}
