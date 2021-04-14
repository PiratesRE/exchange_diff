using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class LogicalNetwork
	{
		public string Name { get; set; }

		public string Description { get; set; }

		public bool ReplicationEnabled { get; set; }

		public bool IgnoreNetwork { get; set; }

		public static string BuildDefaultReplNetName(int index)
		{
			return string.Format("{0}{1:D2}", "ReplicationDagNetwork", index);
		}

		public List<Subnet> Subnets
		{
			get
			{
				return this.m_memberNets;
			}
		}

		public bool HasDnsNic()
		{
			foreach (Subnet subnet in this.Subnets)
			{
				if (subnet.ClusterNetwork != null && subnet.ClusterNetwork.HasDnsNic)
				{
					return true;
				}
			}
			return false;
		}

		public void Add(Subnet subnet)
		{
			subnet.LogicalNetwork = this;
			this.m_memberNets.Add(subnet);
		}

		public LogicalNetwork()
		{
			this.ReplicationEnabled = true;
			this.IgnoreNetwork = false;
		}

		public const string DefaultDnsNetName = "MapiDagNetwork";

		public const string DefaultPrefix = "ReplicationDagNetwork";

		private List<Subnet> m_memberNets = new List<Subnet>(3);
	}
}
