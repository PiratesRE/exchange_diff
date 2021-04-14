using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class CustomSyncState : SyncState
	{
		protected CustomSyncState(SyncStateStorage syncStateStorage, StoreObject storeObject, SyncStateMetadata syncStateMetadata, SyncStateInfo syncStateInfo, bool syncStateIsNew) : base(syncStateStorage, storeObject, syncStateMetadata, syncStateInfo, syncStateIsNew, null)
		{
		}

		public override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<CustomSyncState>(this);
		}

		internal static CustomSyncState CreateSyncState(SyncStateStorage syncStateStorage, SyncStateInfo syncStateInfo, Folder syncStateParentFolder, ISyncLogger syncLogger = null)
		{
			if (syncLogger == null)
			{
				syncLogger = TracingLogger.Singleton;
			}
			StoreObject storeObject = SyncState.CreateSyncStateStoreObject(syncStateStorage, syncStateInfo, syncStateParentFolder, null, null, syncLogger);
			if (syncStateStorage.DeviceMetadata.TryRemove(syncStateInfo.UniqueName, syncLogger) != null)
			{
				syncLogger.TraceDebug<DeviceIdentity, string>(ExTraceGlobals.SyncTracer, 0L, "[CustomSyncState.CreateSyncState] Removed stale cached sync state metadata for device {0}, sync state {1}", syncStateStorage.DeviceMetadata.Id, syncStateInfo.UniqueName);
			}
			SyncStateMetadata syncStateMetadata = (storeObject is Item) ? new SyncStateMetadata(syncStateStorage.DeviceMetadata, syncStateInfo.UniqueName, syncStateStorage.SaveOnDirectItems ? null : storeObject.ParentId, storeObject.Id.ObjectId) : new SyncStateMetadata(syncStateStorage.DeviceMetadata, syncStateInfo.UniqueName, storeObject.Id.ObjectId, null);
			return new CustomSyncState(syncStateStorage, storeObject, syncStateMetadata, syncStateInfo, true);
		}

		internal static CustomSyncState GetSyncState(SyncStateStorage syncStateStorage, Folder syncStateParentFolder, SyncStateInfo syncStateInfo, ISyncLogger syncLogger = null)
		{
			return CustomSyncState.GetSyncState(syncStateStorage, syncStateParentFolder, syncStateInfo, syncLogger, null);
		}

		internal static CustomSyncState GetSyncState(SyncStateStorage syncStateStorage, Folder syncStateParentFolder, SyncStateInfo syncStateInfo, ISyncLogger syncLogger, params PropertyDefinition[] propertiesToFetch)
		{
			if (syncLogger == null)
			{
				syncLogger = TracingLogger.Singleton;
			}
			SyncStateMetadata syncStateMetadata = null;
			StoreObject syncStateStoreObject = SyncState.GetSyncStateStoreObject(syncStateStorage, syncStateParentFolder, syncStateInfo, syncLogger, out syncStateMetadata, propertiesToFetch);
			if (syncStateStoreObject == null)
			{
				return null;
			}
			return new CustomSyncState(syncStateStorage, syncStateStoreObject, syncStateMetadata, syncStateInfo, false);
		}
	}
}
