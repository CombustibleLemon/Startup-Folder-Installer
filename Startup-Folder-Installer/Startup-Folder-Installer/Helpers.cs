using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Xml;

namespace Startup_Folder_Installer
{
    namespace Helpers
    {
        public static class DanXML
        {
            public static XmlNode FindNode(XmlDocument document, List<string> entityTree)
            {
                List<CheckBox> list = new List<CheckBox>();
                XmlNodeList nodes = document.ChildNodes;

                foreach (string currentEntity in entityTree)
                {
                    foreach (XmlNode nodeChild in nodes)
                    {
                        // If names are equal AND this is the final item in entityTree
                        if (currentEntity.ToLower() == nodeChild.Name.ToLower() && entityTree.IndexOf(currentEntity) == entityTree.Count - 1)
                        {
                            return nodeChild;
                        }
                        // If names are equal
                        else if (currentEntity.ToLower() == nodeChild.Name.ToLower())
                        {
                            nodes = nodeChild.ChildNodes;
                        }
                    }
                }
                return null;
            }
        }

        public static class DanFile
        {
            public static void ExtractEmbeddedResource(string resourceLocation, string outputDir, List<string> files)
            {
                foreach (string file in files)
                {
                    using (System.IO.Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceLocation + @"." + file))
                    {
                        using (System.IO.FileStream fileStream = new System.IO.FileStream(System.IO.Path.Combine(outputDir, file), System.IO.FileMode.Create))
                        {
                            for (int i = 0; i < stream.Length; i++)
                            {
                                fileStream.WriteByte((byte)stream.ReadByte());
                            }
                            fileStream.Close();
                        }
                    }
                }
            }
        }
    }
}
