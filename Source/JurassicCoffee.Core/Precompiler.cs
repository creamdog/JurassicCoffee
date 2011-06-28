using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace JurassicCoffee.Core
{
    public class Precompiler
    {
        public static string InsertRequiredFiles(CompilerContext context, string coffeeScript)
        {
            return InsertRequiredFiles(context, coffeeScript, new List<string>());
        }

        public static string ReplaceJurassicCoffeeSplot(CompilerContext context, string coffeeScript)
        {
            return coffeeScript.Replace("#JurassicCoffeeSplot#" + context.Id.ToString(), "`");
        }

        public static string InsertRequiredFiles(CompilerContext context, string script, List<string> includedRequiredFiles)
        {
            var requires = Regex.Matches(script, "#= require\\s+(?<requiredFile>.+)$", RegexOptions.Multiline | RegexOptions.IgnoreCase);

            var offset = 0;
            foreach (Match require in requires)
            {
                var path = require.Groups["requiredFile"].Value.Trim();

                var isInline = path.EndsWith("`");

                path = isInline ? path.Substring(0, path.Length - 1) : path;

                var requiredScriptFile = new FileInfo(path);

                var key = string.Empty;

                if (!requiredScriptFile.Exists)
                    requiredScriptFile = new FileInfo(Path.Combine(context.WorkingDirectory, path));

                if (!includedRequiredFiles.Any(f => f == requiredScriptFile.FullName.ToLower()))
                {
                    includedRequiredFiles.Add(requiredScriptFile.FullName.ToLower());
                    var requiredScript = File.ReadAllText(requiredScriptFile.FullName);
                    requiredScript = InsertRequiredFiles(context, requiredScript, includedRequiredFiles);
                    key = string.Format("{0}", requiredScript.Replace("`","#JurassicCoffeeSplot#" + context.Id.ToString()));
                }

                var matchLength = isInline ? require.Length-2 : require.Length;

                script = script.Remove(require.Index + offset, matchLength);
                script = script.Insert(require.Index + offset, key);
                offset += key.Length - matchLength;
            }

            return script;
        }
    }
}
