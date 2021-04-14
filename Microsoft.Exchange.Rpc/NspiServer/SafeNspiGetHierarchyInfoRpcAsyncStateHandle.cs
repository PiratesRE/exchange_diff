using System;

namespace Microsoft.Exchange.Rpc.NspiServer
{
	internal class SafeNspiGetHierarchyInfoRpcAsyncStateHandle : SafeRpcAsyncStateHandle
	{
		public SafeNspiGetHierarchyInfoRpcAsyncStateHandle(IntPtr handle) : base(handle)
		{
		}

		public SafeNspiGetHierarchyInfoRpcAsyncStateHandle()
		{
		}
	}
}
