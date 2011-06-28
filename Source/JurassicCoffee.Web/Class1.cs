using System;
using System.Collections.Generic;
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
            //CoffeeCompiler.Compile(context.Response.Output)
            //context.Request.Files
        }

        public bool IsReusable
        {
            get { return true; }
        }
    }
}
