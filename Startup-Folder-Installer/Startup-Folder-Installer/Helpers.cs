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
            /// <summary>
            /// Creates <code>CheckBox</code> objects using data in <code>nodes</code> for elements named <code>entityToUse</code>
            /// </summary>
            /// <param name="nodes">An <code>XmlNodeList</code> containing nodes to create <code>CheckBox</code> objects.</param>
            /// <param name="entityToUse">The name of the XML entity with the data for the <code>CheckBox</code> objects.</param>
            /// <param name="entityWithName">The XML entity within <code>entityToUse</code> that contains text for the <code>CheckBox</code>.</param>
            /// <param name="attributeToolip">The XML attribute of the entity <code>entityToUse</code> that determines the tooltip.</param>
            /// <returns>A <code>List&lt;CheckBox&gt;</code> of <code>CheckBox</code> elements with names given in the XML entities named <code>entityWithName</code> and tooltips <code>attributeTooltip</code>.</returns>
            public static List<CheckBox> XML_to_CheckBox(XmlNodeList nodes, string entityToUse, string entityWithName, string attributeToolip)
            {
                List<CheckBox> list = new List<CheckBox>();

                foreach (XmlNode node in nodes)
                {
                    foreach (XmlNode nodeChild in node.ChildNodes)
                    {
                        if (nodeChild.Name == entityToUse)
                        {
                            CheckBox newBox = new CheckBox();
                            for (int i = 0; i < nodeChild.ChildNodes.Count; i++)
                            {
                                if (nodeChild.ChildNodes.Item(i).Name == entityWithName)
                                {
                                    newBox.Content = nodeChild.ChildNodes.Item(i).InnerText;
                                }
                            }
                            newBox.ToolTip = nodeChild.Attributes[attributeToolip];
                            list.Add(newBox);
                        }
                    }
                }

                return list;
            }
        }
    }
}
