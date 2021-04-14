using System;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	[Flags]
	internal enum AmBcsServerChecks
	{
		None = 0,
		DebugOptionDisabled = 1,
		ClusterNodeUp = 2,
		DatacenterActivationModeStarted = 4,
		AutoActivationAllowed = 8
	}
}
