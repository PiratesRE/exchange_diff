using System;
using System.Collections.Generic;
using Microsoft.Exchange.Cluster.ClusApi;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class ClusterNetwork
	{
		public DatabaseAvailabilityGroupSubnetId SubnetId { get; private set; }

		public AmNetworkState ClusterState { get; set; }

		public bool HasDnsNic { get; set; }

		public List<ClusterNic> Nics
		{
			get
			{
				return this.m_nics;
			}
		}

		public ClusterNetwork(AmClusterNetwork clusNet)
		{
			this.SubnetId = ExchangeSubnet.ExtractSubnetId(clusNet);
			this.ClusterState = clusNet.GetState(false);
			if (this.SubnetId == null)
			{
				ExTraceGlobals.NetworkManagerTracer.TraceError<string>(0L, "ClusterNetwork.Subnet is null for network {0}", clusNet.Name);
				throw new ClusterNetworkNullSubnetException(clusNet.Name);
			}
			IEnumerable<AmClusterNetInterface> enumerable = clusNet.EnumerateNetworkInterfaces();
			try
			{
				foreach (AmClusterNetInterface clusNic in enumerable)
				{
					ClusterNic item = new ClusterNic(clusNic, this);
					this.m_nics.Add(item);
				}
			}
			finally
			{
				foreach (AmClusterNetInterface amClusterNetInterface in enumerable)
				{
					using (amClusterNetInterface)
					{
					}
				}
			}
		}

		public ClusterNetwork(DatabaseAvailabilityGroupSubnetId subnetId)
		{
			this.SubnetId = subnetId;
		}

		public LogicalNetwork LogicalNetwork { get; set; }

		private List<ClusterNic> m_nics = new List<ClusterNic>(8);
	}
}
