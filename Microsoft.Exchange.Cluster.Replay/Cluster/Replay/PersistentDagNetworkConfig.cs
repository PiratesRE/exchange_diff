using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Cluster.Replay
{
	[Serializable]
	public class PersistentDagNetworkConfig
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

		public bool ManualDagNetworkConfiguration
		{
			get
			{
				return this.m_manualDagNetworkConfiguration;
			}
			set
			{
				this.m_manualDagNetworkConfiguration = value;
			}
		}

		public List<PersistentDagNetwork> Networks
		{
			get
			{
				return this.m_networks;
			}
		}

		internal static PersistentDagNetworkConfig Deserialize(string xmlText)
		{
			return (PersistentDagNetworkConfig)SerializationUtil.XmlToObject(xmlText, typeof(PersistentDagNetworkConfig));
		}

		internal string Serialize()
		{
			return SerializationUtil.ObjectToXml(this);
		}

		internal PersistentDagNetworkConfig Copy()
		{
			string xmlText = this.Serialize();
			return PersistentDagNetworkConfig.Deserialize(xmlText);
		}

		internal bool RemoveEmptyNetworks()
		{
			List<PersistentDagNetwork> list = new List<PersistentDagNetwork>();
			foreach (PersistentDagNetwork persistentDagNetwork in this.Networks)
			{
				if (persistentDagNetwork.Subnets.Count == 0)
				{
					list.Add(persistentDagNetwork);
				}
			}
			foreach (PersistentDagNetwork item in list)
			{
				this.Networks.Remove(item);
			}
			return list.Count > 0;
		}

		internal PersistentDagNetwork FindNetwork(string nameToFind)
		{
			foreach (PersistentDagNetwork persistentDagNetwork in this.Networks)
			{
				if (DatabaseAvailabilityGroupNetwork.NameComparer.Equals(nameToFind, persistentDagNetwork.Name))
				{
					return persistentDagNetwork;
				}
			}
			return null;
		}

		internal bool RemoveNetwork(string nameToRemove)
		{
			PersistentDagNetwork persistentDagNetwork = this.FindNetwork(nameToRemove);
			if (persistentDagNetwork != null)
			{
				this.Networks.Remove(persistentDagNetwork);
				return true;
			}
			return false;
		}

		private ushort m_replicationPort = 64327;

		private DatabaseAvailabilityGroup.NetworkOption m_networkCompression = DatabaseAvailabilityGroup.NetworkOption.InterSubnetOnly;

		private DatabaseAvailabilityGroup.NetworkOption m_networkEncryption = DatabaseAvailabilityGroup.NetworkOption.InterSubnetOnly;

		private List<PersistentDagNetwork> m_networks = new List<PersistentDagNetwork>();

		private bool m_manualDagNetworkConfiguration;
	}
}
