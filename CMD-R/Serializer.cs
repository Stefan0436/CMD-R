using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace CMDR
{
    public static class Serializer
    {
        public static string Serialize<t>(t input)
        {
            if (input is IDictionary) {
                XmlDocument document = new XmlDocument();
                document.AppendChild(document.CreateElement("Config"));

                IDictionary dictionary = (IDictionary)input;
                int i = 0;
                foreach (Object obj in dictionary.Keys)
                {
                    XmlNode nd = document.DocumentElement.AppendChild(document.CreateElement("Entry"));

                    XmlDocument ch1 = new XmlDocument();
                    ch1.LoadXml(Serialize(obj));
                    ch1.RemoveChild(ch1.FirstChild);
                    nd.AppendChild(document.ImportNode(ch1.FirstChild, true));

                    Object val = null;
                    int i2 = 0;
                    foreach (Object v in dictionary.Values) {
                        if (i2 == i) {
                            val = v;
                            break;
                        }
                        i2++;
                    }

                    XmlDocument ch2 = new XmlDocument();
                    ch2.LoadXml(Serialize(val));
                    ch2.RemoveChild(ch2.FirstChild);
                    nd.AppendChild(document.ImportNode(ch2.FirstChild, true));
                    i++;
                }

                StringWriter strW = new StringWriter();
                XmlTextWriter xmlW = new XmlTextWriter(strW)
                {
                    Formatting = Formatting.Indented
                };
                document.WriteTo(xmlW);
                xmlW.Close();
                strW.Close();
                String xmlO = strW.ToString();
                return xmlO;
            }

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
            if (typeof(IDictionary).IsAssignableFrom(typeof(t)))
            {
                IDictionary d = (IDictionary)typeof(t).GetConstructor(new Type[0]).Invoke(new object[0]);
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);

                foreach (XmlNode nd in doc.DocumentElement.ChildNodes)
                {
                    Object key = null;
                    Object val = null;

                    foreach (XmlNode ch in nd.ChildNodes)
                    {
                        if (key == null)
                        {
                            StringWriter strW = new StringWriter();
                            XmlTextWriter xmlW = new XmlTextWriter(strW);
                            ch.WriteTo(xmlW);
                            xmlW.Close();
                            strW.Close();
                            key = Deserialize<Object>(strW.ToString());
                        }
                        else if (val == null)
                        {
                            StringWriter strW = new StringWriter();
                            XmlTextWriter xmlW = new XmlTextWriter(strW);
                            ch.WriteTo(xmlW);
                            xmlW.Close();
                            strW.Close();
                            val = Deserialize<Object>(strW.ToString());
                        }
                    }

                    d.Add(key, val);
                }

                return (t)d;
            }

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
