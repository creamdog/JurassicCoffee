using System;
using Jurassic;
using JurassicCoffee.Core;

namespace JurassicCoffee.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var compiler = new Core.Compiler();
            compiler.PrecompilationActions.Add(AddCompilationDate);
            compiler.PostcompilationActions.Add(Minify);
            compiler.PreScriptOutputActions.Add(AddMinToFileName);
            compiler.Compile("test.coffee");

        }

        private static string Minify(CompilerContext contex, string source)
        {
            source += "/* I should be minified to decrease bandwidth usage */";

            return source;
        }

        private static string AddCompilationDate(CompilerContext contex, string source)
        {
            return string.Format("compiledate = '{0}'{1}{2}", DateTime.Now.ToShortDateString(), Environment.NewLine, source);
        }

        private static string AddMinToFileName(CompilerContext contex, string filePath)
        {
            return filePath.Replace(".js", ".min.js");
        }
    }
}
