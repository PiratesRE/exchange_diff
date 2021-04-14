using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Exchange.Cluster.ClusApi;
using Microsoft.Exchange.Cluster.Shared;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class ClusterNode
	{
		public AmServerName Name { get; set; }

		public AmNodeState ClusterState { get; set; }

		public IPAddress[] DnsAddresses { get; set; }

		public List<ClusterNic> Nics
		{
			get
			{
				return this.m_nics;
			}
		}

		public ClusterNode(IAmClusterNode clusNode)
		{
			this.Name = clusNode.Name;
			this.ClusterState = clusNode.GetState(false);
		}

		private List<ClusterNic> m_nics = new List<ClusterNic>(3);
	}
}
