using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace JurassicCoffee.Core
{
    public class Precompiler
    {
        public static string CompileRequiredFiles(Compiler compiler, string coffeeScript, string[] compiledRequiredFiles)
        {
            var requires = Regex.Matches(coffeeScript, "^@require\\s+(?<requiredFile>.+)$", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            
            var offset = 0;
            var newCompiledRequiredFiles = new List<string>(compiledRequiredFiles);
            foreach (Match require in requires)
            {
                var requiredCoffeeScriptFile = require.Groups["requiredFile"].Value.Trim();

                var key = string.Empty;

                if (!newCompiledRequiredFiles.Contains(requiredCoffeeScriptFile))
                {
                    newCompiledRequiredFiles.Add(requiredCoffeeScriptFile);
                    var compiledFileInfo = compiler.Compile(requiredCoffeeScriptFile, newCompiledRequiredFiles.ToArray());
                    key = string.Format("/*@require {0}*/", compiledFileInfo.FullName);
                }

                coffeeScript = coffeeScript.Remove(require.Index + offset, require.Length);
                coffeeScript = coffeeScript.Insert(require.Index + offset, key);
                offset += key.Length - require.Length;
            }

            return coffeeScript;
        }

        public static void InsertRequiredFiles(string javascriptFile)
        {
            var fileInfo = new FileInfo(javascriptFile);

            if(!fileInfo.Exists)
                throw new FileNotFoundException(fileInfo.FullName);

            var javaScript = File.ReadAllText(fileInfo.FullName);
            var requires = Regex.Matches(javaScript, "^/\\*@require\\s+(?<requiredFile>.+)\\*/;$", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            var offset = 0;

            foreach (Match require in requires)
            {
                var requiredJavaScriptFile = require.Groups["requiredFile"].Value.Trim();
                var requiredJavaScript = File.ReadAllText(requiredJavaScriptFile);
                javaScript = javaScript.Remove(require.Index + offset, require.Length);
                javaScript = javaScript.Insert(require.Index + offset, requiredJavaScript);
                offset += requiredJavaScript.Length - require.Length;
            }

            if (requires.Count > 0)
                File.WriteAllText(javascriptFile, javaScript);
        }
    }
}
