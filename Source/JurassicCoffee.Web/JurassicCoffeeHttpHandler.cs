using System.Collections.Concurrent;
using System.Configuration;
using System.IO;
using System.Web;
using JurassicCoffee.Core;
using JurassicCoffee.Core.Plugins;
using System.Linq;

namespace JurassicCoffee.Web
{
    public class JurassicCoffeeHttpHandler : IHttpHandler
    {
        private readonly FileSystemWatcher _coffeeScriptFilesWatcher;
        private readonly FileSystemWatcher _javascriptFilesWatcher;
        private static readonly ConcurrentDictionary<string, string> CoffeeJavascriptFilesMap = new ConcurrentDictionary<string, string>();
        
        private string _compiledJavascriptDirectorPath = string.Empty;
        private static readonly object Lock = new object();
        private bool _watchersInitialized;

        private string _compiledJavascriptDirectoryName;
        private string CompiledJavascriptDirectoryName
        {
            get 
            { 
                _compiledJavascriptDirectoryName = _compiledJavascriptDirectoryName ?? ConfigurationManager.AppSettings["JurassicCoffee.CompiledDirectory"];
                return _compiledJavascriptDirectoryName;
            }
        }

        public JurassicCoffeeHttpHandler()
        {
            _coffeeScriptFilesWatcher = new FileSystemWatcher();
            _coffeeScriptFilesWatcher.Changed += FileChanged;
            _coffeeScriptFilesWatcher.Created += FileChanged;
            _coffeeScriptFilesWatcher.Deleted += FileChanged;
            _coffeeScriptFilesWatcher.Renamed += FileRenamed;

            _javascriptFilesWatcher = new FileSystemWatcher();
            _javascriptFilesWatcher.Changed += FileChanged;
            _javascriptFilesWatcher.Deleted += FileChanged;
            _javascriptFilesWatcher.Renamed += FileRenamed;

            _watchersInitialized = false;
        }

        private void FileRenamed(object sender, RenamedEventArgs e)
        {
            var fileInfo = new FileInfo(e.FullPath);

            var dir = fileInfo.Directory.FullName;

            var fullPath = Path.Combine(dir, e.OldName);

            FileChanged(e, new FileSystemEventArgs(e.ChangeType, fullPath, e.OldName));
        }

        private void FileChanged(object sender, FileSystemEventArgs e)
        {
            var fileInfo = new FileInfo(e.FullPath);
            string removeFile;

            if (fileInfo.Extension == ".js" && (e.ChangeType == WatcherChangeTypes.Deleted || e.ChangeType == WatcherChangeTypes.Renamed))
            {
                //javascript files
                var keys = CoffeeJavascriptFilesMap
                    .Where(a => a.Value == fileInfo.FullName)
                    .Select(a => a.Key).ToArray();

                foreach (var key in keys)
                    CoffeeJavascriptFilesMap.TryRemove(key, out removeFile);
            } else if (fileInfo.Extension == ".coffee") {

                if (CoffeeJavascriptFilesMap.ContainsKey(fileInfo.FullName))
                    CoffeeJavascriptFilesMap.TryRemove(fileInfo.FullName, out removeFile);
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

            if (CoffeeJavascriptFilesMap.ContainsKey(coffeeScriptFileInfo.FullName)) {
                var javascriptFileInfo = new FileInfo(CoffeeJavascriptFilesMap[coffeeScriptFileInfo.FullName]);
                context.Response.WriteFile(javascriptFileInfo.FullName);
                return;
            }
            
            var workingDirectory = coffeeScriptFileInfo.Directory.FullName;

            context.Response.ContentType = "text/javascript";

            if(string.IsNullOrEmpty(_compiledJavascriptDirectorPath))
            {
                lock (Lock)
                {
                    if (string.IsNullOrEmpty(_compiledJavascriptDirectorPath))
                    {
                        _compiledJavascriptDirectorPath = context.Server.MapPath("/" + CompiledJavascriptDirectoryName);
                        var outputDirInfo = new DirectoryInfo(_compiledJavascriptDirectorPath);
                        if (!outputDirInfo.Exists)
                            outputDirInfo.Create();

                        if (!_watchersInitialized) {
                            InitializeWatchers(coffeeScriptFileInfo.Directory.FullName, _compiledJavascriptDirectorPath);
                        }
                    }
                }
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
                    CoffeeCoffeeCompiler.Compile(workingDirectory, input, output);
                }
            }

            CoffeeJavascriptFilesMap[coffeeScriptFileInfo.FullName] = outputFile.FullName;
            context.Response.WriteFile(outputFile.FullName);
        }

        private void InitializeWatchers(string coffeescriptFilesPath, string javascriptFilesPath)
        {
            lock(Lock)
            {
                if (_watchersInitialized)
                    return;

                if (string.IsNullOrEmpty(_coffeeScriptFilesWatcher.Path)) {
                    _coffeeScriptFilesWatcher.Path = coffeescriptFilesPath;
                    _coffeeScriptFilesWatcher.Filter = "*.coffee";

                    _coffeeScriptFilesWatcher.NotifyFilter =    NotifyFilters.LastWrite |
                                                                NotifyFilters.FileName |
                                                                NotifyFilters.DirectoryName;

                    _coffeeScriptFilesWatcher.IncludeSubdirectories = true;
                    _coffeeScriptFilesWatcher.EnableRaisingEvents = true;
                }

                if (string.IsNullOrEmpty(_javascriptFilesWatcher.Path)) {
                    _javascriptFilesWatcher.Path = javascriptFilesPath;
                    _javascriptFilesWatcher.Filter = "*.js";
                    _javascriptFilesWatcher.NotifyFilter =      NotifyFilters.LastWrite |
                                                                NotifyFilters.FileName |
                                                                NotifyFilters.DirectoryName;
                    _javascriptFilesWatcher.IncludeSubdirectories = true;
                    _javascriptFilesWatcher.EnableRaisingEvents = true;
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
