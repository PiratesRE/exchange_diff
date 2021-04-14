using System;

namespace Microsoft.Exchange.Rpc.NspiServer
{
	internal class SafeNspiGetTemplateInfoRpcAsyncStateHandle : SafeRpcAsyncStateHandle
	{
		public SafeNspiGetTemplateInfoRpcAsyncStateHandle(IntPtr handle) : base(handle)
		{
		}

		public SafeNspiGetTemplateInfoRpcAsyncStateHandle()
		{
		}
	}
}
