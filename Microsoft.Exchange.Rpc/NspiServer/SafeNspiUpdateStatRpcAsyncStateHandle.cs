using System;

namespace Microsoft.Exchange.Rpc.NspiServer
{
	internal class SafeNspiUpdateStatRpcAsyncStateHandle : SafeRpcAsyncStateHandle
	{
		public SafeNspiUpdateStatRpcAsyncStateHandle(IntPtr handle) : base(handle)
		{
		}

		public SafeNspiUpdateStatRpcAsyncStateHandle()
		{
		}
	}
}
