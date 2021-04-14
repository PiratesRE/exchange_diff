using System;

namespace Microsoft.Exchange.Rpc.PoolRpc
{
	internal abstract class PoolRpcServerBase : PoolRpcServerCommonBase
	{
		public PoolRpcServerBase()
		{
		}

		public static IntPtr RpcIntfHandle = (IntPtr)<Module>.emsmdbpool_v0_1_s_ifspec;
	}
}
