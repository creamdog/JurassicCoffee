using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using JurassicCoffee.Core;
using JurassicCoffee.Core.Plugins;
using JurassicCoffee.Web.Plugins;
using YahooYuiCompressor = JurassicCoffee.Web.Plugins.YahooYuiCompressor;

namespace JurassicCoffee.Web
{
    public class JurassicCoffeeHttpHandler : IHttpHandler
    {
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

            if(file.Directory == null)
                throw new FileNotFoundException(context.Server.MapPath(context.Request.FilePath));

            var workingDirectory = file.Directory.FullName;

            context.Response.ContentType = "text/javascript";

            using (var output = new StreamWriter(context.Response.OutputStream))
            {
                using (var input = new StreamReader(file.OpenRead()))
                {
                    CoffeeCoffeeCompiler.Compile(workingDirectory, input, output);
                }
            }
        }

        public bool IsReusable
        {
            get { return true; }
        }
    }
}
