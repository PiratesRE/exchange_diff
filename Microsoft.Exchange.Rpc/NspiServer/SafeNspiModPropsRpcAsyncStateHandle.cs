using System;

namespace Microsoft.Exchange.Rpc.NspiServer
{
	internal class SafeNspiModPropsRpcAsyncStateHandle : SafeRpcAsyncStateHandle
	{
		public SafeNspiModPropsRpcAsyncStateHandle(IntPtr handle) : base(handle)
		{
		}

		public SafeNspiModPropsRpcAsyncStateHandle()
		{
		}
	}
}
