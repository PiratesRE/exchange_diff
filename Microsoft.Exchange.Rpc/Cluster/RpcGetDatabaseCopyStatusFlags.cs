using System;

namespace Microsoft.Exchange.Rpc.Cluster
{
	[Flags]
	internal enum RpcGetDatabaseCopyStatusFlags : uint
	{
		None = 0U,
		CollectConnectionStatus = 1U,
		CollectExtendedErrorInfo = 2U,
		UseServerSideCaching = 4U
	}
}
