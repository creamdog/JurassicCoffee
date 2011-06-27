using System;

namespace JurassicCoffee.Console
{
    class Program
    {
        static void Main(string[] args)
        {

            var compiler = new Core.Compiler(precompilationSteps: new Func<string, string, string>[] { AddCompilationDate }, postcompilationSteps: new Func<string, string, string>[] { AddSourceFileName, RemoveNewLines });

            
            compiler.Compile("test.coffee");

        }

        private static string AddCompilationDate(string file, string source)
        {
            return string.Format("compiledate = '{0}'{1}{2}", DateTime.Now.ToShortDateString(), Environment.NewLine, source);
        }

        private static string AddSourceFileName(string file, string source)
        {
            return string.Format("/*source file: {0}*/{1}{2}", file, source, Environment.NewLine);
        }

        private static string RemoveNewLines(string file, string source)
        {
            return source.Replace(Environment.NewLine, " ").Replace('\n',' ');
        }
    }
}
