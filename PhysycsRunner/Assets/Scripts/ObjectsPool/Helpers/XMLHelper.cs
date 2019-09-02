using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Helpers
{
    public class XMLHelper  
    {
        public static string Serialize(object details)
        {
            return Serialize(details, details.GetType());
        }

        public static string Serialize<T>(object details)
        {
            return Serialize(details, typeof(T));
        }

        public static string Serialize(object details, System.Type type)
        {
            var serializer = new XmlSerializer(type);
            var stream = new MemoryStream();

            serializer.Serialize(stream, details);

            var reader = new StreamReader(stream);

            stream.Position = 0;

            var retSrt = reader.ReadToEnd();

            stream.Flush();

            return retSrt;
        }

        public static T Deserialize<T>(string details)
        {
            return (T) Deserialize(details, typeof(T));
        }

        public static object Deserialize(string details, System.Type type)
        {
            var serializer = new XmlSerializer(type);
            var reader = XmlReader.Create(new StringReader(details));

            return serializer.Deserialize(reader);
        }
    }
}