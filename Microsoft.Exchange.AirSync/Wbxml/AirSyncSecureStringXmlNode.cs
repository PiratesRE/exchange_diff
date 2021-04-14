using System;
using System.Security;
using System.Xml;

namespace Microsoft.Exchange.AirSync.Wbxml
{
	internal class AirSyncSecureStringXmlNode : XmlElement
	{
		public AirSyncSecureStringXmlNode(string prefix, string localName, string namespaceURI, XmlDocument doc) : base(prefix, localName, namespaceURI, doc)
		{
		}

		public SecureString SecureData
		{
			set
			{
				this.secureData = value;
				this.InnerText = "******";
			}
		}

		public SecureString DetachSecureData()
		{
			SecureString result = this.secureData;
			this.secureData = null;
			return result;
		}

		private SecureString secureData;
	}
}
