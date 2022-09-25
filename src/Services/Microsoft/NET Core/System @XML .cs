using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace DxMLEngine.Services.NET
{
    internal class dotnetXML
    {
        public static XmlDocument ReadXML(string path)
        {
            if (!Path.IsPathFullyQualified(path))
                throw new ArgumentNullException("path is not fully qualified");

            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path is null or empty");
            
            var document = new XmlDocument();
            try
            {
                document.Load(path);
            }
            catch (Exception)
            {
                throw;
            }

            return document;
        }

        public static void WriteXML(string path, string document)
        {
            throw new NotImplementedException();
        }

        public static XmlNode? SelectRootNode(XmlDocument document)
        {
            return document.DocumentElement;
        }

        public static XmlNode? SelectFirstNode(XmlDocument document)
        {
            return document.FirstChild;
        }

        public static XmlNode? SelectLastNode(XmlDocument document)
        {
            return document.LastChild;
        }

        public static XmlNode? SelectNode(XmlDocument document, string xpath)
        {
            return document.SelectSingleNode(xpath);
        }

        public static XmlNodeList? SelectNodes(XmlDocument document, string xpath)
        {
            return document.SelectNodes(xpath);
        }
        
    }
}
