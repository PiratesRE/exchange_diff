using System;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal enum QueuedItemStatus
	{
		Unknown,
		Started,
		Completed,
		Cancelled,
		Failed
	}
}
