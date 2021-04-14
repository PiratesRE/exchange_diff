using System;

namespace Microsoft.Exchange.Rpc.NspiServer
{
	internal class SafeNspiDeleteEntriesRpcAsyncStateHandle : SafeRpcAsyncStateHandle
	{
		public SafeNspiDeleteEntriesRpcAsyncStateHandle(IntPtr handle) : base(handle)
		{
		}

		public SafeNspiDeleteEntriesRpcAsyncStateHandle()
		{
		}
	}
}
