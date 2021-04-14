using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class Subnet
	{
		public DatabaseAvailabilityGroupSubnetId SubnetId { get; private set; }

		public ClusterNetwork ClusterNetwork { get; set; }

		public LogicalNetwork LogicalNetwork { get; set; }

		public Subnet(ClusterNetwork clusNet)
		{
			this.SubnetId = clusNet.SubnetId;
			this.ClusterNetwork = clusNet;
		}

		public Subnet(DatabaseAvailabilityGroupSubnetId subnetId)
		{
			this.SubnetId = subnetId;
		}
	}
}
