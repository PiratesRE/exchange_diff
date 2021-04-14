using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Storage
{
	public class GetSyncStateResult
	{
		[XmlAttribute]
		public bool LoggingEnabled { get; set; }

		public List<DeviceData> Devices { get; set; }
	}
}
