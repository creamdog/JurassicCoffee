using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace JurassicCoffee.Core
{
    public class Precompiler
    {
        public static string InsertRequiredFiles(string file, string coffeeScript)
        {
            return InsertRequiredFiles(coffeeScript, new List<string>() { new FileInfo(file).FullName.ToLower() });
        }

        public static string InsertRequiredFiles(string coffeeScript, List<string> includedRequiredFiles)
        {
            var requires = Regex.Matches(coffeeScript, "^@require\\s+(?<requiredFile>.+)$", RegexOptions.Multiline | RegexOptions.IgnoreCase);

            var offset = 0;
            foreach (Match require in requires)
            {
                var requiredCoffeeScriptFile = new FileInfo(require.Groups["requiredFile"].Value.Trim());

                var key = string.Empty;

                if (!includedRequiredFiles.Any(f => f == requiredCoffeeScriptFile.FullName.ToLower()))
                {
                    includedRequiredFiles.Add(requiredCoffeeScriptFile.FullName.ToLower());
                    var requiredCoffeeScript = File.ReadAllText(requiredCoffeeScriptFile.FullName);
                    requiredCoffeeScript = InsertRequiredFiles(requiredCoffeeScript, includedRequiredFiles);
                    key = string.Format("{0}", requiredCoffeeScript);
                }

                coffeeScript = coffeeScript.Remove(require.Index + offset, require.Length);
                coffeeScript = coffeeScript.Insert(require.Index + offset, key);
                offset += key.Length - require.Length;
            }

            return coffeeScript;
        }
    }
}
