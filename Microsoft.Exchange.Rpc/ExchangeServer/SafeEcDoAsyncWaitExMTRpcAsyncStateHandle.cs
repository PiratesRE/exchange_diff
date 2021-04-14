using System;

namespace Microsoft.Exchange.Rpc.ExchangeServer
{
	internal class SafeEcDoAsyncWaitExMTRpcAsyncStateHandle : SafeRpcAsyncStateHandle
	{
		public SafeEcDoAsyncWaitExMTRpcAsyncStateHandle(IntPtr handle) : base(handle)
		{
		}

		public SafeEcDoAsyncWaitExMTRpcAsyncStateHandle()
		{
		}
	}
}
