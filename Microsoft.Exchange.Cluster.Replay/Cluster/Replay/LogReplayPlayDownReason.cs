using System;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal enum LogReplayPlayDownReason
	{
		None,
		LagDisabled,
		NotEnoughFreeSpace,
		InRequiredRange,
		NormalLogReplay
	}
}
