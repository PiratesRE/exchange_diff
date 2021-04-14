using System;
using Microsoft.Exchange.SharedCache.Client;

namespace Microsoft.Exchange.HttpProxy.Routing.Providers
{
	public interface ISharedCache
	{
		bool TryGet(string key, out byte[] returnBytes, IRoutingDiagnostics diagnostics);

		bool TryGet<T>(string key, out T value, IRoutingDiagnostics diagnostics) where T : ISharedCacheEntry, new();

		string GetSharedCacheKeyFromRoutingKey(IRoutingKey key);
	}
}
