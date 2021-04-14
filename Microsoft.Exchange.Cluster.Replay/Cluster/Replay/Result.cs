using System;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal enum Result
	{
		Success,
		ShortWaitRetry,
		LongWaitRetry,
		GiveUp
	}
}
