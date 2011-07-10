using System.Collections.Concurrent;
using System.Configuration;
using System.IO;
using System.Web;
using JurassicCoffee.Core;
using JurassicCoffee.Core.Diagnostics;
using JurassicCoffee.Core.Plugins;
using System.Linq;
using JurassicCoffee.Web.Collections.Concurrent;

namespace JurassicCoffee.Web
{
    public class JurassicCoffeeHttpHandler : IHttpHandler
    {
        private readonly FileSystemWatcher _inputFilesWatcher;
        private readonly FileSystemWatcher _outputFilesWatcher;
        private static readonly ConcurrentMonoPolyMap<string, string> CoffeeScriptIncludedFilesMap = new ConcurrentMonoPolyMap<string, string>();
        private static readonly ConcurrentDictionary<string, string> CoffeeJavascriptOutputMap = new ConcurrentDictionary<string, string>();
        
        private string _compiledJavascriptDirectorPath = string.Empty;
        private static readonly object Lock = new object();
        private bool _watchersInitialized;

        private readonly string _compiledJavascriptDirectoryName;

        public JurassicCoffeeHttpHandler()
        {
            _inputFilesWatcher = new FileSystemWatcher();
            _inputFilesWatcher.Changed += FileChanged;
            _inputFilesWatcher.Created += FileChanged;
            _inputFilesWatcher.Deleted += FileChanged;
            _inputFilesWatcher.Renamed += FileRenamed;

            _outputFilesWatcher = new FileSystemWatcher();
            _outputFilesWatcher.Changed += FileChanged;
            _outputFilesWatcher.Deleted += FileChanged;
            _outputFilesWatcher.Renamed += FileRenamed;

           

            _compiledJavascriptDirectoryName = ConfigurationManager.AppSettings["JurassicCoffee.CompiledDirectory"];

            _watchersInitialized = false;
        }

        private static void FileRenamed(object sender, RenamedEventArgs e)
        {
            var fileInfo = new FileInfo(e.FullPath);

            var dir = fileInfo.Directory.FullName;

            var fullPath = Path.Combine(dir, e.OldName);

            FileChanged(e, new FileSystemEventArgs(e.ChangeType, fullPath, e.OldName));
        }

        private static void FileChanged(object sender, FileSystemEventArgs e)
        {
            var fileInfo = new FileInfo(e.FullPath);
            string value;

            //do not reset for .js files that are output files except if they are renamed or deleted
            if (fileInfo.Extension != ".js" || (e.ChangeType == WatcherChangeTypes.Deleted || e.ChangeType == WatcherChangeTypes.Renamed))
            {
                CoffeeScriptIncludedFilesMap.RemoveKey(fileInfo.FullName);
                CoffeeJavascriptOutputMap.TryRemove(fileInfo.FullName, out value);
                foreach (var key in CoffeeScriptIncludedFilesMap.GetAllKeysForValue(fileInfo.FullName)) {
                    CoffeeScriptIncludedFilesMap.RemoveKey(key);
                    CoffeeJavascriptOutputMap.TryRemove(key, out value);
                }
            }

            //reset for re-compilation if .js is included or embedded
            if (fileInfo.Extension == ".js" && CoffeeScriptIncludedFilesMap.GetAllKeysForValue(fileInfo.FullName).Length > 0)
            {
                foreach (var key in CoffeeScriptIncludedFilesMap.GetAllKeysForValue(fileInfo.FullName))
                {
                    CoffeeScriptIncludedFilesMap.RemoveKey(key);
                    CoffeeJavascriptOutputMap.TryRemove(key, out value);
                }
            }
        }

        private CoffeeCompiler _coffeeCoffeeCompiler;
        private CoffeeCompiler CoffeeCoffeeCompiler
        {
            get
            {
                _coffeeCoffeeCompiler = _coffeeCoffeeCompiler ?? new CoffeeCompiler();
                _coffeeCoffeeCompiler.PostCompilationActions.Add(YahooYuiCompressor.Compress);
                return _coffeeCoffeeCompiler;
            }
        }

        public void ProcessRequest(HttpContext context)
        {
            var file = new FileInfo(context.Server.MapPath(context.Request.FilePath));
            ProcessCoffeeScriptRequest(context, file);
        }

