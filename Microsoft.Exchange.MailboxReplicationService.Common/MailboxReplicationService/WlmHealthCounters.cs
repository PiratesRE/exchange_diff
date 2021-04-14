using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Serializable]
	public sealed class WlmHealthCounters : XMLSerializableBase
	{
		[XmlAttribute(AttributeName = "UnderloadedCounter")]
		public uint UnderloadedCounter { get; set; }

		[XmlAttribute(AttributeName = "FullCounter")]
		public uint FullCounter { get; set; }

		[XmlAttribute(AttributeName = "OverloadedCounter")]
		public uint OverloadedCounter { get; set; }

		[XmlAttribute(AttributeName = "CriticalCounter")]
		public uint CriticalCounter { get; set; }

		[XmlAttribute(AttributeName = "UnknownCounter")]
		public uint UnknownCounter { get; set; }
	}
}
