using System;
using System.Net;
using Microsoft.Exchange.Cluster.ClusApi;
using Microsoft.Exchange.Cluster.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class ClusterNic
	{
		public string Name { get; set; }

		public string NodeName { get; set; }

		public bool IsDnsRegistered { get; set; }

		public bool HasIPAddress { get; set; }

		public IPAddress IPAddress { get; set; }

		public AmNetInterfaceState ClusterState { get; set; }

		public ClusterNetwork ClusterNetwork { get; set; }

		public ClusterNode ClusterNode { get; set; }

		public ClusterNic()
		{
		}

		public ClusterNic(AmClusterNetInterface clusNic, ClusterNetwork owningClusNet)
		{
			this.Name = clusNic.Name;
			this.ClusterNetwork = owningClusNet;
			this.NodeName = clusNic.GetNodeName();
			this.ClusterState = clusNic.GetState(false);
			bool flag = false;
			string address = clusNic.GetAddress();
			IPAddress ipaddress = NetworkUtil.ConvertStringToIpAddress(address);
			if (ipaddress != null)
			{
				flag = true;
			}
			else
			{
				NetworkManager.TraceError("Ignoring invalid IPV4 address on NIC {0} since it has invalid ip={1}", new object[]
				{
					this.Name,
					address
				});
				string[] ipv6Addresses = clusNic.GetIPv6Addresses();
				if (ipv6Addresses != null && ipv6Addresses.Length > 0)
				{
					ipaddress = NetworkUtil.ConvertStringToIpAddress(ipv6Addresses[0]);
					if (ipaddress != null)
					{
						flag = true;
					}
				}
			}
			if (flag)
			{
				this.IPAddress = ipaddress;
				this.HasIPAddress = true;
				return;
			}
			NetworkManager.TraceError("ClusterNic '{0}' has no ip addr. Nic state is {1}", new object[]
			{
				this.Name,
				this.ClusterState
			});
		}
	}
}
