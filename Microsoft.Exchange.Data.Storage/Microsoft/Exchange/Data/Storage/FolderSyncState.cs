using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class FolderSyncState : SyncState, IFolderSyncState, ISyncState
	{
		protected FolderSyncState(SyncStateStorage syncStateStorage, StoreObject storeObject, FolderSyncStateMetadata syncStateMetadata, SyncStateInfo syncStateInfo, ISyncProviderFactory syncProviderFactory, bool newSyncState, ISyncLogger syncLogger = null) : base(syncStateStorage, storeObject, syncStateMetadata, syncStateInfo, newSyncState, syncLogger)
		{
			this.syncProviderFactory = syncProviderFactory;
		}

		public string SyncFolderId
		{
			get
			{
				base.CheckDisposed("get_SyncFolderId");
				return base.UniqueName;
			}
		}

		public int? CustomVersion
		{
			get
			{
				base.CheckDisposed("get_CustomVersion");
				return base.GetData<NullableData<Int32Data, int>, int?>(SyncStateProp.CustomVersion, null);
			}
			set
			{
				base.CheckDisposed("set_CustomVersion");
				base[SyncStateProp.CustomVersion] = new NullableData<Int32Data, int>(value);
			}
		}

		public ISyncWatermark Watermark
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public static void RegisterCustomDataVersioningHandler(FolderSyncState.HandleCustomDataVersioningDelegate handleCustomVersioning)
		{
			FolderSyncState.handleCustomDataVersioning = handleCustomVersioning;
		}

		public void RegisterColdDataKey(string key)
		{
			base.CheckDisposed("RegisterColdDataKey");
			base.AddColdDataKey(key);
		}

		public override void Commit()
		{
			base.CheckDisposed("Commit");
			this.CommitState(null, null);
		}

		public override void Load()
		{
			base.CheckDisposed("Load");
			this.Load(true, new PropertyDefinition[]
			{
				InternalSchema.SyncFolderSourceKey
			});
		}

		public void CommitState(PropertyDefinition[] properties, object[] values)
		{
			base.CheckDisposed("CommitState");
			if (this.syncProviderFactory == null)
			{
				base.Commit(properties, values);
				return;
			}
			byte[] collectionIdBytes = this.syncProviderFactory.GetCollectionIdBytes();
			if (properties == null)
			{
				base.Commit(new PropertyDefinition[]
				{
					InternalSchema.SyncFolderSourceKey
				}, new object[]
				{
					collectionIdBytes
				});
				return;
			}
			PropertyDefinition[] array = new PropertyDefinition[properties.Length + 1];
			object[] array2 = new object[values.Length + 1];
			array[0] = InternalSchema.SyncFolderSourceKey;
			array2[0] = collectionIdBytes;
			properties.CopyTo(array, 1);
			values.CopyTo(array2, 1);
			base.Commit(array, array2);
		}

		public FolderSync GetFolderSync()
		{
			base.CheckDisposed("GetFolderSync");
			return this.GetFolderSync(ConflictResolutionPolicy.ServerWins);
		}

		public FolderSync GetFolderSync(ConflictResolutionPolicy policy)
		{
			return this.GetFolderSync(policy, null);
		}

		public FolderSync GetFolderSync(ConflictResolutionPolicy policy, Func<ISyncProvider, FolderSyncState, ConflictResolutionPolicy, bool, FolderSync> creator)
		{
			base.CheckDisposed("GetFolderSync");
			EnumValidator.ThrowIfInvalid<ConflictResolutionPolicy>(policy, "policy");
			this.syncLogger.Information<int, ConflictResolutionPolicy>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "SyncState::GetFolderSync. Hashcode = {0}, Policy = {1}.", this.GetHashCode(), policy);
			if (this.syncProviderFactory == null)
			{
				throw new InvalidOperationException("Must set a sync provider factory before calling GetFolderSync");
			}
			try
			{
				this.syncProvider = this.syncProviderFactory.CreateSyncProvider(null);
			}
			catch (ObjectNotFoundException)
			{
				return null;
			}
			if (creator != null)
			{
				return creator(this.syncProvider, this, policy, true);
			}
			return new FolderSync(this.syncProvider, this, policy, true);
		}

		public override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<FolderSyncState>(this);
		}

		public StoreObjectId TryGetStoreObjectId()
		{
			base.CheckDisposed("TryGetStoreObjectId");
			byte[] collectionIdBytes = this.syncProviderFactory.GetCollectionIdBytes();
			if (collectionIdBytes == null)
			{
				return null;
			}
			return StoreObjectId.Deserialize(collectionIdBytes);
		}

		internal static FolderSyncState GetSyncState(SyncStateStorage syncStateStorage, Folder syncStateParentFolder, string syncFolderId, ISyncLogger syncLogger = null)
		{
			return FolderSyncState.GetSyncState(syncStateStorage, syncStateParentFolder, null, syncFolderId, syncLogger);
		}

		internal static FolderSyncState GetSyncState(SyncStateStorage syncStateStorage, Folder deviceFolder, ISyncProviderFactory syncProviderFactory, Func<SyncStateStorage, StoreObject, FolderSyncStateMetadata, SyncStateInfo, ISyncProviderFactory, bool, ISyncLogger, FolderSyncState> creator, ISyncLogger syncLogger = null)
		{
			if (syncLogger == null)
			{
				syncLogger = TracingLogger.Singleton;
			}
			ArgumentValidator.ThrowIfNull("syncStateStorage", syncStateStorage);
			ArgumentValidator.ThrowIfNull("deviceFolder", deviceFolder);
			ArgumentValidator.ThrowIfNull("syncProviderFactory", syncProviderFactory);
			byte[] collectionIdBytes = syncProviderFactory.GetCollectionIdBytes();
			if (collectionIdBytes == null || collectionIdBytes.Length == 0)
			{
				throw new ArgumentException("SyncProviderFactory CollectionId bytes cannot be null or empty.");
			}
			StoreObjectId storeObjectId = null;
			try
			{
				storeObjectId = StoreObjectId.Deserialize(collectionIdBytes);
			}
			catch (ArgumentException innerException)
			{
				syncLogger.TraceError<string>(ExTraceGlobals.SyncTracer, 0L, "[FolderSyncState.GetSyncState(syncProviderFactory)] The IPMFolderBytes that the provider gave us are invalid for folder {0}", deviceFolder.DisplayName);
				throw new CorruptSyncStateException(ServerStrings.ExSyncStateCorrupted(deviceFolder.DisplayName), innerException);
			}
			FolderSyncStateMetadata folderSyncStateMetadata = null;
			if (!syncStateStorage.DeviceMetadata.SyncStatesByIPMFolderId.TryGetValue(storeObjectId, out folderSyncStateMetadata))
			{
				syncLogger.TraceDebug<DeviceIdentity, string>(ExTraceGlobals.SyncTracer, 0L, "[FolderSyncState.GetSyncState(syncProviderFactory)] Cache miss for device {0}, IPM folder Id {1}", syncStateStorage.DeviceMetadata.Id, storeObjectId.ToBase64String());
				return null;
			}
			SyncStateMetadata syncStateMetadata = folderSyncStateMetadata;
			StoreObject storeObject = SyncState.GetSyncStateStoreObject(deviceFolder, ref syncStateMetadata, syncLogger, new PropertyDefinition[]
			{
				InternalSchema.SyncFolderSourceKey
			});
			if (!object.ReferenceEquals(folderSyncStateMetadata, syncStateMetadata))
			{
				FolderSyncStateMetadata folderSyncStateMetadata2 = syncStateMetadata as FolderSyncStateMetadata;
				if (folderSyncStateMetadata2 == null)
				{
					syncLogger.TraceDebug<string, string>(ExTraceGlobals.SyncProcessTracer, 0L, "[FolderSyncState.GetSyncState] Device {0} has non-folder sync state for {1}.  Returning null.", deviceFolder.DisplayName, folderSyncStateMetadata.Name);
					if (storeObject != null)
					{
						storeObject.Dispose();
						storeObject = null;
					}
				}
				else
				{
					syncLogger.TraceDebug<string, string>(ExTraceGlobals.SyncProcessTracer, 0L, "[FolderSyncState.GetSyncState] Device {0} had  state folder sync state metadata for {1}.  Replacing.", deviceFolder.DisplayName, folderSyncStateMetadata.Name);
					folderSyncStateMetadata = folderSyncStateMetadata2;
				}
			}
			if (storeObject == null)
			{
				return null;
			}
			SyncStateInfo syncStateInfo = new FolderSyncStateInfo(folderSyncStateMetadata.Name);
			if (creator == null)
			{
				return new FolderSyncState(syncStateStorage, storeObject, folderSyncStateMetadata, syncStateInfo, syncProviderFactory, false, syncLogger);
			}
			return creator(syncStateStorage, storeObject, folderSyncStateMetadata, syncStateInfo, syncProviderFactory, false, syncLogger);
		}

		internal static FolderSyncState GetSyncState(SyncStateStorage syncStateStorage, Folder syncStateParentFolder, ISyncProviderFactory syncProviderFactory, string syncStateName, ISyncLogger syncLogger = null)
		{
			return FolderSyncState.GetSyncState(syncStateStorage, syncStateParentFolder, syncProviderFactory, syncStateName, null, syncLogger);
		}

		private static FolderSyncStateMetadata GetFolderSyncStateMetadata(SyncStateStorage syncStateStorage, MailboxSession mailboxSession, string name, ISyncLogger syncLogger = null)
		{
			if (syncLogger == null)
			{
				syncLogger = TracingLogger.Singleton;
			}
			SyncStateMetadata syncState = syncStateStorage.DeviceMetadata.GetSyncState(mailboxSession, name, syncLogger);
			FolderSyncStateMetadata folderSyncStateMetadata = syncState as FolderSyncStateMetadata;
			if (folderSyncStateMetadata == null)
			{
				syncLogger.TraceDebug<SmtpAddress, string>(ExTraceGlobals.SyncProcessTracer, 0L, "[FolderSyncState.GetFolderSyncStateMetadata] SyncStateMetadata in place of FolderSyncStateMetadata for Mailbox: {0}, State: {1}.  Trying re-read...", mailboxSession.MailboxOwner.MailboxInfo.PrimarySmtpAddress, name);
				syncStateStorage.DeviceMetadata.TryRemove(name, syncLogger);
				syncState = syncStateStorage.DeviceMetadata.GetSyncState(mailboxSession, name, syncLogger);
				folderSyncStateMetadata = (syncState as FolderSyncStateMetadata);
				if (folderSyncStateMetadata == null)
				{
					syncStateStorage.DeleteFolderSyncState(name);
					syncStateStorage.DeviceMetadata.TryRemove(name, null);
					throw new CorruptSyncStateException(ServerStrings.ExSyncStateCorrupted(name), new InvalidOperationException("SyncStateMetadata in place of FolderSyncStateMetadata"));
				}
				syncLogger.TraceDebug<string>(ExTraceGlobals.SyncProcessTracer, 0L, "[FolderSyncState.GetFolderSyncStateMetadata] Re-read of sync state {0} was successful.", name);
			}
			return folderSyncStateMetadata;
		}

		internal static FolderSyncState GetSyncState(SyncStateStorage syncStateStorage, Folder syncStateParentFolder, ISyncProviderFactory syncProviderFactory, string syncStateName, Func<SyncStateStorage, StoreObject, FolderSyncStateMetadata, SyncStateInfo, ISyncProviderFactory, bool, ISyncLogger, FolderSyncState> creator, ISyncLogger syncLogger = null)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("syncStateName", syncStateName);
			SyncStateInfo syncStateInfo = new FolderSyncStateInfo(syncStateName);
			StoreObject syncStateStoreObject = SyncState.GetSyncStateStoreObject(syncStateStorage, syncStateParentFolder, syncStateInfo, syncLogger, new PropertyDefinition[]
			{
				InternalSchema.SyncFolderSourceKey
			});
			if (syncStateStoreObject == null)
			{
				return null;
			}
			byte[] valueOrDefault = syncStateStoreObject.GetValueOrDefault<byte[]>(InternalSchema.SyncFolderSourceKey);
			if (syncProviderFactory != null)
			{
				try
				{
					syncProviderFactory.SetCollectionIdFromBytes(valueOrDefault);
				}
				catch (ArgumentException innerException)
				{
					syncStateStorage.DeleteFolderSyncState(syncStateName);
					syncStateStorage.DeviceMetadata.TryRemove(syncStateName, null);
					throw new CorruptSyncStateException(ServerStrings.ExSyncStateCorrupted(syncStateName), innerException);
				}
			}
			FolderSyncStateMetadata folderSyncStateMetadata = FolderSyncState.GetFolderSyncStateMetadata(syncStateStorage, syncStateStoreObject.Session as MailboxSession, syncStateName, syncLogger);
			if (creator == null)
			{
				return new FolderSyncState(syncStateStorage, syncStateStoreObject, folderSyncStateMetadata, syncStateInfo, syncProviderFactory, false, syncLogger);
			}
			return creator(syncStateStorage, syncStateStoreObject, folderSyncStateMetadata, syncStateInfo, syncProviderFactory, false, syncLogger);
		}

		internal static FolderSyncState CreateSyncState(SyncStateStorage syncStateStorage, Folder syncStateParentFolder, ISyncProviderFactory syncProviderFactory, string syncFolderId, ISyncLogger syncLogger = null)
		{
			if (syncLogger == null)
			{
				syncLogger = TracingLogger.Singleton;
			}
			SyncStateInfo syncStateInfo = new FolderSyncStateInfo(syncFolderId);
			StoreObject storeObject = SyncState.CreateSyncStateStoreObject(syncStateStorage, syncStateInfo, syncStateParentFolder, new PropertyDefinition[]
			{
				InternalSchema.SyncFolderSourceKey
			}, new object[]
			{
				syncProviderFactory.GetCollectionIdBytes()
			}, syncLogger);
			if (syncStateStorage.DeviceMetadata.TryRemove(syncStateInfo.UniqueName, syncLogger) != null)
			{
				syncLogger.TraceDebug<DeviceIdentity, string>(ExTraceGlobals.SyncTracer, 0L, "[FolderSyncState.CreateSyncState] Removed stale cached sync state metadata for device {0}, sync state {1}", syncStateStorage.DeviceMetadata.Id, syncStateInfo.UniqueName);
			}
			FolderSyncStateMetadata syncStateMetadata = (storeObject is Item) ? new FolderSyncStateMetadata(syncStateStorage.DeviceMetadata, syncStateInfo.UniqueName, syncStateStorage.SaveOnDirectItems ? null : storeObject.ParentId, storeObject.Id.ObjectId, StoreObjectId.Deserialize(syncProviderFactory.GetCollectionIdBytes())) : new FolderSyncStateMetadata(syncStateStorage.DeviceMetadata, syncStateInfo.UniqueName, storeObject.Id.ObjectId, null, StoreObjectId.Deserialize(syncProviderFactory.GetCollectionIdBytes()));
			return new FolderSyncState(syncStateStorage, storeObject, syncStateMetadata, syncStateInfo, syncProviderFactory, true, syncLogger);
		}

		protected static StoreObject GetSyncStateStoreObject(SyncStateStorage syncStateStorage, Folder syncStateParentFolder, SyncStateInfo syncStateInfo, byte[] identBytes, ISyncLogger syncLogger, params PropertyDefinition[] propsToReturn)
		{
			FolderSyncStateMetadata folderSyncStateMetadata = FolderSyncState.GetFolderSyncStateMetadata(syncStateStorage, syncStateParentFolder.Session as MailboxSession, syncStateInfo.UniqueName, syncLogger);
			if (folderSyncStateMetadata == null)
			{
				return null;
			}
			StoreObject storeObject = null;
			if (folderSyncStateMetadata.StorageType != StorageType.Folder)
			{
				if (folderSyncStateMetadata.StorageType != StorageType.Item)
				{
					if (folderSyncStateMetadata.StorageType != StorageType.DirectItem)
					{
						goto IL_85;
					}
				}
				try
				{
					storeObject = Microsoft.Exchange.Data.Storage.Item.Bind(syncStateParentFolder.Session, folderSyncStateMetadata.ItemSyncStateId, SyncState.AppendAdditionalProperties(propsToReturn));
					((Item)storeObject).OpenAsReadWrite();
					return storeObject;
				}
				catch
				{
					storeObject.Dispose();
					throw;
				}
				IL_85:
				throw new InvalidOperationException("Unsupported storage type for sync state");
			}
			storeObject = Folder.Bind(syncStateParentFolder.Session, folderSyncStateMetadata.FolderSyncStateId, SyncState.AppendAdditionalProperties(propsToReturn));
			return storeObject;
		}

		protected override void AddColdDataKeys()
		{
			base.AddColdDataKey(SyncStateProp.ClientState);
			base.AddColdDataKey(SyncStateProp.PrevServerManifest);
			base.AddColdDataKey(SyncStateProp.PrevDelayedServerOperationQueue);
			base.AddColdDataKey(SyncStateProp.PrevMaxWatermark);
			base.AddColdDataKey(SyncStateProp.PrevFilterId);
			base.AddColdDataKey(SyncStateProp.PrevSnapShotWatermark);
			base.AddColdDataKey(SyncStateProp.PrevLastSyncConversationMode);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing && this.syncProvider != null)
			{
				this.syncProvider.Dispose();
			}
			base.InternalDispose(disposing);
		}

		protected override void Load(bool reloadFromBackend, params PropertyDefinition[] additionalPropsToLoad)
		{
			base.Load(reloadFromBackend, additionalPropsToLoad);
			FolderSyncState.handleCustomDataVersioning(this);
		}

		private const ConflictResolutionPolicy DefaultConflictResolutionPolicy = ConflictResolutionPolicy.ServerWins;

		private static FolderSyncState.HandleCustomDataVersioningDelegate handleCustomDataVersioning = delegate(FolderSyncState syncState)
		{
			if (syncState.CustomVersion != null)
			{
				syncState.HandleCorruptSyncState();
			}
		};

		protected ISyncProvider syncProvider;

		private ISyncProviderFactory syncProviderFactory;

		public delegate void HandleCustomDataVersioningDelegate(FolderSyncState syncState);
	}
}
