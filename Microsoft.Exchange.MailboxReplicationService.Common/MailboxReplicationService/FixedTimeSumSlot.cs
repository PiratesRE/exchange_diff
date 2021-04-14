using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	[Serializable]
	public class FixedTimeSumSlot : XMLSerializableBase
	{
		[XmlAttribute(AttributeName = "Value")]
		[DataMember(Name = "Value")]
		[CLSCompliant(false)]
		public uint Value { get; set; }

		[DataMember(Name = "Ticks")]
		[XmlAttribute(AttributeName = "Ticks")]
		public long StartTimeInTicks { get; set; }
	}
}
