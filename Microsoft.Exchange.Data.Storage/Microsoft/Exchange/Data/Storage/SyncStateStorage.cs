using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class SyncStateStorage : IDisposeTrackable, IDisposable
	{
		private SyncStateStorage(Folder folder, DeviceSyncStateMetadata deviceMetadata, ISyncLogger syncLogger = null)
		{
			this.syncLogger = (syncLogger ?? TracingLogger.Singleton);
			StorageGlobals.TraceConstructIDisposable(this);
			this.disposeTracker = this.GetDisposeTracker();
			this.folder = folder;
			this.DeviceMetadata = deviceMetadata;
			if (this.folder.DisplayName != this.DeviceMetadata.Id.CompositeKey)
			{
				throw new ArgumentException(string.Format("[SyncStateStorage.ctor] The folder name '{0}' and the metadata name '{1}' should match.", this.folder.DisplayName, deviceMetadata.Id));
			}
			this.creationTime = (ExDateTime)this.folder.TryGetProperty(StoreObjectSchema.CreationTime);
			long num = 0L;
			long.TryParse(this.folder.GetValueOrDefault<string>(SyncStateStorage.airsyncLockProp, "0"), NumberStyles.Any, CultureInfo.InvariantCulture, out num);
			this.airsyncLock = ((num == 0L) ? ExDateTime.MinValue : ExDateTime.FromBinary(num));
			this.syncRootFolderId = folder.ParentId;
		}

		public bool SaveOnDirectItems { get; set; }

		public DeviceSyncStateMetadata DeviceMetadata { get; private set; }

		public DeviceIdentity DeviceIdentity
		{
			get
			{
				return this.DeviceMetadata.Id;
			}
		}

		public ExDateTime CreationTime
		{
			get
			{
				this.CheckDisposed("CreationTime");
				return this.creationTime;
			}
		}

		public StoreObjectId FolderId
		{
			get
			{
				this.CheckDisposed("FolderId");
				return this.folder.Id.ObjectId;
			}
		}

		public StoreObjectId SyncRootFolderId
		{
			get
			{
				this.CheckDisposed("SyncRootFolderId");
				return this.syncRootFolderId;
			}
		}

		public bool SyncAllowedForDevice
		{
			get
			{
				this.CheckDisposed("SyncAllowedForDevice");
				return ExDateTime.Compare(ExDateTime.UtcNow, this.airsyncLock) >= 0;
			}
		}

		public IExchangePrincipal MailboxOwner
		{
			get
			{
				this.CheckDisposed("MailboxOwner");
				MailboxSession mailboxSession = this.folder.Session as MailboxSession;
				if (mailboxSession == null)
				{
					throw new NotSupportedException();
				}
				return mailboxSession.MailboxOwner;
			}
		}

		public PooledMemoryStream CompressedMemoryStream
		{
			get
			{
				this.CheckDisposed("CompressedMemoryStream");
				if (this.compressedMemoryStream == null)
				{
					this.compressedMemoryStream = new PooledMemoryStream(102400);
				}
				return this.compressedMemoryStream;
			}
		}

		public static SyncStateStorage Bind(MailboxSession mailboxSession, DeviceIdentity deviceIdentity, ISyncLogger syncLogger = null)
		{
			if (syncLogger == null)
			{
				syncLogger = TracingLogger.Singleton;
			}
			ArgumentValidator.ThrowIfNull("mailboxSession", mailboxSession);
			SyncStateTypeFactory.GetInstance().RegisterInternalBuilders();
			UserSyncStateMetadata userSyncStateMetadata = UserSyncStateMetadataCache.Singleton.Get(mailboxSession, syncLogger);
			DeviceSyncStateMetadata device = userSyncStateMetadata.GetDevice(mailboxSession, deviceIdentity, syncLogger);
			if (device != null)
			{
				return SyncStateStorage.GetSyncStateStorage(mailboxSession, device, syncLogger);
			}
			return null;
		}

		public static SyncStateStorage Create(MailboxSession mailboxSession, DeviceIdentity deviceIdentity, StateStorageFeatures features, ISyncLogger syncLogger = null)
		{
			return SyncStateStorage.Create(mailboxSession, deviceIdentity, features, false, syncLogger);
		}

		public static SyncStateStorage Create(MailboxSession mailboxSession, DeviceIdentity deviceIdentity, StateStorageFeatures features, bool onlySetPropsIfAlreadyExists, ISyncLogger syncLogger = null)
		{
			if (syncLogger == null)
			{
				syncLogger = TracingLogger.Singleton;
			}
			EnumValidator.ThrowIfInvalid<StateStorageFeatures>(features, "features");
			SyncStateTypeFactory.GetInstance().RegisterInternalBuilders();
			UserSyncStateMetadata userSyncStateMetadata = UserSyncStateMetadataCache.Singleton.Get(mailboxSession, syncLogger);
			DeviceSyncStateMetadata deviceSyncStateMetadata = userSyncStateMetadata.GetDevice(mailboxSession, deviceIdentity, syncLogger);
			SyncStateStorage syncStateStorage = (deviceSyncStateMetadata == null) ? null : SyncStateStorage.GetSyncStateStorage(mailboxSession, deviceSyncStateMetadata, syncLogger);
			if (syncStateStorage == null || onlySetPropsIfAlreadyExists)
			{
				Folder folder = null;
				SyncStateStorage syncStateStorage2 = null;
				bool flag = false;
				try
				{
					folder = SyncStateStorage.CreateAndSaveFolder(mailboxSession, mailboxSession.GetDefaultFolderId(DefaultFolderType.SyncRoot), CreateMode.OpenIfExists, deviceIdentity.CompositeKey, null, (syncStateStorage == null) ? null : syncStateStorage.folder, syncLogger);
					if (deviceSyncStateMetadata != null && deviceSyncStateMetadata.DeviceFolderId != folder.Id.ObjectId)
					{
						userSyncStateMetadata.TryRemove(deviceSyncStateMetadata.Id, syncLogger);
						deviceSyncStateMetadata = null;
					}
					if (deviceSyncStateMetadata == null)
					{
						deviceSyncStateMetadata = new DeviceSyncStateMetadata(mailboxSession, folder.Id.ObjectId, syncLogger);
						deviceSyncStateMetadata = userSyncStateMetadata.GetOrAdd(deviceSyncStateMetadata);
					}
					syncStateStorage2 = new SyncStateStorage(folder, deviceSyncStateMetadata, syncLogger);
					flag = true;
					return syncStateStorage2;
				}
				finally
				{
					if (!flag)
					{
						if (syncStateStorage2 != null)
						{
							syncStateStorage2.Dispose();
							syncStateStorage2 = null;
						}
						if (folder != null)
						{
							folder.Dispose();
							folder = null;
						}
					}
				}
				return syncStateStorage;
			}
			return syncStateStorage;
		}

		public static bool DeleteSyncStateStorage(MailboxSession mailboxSession, DeviceIdentity deviceIdentity, ISyncLogger syncLogger = null)
		{
			if (syncLogger == null)
			{
				syncLogger = TracingLogger.Singleton;
			}
			ArgumentValidator.ThrowIfNull("mailboxSession", mailboxSession);
			syncLogger.Information<DeviceIdentity>(ExTraceGlobals.SyncTracer, 0L, "SyncStateStorage::DeleteSyncStateStorage. Need to delete folder {0}", deviceIdentity);
			SyncStateTypeFactory.GetInstance().RegisterInternalBuilders();
			UserSyncStateMetadata userSyncStateMetadata = UserSyncStateMetadataCache.Singleton.Get(mailboxSession, syncLogger);
			List<DeviceSyncStateMetadata> allDevices = userSyncStateMetadata.GetAllDevices(mailboxSession, false, syncLogger);
			bool result = false;
			if (allDevices != null)
			{
				foreach (DeviceSyncStateMetadata deviceSyncStateMetadata in allDevices)
				{
					syncLogger.Information<DeviceIdentity>(ExTraceGlobals.SyncTracer, 0L, "SyncStateStorage::DeleteSyncStateStorage. Found syncstate folder {0}", deviceSyncStateMetadata.Id);
					if (string.Compare(deviceSyncStateMetadata.Id.Protocol, deviceIdentity.Protocol, StringComparison.Ordinal) == 0)
					{
						if (deviceSyncStateMetadata.Id.Equals(deviceIdentity))
						{
							mailboxSession.Delete(DeleteItemFlags.SoftDelete, new StoreId[]
							{
								deviceSyncStateMetadata.DeviceFolderId
							});
							userSyncStateMetadata.TryRemove(deviceSyncStateMetadata.Id, syncLogger);
						}
						else
						{
							syncLogger.Information(ExTraceGlobals.SyncTracer, 0L, "SyncStateStorage::DeleteSyncStateStorage. found more devices with same protocol");
							result = true;
						}
					}
				}
			}
			return result;
		}

		public static bool DeleteSyncStateStorage(MailboxSession mailboxSession, StoreObjectId deviceFolderId, DeviceIdentity deviceIdentity, ISyncLogger syncLogger = null)
		{
			if (syncLogger == null)
			{
				syncLogger = TracingLogger.Singleton;
			}
			ArgumentValidator.ThrowIfNull("mailboxSession", mailboxSession);
			ArgumentValidator.ThrowIfNull("folderId", deviceFolderId);
			bool result = false;
			SyncStateTypeFactory.GetInstance().RegisterInternalBuilders();
			UserSyncStateMetadata userSyncStateMetadata = UserSyncStateMetadataCache.Singleton.Get(mailboxSession, syncLogger);
			AggregateOperationResult aggregateOperationResult = mailboxSession.Delete(DeleteItemFlags.SoftDelete, new StoreId[]
			{
				deviceFolderId
			});
			syncLogger.Information<string>(ExTraceGlobals.SyncTracer, 0L, "SyncStateStorage::DeleteSyncStateStorage. Result = {0}", aggregateOperationResult.OperationResult.ToString());
			userSyncStateMetadata.TryRemove(deviceIdentity, syncLogger);
			List<DeviceSyncStateMetadata> allDevices = userSyncStateMetadata.GetAllDevices(mailboxSession, false, syncLogger);
			if (allDevices != null)
			{
				foreach (DeviceSyncStateMetadata deviceSyncStateMetadata in allDevices)
				{
					syncLogger.Information<DeviceIdentity, DeviceIdentity>(ExTraceGlobals.SyncTracer, 0L, "SyncStateStorage::DeleteSyncStateStorage. Found device folder '{0}', Looking for folder '{1}'", deviceSyncStateMetadata.Id, deviceIdentity);
					if (deviceSyncStateMetadata.Id.Equals(deviceIdentity))
					{
						try
						{
							aggregateOperationResult = mailboxSession.Delete(DeleteItemFlags.SoftDelete, new StoreId[]
							{
								deviceSyncStateMetadata.DeviceFolderId
							});
							syncLogger.Information<DeviceIdentity, DeviceIdentity, string>(ExTraceGlobals.SyncTracer, 0L, "SyncStateStorage::DeleteSyncStateStorage. try Deleting SyncState folder again.folderName:{0}, originalFolder:{1}, result:{2}", deviceSyncStateMetadata.Id, deviceIdentity, aggregateOperationResult.OperationResult.ToString());
							continue;
						}
						catch
						{
							syncLogger.Information(ExTraceGlobals.SyncTracer, 0L, "SyncStateStorage::DeleteSyncStateStorage. Error deleting the sync state folder.");
							continue;
						}
					}
					if (deviceSyncStateMetadata.Id.IsProtocol("AirSync"))
					{
						result = true;
						break;
					}
				}
			}
			return result;
		}

		public static SyncStateStorage.SyncStateStorageEnumerator GetEnumerator(MailboxSession mailboxSession, ISyncLogger syncLogger = null)
		{
			return new SyncStateStorage.SyncStateStorageEnumerator(mailboxSession, syncLogger);
		}

		public static void UpdateMailboxLoggingEnabled(MailboxSession mailboxSession, bool mailboxLoggingEnabled, ISyncLogger syncLogger = null)
		{
			using (Folder syncFolderRoot = SyncStateStorage.GetSyncFolderRoot(mailboxSession, syncLogger))
			{
				if (mailboxLoggingEnabled)
				{
					syncFolderRoot[SyncStateStorage.airsyncMailboxLoggingEnabledProp] = ExDateTime.UtcNow;
				}
				else
				{
					syncFolderRoot.Delete(SyncStateStorage.airsyncMailboxLoggingEnabledProp);
				}
				syncFolderRoot.Save();
				syncFolderRoot.Load();
				Folder folder = null;
				try
				{
					folder = SyncStateStorage.CreateAndSaveFolder(mailboxSession, syncFolderRoot.Id.ObjectId, CreateMode.OpenIfExists, SyncStateStorage.MailboxLoggingTriggerFolder, null, null, syncLogger);
					if (folder != null)
					{
						syncFolderRoot.DeleteObjects(DeleteItemFlags.SoftDelete, new StoreId[]
						{
							folder.Id.ObjectId
						});
					}
				}
				finally
				{
					if (folder != null)
					{
						folder.Dispose();
					}
				}
			}
		}

		public static bool GetMailboxLoggingEnabled(MailboxSession mailboxSession, ISyncLogger syncLogger = null)
		{
			if (syncLogger == null)
			{
				syncLogger = TracingLogger.Singleton;
			}
			if (mailboxSession.IsConnected)
			{
				StoreObjectId defaultFolderId = mailboxSession.GetDefaultFolderId(DefaultFolderType.SyncRoot);
				if (defaultFolderId == null)
				{
					return false;
				}
				using (Folder folder = Folder.Bind(mailboxSession, defaultFolderId, SyncStateStorage.loggingEnabledAsArray))
				{
					return SyncStateStorage.IsMailboxLoggingEnabled(folder);
				}
			}
			syncLogger.TraceDebug(ExTraceGlobals.SyncProcessTracer, 0L, "[SyncStateStorage.GetMailboxLoggingEnabled] MailboxSession was not connected - defaulting to false since we can't write to the mailbox.");
			return false;
		}

		public CustomSyncState CreateCustomSyncState(SyncStateInfo syncStateInfo)
		{
			this.CheckDisposed("CreateCustomSyncState");
			this.syncLogger.Information<int>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "SyncStateStorage::CreateCustomSyncState. Hashcode = {0}", this.GetHashCode());
			if (syncStateInfo == null)
			{
				throw new ArgumentNullException("syncStateInfo");
			}
			return CustomSyncState.CreateSyncState(this, syncStateInfo, this.folder, this.syncLogger);
		}

		public FolderHierarchySyncState CreateFolderHierarchySyncState()
		{
			this.CheckDisposed("CreateFolderHierarchySyncState");
			this.syncLogger.Information<int>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "SyncStateStorage::CreateFolderHierarchySyncState. Hashcode = {0}", this.GetHashCode());
			return FolderHierarchySyncState.CreateSyncState(this, this.folder, this.syncLogger);
		}

		public FolderSyncState CreateFolderSyncState(ISyncProviderFactory syncProviderFactory, string syncFolderId)
		{
			this.CheckDisposed("CreateFolderSyncState");
			this.syncLogger.Information<int>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "SyncStateStorage::CreateFolderSyncState. Hashcode = {0}", this.GetHashCode());
			ArgumentValidator.ThrowIfNull("syncProviderFactory", syncProviderFactory);
			ArgumentValidator.ThrowIfNull("syncFolderId", syncFolderId);
			return FolderSyncState.CreateSyncState(this, this.folder, syncProviderFactory, syncFolderId, this.syncLogger);
		}

		public bool DeleteAllSyncStatesIfMoved()
		{
			long num = 0L;
			using (QueryResult queryResult = this.folder.FolderQuery(FolderQueryFlags.None, null, null, new PropertyDefinition[]
			{
				AirSyncStateSchema.MetadataLastSyncTime
			}))
			{
				for (;;)
				{
					object[][] rows = queryResult.GetRows(100);
					if (rows == null || rows.Length == 0)
					{
						break;
					}
					foreach (object[] array2 in rows)
					{
						if (!(array2[0] is PropertyError))
						{
							long num2 = (long)array2[0];
							if (num2 > num)
							{
								num = num2;
							}
						}
					}
				}
			}
			if (this.CreationTime.UtcTicks > num)
			{
				this.syncLogger.TraceDebug<ExDateTime, long>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "Detected mailbox moved! syncStateStorage.CreationTime = {0}, lastSyncTime = {1}. Deleting all SyncState...", this.CreationTime, num);
				this.DeleteAllSyncStates();
				return true;
			}
			return false;
		}

		public GroupOperationResult DeleteAllSyncStates()
		{
			this.CheckDisposed("DeleteAllSyncStates");
			this.syncLogger.Information<int>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "[SyncStateStorage.RemoveAllSyncStates] HashCode = {0}.", this.GetHashCode());
			this.DeviceMetadata.RemoveAll();
			GroupOperationResult groupOperationResult = this.folder.DeleteAllObjects(DeleteItemFlags.SoftDelete);
			this.syncLogger.Information<OperationResult, LocalizedException>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "[SyncStateStorage.RemoveAllSyncStates] Result: '{0}', Exception: '{1}' \n", groupOperationResult.OperationResult, groupOperationResult.Exception);
			return groupOperationResult;
		}

		public AggregateOperationResult DeleteCustomSyncState(SyncStateInfo syncStateInfo)
		{
			this.CheckDisposed("DeleteCustomSyncState");
			this.syncLogger.Information<int>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "[SyncStateStorage.DeleteCustomSyncState] Hashcode = {0}", this.GetHashCode());
			ArgumentValidator.ThrowIfNull("syncStateInfo", syncStateInfo);
			return this.InternalDeleteSyncState(syncStateInfo.UniqueName);
		}

		private AggregateOperationResult InternalDeleteSyncState(string name)
		{
			this.CheckDisposed("DeleteCustomSyncState");
			this.syncLogger.Information<int>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "SyncStateStorage::InternalDeleteSyncState. Hashcode = {0}", this.GetHashCode());
			ArgumentValidator.ThrowIfNullOrEmpty("name", name);
			SyncStateMetadata syncState = this.DeviceMetadata.GetSyncState(this.folder.Session as MailboxSession, name, this.syncLogger);
			this.DeviceMetadata.TryRemove(name, this.syncLogger);
			AggregateOperationResult result = null;
			if (syncState != null)
			{
				if (syncState.FolderSyncStateId == null && syncState.ItemSyncStateId != null)
				{
					this.syncLogger.Information<string>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "[SyncStateStorage.InternalDeleteSyncState] Deleting direct item sync state {0}", name);
					result = this.folder.DeleteObjects(DeleteItemFlags.SoftDelete, new StoreId[]
					{
						syncState.ItemSyncStateId
					});
				}
				else if (syncState.FolderSyncStateId != null)
				{
					this.syncLogger.Information<string>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "[SyncStateStorage.InternalDeleteSyncState] Deleting sync folder for sync state {0}", name);
					result = this.folder.DeleteObjects(DeleteItemFlags.SoftDelete, new StoreId[]
					{
						syncState.FolderSyncStateId
					});
				}
				else
				{
					this.syncLogger.Information<string>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "[SyncStateStorage.InternalDeleteSyncState] Metadata had null for both item and folder id.  Weird - not deleting anything.  Sync State: {0}", name);
				}
				StoreObjectId storeObjectId = (syncState.FolderSyncStateId != null) ? syncState.FolderSyncStateId : syncState.ItemSyncStateId;
				if (storeObjectId != null)
				{
					this.TraceAggregateOperationResultFromDelete(result, name, storeObjectId);
				}
			}
			return result;
		}

		public AggregateOperationResult DeleteFolderHierarchySyncState()
		{
			this.CheckDisposed("DeleteFolderHierarchySyncState");
			this.syncLogger.Information<int>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "SyncStateStorage::DeleteFolderHierarchySyncState. Hashcode = {0}", this.GetHashCode());
			FolderHierarchySyncStateInfo folderHierarchySyncStateInfo = new FolderHierarchySyncStateInfo();
			return this.InternalDeleteSyncState(folderHierarchySyncStateInfo.UniqueName);
		}

		public AggregateOperationResult DeleteFolderSyncState(string syncFolderName)
		{
			this.CheckDisposed("DeleteFolderSyncState");
			this.syncLogger.Information<int>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "SyncStateStorage::DeleteFolderSyncState. Hashcode = {0}", this.GetHashCode());
			ArgumentValidator.ThrowIfNullOrEmpty("syncFolderName", syncFolderName);
			return this.InternalDeleteSyncState(syncFolderName);
		}

		public AggregateOperationResult DeleteFolderSyncState(ISyncProviderFactory syncFactory)
		{
			this.CheckDisposed("DeleteFolderSyncState");
			this.syncLogger.Information<int>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "SyncStateStorage::DeleteFolderSyncState. Hashcode = {0}", this.GetHashCode());
			ArgumentValidator.ThrowIfNull("syncFactory", syncFactory);
			StoreObjectId storeObjectId = StoreObjectId.Deserialize(syncFactory.GetCollectionIdBytes());
			FolderSyncStateMetadata folderSyncStateMetadata;
			if (this.DeviceMetadata.SyncStatesByIPMFolderId.TryGetValue(storeObjectId, out folderSyncStateMetadata))
			{
				return this.InternalDeleteSyncState(folderSyncStateMetadata.Name);
			}
			this.syncLogger.TraceDebug<string>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "[SyncStateStorage.DeleteFolderSyncState] Did not find cached mapping for IPM Folder Id {0}", storeObjectId.ToBase64String());
			return null;
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<SyncStateStorage>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		public CustomSyncState GetCustomSyncState(SyncStateInfo syncStateInfo, params PropertyDefinition[] additionalPropsToFetch)
		{
			this.CheckDisposed("GetCustomSyncState");
			this.syncLogger.Information<int>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "SyncStateStorage::GetCustomSyncState. Hashcode = {0}", this.GetHashCode());
			this.GetMailboxSession();
			CustomSyncState result = null;
			try
			{
				result = CustomSyncState.GetSyncState(this, this.folder, syncStateInfo, this.syncLogger, additionalPropsToFetch);
			}
			catch (ObjectNotFoundException arg)
			{
				this.syncLogger.TraceDebug<string, ObjectNotFoundException>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "[SyncStateStorage.GetCustomSyncState] Hark! SyncState {0} was not found.  Exception: {1}", syncStateInfo.UniqueName, arg);
			}
			return result;
		}

		private MailboxSession GetMailboxSession()
		{
			MailboxSession mailboxSession = this.folder.Session as MailboxSession;
			if (mailboxSession == null)
			{
				throw new NotSupportedException();
			}
			return mailboxSession;
		}

		public FolderHierarchySyncState GetFolderHierarchySyncState()
		{
			this.CheckDisposed("GetFolderHierarchySyncState");
			this.syncLogger.Information<int>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "SyncStateStorage::GetFolderHierarchySyncState. Hashcode = {0}", this.GetHashCode());
			this.GetMailboxSession();
			FolderHierarchySyncState result = null;
			try
			{
				result = FolderHierarchySyncState.GetSyncState(this, this.folder, this.syncLogger);
			}
			catch (ObjectNotFoundException arg)
			{
				this.syncLogger.TraceDebug<string, ObjectNotFoundException>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "[SyncStateStorage.GetFolderHierarchySyncState] Hark! SyncState {0} was not found.  Exception: {1}", "FolderHierarchy", arg);
			}
			return result;
		}

		public FolderSyncState GetFolderSyncState(string syncStateName)
		{
			return this.GetFolderSyncState(null, syncStateName);
		}

		public FolderSyncState GetFolderSyncState(ISyncProviderFactory syncProviderFactory)
		{
			return this.GetFolderSyncState(syncProviderFactory, null);
		}

		public FolderSyncState GetFolderSyncState(ISyncProviderFactory syncProviderFactory, string syncStateName)
		{
			return this.GetFolderSyncState(syncProviderFactory, syncStateName, null);
		}

		public FolderSyncState GetFolderSyncState(ISyncProviderFactory syncProviderFactory, string syncFolderName, Func<SyncStateStorage, StoreObject, FolderSyncStateMetadata, SyncStateInfo, ISyncProviderFactory, bool, ISyncLogger, FolderSyncState> creator)
		{
			this.CheckDisposed("GetFolderSyncState");
			this.syncLogger.Information<int>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "SyncStateStorage::GetFolderSyncState. Hashcode = {0}", this.GetHashCode());
			if (syncProviderFactory == null && string.IsNullOrEmpty(syncFolderName))
			{
				throw new ArgumentNullException("syncProviderFactory and syncFolderName");
			}
			FolderSyncState result;
			try
			{
				result = (string.IsNullOrEmpty(syncFolderName) ? FolderSyncState.GetSyncState(this, this.folder, syncProviderFactory, creator, this.syncLogger) : FolderSyncState.GetSyncState(this, this.folder, syncProviderFactory, syncFolderName, creator, this.syncLogger));
			}
			catch (ObjectNotFoundException arg)
			{
				this.syncLogger.TraceDebug<string, ObjectNotFoundException>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "[SyncStateStorage.GetFolderSyncState] Hark! SyncState '{0}' was not found.  Exception: {1}", (syncFolderName == null) ? "<Null>" : syncFolderName, arg);
				result = null;
			}
			return result;
		}

		public int CountFolderSyncStates()
		{
			this.CheckDisposed("CountFolderSyncStates");
			this.syncLogger.Information<int>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "SyncStateStorage::CountFolderSyncStates. HashCode = {0}.", this.GetHashCode());
			return this.DeviceMetadata.FolderSyncStateCount;
		}

		public static string ConstructSyncFolderName(string protocol, string deviceType, string deviceId)
		{
			return string.Format("{0}-{1}-{2}", protocol, deviceType, deviceId);
		}

		private static Folder CreateAndSaveFolder(MailboxSession mailboxSession, StoreObjectId containerId, CreateMode createMode, string displayName, string containerClass, Folder folderIn, ISyncLogger syncLogger = null)
		{
			if (syncLogger == null)
			{
				syncLogger = TracingLogger.Singleton;
			}
			Folder folder = null;
			bool flag = false;
			Folder result;
			try
			{
				if (folderIn == null)
				{
					folder = Folder.Create(mailboxSession, containerId, StoreObjectType.Folder, displayName, createMode);
					folder[SyncStateStorage.airsyncLockProp] = "0";
				}
				else
				{
					folder = folderIn;
				}
				if (containerClass != null)
				{
					folder[InternalSchema.ContainerClass] = containerClass;
				}
				StoreObjectId storeObjectId = null;
				if (!folder.IsNew)
				{
					storeObjectId = folder.Id.ObjectId;
				}
				FolderSaveResult folderSaveResult = folder.Save();
				if (folderSaveResult.OperationResult != OperationResult.Succeeded)
				{
					syncLogger.TraceDebug<string, FolderSaveResult>(ExTraceGlobals.SyncTracer, 0L, "SyncStateStorage::CreateAndSaveFolder. Failed to create folder {0}, due to {1}.", displayName, folderSaveResult);
					if (storeObjectId == null)
					{
						folder.Load(null);
						storeObjectId = folder.StoreObjectId;
					}
					mailboxSession.Delete(DeleteItemFlags.SoftDelete, new StoreId[]
					{
						storeObjectId
					});
					throw folderSaveResult.ToException(ServerStrings.ExCannotCreateFolder(folderSaveResult));
				}
				folder.Load(SyncStateStorage.loggingEnabledAndCreateTimeAsArray);
				flag = true;
				result = folder;
			}
			finally
			{
				if (!flag && folder != null)
				{
					folder.Dispose();
					folder = null;
				}
			}
			return result;
		}

		internal static Folder GetSyncFolderRoot(MailboxSession mailboxSession, ISyncLogger syncLogger = null)
		{
			if (syncLogger == null)
			{
				syncLogger = TracingLogger.Singleton;
			}
			StoreObjectId storeObjectId = mailboxSession.GetDefaultFolderId(DefaultFolderType.SyncRoot);
			if (storeObjectId == null)
			{
				syncLogger.TraceDebug<Guid>(ExTraceGlobals.SyncTracer, 0L, "[SyncStateStorage.GetSyncFolderRoot] ExchangeSyncData folder missing for mailbox {0}.  Creating it now.", mailboxSession.MailboxGuid);
				storeObjectId = mailboxSession.CreateDefaultFolder(DefaultFolderType.SyncRoot);
			}
			return Folder.Bind(mailboxSession, storeObjectId, SyncStateStorage.loggingEnabledAsArray);
		}

		private static SyncStateStorage GetSyncStateStorage(MailboxSession session, DeviceSyncStateMetadata deviceMetadata, ISyncLogger syncLogger = null)
		{
			if (deviceMetadata == null)
			{
				return null;
			}
			if (syncLogger == null)
			{
				syncLogger = TracingLogger.Singleton;
			}
			Folder folder = null;
			SyncStateStorage syncStateStorage = null;
			bool flag = false;
			SyncStateStorage result;
			try
			{
				try
				{
					folder = Folder.Bind(session, deviceMetadata.DeviceFolderId, SyncStateStorage.loggingEnabledAndCreateTimeAsArray);
					syncStateStorage = new SyncStateStorage(folder, deviceMetadata, syncLogger);
				}
				catch (ObjectNotFoundException)
				{
					syncLogger.TraceDebug<DeviceSyncStateMetadata>(ExTraceGlobals.SyncTracer, 0L, "[SyncStateStorage.Create] Did not find SyncStateStorage for device {0}.  Removing from cache.", deviceMetadata);
					UserSyncStateMetadata userSyncStateMetadata = UserSyncStateMetadataCache.Singleton.Get(session, syncLogger);
					userSyncStateMetadata.TryRemove(deviceMetadata.Id, syncLogger);
					deviceMetadata = userSyncStateMetadata.GetDevice(session, deviceMetadata.Id, syncLogger);
					if (deviceMetadata != null)
					{
						folder = Folder.Bind(session, deviceMetadata.DeviceFolderId, SyncStateStorage.loggingEnabledAndCreateTimeAsArray);
						syncStateStorage = new SyncStateStorage(folder, deviceMetadata, syncLogger);
					}
				}
				flag = true;
				result = syncStateStorage;
			}
			finally
			{
				if (!flag)
				{
					if (syncStateStorage != null)
					{
						syncStateStorage.Dispose();
						syncStateStorage = null;
					}
					if (folder != null)
					{
						folder.Dispose();
						folder = null;
					}
				}
			}
			return result;
		}

		internal static bool IsMailboxLoggingEnabled(Folder rootFolder)
		{
			ExDateTime? loggingEnabledTime = SyncStateStorage.GetLoggingEnabledTime(rootFolder);
			return loggingEnabledTime != null && ExDateTime.UtcNow < loggingEnabledTime.Value.IncrementDays(SyncStateStorage.loggingExpirationInDays);
		}

		internal static ExDateTime? GetLoggingEnabledTime(Folder rootFolder)
		{
			if (rootFolder == null)
			{
				return null;
			}
			return rootFolder.GetValueAsNullable<ExDateTime>(SyncStateStorage.airsyncMailboxLoggingEnabledProp);
		}

		private void TraceAggregateOperationResultFromDelete(AggregateOperationResult result, string syncStateName, StoreObjectId id)
		{
			string text = null;
			if (result == null)
			{
				text = string.Format("SyncState '{0}' with ID = '{1}' doesn't exist.", syncStateName, id);
			}
			else if (!result.OperationResult.Equals(OperationResult.Succeeded))
			{
				StringBuilder stringBuilder = new StringBuilder(500);
				stringBuilder.AppendFormat("SyncState '{0}' is not deleted properly. Result: '{1}' ", syncStateName, result);
				stringBuilder.Append("GroupOperationResult: \n\n");
				if (result.GroupOperationResults != null)
				{
					foreach (GroupOperationResult groupOperationResult in result.GroupOperationResults)
					{
						stringBuilder.AppendFormat("Result: '{0}', Exception: '{1}' \n", groupOperationResult.OperationResult, groupOperationResult.Exception);
					}
				}
				text = stringBuilder.ToString();
			}
			if (text != null)
			{
				this.syncLogger.Information(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), text);
			}
		}

		private void CheckDisposed(string methodName)
		{
			if (this.disposed)
			{
				StorageGlobals.TraceFailedCheckDisposed(this, methodName);
				throw new ObjectDisposedException(base.GetType().ToString());
			}
		}

		private void Dispose(bool disposing)
		{
			StorageGlobals.TraceDispose(this, this.disposed, disposing);
			if (!this.disposed)
			{
				this.disposed = true;
				this.InternalDispose(disposing);
			}
		}

		private void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				if (this.folder != null)
				{
					this.folder.Dispose();
				}
				if (this.compressedMemoryStream != null)
				{
					this.compressedMemoryStream.Dispose();
					this.compressedMemoryStream = null;
				}
				if (this.disposeTracker != null)
				{
					this.disposeTracker.Dispose();
				}
			}
			this.folder = null;
		}

		public static readonly string MailboxLoggingTriggerFolder = "MailboxLoggingTriggerFolder";

		private readonly DisposeTracker disposeTracker;

		private static int loggingExpirationInDays = 3;

		private static StorePropertyDefinition airsyncLockProp = GuidNamePropertyDefinition.CreateCustom("AirSyncLock", typeof(string), WellKnownPropertySet.AirSync, "AirSync:AirSyncLock", PropertyFlags.None);

		internal static StorePropertyDefinition airsyncMailboxLoggingEnabledProp = GuidNamePropertyDefinition.CreateCustom("AirSyncMailboxLoggingEnabled", typeof(ExDateTime), WellKnownPropertySet.AirSync, "AirSync:AirSyncMailboxLoggingEnabled", PropertyFlags.None);

		private static readonly PropertyDefinition[] loggingEnabledAsArray = new PropertyDefinition[]
		{
			SyncStateStorage.airsyncMailboxLoggingEnabledProp
		};

		private static readonly PropertyDefinition[] loggingEnabledAndCreateTimeAsArray = new PropertyDefinition[]
		{
			StoreObjectSchema.CreationTime,
			SyncStateStorage.airsyncLockProp
		};

		private ExDateTime airsyncLock;

		private ExDateTime creationTime;

		private ISyncLogger syncLogger;

		private Folder folder;

		private bool disposed;

		private StoreObjectId syncRootFolderId;

		private PooledMemoryStream compressedMemoryStream;

		internal class SyncStateStorageEnumerator : IEnumerator, IDisposable
		{
			public SyncStateStorageEnumerator(MailboxSession mailboxSession, ISyncLogger syncLogger = null)
			{
				this.syncLogger = (syncLogger ?? TracingLogger.Singleton);
				UserSyncStateMetadata userSyncStateMetadata = UserSyncStateMetadataCache.Singleton.Get(mailboxSession, this.syncLogger);
				this.devices = userSyncStateMetadata.GetAllDevices(mailboxSession, true, this.syncLogger);
				this.index = -1;
				this.mailboxSession = mailboxSession;
			}

			public object Current
			{
				get
				{
					this.CheckDisposed("get_Current");
					return this.current;
				}
			}

			public void Dispose()
			{
				this.Dispose(true);
			}

			public bool MoveNext()
			{
				this.CheckDisposed("MoveNext");
				if (this.current != null)
				{
					this.current.Dispose();
					this.current = null;
				}
				if (this.devices == null || this.index >= this.devices.Count)
				{
					return false;
				}
				this.index++;
				if (this.index >= this.devices.Count)
				{
					return false;
				}
				this.current = SyncStateStorage.GetSyncStateStorage(this.mailboxSession, this.devices[this.index], this.syncLogger);
				return this.current != null || this.MoveNext();
			}

			public void Reset()
			{
				this.CheckDisposed("Reset");
				if (this.current != null)
				{
					this.current.Dispose();
				}
				this.current = null;
				this.index = -1;
			}

			private void CheckDisposed(string methodName)
			{
				if (this.disposed)
				{
					this.syncLogger.TraceDebug<string, Type, int>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "SyncStateStorageEnumerator::{0}. Object type = {1}, hashcode = {2} was already disposed.", methodName, base.GetType(), this.GetHashCode());
					throw new ObjectDisposedException(base.GetType().ToString());
				}
			}

			private void Dispose(bool disposing)
			{
				this.syncLogger.Information<int, Type, bool>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "SyncStateStorage::Dispose. HashCode = {0}, type = {1}, disposing = {2}.", this.GetHashCode(), base.GetType(), disposing);
				if (!this.disposed)
				{
					this.disposed = true;
					this.InternalDispose(disposing);
				}
			}

			private void InternalDispose(bool disposing)
			{
				if (disposing && this.current != null)
				{
					this.current.Dispose();
				}
				this.mailboxSession = null;
				this.current = null;
			}

			private SyncStateStorage current;

			private int index;

			private bool disposed;

			private List<DeviceSyncStateMetadata> devices;

			private MailboxSession mailboxSession;

			private ISyncLogger syncLogger;
		}

		private class MemoryStream100K : MemoryStream
		{
			public MemoryStream100K() : base(102400)
			{
			}
		}
	}
}
