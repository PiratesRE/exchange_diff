using System;
using System.Web;
using System.Web.Caching;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal static class BadExchangePrincipalCache
	{
		public static void Add(string key)
		{
			if (key.Length > 1000)
			{
				return;
			}
			if (!BadExchangePrincipalCache.Contains(key))
			{
				Cache cache = HttpRuntime.Cache;
				cache.Add(BadExchangePrincipalCache.BuildKey(key), key, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(5.0), CacheItemPriority.Low, null);
			}
		}

		public static bool Contains(string key)
		{
			Cache cache = HttpRuntime.Cache;
			return cache.Get(BadExchangePrincipalCache.BuildKey(key)) != null;
		}

		private static string BuildKey(string key)
		{
			return "_BPKP_" + key;
		}

		internal static string BuildKey(string key, string orgPrefix)
		{
			return orgPrefix + key;
		}

		public const int MaxEntryLength = 1000;

		private const string BadPrincipalKeyPrefix = "_BPKP_";

		private const int CacheTimeoutInMinutes = 5;
	}
}
