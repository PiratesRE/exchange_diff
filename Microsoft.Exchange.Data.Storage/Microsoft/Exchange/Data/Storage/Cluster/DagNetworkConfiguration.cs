using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Cluster
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class DagNetworkConfiguration
	{
		public ushort ReplicationPort
		{
			get
			{
				return this.m_replicationPort;
			}
			set
			{
				this.m_replicationPort = value;
			}
		}

		public DatabaseAvailabilityGroup.NetworkOption NetworkCompression
		{
			get
			{
				return this.m_networkCompression;
			}
			set
			{
				this.m_networkCompression = value;
			}
		}

		public DatabaseAvailabilityGroup.NetworkOption NetworkEncryption
		{
			get
			{
				return this.m_networkEncryption;
			}
			set
			{
				this.m_networkEncryption = value;
			}
		}

		public DatabaseAvailabilityGroupNetwork[] Networks
		{
			get
			{
				return this.m_networks;
			}
			set
			{
				this.m_networks = value;
			}
		}

		public DatabaseAvailabilityGroupNetwork FindNetwork(string nameToFind)
		{
			foreach (DatabaseAvailabilityGroupNetwork databaseAvailabilityGroupNetwork in this.Networks)
			{
				if (DatabaseAvailabilityGroupNetwork.NameComparer.Equals(nameToFind, databaseAvailabilityGroupNetwork.Name))
				{
					return databaseAvailabilityGroupNetwork;
				}
			}
			return null;
		}

		public bool FindSubNet(DatabaseAvailabilityGroupSubnetId subnetToFind, out DatabaseAvailabilityGroupNetwork existingNetwork, out DatabaseAvailabilityGroupNetworkSubnet existingSubnet)
		{
			existingNetwork = null;
			existingSubnet = null;
			foreach (DatabaseAvailabilityGroupNetwork databaseAvailabilityGroupNetwork in this.Networks)
			{
				foreach (DatabaseAvailabilityGroupNetworkSubnet databaseAvailabilityGroupNetworkSubnet in databaseAvailabilityGroupNetwork.Subnets)
				{
					if (databaseAvailabilityGroupNetworkSubnet.SubnetId.Equals(subnetToFind))
					{
						existingNetwork = databaseAvailabilityGroupNetwork;
						existingSubnet = databaseAvailabilityGroupNetworkSubnet;
						return true;
					}
				}
			}
			return false;
		}

		private ushort m_replicationPort = 64327;

		private DatabaseAvailabilityGroup.NetworkOption m_networkCompression = DatabaseAvailabilityGroup.NetworkOption.InterSubnetOnly;

		private DatabaseAvailabilityGroup.NetworkOption m_networkEncryption = DatabaseAvailabilityGroup.NetworkOption.InterSubnetOnly;

		private DatabaseAvailabilityGroupNetwork[] m_networks;
	}
}
