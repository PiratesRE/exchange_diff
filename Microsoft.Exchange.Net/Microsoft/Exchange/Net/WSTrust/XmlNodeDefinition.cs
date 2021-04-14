using System;
using System.Xml;

namespace Microsoft.Exchange.Net.WSTrust
{
	internal abstract class XmlNodeDefinition
	{
		protected XmlNodeDefinition(string localName, XmlNamespaceDefinition xmlNamespaceDefinition)
		{
			this.localName = localName;
			this.xmlNamespaceDefinition = xmlNamespaceDefinition;
		}

		public bool IsMatch(XmlNode node)
		{
			return StringComparer.OrdinalIgnoreCase.Equals(node.LocalName, this.localName) && StringComparer.OrdinalIgnoreCase.Equals(node.NamespaceURI, this.xmlNamespaceDefinition.NamespaceURI);
		}

		protected string LocalName
		{
			get
			{
				return this.localName;
			}
		}

		protected XmlNamespaceDefinition XmlNamespaceDefinition
		{
			get
			{
				return this.xmlNamespaceDefinition;
			}
		}

		public override string ToString()
		{
			if (this.xmlNamespaceDefinition.NamespaceURI == null)
			{
				return "<" + this.localName + ">";
			}
			return string.Concat(new string[]
			{
				"<",
				this.localName,
				" ",
				this.xmlNamespaceDefinition.Prefix,
				":xmlns=\"",
				this.xmlNamespaceDefinition.NamespaceURI,
				"\">"
			});
		}

		private string localName;

		private XmlNamespaceDefinition xmlNamespaceDefinition;
	}
}
