using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ContentAggregation;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Exceptions;
using Microsoft.Exchange.Transport.Sync.Common.Logging;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;
using Microsoft.Exchange.Transport.Sync.Worker.Throttling;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class StateStorage : DisposeTrackableBase, IStateStorage, INativeStateStorage, ISimpleStateStorage, IDisposeTrackable, IDisposable
	{
		internal StateStorage(MailboxSession mailboxSession, ISyncWorkerData subscription, SyncLogSession syncLogSession, EventHandler<RoundtripCompleteEventArgs> roundtripComplete)
		{
			SyncUtilities.ThrowIfArgumentNull("mailboxSession", mailboxSession);
			SyncUtilities.ThrowIfArgumentNull("subscription", subscription);
			SyncUtilities.ThrowIfArgumentNull("syncLogSession", syncLogSession);
			this.subscriptionGuid = subscription.SubscriptionGuid;
			this.mailboxSession = mailboxSession;
			this.syncLogSession = syncLogSession;
			string deviceId = SubscriptionManager.GenerateDeviceId(subscription.SubscriptionGuid);
			bool flag = false;
			try
			{
				this.syncStateStorage = SyncStoreLoadManager.SyncStateStorageBind(mailboxSession, SubscriptionManager.Protocol, subscription.SubscriptionType.ToString(), deviceId, roundtripComplete);
				if (this.syncStateStorage == null)
				{
					this.syncStateStorage = SyncStoreLoadManager.SyncStateStorageCreate(mailboxSession, SubscriptionManager.Protocol, subscription.SubscriptionType.ToString(), deviceId, StateStorageFeatures.ContentState, roundtripComplete);
				}
				if (this.syncStateStorage == null)
				{
					syncLogSession.LogError((TSLID)1488UL, StateStorage.Tracer, "Constructor: Could not create XSO SyncStateStorage.", new object[0]);
					throw new ArgumentNullException("syncStateStorage");
				}
				StateStorage.EnsureSyncStateTypesRegistered();
				this.LoadCustomSyncState(roundtripComplete);
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					if (this.storageCustomSyncState != null)
					{
						this.storageCustomSyncState.Dispose();
						this.storageCustomSyncState = null;
					}
					if (this.syncStateStorage != null)
					{
						this.syncStateStorage.Dispose();
						this.syncStateStorage = null;
					}
				}
			}
		}

		public SyncStateStorage SyncStateStorage
		{
			get
			{
				return this.syncStateStorage;
			}
		}

		public bool IsDirty
		{
			get
			{
				return this.customSyncState.IsDirty;
			}
		}

		public SyncProgress SyncProgress
		{
			get
			{
				return this.syncProgress;
			}
		}

		public bool ForceRecoverySyncNext
		{
			get
			{
				return this.forceRecoverySyncNext;
			}
		}

		public bool InitialSyncDone
		{
			get
			{
				return this.customSyncState.InitialSyncDone;
			}
		}

		public static bool TryDelete(MailboxSession mailboxSession, ISyncWorkerData subscription, EventHandler<RoundtripCompleteEventArgs> roundtripComplete)
		{
			bool result;
			try
			{
				SyncStoreLoadManager.SyncStateStorageDelete(mailboxSession, SubscriptionManager.Protocol, subscription.SubscriptionType.ToString(), SubscriptionManager.GenerateDeviceId(subscription.SubscriptionGuid), roundtripComplete);
				result = true;
			}
			catch (StoragePermanentException)
			{
				result = false;
			}
			catch (StorageTransientException)
			{
				result = false;
			}
			catch (SerializationException)
			{
				result = false;
			}
			catch (InvalidOperationException)
			{
				result = false;
			}
			catch (SyncStoreUnhealthyException)
			{
				result = false;
			}
			return result;
		}

		public void MarkInitialSyncDone()
		{
			this.customSyncState.MarkInitialSyncDone();
		}

		public void SetSyncProgress(SyncProgress progress)
		{
			this.syncProgress = progress;
		}

		public void SetForceRecoverySyncNext(bool forceRecoverySyncNext)
		{
			this.forceRecoverySyncNext = forceRecoverySyncNext;
		}

		public void AddProperty(string property, string value)
		{
			this.customSyncState.AddProperty(property, value);
		}

		public bool TryGetPropertyValue(string property, out string value)
		{
			return this.customSyncState.TryGetPropertyValue(property, out value);
		}

		public bool TryRemoveProperty(string property)
		{
			return this.customSyncState.TryRemoveProperty(property);
		}

		public void ChangePropertyValue(string property, string value)
		{
			this.customSyncState.ChangePropertyValue(property, value);
		}

		public bool ContainsProperty(string property)
		{
			return this.customSyncState.ContainsProperty(property);
		}

		public bool ContainsItem(string cloudId)
		{
			return this.customSyncState.ContainsItem(cloudId);
		}

		public bool ContainsFailedItem(string cloudId)
		{
			return this.customSyncState.ContainsFailedItem(cloudId);
		}

		public bool ContainsFolder(StoreObjectId nativeId)
		{
			return this.customSyncState.ContainsFolder(nativeId);
		}

		public bool ContainsFolder(string cloudId)
		{
			return this.customSyncState.ContainsFolder(cloudId);
		}

		public bool ContainsFailedFolder(string cloudId)
		{
			return this.customSyncState.ContainsFailedFolder(cloudId);
		}

		public bool ContainsItem(StoreObjectId nativeId)
		{
			return this.customSyncState.ContainsItem(nativeId);
		}

		public bool TryAddItem(string cloudId, string cloudFolderId, StoreObjectId nativeId, byte[] changeKey, StoreObjectId nativeFolderId, string cloudVersion, Dictionary<string, string> itemProperties)
		{
			return this.customSyncState.TryAddItem(cloudId, cloudFolderId, nativeId, changeKey, nativeFolderId, cloudVersion, itemProperties);
		}

		public bool TryAddFailedItem(string cloudId, string cloudFolderId)
		{
			return this.customSyncState.TryAddFailedItem(cloudId, cloudFolderId);
		}

		public bool TryFindItem(string cloudId, out string cloudFolderId, out string cloudVersion)
		{
			Dictionary<string, string> dictionary;
			return this.TryFindItem(cloudId, out cloudFolderId, out cloudVersion, out dictionary);
		}

		public bool TryFindItem(string cloudId, out string cloudFolderId, out string cloudVersion, out Dictionary<string, string> itemProperties)
		{
			StoreObjectId storeObjectId;
			byte[] array;
			StoreObjectId storeObjectId2;
			return this.TryFindItem(cloudId, out cloudFolderId, out storeObjectId, out array, out storeObjectId2, out cloudVersion, out itemProperties);
		}

		public bool TryFindItem(string cloudId, out string cloudFolderId, out StoreObjectId nativeId, out byte[] changeKey, out StoreObjectId nativeFolderId, out string cloudVersion, out Dictionary<string, string> itemProperties)
		{
			return this.customSyncState.TryFindItem(cloudId, out cloudFolderId, out nativeId, out changeKey, out nativeFolderId, out cloudVersion, out itemProperties);
		}

		public bool TryFindItem(StoreObjectId nativeId, out string cloudId, out string cloudFolderId, out byte[] changeKey, out StoreObjectId nativeFolderId, out string cloudVersion, out Dictionary<string, string> itemProperties)
		{
			return this.customSyncState.TryFindItem(nativeId, out cloudId, out cloudFolderId, out changeKey, out nativeFolderId, out cloudVersion, out itemProperties);
		}

		public bool TryUpdateItem(StoreObjectId nativeId, byte[] changeKey, string cloudVersion, Dictionary<string, string> itemProperties)
		{
			return this.customSyncState.TryUpdateItem(nativeId, changeKey, cloudVersion, itemProperties);
		}

		public bool TryRemoveItem(string cloudId)
		{
			return this.customSyncState.TryRemoveItem(cloudId);
		}

		public bool TryRemoveItem(StoreObjectId nativeId)
		{
			return this.customSyncState.TryRemoveItem(nativeId);
		}

		public bool TryRemoveFailedItem(string cloudId)
		{
			return this.customSyncState.TryRemoveFailedItem(cloudId);
		}

		public bool TryAddFolder(bool isInbox, string cloudId, string cloudFolderId, StoreObjectId nativeId, byte[] changeKey, StoreObjectId nativeFolderId, string cloudVersion, Dictionary<string, string> folderProperties)
		{
			return this.customSyncState.TryAddFolder(isInbox, cloudId, cloudFolderId, nativeId, changeKey, nativeFolderId, cloudVersion, folderProperties);
		}

		public bool TryAddFailedFolder(string cloudId, string cloudFolderId)
		{
			return this.customSyncState.TryAddFailedFolder(cloudId, cloudFolderId);
		}

		public bool TryFindFolder(string cloudId, out string cloudFolderId, out string cloudVersion)
		{
			Dictionary<string, string> dictionary;
			return this.TryFindFolder(cloudId, out cloudFolderId, out cloudVersion, out dictionary);
		}

		public bool TryFindFolder(string cloudId, out string cloudFolderId, out string cloudVersion, out Dictionary<string, string> folderProperties)
		{
			StoreObjectId storeObjectId;
			byte[] array;
			StoreObjectId storeObjectId2;
			return this.TryFindFolder(cloudId, out cloudFolderId, out storeObjectId, out array, out storeObjectId2, out cloudVersion, out folderProperties);
		}

		public bool TryFindFolder(string cloudId, out string cloudFolderId, out StoreObjectId nativeId, out byte[] changeKey, out StoreObjectId nativeFolderId, out string cloudVersion, out Dictionary<string, string> folderProperties)
		{
			return this.customSyncState.TryFindFolder(cloudId, out cloudFolderId, out nativeId, out changeKey, out nativeFolderId, out cloudVersion, out folderProperties);
		}

		public bool TryFindFolder(StoreObjectId nativeId, out string cloudId, out string cloudFolderId, out byte[] changeKey, out StoreObjectId nativeFolderId, out string cloudVersion, out Dictionary<string, string> folderProperties)
		{
			return this.customSyncState.TryFindFolder(nativeId, out cloudId, out cloudFolderId, out changeKey, out nativeFolderId, out cloudVersion, out folderProperties);
		}

		public bool TryUpdateFolder(ISyncWorkerData subscription, string cloudId, string newCloudId, string cloudVersion)
		{
			if ((subscription.SyncQuirks & SyncQuirks.AllowDirectCloudFolderUpdates) == SyncQuirks.None)
			{
				throw new InvalidOperationException("Direct update of folders by cloud ID is not allowed for this subscription. You should not be using this API.");
			}
			string text;
			StoreObjectId nativeId;
			byte[] changeKey;
			StoreObjectId storeObjectId;
			string text2;
			Dictionary<string, string> folderProperties;
			if (!this.TryFindFolder(cloudId, out text, out nativeId, out changeKey, out storeObjectId, out text2, out folderProperties))
			{
				return false;
			}
			bool isInbox = false;
			return this.TryUpdateFolder(isInbox, nativeId, null, cloudId, newCloudId, null, changeKey, null, cloudVersion, folderProperties);
		}

		public bool TryUpdateFolder(bool isInbox, StoreObjectId nativeId, StoreObjectId newNativeId, string cloudId, string newCloudId, string newCloudFolderId, byte[] changeKey, StoreObjectId newNativeFolderId, string cloudVersion, Dictionary<string, string> folderProperties)
		{
			return this.customSyncState.TryUpdateFolder(isInbox, nativeId, newNativeId, cloudId, newCloudId, newCloudFolderId, changeKey, newNativeFolderId, cloudVersion, folderProperties);
		}

		public bool TryUpdateFolder(bool isInbox, StoreObjectId nativeId, StoreObjectId newNativeId)
		{
			return this.customSyncState.TryUpdateFolder(isInbox, nativeId, newNativeId);
		}

		public bool TryRemoveFolder(string cloudId)
		{
			return this.customSyncState.TryRemoveFolder(cloudId);
		}

		public bool TryRemoveFailedFolder(string cloudId)
		{
			return this.customSyncState.TryRemoveFailedFolder(cloudId);
		}

		public bool TryRemoveFolder(StoreObjectId nativeId)
		{
			return this.customSyncState.TryRemoveFolder(nativeId);
		}

		public IEnumerator<string> GetCloudItemEnumerator()
		{
			return this.customSyncState.GetCloudItemEnumerator();
		}

		public IEnumerator<string> GetFailedCloudItemEnumerator()
		{
			return this.customSyncState.GetFailedCloudItemEnumerator();
		}

		public IEnumerator<string> GetCloudFolderEnumerator()
		{
			return this.customSyncState.GetCloudFolderEnumerator();
		}

		public IEnumerator<StoreObjectId> GetNativeItemEnumerator()
		{
			return this.customSyncState.GetNativeItemEnumerator();
		}

		public IEnumerator<StoreObjectId> GetNativeFolderEnumerator()
		{
			return this.customSyncState.GetNativeFolderEnumerator();
		}

		public IEnumerator<string> GetCloudItemFilteredByCloudFolderIdEnumerator(string cloudFolderId)
		{
			return this.customSyncState.GetCloudItemFilteredByCloudFolderIdEnumerator(cloudFolderId);
		}

		public IEnumerator<string> GetCloudFolderFilteredByCloudFolderIdEnumerator(string cloudFolderId)
		{
			return this.customSyncState.GetCloudFolderFilteredByCloudFolderIdEnumerator(cloudFolderId);
		}

		public IEnumerator<StoreObjectId> GetNativeItemFilteredByNativeFolderIdEnumerator(StoreObjectId nativeFolderId)
		{
			return this.customSyncState.GetNativeItemFilteredByNativeFolderIdEnumerator(nativeFolderId);
		}

		public IEnumerator<StoreObjectId> GetNativeFolderFilteredByNativeFolderIdEnumerator(StoreObjectId nativeFolderId)
		{
			return this.customSyncState.GetNativeFolderFilteredByNativeFolderIdEnumerator(nativeFolderId);
		}

		public IEnumerator<string> GetFailedCloudItemFilteredByCloudFolderIdEnumerator(string cloudFolderId)
		{
			return this.customSyncState.GetFailedCloudItemFilteredByCloudFolderIdEnumerator(cloudFolderId);
		}

		public bool TryUpdateItemCloudVersion(string cloudId, string cloudVersion)
		{
			return this.customSyncState.TryUpdateItemCloudVersion(cloudId, cloudVersion);
		}

		public bool TryUpdateFolderCloudVersion(string cloudId, string cloudVersion)
		{
			return this.customSyncState.TryUpdateFolderCloudVersion(cloudId, cloudVersion);
		}

		public bool ShouldPromoteItemTransientException(string cloudId, SyncTransientException exception)
		{
			return this.customSyncState.ShouldPromoteItemTransientException(cloudId, exception);
		}

		public bool ShouldPromoteItemTransientException(StoreObjectId nativeId, SyncTransientException exception)
		{
			return this.customSyncState.ShouldPromoteItemTransientException(nativeId, exception);
		}

		public bool ShouldPromoteFolderTransientException(string cloudId, SyncTransientException exception)
		{
			return this.customSyncState.ShouldPromoteFolderTransientException(cloudId, exception);
		}

		public bool ShouldPromoteFolderTransientException(StoreObjectId nativeId, SyncTransientException exception)
		{
			return this.customSyncState.ShouldPromoteFolderTransientException(nativeId, exception);
		}

		public Exception Commit(bool commitState, MailboxSession mailboxSession, EventHandler<RoundtripCompleteEventArgs> roundtripComplete)
		{
			Exception ex = null;
			try
			{
				if (this.requiresReload)
				{
					this.syncLogSession.LogDebugging((TSLID)1489UL, StateStorage.Tracer, "Loading State Storage as reload was set.", new object[0]);
					this.storageCustomSyncState.KeepCachedDataOnReload = true;
					this.storageCustomSyncState.Load();
					this.requiresReload = false;
					this.storageCustomSyncState.KeepCachedDataOnReload = false;
				}
				if (this.IsDirty && commitState)
				{
					this.storageCustomSyncState.Commit();
					long lastUncompressedSize = this.storageCustomSyncState.GetLastUncompressedSize();
					UncompressedSyncStateSizeExceededException innerException;
					if (StateStorage.mappingTableSizeChecker.IsUncompressedSyncStateExceededBounds(this.subscriptionGuid, StateStorage.UnifiedStateTypeInfo.UniqueName, lastUncompressedSize, out innerException))
					{
						ex = SyncPermanentException.CreateOperationLevelException(DetailedAggregationStatus.SyncStateSizeError, innerException);
					}
					else
					{
						this.syncLogSession.LogDebugging((TSLID)1449UL, StateStorage.Tracer, "MappingTable syncState size in memory {0}", new object[]
						{
							lastUncompressedSize
						});
						this.storageCustomSyncState.Dispose();
						this.storageCustomSyncState = null;
					}
				}
			}
			catch (StoragePermanentException ex2)
			{
				CompressedSyncStateSizeExceededException innerException2;
				if (StateStorage.mappingTableSizeChecker.IsCompressedSyncStateExceededBounds(this.subscriptionGuid, StateStorage.UnifiedStateTypeInfo.UniqueName, ex2, out innerException2))
				{
					ex = SyncPermanentException.CreateOperationLevelException(DetailedAggregationStatus.SyncStateSizeError, innerException2);
				}
				else
				{
					ex = SyncTransientException.CreateOperationLevelException(DetailedAggregationStatus.CommunicationError, ex2, true);
				}
			}
			catch (StorageTransientException ex3)
			{
				this.requiresReload = true;
				this.syncLogSession.LogDebugging((TSLID)977UL, StateStorage.Tracer, "Marking state storage as requiring reload before commit as a transient exception was hit when reloading: {0}", new object[]
				{
					ex3
				});
				ex = SyncTransientException.CreateOperationLevelException(DetailedAggregationStatus.CommunicationError, ex3);
			}
			catch (SerializationException innerException3)
			{
				ex = SyncTransientException.CreateOperationLevelException(DetailedAggregationStatus.CommunicationError, innerException3);
			}
			catch (InvalidOperationException innerException4)
			{
				ex = SyncTransientException.CreateOperationLevelException(DetailedAggregationStatus.CommunicationError, innerException4);
			}
			catch (SyncStoreUnhealthyException ex4)
			{
				this.requiresReload = true;
				this.syncLogSession.LogDebugging((TSLID)1354UL, StateStorage.Tracer, "Marking state storage as requiring reload before commit as a store unhealthy exception was hit when reloading: {0}", new object[]
				{
					ex4
				});
				ex = SyncTransientException.CreateOperationLevelException(DetailedAggregationStatus.CommunicationError, ex4);
			}
			if (ex != null)
			{
				return ex;
			}
			try
			{
				this.SaveSyncProgress(mailboxSession, roundtripComplete);
			}
			catch (StoragePermanentException innerException5)
			{
				ex = SyncTransientException.CreateOperationLevelException(DetailedAggregationStatus.CommunicationError, innerException5, true);
			}
			catch (StorageTransientException innerException6)
			{
				ex = SyncTransientException.CreateOperationLevelException(DetailedAggregationStatus.CommunicationError, innerException6);
			}
			catch (SyncStoreUnhealthyException innerException7)
			{
				ex = SyncTransientException.CreateOperationLevelException(DetailedAggregationStatus.CommunicationError, innerException7);
			}
			return ex;
		}

		public void ReloadForRetry(EventHandler<RoundtripCompleteEventArgs> roundtripComplete)
		{
			if (this.storageCustomSyncState != null)
			{
				return;
			}
			this.LoadCustomSyncState(roundtripComplete);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				if (this.syncStateStorage != null)
				{
					this.syncStateStorage.Dispose();
					this.syncStateStorage = null;
				}
				if (this.storageCustomSyncState != null)
				{
					this.storageCustomSyncState.Dispose();
					this.storageCustomSyncState = null;
				}
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<StateStorage>(this);
		}

		private static void EnsureSyncStateTypesRegistered()
		{
			if (!StateStorage.initialized)
			{
				lock (StateStorage.globalLock)
				{
					if (!StateStorage.initialized)
					{
						SyncStateTypeFactory.GetInstance().RegisterBuilder(new UnifiedCustomSyncState());
						SyncStateTypeFactory.GetInstance().RegisterBuilder(new UnifiedCustomSyncStateItem());
						StateStorage.initialized = true;
					}
				}
			}
		}

		private void LoadSyncProgress(MailboxSession mailboxSession, EventHandler<RoundtripCompleteEventArgs> roundtripComplete)
		{
			PropertyDefinition[] properties = new PropertyDefinition[]
			{
				FolderSchema.AggregationSyncProgress
			};
			using (Folder folder = SyncStoreLoadManager.FolderBind(mailboxSession, this.syncStateStorage.FolderId, properties, roundtripComplete))
			{
				int? num = folder.TryGetProperty(FolderSchema.AggregationSyncProgress) as int?;
				if (num != null)
				{
					this.lastSavedSyncProgress = (SyncProgress)num.Value;
				}
				else
				{
					this.lastSavedSyncProgress = SyncProgress.None;
				}
			}
			this.syncProgress = this.lastSavedSyncProgress;
		}

		private void SaveSyncProgress(MailboxSession mailboxSession, EventHandler<RoundtripCompleteEventArgs> roundtripComplete)
		{
			if (this.lastSavedSyncProgress == this.syncProgress)
			{
				this.syncLogSession.LogDebugging((TSLID)299UL, "Skipping save of SyncProgress as it is same as earlier: {0}", new object[]
				{
					this.syncProgress
				});
				return;
			}
			PropertyDefinition[] properties = new PropertyDefinition[]
			{
				FolderSchema.AggregationSyncProgress
			};
			using (Folder folder = SyncStoreLoadManager.FolderBind(mailboxSession, this.syncStateStorage.FolderId, properties, roundtripComplete))
			{
				folder[FolderSchema.AggregationSyncProgress] = (int)this.syncProgress;
				SyncStoreLoadManager.FolderSave(folder, mailboxSession, roundtripComplete);
				this.lastSavedSyncProgress = this.syncProgress;
			}
		}

		private void LoadCustomSyncState(EventHandler<RoundtripCompleteEventArgs> roundtripComplete)
		{
			this.storageCustomSyncState = SyncStoreLoadManager.SyncStateStorageGetCustomSyncState(this.mailboxSession.MdbGuid, this.syncStateStorage, StateStorage.UnifiedStateTypeInfo, roundtripComplete);
			if (this.storageCustomSyncState == null)
			{
				this.syncLogSession.LogVerbose((TSLID)978UL, StateStorage.Tracer, "There is no custom sync state. We'll create one.", new object[0]);
				this.storageCustomSyncState = SyncStoreLoadManager.SyncStateStorageCreateCustomSyncState(this.mailboxSession.MdbGuid, this.syncStateStorage, StateStorage.UnifiedStateTypeInfo, roundtripComplete);
			}
			else
			{
				this.LoadSyncProgress(this.mailboxSession, roundtripComplete);
			}
			this.customSyncState = (this.storageCustomSyncState[StateStorage.UnifiedStateTypeInfo.UniqueName] as UnifiedCustomSyncState);
			if (this.customSyncState == null)
			{
				this.customSyncState = new UnifiedCustomSyncState();
				this.storageCustomSyncState[StateStorage.UnifiedStateTypeInfo.UniqueName] = this.customSyncState;
			}
		}

		internal static readonly Trace Tracer = ExTraceGlobals.StateStorageTracer;

		private static readonly SyncStateSizeLimitChecker mappingTableSizeChecker = new SyncStateSizeLimitChecker(Convert.ToInt64(AggregationConfiguration.Instance.MaxMappingTableSizeInMemory.ToBytes()));

		private static readonly SyncStateInfo UnifiedStateTypeInfo = new UnifiedCustomSyncStateInfo();

		private static object globalLock = new object();

		private static bool initialized;

		private readonly Guid subscriptionGuid;

		private SyncStateStorage syncStateStorage;

		private CustomSyncState storageCustomSyncState;

		private UnifiedCustomSyncState customSyncState;

		private SyncProgress syncProgress;

		private SyncProgress lastSavedSyncProgress;

		private bool forceRecoverySyncNext;

		private MailboxSession mailboxSession;

		private SyncLogSession syncLogSession;

		private bool requiresReload;
	}
}
