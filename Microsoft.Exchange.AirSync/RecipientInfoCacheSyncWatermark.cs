using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.AirSync
{
	internal sealed class RecipientInfoCacheSyncWatermark : ISyncWatermark, ICustomSerializableBuilder, ICustomSerializable, IComparable, ICloneable
	{
		public RecipientInfoCacheSyncWatermark()
		{
			this.cacheEntryIdToLastUpdateTime = new Dictionary<RecipientInfoCacheSyncItemId, long>(100);
			this.lastModifiedTime = ExDateTime.MinValue;
		}

		private RecipientInfoCacheSyncWatermark(Dictionary<RecipientInfoCacheSyncItemId, long> entries, ExDateTime lastModifiedTime)
		{
			this.cacheEntryIdToLastUpdateTime = entries;
			this.lastModifiedTime = lastModifiedTime;
		}

		public bool IsNew
		{
			get
			{
				return this.lastModifiedTime == ExDateTime.MinValue;
			}
		}

		public ExDateTime LastModifiedTime
		{
			get
			{
				return this.lastModifiedTime;
			}
			set
			{
				this.lastModifiedTime = value;
			}
		}

		public ushort TypeId
		{
			get
			{
				return RecipientInfoCacheSyncWatermark.typeId;
			}
			set
			{
				RecipientInfoCacheSyncWatermark.typeId = value;
			}
		}

		internal Dictionary<RecipientInfoCacheSyncItemId, long> Entries
		{
			get
			{
				return this.cacheEntryIdToLastUpdateTime;
			}
		}

		public static RecipientInfoCacheSyncWatermark Create()
		{
			return new RecipientInfoCacheSyncWatermark();
		}

		public static RecipientInfoCacheSyncWatermark Create(Dictionary<RecipientInfoCacheSyncItemId, long> dictionary, ExDateTime lastModifiedTime)
		{
			if (dictionary == null)
			{
				throw new ArgumentNullException("dictionary");
			}
			return new RecipientInfoCacheSyncWatermark(dictionary, lastModifiedTime);
		}

		public static RecipientInfoCacheSyncWatermark Create(List<RecipientInfoCacheEntry> cache, ExDateTime lastModifiedTime)
		{
			Dictionary<RecipientInfoCacheSyncItemId, long> dictionary = new Dictionary<RecipientInfoCacheSyncItemId, long>(cache.Count);
			foreach (RecipientInfoCacheEntry recipientInfoCacheEntry in cache)
			{
				using (RecipientInfoCacheSyncItem recipientInfoCacheSyncItem = RecipientInfoCacheSyncItem.Bind(recipientInfoCacheEntry))
				{
					dictionary[(RecipientInfoCacheSyncItemId)recipientInfoCacheSyncItem.Id] = recipientInfoCacheEntry.DateTimeTicks;
				}
			}
			return new RecipientInfoCacheSyncWatermark(dictionary, lastModifiedTime);
		}

		public ICustomSerializable BuildObject()
		{
			return new RecipientInfoCacheSyncWatermark();
		}

		public void DeserializeData(BinaryReader reader, ComponentDataPool componentDataPool)
		{
			GenericDictionaryData<DerivedData<RecipientInfoCacheSyncItemId>, RecipientInfoCacheSyncItemId, Int64Data, long> genericDictionaryData = new GenericDictionaryData<DerivedData<RecipientInfoCacheSyncItemId>, RecipientInfoCacheSyncItemId, Int64Data, long>();
			genericDictionaryData.DeserializeData(reader, componentDataPool);
			this.cacheEntryIdToLastUpdateTime = genericDictionaryData.Data;
			DateTimeData dateTimeData = new DateTimeData();
			dateTimeData.DeserializeData(reader, componentDataPool);
			this.lastModifiedTime = dateTimeData.Data;
		}

		public void SerializeData(BinaryWriter writer, ComponentDataPool componentDataPool)
		{
			GenericDictionaryData<DerivedData<RecipientInfoCacheSyncItemId>, RecipientInfoCacheSyncItemId, Int64Data, long> genericDictionaryData = new GenericDictionaryData<DerivedData<RecipientInfoCacheSyncItemId>, RecipientInfoCacheSyncItemId, Int64Data, long>();
			genericDictionaryData.Bind(this.cacheEntryIdToLastUpdateTime).SerializeData(writer, componentDataPool);
			DateTimeData dateTimeData = new DateTimeData(this.LastModifiedTime);
			dateTimeData.SerializeData(writer, componentDataPool);
		}

		public int CompareTo(object thatObject)
		{
			RecipientInfoCacheSyncWatermark recipientInfoCacheSyncWatermark = (RecipientInfoCacheSyncWatermark)thatObject;
			return this.lastModifiedTime.CompareTo(recipientInfoCacheSyncWatermark.lastModifiedTime);
		}

		public override bool Equals(object thatObject)
		{
			RecipientInfoCacheSyncWatermark recipientInfoCacheSyncWatermark = thatObject as RecipientInfoCacheSyncWatermark;
			return thatObject != null && this.lastModifiedTime == recipientInfoCacheSyncWatermark.lastModifiedTime;
		}

		public override int GetHashCode()
		{
			throw new NotImplementedException("RecipientInfoCacheSyncWatermark.GetHashCode()");
		}

		public object Clone()
		{
			Dictionary<RecipientInfoCacheSyncItemId, long> dictionary = new Dictionary<RecipientInfoCacheSyncItemId, long>(this.cacheEntryIdToLastUpdateTime.Count);
			foreach (KeyValuePair<RecipientInfoCacheSyncItemId, long> keyValuePair in this.cacheEntryIdToLastUpdateTime)
			{
				dictionary[keyValuePair.Key] = keyValuePair.Value;
			}
			return new RecipientInfoCacheSyncWatermark(dictionary, this.lastModifiedTime);
		}

		private static ushort typeId;

		private Dictionary<RecipientInfoCacheSyncItemId, long> cacheEntryIdToLastUpdateTime;

		private ExDateTime lastModifiedTime;
	}
}
