using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using JurassicCoffee.Core.Diagnostics;
using JurassicCoffee.Core.IO;
using JurassicCoffee.Core.Plugins;
using NUnit.Framework;

namespace JurassicCoffee.Core.Tests.Tests.Plugins
{
    [TestFixture]
    public class RequiredCoffeeFilesTests
    {
        [Test]
        public void StupidRequiredFilesTest()
        {
            var list = RequiredCoffeeFiles.GetRequiredFiles(new CompilerContext(new CompilationRecorder(), false){WorkingDirectory = "C:/"}, @"
#= require apa/dsag/jhdfsfjsg.js
#= require `http://wwww.google.se`
#= require ftp://jfdhsjkfhsfh.com
#= require `ssl://jkfsdhjkfsh.com`
").ToArray();

            Assert.AreEqual(FileProtocol.LocalFile, list[0].Protocol);
            Assert.AreEqual(FileProtocol.Http, list[1].Protocol);
            Assert.AreEqual(FileProtocol.Ftp, list[2].Protocol);
            Assert.AreEqual(FileProtocol.Unknown, list[3].Protocol);

            Assert.AreEqual(false, list[0].IsEmbedded, "#= require apa/dsag/jhdfsfjsg.js should not be embedded");
            Assert.AreEqual(true, list[1].IsEmbedded, "#= require `http://wwww.google.se` should be embedded");
            Assert.AreEqual(false, list[2].IsEmbedded, "#= require ftp://jfdhsjkfhsfh.com should not be embedded");
            Assert.AreEqual(true, list[3].IsEmbedded, "#= require `ssl://jkfsdhjkfsh.com` should be embedded");

            foreach (var requiredFile in list)
                Console.WriteLine("location: {0}, protocol: {1}, Embedded: {2}",requiredFile.Location, requiredFile.Protocol, requiredFile.IsEmbedded);
        }

        [Test]
        public void TestRequireHttp()
        {
            var coffee =
@"
#= require `https://ajax.googleapis.com/ajax/libs/jquery/1.6.2/jquery.min.js`
/*this is the end. my only friend. the end*/
";

            var compiler = new CoffeeCompiler(true);

            string output;

            using (var ms = new MemoryStream(Encoding.Default.GetBytes(coffee)))
            using (var os = new MemoryStream())
            {
                compiler.Compile(new StreamReader(ms), new StreamWriter(os));
                output = Encoding.Default.GetString(os.GetBuffer());

            }


            Console.WriteLine(output);
        }
    }
}
