using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JurassicCoffee.Core;

namespace JurassicCoffee.Web.Plugins
{
    public class YahooYuiCompressor
    {
        public static string Compress(CompilerContext context, string javascript)
        {
            var minifiedJavascript = Yahoo.Yui.Compressor.JavaScriptCompressor.Compress(javascript);
            return minifiedJavascript;
        }
    }
}
