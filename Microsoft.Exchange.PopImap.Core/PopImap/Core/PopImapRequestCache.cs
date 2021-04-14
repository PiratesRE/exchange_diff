using System;
using Microsoft.Exchange.Collections.TimeoutCache;

namespace Microsoft.Exchange.PopImap.Core
{
	internal sealed class PopImapRequestCache : LazyLookupExactTimeoutCache<Guid, PopImapRequestData>
	{
		public static PopImapRequestCache Instance
		{
			get
			{
				if (PopImapRequestCache.instance == null)
				{
					lock (PopImapRequestCache.lockObject)
					{
						if (PopImapRequestCache.instance == null)
						{
							PopImapRequestCache.instance = new PopImapRequestCache();
						}
					}
				}
				return PopImapRequestCache.instance;
			}
		}

		private PopImapRequestCache() : base(PopImapRequestCache.MaxCacheCount, false, PopImapRequestCache.AbsoluteLiveTime, CacheFullBehavior.ExpireExisting)
		{
		}

		protected override PopImapRequestData CreateOnCacheMiss(Guid key, ref bool shouldAdd)
		{
			shouldAdd = true;
			return new PopImapRequestData(key);
		}

		private static readonly int MaxCacheCount = 5000;

		private static readonly TimeSpan AbsoluteLiveTime = TimeSpan.FromMinutes(10.0);

		private static object lockObject = new object();

		private static PopImapRequestCache instance;
	}
}
