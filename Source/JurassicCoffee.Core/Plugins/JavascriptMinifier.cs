using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Jurassic;

namespace JurassicCoffee.Core.Plugins
{
    public class JavascriptMinifier
    {
        [ThreadStatic]
        private static ScriptEngine _engine;
        private static ScriptEngine Engine
        {
            get
            {
                if (_engine == null)
                {
                    _engine = new ScriptEngine();
                    _engine.Execute(JsMinResource);
                }

                return _engine;
            }
        }

        private static String JsMinResource
        {
            get
            {

                using (var stream = Assembly.GetAssembly(typeof(Compiler)).GetManifestResourceStream("JurassicCoffee.Core.jsmin.js"))
                {
                    if (stream == null)
                        throw new NullReferenceException();

                    using (var reader = new StreamReader(stream))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
        }

        public static string Minify(CompilerContext context, string javascript)
        {
            Engine.SetGlobalValue("Source", javascript);
            var minifiedJavascript = Engine.Evaluate<string>("jsmin(Source)");
            return minifiedJavascript;
        }
    }
}
