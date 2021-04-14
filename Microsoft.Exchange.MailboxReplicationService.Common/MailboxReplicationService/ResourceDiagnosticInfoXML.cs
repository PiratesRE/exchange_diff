using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[XmlType(TypeName = "Resource")]
	public class ResourceDiagnosticInfoXML : XMLSerializableBase
	{
		[XmlAttribute(AttributeName = "Name")]
		public string ResourceName { get; set; }

		[XmlAttribute(AttributeName = "Guid")]
		public Guid ResourceGuid { get; set; }

		[XmlAttribute(AttributeName = "Type")]
		public string ResourceType { get; set; }

		[XmlAttribute(AttributeName = "StaticCapacity")]
		public int StaticCapacity { get; set; }

		[XmlAttribute(AttributeName = "DynamicCapacity")]
		public int DynamicCapacity { get; set; }

		[XmlAttribute(AttributeName = "Utilization")]
		public int Utilization { get; set; }

		[XmlAttribute(AttributeName = "IsUnhealthy")]
		public bool IsUnhealthy { get; set; }

		[XmlAttribute(AttributeName = "WlmWorkloadType")]
		public string WlmWorkloadType { get; set; }

		[XmlAttribute(AttributeName = "BytesPerMin")]
		public uint TransferRatePerMin { get; set; }

		[XmlArray("WlmResources")]
		public List<WlmResourceHealthMonitorDiagnosticInfoXML> WlmResourceHealthMonitors { get; set; }
	}
}
