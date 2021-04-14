using System;

namespace Microsoft.Exchange.Rpc.NspiServer
{
	internal class SafeNspiGetMatchesRpcAsyncStateHandle : SafeRpcAsyncStateHandle
	{
		public SafeNspiGetMatchesRpcAsyncStateHandle(IntPtr handle) : base(handle)
		{
		}

		public SafeNspiGetMatchesRpcAsyncStateHandle()
		{
		}
	}
}
