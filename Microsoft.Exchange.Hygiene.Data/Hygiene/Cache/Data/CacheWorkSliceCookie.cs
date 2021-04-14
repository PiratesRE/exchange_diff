using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Hygiene.Data;

namespace Microsoft.Exchange.Hygiene.Cache.Data
{
	[Serializable]
	internal class CacheWorkSliceCookie : ConfigurablePropertyBag
	{
		public CacheWorkSliceCookie()
		{
			this.LastSyncTime = DateTime.MinValue;
			this.LastFullSyncTime = DateTime.MinValue;
			this.CookieOffset = -1L;
			this.DBRead = false;
		}

		public CacheWorkSliceCookie(string entityName, int partitionIndex, CachePrimingMode primingMode)
		{
			this.EntityName = entityName;
			this.PartitionIndex = partitionIndex;
			this.PrimingMode = primingMode;
			this.LastSyncTime = DateTime.MinValue;
			this.LastFullSyncTime = DateTime.MinValue;
			this.CookieOffset = -1L;
			this.DBRead = false;
		}

		public override ObjectId Identity
		{
			get
			{
				return new ConfigObjectId(this.CacheIdentity);
			}
		}

		public string CacheIdentity
		{
			get
			{
				return CacheWorkSliceCookie.ConstructCacheIdentity(this.EntityName, this.PartitionIndex, this.PrimingMode);
			}
		}

		public string EntityName
		{
			get
			{
				return this[CacheWorkSliceCookie.EntityNameProp] as string;
			}
			set
			{
				this[CacheWorkSliceCookie.EntityNameProp] = value;
			}
		}

		public int PartitionIndex
		{
			get
			{
				return (int)this[CacheWorkSliceCookie.PartitionIndexProp];
			}
			set
			{
				this[CacheWorkSliceCookie.PartitionIndexProp] = value;
			}
		}

		public CachePrimingMode PrimingMode
		{
			get
			{
				return (CachePrimingMode)this[CacheWorkSliceCookie.PrimingModeProp];
			}
			set
			{
				this[CacheWorkSliceCookie.PrimingModeProp] = value;
			}
		}

		public string Cookie
		{
			get
			{
				return this[CacheWorkSliceCookie.CookieProp] as string;
			}
			set
			{
				this[CacheWorkSliceCookie.CookieProp] = value;
			}
		}

		public DateTime LastSyncTime
		{
			get
			{
				return (DateTime)this[CacheWorkSliceCookie.LastSyncTimeProp];
			}
			set
			{
				this[CacheWorkSliceCookie.LastSyncTimeProp] = value;
			}
		}

		public DateTime LastFullSyncTime
		{
			get
			{
				return (DateTime)this[CacheWorkSliceCookie.LastFullSyncTimeProp];
			}
			set
			{
				this[CacheWorkSliceCookie.LastFullSyncTimeProp] = value;
			}
		}

		public string CookieFile
		{
			get
			{
				return this[CacheWorkSliceCookie.CookieFileProp] as string;
			}
			set
			{
				this[CacheWorkSliceCookie.CookieFileProp] = value;
			}
		}

		public long CookieOffset
		{
			get
			{
				return (long)this[CacheWorkSliceCookie.CookieOffsetProp];
			}
			set
			{
				this[CacheWorkSliceCookie.CookieOffsetProp] = value;
			}
		}

		public bool DBRead
		{
			get
			{
				return (bool)this[CacheWorkSliceCookie.DBReadProp];
			}
			set
			{
				this[CacheWorkSliceCookie.DBReadProp] = value;
			}
		}

		public static string ConstructCacheIdentity(string entityName, int partitionIndex, CachePrimingMode primingMode)
		{
			return string.Format("{0}:{1}:{2}", entityName, partitionIndex, primingMode);
		}

		protected static readonly HygienePropertyDefinition EntityNameProp = CacheObjectSchema.EntityNameProp;

		protected static readonly HygienePropertyDefinition LastSyncTimeProp = CacheObjectSchema.LastSyncTimeProp;

		protected static readonly HygienePropertyDefinition LastFullSyncTimeProp = CacheObjectSchema.LastFullSyncTimeProp;

		protected static readonly HygienePropertyDefinition PartitionIndexProp = new HygienePropertyDefinition("PartitionIndex", typeof(int), -1, ADPropertyDefinitionFlags.PersistDefaultValue);

		protected static readonly HygienePropertyDefinition PrimingModeProp = new HygienePropertyDefinition("PrimingMode", typeof(CachePrimingMode));

		protected static readonly HygienePropertyDefinition CookieProp = new HygienePropertyDefinition("Cookie", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		protected static readonly HygienePropertyDefinition CookieFileProp = new HygienePropertyDefinition("CookieFile", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		protected static readonly HygienePropertyDefinition CookieOffsetProp = new HygienePropertyDefinition("CookieOffset", typeof(long), -1L, ADPropertyDefinitionFlags.PersistDefaultValue);

		protected static readonly HygienePropertyDefinition DBReadProp = new HygienePropertyDefinition("DBRead", typeof(bool), false, ADPropertyDefinitionFlags.PersistDefaultValue);
	}
}
