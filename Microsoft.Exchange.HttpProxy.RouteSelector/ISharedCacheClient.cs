using System;
using Microsoft.Exchange.HttpProxy.Routing.Providers;
using Microsoft.Exchange.SharedCache.Client;

namespace Microsoft.Exchange.HttpProxy.RouteSelector
{
	public interface ISharedCacheClient : ISharedCache
	{
		bool TryInsert(string key, byte[] dataBytes, DateTime cacheExpiry, out string diagInfo);

		bool TryInsert(string key, ISharedCacheEntry value, DateTime valueTimeStamp, out string diagInfo);
	}
}
