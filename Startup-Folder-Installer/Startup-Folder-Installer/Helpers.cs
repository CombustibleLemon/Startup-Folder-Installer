using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Xml;

namespace Startup_Folder_Installer
{
    public static class Helpers
    {
        /// <summary>
        /// Creates <code>CheckBox</code> objects using data in <code>nodes</code> for elements named <code>entityToUse</code>
        /// </summary>
        /// <param name="nodes">An <code>XmlNodeList</code> containing nodes to create <code>CheckBox</code> objects.</param>
        /// <param name="entityTree">The name of the XML entity with the data for the <code>CheckBox</code> objects.</param>
        /// <param name="nameEntity">The XML entity within <code>entityToUse</code> that contains text for the <code>CheckBox</code>.</param>
        /// <param name="attributeToolip">The XML attribute of the entity <code>entityToUse</code> that determines the tooltip.</param>
        /// <returns>A <code>List&lt;CheckBox&gt;</code> of <code>CheckBox</code> elements with names given in the XML entities named <code>entityWithName</code> and tooltips <code>attributeTooltip</code>.</returns>
        public static List<CheckBox> XML_to_CheckBox(XmlDocument document, List<string> entityTree, string nameEntity, string attributeToolip)
        {
            List<CheckBox> list = new List<CheckBox>();
            XmlNodeList nodes = document.ChildNodes;

            for (int i = 0; i < entityTree.Count; i++)
            {
                string currentEntity = entityTree[i];

                foreach (XmlNode nodeChild in nodes)
                {
                    // If names are equal AND this is the final entity
                    if (currentEntity.ToLower() == nodeChild.Name.ToLower() && i == entityTree.Count - 1)
                    {
                        CheckBox newBox = new CheckBox();
                        for (int j = 0; j < nodeChild.ChildNodes.Count; j++)
                        {
                            if (nodeChild.ChildNodes.Item(j).Name.ToLower() == nameEntity.ToLower())
                            {
                                newBox.Content = nodeChild.ChildNodes.Item(j).InnerText;
                            }
                        }
                        newBox.ToolTip = nodeChild.Attributes[attributeToolip].InnerText;
                        list.Add(newBox);
                    }
                    // If names are equal
                    else if (currentEntity.ToLower() == nodeChild.Name.ToLower())
                    {
                        nodes = nodeChild.ChildNodes;
                    }
                }
            }
            if (list.Count != 0)
            {
                return list;
            }
            return null;
        }

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
