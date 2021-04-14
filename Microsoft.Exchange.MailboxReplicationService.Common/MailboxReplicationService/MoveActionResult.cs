using System;
using System.Runtime.Serialization;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal sealed class MoveActionResult : ReplayActionResult
	{
		public MoveActionResult()
		{
		}

		[DataMember]
		public byte[] ItemId { get; set; }

		[DataMember]
		public byte[] PreviousItemId { get; set; }

		[DataMember]
		public bool MoveAsDelete { get; set; }

		public MoveActionResult(byte[] itemId, byte[] prevItemId, bool moveAsDelete = false)
		{
			this.ItemId = itemId;
			this.PreviousItemId = prevItemId;
			this.MoveAsDelete = moveAsDelete;
		}

		public override string ToString()
		{
			return string.Concat(new string[]
			{
				base.ToString(),
				", ItemId: ",
				TraceUtils.DumpEntryId(this.ItemId),
				", PreviousItemId: ",
				TraceUtils.DumpEntryId(this.PreviousItemId)
			});
		}
	}
}
