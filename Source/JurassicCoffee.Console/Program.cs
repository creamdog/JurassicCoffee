using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Jurassic;
using System.Linq;
using JurassicCoffee.Core;
//"F:\Development\Jurassic Coffee\Source\JurassicCoffee.Console\Test\test.coffee"
namespace JurassicCoffee.Console
{
	class Program
	{
		static void Main(string[] args)
		{
			var input = args.FirstOrDefault();
			var output = string.Empty;
			var workingDirectory = string.Empty;
			var compress = true;
			var require = true;
			var externalCompilationScriptPath = string.Empty;

			var optionSet = new OptionSet {
                { "o|output=", "set output file", f => output = f },
                { "w|workingdir=", "set working directory", w => workingDirectory = w },
                { "c|compression","enable compression", c => compress = true },
                { "d|disableimport","disable #= require statements", d => require = false },
                { "e|external=", "set .js coffee compilation script", e => externalCompilationScriptPath = e },
                { "h|?|help","this help text", c => input = string.Empty }
            };
			optionSet.Parse(args);

			if (!string.IsNullOrEmpty(externalCompilationScriptPath))
			{
				if (!File.Exists(externalCompilationScriptPath))
				{
					System.Console.WriteLine("[!] cannot find {0}", externalCompilationScriptPath);
					PrintHelp(optionSet);
					return;
				}
				System.Console.WriteLine("compiling with script: {0}", externalCompilationScriptPath);
			}

			//var fileExtension = (compress) ? ".min.js" : ".js";
			var fileExtension = ".js";

			if (input.IsDirectory())
			{
				var inputDirectory = new DirectoryInfo(input);
				var outputDirectory = GetOutputDirectory(output, ref workingDirectory, inputDirectory);
				foreach (var inputFile in inputDirectory.EnumerateFiles("*.coffee"))
				{
					CompileFile(workingDirectory, compress, require, externalCompilationScriptPath, outputDirectory, fileExtension, inputFile);
				}
			}
			else if (input.IsFile())
			{
				var inputFile = new FileInfo(input);
				var inputDirectory = new DirectoryInfo(Path.GetDirectoryName(input));
				var outputDirectory = GetOutputDirectory(output, ref workingDirectory, inputDirectory);
				CompileFile(workingDirectory, compress, require, externalCompilationScriptPath, outputDirectory, fileExtension, inputFile);
			}
			else
			{
				System.Console.WriteLine("[!] cannot find {0}", input);
				PrintHelp(optionSet);
				return;
			}
			System.Console.WriteLine("-----------------------");
		}

		private static DirectoryInfo GetOutputDirectory(string output, ref string workingDirectory, DirectoryInfo inputDirectory)
		{
			var outputDirectory = string.IsNullOrEmpty(output) ? inputDirectory : new DirectoryInfo(output);
			if (!outputDirectory.Exists) outputDirectory.Create();
			workingDirectory = string.IsNullOrEmpty(workingDirectory) ? inputDirectory.FullName : workingDirectory;
			return outputDirectory;
		}

		private static void CompileFile(string workingDirectory, bool compress, bool require, string externalCompilationScriptPath, DirectoryInfo outputDirectory, string fileExtension, FileInfo inputFile)
		{
			var outputFile = Path.Combine(outputDirectory.FullName, Regex.Replace(inputFile.Name, inputFile.Extension + "$", fileExtension, RegexOptions.IgnoreCase));
			System.Console.WriteLine("coffee: {0}", inputFile);
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
