using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[XmlType(TypeName = "TransportServerConfigXml")]
	[Serializable]
	public sealed class ServerConfigXML : XMLSerializableBase
	{
		public ServerConfigXML()
		{
			this.QueueLog = new LogConfigXML();
			this.WlmLog = new LogConfigXML();
			this.AgentLog = new LogConfigXML();
			this.FlowControlLog = new LogConfigXML();
			this.ResourceLog = new LogConfigXML();
			this.ProcessingSchedulerLog = new LogConfigXML();
			this.DnsLog = new LogConfigXML();
			this.JournalLog = new LogConfigXML();
			this.TransportMaintenanceLog = new LogConfigXML();
			this.MaximumPreferredActiveDatabases = null;
		}

		[XmlElement(ElementName = "QueueLog")]
		public LogConfigXML QueueLog { get; set; }

		[XmlElement(ElementName = "WlmLog")]
		public LogConfigXML WlmLog { get; set; }

		[XmlElement(ElementName = "AgentLog")]
		public LogConfigXML AgentLog { get; set; }

		[XmlElement(ElementName = "FlowControlLog")]
		public LogConfigXML FlowControlLog { get; set; }

		[XmlElement(ElementName = "ResourceLog")]
		public LogConfigXML ResourceLog { get; set; }

		[XmlElement(ElementName = "ProcessingSchedulerLog")]
		public LogConfigXML ProcessingSchedulerLog { get; set; }

		[XmlElement(ElementName = "DnsLog")]
		public LogConfigXML DnsLog { get; set; }

		[XmlElement(ElementName = "JournalLog")]
		public LogConfigXML JournalLog { get; set; }

		[XmlElement(ElementName = "MaintenanceLog")]
		public LogConfigXML TransportMaintenanceLog { get; set; }

		[XmlElement(ElementName = "MailboxProvisioningAttributes")]
		public MailboxProvisioningAttributes MailboxProvisioningAttributes { get; set; }

		[XmlElement(ElementName = "MaxPrefActives")]
		public int? MaximumPreferredActiveDatabases { get; set; }
	}
}
