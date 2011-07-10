using System;

namespace JurassicCoffee.Core.Plugins
{
    public class YahooYuiCompressor
    {
        public static string Compress(CompilerContext context, string javascript)
        {
            try
            {
                var minifiedJavascript = Yahoo.Yui.Compressor.JavaScriptCompressor.Compress(javascript);
                return minifiedJavascript;
            }catch(Exception ex)
            {
                return string.Format("/*Yahoo.Yui.Compressor.Exception: {0}*/{1}{2}", ex.Message, Environment.NewLine, javascript);
            }
        }
    }
}
