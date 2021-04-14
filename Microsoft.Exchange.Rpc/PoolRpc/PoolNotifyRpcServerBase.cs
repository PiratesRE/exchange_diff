using System;

namespace Microsoft.Exchange.Rpc.PoolRpc
{
	internal abstract class PoolNotifyRpcServerBase : PoolRpcServerCommonBase
	{
		public PoolNotifyRpcServerBase()
		{
		}

		public static IntPtr RpcIntfHandle = (IntPtr)<Module>.emsmdbpoolNotify_v0_1_s_ifspec;
	}
}
