using System;

namespace Microsoft.Exchange.Rpc.ExchangeServer
{
	internal class SafeEcDoConnectExRpcAsyncStateHandle : SafeRpcAsyncStateHandle
	{
		public SafeEcDoConnectExRpcAsyncStateHandle(IntPtr handle) : base(handle)
		{
		}

		public SafeEcDoConnectExRpcAsyncStateHandle()
		{
		}
	}
}
