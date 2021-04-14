using System;

namespace Microsoft.Exchange.Cluster.Shared
{
	internal enum WatchdogAction : uint
	{
		Disable,
		Log,
		TerminateProcess,
		BugCheck,
		Max
	}
}
