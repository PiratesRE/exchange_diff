using System;

namespace Microsoft.Exchange.Rpc.NspiServer
{
	internal class SafeNspiGetIDsFromNamesRpcAsyncStateHandle : SafeRpcAsyncStateHandle
	{
		public SafeNspiGetIDsFromNamesRpcAsyncStateHandle(IntPtr handle) : base(handle)
		{
		}

		public SafeNspiGetIDsFromNamesRpcAsyncStateHandle()
		{
		}
	}
}
