using System;

namespace Microsoft.Exchange.Rpc.PoolRpc
{
	internal class SafeEcPoolCloseSessionRpcAsyncStateHandle : SafeRpcAsyncStateHandle
	{
		public SafeEcPoolCloseSessionRpcAsyncStateHandle(IntPtr handle) : base(handle)
		{
		}

		public SafeEcPoolCloseSessionRpcAsyncStateHandle()
		{
		}
	}
}
