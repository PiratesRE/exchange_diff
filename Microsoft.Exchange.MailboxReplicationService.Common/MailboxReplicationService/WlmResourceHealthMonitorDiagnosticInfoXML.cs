using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[XmlType(TypeName = "WlmResourceHealthMonitor")]
	public class WlmResourceHealthMonitorDiagnosticInfoXML : XMLSerializableBase
	{
		[XmlAttribute(AttributeName = "Key")]
		public string WlmResourceKey { get; set; }

		[XmlAttribute(AttributeName = "State")]
		public string LoadState { get; set; }

		[XmlAttribute(AttributeName = "Load")]
		public double LoadRatio { get; set; }

		[XmlAttribute(AttributeName = "Metric")]
		public string Metric { get; set; }

		[XmlAttribute(AttributeName = "DynamicCapacity")]
		public double DynamicCapacity { get; set; }

		[XmlAttribute(AttributeName = "IsDisabled")]
		public string IsDisabled { get; set; }

		[XmlAttribute(AttributeName = "DynamicThrottlingDisabled")]
		public string DynamicThrottingDisabled { get; set; }

		[XmlElement(ElementName = "HealthStats")]
		public WlmHealthStatistics WlmHealthStatistics { get; set; }
	}
}
