using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ShortcutMessage : MessageItem
	{
		public string FavoriteDisplayAlias
		{
			get
			{
				this.CheckDisposed("FavoriteDisplayAlias::get");
				return base.GetValueOrDefault<string>(ShortcutMessageSchema.FavoriteDisplayAlias);
			}
			set
			{
				this.CheckDisposed("FavoriteDisplayAlias::set");
				this[ShortcutMessageSchema.FavoriteDisplayAlias] = value;
			}
		}

		public string FavoriteDisplayName
		{
			get
			{
				this.CheckDisposed("FavoriteDisplayName::get");
				return base.GetValueOrDefault<string>(ShortcutMessageSchema.FavoriteDisplayName);
			}
			set
			{
				this.CheckDisposed("FavoriteDisplayName::set");
				this[ShortcutMessageSchema.FavoriteDisplayName] = value;
			}
		}

		public byte[] FavPublicSourceKey
		{
			get
			{
				this.CheckDisposed("FavPublicSourceKey::get");
				return base.GetValueOrDefault<byte[]>(ShortcutMessageSchema.FavPublicSourceKey);
			}
			set
			{
				this.CheckDisposed("FavPublicSourceKey::set");
				this[ShortcutMessageSchema.FavPublicSourceKey] = value;
			}
		}

		internal ShortcutMessage(ICoreItem coreItem) : base(coreItem, false)
		{
			if (base.IsNew)
			{
				this.InitializeNewShortcutMessage();
			}
		}

		public static ShortcutMessage Create(MailboxSession session, byte[] longTermFolderId, string folderName)
		{
			Util.ThrowOnNullArgument(session, "session");
			Util.ThrowOnNullArgument(longTermFolderId, "longTermFolderId");
			ShortcutMessage shortcutMessage = ItemBuilder.CreateNewItem<ShortcutMessage>(session, session.GetDefaultFolderId(DefaultFolderType.LegacyShortcuts), ItemCreateInfo.ShortcutMessageInfo, CreateMessageType.Normal);
			shortcutMessage.FavoriteDisplayName = folderName;
			shortcutMessage.FavPublicSourceKey = longTermFolderId;
			return shortcutMessage;
		}

		public static ShortcutMessage Bind(MailboxSession session, StoreId storeId)
		{
			return ShortcutMessage.Bind(session, storeId, null);
		}

		public static ShortcutMessage Bind(MailboxSession session, StoreId storeId, PropertyDefinition[] propsToReturn)
		{
			Util.ThrowOnNullArgument(session, "session");
			Util.ThrowOnNullArgument(storeId, "storeId");
			return ItemBuilder.ItemBind<ShortcutMessage>(session, storeId, ShortcutMessageSchema.Instance, propsToReturn);
		}

		public override Schema Schema
		{
			get
			{
				this.CheckDisposed("Schema::get");
				return ShortcutMessageSchema.Instance;
			}
		}

		private void InitializeNewShortcutMessage()
		{
			this[ItemSchema.FavLevelMask] = 1;
			this[InternalSchema.ItemClass] = "IPM.Note";
		}
	}
}
