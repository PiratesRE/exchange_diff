using System;

namespace Microsoft.Exchange.Rpc.RfriServer
{
	internal class SafeRfrGetNewDSARpcAsyncStateHandle : SafeRpcAsyncStateHandle
	{
		public SafeRfrGetNewDSARpcAsyncStateHandle(IntPtr handle) : base(handle)
		{
		}

		public SafeRfrGetNewDSARpcAsyncStateHandle()
		{
		}
	}
}
