using System;
using System.Xml;

namespace Microsoft.Exchange.Net.WSTrust
{
	internal sealed class XmlAttributeDefinition : XmlNodeDefinition
	{
		internal XmlAttributeDefinition(string localName, XmlNamespaceDefinition xmlNamespaceDefinition) : base(localName, xmlNamespaceDefinition)
		{
		}

		public XmlAttribute CreateAttribute(XmlDocument xmlDocument, string value)
		{
			XmlAttribute xmlAttribute = this.CreateAttribute(xmlDocument);
			xmlAttribute.Value = value;
			return xmlAttribute;
		}

		public string GetAttributeValue(XmlElement xmlElement)
		{
			return xmlElement.GetAttribute(base.LocalName, base.XmlNamespaceDefinition.NamespaceURI);
		}

		private XmlAttribute CreateAttribute(XmlDocument xmlDocument)
		{
			return xmlDocument.CreateAttribute(base.XmlNamespaceDefinition.Prefix, base.LocalName, base.XmlNamespaceDefinition.NamespaceURI);
		}
	}
}
