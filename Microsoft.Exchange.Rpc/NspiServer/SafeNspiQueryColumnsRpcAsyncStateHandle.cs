using System;

namespace Microsoft.Exchange.Rpc.NspiServer
{
	internal class SafeNspiQueryColumnsRpcAsyncStateHandle : SafeRpcAsyncStateHandle
	{
		public SafeNspiQueryColumnsRpcAsyncStateHandle(IntPtr handle) : base(handle)
		{
		}

		public SafeNspiQueryColumnsRpcAsyncStateHandle()
		{
		}
	}
}
