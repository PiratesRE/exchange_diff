using System;
using Microsoft.Exchange.Common.Cache;

namespace Microsoft.Exchange.Data.Directory.IsMemberOfProvider
{
	internal class IsMemberOfResolverPerformanceCounters
	{
		public IsMemberOfResolverPerformanceCounters(string componentName)
		{
			this.perfCountersInstance = IsMemberOfResolverPerfCounters.GetInstance(componentName);
			this.Reset();
		}

		protected IsMemberOfResolverPerformanceCounters()
		{
		}

		public virtual ICachePerformanceCounters GetResolvedGroupsCacheCounters(long maxCacheSizeInBytes)
		{
			return new CachePerformanceCounters(this.perfCountersInstance.ResolvedGroupsHitCount, this.perfCountersInstance.ResolvedGroupsMissCount, this.perfCountersInstance.ResolvedGroupsCacheSize, this.perfCountersInstance.ResolvedGroupsCacheSizePercentage, maxCacheSizeInBytes);
		}

		public virtual ICachePerformanceCounters GetExpandedGroupsCacheCounters(long maxCacheSizeInBytes)
		{
			return new CachePerformanceCounters(this.perfCountersInstance.ExpandedGroupsHitCount, this.perfCountersInstance.ExpandedGroupsMissCount, this.perfCountersInstance.ExpandedGroupsCacheSize, this.perfCountersInstance.ExpandedGroupsCacheSizePercentage, maxCacheSizeInBytes);
		}

		public virtual void Reset()
		{
			this.perfCountersInstance.ResolvedGroupsHitCount.RawValue = 0L;
			this.perfCountersInstance.ResolvedGroupsMissCount.RawValue = 0L;
			this.perfCountersInstance.ResolvedGroupsCacheSize.RawValue = 0L;
			this.perfCountersInstance.ResolvedGroupsCacheSizePercentage.RawValue = 0L;
			this.perfCountersInstance.ExpandedGroupsHitCount.RawValue = 0L;
			this.perfCountersInstance.ExpandedGroupsMissCount.RawValue = 0L;
			this.perfCountersInstance.ExpandedGroupsCacheSize.RawValue = 0L;
			this.perfCountersInstance.ExpandedGroupsCacheSizePercentage.RawValue = 0L;
			this.perfCountersInstance.LdapQueries.RawValue = 0L;
		}

		public virtual void IncrementLDAPQueryCount(int count)
		{
			this.perfCountersInstance.LdapQueries.IncrementBy((long)count);
		}

		private IsMemberOfResolverPerfCountersInstance perfCountersInstance;
	}
}
