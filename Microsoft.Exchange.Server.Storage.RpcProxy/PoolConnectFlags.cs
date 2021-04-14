using System;

namespace Microsoft.Exchange.Server.Storage.RpcProxy
{
	[Flags]
	internal enum PoolConnectFlags : uint
	{
		None = 0U,
		NoProxy = 1U
	}
}
