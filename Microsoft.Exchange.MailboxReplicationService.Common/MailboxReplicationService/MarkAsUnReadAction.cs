using System;
using System.Runtime.Serialization;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal sealed class MarkAsUnReadAction : ReplayAction
	{
		public MarkAsUnReadAction()
		{
		}

		public MarkAsUnReadAction(byte[] itemId, byte[] folderId, string watermark) : base(watermark)
		{
			base.ItemId = itemId;
			base.FolderId = folderId;
		}

		internal override ActionId Id
		{
			get
			{
				return ActionId.MarkAsUnRead;
			}
		}

		public override string ToString()
		{
			return base.ToString() + ", EntryId: " + TraceUtils.DumpEntryId(base.ItemId);
		}
	}
}
