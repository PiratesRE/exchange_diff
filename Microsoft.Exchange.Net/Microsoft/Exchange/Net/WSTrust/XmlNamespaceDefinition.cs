using System;
using System.Xml;

namespace Microsoft.Exchange.Net.WSTrust
{
	internal class XmlNamespaceDefinition
	{
		internal XmlNamespaceDefinition(string prefix, string namespaceURI)
		{
			this.prefix = prefix;
			this.namespaceURI = namespaceURI;
		}

		internal string Prefix
		{
			get
			{
				return this.prefix;
			}
		}

		internal string NamespaceURI
		{
			get
			{
				return this.namespaceURI;
			}
		}

		internal static void AddPrefixes(XmlDocument xmlDocument, XmlElement xmlElement, params XmlNamespaceDefinition[] xmlNamespaceDefinitions)
		{
			foreach (XmlNamespaceDefinition xmlNamespaceDefinition in xmlNamespaceDefinitions)
			{
				if (xmlNamespaceDefinition.NamespaceURI != null && xmlNamespaceDefinition.Prefix != null)
				{
					XmlAttribute xmlAttribute = xmlDocument.CreateAttribute("xmlns", xmlNamespaceDefinition.Prefix, "http://www.w3.org/2000/xmlns/");
					xmlAttribute.Value = xmlNamespaceDefinition.NamespaceURI;
					xmlElement.Attributes.Append(xmlAttribute);
				}
			}
		}

		internal void WriteAttribute(XmlWriter writer)
		{
			writer.WriteAttributeString("xmlns", this.prefix, null, this.namespaceURI);
		}

		private const string XmlNamespacePrefix = "xmlns";

		private const string XmlNamespaceURI = "http://www.w3.org/2000/xmlns/";

		private string prefix;

		private string namespaceURI;
	}
}
