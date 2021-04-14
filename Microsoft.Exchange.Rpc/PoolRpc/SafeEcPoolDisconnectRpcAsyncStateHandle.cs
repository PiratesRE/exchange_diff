using System;

namespace Microsoft.Exchange.Rpc.PoolRpc
{
	internal class SafeEcPoolDisconnectRpcAsyncStateHandle : SafeRpcAsyncStateHandle
	{
		public SafeEcPoolDisconnectRpcAsyncStateHandle(IntPtr handle) : base(handle)
		{
		}

		public SafeEcPoolDisconnectRpcAsyncStateHandle()
		{
		}
	}
}
