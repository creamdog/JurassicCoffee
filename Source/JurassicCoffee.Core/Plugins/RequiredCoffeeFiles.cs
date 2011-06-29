using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace JurassicCoffee.Core.Plugins
{
    public class RequiredCoffeeFiles
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
            foreach (Match requiredMatch in requires)
            {
                var path = requiredMatch.Groups["requiredFile"].Value.Trim();

                var isInline = path.EndsWith("`");

                path = isInline ? path.Substring(0, path.Length - 1) : path;

                var requiredScriptFile = new FileInfo(path);

                var source = string.Empty;

                if (!requiredScriptFile.Exists)
                    requiredScriptFile = new FileInfo(Path.Combine(context.WorkingDirectory, path));

                if (!includedRequiredFiles.Any(f => f == requiredScriptFile.FullName.ToLower()))
                {
                    includedRequiredFiles.Add(requiredScriptFile.FullName.ToLower());
                    var requiredScriptSource = File.ReadAllText(requiredScriptFile.FullName);
                    requiredScriptSource = InsertRequiredFiles(context, requiredScriptSource, includedRequiredFiles);

                    requiredScriptSource = isInline ? requiredScriptSource.Replace("`", "#JurassicCoffeeSplot#" + context.Id.ToString()) : requiredScriptSource;

                    source = string.Format("{0}", requiredScriptSource);
                }

                var matchLength = isInline ? requiredMatch.Length - 2 : requiredMatch.Length;

                script = script.Remove(requiredMatch.Index + offset, matchLength);
                script = script.Insert(requiredMatch.Index + offset, source);
                offset += source.Length - matchLength;
            }

            return script;
        }
    }
}
