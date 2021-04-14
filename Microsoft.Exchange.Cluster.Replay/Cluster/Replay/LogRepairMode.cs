using System;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal enum LogRepairMode : long
	{
		Off,
		Enabled,
		ReplayPending
	}
}
