using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[XmlType(TypeName = "OutlookProviderConfig")]
	[Serializable]
	public sealed class OutlookProviderConfigXML : XMLSerializableBase
	{
		[XmlElement(ElementName = "RequiredClientVersions")]
		public ClientVersionCollection RequiredClientVersions { get; set; }
	}
}
