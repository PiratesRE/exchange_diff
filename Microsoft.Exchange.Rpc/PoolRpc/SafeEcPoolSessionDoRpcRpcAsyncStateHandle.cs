using System;

namespace Microsoft.Exchange.Rpc.PoolRpc
{
	internal class SafeEcPoolSessionDoRpcRpcAsyncStateHandle : SafeRpcAsyncStateHandle
	{
		public SafeEcPoolSessionDoRpcRpcAsyncStateHandle(IntPtr handle) : base(handle)
		{
		}

		public SafeEcPoolSessionDoRpcRpcAsyncStateHandle()
		{
		}
	}
}
