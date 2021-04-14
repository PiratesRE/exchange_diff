using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class SyncStateRootStorage : DisposeTrackableBase
	{
		public StoreObjectId FolderId
		{
			get
			{
				base.CheckDisposed();
				return this.syncStateStorage.FolderId;
			}
		}

		private SyncStateRootStorage(SyncStateStorage syncStateStorage)
		{
			if (syncStateStorage == null)
			{
				throw new ArgumentNullException("syncStateStorage");
			}
			this.syncStateStorage = syncStateStorage;
		}

		public static SyncStateRootStorage GetOrCreateSyncStateRootStorage(MailboxSession mailboxSession, string protocol, ISyncLogger syncLogger = null)
		{
			if (mailboxSession == null)
			{
				throw new ArgumentNullException("mailboxSession");
			}
			if (protocol == null)
			{
				throw new ArgumentNullException("protocol");
			}
			if (syncLogger == null)
			{
				syncLogger = TracingLogger.Singleton;
			}
			string protocol2 = protocol + "Root";
			DeviceIdentity deviceIdentity = new DeviceIdentity("RootDeviceId", "RootDeviceType", protocol2);
			SyncStateStorage syncStateStorage = SyncStateStorage.Bind(mailboxSession, deviceIdentity, syncLogger);
			if (syncStateStorage == null)
			{
				syncStateStorage = SyncStateStorage.Create(mailboxSession, deviceIdentity, StateStorageFeatures.ContentState, syncLogger);
			}
			return new SyncStateRootStorage(syncStateStorage);
		}

		public CustomSyncState CreateCustomSyncState(SyncStateInfo syncStateInfo)
		{
			base.CheckDisposed();
			return this.syncStateStorage.CreateCustomSyncState(syncStateInfo);
		}

		public AggregateOperationResult DeleteCustomSyncState(SyncStateInfo syncStateInfo)
		{
			base.CheckDisposed();
			return this.syncStateStorage.DeleteCustomSyncState(syncStateInfo);
		}

		public CustomSyncState GetCustomSyncState(SyncStateInfo syncStateInfo)
		{
			base.CheckDisposed();
			return this.syncStateStorage.GetCustomSyncState(syncStateInfo, new PropertyDefinition[0]);
		}

		protected override void InternalDispose(bool isDisposing)
		{
			if (isDisposing && this.syncStateStorage != null)
			{
				this.syncStateStorage.Dispose();
				this.syncStateStorage = null;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<SyncStateRootStorage>(this);
		}

		private const string RootProtocolPostFix = "Root";

		private const string RootDeviceType = "RootDeviceType";

		private const string RootDeviceId = "RootDeviceId";

		private SyncStateStorage syncStateStorage;
	}
}
