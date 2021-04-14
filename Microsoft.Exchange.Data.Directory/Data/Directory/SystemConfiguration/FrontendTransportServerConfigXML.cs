using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[XmlType(TypeName = "FrontendTransportServerConfigXml")]
	[Serializable]
	public sealed class FrontendTransportServerConfigXML : XMLSerializableBase
	{
		public FrontendTransportServerConfigXML()
		{
			this.AgentLog = new LogConfigXML();
			this.AttributionLog = new LogConfigXML();
			this.DnsLog = new LogConfigXML();
			this.ResourceLog = new LogConfigXML();
		}

		[XmlElement(ElementName = "AgentLog")]
		public LogConfigXML AgentLog { get; set; }

		[XmlElement(ElementName = "DnsLog")]
		public LogConfigXML DnsLog { get; set; }

		[XmlElement(ElementName = "ResourceLog")]
		public LogConfigXML ResourceLog { get; set; }

		[XmlElement(ElementName = "AttributionLog")]
		public LogConfigXML AttributionLog { get; set; }

		[XmlElement(ElementName = "MaxReceiveTlsRatePerMinute")]
		public int MaxReceiveTlsRatePerMinute { get; set; }
	}
}
