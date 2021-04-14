using System;

namespace Microsoft.Exchange.Rpc.NspiServer
{
	internal class SafeNspiModLinkAttRpcAsyncStateHandle : SafeRpcAsyncStateHandle
	{
		public SafeNspiModLinkAttRpcAsyncStateHandle(IntPtr handle) : base(handle)
		{
		}

		public SafeNspiModLinkAttRpcAsyncStateHandle()
		{
		}
	}
}
