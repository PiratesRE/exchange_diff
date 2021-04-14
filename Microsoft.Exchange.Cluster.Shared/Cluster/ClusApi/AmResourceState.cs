using System;

namespace Microsoft.Exchange.Cluster.ClusApi
{
	internal enum AmResourceState
	{
		Unknown = -1,
		Inherited,
		Initializing,
		Online,
		Offline,
		Failed,
		CannotComeOnlineOnAnyNode = 126,
		CannotComeOnlineOnThisNode,
		Pending,
		OnlinePending,
		OfflinePending
	}
}
