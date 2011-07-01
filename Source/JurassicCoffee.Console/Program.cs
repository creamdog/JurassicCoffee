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
            var externalCompilationScriptPath = string.Empty;

            var p = new OptionSet {
                { "o|output=", "set output file", f => output = f },
                { "w|workingdir=", "set working directory", w => workingDirectory = w },
                { "c|compression","enable compression", c => compress = true },
                { "d|disableimport","disable #= require statements", d => require = false },
                { "e|external=", "set .js coffee compilation script", e => externalCompilationScriptPath = e },
                { "h|?|help","this help text", c => input = string.Empty }
            };

             p.Parse(args);

            if (string.IsNullOrEmpty(input) || !File.Exists(input)) {
                System.Console.WriteLine("[!] cannot find {0}", input);
                PrintHelp(p);
                return;
            }

            var inputFileInfo = new FileInfo(input);

            workingDirectory = string.IsNullOrEmpty(workingDirectory) ? inputFileInfo.Directory.FullName : workingDirectory;

            output = string.IsNullOrEmpty(output) ? Regex.Replace(inputFileInfo.FullName, inputFileInfo.Extension + "$", ".js", RegexOptions.IgnoreCase) : output;

            if (!string.IsNullOrEmpty(externalCompilationScriptPath)) {
                if (!File.Exists(externalCompilationScriptPath)) {
                    System.Console.WriteLine("[!] cannot find {0}", externalCompilationScriptPath);
                    PrintHelp(p);
                    return;
                }
                System.Console.WriteLine("compiling with script: {0}", externalCompilationScriptPath);
            }

            System.Console.WriteLine("compiling: {0}",input);
            System.Console.WriteLine("output: {0}", output);
            System.Console.WriteLine("workingDirectory: {0}", workingDirectory);
            System.Console.WriteLine("compression: {0}", compress);
            System.Console.WriteLine("imports: {0}", require);
            try
            {
                using (var inputStream = new StreamReader(File.OpenRead(input)))
                {
                    using(var outputStream = new StreamWriter(File.Open(output, FileMode.Create)))
                    {

                        var compiler = new CoffeeCompiler();

                        if (!string.IsNullOrEmpty(externalCompilationScriptPath))
                            compiler.CoffeeCompilerScriptPath = externalCompilationScriptPath;

                        if(compress)
                            compiler.PostCompilationActions.Add(Core.Plugins.YahooYuiCompressor.Compress);

                        if(!require)
                            compiler.PreCompilationActions.Clear();


                            compiler.Compile(workingDirectory, inputStream, outputStream);

                    }
                }

            } catch(Exception ex) {
                System.Console.WriteLine(ex.Message);
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
