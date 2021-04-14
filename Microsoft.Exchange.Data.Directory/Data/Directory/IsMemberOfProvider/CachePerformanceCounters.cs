using System;
using Microsoft.Exchange.Common.Cache;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory.IsMemberOfProvider
{
	internal class CachePerformanceCounters : ICachePerformanceCounters
	{
		public CachePerformanceCounters(ExPerformanceCounter hitCount, ExPerformanceCounter missCount, ExPerformanceCounter cacheSize, ExPerformanceCounter cacheSizePercentage, long maxCacheSizeInBytes)
		{
			this.hitCount = hitCount;
			this.missCount = missCount;
			this.cacheSize = cacheSize;
			this.cacheSizePercentage = cacheSizePercentage;
			this.maxCacheSize = maxCacheSizeInBytes;
		}

		public void Accessed(AccessStatus accessStatus)
		{
			switch (accessStatus)
			{
			case AccessStatus.Hit:
				this.hitCount.Increment();
				return;
			case AccessStatus.Miss:
				this.missCount.Increment();
				return;
			default:
				return;
			}
		}

		public void SizeUpdated(long cacheSize)
		{
			this.cacheSize.RawValue = cacheSize;
			if (this.maxCacheSize != 0L)
			{
				double num = (double)cacheSize / (double)this.maxCacheSize;
				this.cacheSizePercentage.RawValue = (long)(num * 100.0);
			}
		}

		private readonly long maxCacheSize;

		private ExPerformanceCounter hitCount;

		private ExPerformanceCounter missCount;

		private ExPerformanceCounter cacheSize;

		private ExPerformanceCounter cacheSizePercentage;
	}
}
