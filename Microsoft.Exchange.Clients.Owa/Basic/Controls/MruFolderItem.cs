using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Basic.Controls
{
	internal class MruFolderItem
	{
		public MruFolderItem(StoreObjectId id, string displayName, int itemCount, int unreadCount, object extendedFolderFlags)
		{
			this.id = id;
			this.displayName = displayName;
			this.itemCount = itemCount;
			this.unreadCount = unreadCount;
			this.extendedFolderFlags = extendedFolderFlags;
		}

		public StoreObjectId Id
		{
			get
			{
				return this.id;
			}
		}

		public string DisplayName
		{
			get
			{
				return this.displayName;
			}
		}

		public int ItemCount
		{
			get
			{
				return this.itemCount;
			}
		}

		public int UnreadCount
		{
			get
			{
				return this.unreadCount;
			}
		}

		public object ExtendedFolderFlags
		{
			get
			{
				return this.extendedFolderFlags;
			}
		}

		private StoreObjectId id;

		private string displayName;

		private int itemCount;

		private int unreadCount;

		private object extendedFolderFlags;
	}
}
