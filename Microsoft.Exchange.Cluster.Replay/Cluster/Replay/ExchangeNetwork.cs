using System;
using Microsoft.Exchange.Cluster.ClusApi;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.EseRepl;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class ExchangeNetwork
	{
		public ExchangeNetwork(string name, ExchangeNetworkMap map)
		{
			this.m_name = name;
			this.m_map = map;
		}

		public string Name
		{
			get
			{
				return this.m_name;
			}
			set
			{
				this.m_name = value;
			}
		}

		public string Description
		{
			get
			{
				return this.m_description;
			}
			set
			{
				this.m_description = value;
			}
		}

		public bool MapiAccessEnabled
		{
			get
			{
				return this.m_mapiAccessEnabled;
			}
			internal set
			{
				this.m_mapiAccessEnabled = value;
			}
		}

		public bool ReplicationEnabled
		{
			get
			{
				return this.m_replicationEnabled;
			}
			set
			{
				this.m_replicationEnabled = value;
			}
		}

		public bool IgnoreNetwork
		{
			get
			{
				return this.m_ignoreNetwork;
			}
			set
			{
				this.m_ignoreNetwork = value;
			}
		}

		public DatabaseAvailabilityGroup.NetworkOption Compression
		{
			get
			{
				return this.m_map.NetworkManager.NetworkCompression;
			}
		}

		public DatabaseAvailabilityGroup.NetworkOption Encryption
		{
			get
			{
				return this.m_map.NetworkManager.NetworkEncryption;
			}
		}

		public ExchangeSubnetList Subnets
		{
			get
			{
				return this.m_subnets;
			}
		}

		internal bool IsMisconfigured
		{
			get
			{
				return this.m_isMisconfigured;
			}
			set
			{
				this.m_isMisconfigured = value;
			}
		}

		internal NetworkNodeEndPoints[] EndPoints
		{
			get
			{
				return this.m_endPoints;
			}
		}

		internal ExchangeNetworkPerfmonCounters PerfCounters
		{
			get
			{
				return this.m_perfmonCounters;
			}
			set
			{
				this.m_perfmonCounters = value;
			}
		}

		internal void AddEndPoint(NetworkEndPoint ep, int nodeIndex)
		{
			if (this.m_endPoints == null)
			{
				this.m_endPoints = new NetworkNodeEndPoints[this.m_map.Nodes.Count];
			}
			NetworkNodeEndPoints networkNodeEndPoints = this.m_endPoints[nodeIndex];
			if (networkNodeEndPoints == null)
			{
				networkNodeEndPoints = new NetworkNodeEndPoints();
				this.m_endPoints[nodeIndex] = networkNodeEndPoints;
			}
			if (networkNodeEndPoints.EndPoints.Count > 0)
			{
				string errorText = string.Format("Multiple endpoints for node {0} on network {1}. Ignoring ep:{2}.", ep.NodeName, this.Name, ep.IPAddress);
				this.m_map.RecordInconsistency(errorText);
				this.IsMisconfigured = true;
			}
			else
			{
				NetworkManager.TraceDebug("Added endpoint for node {0} on network {1} at {2}", new object[]
				{
					ep.NodeName,
					this.Name,
					ep.IPAddress
				});
			}
			networkNodeEndPoints.EndPoints.Add(ep);
		}

		internal void ReportError(NetworkPath path)
		{
			this.m_map.ReportError(path, this);
		}

		internal AmNetworkRole GetNativeClusterNetworkRole()
		{
			AmNetworkRole result;
			if (this.IgnoreNetwork)
			{
				result = AmNetworkRole.ClusterNetworkRoleNone;
			}
			else if (this.MapiAccessEnabled)
			{
				result = AmNetworkRole.ClusterNetworkRoleInternalAndClient;
			}
			else
			{
				result = AmNetworkRole.ClusterNetworkRoleInternalUse;
			}
			return result;
		}

		private string m_name;

		private string m_description;

		private ExchangeNetworkMap m_map;

		private bool m_mapiAccessEnabled = true;

		private bool m_replicationEnabled = true;

		private bool m_ignoreNetwork;

		private bool m_isMisconfigured;

		private ExchangeSubnetList m_subnets = new ExchangeSubnetList();

		private NetworkNodeEndPoints[] m_endPoints;

		private ExchangeNetworkPerfmonCounters m_perfmonCounters;
	}
}
