using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.AirSync;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.AirSync
{
	internal class RecipientInfoCacheSyncProviderFactory : ISyncProviderFactory
	{
		public RecipientInfoCacheSyncProviderFactory(MailboxSession mailboxSession)
		{
			this.mailboxSession = mailboxSession;
			this.LastModifiedTime = ExDateTime.MinValue;
			try
			{
				using (UserConfiguration mailboxConfiguration = mailboxSession.UserConfigurationManager.GetMailboxConfiguration("OWA.AutocompleteCache", UserConfigurationTypes.XML))
				{
					this.LastModifiedTime = mailboxConfiguration.LastModifiedTime;
					this.NativeStoreObjectId = mailboxConfiguration.Id;
				}
			}
			catch (ObjectNotFoundException)
			{
				AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "Failed to find the cache and set the last modified time!");
			}
		}

		public int MaxEntries
		{
			get
			{
				return this.maxEntries;
			}
			set
			{
				this.maxEntries = value;
			}
		}

		public ExDateTime LastModifiedTime { get; set; }

		public StoreObjectId NativeStoreObjectId { get; private set; }

		public ISyncProvider CreateSyncProvider(ISyncLogger syncLogger = null)
		{
			return new RecipientInfoCacheSyncProvider(this.mailboxSession, this.maxEntries);
		}

		public byte[] GetCollectionIdBytes()
		{
			if (this.folderId == null)
			{
				using (RecipientInfoCacheSyncProvider recipientInfoCacheSyncProvider = (RecipientInfoCacheSyncProvider)this.CreateSyncProvider(null))
				{
					this.folderId = recipientInfoCacheSyncProvider.ItemId;
				}
			}
			if (this.folderId == null)
			{
				return null;
			}
			return this.folderId.GetBytes();
		}

		public void SetCollectionIdFromBytes(byte[] collectionBytes)
		{
			this.folderId = StoreObjectId.Deserialize(collectionBytes);
		}

		private MailboxSession mailboxSession;

		private int maxEntries = int.MaxValue;

		private StoreObjectId folderId;

		private enum PropertiesToFetchEnum
		{
			LastModifiedTime,
			ItemClass
		}
	}
}
