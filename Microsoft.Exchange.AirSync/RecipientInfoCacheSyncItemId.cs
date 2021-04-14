using System;
using System.IO;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.AirSync
{
	internal sealed class RecipientInfoCacheSyncItemId : ISyncItemId, ICustomSerializableBuilder, ICustomSerializable
	{
		public RecipientInfoCacheSyncItemId()
		{
		}

		public RecipientInfoCacheSyncItemId(int cacheEntryId)
		{
			if (cacheEntryId <= 0)
			{
				throw new ArgumentException("cacheEntryId is less than or equal to 0");
			}
			this.cacheEntryId = cacheEntryId;
		}

		public object NativeId
		{
			get
			{
				return this.cacheEntryId;
			}
		}

		public int CacheEntryId
		{
			get
			{
				return this.cacheEntryId;
			}
		}

		public ushort TypeId
		{
			get
			{
				return RecipientInfoCacheSyncItemId.typeId;
			}
			set
			{
				RecipientInfoCacheSyncItemId.typeId = value;
			}
		}

		public ICustomSerializable BuildObject()
		{
			return new RecipientInfoCacheSyncItemId();
		}

		public void DeserializeData(BinaryReader reader, ComponentDataPool componentDataPool)
		{
			Int32Data int32DataInstance = componentDataPool.GetInt32DataInstance();
			int32DataInstance.DeserializeData(reader, componentDataPool);
			this.cacheEntryId = int32DataInstance.Data;
		}

		public void SerializeData(BinaryWriter writer, ComponentDataPool componentDataPool)
		{
			componentDataPool.GetInt32DataInstance().Bind(this.cacheEntryId).SerializeData(writer, componentDataPool);
		}

		public override bool Equals(object syncItemId)
		{
			RecipientInfoCacheSyncItemId recipientInfoCacheSyncItemId = syncItemId as RecipientInfoCacheSyncItemId;
			return recipientInfoCacheSyncItemId != null && this.cacheEntryId == recipientInfoCacheSyncItemId.cacheEntryId;
		}

		public override int GetHashCode()
		{
			return this.cacheEntryId.GetHashCode();
		}

		private static ushort typeId;

		private int cacheEntryId = -1;
	}
}
