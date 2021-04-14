using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal abstract class ServicesSyncStateBase : ISyncState
	{
		public ServicesSyncStateBase(StoreObjectId folderId, ISyncProvider syncProvider)
		{
			ExTraceGlobals.SynchronizationTracer.TraceDebug((long)this.GetHashCode(), "ServicesSyncStateBase constructor called");
			this.syncProvider = syncProvider;
			this.syncStateTable = new GenericDictionaryData<StringData, string, DerivedData<ICustomSerializableBuilder>>(new Dictionary<string, DerivedData<ICustomSerializableBuilder>>());
			this.folderId = folderId;
		}

		public int Version
		{
			get
			{
				return this.version;
			}
			set
			{
				this.version = value;
			}
		}

		public int? BackendVersion
		{
			get
			{
				return null;
			}
		}

		public ICustomSerializableBuilder this[string key]
		{
			get
			{
				if (!this.syncStateTable.Data.ContainsKey(key))
				{
					return null;
				}
				return this.syncStateTable.Data[key].Data;
			}
			set
			{
				this.syncStateTable.Data[key] = new DerivedData<ICustomSerializableBuilder>(value);
			}
		}

		public bool Contains(string key)
		{
			return this.syncStateTable.Data.ContainsKey(key);
		}

		public void Remove(string key)
		{
			this.syncStateTable.Data.Remove(key);
		}

		public string SerializeAsBase64String()
		{
			ExTraceGlobals.SynchronizationTracer.TraceDebug((long)this.GetHashCode(), "ServicesFolderSyncState.SerializeAsBase64String called.");
			SyncStateTypeFactory.GetInstance().RegisterInternalBuilders();
			ComponentDataPool componentDataPool = new ComponentDataPool();
			string result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
				{
					this.syncStateTable.SerializeData(binaryWriter, componentDataPool);
					using (MemoryStream memoryStream2 = new MemoryStream())
					{
						memoryStream.Seek(0L, SeekOrigin.Begin);
						SerializationHelper.Compress(memoryStream, memoryStream2);
						result = Convert.ToBase64String(memoryStream2.ToArray());
					}
				}
			}
			return result;
		}

		protected void Load(string base64SyncData)
		{
			byte[] array = ServicesSyncStateBase.ConvertBase64SyncStateData(base64SyncData);
			ExTraceGlobals.SynchronizationTracer.TraceDebug((long)this.GetHashCode(), "ServicesFolderSyncState.Load called.");
			GenericDictionaryData<StringData, string, DerivedData<ICustomSerializableBuilder>> genericDictionaryData = null;
			try
			{
				SyncStateTypeFactory.GetInstance().RegisterInternalBuilders();
				if (array != null && array.Length > 0)
				{
					using (MemoryStream memoryStream = new MemoryStream(array))
					{
						using (MemoryStream memoryStream2 = new MemoryStream())
						{
							using (BinaryReader binaryReader = new BinaryReader(memoryStream2))
							{
								ComponentDataPool componentDataPool = new ComponentDataPool();
								byte[] transferBuffer = new byte[71680];
								SerializationHelper.Decompress(memoryStream, memoryStream2, transferBuffer);
								memoryStream2.Seek(0L, SeekOrigin.Begin);
								GenericDictionaryData<StringData, string, DerivedData<ICustomSerializableBuilder>> genericDictionaryData2 = new GenericDictionaryData<StringData, string, DerivedData<ICustomSerializableBuilder>>();
								genericDictionaryData2.DeserializeData(binaryReader, componentDataPool);
								genericDictionaryData = genericDictionaryData2;
							}
						}
					}
					if (genericDictionaryData == null)
					{
						throw new InvalidSyncStateDataException();
					}
				}
				else
				{
					genericDictionaryData = new GenericDictionaryData<StringData, string, DerivedData<ICustomSerializableBuilder>>(new Dictionary<string, DerivedData<ICustomSerializableBuilder>>());
					this.InitializeSyncState(genericDictionaryData.Data);
				}
				this.VerifySyncState(genericDictionaryData.Data);
			}
			catch (CustomSerializationException innerException)
			{
				throw new InvalidSyncStateDataException(innerException);
			}
			catch (ArgumentException innerException2)
			{
				throw new InvalidSyncStateDataException(innerException2);
			}
			catch (EndOfStreamException innerException3)
			{
				throw new InvalidSyncStateDataException(innerException3);
			}
			this.syncStateTable = genericDictionaryData;
			ExTraceGlobals.SynchronizationTracer.TraceDebug((long)this.GetHashCode(), "ServicesFolderSyncState.Load successful.");
		}

		private static byte[] ConvertBase64SyncStateData(string base64SyncData)
		{
			byte[] result;
			try
			{
				if (string.IsNullOrEmpty(base64SyncData))
				{
					result = null;
				}
				else
				{
					byte[] array = Convert.FromBase64String(base64SyncData);
					if (array == null || array.Length == 0)
					{
						throw new InvalidSyncStateDataException();
					}
					result = array;
				}
			}
			catch (FormatException innerException)
			{
				throw new InvalidSyncStateDataException(innerException);
			}
			return result;
		}

		protected virtual void InitializeSyncState(Dictionary<string, DerivedData<ICustomSerializableBuilder>> obj)
		{
			obj.Add("{49A4350A-C7A8-4AC3-BCBC-F2B8CB7F9550}WS.SyncStateCanary", new DerivedData<ICustomSerializableBuilder>(this.SyncStateTag));
			obj.Add("{49A4350A-C7A8-4AC3-BCBC-F2B8CB7F9550}WS.SyncStateFolderId", new DerivedData<ICustomSerializableBuilder>(this.SyncStoreFolderId));
			obj.Add("{49A4350A-C7A8-4AC3-BCBC-F2B8CB7F9550}WS.SyncStateVersion", new DerivedData<ICustomSerializableBuilder>(this.SyncStoreVersion));
		}

		protected void InitializeSyncStateTable()
		{
			GenericDictionaryData<StringData, string, DerivedData<ICustomSerializableBuilder>> genericDictionaryData = new GenericDictionaryData<StringData, string, DerivedData<ICustomSerializableBuilder>>(new Dictionary<string, DerivedData<ICustomSerializableBuilder>>());
			this.InitializeSyncState(genericDictionaryData.Data);
			this.syncStateTable = genericDictionaryData;
		}

		protected virtual void VerifySyncState(Dictionary<string, DerivedData<ICustomSerializableBuilder>> obj)
		{
			StringData stringData = obj["{49A4350A-C7A8-4AC3-BCBC-F2B8CB7F9550}WS.SyncStateCanary"].Data as StringData;
			StoreObjectIdData storeObjectIdData = obj["{49A4350A-C7A8-4AC3-BCBC-F2B8CB7F9550}WS.SyncStateFolderId"].Data as StoreObjectIdData;
			Int32Data int32Data = obj["{49A4350A-C7A8-4AC3-BCBC-F2B8CB7F9550}WS.SyncStateVersion"].Data as Int32Data;
			if (stringData == null || !this.SyncStateTag.Data.Equals(stringData.Data))
			{
				throw new InvalidSyncStateDataException();
			}
			if (storeObjectIdData == null || !this.SyncStoreFolderId.Data.Equals(storeObjectIdData.Data))
			{
				throw new InvalidSyncStateDataException();
			}
			if (int32Data == null)
			{
				throw new InvalidSyncStateDataException();
			}
			this.version = int32Data.Data;
		}

		internal StoreObjectIdData SyncStoreFolderId
		{
			get
			{
				return new StoreObjectIdData(this.folderId);
			}
		}

		internal Int32Data SyncStoreVersion
		{
			get
			{
				return new Int32Data(this.version);
			}
		}

		internal abstract StringData SyncStateTag { get; }

		protected const string SyncStateGuid = "{49A4350A-C7A8-4AC3-BCBC-F2B8CB7F9550}";

		private const string SyncStateTagKeyName = "{49A4350A-C7A8-4AC3-BCBC-F2B8CB7F9550}WS.SyncStateCanary";

		private const string SyncStateFolderIdKeyName = "{49A4350A-C7A8-4AC3-BCBC-F2B8CB7F9550}WS.SyncStateFolderId";

		protected const string SyncStateVersionKeyName = "{49A4350A-C7A8-4AC3-BCBC-F2B8CB7F9550}WS.SyncStateVersion";

		private GenericDictionaryData<StringData, string, DerivedData<ICustomSerializableBuilder>> syncStateTable;

		private int version;

		protected ISyncProvider syncProvider;

		protected StoreObjectId folderId;
	}
}
