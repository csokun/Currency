using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Currency.Helpers
{
    public static class XmlSerializerHelper
    {
        public static void SerializeToXML<T>(T documentScheme, string documentPath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));

            using (TextWriter textWriter = new StreamWriter(documentPath))
            {
                serializer.Serialize(textWriter, documentScheme);
            }
        }

        public static List<T> DeserializeParams<T>(string documentPath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<T>));

            List<T> result = new List<T>();

            using (XmlReader reader = XmlReader.Create(documentPath))
            {
                result = (List<T>)serializer.Deserialize(reader);
            }

            return result;
        }

        public static T Deserialize<T>(string documentPath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));

            T obj;

            using (XmlReader reader = XmlReader.Create(documentPath))
            {
                obj = (T)serializer.Deserialize(reader);
            }

            return obj;
        }

    }
}
