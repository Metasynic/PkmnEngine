using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;
using System.Xml;

namespace XLvlEditor
{
    /* This class is an exact copy of the PkmnEditor.XmlTool in the PkmnEditor project.
     * It works in exactly the same way as the other copy of the XmlTool.
     * Please refer to the annotations in PkmnEditor.XmlTool for how this works. */
    public static class XmlTool
    {
        public static T Load<T>(string path)
        {
            T data;
            using (FileStream stream = new FileStream(path, System.IO.FileMode.Open))
            {
                using (XmlReader reader = XmlReader.Create(stream))
                {
                    data = IntermediateSerializer.Deserialize<T>(reader, null);
                }
            }
            return data;
        }

        public static void Save<T>(string path, T obj)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            using (XmlWriter writer = XmlWriter.Create(path, settings))
            {
                IntermediateSerializer.Serialize<T>(writer, obj, null);
            }
        }
    }
}
