using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using JurassicCoffee.Core.Diagnostics;
using JurassicCoffee.Core.IO;

namespace JurassicCoffee.Core.Plugins
{
    public class RequiredCoffeeFiles
    {
        public static string RequireMatchPattern = "#= require\\s+(?<requiredFile>`?(?<protocol>[a-z]+://)?.+)$";

        public class RequiredFile
        {
            public string Location { get; set; }
            public FileProtocol Protocol { get; set; }
            public bool IsEmbedded { get; set; }
            public Match RegexMatch { get; set; }
            public CompilerContext Context { get; set; }
        }

        public static string InsertRequiredFiles(CompilerContext context, string coffeeScript)
        {
            return InsertRequiredFiles(context, coffeeScript, new List<string>());
        }

        public static string ReplaceJurassicCoffeeSplot(CompilerContext context, string coffeeScript)
        {
            return coffeeScript.Replace("#JurassicCoffeeSplot#" + context.Id.ToString(), "`");
        }

        public static IEnumerable<RequiredFile> GetRequiredFiles(CompilerContext context, string script)
        {
            var matches = Regex.Matches(script, RequireMatchPattern, RegexOptions.Multiline | RegexOptions.IgnoreCase);
            var result = new List<RequiredFile>();
            foreach (Match match in matches)
            {
                var location = match.Groups["requiredFile"].Value.Trim();
                var protocolString = match.Groups["protocol"].Value ?? string.Empty;
                var isEmbedded = location.EndsWith("`") && location.StartsWith("`");
                location = isEmbedded ? location.Substring(1, location.Length - 2) : location;

                var file = new RequiredFile
                {
                    IsEmbedded = isEmbedded,
                    Location = location,
                    Protocol = FileProtocol.LocalFile,
                    RegexMatch = match,
                    Context = context
                };

                if (!string.IsNullOrEmpty(protocolString)) {
                    FileProtocol protocol;
                    Enum.TryParse<FileProtocol>(protocolString.Replace("://", string.Empty), true, out protocol);
                    file.Protocol = protocol;
                }

                if (file.Protocol == FileProtocol.LocalFile)
                    if (!new FileInfo(file.Location).Exists)
                        file.Location = Path.Combine(context.WorkingDirectory, file.Location);

                result.Add(file);
            }
            return result;
        }


        public static string InsertRequiredFiles(CompilerContext context, string script, List<string> includedRequiredFiles)
        {
            var requiredFiles = GetRequiredFiles(context, script);

            var offset = 0;
            foreach (var file in requiredFiles)
            {
                var source = string.Empty;

                if (!includedRequiredFiles.Any(f => f == file.Location.ToLower())) {
                    includedRequiredFiles.Add(file.Location.ToLower());
                    
                    //read source
                    string requiredScriptSource;
                    switch (file.Protocol)
                    {
                        case FileProtocol.LocalFile:
                            requiredScriptSource = File.ReadAllText(file.Location);
                        break;
                        case FileProtocol.Ftp:
                        case FileProtocol.Http:
                        case FileProtocol.Https:
                            using(var webclient = new WebClient())
                               requiredScriptSource = webclient.DownloadString(file.Location);
                        break;
                            
                        default:
                        requiredScriptSource = string.Format("`/*ERR: UNKNOWN PROTOCOL: {0}*/`", file.Location);
                        break;
                    }

                    if(file.Protocol == FileProtocol.LocalFile)
                        requiredScriptSource = InsertRequiredFiles(context, requiredScriptSource, includedRequiredFiles);

                    requiredScriptSource = file.IsEmbedded ? requiredScriptSource.Replace("`", "#JurassicCoffeeSplot#" + context.Id.ToString()) : requiredScriptSource;

                    source = string.Format("{0}", string.Format("{0}{1}{0}", file.IsEmbedded ? "`" : "", requiredScriptSource));

                    if (context.IsDebug)
                        source = string.Format("`/*[DBG:BEGIN] FILE: {0}*/`{2}{1}{2}`/*[DBG:END] FILE: {0}*/`", file.Location, source, Environment.NewLine);
                }
                else if (context.IsDebug)
                {
                    source = string.Format("`/*[DBG] DUPLICATE INCLUDE: {0}*/`{2}{1}", file.Location, source, Environment.NewLine);
                }

                script = script.Remove(file.RegexMatch.Index + offset, file.RegexMatch.Length);
                script = script.Insert(file.RegexMatch.Index + offset, source);
                offset += source.Length - file.RegexMatch.Length;

                context.CompilationRecorder.AddRecordEntry(new FileRecordEntry(file.Location, file.Protocol, file.IsEmbedded ? FileRecordEntry.FileInsertionMode.Embedded : FileRecordEntry.FileInsertionMode.Compiled, file.Location));
            }

            return script;
        }

        /*
        public static string InsertRequiredFiles(CompilerContext context, string script, List<string> includedRequiredFiles)
        {
            var requires = Regex.Matches(script, RequireMatchPattern, RegexOptions.Multiline | RegexOptions.IgnoreCase);

            var offset = 0;
            foreach (Match requiredMatch in requires)
            {
                var path = requiredMatch.Groups["requiredFile"].Value.Trim();

                var isEmbedded = path.EndsWith("`") && path.StartsWith("`");

                path = isEmbedded ? path.Substring(1, path.Length - 2) : path;

                var requiredScriptFile = new FileInfo(path);

                var source = string.Empty;

                if (!requiredScriptFile.Exists)
                    requiredScriptFile = new FileInfo(Path.Combine(context.WorkingDirectory, path));

                if (!includedRequiredFiles.Any(f => f == requiredScriptFile.FullName.ToLower()))
                {
                    includedRequiredFiles.Add(requiredScriptFile.FullName.ToLower());
                    var requiredScriptSource = File.ReadAllText(requiredScriptFile.FullName);
                    requiredScriptSource = InsertRequiredFiles(context, requiredScriptSource, includedRequiredFiles);

                    requiredScriptSource = isEmbedded ? requiredScriptSource.Replace("`", "#JurassicCoffeeSplot#" + context.Id.ToString()) : requiredScriptSource;

                    source = string.Format("{0}", string.Format("{0}{1}{0}", isEmbedded ? "`" : "", requiredScriptSource));

                    if (context.IsDebug)
                        source = string.Format("/*[DBG:BEGIN] FILE: {0}/{2}{1}{2}/*[DBG:END] FILE: {0}/", requiredScriptFile.FullName, source, Environment.NewLine);
                }
                else if (context.IsDebug)
                {
                    source = string.Format("/*[DBG] DUPLICATE INCLUDE: {0}/{2}{1}", requiredScriptFile.FullName, source, Environment.NewLine);
                }

                script = script.Remove(requiredMatch.Index + offset, requiredMatch.Length);
                script = script.Insert(requiredMatch.Index + offset, source);
                offset += source.Length - requiredMatch.Length;

                context.CompilationRecorder.AddRecordEntry(new FileRecordEntry(requiredScriptFile, isEmbedded ? FileRecordEntry.FileInsertionMode.Embedded : FileRecordEntry.FileInsertionMode.Compiled, requiredScriptFile.FullName));
            }

            return script;
        }
        */
    }
}
