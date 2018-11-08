using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;
using System.Xml;

namespace PkmnEditor
{
    /* This class provides two functions that wrap the Monogame IntermediateSerializer.
     * The functions are used to convert the Pokemon, moves, and trainers to and from XML. */
    public static class XmlTool
    {
        /* The Load<T>() function loads an object from a certain file path. */
        public static T Load<T>(string path)
        {
            /* A generically typed variable is declared, which will be returned at the end.
             * Using a new FileStream with Open as the FileMode, and an XmlReader that uses the stream, 
             * the object is deserialized from the XML file in the path. */
            T data;
            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                using (XmlReader reader = XmlReader.Create(stream))
                {
                    data = IntermediateSerializer.Deserialize<T>(reader, null);
                }
            }

            /* The object is then returned. */
            return data;
        }

        /* The Save<T>() function is the opposite of Load<T>(). */
        public static void Save<T>(string path, T obj)
        {
            /* A new XmlWriterSettings object with Indent as true is created so that the serialized XML doesn't look horrible. */
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;

            /* Using a new XmlWriter and the settings, the object passed in is serialized to an XML file at the path passed in. */
            using (XmlWriter writer = XmlWriter.Create(path, settings))
            {
                IntermediateSerializer.Serialize<T>(writer, obj, null);
            }
        }
    }
}
