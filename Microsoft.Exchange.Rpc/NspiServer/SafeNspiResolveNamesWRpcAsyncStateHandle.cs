using System;

namespace Microsoft.Exchange.Rpc.NspiServer
{
	internal class SafeNspiResolveNamesWRpcAsyncStateHandle : SafeRpcAsyncStateHandle
	{
		public SafeNspiResolveNamesWRpcAsyncStateHandle(IntPtr handle) : base(handle)
		{
		}

		public SafeNspiResolveNamesWRpcAsyncStateHandle()
		{
		}
	}
}
