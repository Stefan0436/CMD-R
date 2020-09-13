using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace CMDR
{
    public static class Serializer
    {
        public static string Serialize<t>(t input)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(t));
            string xml = "";

            using (StringWriter writer = new StringWriter())
            {
                serializer.Serialize(writer, input);
                xml = writer.ToString();
            }

            return xml;
        }

        public static t Deserialize<t>(string xml)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(t));
            t output;
            using (StringReader reader = new StringReader(xml))
            {
                output = (t)serializer.Deserialize(reader);
            }

            return output;
        }
    }
}
