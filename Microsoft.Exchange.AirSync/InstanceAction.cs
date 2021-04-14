using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.AirSync
{
	public struct InstanceAction
	{
		[XmlAttribute("Date")]
		public DateTime Time { get; set; }

		[XmlAttribute]
		public string Action { get; set; }

		[XmlAttribute("TID")]
		public int ThreadId { get; set; }

		[XmlAttribute]
		public string XsoEventType { get; set; }

		[XmlAttribute]
		public string XsoObjectType { get; set; }

		[XmlAttribute]
		public string XsoException { get; set; }
	}
}
