using System;

namespace Microsoft.Exchange.Rpc.NspiServer
{
	internal class SafeNspiResolveNamesRpcAsyncStateHandle : SafeRpcAsyncStateHandle
	{
		public SafeNspiResolveNamesRpcAsyncStateHandle(IntPtr handle) : base(handle)
		{
		}

		public SafeNspiResolveNamesRpcAsyncStateHandle()
		{
		}
	}
}
