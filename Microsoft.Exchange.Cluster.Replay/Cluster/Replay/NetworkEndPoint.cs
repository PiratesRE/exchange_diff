using System;
using System.Net;
using Microsoft.Exchange.Cluster.ClusApi;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class NetworkEndPoint
	{
		public NetworkEndPoint(IPAddress ipAddr, string nodeName, ExchangeSubnet subnet)
		{
			this.m_ipAddress = ipAddr;
			this.m_nodeName = nodeName;
			this.m_subnet = subnet;
		}

		public IPAddress IPAddress
		{
			get
			{
				return this.m_ipAddress;
			}
		}

		public string NodeName
		{
			get
			{
				return this.m_nodeName;
			}
		}

		public ExchangeSubnet Subnet
		{
			get
			{
				return this.m_subnet;
			}
		}

		internal DatabaseAvailabilityGroupNetworkInterface.InterfaceState ClusterNicState
		{
			get
			{
				return this.m_clusterNicState;
			}
			set
			{
				this.m_clusterNicState = value;
			}
		}

		internal bool Usable
		{
			get
			{
				return this.m_usable;
			}
			set
			{
				this.m_usable = value;
			}
		}

		internal static DatabaseAvailabilityGroupNetworkInterface.InterfaceState MapNicState(AmNetInterfaceState clusterNicState)
		{
			switch (clusterNicState)
			{
			case AmNetInterfaceState.Unavailable:
				return DatabaseAvailabilityGroupNetworkInterface.InterfaceState.Unavailable;
			case AmNetInterfaceState.Failed:
				return DatabaseAvailabilityGroupNetworkInterface.InterfaceState.Failed;
			case AmNetInterfaceState.Unreachable:
				return DatabaseAvailabilityGroupNetworkInterface.InterfaceState.Unreachable;
			case AmNetInterfaceState.Up:
				return DatabaseAvailabilityGroupNetworkInterface.InterfaceState.Up;
			default:
				return DatabaseAvailabilityGroupNetworkInterface.InterfaceState.Unknown;
			}
		}

		internal void CopyClusterNicState(AmNetInterfaceState clusterNicState)
		{
			this.ClusterNicState = NetworkEndPoint.MapNicState(clusterNicState);
			if (this.ClusterNicState != DatabaseAvailabilityGroupNetworkInterface.InterfaceState.Up && this.ClusterNicState != DatabaseAvailabilityGroupNetworkInterface.InterfaceState.Unreachable)
			{
				NetworkManager.TraceError("NIC {0} is down: {1}", new object[]
				{
					this.IPAddress,
					clusterNicState
				});
				this.Usable = false;
			}
		}

		private IPAddress m_ipAddress;

		private string m_nodeName;

		private ExchangeSubnet m_subnet;

		private DatabaseAvailabilityGroupNetworkInterface.InterfaceState m_clusterNicState;

		private bool m_usable = true;
	}
}
