using System;
using Microsoft.Exchange.Collections.TimeoutCache;

namespace Microsoft.Exchange.AirSync
{
	internal sealed class ActiveSyncRequestCache : LazyLookupExactTimeoutCache<Guid, ActiveSyncRequestData>
	{
		public static ActiveSyncRequestCache Instance
		{
			get
			{
				if (ActiveSyncRequestCache.instance == null)
				{
					lock (ActiveSyncRequestCache.lockObject)
					{
						if (ActiveSyncRequestCache.instance == null)
						{
							ActiveSyncRequestCache.instance = new ActiveSyncRequestCache();
						}
					}
				}
				return ActiveSyncRequestCache.instance;
			}
		}

		private ActiveSyncRequestCache() : base(ActiveSyncRequestCache.MaxCacheCount, false, ActiveSyncRequestCache.AbsoluteLiveTime, CacheFullBehavior.ExpireExisting)
		{
		}

		protected override ActiveSyncRequestData CreateOnCacheMiss(Guid key, ref bool shouldAdd)
		{
			shouldAdd = true;
			return new ActiveSyncRequestData(key);
		}

		private static readonly int MaxCacheCount = GlobalSettings.RequestCacheMaxCount;

		private static readonly TimeSpan AbsoluteLiveTime = TimeSpan.FromMinutes((double)GlobalSettings.RequestCacheTimeInterval);

		private static object lockObject = new object();

		private static ActiveSyncRequestCache instance;
	}
}
