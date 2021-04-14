using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class IsReadProperty : SimpleProperty, IPostSavePropertyCommand
	{
		public IsReadProperty(CommandContext commandContext) : base(commandContext)
		{
		}

		public new static IsReadProperty CreateCommand(CommandContext commandContext)
		{
			return new IsReadProperty(commandContext);
		}

		public override void SetUpdate(SetPropertyUpdate setPropertyUpdate, UpdateCommandSettings updateCommandSettings)
		{
			this.isRead = setPropertyUpdate.ServiceObject.GetValueOrDefault<bool>(this.commandContext.PropertyInformation);
			this.suppressReadReceipts = updateCommandSettings.SuppressReadReceipts;
			EWSSettings.PostSavePropertyCommands.Add(updateCommandSettings.StoreObject.StoreObjectId, this);
		}

		public void ExecutePostSaveOperation(StoreObject item)
		{
			StoreObjectId parentId = item.ParentId;
			MailboxSession mailboxSession = item.Session as MailboxSession;
			bool flag = this.suppressReadReceipts || (mailboxSession != null && mailboxSession.IsDefaultFolderType(parentId) == DefaultFolderType.JunkEmail);
			if (this.isRead)
			{
				item.Session.MarkAsRead(flag, new StoreId[]
				{
					item.StoreObjectId
				});
				return;
			}
			item.Session.MarkAsUnread(flag, new StoreId[]
			{
				item.StoreObjectId
			});
		}

		private bool isRead;

		private bool suppressReadReceipts;
	}
}
