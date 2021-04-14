using System;

namespace Microsoft.Exchange.Rpc.NspiServer
{
	internal class SafeNspiDNToEphRpcAsyncStateHandle : SafeRpcAsyncStateHandle
	{
		public SafeNspiDNToEphRpcAsyncStateHandle(IntPtr handle) : base(handle)
		{
		}

		public SafeNspiDNToEphRpcAsyncStateHandle()
		{
		}
	}
}
