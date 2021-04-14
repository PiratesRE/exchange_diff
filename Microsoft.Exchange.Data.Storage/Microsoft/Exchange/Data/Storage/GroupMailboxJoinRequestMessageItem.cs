using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class GroupMailboxJoinRequestMessageItem : MessageItem
	{
		internal GroupMailboxJoinRequestMessageItem(ICoreItem coreItem) : base(coreItem, false)
		{
			if (base.IsNew)
			{
				this[InternalSchema.ItemClass] = "IPM.GroupMailbox.JoinRequest";
			}
		}

		public string GroupSmtpAddress
		{
			get
			{
				this.CheckDisposed("GroupSmtpAddress::get");
				return base.GetValueOrDefault<string>(GroupMailboxJoinRequestMessageSchema.GroupSmtpAddress);
			}
			set
			{
				this.CheckDisposed("GroupSmtpAddress::set");
				this[GroupMailboxJoinRequestMessageSchema.GroupSmtpAddress] = value;
			}
		}

		public new static GroupMailboxJoinRequestMessageItem Bind(StoreSession session, StoreId messageId)
		{
			return GroupMailboxJoinRequestMessageItem.Bind(session, messageId, null);
		}

		public static GroupMailboxJoinRequestMessageItem Create(MailboxSession mailboxSession, StoreObjectId folderId)
		{
			return ItemBuilder.CreateNewItem<GroupMailboxJoinRequestMessageItem>(mailboxSession, folderId, ItemCreateInfo.GroupMailboxJoinRequestMessageInfo);
		}

		public new static GroupMailboxJoinRequestMessageItem Bind(StoreSession session, StoreId messageId, ICollection<PropertyDefinition> propsToReturn)
		{
			if (!(session is MailboxSession))
			{
				throw new ArgumentException("session");
			}
			return ItemBuilder.ItemBind<GroupMailboxJoinRequestMessageItem>(session, messageId, GroupMailboxJoinRequestMessageSchema.Instance, propsToReturn);
		}
	}
}
