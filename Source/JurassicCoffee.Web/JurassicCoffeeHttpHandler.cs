using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using JurassicCoffee.Core;

namespace JurassicCoffee.Web
{
    public class JurassicCoffeeHttpHandler : IHttpHandler
    {
        private Compiler _coffeeCompiler;
        private Compiler CoffeeCompiler
        {
            get
            {
                _coffeeCompiler = _coffeeCompiler ?? new Compiler();
                return _coffeeCompiler;
            }
        }

        public void ProcessRequest(HttpContext context)
        {
            var file = new FileInfo(context.Server.MapPath(context.Request.FilePath));
            using (var output = new StreamWriter(context.Response.OutputStream))
            {
                using (var input = new StreamReader(file.OpenRead()))
                {
                    context.Response.ContentType = "text/javascript";
                    var workingDirectory = file.Directory.FullName;
                    CoffeeCompiler.Compile(workingDirectory, input, output);
                }
            }
        }

        public bool IsReusable
        {
            get { return true; }
        }
    }
}
