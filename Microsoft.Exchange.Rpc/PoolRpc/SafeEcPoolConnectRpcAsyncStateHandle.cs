using System;

namespace Microsoft.Exchange.Rpc.PoolRpc
{
	internal class SafeEcPoolConnectRpcAsyncStateHandle : SafeRpcAsyncStateHandle
	{
		public SafeEcPoolConnectRpcAsyncStateHandle(IntPtr handle) : base(handle)
		{
		}

		public SafeEcPoolConnectRpcAsyncStateHandle()
		{
		}
	}
}
