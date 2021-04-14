using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Sync.Common;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class UnifiedCustomSyncStateItem : ICustomSerializableBuilder, ICustomSerializable
	{
		internal UnifiedCustomSyncStateItem() : this(short.MinValue)
		{
		}

		internal UnifiedCustomSyncStateItem(short version)
		{
			this.version = version;
		}

		internal UnifiedCustomSyncStateItem(StoreObjectId nativeId, byte[] changeKey, StoreObjectId nativeFolderId, string cloudId, string cloudFolderId, string cloudVersion, Dictionary<string, string> properties, short version) : this(version)
		{
			SyncUtilities.ThrowIfArgumentNull("cloudId", cloudId);
			this.nativeId = nativeId;
			this.changeKey = changeKey;
			this.nativeFolderId = nativeFolderId;
			this.cloudId = cloudId;
			this.cloudFolderId = cloudFolderId;
			this.cloudVersion = cloudVersion;
			this.properties = properties;
		}

		public StoreObjectId NativeFolderId
		{
			get
			{
				return this.nativeFolderId;
			}
			set
			{
				this.nativeFolderId = value;
			}
		}

		public StoreObjectId NativeId
		{
			get
			{
				return this.nativeId;
			}
			set
			{
				this.nativeId = value;
			}
		}

		public byte[] ChangeKey
		{
			get
			{
				return this.changeKey;
			}
			set
			{
				this.changeKey = value;
			}
		}

		public string CloudId
		{
			get
			{
				return this.cloudId;
			}
			set
			{
				this.cloudId = value;
			}
		}

		public string CloudFolderId
		{
			get
			{
				return this.cloudFolderId;
			}
			set
			{
				this.cloudFolderId = value;
			}
		}

		public string CloudVersion
		{
			get
			{
				return this.cloudVersion;
			}
			set
			{
				this.cloudVersion = value;
			}
		}

		public Dictionary<string, string> Properties
		{
			get
			{
				return this.properties;
			}
			set
			{
				this.properties = value;
			}
		}

		public ushort TypeId
		{
			get
			{
				return UnifiedCustomSyncStateItem.typeId;
			}
			set
			{
				UnifiedCustomSyncStateItem.typeId = value;
			}
		}

		public ICustomSerializable BuildObject()
		{
			return new UnifiedCustomSyncStateItem();
		}

		public void SerializeData(BinaryWriter writer, ComponentDataPool componentDataPool)
		{
			componentDataPool.GetStoreObjectIdDataInstance().Bind(this.nativeId).SerializeData(writer, componentDataPool);
			componentDataPool.GetByteArrayInstance().Bind(this.changeKey).SerializeData(writer, componentDataPool);
			componentDataPool.GetStoreObjectIdDataInstance().Bind(this.nativeFolderId).SerializeData(writer, componentDataPool);
			componentDataPool.GetStringDataInstance().Bind(this.cloudId).SerializeData(writer, componentDataPool);
			componentDataPool.GetStringDataInstance().Bind(this.cloudFolderId).SerializeData(writer, componentDataPool);
			componentDataPool.GetStringDataInstance().Bind(this.cloudVersion).SerializeData(writer, componentDataPool);
			new GenericDictionaryData<StringData, string, StringData, string>(this.properties).SerializeData(writer, componentDataPool);
		}

		public void DeserializeData(BinaryReader reader, ComponentDataPool componentDataPool)
		{
			StoreObjectIdData storeObjectIdDataInstance = componentDataPool.GetStoreObjectIdDataInstance();
			storeObjectIdDataInstance.DeserializeData(reader, componentDataPool);
			this.nativeId = storeObjectIdDataInstance.Data;
			if (this.version > 0)
			{
				ByteArrayData byteArrayInstance = componentDataPool.GetByteArrayInstance();
				byteArrayInstance.DeserializeData(reader, componentDataPool);
				this.changeKey = byteArrayInstance.Data;
			}
			storeObjectIdDataInstance.DeserializeData(reader, componentDataPool);
			this.nativeFolderId = storeObjectIdDataInstance.Data;
			StringData stringDataInstance = componentDataPool.GetStringDataInstance();
			stringDataInstance.DeserializeData(reader, componentDataPool);
			this.cloudId = stringDataInstance.Data;
			stringDataInstance.DeserializeData(reader, componentDataPool);
			this.cloudFolderId = stringDataInstance.Data;
			stringDataInstance.DeserializeData(reader, componentDataPool);
			this.cloudVersion = stringDataInstance.Data;
			if (this.version <= 2)
			{
				DateTimeData dateTimeDataInstance = componentDataPool.GetDateTimeDataInstance();
				dateTimeDataInstance.DeserializeData(reader, componentDataPool);
			}
			if (this.version >= 3)
			{
				GenericDictionaryData<StringData, string, StringData, string> genericDictionaryData = new GenericDictionaryData<StringData, string, StringData, string>();
				genericDictionaryData.DeserializeData(reader, componentDataPool);
				this.properties = genericDictionaryData.Data;
			}
			if (this.properties == null)
			{
				this.properties = new Dictionary<string, string>(3);
			}
		}

		private const int DefaultEstimatePropertyCapacity = 3;

		private static ushort typeId;

		private StoreObjectId nativeFolderId;

		private StoreObjectId nativeId;

		private byte[] changeKey;

		private string cloudId;

		private string cloudFolderId;

		private string cloudVersion;

		private Dictionary<string, string> properties;

		private short version;
	}
}
