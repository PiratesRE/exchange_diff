using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Hygiene.Data;

namespace Microsoft.Exchange.Hygiene.Cache.Data
{
	[Serializable]
	internal class CacheKeyLookup : ConfigurablePropertyBag
	{
		public CacheKeyLookup()
		{
			this.ReferenceKeys = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
		}

		public CacheKeyLookup(string entityName, string primaryKey)
		{
			this.EntityName = entityName;
			this.PrimaryKey = primaryKey;
			this.ReferenceKeys = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
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
				return CacheKeyLookup.ConstructCacheIdentity(this.EntityName, this.PrimaryKey);
			}
		}

		public string EntityName
		{
			get
			{
				return this[CacheKeyLookup.EntityNameProp] as string;
			}
			set
			{
				this[CacheKeyLookup.EntityNameProp] = value;
			}
		}

		public string PrimaryKey
		{
			get
			{
				return this[CacheKeyLookup.PrimaryKeyProp] as string;
			}
			set
			{
				this[CacheKeyLookup.PrimaryKeyProp] = value;
			}
		}

		public HashSet<string> ReferenceKeys
		{
			get
			{
				return this[CacheKeyLookup.ReferenceKeysProp] as HashSet<string>;
			}
			set
			{
				this[CacheKeyLookup.ReferenceKeysProp] = value;
			}
		}

		public static string ConstructCacheIdentity(string entityName, string primaryKey)
		{
			return string.Format("{0}:{1}", entityName, primaryKey);
		}

		internal bool IsReferenceKeyInLookup(string referenceKey)
		{
			return this.ReferenceKeys.Contains(referenceKey);
		}

		internal void AddReferenceKeysToLookup(IEnumerable<string> referenceKeys)
		{
			this.ReferenceKeys.UnionWith(referenceKeys);
		}

		internal void RemoveReferenceKeysFromLookup(IEnumerable<string> referenceKeys)
		{
			this.ReferenceKeys.ExceptWith(referenceKeys);
		}

		private static readonly HygienePropertyDefinition EntityNameProp = CacheObjectSchema.EntityNameProp;

		private static readonly HygienePropertyDefinition PrimaryKeyProp = new HygienePropertyDefinition("PrimaryKey", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		private static readonly HygienePropertyDefinition ReferenceKeysProp = new HygienePropertyDefinition("ReferenceKeys", typeof(ISet<string>));
	}
}
