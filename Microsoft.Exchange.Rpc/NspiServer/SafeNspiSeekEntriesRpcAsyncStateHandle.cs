using System;

namespace Microsoft.Exchange.Rpc.NspiServer
{
	internal class SafeNspiSeekEntriesRpcAsyncStateHandle : SafeRpcAsyncStateHandle
	{
		public SafeNspiSeekEntriesRpcAsyncStateHandle(IntPtr handle) : base(handle)
		{
		}

		public SafeNspiSeekEntriesRpcAsyncStateHandle()
		{
		}
	}
}
