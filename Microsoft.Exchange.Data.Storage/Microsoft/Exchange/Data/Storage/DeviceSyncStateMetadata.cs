using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class DeviceSyncStateMetadata
	{
		public DeviceSyncStateMetadata(MailboxSession mailboxSession, StoreObjectId syncStateFolderId, ISyncLogger syncLogger = null)
		{
			if (syncLogger == null)
			{
				syncLogger = TracingLogger.Singleton;
			}
			ArgumentValidator.ThrowIfNull("mailboxSession", mailboxSession);
			ArgumentValidator.ThrowIfNull("syncStateFolderId", syncStateFolderId);
			this.DeviceFolderId = syncStateFolderId;
			this.LastAccessUtc = ExDateTime.UtcNow;
			using (Folder folder = Folder.Bind(mailboxSession, syncStateFolderId, new PropertyDefinition[]
			{
				FolderSchema.ItemCount,
				FolderSchema.ChildCount
			}))
			{
				this.Id = new DeviceIdentity(folder.DisplayName);
				if (folder.ItemCount > 0)
				{
					this.ProcessSyncStateItems(folder, syncLogger);
				}
				if (folder.HasSubfolders)
				{
					this.ProcessSyncStateFolders(folder, syncLogger);
				}
			}
		}

		private void ProcessSyncStateItems(Folder deviceFolder, ISyncLogger syncLogger)
		{
			using (QueryResult queryResult = deviceFolder.ItemQuery(ItemQueryType.None, null, null, DeviceSyncStateMetadata.NullSyncPropertiesItems))
			{
				for (;;)
				{
					IStorePropertyBag[] propertyBags = queryResult.GetPropertyBags(100);
					if (propertyBags == null || propertyBags.Length == 0)
					{
						break;
					}
					foreach (IStorePropertyBag storePropertyBag in propertyBags)
					{
						string displayName = (string)storePropertyBag.TryGetProperty(ItemSchema.Subject);
						SyncStateMetadata metadataFromPropertyBag = this.GetMetadataFromPropertyBag(storePropertyBag, displayName, syncLogger);
						if (metadataFromPropertyBag != null)
						{
							StoreObjectId objectId = ((VersionedId)storePropertyBag.TryGetProperty(ItemSchema.Id)).ObjectId;
							metadataFromPropertyBag.FolderSyncStateId = null;
							metadataFromPropertyBag.ItemSyncStateId = objectId;
							syncLogger.TraceDebug<SyncStateMetadata>(ExTraceGlobals.SyncProcessTracer, (long)this.GetHashCode(), "[DeviceSyncStateMetadata.ProcessSyncStateItems] Found Item SyncState: {0}", metadataFromPropertyBag);
							this.syncStateMap.TryAdd(metadataFromPropertyBag.Name, metadataFromPropertyBag);
						}
						else
						{
							syncLogger.TraceDebug<DeviceIdentity>(ExTraceGlobals.SyncProcessTracer, (long)this.GetHashCode(), "[DeviceSyncStateMetadata.ProcessSyncStateItems] Discovered unusable sync state for device {0}", this.Id);
						}
					}
				}
			}
		}

		private void ProcessSyncStateFolders(Folder deviceFolder, ISyncLogger syncLogger)
		{
			using (QueryResult queryResult = deviceFolder.FolderQuery(FolderQueryFlags.None, null, null, DeviceSyncStateMetadata.NullSyncPropertiesFolders))
			{
				for (;;)
				{
					IStorePropertyBag[] propertyBags = queryResult.GetPropertyBags(100);
					if (propertyBags == null || propertyBags.Length == 0)
					{
						break;
					}
					foreach (IStorePropertyBag propertyBag in propertyBags)
					{
						SyncStateMetadata metadataFromPropertyBag = this.GetMetadataFromPropertyBag(deviceFolder.Session as MailboxSession, propertyBag, syncLogger);
						if (metadataFromPropertyBag != null)
						{
							syncLogger.TraceDebug<SyncStateMetadata>(ExTraceGlobals.SyncProcessTracer, (long)this.GetHashCode(), "[DeviceSyncStateMetadata.ProcessSyncStateFolders] Found SyncState: {0}", metadataFromPropertyBag);
							this.syncStateMap.TryAdd(metadataFromPropertyBag.Name, metadataFromPropertyBag);
						}
						else
						{
							syncLogger.TraceDebug<DeviceIdentity>(ExTraceGlobals.SyncProcessTracer, (long)this.GetHashCode(), "[DeviceSyncStateMetadata.ProcessSyncStateFolders] Discovered unusable sync state for device {0}", this.Id);
						}
					}
				}
			}
		}

		public FolderHierarchyChangeDetector.SyncHierarchyManifestState SyncHierarchyManifestState { get; private set; }

		public object PingFolderList { get; set; }

		public IDictionary<string, SyncStateMetadata> SyncStates
		{
			get
			{
				return this.syncStateMap;
			}
		}

		public IDictionary<StoreObjectId, FolderSyncStateMetadata> SyncStatesByIPMFolderId
		{
			get
			{
				return this.ipmToFolderSyncStateMap;
			}
		}

		public ExDateTime LastAccessUtc { get; set; }

		public byte[] LastCachedSyncRequest { get; private set; }

		public string LastSyncRequestRandomString { get; private set; }

		public bool ClientCanSendUpEmptyRequests { get; private set; }

		public void SaveSyncStatusData(string lastSyncRequestRandomString, byte[] lastCachableWbxmlDocument, bool clientCanSendUpEmtpyRequests)
		{
			lock (this.syncStatusInstanceLock)
			{
				this.LastSyncRequestRandomString = lastSyncRequestRandomString;
				this.LastCachedSyncRequest = lastCachableWbxmlDocument;
				this.ClientCanSendUpEmptyRequests = clientCanSendUpEmtpyRequests;
			}
		}

		public int FolderSyncStateCount
		{
			get
			{
				int num = 0;
				foreach (KeyValuePair<string, SyncStateMetadata> keyValuePair in this.syncStateMap)
				{
					if (keyValuePair.Value is FolderSyncStateMetadata)
					{
						num++;
					}
				}
				return num;
			}
		}

		public void RecordLatestFolderHierarchySnapshot(FolderHierarchyChangeDetector.SyncHierarchyManifestState lastState)
		{
			this.SyncHierarchyManifestState = lastState;
		}

		public void RecordLatestFolderHierarchySnapshot(MailboxSession mailboxSession, ISyncLogger syncLogger = null)
		{
			if (syncLogger == null)
			{
				syncLogger = TracingLogger.Singleton;
			}
			FolderHierarchyChangeDetector.SyncHierarchyManifestState syncHierarchyManifestState = this.SyncHierarchyManifestState;
			bool catchup = false;
			if (syncHierarchyManifestState == null)
			{
				syncLogger.TraceDebug(ExTraceGlobals.SyncProcessTracer, (long)this.GetHashCode(), "[DeviceSyncStateMetadata.RecordLatestFolderHierarchySnapshot] Last ICS snapshot was null.  Doing a catchup sync.");
				syncHierarchyManifestState = new FolderHierarchyChangeDetector.SyncHierarchyManifestState();
				this.SyncHierarchyManifestState = syncHierarchyManifestState;
				catchup = true;
			}
			FolderHierarchyChangeDetector.RunICSManifestSync(catchup, syncHierarchyManifestState, mailboxSession, syncLogger);
		}

		public FolderHierarchyChangeDetector.MailboxChangesManifest GetFolderHierarchyICSChanges(MailboxSession mailboxSession, out FolderHierarchyChangeDetector.SyncHierarchyManifestState latestState, ISyncLogger syncLogger = null)
		{
			if (syncLogger == null)
			{
				syncLogger = TracingLogger.Singleton;
			}
			latestState = this.SyncHierarchyManifestState;
			if (latestState == null)
			{
				syncLogger.TraceDebug(ExTraceGlobals.SyncProcessTracer, (long)this.GetHashCode(), "[DeviceSyncStateMetadata.GetFolderHierarchyICSChanges] Old ICS state was missing from cache.  Must do expensive check instead.");
				return null;
			}
			latestState = latestState.Clone();
			FolderHierarchyChangeDetector.MailboxChangesManifest mailboxChangesManifest = FolderHierarchyChangeDetector.RunICSManifestSync(false, latestState, mailboxSession, syncLogger);
			syncLogger.TraceDebug<string, string>(ExTraceGlobals.SyncProcessTracer, (long)this.GetHashCode(), "[DeviceSyncStateMetadata.GetFolderHierarchyICSChanges] Changes: {0}, Deletes: {1}", (mailboxChangesManifest == null || mailboxChangesManifest.ChangedFolders == null) ? "<NULL>" : mailboxChangesManifest.ChangedFolders.Count.ToString(), (mailboxChangesManifest == null || mailboxChangesManifest.DeletedFolders == null) ? "<NULL>" : mailboxChangesManifest.DeletedFolders.Count.ToString());
			return mailboxChangesManifest;
		}

		public SyncStateMetadata TryAdd(SyncStateMetadata syncStateMetadata, ISyncLogger syncLogger = null)
		{
			if (syncLogger == null)
			{
				syncLogger = TracingLogger.Singleton;
			}
			SyncStateMetadata syncStateMetadata2 = this.syncStateMap.GetOrAdd(syncStateMetadata.Name, syncStateMetadata);
			if (syncStateMetadata.GetType().IsSubclassOf(syncStateMetadata2.GetType()))
			{
				syncLogger.TraceDebug<string, string>(ExTraceGlobals.SyncProcessTracer, (long)this.GetHashCode(), "[DeviceSyncStateMetadata.TryAdd] New sync state metadata instance ({0}) is subclass of cached one ({1}).  Replacing.", syncStateMetadata.GetType().Name, syncStateMetadata2.GetType().Name);
				this.syncStateMap[syncStateMetadata.Name] = syncStateMetadata;
				syncStateMetadata2 = syncStateMetadata;
			}
			FolderSyncStateMetadata folderSyncStateMetadata = syncStateMetadata2 as FolderSyncStateMetadata;
			if (folderSyncStateMetadata != null && folderSyncStateMetadata.IPMFolderId != null)
			{
				syncLogger.TraceDebug<string>(ExTraceGlobals.SyncProcessTracer, (long)this.GetHashCode(), "[DeviceSyncStateMetadata.TryAdd] Encountered FolderSyncStateMetadata instance for '{0}'.  Adding source key to reverse mapping.", syncStateMetadata.Name);
				this.ipmToFolderSyncStateMap[folderSyncStateMetadata.IPMFolderId] = folderSyncStateMetadata;
			}
			return syncStateMetadata2;
		}

		public void ChangeIPMFolderId(FolderSyncStateMetadata folderSyncStateMetadata, StoreObjectId oldId, ISyncLogger syncLogger = null)
		{
			if (syncLogger == null)
			{
				syncLogger = TracingLogger.Singleton;
			}
			if (oldId != null)
			{
				FolderSyncStateMetadata folderSyncStateMetadata2;
				this.ipmToFolderSyncStateMap.TryRemove(oldId, out folderSyncStateMetadata2);
			}
			syncLogger.TraceDebug<string, string, string>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "[DeviceSyncStateMetadata.ChangeIPMFolderId] IPM Folder Id for collection '{0}' changed from '{1}' to '{2}'", folderSyncStateMetadata.Name, (oldId == null) ? "<NULL>" : oldId.ToBase64String(), (folderSyncStateMetadata.IPMFolderId == null) ? "<NULL>" : folderSyncStateMetadata.IPMFolderId.ToBase64String());
			if (folderSyncStateMetadata.IPMFolderId != null)
			{
				this.ipmToFolderSyncStateMap.TryAdd(folderSyncStateMetadata.IPMFolderId, folderSyncStateMetadata);
			}
		}

		public SyncStateMetadata TryRemove(string name, ISyncLogger syncLogger = null)
		{
			if (syncLogger == null)
			{
				syncLogger = TracingLogger.Singleton;
			}
			SyncStateMetadata syncStateMetadata;
			bool arg = this.syncStateMap.TryRemove(name, out syncStateMetadata);
			syncLogger.TraceDebug<string, bool>(ExTraceGlobals.SyncProcessTracer, (long)this.GetHashCode(), "[DeviceSyncStateMetadata.TryRemove] Removing '{0}'.  Success? {1}", name, arg);
			FolderSyncStateMetadata folderSyncStateMetadata = syncStateMetadata as FolderSyncStateMetadata;
			if (folderSyncStateMetadata != null && folderSyncStateMetadata.IPMFolderId != null)
			{
				this.ipmToFolderSyncStateMap.TryRemove(folderSyncStateMetadata.IPMFolderId, out folderSyncStateMetadata);
			}
			return syncStateMetadata;
		}

		public void RemoveAll()
		{
			this.syncStateMap.Clear();
			this.ipmToFolderSyncStateMap.Clear();
		}

		public SyncStateMetadata GetSyncState(MailboxSession mailboxSession, string name, ISyncLogger syncLogger = null)
		{
			if (syncLogger == null)
			{
				syncLogger = TracingLogger.Singleton;
			}
			SyncStateMetadata result;
			if (this.syncStateMap.TryGetValue(name, out result))
			{
				syncLogger.TraceDebug<string>(ExTraceGlobals.SyncProcessTracer, (long)this.GetHashCode(), "[DeviceSyncStateMetadata.GetSyncState] Cache hit for sync state: {0}", name);
				return result;
			}
			syncLogger.TraceDebug<string>(ExTraceGlobals.SyncProcessTracer, (long)this.GetHashCode(), "[DeviceSyncStateMetadata.GetSyncState] Cache MISS for sync state: {0}", name);
			using (Folder folder = Folder.Bind(mailboxSession, this.DeviceFolderId, new PropertyDefinition[]
			{
				FolderSchema.ItemCount,
				FolderSchema.ChildCount
			}))
			{
				int itemCount = folder.ItemCount;
				syncLogger.TraceDebug<string, int, bool>(ExTraceGlobals.SyncProcessTracer, (long)this.GetHashCode(), "[DeviceSyncStateMetadata.GetSyncState] deviceFolder {0} has {1} items and subfolders? {2}", folder.DisplayName, itemCount, folder.HasSubfolders);
				SyncStateMetadata syncStateMetadata = null;
				if (itemCount > 0)
				{
					syncStateMetadata = this.GetSyncStateItemMetadata(mailboxSession, folder, name, syncLogger);
					if (syncStateMetadata != null)
					{
						syncLogger.TraceDebug<SyncStateMetadata>(ExTraceGlobals.SyncProcessTracer, (long)this.GetHashCode(), "[DeviceSyncStateMetadata.GetSyncState] Retrieved DIRECT item sync state: {0}", syncStateMetadata);
						syncStateMetadata.FolderSyncStateId = null;
					}
				}
				if (syncStateMetadata == null && folder.HasSubfolders)
				{
					using (QueryResult queryResult = folder.FolderQuery(FolderQueryFlags.None, null, new SortBy[]
					{
						new SortBy(FolderSchema.DisplayName, SortOrder.Ascending)
					}, DeviceSyncStateMetadata.NullSyncPropertiesFolders))
					{
						if (queryResult.SeekToCondition(SeekReference.OriginBeginning, new ComparisonFilter(ComparisonOperator.Equal, FolderSchema.DisplayName, name)))
						{
							IStorePropertyBag propertyBag = queryResult.GetPropertyBags(1)[0];
							syncStateMetadata = this.GetMetadataFromPropertyBag(mailboxSession, propertyBag, syncLogger);
							if (syncStateMetadata != null)
							{
								syncLogger.TraceDebug<SyncStateMetadata>(ExTraceGlobals.SyncProcessTracer, (long)this.GetHashCode(), "[DeviceSyncStateMetadata.GetSyncState] Retrieved sub folder sync state: {0}", syncStateMetadata);
							}
						}
					}
				}
				if (syncStateMetadata != null)
				{
					SyncStateMetadata syncStateMetadata2 = this.syncStateMap.GetOrAdd(syncStateMetadata.Name, syncStateMetadata);
					if (syncStateMetadata2.StorageType != syncStateMetadata.StorageType)
					{
						syncLogger.TraceDebug<StorageType, StorageType>(ExTraceGlobals.SyncProcessTracer, (long)this.GetHashCode(), "[DeviceSyncStateMetadata.GetSyncState] Metadata was already cached but had store type: {0}.  New instance was: {1}.  Using new instance.", syncStateMetadata2.StorageType, syncStateMetadata.StorageType);
						this.syncStateMap[syncStateMetadata.Name] = syncStateMetadata;
						syncStateMetadata2 = syncStateMetadata;
					}
					return syncStateMetadata2;
				}
				syncLogger.TraceDebug<string, Guid>(ExTraceGlobals.SyncProcessTracer, (long)this.GetHashCode(), "[DeviceSyncStateMetadata.GetSyncState] Cache miss for sync state {0}, Mailbox {1}, but search did not find it.", name, mailboxSession.MailboxGuid);
			}
			return null;
		}

		private SyncStateMetadata GetMetadataFromPropertyBag(MailboxSession mailboxSession, IStorePropertyBag propertyBag, ISyncLogger syncLogger = null)
		{
			if (syncLogger == null)
			{
				syncLogger = TracingLogger.Singleton;
			}
			StoreObjectId objectId = ((VersionedId)propertyBag.TryGetProperty(FolderSchema.Id)).ObjectId;
			int num = (int)propertyBag.TryGetProperty(FolderSchema.ItemCount);
			string text = (string)propertyBag.TryGetProperty(FolderSchema.DisplayName);
			syncLogger.TraceDebug<string, int>(ExTraceGlobals.SyncProcessTracer, (long)this.GetHashCode(), "[DeviceSyncStateMetadata.GetMetadataFromPropertyBag] SyncState {0} has {1} children.", text, num);
			SyncStateMetadata syncStateMetadata;
			if (num > 0)
			{
				syncStateMetadata = this.GetSyncStateItemMetadata(mailboxSession, objectId, text, syncLogger);
				if (syncStateMetadata != null)
				{
					syncLogger.TraceDebug(ExTraceGlobals.SyncProcessTracer, (long)this.GetHashCode(), "[DeviceSyncStateMetadata.GetMetadataFromPropertyBag] Sync state was from item.");
					return syncStateMetadata;
				}
			}
			syncLogger.TraceDebug(ExTraceGlobals.SyncProcessTracer, (long)this.GetHashCode(), "[DeviceSyncStateMetadata.GetMetadataFromPropertyBag] Sync state was from folder.");
			syncStateMetadata = this.GetMetadataFromPropertyBag(propertyBag, text, syncLogger);
			syncStateMetadata.FolderSyncStateId = objectId;
			syncStateMetadata.ItemSyncStateId = null;
			return syncStateMetadata;
		}

		public SyncStateMetadata GetMetadataFromPropertyBag(IStorePropertyBag propertyBag, string displayName, ISyncLogger syncLogger = null)
		{
			byte[] byteArray = null;
			if (!DeviceSyncStateMetadata.TryGetPropertyFromBag<byte[]>(propertyBag, InternalSchema.SyncFolderSourceKey, out byteArray, syncLogger))
			{
				syncLogger.TraceDebug<string>(ExTraceGlobals.SyncProcessTracer, (long)this.GetHashCode(), "[DeviceSyncStateMetadata.GetMetadataFromPropertyBag] Creating custom sync state metadata for folder '{0}'", displayName);
				return new SyncStateMetadata(this, displayName, null, null);
			}
			StoreObjectId storeObjectId = StoreObjectId.Deserialize(byteArray);
			syncLogger.TraceDebug<string, StoreObjectId>(ExTraceGlobals.SyncProcessTracer, (long)this.GetHashCode(), "[DeviceSyncStateMetadata.GetMetadataFromPropertyBag] Found SyncFolderSourceKey for {0}, so it is a FolderSyncState: {1}", displayName, storeObjectId);
			long localCommitTimeMax = 0L;
			int deletedCountTotal = 0;
			int syncKey = 0;
			int airSyncFilter = 0;
			bool conversationMode = false;
			int airSyncSettingsHash = 0;
			int airSyncMaxItems = 0;
			VariantConfigurationSnapshot snapshot = VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null);
			bool flag = snapshot != null && snapshot.DataStorage.IgnoreInessentialMetaDataLoadErrors != null && snapshot.DataStorage.IgnoreInessentialMetaDataLoadErrors.Enabled;
			syncLogger.TraceDebug<bool>(ExTraceGlobals.SyncProcessTracer, (long)this.GetHashCode(), "[DeviceSyncStateMetadata.GetMetadataFromPropertyBag] ignoreInessentialMetaDataLoadErrors: {0}", flag);
			long airSyncLastSyncTime;
			bool flag2 = DeviceSyncStateMetadata.TryGetPropertyFromBag<long>(propertyBag, AirSyncStateSchema.MetadataLastSyncTime, out airSyncLastSyncTime, syncLogger) && DeviceSyncStateMetadata.TryGetPropertyFromBag<long>(propertyBag, AirSyncStateSchema.MetadataLocalCommitTimeMax, out localCommitTimeMax, syncLogger) && DeviceSyncStateMetadata.TryGetPropertyFromBag<int>(propertyBag, AirSyncStateSchema.MetadataDeletedCountTotal, out deletedCountTotal, syncLogger) && DeviceSyncStateMetadata.TryGetPropertyFromBag<int>(propertyBag, AirSyncStateSchema.MetadataSyncKey, out syncKey, syncLogger) && DeviceSyncStateMetadata.TryGetPropertyFromBag<int>(propertyBag, AirSyncStateSchema.MetadataFilter, out airSyncFilter, syncLogger) && DeviceSyncStateMetadata.TryGetPropertyFromBag<bool>(propertyBag, AirSyncStateSchema.MetadataConversationMode, out conversationMode, syncLogger) && DeviceSyncStateMetadata.TryGetPropertyFromBag<int>(propertyBag, AirSyncStateSchema.MetadataSettingsHash, out airSyncSettingsHash, syncLogger);
			bool flag3 = flag2 && DeviceSyncStateMetadata.TryGetPropertyFromBag<int>(propertyBag, AirSyncStateSchema.MetadataMaxItems, out airSyncMaxItems, syncLogger);
			flag2 = (flag ? flag2 : flag3);
			if (flag2)
			{
				syncLogger.TraceDebug<string>(ExTraceGlobals.SyncProcessTracer, (long)this.GetHashCode(), "[DeviceSyncStateMetadata.GetMetadataFromPropertyBag] Creating FolderSync metadata for folder '{0}'", displayName);
				return new FolderSyncStateMetadata(this, displayName, null, null, localCommitTimeMax, deletedCountTotal, syncKey, conversationMode, airSyncFilter, airSyncLastSyncTime, airSyncSettingsHash, airSyncMaxItems, storeObjectId);
			}
			syncLogger.TraceDebug<string>(ExTraceGlobals.SyncProcessTracer, (long)this.GetHashCode(), "[DeviceSyncStateMetadata.GetMetadataFromPropertyBag] Failed to get nullSync properties for sync folder '{0}'.", displayName);
			return new FolderSyncStateMetadata(this, displayName, null, null, 0L, 0, 0, false, 0, 0L, 0, 0, storeObjectId);
		}

		private SyncStateMetadata GetSyncStateItemMetadata(MailboxSession mailboxSession, StoreObjectId parentFolderId, string displayName, ISyncLogger syncLogger = null)
		{
			SyncStateMetadata syncStateItemMetadata;
			using (Folder folder = Folder.Bind(mailboxSession, parentFolderId, DeviceSyncStateMetadata.NullSyncPropertiesFolders))
			{
				syncStateItemMetadata = this.GetSyncStateItemMetadata(mailboxSession, folder, displayName, syncLogger);
			}
			return syncStateItemMetadata;
		}

		private SyncStateMetadata GetSyncStateItemMetadata(MailboxSession mailboxSession, Folder parentFolder, string displayName, ISyncLogger syncLogger = null)
		{
			if (syncLogger == null)
			{
				syncLogger = TracingLogger.Singleton;
			}
			using (QueryResult queryResult = parentFolder.ItemQuery(ItemQueryType.None, null, new SortBy[]
			{
				new SortBy(ItemSchema.Subject, SortOrder.Ascending)
			}, DeviceSyncStateMetadata.NullSyncPropertiesItems))
			{
				if (queryResult.SeekToCondition(SeekReference.OriginBeginning, new ComparisonFilter(ComparisonOperator.Equal, ItemSchema.Subject, displayName)))
				{
					IStorePropertyBag storePropertyBag = queryResult.GetPropertyBags(1)[0];
					StoreObjectId objectId = ((VersionedId)storePropertyBag.TryGetProperty(ItemSchema.Id)).ObjectId;
					syncLogger.TraceDebug<string, SmtpAddress, string>(ExTraceGlobals.SyncProcessTracer, (long)this.GetHashCode(), "[DeviceSyncStateMetadata.GetSyncStateItemMetadata] Sync state '{0}' for mailbox '{1}' is stored on item id '{2}'", displayName, mailboxSession.MailboxOwner.MailboxInfo.PrimarySmtpAddress, objectId.ToBase64String());
					SyncStateMetadata metadataFromPropertyBag = this.GetMetadataFromPropertyBag(storePropertyBag, displayName, syncLogger);
					metadataFromPropertyBag.FolderSyncStateId = parentFolder.Id.ObjectId;
					metadataFromPropertyBag.ItemSyncStateId = objectId;
					return metadataFromPropertyBag;
				}
			}
			syncLogger.TraceDebug<string>(ExTraceGlobals.SyncProcessTracer, (long)this.GetHashCode(), "[DeviceSyncStateMetadata.GetSyncStateItemMetadata] Did not find child item with name {0}", displayName);
			return null;
		}

		private static bool TryGetPropertyFromBag<T>(IStorePropertyBag propertyBag, PropertyDefinition propDef, out T value, ISyncLogger syncLogger = null)
		{
			object obj = propertyBag.TryGetProperty(propDef);
			if (obj is T)
			{
				value = (T)((object)obj);
				return true;
			}
			PropertyError propertyError = obj as PropertyError;
			if (propertyError != null)
			{
				syncLogger.TraceError<Type, string, PropertyErrorCode>(ExTraceGlobals.SyncProcessTracer, 0L, "[DeviceSyncStateMetadata.TryGetPropertyFromBag] Expected property of type {0} in bag for propDef {1}, but encountered error {2}.", typeof(T), propDef.Name, propertyError.PropertyErrorCode);
			}
			else
			{
				try
				{
					value = (T)((object)obj);
					return true;
				}
				catch (InvalidCastException ex)
				{
					syncLogger.TraceError(ExTraceGlobals.SyncProcessTracer, 0L, "[DeviceSyncStateMetadata.TryGetPropertyFromBag] Tried to cast property '{0}' with value '{1}' to type '{2}', but the cast failed with error '{3}'.", new object[]
					{
						propDef.Name,
						(obj == null) ? "<NULL>" : obj,
						typeof(T).FullName,
						ex
					});
				}
			}
			value = default(T);
			return false;
		}

		public DeviceIdentity Id { get; private set; }

		public StoreObjectId DeviceFolderId { get; private set; }

		public override string ToString()
		{
			return string.Format("Device: {0} - {1}", this.Id, this.DeviceFolderId.ToBase64String());
		}

		public static readonly PropertyDefinition[] NullSyncPropertiesFolders = new PropertyDefinition[]
		{
			FolderSchema.Id,
			FolderSchema.DisplayName,
			FolderSchema.ItemCount,
			FolderSchema.SyncFolderSourceKey,
			AirSyncStateSchema.MetadataLastSyncTime,
			AirSyncStateSchema.MetadataLocalCommitTimeMax,
			AirSyncStateSchema.MetadataDeletedCountTotal,
			AirSyncStateSchema.MetadataSyncKey,
			AirSyncStateSchema.MetadataFilter,
			AirSyncStateSchema.MetadataMaxItems,
			AirSyncStateSchema.MetadataConversationMode,
			AirSyncStateSchema.MetadataSettingsHash
		};

		public static readonly PropertyDefinition[] NullSyncPropertiesItems = new PropertyDefinition[]
		{
			ItemSchema.Id,
			ItemSchema.Subject,
			FolderSchema.SyncFolderSourceKey,
			AirSyncStateSchema.MetadataLastSyncTime,
			AirSyncStateSchema.MetadataLocalCommitTimeMax,
			AirSyncStateSchema.MetadataDeletedCountTotal,
			AirSyncStateSchema.MetadataSyncKey,
			AirSyncStateSchema.MetadataFilter,
			AirSyncStateSchema.MetadataMaxItems,
			AirSyncStateSchema.MetadataConversationMode,
			AirSyncStateSchema.MetadataSettingsHash
		};

		private ConcurrentDictionary<string, SyncStateMetadata> syncStateMap = new ConcurrentDictionary<string, SyncStateMetadata>();

		private ConcurrentDictionary<StoreObjectId, FolderSyncStateMetadata> ipmToFolderSyncStateMap = new ConcurrentDictionary<StoreObjectId, FolderSyncStateMetadata>();

		private object syncStatusInstanceLock = new object();
	}
}
