using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Jurassic;
using System.Linq;
using JurassicCoffee.Core;

namespace JurassicCoffee.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = args.FirstOrDefault();
            var output = string.Empty;
            var workingDirectory = string.Empty;
            var compress = false;
            var require = true;

            var p = new OptionSet
            {
                { "o|output=", "set output file", f => output = f },
                { "w|workingdir=", "set working directory", w => workingDirectory = w },
                { "c|compression","enable compression", c => compress = true },
                { "d|disableimport","disable #= require statements", c => require = false },
                { "h|?|help","this help text", c => input = string.Empty }
            };

            var e = p.Parse(args);

            if (string.IsNullOrEmpty(input) || !File.Exists(input))
            {
                PrintHelp(p);
                return;
            }

            var inputFileInfo = new FileInfo(input);

            workingDirectory = string.IsNullOrEmpty(workingDirectory) ? inputFileInfo.Directory.FullName : workingDirectory;

            output = string.IsNullOrEmpty(output) ? Regex.Replace(inputFileInfo.FullName, inputFileInfo.Extension + "$", ".js", RegexOptions.IgnoreCase) : output;


            System.Console.WriteLine("compiling: {0}",input);
            System.Console.WriteLine("output: {0}", output);
            System.Console.WriteLine("workingDirectory: {0}", workingDirectory);
            System.Console.WriteLine("compression: {0}", compress);
            System.Console.WriteLine("imports: {0}", require);

            using(var inputStream = new StreamReader(File.OpenRead(input)))
            {
                using(var outputStream = new StreamWriter(File.Open(output, FileMode.Create)))
                {

                    var compiler = new CoffeeCompiler();

                    if(compress)
                        compiler.PostcompilationActions.Add(Core.Plugins.YahooYuiCompressor.Compress);

                    if(!require)
                        compiler.PrecompilationActions.Clear();

                    compiler.Compile(workingDirectory, inputStream, outputStream);
                }
            }
            
        }

        private static void PrintHelp(OptionSet optionSet)
        {
            System.Console.WriteLine(HelpFileResource);

            var sb = new StringBuilder();
            var sw = new StringWriter(sb);
            optionSet.WriteOptionDescriptions(sw);

            System.Console.WriteLine(sb);
       
        }

        private static String HelpFileResource
        {
            get
            {

                using (var stream = Assembly.GetAssembly(typeof(Program)).GetManifestResourceStream("JurassicCoffee.Console.Help.txt"))
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

    }
}
