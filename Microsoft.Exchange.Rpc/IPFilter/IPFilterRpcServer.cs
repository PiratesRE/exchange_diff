using System;

namespace Microsoft.Exchange.Rpc.IPFilter
{
	internal abstract class IPFilterRpcServer : RpcServerBase
	{
		public abstract int Add(IPFilterRange element);

		public abstract int Remove(int identity, int filter);

		public abstract IPFilterRange[] GetItems(int startIdentity, int flags, ulong highBytes, ulong lowBytes, int count);

		public IPFilterRpcServer()
		{
		}

		public static IntPtr RpcIntfHandle = (IntPtr)<Module>.IAdminIPFilters_v1_0_s_ifspec;
	}
}
