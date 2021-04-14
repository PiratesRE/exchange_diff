using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal sealed class MSInternal2
	{
		[DataMember]
		public long Field1 { get; set; }

		[DataMember]
		public byte Field2 { get; set; }

		[DataMember]
		public MSInternal3[] Field3 { get; set; }
	}
}
