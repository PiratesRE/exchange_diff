using System;

namespace Microsoft.Exchange.Rpc.NspiServer
{
	internal class SafeNspiGetPropsRpcAsyncStateHandle : SafeRpcAsyncStateHandle
	{
		public SafeNspiGetPropsRpcAsyncStateHandle(IntPtr handle) : base(handle)
		{
		}

		public SafeNspiGetPropsRpcAsyncStateHandle()
		{
		}
	}
}
