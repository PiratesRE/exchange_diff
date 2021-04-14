using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Common.Cache
{
	internal abstract class DefaultCachePerformanceCounters : ICachePerformanceCounters
	{
		public DefaultCachePerformanceCounters()
		{
		}

		public virtual void Accessed(AccessStatus accessStatus)
		{
			if (this.requestsCounter != null)
			{
				this.requestsCounter.Increment();
				if (accessStatus.Equals(AccessStatus.Hit) && this.hitRatioCounter != null)
				{
					this.hitRatioCounter.Increment();
				}
				if (this.hitRatioBaseCounter != null)
				{
					this.hitRatioBaseCounter.Increment();
				}
			}
		}

		public virtual void SizeUpdated(long cacheSize)
		{
			if (this.cacheSizeCounter != null)
			{
				this.cacheSizeCounter.RawValue = cacheSize;
			}
		}

		protected void InitializeCounters(ExPerformanceCounter requestsCounter, ExPerformanceCounter hitRatioCounter, ExPerformanceCounter hitRatioBaseCounter, ExPerformanceCounter cacheSizeCounter)
		{
			this.requestsCounter = requestsCounter;
			this.hitRatioCounter = hitRatioCounter;
			this.hitRatioBaseCounter = hitRatioBaseCounter;
			this.cacheSizeCounter = cacheSizeCounter;
		}

		private ExPerformanceCounter requestsCounter;

		private ExPerformanceCounter hitRatioCounter;

		private ExPerformanceCounter hitRatioBaseCounter;

		private ExPerformanceCounter cacheSizeCounter;
	}
}
