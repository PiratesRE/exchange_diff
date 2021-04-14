using System;

namespace Microsoft.Exchange.Rpc.NspiServer
{
	internal class SafeNspiQueryRowsRpcAsyncStateHandle : SafeRpcAsyncStateHandle
	{
		public SafeNspiQueryRowsRpcAsyncStateHandle(IntPtr handle) : base(handle)
		{
		}

		public SafeNspiQueryRowsRpcAsyncStateHandle()
		{
		}
	}
}
