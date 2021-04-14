using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MailboxSyncProviderFactory : ISyncProviderFactory
	{
		public MailboxSyncProviderFactory(StoreSession storeSession, StoreObjectId folderId, bool allowTableRestrict)
		{
			this.storeSession = storeSession;
			this.folderId = folderId;
			this.allowTableRestrict = allowTableRestrict;
		}

		public MailboxSyncProviderFactory(StoreSession storeSession, bool allowTableRestrict)
		{
			this.storeSession = storeSession;
			this.folderId = null;
			this.allowTableRestrict = allowTableRestrict;
		}

		public MailboxSyncProviderFactory(StoreSession storeSession, StoreObjectId folderId)
		{
			this.storeSession = storeSession;
			this.folderId = folderId;
			this.allowTableRestrict = false;
		}

		public MailboxSyncProviderFactory(StoreSession storeSession)
		{
			this.storeSession = storeSession;
			this.folderId = null;
			this.allowTableRestrict = false;
		}

		public QueryFilter IcsPropertyGroupFilter { get; set; }

		public Folder Folder
		{
			get
			{
				return this.folder;
			}
			set
			{
				this.folder = value;
			}
		}

		public StoreObjectId FolderId
		{
			get
			{
				return this.folderId;
			}
			set
			{
				this.folderId = value;
			}
		}

		public StoreSession StoreSession
		{
			get
			{
				return this.storeSession;
			}
			set
			{
				this.storeSession = value;
			}
		}

		public void GenerateReadFlagChanges()
		{
			this.trackReadFlagChanges = true;
		}

		public void GenerateAssociatedMessageChanges()
		{
			this.trackAssociatedMessageChanges = true;
		}

		public void ReturnNewestChangesFirst()
		{
			this.returnNewestChangesFirst = true;
		}

		public void GenerateConversationChanges()
		{
			this.trackConversations = true;
		}

		public virtual ISyncProvider CreateSyncProvider(ISyncLogger syncLogger = null)
		{
			Folder folder;
			if (this.folder != null)
			{
				folder = this.folder;
				this.folder = null;
			}
			else
			{
				folder = Folder.Bind(this.storeSession, this.folderId);
			}
			return this.CreateSyncProvider(folder, this.trackReadFlagChanges, this.trackAssociatedMessageChanges, this.returnNewestChangesFirst, this.trackConversations, this.allowTableRestrict, true, syncLogger);
		}

		public virtual ISyncProvider CreateSyncProvider(Folder folder, ISyncLogger syncLogger = null)
		{
			return this.CreateSyncProvider(folder, this.trackReadFlagChanges, this.trackAssociatedMessageChanges, this.returnNewestChangesFirst, this.trackConversations, this.allowTableRestrict, false, syncLogger);
		}

		protected virtual ISyncProvider CreateSyncProvider(Folder folder, bool trackReadFlagChanges, bool trackAssociatedMessageChanges, bool returnNewestChangesFirst, bool trackConversations, bool allowTableRestrict, bool disposeFolder, ISyncLogger syncLogger = null)
		{
			MailboxSyncProvider mailboxSyncProvider = MailboxSyncProvider.Bind(folder, trackReadFlagChanges, trackAssociatedMessageChanges, returnNewestChangesFirst, trackConversations, allowTableRestrict, disposeFolder, syncLogger);
			mailboxSyncProvider.IcsPropertyGroupFilter = this.IcsPropertyGroupFilter;
			return mailboxSyncProvider;
		}

		public byte[] GetCollectionIdBytes()
		{
			return this.folderId.GetBytes();
		}

		public void SetCollectionIdFromBytes(byte[] collectionBytes)
		{
			this.folderId = StoreObjectId.Deserialize(collectionBytes);
		}

		protected bool allowTableRestrict;

		protected Folder folder;

		protected StoreObjectId folderId;

		protected StoreSession storeSession;

		protected bool trackReadFlagChanges;

		protected bool trackAssociatedMessageChanges;

		protected bool returnNewestChangesFirst;

		protected bool trackConversations;
	}
}
