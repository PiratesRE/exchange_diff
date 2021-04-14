using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Serializable]
	public class TimeSlotXML
	{
		[XmlAttribute(AttributeName = "StartTime")]
		public string StartTime { get; set; }

		[CLSCompliant(false)]
		[XmlAttribute(AttributeName = "Value")]
		public ulong Value { get; set; }
	}
}
