using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class FolderHierarchySyncState : SyncState, IFolderHierarchySyncState, ISyncState
	{
		protected FolderHierarchySyncState(SyncStateStorage syncStateStorage, StoreObject storeObject, SyncStateMetadata syncStateMetadata, SyncStateInfo syncStateInfo, bool newSyncState, ISyncLogger syncLogger = null) : base(syncStateStorage, storeObject, syncStateMetadata, syncStateInfo, newSyncState, syncLogger)
		{
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

		public static void RegisterCustomDataVersioningHandler(FolderHierarchySyncState.HandleCustomDataVersioningDelegate handleCustomVersioning)
		{
			FolderHierarchySyncState.handleCustomDataVersioning = handleCustomVersioning;
		}

		public FolderHierarchySync GetFolderHierarchySync()
		{
			return this.GetFolderHierarchySync(null);
		}

		public FolderHierarchySync GetFolderHierarchySync(ChangeTrackingDelegate changeTrackingDelegate)
		{
			base.CheckDisposed("GetFolderHierarchySync");
			this.syncLogger.Information<int>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "FolderHierarchySyncState::GetFolderHierarchySync. Hashcode = {0}", this.GetHashCode());
			MailboxSession mailboxSession = base.StoreObject.Session as MailboxSession;
			if (mailboxSession == null)
			{
				throw new NotSupportedException();
			}
			return new FolderHierarchySync(mailboxSession, this, changeTrackingDelegate);
		}

		public override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<FolderHierarchySyncState>(this);
		}

		internal static FolderHierarchySyncState CreateSyncState(SyncStateStorage syncStateStorage, Folder syncStateParentFolder, ISyncLogger syncLogger = null)
		{
			if (syncLogger == null)
			{
				syncLogger = TracingLogger.Singleton;
			}
			SyncStateInfo syncStateInfo = new FolderHierarchySyncStateInfo();
			StoreObject storeObject = SyncState.CreateSyncStateStoreObject(syncStateStorage, syncStateInfo, syncStateParentFolder, null, null, syncLogger);
			if (syncStateStorage.DeviceMetadata.TryRemove(syncStateInfo.UniqueName, syncLogger) != null)
			{
				syncLogger.TraceDebug<DeviceIdentity, string>(ExTraceGlobals.SyncTracer, 0L, "[FolderHierarchySyncState.CreateSyncState] Removed stale cached sync state metadata for device {0}, sync state {1}", syncStateStorage.DeviceMetadata.Id, syncStateInfo.UniqueName);
			}
			SyncStateMetadata syncStateMetadata = (storeObject is Item) ? new SyncStateMetadata(syncStateStorage.DeviceMetadata, syncStateInfo.UniqueName, syncStateStorage.SaveOnDirectItems ? null : storeObject.ParentId, storeObject.Id.ObjectId) : new SyncStateMetadata(syncStateStorage.DeviceMetadata, syncStateInfo.UniqueName, storeObject.Id.ObjectId, null);
			return new FolderHierarchySyncState(syncStateStorage, storeObject, syncStateMetadata, syncStateInfo, true, syncLogger);
		}

		internal static FolderHierarchySyncState GetSyncState(SyncStateStorage syncStateStorage, Folder syncStateParentFolder, ISyncLogger syncLogger = null)
		{
			return FolderHierarchySyncState.GetSyncState(syncStateStorage, syncStateParentFolder, null, syncLogger);
		}

		internal static FolderHierarchySyncState GetSyncState(SyncStateStorage syncStateStorage, Folder syncStateParentFolder, StoreObjectId storeObjectId, ISyncLogger syncLogger = null)
		{
			SyncStateInfo syncStateInfo = new FolderHierarchySyncStateInfo();
			StoreObject syncStateStoreObject = SyncState.GetSyncStateStoreObject(syncStateStorage, syncStateParentFolder, syncStateInfo, syncLogger, new PropertyDefinition[0]);
			if (syncStateStoreObject == null)
			{
				return null;
			}
			SyncStateMetadata syncState = syncStateStorage.DeviceMetadata.GetSyncState(syncStateParentFolder.Session as MailboxSession, syncStateInfo.UniqueName, syncLogger);
			return new FolderHierarchySyncState(syncStateStorage, syncStateStoreObject, syncState, syncStateInfo, false, syncLogger);
		}

		protected override void Load(bool reloadFromBackend, params PropertyDefinition[] additionalPropsToLoad)
		{
			base.Load(reloadFromBackend, additionalPropsToLoad);
			FolderHierarchySyncState.handleCustomDataVersioning(this);
		}

		private static FolderHierarchySyncState.HandleCustomDataVersioningDelegate handleCustomDataVersioning = delegate(FolderHierarchySyncState syncState)
		{
			if (syncState.CustomVersion != null)
			{
				syncState.HandleCorruptSyncState();
			}
		};

		public delegate void HandleCustomDataVersioningDelegate(FolderHierarchySyncState syncState);
	}
}
