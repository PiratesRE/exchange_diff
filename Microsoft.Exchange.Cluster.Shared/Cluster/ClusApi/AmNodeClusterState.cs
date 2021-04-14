using System;

namespace Microsoft.Exchange.Cluster.ClusApi
{
	internal enum AmNodeClusterState
	{
		NotInstalled,
		NotConfigured,
		NotRunning = 3,
		Running = 19
	}
}
