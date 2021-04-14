using System;
using System.Runtime.Caching;

namespace Microsoft.Exchange.Data.Directory.Cache
{
	internal interface ICacheConfiguration
	{
		bool IsCacheEnabled(string processNameOrProcessAppName);

		CacheMode GetCacheMode(string processNameOrProcessAppName);

		CacheMode GetCacheModeForCurrentProcess();

		bool IsCacheEnableForCurrentProcess();

		bool IsCacheEnabled(Type type);

		bool IsCacheEnabledForInsertOnSave(ADRawEntry rawEntry);

		int GetCacheExpirationForObject(ADRawEntry rawEntry);

		CacheItemPriority GetCachePriorityForObject(ADRawEntry rawEntry);
	}
}
