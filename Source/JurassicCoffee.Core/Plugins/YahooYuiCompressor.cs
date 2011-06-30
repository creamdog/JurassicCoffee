namespace JurassicCoffee.Core.Plugins
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
