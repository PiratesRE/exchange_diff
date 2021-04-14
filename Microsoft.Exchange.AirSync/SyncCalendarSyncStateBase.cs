using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.AirSync;

namespace Microsoft.Exchange.AirSync
{
	internal abstract class SyncCalendarSyncStateBase : ISyncState
	{
		public SyncCalendarSyncStateBase(StoreObjectId folderId, ISyncProvider syncProvider)
		{
			ExTraceGlobals.MethodEnterExitTracer.TraceDebug((long)this.GetHashCode(), "SyncCalendarSyncStateBase constructor called");
			this.syncProvider = syncProvider;
			this.syncStateTable = new GenericDictionaryData<StringData, string, DerivedData<ICustomSerializableBuilder>>(new Dictionary<string, DerivedData<ICustomSerializableBuilder>>());
			this.folderId = folderId;
		}

		public int Version { get; set; }

		public int? BackendVersion
		{
			get
			{
				return null;
			}
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
				return new Int32Data(this.Version);
			}
		}

		internal abstract StringData SyncStateTag { get; }

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
			ExTraceGlobals.MethodEnterExitTracer.TraceDebug((long)this.GetHashCode(), "SyncCalendarSyncStateBase.SerializeAsBase64String called.");
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
			byte[] array = SyncCalendarSyncStateBase.ConvertBase64SyncStateData(base64SyncData);
			ExTraceGlobals.MethodEnterExitTracer.TraceDebug((long)this.GetHashCode(), "SyncCalendarSyncStateBase.Load called.");
			GenericDictionaryData<StringData, string, DerivedData<ICustomSerializableBuilder>> genericDictionaryData = null;
			try
			{
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
						throw new CorruptSyncStateException("No SyncStateDictionaryData", null);
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
				throw new CorruptSyncStateException("SyncCalendarSyncStateBase.Load caught", innerException);
			}
			catch (ArgumentException innerException2)
			{
				throw new CorruptSyncStateException("SyncCalendarSyncStateBase.Load caught", innerException2);
			}
			catch (EndOfStreamException innerException3)
			{
				throw new CorruptSyncStateException("SyncCalendarSyncStateBase.Load caught", innerException3);
			}
			this.syncStateTable = genericDictionaryData;
			ExTraceGlobals.MethodEnterExitTracer.TraceDebug((long)this.GetHashCode(), "SyncCalendarSyncStateBase.Load successful.");
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
						throw new CorruptSyncStateException("result is empty", null);
					}
					result = array;
				}
			}
			catch (FormatException innerException)
			{
				throw new CorruptSyncStateException("SyncCalendarSyncStateBase.ConvertBase64SyncStateData caught", innerException);
			}
			return result;
		}

		protected virtual void InitializeSyncState(Dictionary<string, DerivedData<ICustomSerializableBuilder>> obj)
		{
			obj.Add("{9150227d-9140-45d0-b4c2-e987f59cfc46}SyncCalendar.SyncStateCanary", new DerivedData<ICustomSerializableBuilder>(this.SyncStateTag));
			obj.Add("{9150227d-9140-45d0-b4c2-e987f59cfc46}SyncCalendar.SyncStateFolderId", new DerivedData<ICustomSerializableBuilder>(this.SyncStoreFolderId));
			obj.Add("{9150227d-9140-45d0-b4c2-e987f59cfc46}SyncCalendar.SyncStateVersion", new DerivedData<ICustomSerializableBuilder>(this.SyncStoreVersion));
		}

		protected void InitializeSyncStateTable()
		{
			GenericDictionaryData<StringData, string, DerivedData<ICustomSerializableBuilder>> genericDictionaryData = new GenericDictionaryData<StringData, string, DerivedData<ICustomSerializableBuilder>>(new Dictionary<string, DerivedData<ICustomSerializableBuilder>>());
			this.InitializeSyncState(genericDictionaryData.Data);
			this.syncStateTable = genericDictionaryData;
		}

		protected virtual void VerifySyncState(Dictionary<string, DerivedData<ICustomSerializableBuilder>> obj)
		{
			StringData stringData = obj["{9150227d-9140-45d0-b4c2-e987f59cfc46}SyncCalendar.SyncStateCanary"].Data as StringData;
			StoreObjectIdData storeObjectIdData = obj["{9150227d-9140-45d0-b4c2-e987f59cfc46}SyncCalendar.SyncStateFolderId"].Data as StoreObjectIdData;
			Int32Data int32Data = obj["{9150227d-9140-45d0-b4c2-e987f59cfc46}SyncCalendar.SyncStateVersion"].Data as Int32Data;
			if (stringData == null || !this.SyncStateTag.Data.Equals(stringData.Data))
			{
				throw new CorruptSyncStateException("tagKey is invalid", null);
			}
			if (storeObjectIdData == null || !this.SyncStoreFolderId.Data.Equals(storeObjectIdData.Data))
			{
				throw new CorruptSyncStateException("storeId is invalid", null);
			}
			if (int32Data == null)
			{
				throw new CorruptSyncStateException("storedVersion is invalid", null);
			}
			this.Version = int32Data.Data;
		}

		protected const string SyncStateGuid = "{9150227d-9140-45d0-b4c2-e987f59cfc46}";

		private const string SyncStateTagKeyName = "{9150227d-9140-45d0-b4c2-e987f59cfc46}SyncCalendar.SyncStateCanary";

		private const string SyncStateFolderIdKeyName = "{9150227d-9140-45d0-b4c2-e987f59cfc46}SyncCalendar.SyncStateFolderId";

		protected const string SyncStateVersionKeyName = "{9150227d-9140-45d0-b4c2-e987f59cfc46}SyncCalendar.SyncStateVersion";

		private GenericDictionaryData<StringData, string, DerivedData<ICustomSerializableBuilder>> syncStateTable;

		protected ISyncProvider syncProvider;

		protected StoreObjectId folderId;
	}
}
