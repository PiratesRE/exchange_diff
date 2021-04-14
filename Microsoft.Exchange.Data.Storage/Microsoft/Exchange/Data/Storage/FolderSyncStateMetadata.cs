using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class FolderSyncStateMetadata : SyncStateMetadata
	{
		public FolderSyncStateMetadata(DeviceSyncStateMetadata parentDevice, string name, StoreObjectId folderSyncStateId, StoreObjectId itemSyncStateId, StoreObjectId ipmFolderId) : this(parentDevice, name, folderSyncStateId, itemSyncStateId, 0L, 0, 0, false, 0, 0L, 0, 0, ipmFolderId)
		{
		}

		public bool HasValidNullSyncData
		{
			get
			{
				return this.AirSyncLocalCommitTime != 0L || this.AirSyncDeletedCountTotal != 0 || this.AirSyncSyncKey != 0 || this.AirSyncConversationMode || this.AirSyncFilter != 0 || this.AirSyncSettingsHash != 0 || this.AirSyncMaxItems != 0;
			}
		}

		public FolderSyncStateMetadata(DeviceSyncStateMetadata parentDevice, string name, StoreObjectId folderSyncStateId, StoreObjectId itemSyncStateId, long localCommitTimeMax, int deletedCountTotal, int syncKey, bool conversationMode, int airSyncFilter, long airSyncLastSyncTime, int airSyncSettingsHash, int airSyncMaxItems, StoreObjectId ipmFolderId) : base(parentDevice, name, folderSyncStateId, itemSyncStateId)
		{
			ArgumentValidator.ThrowIfNull("ipmFolderId", ipmFolderId);
			ArgumentValidator.ThrowIfNull("parentDevice", parentDevice);
			this.IPMFolderId = ipmFolderId;
			this.AirSyncLocalCommitTime = localCommitTimeMax;
			this.AirSyncDeletedCountTotal = deletedCountTotal;
			this.AirSyncSyncKey = syncKey;
			this.AirSyncConversationMode = conversationMode;
			this.AirSyncFilter = airSyncFilter;
			this.AirSyncLastSyncTime = airSyncLastSyncTime;
			this.AirSyncSettingsHash = airSyncSettingsHash;
			this.AirSyncMaxItems = airSyncMaxItems;
		}

		public StoreObjectId IPMFolderId
		{
			get
			{
				return this.ipmFolderId;
			}
			private set
			{
				StoreObjectId oldId;
				lock (this.instanceLock)
				{
					oldId = this.ipmFolderId;
					this.ipmFolderId = value;
				}
				base.ParentDevice.ChangeIPMFolderId(this, oldId, null);
			}
		}

		public long AirSyncLocalCommitTime { get; private set; }

		public int AirSyncDeletedCountTotal { get; private set; }

		public int AirSyncSyncKey { get; private set; }

		public bool AirSyncConversationMode { get; private set; }

		public int AirSyncFilter { get; private set; }

		public long AirSyncLastSyncTime { get; private set; }

		public int AirSyncSettingsHash { get; private set; }

		public int AirSyncMaxItems { get; private set; }

		public void UpdateRecipientInfoCacheNullSyncValues(long airSyncLocalCommitTime, int airSyncSyncKey, int airSyncMaxItems)
		{
			lock (this.instanceLock)
			{
				this.AirSyncLocalCommitTime = airSyncLocalCommitTime;
				this.AirSyncSyncKey = airSyncSyncKey;
				this.AirSyncMaxItems = airSyncMaxItems;
			}
		}

		public void UpdateSyncCollectionNullSyncValues(bool conversationMode, int deletedCountTotal, int filter, long lastSyncTime, long localCommitTime, int settingsHash, int syncKey)
		{
			lock (this.instanceLock)
			{
				this.AirSyncConversationMode = conversationMode;
				this.AirSyncDeletedCountTotal = deletedCountTotal;
				this.AirSyncFilter = filter;
				this.AirSyncLastSyncTime = lastSyncTime;
				this.AirSyncLocalCommitTime = localCommitTime;
				this.AirSyncSettingsHash = settingsHash;
				this.AirSyncSyncKey = syncKey;
			}
		}

		public IStorePropertyBag GetNullSyncPropertiesFromIPMFolder(MailboxSession mailboxSession)
		{
			IStorePropertyBag result;
			using (Folder folder = Folder.Bind(mailboxSession, this.IPMFolderId, FolderSyncStateMetadata.IPMFolderNullSyncProperties))
			{
				result = folder.PropertyBag.AsIStorePropertyBag();
			}
			return result;
		}

		private StoreObjectId ipmFolderId;

		private object instanceLock = new object();

		public static readonly PropertyDefinition[] IPMFolderNullSyncProperties = new PropertyDefinition[]
		{
			FolderSchema.Id,
			StoreObjectSchema.ParentItemId,
			StoreObjectSchema.ChangeKey,
			FolderSchema.IsHidden,
			FolderSchema.ExtendedFolderFlags,
			FolderSchema.LocalCommitTimeMax,
			FolderSchema.DeletedCountTotal,
			StoreObjectSchema.ContainerClass,
			FolderSchema.DisplayName
		};
	}
}
