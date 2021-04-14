using System;
using System.Security;
using System.Xml;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.WBXml
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class WBXmlSecureStringNode : XmlElement
	{
		internal WBXmlSecureStringNode(string prefix, string localName, string namespaceUri, XmlDocument doc) : base(prefix, localName, namespaceUri, doc)
		{
		}

		internal SecureString SecureData
		{
			set
			{
				this.secureData = value;
				this.InnerText = "******";
			}
		}

		internal SecureString DetachSecureData()
		{
			SecureString result = this.secureData;
			this.secureData = null;
			return result;
		}

		private SecureString secureData;
	}
}
