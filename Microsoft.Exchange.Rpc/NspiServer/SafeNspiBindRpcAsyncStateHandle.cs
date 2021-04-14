using System;

namespace Microsoft.Exchange.Rpc.NspiServer
{
	internal class SafeNspiBindRpcAsyncStateHandle : SafeRpcAsyncStateHandle
	{
		public SafeNspiBindRpcAsyncStateHandle(IntPtr handle) : base(handle)
		{
		}

		public SafeNspiBindRpcAsyncStateHandle()
		{
		}
	}
}
