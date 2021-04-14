using System;
using System.Runtime.Serialization;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal sealed class FlagCompleteAction : ReplayAction
	{
		public FlagCompleteAction()
		{
		}

		public FlagCompleteAction(byte[] itemId, byte[] folderId, string watermark) : base(watermark)
		{
			base.ItemId = itemId;
			base.FolderId = folderId;
		}

		internal override ActionId Id
		{
			get
			{
				return ActionId.FlagComplete;
			}
		}

		public override string ToString()
		{
			return base.ToString() + ", EntryId: " + TraceUtils.DumpEntryId(base.ItemId);
		}
	}
}
