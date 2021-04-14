using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Hygiene.Data;

namespace Microsoft.Exchange.Hygiene.Cache.Data
{
	internal class CachePrimingInfo : ConfigurablePropertyBag
	{
		public CachePrimingInfo()
		{
		}

		internal CachePrimingInfo(string cacheName, CachePrimingState primingState, CacheEntityState entityState = null)
		{
			this.CacheName = cacheName;
			this.PrimingState = primingState;
			if (entityState != null)
			{
				this.LastSyncTime = entityState.LastSyncTime;
				this.SyncWatermark = entityState.LastFullSyncTime;
				this.LastTracerSyncTime = entityState.LastTracerSyncTime;
			}
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
				return this.CacheName;
			}
		}

		public string CacheName
		{
			get
			{
				return this[CachePrimingInfo.CacheNameProp] as string;
			}
			set
			{
				this[CachePrimingInfo.CacheNameProp] = value;
			}
		}

		public CachePrimingState PrimingState
		{
			get
			{
				return (CachePrimingState)this[CachePrimingInfo.PrimingStateProp];
			}
			set
			{
				this[CachePrimingInfo.PrimingStateProp] = value;
			}
		}

		public DateTime LastSyncTime
		{
			get
			{
				return (DateTime)this[CachePrimingInfo.LastSyncTimeProp];
			}
			set
			{
				this[CachePrimingInfo.LastSyncTimeProp] = value;
			}
		}

		public DateTime SyncWatermark
		{
			get
			{
				return (DateTime)this[CachePrimingInfo.SyncWatermarkProp];
			}
			set
			{
				this[CachePrimingInfo.SyncWatermarkProp] = value;
			}
		}

		public DateTime LastTracerSyncTime
		{
			get
			{
				return (DateTime)this[CachePrimingInfo.LastTracerSyncTimeProp];
			}
			set
			{
				this[CachePrimingInfo.LastTracerSyncTimeProp] = value;
			}
		}

		private static readonly HygienePropertyDefinition CacheNameProp = CacheObjectSchema.EntityNameProp;

		private static readonly HygienePropertyDefinition LastSyncTimeProp = CacheObjectSchema.LastSyncTimeProp;

		private static readonly HygienePropertyDefinition SyncWatermarkProp = CacheObjectSchema.LastFullSyncTimeProp;

		private static readonly HygienePropertyDefinition LastTracerSyncTimeProp = CacheObjectSchema.LastTracerSyncTimeProp;

		private static readonly HygienePropertyDefinition PrimingStateProp = new HygienePropertyDefinition("PrimingState", typeof(CachePrimingState), CachePrimingState.Unknown, ADPropertyDefinitionFlags.PersistDefaultValue);
	}
}
