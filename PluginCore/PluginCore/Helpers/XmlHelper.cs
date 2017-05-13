using System;
using System.Xml;
using PluginCore.Managers;

namespace PluginCore.Helpers
{
    public static class XmlHelper
    {
        /// <summary>
        /// Gets the value of the specified XmlNode.
        /// </summary>
        public static String GetValue(XmlNode node)
        {
            return node?.FirstChild?.Value;
        }

        /// <summary>
        /// Gets the specified attribute from the specified XmlNode.
        /// </summary>
        public static String GetAttribute(XmlNode node, String attName)
        {
            return node?.Attributes[attName]?.Value;
        }

        /// <summary>
        /// Checks that if the XmlNode has a value.
        /// </summary>
        public static Boolean HasValue(XmlNode node)
        {
            return (node != null && node.FirstChild != null && node.FirstChild.Value != null);
        }

        /// <summary>
        /// Checks if the XmlNode has the specified attribute.
        /// </summary>
        public static Boolean HasAttribute(XmlNode node, String attName)
        {
            return (node != null && node.Attributes[attName] != null);
        }

        /// <summary>
        /// Reads a xml file and returns it as a XmlNode. Returns null on failure.
        /// </summary>
        public static XmlNode LoadXmlDocument(String file)
        {
            try
            {
                XmlDocument document = new XmlDocument();
                document.PreserveWhitespace = false;
                document.Load(file);
                try
                {
                    XmlNode declNode = document.FirstChild;
                    XmlNode rootNode = declNode.NextSibling;
                    return rootNode;
                }
                catch (Exception ex1)
                {
                    ErrorManager.ShowError(ex1);
                    return null;
                }
            }
            catch (Exception ex2)
            {
                ErrorManager.ShowError(ex2);
                return null;
            }
        }

    }

}
