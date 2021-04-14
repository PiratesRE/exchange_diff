using System;

namespace Microsoft.Exchange.Rpc.PoolRpc
{
	internal class SafeEcPoolCreateSessionRpcAsyncStateHandle : SafeRpcAsyncStateHandle
	{
		public SafeEcPoolCreateSessionRpcAsyncStateHandle(IntPtr handle) : base(handle)
		{
		}

		public SafeEcPoolCreateSessionRpcAsyncStateHandle()
		{
		}
	}
}
