using System;

namespace Microsoft.Exchange.Rpc.ExchangeServer
{
	internal class SafeEcDoDummyRpcAsyncStateHandle : SafeRpcAsyncStateHandle
	{
		public SafeEcDoDummyRpcAsyncStateHandle(IntPtr handle) : base(handle)
		{
		}

		public SafeEcDoDummyRpcAsyncStateHandle()
		{
		}
	}
}
