using System;
using System.Runtime.Serialization;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal class MoveAction : ReplayAction
	{
		public MoveAction()
		{
		}

		[DataMember]
		public byte[] PreviousFolderId { get; set; }

		public MoveAction(byte[] itemId, byte[] folderId, byte[] prevFolderId, string watermark) : base(watermark)
		{
			base.ItemId = itemId;
			base.FolderId = folderId;
			this.PreviousFolderId = prevFolderId;
		}

		internal override ActionId Id
		{
			get
			{
				return ActionId.Move;
			}
		}

		public override string ToString()
		{
			return string.Concat(new string[]
			{
				base.ToString(),
				", EntryId: ",
				TraceUtils.DumpEntryId(base.ItemId),
				", FolderId: ",
				TraceUtils.DumpEntryId(base.FolderId),
				", PreviousFolderId: ",
				TraceUtils.DumpEntryId(this.PreviousFolderId)
			});
		}

		internal override void TranslateEntryIds(IEntryIdTranslator translator)
		{
			base.TranslateEntryIds(translator);
			byte[] previousFolderId = this.PreviousFolderId;
			byte[] sourceFolderIdFromTargetFolderId = translator.GetSourceFolderIdFromTargetFolderId(previousFolderId);
			if (sourceFolderIdFromTargetFolderId == null)
			{
				MrsTracer.Common.Warning("Previous destination folder {0} doesn't have mapped source folder for action {1}", new object[]
				{
					TraceUtils.DumpEntryId(previousFolderId),
					this
				});
				base.Ignored = true;
			}
			this.PreviousFolderId = sourceFolderIdFromTargetFolderId;
		}
	}
}