        private void ProcessCoffeeScriptRequest(HttpContext context, FileInfo coffeeScriptFileInfo)
        {
            if (coffeeScriptFileInfo.Directory == null)
                throw new FileNotFoundException(context.Server.MapPath(context.Request.FilePath));

            if (CoffeeJavascriptOutputMap.ContainsKey(coffeeScriptFileInfo.FullName)) {
                var javascriptFileInfo = new FileInfo(CoffeeJavascriptOutputMap[coffeeScriptFileInfo.FullName]);
                context.Response.WriteFile(javascriptFileInfo.FullName);
                return;
            }
            
            var workingDirectory = coffeeScriptFileInfo.Directory.FullName;

            context.Response.ContentType = "text/javascript";



            if (!_watchersInitialized)
            {
                InitializeDirectory(context);
                InitializeWatchers(coffeeScriptFileInfo.Directory.FullName, _compiledJavascriptDirectorPath);
            }

            var root = context.Server.MapPath("/");
            var dir = coffeeScriptFileInfo.FullName.Substring(root.Length);
            dir = dir.Substring(0, dir.IndexOf(coffeeScriptFileInfo.Name));

            var outputFile = new FileInfo(Path.Combine(_compiledJavascriptDirectorPath, Path.Combine(dir, coffeeScriptFileInfo.Name + ".js")));

            if (!outputFile.Directory.Exists) {
                lock(Lock) {
                    if (!outputFile.Directory.Exists) {
                        outputFile.Directory.Create();
                    }
                }
            }

            using (var output = new StreamWriter(outputFile.FullName, false)) {
                using (var input = new StreamReader(coffeeScriptFileInfo.OpenRead())) {
                    var report = CoffeeCoffeeCompiler.Compile(workingDirectory, input, output);

                    CoffeeJavascriptOutputMap[coffeeScriptFileInfo.FullName] = outputFile.FullName;

                    foreach (var entry in report.Entries.Where(entry => entry is FileRecordEntry).Cast<FileRecordEntry>())
                        CoffeeScriptIncludedFilesMap.Add(coffeeScriptFileInfo.FullName, entry.FileInfo.FullName);      
                }
            }

            context.Response.WriteFile(outputFile.FullName);
        }

        private void InitializeDirectory(HttpContext context)
        {
            if (string.IsNullOrEmpty(_compiledJavascriptDirectorPath))
            {
                lock (Lock)
                {
                    if (string.IsNullOrEmpty(_compiledJavascriptDirectorPath))
                    {
                        _compiledJavascriptDirectorPath = context.Server.MapPath("/" + _compiledJavascriptDirectoryName);
                        var outputDirInfo = new DirectoryInfo(_compiledJavascriptDirectorPath);
                        if (!outputDirInfo.Exists)
                            outputDirInfo.Create();


                    }
                }
            }
        }

        private void InitializeWatchers(string coffeescriptFilesPath, string javascriptFilesPath)
        {
            lock(Lock)
            {
                if (_watchersInitialized)
                    return;

                if (string.IsNullOrEmpty(_inputFilesWatcher.Path)) {
                    _inputFilesWatcher.Path = coffeescriptFilesPath;
                    _inputFilesWatcher.Filter = "*.*";

                    _inputFilesWatcher.NotifyFilter =    NotifyFilters.LastWrite |
                                                                NotifyFilters.FileName |
                                                                NotifyFilters.DirectoryName;

                    _inputFilesWatcher.IncludeSubdirectories = true;
                    _inputFilesWatcher.EnableRaisingEvents = true;
                }

                if (string.IsNullOrEmpty(_outputFilesWatcher.Path)) {
                    _outputFilesWatcher.Path = javascriptFilesPath;
                    _outputFilesWatcher.Filter = "*.*";
                    _outputFilesWatcher.NotifyFilter =      NotifyFilters.LastWrite |
                                                                NotifyFilters.FileName |
                                                                NotifyFilters.DirectoryName;
                    _outputFilesWatcher.IncludeSubdirectories = true;
                    _outputFilesWatcher.EnableRaisingEvents = true;
                }

                _watchersInitialized = true;
            }
        }

        public bool IsReusable
        {
            get { return true; }
        }
    }
}
