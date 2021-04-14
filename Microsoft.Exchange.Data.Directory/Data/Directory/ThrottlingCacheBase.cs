using System;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;

namespace Microsoft.Exchange.Data.Directory
{
	internal abstract class ThrottlingCacheBase<K, T> : LazyLookupExactTimeoutCache<K, T>
	{
		protected ThrottlingCacheBase(int maxCount, bool shouldCallbackOnDispose, TimeSpan absoluteLiveTime, CacheFullBehavior cacheFullBehavior) : base(maxCount, shouldCallbackOnDispose, absoluteLiveTime, cacheFullBehavior)
		{
		}

		protected ThrottlingCacheBase(int maxCount, bool shouldCallbackOnDispose, TimeSpan slidingLiveTime, TimeSpan absoluteLiveTime, CacheFullBehavior cacheFullBehavior) : base(maxCount, shouldCallbackOnDispose, slidingLiveTime, absoluteLiveTime, cacheFullBehavior)
		{
		}

		protected override void BeforeGet(K key)
		{
			int num = 0;
			ExTraceGlobals.FaultInjectionTracer.TraceTest<int>(2663787837U, ref num);
			if (num != 0 && num != this.lastClearCacheStamp)
			{
				this.lastClearCacheStamp = num;
				this.Clear();
			}
		}

		internal override void Clear()
		{
			base.Clear();
			ThrottlingPerfCounterWrapper.ClearCaches();
		}

		private const uint LidClearThrottlingCaches = 2663787837U;

		private int lastClearCacheStamp;
	}
}
