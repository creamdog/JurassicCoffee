using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace JurassicCoffee.Console
{
	public static class Extensions
	{
		public static bool IsDirectory(this string path)
		{
			return !string.IsNullOrEmpty(path) && Directory.Exists(path) && !File.Exists(path);
		}
		public static bool IsFile(this string path)
		{
			return !string.IsNullOrEmpty(path) && !Directory.Exists(path) && File.Exists(path);
		}
	}
}
