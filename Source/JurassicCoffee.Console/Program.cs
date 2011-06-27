using System;

namespace JurassicCoffee.Console
{
    class Program
    {
        static void Main(string[] args)
        {

            var compiler = new Core.Compiler(postcompilationSteps: new Func<string, string, string>[] { AddSourceFileName });

            
            compiler.Compile("test.coffee");

        }


        private static string AddSourceFileName(string file, string source)
        {
            source += Environment.NewLine+"//from file: " + file + Environment.NewLine;
            return source;
        }
    }
}
