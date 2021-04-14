using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public class ActiveSyncDeviceFilterArray : XMLSerializableBase
	{
		[XmlArray("DeviceFilters")]
		[XmlArrayItem("DeviceFilter")]
		public List<ActiveSyncDeviceFilter> DeviceFilters { get; set; }
	}
}
