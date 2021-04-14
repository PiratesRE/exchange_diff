using System;
using System.Runtime.Serialization;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal sealed class CreateCalendarEventActionResult : ReplayActionResult
	{
		public CreateCalendarEventActionResult()
		{
		}

		[DataMember]
		public byte[] SourceItemId { get; set; }

		public CreateCalendarEventActionResult(byte[] sourceItemId)
		{
			this.SourceItemId = sourceItemId;
		}

		public override string ToString()
		{
			return base.ToString() + ", RemoteItemId: " + TraceUtils.DumpEntryId(this.SourceItemId);
		}
	}
}
