using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics.Components.Management.ControlPanel;
using Microsoft.Online.BOX.UI.Shell;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public static class NavBarCache
	{
		private static Dictionary<string, NavBarPack> Cache
		{
			get
			{
				if (NavBarCache.navBarCache == null)
				{
					ExTraceGlobals.WebServiceTracer.TraceInformation(0, 0L, "Create NavBarCache.cache.");
					NavBarCache.navBarCache = new Dictionary<string, NavBarPack>(NavBarCache.navBarCacheCapacity);
				}
				return NavBarCache.navBarCache;
			}
		}

		public static NavBarPack Get(string userPuid, string userPrincipalName, string cultureName)
		{
			NavBarPack navBarPack = null;
			Dictionary<string, NavBarPack> cache = NavBarCache.Cache;
			lock (cache)
			{
				cache.TryGetValue(userPuid, out navBarPack);
			}
			if (navBarPack != null && navBarPack.Culture != cultureName)
			{
				ExTraceGlobals.WebServiceTracer.TraceInformation(0, 0L, "NavPack exist but with different culture so can't use. Requested for user: {0} {1}, culure: {2}, cache created: {3} with culture: {4}.", new object[]
				{
					userPuid,
					userPrincipalName,
					cultureName,
					navBarPack.CreateTime,
					navBarPack.Culture
				});
				navBarPack = null;
			}
			return navBarPack;
		}

		public static void Set(string userPuid, string userPrincipalName, NavBarPack navBarPack)
		{
			if (!string.IsNullOrEmpty(userPuid))
			{
				ExTraceGlobals.WebServiceTracer.TraceInformation<string>(0, 0L, "NavBarCache::Set() Add {0} to NavBarCache.", userPuid);
				Dictionary<string, NavBarPack> cache = NavBarCache.Cache;
				lock (cache)
				{
					cache[userPuid] = navBarPack;
				}
			}
			NavBarCache.CleanUpIfNeeded();
		}

		private static void CleanUpIfNeeded()
		{
			if (NavBarCache.Cache.Count > NavBarCache.navBarCacheWarningSize && DateTime.UtcNow - NavBarCache.lastCleanUpTime > NavBarCache.minCleanUpInterval)
			{
				NavBarCache.lastCleanUpTime = DateTime.UtcNow;
				DateTime t = DateTime.UtcNow - NavBarCache.minCacheLife;
				List<string> list = new List<string>();
				Dictionary<string, NavBarPack> cache = NavBarCache.Cache;
				lock (cache)
				{
					foreach (KeyValuePair<string, NavBarPack> keyValuePair in cache)
					{
						if (keyValuePair.Value.CreateTime < t)
						{
							list.Add(keyValuePair.Key);
						}
					}
					foreach (string key in list)
					{
						cache.Remove(key);
					}
				}
				ExTraceGlobals.WebServiceTracer.TraceInformation<DateTime, int, int>(0, 0L, "NavBarCache::CleanUpIfNeeded() Time: {0}, cleaned: {1}, left: {2}.", NavBarCache.lastCleanUpTime, list.Count, NavBarCache.Cache.Count);
			}
		}

		private static int navBarCacheCapacity = 8192;

		private static int navBarCacheWarningSize = (int)((double)NavBarCache.navBarCacheCapacity * 0.7);

		private static TimeSpan minCacheLife = new TimeSpan(4, 0, 0);

		private static TimeSpan minCleanUpInterval = new TimeSpan(1, 0, 0);

		private static DateTime lastCleanUpTime = DateTime.MinValue;

		private static Dictionary<string, NavBarPack> navBarCache;
	}
}
