using System;

namespace Microsoft.Exchange.Cluster.ClusApi
{
	internal enum AmGroupState
	{
		Unknown = -1,
		Online,
		Offline,
		Failed,
		PartialOnline,
		Pending
	}
}
