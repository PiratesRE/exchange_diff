using System;

namespace Microsoft.Exchange.Rpc.SharedCache
{
	internal abstract class SharedCacheRpcServer : RpcServerBase
	{
		public abstract void Get(Guid guid, string key, ref CacheResponse cacheResponse);

		public abstract void Insert(Guid guid, string key, byte[] pInBytes, long entryValidTime, ref CacheResponse cacheResponse);

		public abstract void Delete(Guid guid, string key, ref CacheResponse cacheResponse);

		public SharedCacheRpcServer()
		{
		}

		public static IntPtr RpcIntfHandle = (IntPtr)<Module>.ISharedCache_v1_0_s_ifspec;
	}
}
