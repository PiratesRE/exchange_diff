using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[XmlType(TypeName = "PartnerApplicationConfig")]
	[Serializable]
	public sealed class PartnerApplicationConfigXML : XMLSerializableBase
	{
		[XmlElement(ElementName = "IssuerIdentifier")]
		public string IssuerIdentifier { get; set; }

		[XmlElement(ElementName = "AppOnlyPermissions")]
		public string[] AppOnlyPermissions { get; set; }

		[XmlElement(ElementName = "ActAsPermissions")]
		public string[] ActAsPermissions { get; set; }
	}
}
