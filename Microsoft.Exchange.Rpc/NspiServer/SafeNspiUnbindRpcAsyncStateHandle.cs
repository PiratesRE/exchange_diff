using System;

namespace Microsoft.Exchange.Rpc.NspiServer
{
	internal class SafeNspiUnbindRpcAsyncStateHandle : SafeRpcAsyncStateHandle
	{
		public SafeNspiUnbindRpcAsyncStateHandle(IntPtr handle) : base(handle)
		{
		}

		public SafeNspiUnbindRpcAsyncStateHandle()
		{
		}
	}
}
