using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Hygiene.Data;

namespace Microsoft.Exchange.Hygiene.Cache.Data
{
	[Serializable]
	internal class CacheEntityState : ConfigurablePropertyBag
	{
		public CacheEntityState()
		{
			this.LastSyncTime = DateTime.MinValue;
			this.LastFullSyncTime = DateTime.MinValue;
			this.LastTracerSyncTime = DateTime.MinValue;
		}

		public CacheEntityState(string entityName) : this()
		{
			this.EntityName = entityName;
		}

		public CacheEntityState(string entityName, CachePrimingAction action) : this(entityName)
		{
			this.PrimingAction = action;
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
				return CacheEntityState.ConstructCacheIdentity(this.EntityName);
			}
		}

		public string EntityName
		{
			get
			{
				return this[CacheEntityState.EntityNameProp] as string;
			}
			set
			{
				this[CacheEntityState.EntityNameProp] = value;
			}
		}

		public DateTime LastSyncTime
		{
			get
			{
				return (DateTime)this[CacheEntityState.LastSyncTimeProp];
			}
			set
			{
				this[CacheEntityState.LastSyncTimeProp] = value;
			}
		}

		public DateTime LastFullSyncTime
		{
			get
			{
				return (DateTime)this[CacheEntityState.LastFullSyncTimeProp];
			}
			set
			{
				this[CacheEntityState.LastFullSyncTimeProp] = value;
			}
		}

		public DateTime LastTracerSyncTime
		{
			get
			{
				return (DateTime)this[CacheEntityState.LastTracerSyncTimeProp];
			}
			set
			{
				this[CacheEntityState.LastTracerSyncTimeProp] = value;
			}
		}

		public CachePrimingAction PrimingAction
		{
			get
			{
				return (CachePrimingAction)this[CacheEntityState.PrimingActionProp];
			}
			set
			{
				this[CacheEntityState.PrimingActionProp] = value;
			}
		}

		public static string ConstructCacheIdentity(string entityName)
		{
			return string.Format("{0}", entityName);
		}

		private static readonly HygienePropertyDefinition EntityNameProp = CacheObjectSchema.EntityNameProp;

		private static readonly HygienePropertyDefinition LastSyncTimeProp = CacheObjectSchema.LastSyncTimeProp;

		private static readonly HygienePropertyDefinition LastFullSyncTimeProp = CacheObjectSchema.LastFullSyncTimeProp;

		private static readonly HygienePropertyDefinition LastTracerSyncTimeProp = CacheObjectSchema.LastTracerSyncTimeProp;

		private static readonly HygienePropertyDefinition PrimingActionProp = new HygienePropertyDefinition("PrimingAction", typeof(CachePrimingAction), CachePrimingAction.Priming, ADPropertyDefinitionFlags.PersistDefaultValue);
	}
}
