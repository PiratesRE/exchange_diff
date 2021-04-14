using System;

namespace Microsoft.Exchange.Rpc.RfriServer
{
	internal class SafeRfrGetFQDNFromLegacyDNRpcAsyncStateHandle : SafeRpcAsyncStateHandle
	{
		public SafeRfrGetFQDNFromLegacyDNRpcAsyncStateHandle(IntPtr handle) : base(handle)
		{
		}

		public SafeRfrGetFQDNFromLegacyDNRpcAsyncStateHandle()
		{
		}
	}
}
