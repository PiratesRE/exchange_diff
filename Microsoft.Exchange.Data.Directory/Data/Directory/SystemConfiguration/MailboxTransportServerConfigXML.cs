using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[XmlType(TypeName = "MailboxTransportServerConfigXml")]
	[Serializable]
	public sealed class MailboxTransportServerConfigXML : XMLSerializableBase
	{
		public MailboxTransportServerConfigXML()
		{
			this.MailboxSubmissionAgentLog = new LogConfigXML();
			this.MailboxDeliveryAgentLog = new LogConfigXML();
			this.MailboxDeliveryThrottlingLog = new LogConfigXML();
		}

		[XmlElement(ElementName = "MailboxSubmissionAgentLog")]
		public LogConfigXML MailboxSubmissionAgentLog { get; set; }

		[XmlElement(ElementName = "MailboxDeliveryAgentLog")]
		public LogConfigXML MailboxDeliveryAgentLog { get; set; }

		[XmlElement(ElementName = "MailboxDeliveryThrottlingLog")]
		public LogConfigXML MailboxDeliveryThrottlingLog { get; set; }
	}
}
