using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.AirSync
{
	internal class EntitySyncProviderFactory : MailboxSyncProviderFactory
	{
		public EntitySyncProviderFactory(StoreSession storeSession) : this(storeSession, null, false)
		{
		}

		public EntitySyncProviderFactory(StoreSession storeSession, StoreObjectId folderId) : this(storeSession, folderId, false)
		{
		}

		public EntitySyncProviderFactory(StoreSession storeSession, bool allowTableRestrict) : this(storeSession, null, allowTableRestrict)
		{
		}

		public EntitySyncProviderFactory(StoreSession storeSession, StoreObjectId folderId, bool allowTableRestrict) : base(storeSession, folderId, allowTableRestrict)
		{
		}

		protected override ISyncProvider CreateSyncProvider(Folder folder, bool trackReadFlagChanges, bool trackAssociatedMessageChanges, bool returnNewestChangesFirst, bool trackConversations, bool allowTableRestrict, bool disposeFolder, ISyncLogger syncLogger = null)
		{
			return new EntitySyncProvider(folder, trackReadFlagChanges, trackAssociatedMessageChanges, returnNewestChangesFirst, trackConversations, allowTableRestrict, disposeFolder);
		}
	}
}
