using System;

namespace Microsoft.Exchange.Rpc.ExchangeServer
{
	internal class SafeEcDoDisconnectRpcAsyncStateHandle : SafeRpcAsyncStateHandle
	{
		public SafeEcDoDisconnectRpcAsyncStateHandle(IntPtr handle) : base(handle)
		{
		}

		public SafeEcDoDisconnectRpcAsyncStateHandle()
		{
		}
	}
}
