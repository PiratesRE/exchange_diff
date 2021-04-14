using System;
using Microsoft.Exchange.Rpc.SharedCache;

namespace Microsoft.Exchange.SharedCache.Client
{
	internal class SharedCacheRpcClientImpl : SharedCacheRpcClient, ISharedCacheRpcClient
	{
		public SharedCacheRpcClientImpl(string machineName, int timeoutMilliseconds) : base(machineName, timeoutMilliseconds)
		{
		}

		CacheResponse ISharedCacheRpcClient.Get(Guid A_1, string A_2)
		{
			return base.Get(A_1, A_2);
		}

		CacheResponse ISharedCacheRpcClient.Insert(Guid A_1, string A_2, byte[] A_3, long A_4)
		{
			return base.Insert(A_1, A_2, A_3, A_4);
		}

		CacheResponse ISharedCacheRpcClient.Delete(Guid A_1, string A_2)
		{
			return base.Delete(A_1, A_2);
		}
	}
}
