using System;

namespace Microsoft.Exchange.Rpc.NspiServer
{
	internal class SafeNspiGetPropListRpcAsyncStateHandle : SafeRpcAsyncStateHandle
	{
		public SafeNspiGetPropListRpcAsyncStateHandle(IntPtr handle) : base(handle)
		{
		}

		public SafeNspiGetPropListRpcAsyncStateHandle()
		{
		}
	}
}
