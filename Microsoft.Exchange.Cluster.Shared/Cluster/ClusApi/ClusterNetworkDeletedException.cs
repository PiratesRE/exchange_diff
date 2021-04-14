using System;
using Microsoft.Exchange.Cluster.Shared;

namespace Microsoft.Exchange.Cluster.ClusApi
{
	internal class ClusterNetworkDeletedException : ClusterException
	{
		private ClusterNetworkDeletedException() : base("Internal exception due to network deletion")
		{
		}
	}
}
