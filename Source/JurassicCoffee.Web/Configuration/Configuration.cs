using System;
using System.Configuration;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace JurassicCoffee.Web.Configuration
{
    public class ConfigurationHandler : IConfigurationSectionHandler
    {
        public static string SectionName = "jurassic.coffee";
        public object Create(object parent, object configContext, XmlNode section)
        {
            var sb = new StringBuilder();
            var xmlDoc = new XmlDocument();
            xmlDoc.AppendChild(xmlDoc.ImportNode(section, true));
            xmlDoc.Save(XmlWriter.Create(sb));

            var sr = new StringReader(sb.ToString());
            return new XmlSerializer(typeof(Configuration), new XmlRootAttribute(SectionName)).Deserialize(sr) as Configuration;
        }
    }

    [Serializable]
    public class Configuration
    {
        [XmlElement]
        public string CompiledDirectory { get; set; }
        [XmlElement]
        public bool EnableCompression { get; set; }
        [XmlElement]
        public bool DebugMode { get; set; }
    }
}
