using System;

namespace Microsoft.Exchange.Rpc.NspiServer
{
	internal class SafeNspiGetNamesFromIDsRpcAsyncStateHandle : SafeRpcAsyncStateHandle
	{
		public SafeNspiGetNamesFromIDsRpcAsyncStateHandle(IntPtr handle) : base(handle)
		{
		}

		public SafeNspiGetNamesFromIDsRpcAsyncStateHandle()
		{
		}
	}
}
