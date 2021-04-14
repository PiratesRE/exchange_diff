using System;
using System.Collections.Generic;
using Microsoft.Exchange.Cluster.ClusApi;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class ExchangeSubnet
	{
		internal ExchangeSubnet(DatabaseAvailabilityGroupNetworkSubnet subnet)
		{
			this.m_subnet = subnet;
		}

		internal ExchangeSubnet(DatabaseAvailabilityGroupSubnetId subnetId)
		{
			this.m_subnet = new DatabaseAvailabilityGroupNetworkSubnet();
			this.m_subnet.SubnetId = subnetId;
		}

		internal DatabaseAvailabilityGroupSubnetId SubnetId
		{
			get
			{
				return this.m_subnet.SubnetId;
			}
		}

		internal DatabaseAvailabilityGroupNetworkSubnet SubnetAndState
		{
			get
			{
				return this.m_subnet;
			}
		}

		internal ExchangeNetwork Network
		{
			get
			{
				return this.m_network;
			}
			set
			{
				this.m_network = value;
			}
		}

		internal static DatabaseAvailabilityGroupNetworkSubnet.SubnetState MapSubnetState(AmNetworkState clusterState)
		{
			switch (clusterState)
			{
			case AmNetworkState.Unavailable:
				return DatabaseAvailabilityGroupNetworkSubnet.SubnetState.Unavailable;
			case AmNetworkState.Down:
				return DatabaseAvailabilityGroupNetworkSubnet.SubnetState.Down;
			case AmNetworkState.Partitioned:
				return DatabaseAvailabilityGroupNetworkSubnet.SubnetState.Partitioned;
			case AmNetworkState.Up:
				return DatabaseAvailabilityGroupNetworkSubnet.SubnetState.Up;
			default:
				return DatabaseAvailabilityGroupNetworkSubnet.SubnetState.Unknown;
			}
		}

		internal static DatabaseAvailabilityGroupSubnetId ExtractSubnetId(AmClusterNetwork clusNet)
		{
			DatabaseAvailabilityGroupSubnetId databaseAvailabilityGroupSubnetId = null;
			IEnumerable<string> enumerable = clusNet.EnumerateAlternateIPv4Names();
			foreach (string text in enumerable)
			{
				try
				{
					databaseAvailabilityGroupSubnetId = new DatabaseAvailabilityGroupSubnetId(text);
					break;
				}
				catch (FormatException ex)
				{
					NetworkManager.TraceError("Ignoring invalid ipv4 subnet {0}. Exception:{1}", new object[]
					{
						text,
						ex
					});
					databaseAvailabilityGroupSubnetId = null;
				}
			}
			if (databaseAvailabilityGroupSubnetId == null)
			{
				IEnumerable<string> enumerable2 = clusNet.EnumeratePureAlternateIPv6Names();
				foreach (string text2 in enumerable2)
				{
					try
					{
						databaseAvailabilityGroupSubnetId = new DatabaseAvailabilityGroupSubnetId(text2);
						break;
					}
					catch (FormatException ex2)
					{
						NetworkManager.TraceError("Ignoring invalid ipv6 subnet {0}. Exception:{1}", new object[]
						{
							text2,
							ex2
						});
						databaseAvailabilityGroupSubnetId = null;
					}
				}
			}
			return databaseAvailabilityGroupSubnetId;
		}

		private DatabaseAvailabilityGroupNetworkSubnet m_subnet;

		private ExchangeNetwork m_network;
	}
}
