using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class SyncState : ISyncState, IDisposeTrackable, IDisposable
	{
		protected SyncState(SyncStateStorage syncStateStorage, StoreObject storeObject, SyncStateMetadata metadata, SyncStateInfo syncStateInfo, bool syncStateIsNew, ISyncLogger syncLogger = null)
		{
			this.syncLogger = (syncLogger ?? TracingLogger.Singleton);
			using (DisposeGuard disposeGuard = this.Guard())
			{
				StorageGlobals.TraceConstructIDisposable(this);
				this.disposeTracker = this.GetDisposeTracker();
				if (!(storeObject is Folder) && !(storeObject is Item))
				{
					throw new ArgumentException("storeObject is of invalid type: " + storeObject.GetType(), "storeObject");
				}
				this.storeObject = storeObject;
				this.syncStateInfo = syncStateInfo;
				this.syncStateStorage = syncStateStorage;
				this.syncStateIsNew = syncStateIsNew;
				this.SyncStateMetadata = metadata;
				this.Load(false, new PropertyDefinition[0]);
				disposeGuard.Success();
			}
		}

		public SyncStateMetadata SyncStateMetadata { get; private set; }

		public int? BackendVersion
		{
			get
			{
				this.CheckDisposed("get_BackendVersion");
				return this.GetData<NullableData<Int32Data, int>, int?>(SyncStateProp.Version, null);
			}
			private set
			{
				this.CheckDisposed("set_BackendVersion");
				this[SyncStateProp.Version] = new NullableData<Int32Data, int>(value);
			}
		}

		public bool KeepCachedDataOnReload
		{
			get
			{
				this.CheckDisposed("get_KeepCachedDataOnReload");
				return this.keepCachedDataOnReload;
			}
			set
			{
				this.CheckDisposed("set_KeepCachedDataOnReload");
				this.keepCachedDataOnReload = value;
			}
		}

		public string UniqueName
		{
			get
			{
				this.CheckDisposed("get_UniqueName");
				if (this.syncStateInfo.UniqueName == null)
				{
					if (this.storeObject is Folder)
					{
						this.syncStateInfo.UniqueName = this.storeObject.GetValueOrDefault<string>(InternalSchema.DisplayName);
					}
					else
					{
						this.syncStateInfo.UniqueName = this.storeObject.GetValueOrDefault<string>(InternalSchema.Subject);
					}
				}
				return this.syncStateInfo.UniqueName;
			}
		}

		public int Version
		{
			get
			{
				this.CheckDisposed("get_Version");
				return this.syncStateInfo.Version;
			}
			set
			{
				this.CheckDisposed("set_Version");
				throw new InvalidOperationException("SyncState.Version is not settable.  It should be set through SyncStateInfo");
			}
		}

		public StoreObject StoreObject
		{
			get
			{
				this.CheckDisposed("get_StoreObject");
				return this.storeObject;
			}
		}

		public bool SyncStateIsNew
		{
			get
			{
				this.CheckDisposed("get_SyncStateIsNew");
				return this.syncStateIsNew;
			}
		}

		internal SyncStateStorage SyncStateStorage
		{
			get
			{
				this.CheckDisposed("get_SyncStateStorage");
				return this.syncStateStorage;
			}
		}

		public int TotalSaveCount
		{
			get
			{
				this.CheckDisposed("get_TotalSaveCount");
				return this.totalSaveCount;
			}
		}

		public int ColdSaveCount
		{
			get
			{
				this.CheckDisposed("get_ColdSaveCount");
				return this.coldSaveCount;
			}
		}

		public int ColdCopyCount
		{
			get
			{
				this.CheckDisposed("get_ColdCopyCount");
				return this.coldCopyCount;
			}
		}

		public int TotalLoadCount
		{
			get
			{
				this.CheckDisposed("get_TotalLoadCount");
				return this.totalLoadCount;
			}
		}

		public ICustomSerializableBuilder this[string key]
		{
			get
			{
				this.CheckDisposed("get_this[]");
				GenericDictionaryData<ConstStringData, string, DerivedData<ICustomSerializableBuilder>> syncStateTableForKey = this.GetSyncStateTableForKey(key);
				DerivedData<ICustomSerializableBuilder> derivedData;
				if (syncStateTableForKey.Data.TryGetValue(key, out derivedData))
				{
					return derivedData.Data;
				}
				return null;
			}
			set
			{
				this.CheckDisposed("set_this[]");
				this.GetSyncStateTableForKey(key).Data[key] = new DerivedData<ICustomSerializableBuilder>(value);
			}
		}

		public virtual void Commit()
		{
			if (this.syncStateInfo.ReadOnly)
			{
				throw new ApplicationException("Commit should not be called when readOnly is set to true.");
			}
			this.Commit(null, null);
		}

		public bool Contains(string key)
		{
			this.CheckDisposed("Contains");
			GenericDictionaryData<ConstStringData, string, DerivedData<ICustomSerializableBuilder>> syncStateTableForKey = this.GetSyncStateTableForKey(key);
			return syncStateTableForKey.Data.ContainsKey(key);
		}

		public abstract DisposeTracker GetDisposeTracker();

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

		public long GetColdStateCompressedSize()
		{
			this.CheckDisposed("GetColdStateCompressedSize");
			if (this.syncStateIsNew)
			{
				return 0L;
			}
			if (0L == this.hotDataBeginsAt)
			{
				return 0L;
			}
			if (0L == this.coldDataBeginsAt)
			{
				return 0L;
			}
			return this.hotDataBeginsAt - this.coldDataBeginsAt;
		}

		public RawT GetData<T, RawT>(string key, RawT defaultValue) where T : ComponentData<RawT>, new()
		{
			this.CheckDisposed("GetData");
			GenericDictionaryData<ConstStringData, string, DerivedData<ICustomSerializableBuilder>> syncStateTableForKey = this.GetSyncStateTableForKey(key);
			DerivedData<ICustomSerializableBuilder> derivedData;
			if (!syncStateTableForKey.Data.TryGetValue(key, out derivedData))
			{
				return defaultValue;
			}
			if (derivedData == null || derivedData.Data == null)
			{
				return defaultValue;
			}
			T t = (T)((object)derivedData.Data);
			return t.Data;
		}

		public static RawT GetData<T, RawT>(string key, RawT defaultValue, GenericDictionaryData<ConstStringData, string, DerivedData<ICustomSerializableBuilder>> syncStateTable) where T : ComponentData<RawT>, new()
		{
			DerivedData<ICustomSerializableBuilder> derivedData;
			if (!syncStateTable.Data.TryGetValue(key, out derivedData))
			{
				return defaultValue;
			}
			if (derivedData == null || derivedData.Data == null)
			{
				return defaultValue;
			}
			T t = (T)((object)derivedData.Data);
			return t.Data;
		}

		public T GetData<T>(string key) where T : ICustomSerializable, new()
		{
			this.CheckDisposed("GetData");
			GenericDictionaryData<ConstStringData, string, DerivedData<ICustomSerializableBuilder>> syncStateTableForKey = this.GetSyncStateTableForKey(key);
			DerivedData<ICustomSerializableBuilder> derivedData;
			if (!syncStateTableForKey.Data.TryGetValue(key, out derivedData))
			{
				return default(T);
			}
			if (derivedData == null || derivedData.Data == null)
			{
				return default(T);
			}
			if (derivedData.Data is T)
			{
				return (T)((object)derivedData.Data);
			}
			throw new InvalidOperationException(ServerStrings.ExMismatchedSyncStateDataType(typeof(T).ToString(), derivedData.GetType().ToString()));
		}

		public long GetHotStateCompressedSize()
		{
			this.CheckDisposed("GetHotStateCompressedSize");
			if (0L == this.hotDataBeginsAt)
			{
				return 0L;
			}
			return this.syncStateStorage.CompressedMemoryStream.Length - this.hotDataBeginsAt;
		}

		public long GetTotalCompressedSize()
		{
			this.CheckDisposed("GetTotalCompressedSize");
			return this.syncStateStorage.CompressedMemoryStream.Length;
		}

		public long GetLastCommittedSize()
		{
			this.CheckDisposed("GetLastCommittedSize");
			return this.lastCommittedSize;
		}

		public long GetLastUncompressedSize()
		{
			this.CheckDisposed("GetLastUncompressedSize");
			return this.lastUncompressedSize;
		}

		public bool IsColdStateDeserialized()
		{
			this.CheckDisposed("IsColdStateDeserialized");
			return null != this.coldSyncStateTable;
		}

		public virtual void Load()
		{
			this.Load(true, new PropertyDefinition[0]);
		}

		public void OnCommitStateModifications(FolderSyncStateUtil.CommitStateModificationsDelegate commitStateModificationsDelegate)
		{
			this.CheckDisposed("OnCommitStateModifications");
			this.commitStateModificationsDelegate = commitStateModificationsDelegate;
		}

		public void Remove(string key)
		{
			this.CheckDisposed("Remove");
			GenericDictionaryData<ConstStringData, string, DerivedData<ICustomSerializableBuilder>> syncStateTableForKey = this.GetSyncStateTableForKey(key);
			if (syncStateTableForKey.Data.ContainsKey(key))
			{
				syncStateTableForKey.Data.Remove(key);
			}
		}

		public void HandleCorruptSyncState()
		{
			this.CheckDisposed("HandleCorruptSyncState");
			this.HandleCorruptSyncState(null);
		}

		internal static StoreObject CreateSyncStateStoreObject(SyncStateStorage syncStateStorage, SyncStateInfo syncStateInfo, Folder syncStateParentFolder, PropertyDefinition[] properties, object[] values, ISyncLogger syncLogger = null)
		{
			StoreObject result = null;
			if (syncStateStorage.SaveOnDirectItems)
			{
				result = SyncState.CreateSyncStateItemInFolder(syncStateInfo, syncStateStorage.FolderId, syncStateParentFolder.Session as MailboxSession, properties, values, syncLogger);
			}
			else
			{
				using (Folder folder = SyncState.CreateSyncStateFolderInFolder(syncStateInfo, syncStateParentFolder, properties, values, syncLogger))
				{
					result = SyncState.CreateSyncStateItemInFolder(syncStateInfo, folder.StoreObjectId, syncStateParentFolder.Session as MailboxSession, properties, values, syncLogger);
				}
			}
			return result;
		}

		internal static StoreObject GetSyncStateStoreObject(SyncStateStorage syncStateStorage, Folder syncStateParentFolder, SyncStateInfo syncStateInfo, ISyncLogger syncLogger, params PropertyDefinition[] propsToReturn)
		{
			SyncStateMetadata syncStateMetadata;
			return SyncState.GetSyncStateStoreObject(syncStateStorage, syncStateParentFolder, syncStateInfo, syncLogger, out syncStateMetadata, propsToReturn);
		}

		internal static StoreObject GetSyncStateStoreObject(SyncStateStorage syncStateStorage, Folder syncStateParentFolder, SyncStateInfo syncStateInfo, ISyncLogger syncLogger, out SyncStateMetadata syncStateMetadata, params PropertyDefinition[] propsToReturn)
		{
			syncStateMetadata = syncStateStorage.DeviceMetadata.GetSyncState(syncStateParentFolder.Session as MailboxSession, syncStateInfo.UniqueName, syncLogger);
			return SyncState.GetSyncStateStoreObject(syncStateParentFolder, ref syncStateMetadata, syncLogger, propsToReturn);
		}

		internal static StoreObject GetSyncStateStoreObject(Folder syncStateParentFolder, ref SyncStateMetadata syncStateMetadata, ISyncLogger syncLogger, params PropertyDefinition[] propsToReturn)
		{
			if (syncLogger == null)
			{
				syncLogger = TracingLogger.Singleton;
			}
			if (syncStateMetadata == null)
			{
				return null;
			}
			StoreObject storeObject = null;
			if (syncStateMetadata.ItemSyncStateId != null)
			{
				try
				{
					try
					{
						storeObject = Microsoft.Exchange.Data.Storage.Item.Bind(syncStateParentFolder.Session, syncStateMetadata.ItemSyncStateId, SyncState.AppendAdditionalProperties(propsToReturn));
					}
					catch (ObjectNotFoundException)
					{
						syncLogger.TraceDebug<string, string>(ExTraceGlobals.SyncTracer, 0L, "[SyncState.GetSyncStateStorageObject] Cached sync state item does not exist.  Clearing from cache.  Name: {0}, ItemId: {1}", syncStateMetadata.Name, syncStateMetadata.ItemSyncStateId.ToBase64String());
						syncStateMetadata.ParentDevice.TryRemove(syncStateMetadata.Name, syncLogger);
						syncStateMetadata = syncStateMetadata.ParentDevice.GetSyncState(syncStateParentFolder.Session as MailboxSession, syncStateMetadata.Name, syncLogger);
						return (syncStateMetadata == null) ? null : SyncState.GetSyncStateStoreObject(syncStateParentFolder, ref syncStateMetadata, syncLogger, propsToReturn);
					}
					((Item)storeObject).OpenAsReadWrite();
					return storeObject;
				}
				catch
				{
					if (storeObject != null)
					{
						storeObject.Dispose();
						storeObject = null;
					}
					throw;
				}
			}
			try
			{
				storeObject = Folder.Bind(syncStateParentFolder.Session, syncStateMetadata.FolderSyncStateId, SyncState.AppendAdditionalProperties(propsToReturn));
			}
			catch (ObjectNotFoundException)
			{
				syncLogger.TraceDebug<string, string>(ExTraceGlobals.SyncTracer, 0L, "[SyncState.GetSyncStateStorageObject] Cached sync state folder does not exist.  Clearing from cache.  Name: {0}, ItemId: {1}", syncStateMetadata.Name, syncStateMetadata.FolderSyncStateId.ToBase64String());
				syncStateMetadata.ParentDevice.TryRemove(syncStateMetadata.Name, syncLogger);
				syncStateMetadata = syncStateMetadata.ParentDevice.GetSyncState(syncStateParentFolder.Session as MailboxSession, syncStateMetadata.Name, syncLogger);
				return (syncStateMetadata == null) ? null : SyncState.GetSyncStateStoreObject(syncStateParentFolder, ref syncStateMetadata, syncLogger, propsToReturn);
			}
			return storeObject;
		}

		internal bool IsColdDataKey(string key)
		{
			return this.coldDataKeys.ContainsKey(key);
		}

		protected static PropertyDefinition[] AppendAdditionalProperties(params PropertyDefinition[] additionalProperties)
		{
			if (additionalProperties == null)
			{
				return SyncState.storageLocationAsArray;
			}
			PropertyDefinition[] array = new PropertyDefinition[1 + additionalProperties.Length];
			array[0] = SyncStateInfo.StorageLocation;
			Array.Copy(additionalProperties, 0, array, 1, additionalProperties.Length);
			return array;
		}

		protected void AddColdDataKey(string key)
		{
			this.coldDataKeys[key] = true;
		}

		protected virtual void AddColdDataKeys()
		{
		}

		protected void CheckDisposed(string methodName)
		{
			if (this.syncStateIsDisposed)
			{
				StorageGlobals.TraceFailedCheckDisposed(this, methodName);
				throw new ObjectDisposedException(base.GetType().ToString());
			}
		}

		private void WriteSyncStateToItem(PooledMemoryStream syncStateStream)
		{
			if (this.storeObject == null || this.storeObject is Folder)
			{
				throw new InvalidOperationException("Item storage can not be null here");
			}
			using (Stream stream = this.storeObject.OpenPropertyStream(SyncStateInfo.StorageLocation, PropertyOpenMode.Create))
			{
				this.syncLogger.TraceDebug<long, string>(ExTraceGlobals.SyncTracer, 0L, "SyncState::WriteSyncStateToItem. Saving {0} bytes of data to Item property stream for sync state {1}.", syncStateStream.Length, this.syncStateInfo.UniqueName);
				byte[] array = SyncState.transferBufferPool.Acquire();
				try
				{
					Util.StreamHandler.CopyStreamData(syncStateStream, stream, null, 0, array);
				}
				finally
				{
					SyncState.transferBufferPool.Release(array);
				}
			}
		}

		private void WriteSyncStateToFolder(PooledMemoryStream syncStateStream, out StoreObjectId deleteItemId)
		{
			deleteItemId = null;
			if (this.storeObject is Folder)
			{
				this.storeObject[SyncStateInfo.StorageLocation] = syncStateStream.ToArray();
				return;
			}
			using (Item item = this.storeObject as Item)
			{
				this.storeObject = Folder.Bind(this.storeObject.Session, this.storeObject.ParentId, new PropertyDefinition[]
				{
					SyncStateInfo.StorageLocation
				});
				this.storeObject[SyncStateInfo.StorageLocation] = syncStateStream.ToArray();
				deleteItemId = item.Id.ObjectId;
			}
		}

		private void CreateSyncStateDirectItem(PropertyDefinition[] properties, object[] values)
		{
			StoreObjectId storeObjectId = (this.storeObject is Folder) ? this.storeObject.StoreObjectId : this.storeObject.ParentId;
			using (this.storeObject)
			{
				this.storeObject = SyncState.CreateSyncStateItemInFolder(this.syncStateInfo, this.SyncStateMetadata.ParentDevice.DeviceFolderId, this.storeObject.Session as MailboxSession, properties, values, this.syncLogger);
				this.SyncStateMetadata.ItemSyncStateId = this.storeObject.Id.ObjectId;
				this.SyncStateMetadata.FolderSyncStateId = null;
				((Item)this.storeObject).OpenAsReadWrite();
			}
			this.storeObject.Session.Delete(DeleteItemFlags.HardDelete, new StoreId[]
			{
				storeObjectId
			});
		}

		private void CreateSyncStateItemFromDirectItem(PropertyDefinition[] properties, object[] values)
		{
			using (Folder folder = Folder.Bind(this.storeObject.Session, this.SyncStateMetadata.ParentDevice.DeviceFolderId))
			{
				using (Folder folder2 = SyncState.CreateSyncStateFolderInFolder(this.syncStateInfo, folder, null, null, this.syncLogger))
				{
					this.storeObject.Session.Delete(DeleteItemFlags.HardDelete, new StoreId[]
					{
						this.storeObject.Id
					});
					this.storeObject.Dispose();
					this.storeObject = SyncState.CreateSyncStateItemInFolder(this.syncStateInfo, folder2.StoreObjectId, folder2.Session as MailboxSession, properties, values, null);
					this.SyncStateMetadata.ItemSyncStateId = this.storeObject.StoreObjectId;
					this.SyncStateMetadata.FolderSyncStateId = folder2.StoreObjectId;
					((Item)this.storeObject).OpenAsReadWrite();
				}
			}
		}

		private void CreateSyncStateItem(PropertyDefinition[] properties, object[] values, out StoreObjectId clearBlobOnFolderId)
		{
			using (Folder folder = this.storeObject as Folder)
			{
				if (this.SyncStateMetadata.ItemSyncStateId != null)
				{
					ExTraceGlobals.SyncTracer.TraceDebug<string>((long)this.GetHashCode(), "[SyncState.CreateSyncStateItem] storeObject was folder, but metadata had Item.  Opening item.  SyncState: {0}", this.UniqueName);
					SyncStateMetadata syncStateMetadata = this.SyncStateMetadata;
					this.storeObject = SyncState.GetSyncStateStoreObject(folder, ref syncStateMetadata, this.syncLogger, properties);
					if (!object.ReferenceEquals(this.SyncStateMetadata, syncStateMetadata))
					{
						this.SyncStateMetadata = syncStateMetadata;
					}
				}
				else
				{
					ExTraceGlobals.SyncTracer.TraceDebug<string>((long)this.GetHashCode(), "[SyncState.CreateSyncStateItem] storeObject was folder and metadata had null Item.  Creating subitem.  SyncState: {0}", this.UniqueName);
					this.storeObject = SyncState.CreateSyncStateItemInFolder(this.syncStateInfo, folder.StoreObjectId, folder.Session as MailboxSession, properties, values, null);
					this.SyncStateMetadata.ItemSyncStateId = this.storeObject.Id.ObjectId;
				}
				((Item)this.storeObject).OpenAsReadWrite();
				clearBlobOnFolderId = folder.Id.ObjectId;
			}
		}

		private void SaveSyncStateItem(PropertyDefinition[] properties, object[] values, PooledMemoryStream syncStateStream, StoreObjectId clearBlobOnFolderId)
		{
			StoreObjectId storeObjectId = this.storeObject.StoreObjectId;
			StoreSession session = this.storeObject.Session;
			try
			{
				ConflictResolutionResult conflictResolutionResult = ((Item)this.storeObject).Save(this.syncStateInfo.SaveModeForSyncState);
				if (conflictResolutionResult.SaveStatus == SaveResult.IrresolvableConflict)
				{
					throw new SaveConflictException(ServerStrings.SyncStateCollision(this.syncStateInfo.UniqueName), conflictResolutionResult);
				}
			}
			catch (SaveConflictException ex)
			{
				this.syncLogger.TraceDebug<int, string, string>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "SyncState::Commit()::SaveConflictException1. Hashcode = {0}, name={1}, exception={2}", this.GetHashCode(), this.syncStateInfo.UniqueName, ex.ToString());
				throw new SyncStateSaveConflictException(ServerStrings.SyncStateCollision(this.syncStateInfo.UniqueName), ex);
			}
			if (clearBlobOnFolderId != null)
			{
				this.storeObject.Load(new PropertyDefinition[]
				{
					StoreObjectSchema.ParentItemId
				});
				StoreObjectId parentId = this.storeObject.ParentId;
				using (Folder folder = Folder.Bind(session, parentId))
				{
					if (clearBlobOnFolderId != null)
					{
						folder.Delete(SyncStateInfo.StorageLocation);
					}
					folder.Save();
				}
			}
		}

		protected void Commit(PropertyDefinition[] properties, object[] values)
		{
			this.CheckDisposed("Commit");
			this.lastCommittedSize = 0L;
			StoreObjectId clearBlobOnFolderId = null;
			this.BackendVersion = new int?(this.Version);
			PooledMemoryStream pooledMemoryStream = this.Serialize();
			if (this.syncStateStorage.SaveOnDirectItems)
			{
				if (this.SyncStateMetadata.StorageType != StorageType.DirectItem)
				{
					this.CreateSyncStateDirectItem(properties, values);
				}
			}
			else if (this.SyncStateMetadata.StorageType == StorageType.DirectItem)
			{
				this.CreateSyncStateItemFromDirectItem(properties, values);
			}
			else if (this.SyncStateMetadata.StorageType == StorageType.Folder)
			{
				this.CreateSyncStateItem(properties, values, out clearBlobOnFolderId);
			}
			this.WriteSyncStateToItem(pooledMemoryStream);
			if (properties != null && values != null && properties.Length == values.Length)
			{
				for (int i = 0; i < properties.Length; i++)
				{
					this.storeObject[properties[i]] = values[i];
				}
			}
			try
			{
				this.SaveSyncStateItem(properties, values, pooledMemoryStream, clearBlobOnFolderId);
				if (pooledMemoryStream != null)
				{
					this.lastCommittedSize = pooledMemoryStream.Length;
				}
			}
			catch (ObjectNotFoundException innerException)
			{
				throw new SyncStateNotFoundException(ServerStrings.SyncStateMissing(this.syncStateInfo.UniqueName), innerException);
			}
		}

		protected void Deserialize(PropertyDefinition property)
		{
			this.CheckDisposed("Deserialize");
			this.syncStateStorage.CompressedMemoryStream.SetLength(0L);
			if (this.SyncStateMetadata.StorageType == StorageType.Folder)
			{
				object obj = this.storeObject.TryGetProperty(property);
				if (!PropertyError.IsPropertyError(obj))
				{
					byte[] array = obj as byte[];
					this.syncStateStorage.CompressedMemoryStream.Write(array, 0, array.Length);
				}
				else
				{
					if (PropertyError.IsPropertyNotFound(obj))
					{
						this.syncLogger.TraceDebug<string>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "[SyncState.Deserialize] Encountered PropertyError.NotFound trying to deserialize the folder sync state blob on state {0}.  Treating as empty sync state.", this.syncStateInfo.UniqueName);
						this.TreatAsNewSyncState();
						return;
					}
					if (!PropertyError.IsPropertyValueTooBig(obj))
					{
						throw PropertyError.ToException(new PropertyError[]
						{
							obj as PropertyError
						});
					}
					this.syncLogger.TraceError<int, string, PropertyDefinition>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "SyncState::Deserialize. Cannot load folder propback! Hashcode = {0}, Name = {1}, PropertyName = {2}", this.GetHashCode(), this.syncStateInfo.UniqueName, property);
					this.HandleCorruptSyncState();
				}
			}
			else
			{
				try
				{
					using (Stream stream = this.storeObject.OpenPropertyStream(property, PropertyOpenMode.ReadOnly))
					{
						byte[] array2 = SyncState.transferBufferPool.Acquire();
						try
						{
							long arg = Util.StreamHandler.CopyStreamData(stream, this.syncStateStorage.CompressedMemoryStream, null, 0, array2);
							ExTraceGlobals.SyncTracer.TraceDebug<long, string>((long)this.GetHashCode(), "[SyncState.Deserialize] Copied {0} bytes from the property stream for sync state {1}", arg, this.UniqueName);
						}
						finally
						{
							SyncState.transferBufferPool.Release(array2);
						}
					}
				}
				catch (ObjectNotFoundException arg2)
				{
					this.syncLogger.TraceDebug<string, ObjectNotFoundException>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "[SyncState.Deserialize] Encountered ObjectNotFoundException trying to deserialize the sync state blob for state {0}.  Treating as empty sync state.  Exception: {1}", this.syncStateInfo.UniqueName, arg2);
					this.TreatAsNewSyncState();
					return;
				}
			}
			this.syncStateStorage.CompressedMemoryStream.Seek(0L, SeekOrigin.Begin);
			if (this.syncStateStorage.CompressedMemoryStream.Length != 0L)
			{
				try
				{
					try
					{
						this.hotSyncStateTable = SyncState.InternalDeserializeData(this.syncStateStorage.CompressedMemoryStream, out this.internalVersion, out this.externalVersion, out this.hotDataBeginsAt, out this.coldDataBeginsAt, out this.coldDataKeys);
					}
					catch (EndOfStreamException innerException)
					{
						throw new CustomSerializationException(ServerStrings.ExSyncStateCorrupted("SyncState truncated"), innerException);
					}
					return;
				}
				catch (CustomSerializationException ex)
				{
					this.HandleCorruptSyncState(ex);
					return;
				}
			}
			this.syncLogger.TraceDebug(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "[SyncState.Deserialize] Compressed memory stream has zero length for sync state {0}.  Was New? {1}. Metadata type: {2}, StoreObjectType: {3}. Date Created: {4}.  Treating as new sync state.", new object[]
			{
				this.syncStateInfo.UniqueName,
				this.syncStateIsNew,
				this.SyncStateMetadata.StorageType,
				this.storeObject.GetType().Name,
				this.storeObject.CreationTime
			});
			this.TreatAsNewSyncState();
		}

		private void TreatAsNewSyncState()
		{
			this.coldSyncStateTable = new GenericDictionaryData<ConstStringData, string, DerivedData<ICustomSerializableBuilder>>(new Dictionary<string, DerivedData<ICustomSerializableBuilder>>());
			this.CreateNewHotSyncStateTable();
			this.syncStateIsNew = true;
		}

		public static GenericDictionaryData<ConstStringData, string, DerivedData<ICustomSerializableBuilder>> InternalDeserializeData(PooledMemoryStream compressedStream, out int internalVersion, out int externalVersion, out long hotDataBeginsAt, out long coldDataBeginsAt, out Dictionary<string, bool> coldKeys)
		{
			BinaryReader binaryReader = new BinaryReader(compressedStream);
			string text = binaryReader.ReadString();
			if (!int.TryParse(text, out internalVersion) && SyncStateTypeFactory.InternalSignature != text)
			{
				throw new CustomSerializationException(ServerStrings.ExSyncStateCorrupted("internal syncStateSignature"));
			}
			text = binaryReader.ReadString();
			if (!int.TryParse(text, out externalVersion) && SyncStateTypeFactory.ExternalSignature != text)
			{
				throw new CustomSerializationException(ServerStrings.ExSyncStateCorrupted("syncStateSignature"));
			}
			uint num = binaryReader.ReadUInt32();
			uint num2 = ComputeCRC.Compute(0U, compressedStream.GetBuffer(), (int)compressedStream.Position, (int)(compressedStream.Length - compressedStream.Position));
			if (num2 != num)
			{
				throw new CustomSerializationException(ServerStrings.ExSyncStateCorrupted("CRC"));
			}
			hotDataBeginsAt = binaryReader.ReadInt64();
			coldDataBeginsAt = binaryReader.ReadInt64();
			GenericDictionaryData<ConstStringData, string, DerivedData<ICustomSerializableBuilder>> genericDictionaryData = SyncState.DeserializeSyncStateTable(internalVersion, externalVersion, compressedStream, hotDataBeginsAt);
			coldKeys = SyncState.GetData<GenericDictionaryData<StringData, string, BooleanData, bool>, Dictionary<string, bool>>(SyncStateProp.ColdDataKeys, null, genericDictionaryData);
			return genericDictionaryData;
		}

		protected GenericDictionaryData<ConstStringData, string, DerivedData<ICustomSerializableBuilder>> GetSyncStateTableForKey(string key)
		{
			if (!this.IsColdDataKey(key))
			{
				return this.hotSyncStateTable;
			}
			if (this.coldSyncStateTable == null)
			{
				try
				{
					this.coldSyncStateTable = SyncState.DeserializeSyncStateTable(this.internalVersion, this.externalVersion, this.syncStateStorage.CompressedMemoryStream, this.coldDataBeginsAt);
				}
				catch (CustomSerializationException ex)
				{
					this.HandleCorruptSyncState(ex);
				}
				catch (EndOfStreamException ex2)
				{
					this.HandleCorruptSyncState(ex2);
				}
			}
			return this.coldSyncStateTable;
		}

		protected byte[] GetOriginalColdSyncStateBytes()
		{
			long coldStateCompressedSize = this.GetColdStateCompressedSize();
			if (this.syncStateIsNew || coldStateCompressedSize == 0L)
			{
				return SyncState.emptyDictionaryCompressedBytes;
			}
			this.syncStateStorage.CompressedMemoryStream.Seek(this.coldDataBeginsAt, SeekOrigin.Begin);
			byte[] array = new byte[(int)coldStateCompressedSize];
			this.syncStateStorage.CompressedMemoryStream.Read(array, 0, (int)coldStateCompressedSize);
			return array;
		}

		private static byte[] GetEmptyDictionaryCompressedBytes()
		{
			byte[] result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				GenericDictionaryData<ConstStringData, string, DerivedData<ICustomSerializableBuilder>> syncStateTable = new GenericDictionaryData<ConstStringData, string, DerivedData<ICustomSerializableBuilder>>(new Dictionary<string, DerivedData<ICustomSerializableBuilder>>());
				SyncState.SerializeSyncStateTable(syncStateTable, memoryStream);
				result = memoryStream.ToArray();
			}
			return result;
		}

		protected virtual void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				if (this.storeObject != null)
				{
					this.storeObject.Dispose();
				}
				if (this.disposeTracker != null)
				{
					this.disposeTracker.Dispose();
				}
			}
			this.storeObject = null;
			this.syncStateStorage = null;
		}

		protected virtual void Load(bool reloadFromBackend, params PropertyDefinition[] additionalPropsToLoad)
		{
			this.CheckDisposed("Load");
			if (reloadFromBackend)
			{
				PropertyDefinition[] properties = SyncState.AppendAdditionalProperties(additionalPropsToLoad);
				this.storeObject.Load(properties);
				this.syncStateIsNew = false;
			}
			if (this.KeepCachedDataOnReload)
			{
				return;
			}
			if (this.syncStateIsNew)
			{
				this.TreatAsNewSyncState();
			}
			else
			{
				try
				{
					this.totalLoadCount++;
					this.Deserialize(SyncStateInfo.StorageLocation);
				}
				catch (Exception ex)
				{
					this.HandleCorruptSyncState(ex);
				}
			}
			int? num = null;
			try
			{
				bool flag = false;
				ExTraceGlobals.FaultInjectionTracer.TraceTest<bool>(2164665661U, ref flag);
				if (flag)
				{
					int? backendVersion = this.BackendVersion;
					try
					{
						this[SyncStateProp.Version] = new NullableData<DateTimeData, ExDateTime>(new ExDateTime?(ExDateTime.UtcNow));
						num = this.BackendVersion;
					}
					finally
					{
						this.BackendVersion = backendVersion;
					}
				}
				num = this.BackendVersion;
			}
			catch (InvalidCastException ex2)
			{
				this.HandleCorruptSyncState(ex2);
			}
			if (num == null && !this.syncStateIsNew)
			{
				this.HandleCorruptSyncState();
			}
			if (num != null)
			{
				if (num.Value > this.Version)
				{
					throw new InvalidSyncStateVersionException(ServerStrings.ExNewerVersionedSyncState(this.UniqueName, num.Value, this.Version));
				}
				this.syncStateInfo.HandleSyncStateVersioning(this);
			}
		}

		protected PooledMemoryStream Serialize()
		{
			this.CheckDisposed("Serialize");
			this.totalSaveCount++;
			this.CommitStateModifications();
			byte[] array = null;
			if (this.coldSyncStateTable == null || this.coldSyncStateTable.Data == null || this.coldSyncStateTable.Data.Count == 0)
			{
				array = this.GetOriginalColdSyncStateBytes();
			}
			this.lastUncompressedSize = 0L;
			this.syncStateStorage.CompressedMemoryStream.SetLength(0L);
			BinaryWriter binaryWriter = new BinaryWriter(this.syncStateStorage.CompressedMemoryStream);
			binaryWriter.Write(SyncStateTypeFactory.InternalVersion.ToString(CultureInfo.InvariantCulture));
			binaryWriter.Write(SyncStateTypeFactory.ExternalVersion.ToString(CultureInfo.InvariantCulture));
			uint value = 0U;
			long position = this.syncStateStorage.CompressedMemoryStream.Position;
			binaryWriter.Write(value);
			long position2 = this.syncStateStorage.CompressedMemoryStream.Position;
			binaryWriter.Write(0UL);
			long position3 = this.syncStateStorage.CompressedMemoryStream.Position;
			binaryWriter.Write(0UL);
			this.coldDataBeginsAt = this.syncStateStorage.CompressedMemoryStream.Position;
			if (array != null)
			{
				this.coldCopyCount++;
				this.lastUncompressedSize += (long)array.Length;
				this.syncStateStorage.CompressedMemoryStream.Write(array, 0, array.Length);
			}
			else
			{
				this.coldSaveCount++;
				this.lastUncompressedSize += (long)SyncState.SerializeSyncStateTable(this.coldSyncStateTable, this.syncStateStorage.CompressedMemoryStream);
			}
			this.hotDataBeginsAt = this.syncStateStorage.CompressedMemoryStream.Position;
			this.lastUncompressedSize += (long)SyncState.SerializeSyncStateTable(this.hotSyncStateTable, this.syncStateStorage.CompressedMemoryStream);
			this.syncStateStorage.CompressedMemoryStream.Seek(position3, SeekOrigin.Begin);
			binaryWriter.Write(this.coldDataBeginsAt);
			this.syncStateStorage.CompressedMemoryStream.Seek(position2, SeekOrigin.Begin);
			binaryWriter.Write(this.hotDataBeginsAt);
			value = ComputeCRC.Compute(0U, this.syncStateStorage.CompressedMemoryStream.GetBuffer(), (int)position + 4, (int)(this.syncStateStorage.CompressedMemoryStream.Length - position - 4L));
			this.syncStateStorage.CompressedMemoryStream.Seek(position, SeekOrigin.Begin);
			binaryWriter.Write(value);
			this.syncStateStorage.CompressedMemoryStream.Seek(0L, SeekOrigin.Begin);
			return this.syncStateStorage.CompressedMemoryStream;
		}

		private static Folder CreateSyncStateFolderInFolder(SyncStateInfo syncStateInfo, Folder syncStateParentFolder, PropertyDefinition[] properties, object[] values, ISyncLogger syncLogger = null)
		{
			Folder folder = null;
			bool flag = false;
			Folder result;
			try
			{
				try
				{
					folder = Folder.Create(syncStateParentFolder.Session, syncStateParentFolder.Id, StoreObjectType.Folder, syncStateInfo.UniqueName, CreateMode.CreateNew);
					if (properties != null && values != null && properties.Length == values.Length)
					{
						for (int i = 0; i < properties.Length; i++)
						{
							folder[properties[i]] = values[i];
						}
					}
					folder.Save();
				}
				catch (ObjectExistedException innerException)
				{
					throw new SyncStateExistedException(ServerStrings.ExSyncStateAlreadyExists(syncStateInfo.UniqueName), innerException);
				}
				folder.Load(SyncState.AppendAdditionalProperties(properties));
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

		private static Item CreateSyncStateItemInFolder(SyncStateInfo syncStateInfo, StoreObjectId parentFolderId, MailboxSession mailboxSession, PropertyDefinition[] properties, object[] values, ISyncLogger syncLogger = null)
		{
			MessageItem messageItem = null;
			bool flag = false;
			Item result;
			try
			{
				messageItem = MessageItem.Create(mailboxSession, parentFolderId);
				messageItem.ClassName = "Exchange.ContentsSyncData";
				messageItem[InternalSchema.Subject] = syncStateInfo.UniqueName;
				if (properties != null && values != null && properties.Length == values.Length)
				{
					for (int i = 0; i < properties.Length; i++)
					{
						messageItem[properties[i]] = values[i];
					}
				}
				messageItem.Save(SaveMode.NoConflictResolution);
				messageItem.Load(SyncState.AppendAdditionalProperties(properties));
				flag = true;
				result = messageItem;
			}
			catch (ObjectNotFoundException innerException)
			{
				throw new SyncStateNotFoundException(ServerStrings.SyncStateMissing(syncStateInfo.UniqueName), innerException);
			}
			finally
			{
				if (!flag && messageItem != null)
				{
					messageItem.Dispose();
					messageItem = null;
				}
			}
			return result;
		}

		private void CommitStateModifications()
		{
			if (this.commitStateModificationsDelegate != null)
			{
				this.commitStateModificationsDelegate();
			}
		}

		private void CreateNewHotSyncStateTable()
		{
			this.hotSyncStateTable = new GenericDictionaryData<ConstStringData, string, DerivedData<ICustomSerializableBuilder>>(new Dictionary<string, DerivedData<ICustomSerializableBuilder>>());
			this.coldDataKeys = new Dictionary<string, bool>(10);
			this.AddColdDataKeys();
			this[SyncStateProp.ColdDataKeys] = new GenericDictionaryData<StringData, string, BooleanData, bool>(this.coldDataKeys);
		}

		private static GenericDictionaryData<ConstStringData, string, DerivedData<ICustomSerializableBuilder>> DeserializeSyncStateTable(int internalVersion, int externalVersion, PooledMemoryStream memoryStream, long idxTable)
		{
			GenericDictionaryData<ConstStringData, string, DerivedData<ICustomSerializableBuilder>> genericDictionaryData = null;
			memoryStream.Seek(idxTable, SeekOrigin.Begin);
			SyncState.SyncStateAdapter syncStateAdapter = null;
			try
			{
				syncStateAdapter = SyncState.SyncStateAdapterPool.Acquire();
				syncStateAdapter.ComponentDataPool.InternalVersion = internalVersion;
				syncStateAdapter.ComponentDataPool.ExternalVersion = externalVersion;
				syncStateAdapter.Initialize(memoryStream, CompressionMode.Decompress);
				genericDictionaryData = new GenericDictionaryData<ConstStringData, string, DerivedData<ICustomSerializableBuilder>>();
				genericDictionaryData.DeserializeData(syncStateAdapter.BinaryReader, syncStateAdapter.ComponentDataPool);
			}
			finally
			{
				if (syncStateAdapter != null)
				{
					syncStateAdapter.CloseStream();
					SyncState.SyncStateAdapterPool.Release(syncStateAdapter);
				}
			}
			return genericDictionaryData;
		}

		private void Dispose(bool disposing)
		{
			StorageGlobals.TraceDispose(this, this.syncStateIsDisposed, disposing);
			if (!this.syncStateIsDisposed)
			{
				this.syncStateIsDisposed = true;
				this.InternalDispose(disposing);
			}
		}

		private void HandleCorruptSyncState(Exception ex)
		{
			ExTraceGlobals.SyncTracer.TraceError<string, bool, string>((long)this.GetHashCode(), "[SyncState.HandleCorruptSyncState] SyncState {0} was corrupt.  Deleting all sync states? {1}.  Inner exception: {2}", this.syncStateInfo.UniqueName, !this.syncStateInfo.ReadOnly, (ex == null) ? "NULL" : ex.ToString());
			if (!this.syncStateInfo.ReadOnly)
			{
				this.syncStateStorage.DeleteAllSyncStates();
			}
			if (ex != null)
			{
				throw new CorruptSyncStateException(this.UniqueName, ServerStrings.ExSyncStateCorrupted(this.UniqueName), ex);
			}
			throw new CorruptSyncStateException(this.UniqueName, ServerStrings.ExSyncStateCorrupted(this.UniqueName));
		}

		private static int SerializeSyncStateTable(GenericDictionaryData<ConstStringData, string, DerivedData<ICustomSerializableBuilder>> syncStateTable, Stream compressedStream)
		{
			SyncState.SyncStateAdapter syncStateAdapter = null;
			int uncompressedByteCount;
			try
			{
				syncStateAdapter = SyncState.SyncStateAdapterPool.Acquire();
				syncStateAdapter.ComponentDataPool.InternalVersion = SyncStateTypeFactory.InternalVersion;
				syncStateAdapter.ComponentDataPool.ExternalVersion = SyncStateTypeFactory.ExternalVersion;
				syncStateAdapter.Initialize(compressedStream, CompressionMode.Compress);
				syncStateTable.SerializeData(syncStateAdapter.BinaryWriter, syncStateAdapter.ComponentDataPool);
				uncompressedByteCount = syncStateAdapter.UncompressedByteCount;
			}
			finally
			{
				if (syncStateAdapter != null)
				{
					syncStateAdapter.CloseStream();
					SyncState.SyncStateAdapterPool.Release(syncStateAdapter);
				}
			}
			return uncompressedByteCount;
		}

		internal const uint LidCorruptBackEndVersionType = 2164665661U;

		private const long FolderPropertySizeLimit = 30720L;

		private readonly DisposeTracker disposeTracker;

		private static readonly ThrottlingObjectPool<SyncState.SyncStateAdapter> SyncStateAdapterPool = new ThrottlingObjectPool<SyncState.SyncStateAdapter>(Environment.ProcessorCount);

		private static BufferPool transferBufferPool = new BufferPool(4096, false);

		private static readonly byte[] emptyDictionaryCompressedBytes = SyncState.GetEmptyDictionaryCompressedBytes();

		private static readonly PropertyDefinition[] storageLocationAsArray = new PropertyDefinition[]
		{
			SyncStateInfo.StorageLocation
		};

		private int internalVersion;

		private int externalVersion;

		private bool syncStateIsNew;

		private int totalSaveCount;

		private int coldSaveCount;

		private int coldCopyCount;

		private int totalLoadCount;

		private StoreObject storeObject;

		private SyncStateInfo syncStateInfo;

		private SyncStateStorage syncStateStorage;

		private long coldDataBeginsAt;

		private long hotDataBeginsAt;

		private Dictionary<string, bool> coldDataKeys = new Dictionary<string, bool>();

		private GenericDictionaryData<ConstStringData, string, DerivedData<ICustomSerializableBuilder>> coldSyncStateTable;

		private FolderSyncStateUtil.CommitStateModificationsDelegate commitStateModificationsDelegate;

		private GenericDictionaryData<ConstStringData, string, DerivedData<ICustomSerializableBuilder>> hotSyncStateTable;

		private bool syncStateIsDisposed;

		private bool keepCachedDataOnReload;

		private long lastCommittedSize;

		private long lastUncompressedSize;

		protected ISyncLogger syncLogger;

		private class SyncStateAdapter : Stream
		{
			public SyncStateAdapter()
			{
				this.componentDataPool = new ComponentDataPool();
			}

			private BufferedStream Stream
			{
				get
				{
					if (this.stream == null)
					{
						throw new InvalidOperationException("Stream is null.  Make sure to create a compressor or decompressor before using Reader/Writer!");
					}
					return this.stream;
				}
			}

			public BinaryReader BinaryReader
			{
				get
				{
					if (this.binaryReader == null)
					{
						this.binaryReader = new BinaryReader(this);
					}
					return this.binaryReader;
				}
			}

			public BinaryWriter BinaryWriter
			{
				get
				{
					if (this.binaryWriter == null)
					{
						this.binaryWriter = new BinaryWriter(this, new UTF8Encoding(false, false));
					}
					return this.binaryWriter;
				}
			}

			public ComponentDataPool ComponentDataPool
			{
				get
				{
					return this.componentDataPool;
				}
			}

			public void Initialize(Stream underlyingStream, CompressionMode compressionMode)
			{
				if (this.stream != null)
				{
					throw new InvalidOperationException("You must close any outstanding streams before attempting to create a new one.");
				}
				this.UncompressedByteCount = 0;
				this.stream = new BufferedStream(new GZipStream(underlyingStream, compressionMode, true));
			}

			public void CloseStream()
			{
				if (this.stream != null)
				{
					this.stream.Flush();
					this.stream.Dispose();
					this.stream = null;
				}
			}

			protected override void Dispose(bool disposing)
			{
				if (disposing)
				{
					this.CloseStream();
					if (this.binaryReader != null)
					{
						this.binaryReader.Close();
						this.binaryReader = null;
					}
					if (this.binaryWriter != null)
					{
						this.binaryWriter.Close();
						this.binaryWriter = null;
					}
				}
				base.Dispose(disposing);
			}

			public override bool CanRead
			{
				get
				{
					return this.Stream.CanRead;
				}
			}

			public override bool CanSeek
			{
				get
				{
					return this.Stream.CanSeek;
				}
			}

			public override bool CanWrite
			{
				get
				{
					return this.Stream.CanWrite;
				}
			}

			public override void Flush()
			{
				this.Stream.Flush();
			}

			public override long Length
			{
				get
				{
					return this.Stream.Length;
				}
			}

			public override long Position
			{
				get
				{
					return this.Stream.Position;
				}
				set
				{
					this.Stream.Position = value;
				}
			}

			public int UncompressedByteCount { get; private set; }

			public override long Seek(long offset, SeekOrigin origin)
			{
				return this.Stream.Seek(offset, origin);
			}

			public override void SetLength(long value)
			{
				this.Stream.SetLength(value);
			}

			public override int Read(byte[] outBuffer, int offset, int count)
			{
				int num = this.stream.Read(outBuffer, offset, count);
				this.UncompressedByteCount += num;
				return num;
			}

			public override void Write(byte[] buffer, int offset, int count)
			{
				this.stream.Write(buffer, offset, count);
				this.UncompressedByteCount += count;
			}

			private BufferedStream stream;

			private BinaryReader binaryReader;

			private BinaryWriter binaryWriter;

			private ComponentDataPool componentDataPool;
		}
	}
}
