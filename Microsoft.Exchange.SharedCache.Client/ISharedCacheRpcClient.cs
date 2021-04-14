using System;
using Microsoft.Exchange.Rpc.SharedCache;

namespace Microsoft.Exchange.SharedCache.Client
{
	internal interface ISharedCacheRpcClient
	{
		CacheResponse Get(Guid guid, string key);

		CacheResponse Insert(Guid guid, string key, byte[] inBlob, long entryValidTime);

		CacheResponse Delete(Guid guid, string key);
	}
}
