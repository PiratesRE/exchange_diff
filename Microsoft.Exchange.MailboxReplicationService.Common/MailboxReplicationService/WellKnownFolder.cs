using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal sealed class WellKnownFolder
	{
		public WellKnownFolder(int wkfType, byte[] entryId)
		{
			this.WKFType = wkfType;
			this.EntryId = entryId;
		}

		[DataMember]
		public byte[] EntryId { get; set; }

		[DataMember]
		public int WKFType { get; set; }
	}
}
