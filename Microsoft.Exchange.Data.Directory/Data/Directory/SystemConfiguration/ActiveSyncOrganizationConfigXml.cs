using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[XmlType(TypeName = "ActiveSyncOrganizationConfig")]
	[Serializable]
	public class ActiveSyncOrganizationConfigXml : XMLSerializableBase
	{
		[XmlElement("DeviceFiltering")]
		public ActiveSyncDeviceFilterArray DeviceFiltering { get; set; }
	}
}
