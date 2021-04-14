using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ContactFolders
	{
		internal ContactFolders(MyContactFolders myContactFolders, StoreObjectId myContactsSearchFolderId, StoreObjectId quickContactsFolderId, StoreObjectId favoritesFolderId)
		{
			this.myContactFolders = myContactFolders;
			this.myContactsSearchFolderId = myContactsSearchFolderId;
			this.quickContactsFolderId = quickContactsFolderId;
			this.favoritesFolderId = favoritesFolderId;
		}

		internal static ContactFolders Load(IXSOFactory xsoFactory, IMailboxSession mailboxSession)
		{
			MyContactFolders myContactFolders = new MyContactFolders(xsoFactory, mailboxSession);
			StoreObjectId defaultFolderId = mailboxSession.GetDefaultFolderId(DefaultFolderType.MyContacts);
			StoreObjectId defaultFolderId2 = mailboxSession.GetDefaultFolderId(DefaultFolderType.QuickContacts);
			StoreObjectId defaultFolderId3 = mailboxSession.GetDefaultFolderId(DefaultFolderType.Favorites);
			return new ContactFolders(myContactFolders, defaultFolderId, defaultFolderId2, defaultFolderId3);
		}

		public MyContactFolders MyContactFolders
		{
			get
			{
				return this.myContactFolders;
			}
		}

		public StoreObjectId MyContactsSearchFolderId
		{
			get
			{
				return this.myContactsSearchFolderId;
			}
		}

		public StoreObjectId QuickContactsFolderId
		{
			get
			{
				return this.quickContactsFolderId;
			}
		}

		public StoreObjectId FavoritesFolderId
		{
			get
			{
				return this.favoritesFolderId;
			}
		}

		private readonly MyContactFolders myContactFolders;

		private readonly StoreObjectId myContactsSearchFolderId;

		private readonly StoreObjectId quickContactsFolderId;

		private readonly StoreObjectId favoritesFolderId;
	}
}
