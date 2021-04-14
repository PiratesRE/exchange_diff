using System;

namespace Microsoft.Exchange.Rpc.ExchangeServer
{
	internal class SafeEcDoAsyncConnectExRpcAsyncStateHandle : SafeRpcAsyncStateHandle
	{
		public SafeEcDoAsyncConnectExRpcAsyncStateHandle(IntPtr handle) : base(handle)
		{
		}

		public SafeEcDoAsyncConnectExRpcAsyncStateHandle()
		{
		}
	}
}
