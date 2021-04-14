using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data.Directory.Sync.TenantRelocationSync
{
	internal class TenantRelocationSyncMappingDictionary
	{
		public TenantRelocationSyncMappingDictionary()
		{
			this.InitializeCache();
		}

		public void Insert(ADObjectId source, ADObjectId target, Guid correlationId)
		{
			DistinguishedNameMapItem value = new DistinguishedNameMapItem(source, target, correlationId);
			if (source != null && !string.IsNullOrEmpty(source.DistinguishedName))
			{
				this.sourceDnDictionary[source.DistinguishedName] = value;
			}
			if (target != null && !string.IsNullOrEmpty(target.DistinguishedName))
			{
				this.targetDnDictionary[target.DistinguishedName] = value;
			}
			if (!correlationId.Equals(Guid.Empty))
			{
				this.correlationGuidDictionary[correlationId] = value;
			}
		}

		public void Remove(DistinguishedNameMapItem item)
		{
			if (item.SourceDN != null && !string.IsNullOrEmpty(item.SourceDN.DistinguishedName))
			{
				this.sourceDnDictionary[item.SourceDN.DistinguishedName] = null;
			}
			if (item.TargetDN != null && !string.IsNullOrEmpty(item.TargetDN.DistinguishedName))
			{
				this.targetDnDictionary[item.TargetDN.DistinguishedName] = null;
			}
			if (!item.CorrelationId.Equals(Guid.Empty))
			{
				this.correlationGuidDictionary[item.CorrelationId] = null;
			}
		}

		public DistinguishedNameMapItem LookupBySourceDn(string source)
		{
			DistinguishedNameMapItem result;
			if (this.sourceDnDictionary.TryGetValue(source, out result))
			{
				return result;
			}
			return null;
		}

		public DistinguishedNameMapItem LookupBySourceADObjectId(ADObjectId id)
		{
			if (!string.IsNullOrEmpty(id.DistinguishedName))
			{
				return this.LookupBySourceDn(id.DistinguishedName);
			}
			if (!Guid.Empty.Equals(id.ObjectGuid))
			{
				return this.LookupByCorrelationGuid(id.ObjectGuid);
			}
			throw new ArgumentException("invalid paramter id");
		}

		public DistinguishedNameMapItem LookupByTargetDn(string target)
		{
			DistinguishedNameMapItem result;
			if (this.targetDnDictionary.TryGetValue(target, out result))
			{
				return result;
			}
			return null;
		}

		public DistinguishedNameMapItem LookupByCorrelationGuid(Guid cId)
		{
			DistinguishedNameMapItem result;
			if (this.correlationGuidDictionary.TryGetValue(cId, out result))
			{
				return result;
			}
			return null;
		}

		public void ClearCacheIfNecessary(bool fForce)
		{
			if (fForce || this.correlationGuidDictionary.Count > TenantRelocationSyncMappingDictionary.MaxCacheSize)
			{
				this.InitializeCache();
			}
		}

		private void InitializeCache()
		{
			lock (this)
			{
				this.sourceDnDictionary = new Dictionary<string, DistinguishedNameMapItem>();
				this.targetDnDictionary = new Dictionary<string, DistinguishedNameMapItem>();
				this.correlationGuidDictionary = new Dictionary<Guid, DistinguishedNameMapItem>();
			}
		}

		private static readonly int MaxCacheSize = 100000;

		private Dictionary<string, DistinguishedNameMapItem> sourceDnDictionary;

		private Dictionary<string, DistinguishedNameMapItem> targetDnDictionary;

		private Dictionary<Guid, DistinguishedNameMapItem> correlationGuidDictionary;
	}
}
