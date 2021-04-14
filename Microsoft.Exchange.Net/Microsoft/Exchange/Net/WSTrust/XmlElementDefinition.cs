using System;
using System.Collections.Generic;
using System.Xml;

namespace Microsoft.Exchange.Net.WSTrust
{
	internal sealed class XmlElementDefinition : XmlNodeDefinition
	{
		internal XmlElementDefinition(string localName, XmlNamespaceDefinition xmlNamespaceDefinition) : base(localName, xmlNamespaceDefinition)
		{
		}

		public XmlElement GetSingleElementByName(XmlElement parent)
		{
			XmlElement xmlElement = null;
			foreach (object obj in parent.ChildNodes)
			{
				XmlElement xmlElement2 = (XmlElement)obj;
				if (base.IsMatch(xmlElement2))
				{
					if (xmlElement != null)
					{
						return null;
					}
					xmlElement = xmlElement2;
				}
			}
			return xmlElement;
		}

		public XmlElement CreateElement(XmlDocument xmlDocument, IEnumerable<XmlAttribute> attributes)
		{
			return this.CreateElement(xmlDocument, attributes, null, null);
		}

		public XmlElement CreateElement(XmlDocument xmlDocument, IEnumerable<XmlAttribute> attributes, string innerText)
		{
			return this.CreateElement(xmlDocument, attributes, innerText, null);
		}

		public XmlElement CreateElement(XmlDocument xmlDocument, IEnumerable<XmlAttribute> attributes, IEnumerable<XmlElement> childELements)
		{
			return this.CreateElement(xmlDocument, attributes, null, childELements);
		}

		public XmlElement CreateElement(XmlDocument xmlDocument, string innerText)
		{
			return this.CreateElement(xmlDocument, null, innerText, null);
		}

		public XmlElement CreateElement(XmlDocument xmlDocument, IEnumerable<XmlElement> childELements)
		{
			return this.CreateElement(xmlDocument, null, null, childELements);
		}

		private XmlElement CreateElement(XmlDocument xmlDocument, IEnumerable<XmlAttribute> attributes, string innerText, IEnumerable<XmlElement> childELements)
		{
			XmlElement xmlElement = xmlDocument.CreateElement(base.XmlNamespaceDefinition.Prefix, base.LocalName, base.XmlNamespaceDefinition.NamespaceURI);
			xmlElement.InnerText = innerText;
			if (childELements != null)
			{
				foreach (XmlElement newChild in childELements)
				{
					xmlElement.AppendChild(newChild);
				}
			}
			if (attributes != null)
			{
				foreach (XmlAttribute node in attributes)
				{
					xmlElement.Attributes.Append(node);
				}
			}
			return xmlElement;
		}
	}
}
