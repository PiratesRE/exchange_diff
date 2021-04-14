using System;

namespace Microsoft.Exchange.Cluster.Replay
{
	[Flags]
	internal enum CopyStatusClientLookupFlags
	{
		None = 0,
		ReadThrough = 1
	}
}
