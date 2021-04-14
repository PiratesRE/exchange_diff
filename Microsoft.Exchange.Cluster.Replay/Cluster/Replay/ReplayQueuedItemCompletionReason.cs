using System;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal enum ReplayQueuedItemCompletionReason
	{
		None,
		Finished,
		Timedout,
		Cancelled
	}
}
