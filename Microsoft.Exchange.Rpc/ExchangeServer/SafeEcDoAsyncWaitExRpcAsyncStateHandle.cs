using System;

namespace Microsoft.Exchange.Rpc.ExchangeServer
{
	internal class SafeEcDoAsyncWaitExRpcAsyncStateHandle : SafeRpcAsyncStateHandle
	{
		public SafeEcDoAsyncWaitExRpcAsyncStateHandle(IntPtr handle) : base(handle)
		{
		}

		public SafeEcDoAsyncWaitExRpcAsyncStateHandle()
		{
		}
	}
}
