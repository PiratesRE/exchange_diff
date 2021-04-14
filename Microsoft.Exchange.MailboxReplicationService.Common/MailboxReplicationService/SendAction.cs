using System;
using System.Runtime.Serialization;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal sealed class SendAction : ReplayAction
	{
		public SendAction()
		{
		}

		[DataMember]
		public string[] Recipients { get; set; }

		[DataMember]
		public byte[] Data { get; set; }

		public SendAction(byte[] itemId, byte[] folderId, string watermark) : base(watermark)
		{
			base.ItemId = itemId;
			base.FolderId = folderId;
		}

		internal override ActionId Id
		{
			get
			{
				return ActionId.Send;
			}
		}

		public override string ToString()
		{
			return string.Concat(new object[]
			{
				base.ToString(),
				", EntryId: ",
				TraceUtils.DumpEntryId(base.ItemId),
				", Data: [len=",
				this.Data.Length,
				"], Recipients: ",
				string.Join(",", this.Recipients)
			});
		}

		internal override EntryIdMap<byte[]> GetMessageEntryIdsToTranslate()
		{
			return SendAction.EmptyEntryIdMap;
		}

		internal override void TranslateEntryIds(IEntryIdTranslator translator)
		{
			base.OriginalItemId = base.ItemId;
			base.OriginalFolderId = base.FolderId;
			base.ItemId = null;
			base.FolderId = null;
		}

		private static readonly EntryIdMap<byte[]> EmptyEntryIdMap = new EntryIdMap<byte[]>(0);
	}
}
