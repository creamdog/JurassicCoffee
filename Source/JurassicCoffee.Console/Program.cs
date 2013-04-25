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

			if (string.IsNullOrEmpty(input) || !Directory.Exists(input))
			{
				System.Console.WriteLine("[!] cannot find {0}", input);
				PrintHelp(p);
				return;
			}

			System.Console.WriteLine("workingDirectory: {0}", workingDirectory);
			System.Console.WriteLine("compression: {0}", compress);
			System.Console.WriteLine("imports: {0}", require);

			var inputDirectory = new DirectoryInfo(input);
			var outputDirectory = (string.IsNullOrEmpty(output))? inputDirectory: new DirectoryInfo(output);
			workingDirectory = string.IsNullOrEmpty(workingDirectory) ? inputDirectory.FullName : workingDirectory;

			if (!outputDirectory.Exists)
				outputDirectory.Create();

			foreach (var inputFile in inputDirectory.EnumerateFiles("*.coffee"))
			{
				var outputFile = Path.Combine(outputDirectory.FullName, Regex.Replace(inputFile.Name, inputFile.Extension + "$", ".js", RegexOptions.IgnoreCase));

				if (!string.IsNullOrEmpty(externalCompilationScriptPath))
				{
					if (!File.Exists(externalCompilationScriptPath))
					{
						System.Console.WriteLine("[!] cannot find {0}", externalCompilationScriptPath);
						PrintHelp(p);
						return;
					}
					System.Console.WriteLine("compiling with script: {0}", externalCompilationScriptPath);
				}

				System.Console.WriteLine("compiling: {0}", inputFile);
				System.Console.WriteLine("output: {0}", outputFile);

				try
				{
					using (var inputStream = new StreamReader(inputFile.OpenRead()))
					{
						using (var outputStream = new StreamWriter(File.Open(outputFile, FileMode.Create, FileAccess.Write)))
						{
							var compiler = new CoffeeCompiler();

							if (!string.IsNullOrEmpty(externalCompilationScriptPath))
								compiler.CoffeeCompilerScriptPath = externalCompilationScriptPath;

							if (compress)
								compiler.PostCompilationActions.Add(Core.Plugins.YahooYuiCompressor.Compress);

							if (!require)
								compiler.PreCompilationActions.Clear();

							compiler.Compile(workingDirectory, inputStream, outputStream);
						}
					}
				}
				catch (Exception ex)
				{
					System.Console.WriteLine(ex.Message);
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
