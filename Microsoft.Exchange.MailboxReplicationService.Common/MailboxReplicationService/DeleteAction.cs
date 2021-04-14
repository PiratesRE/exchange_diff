using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal sealed class DeleteAction : MoveAction
	{
		public DeleteAction()
		{
		}

		public DeleteAction(byte[] itemId, byte[] folderId, byte[] prevFolderId, string watermark) : base(itemId, folderId, prevFolderId, watermark)
		{
		}

		internal override ActionId Id
		{
			get
			{
				return ActionId.Delete;
			}
		}

		internal override void TranslateEntryIds(IEntryIdTranslator translator)
		{
			base.TranslateEntryIds(translator);
			if (base.ItemId != null && base.PreviousFolderId != null)
			{
				base.Ignored = false;
			}
		}
	}
}
