using System;

namespace Microsoft.Exchange.Rpc.ExchangeServer
{
	internal class SafeEcDoRpcExt2RpcAsyncStateHandle : SafeRpcAsyncStateHandle
	{
		public SafeEcDoRpcExt2RpcAsyncStateHandle(IntPtr handle) : base(handle)
		{
		}

		public SafeEcDoRpcExt2RpcAsyncStateHandle()
		{
		}
	}
}
