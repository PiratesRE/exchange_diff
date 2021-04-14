using System;

namespace Microsoft.Exchange.Rpc.SubscriptionCache
{
	internal abstract class SubscriptionCacheRpcServer : RpcServerBase
	{
		public abstract byte[] TestUserCache(int version, byte[] pInBytes);

		public SubscriptionCacheRpcServer()
		{
		}

		public static IntPtr RpcIntfHandle = (IntPtr)<Module>.ISubscriptionCacheServer_v1_0_s_ifspec;
	}
}
