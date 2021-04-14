using System;

namespace Microsoft.Exchange.Rpc.NspiServer
{
	internal class SafeNspiCompareDNTsRpcAsyncStateHandle : SafeRpcAsyncStateHandle
	{
		public SafeNspiCompareDNTsRpcAsyncStateHandle(IntPtr handle) : base(handle)
		{
		}

		public SafeNspiCompareDNTsRpcAsyncStateHandle()
		{
		}
	}
}
