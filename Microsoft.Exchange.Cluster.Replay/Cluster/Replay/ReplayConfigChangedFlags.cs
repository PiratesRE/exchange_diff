using System;

namespace Microsoft.Exchange.Cluster.Replay
{
	[Flags]
	internal enum ReplayConfigChangedFlags
	{
		None = 0,
		ActiveServer = 1,
		Other = 2
	}
}
