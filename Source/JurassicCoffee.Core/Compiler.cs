using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
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
                    using(var reader = new StreamReader(stream))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
        }

        public void Compile(string coffeScriptFile)
        {
            var coffeeScript = File.ReadAllText(coffeScriptFile);
            var javascript = CompileString(coffeeScript);
            var javaScriptFile = Regex.Replace(coffeScriptFile, ".coffee$", ".js", RegexOptions.IgnoreCase);
            File.WriteAllText(javaScriptFile, javascript);
        }

        public string CompileString(string coffeeScript)
        {
            Engine.SetGlobalValue("Source", coffeeScript);
            var javascript =  Engine.Evaluate<string>("CoffeeScript.compile(Source, {bare: true})");
            return javascript;
        }
    }
}
