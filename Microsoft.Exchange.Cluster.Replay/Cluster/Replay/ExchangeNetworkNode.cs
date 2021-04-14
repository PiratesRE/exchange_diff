using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using Microsoft.Exchange.Cluster.ClusApi;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class ExchangeNetworkNode
	{
		public ExchangeNetworkNode(string name)
		{
			this.m_name = name;
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

		public AmNodeState ClusterState
		{
			get
			{
				return this.m_clusterState;
			}
			set
			{
				this.m_clusterState = value;
			}
		}

		internal bool HasDnsBeenChecked
		{
			get
			{
				return this.m_hasDnsBeenChecked;
			}
			set
			{
				this.m_hasDnsBeenChecked = value;
			}
		}

		public static List<IPAddress> FindCandidateDnsAddrs()
		{
			List<IPAddress> list = new List<IPAddress>();
			NetworkInterface[] allNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
			foreach (NetworkInterface networkInterface in allNetworkInterfaces)
			{
				IPInterfaceProperties ipproperties = networkInterface.GetIPProperties();
				bool flag = ipproperties.IsDnsEnabled || ipproperties.IsDynamicDnsEnabled;
				if (flag)
				{
					foreach (IPAddressInformation ipaddressInformation in ipproperties.UnicastAddresses)
					{
						if (ipaddressInformation.IsDnsEligible && !ipaddressInformation.IsTransient)
						{
							list.Add(ipaddressInformation.Address);
						}
					}
				}
			}
			return list;
		}

		internal static bool IsAddressPresent(IPAddress[] set, IPAddress addr)
		{
			if (set != null)
			{
				foreach (IPAddress obj in set)
				{
					if (addr.Equals(obj))
					{
						return true;
					}
				}
			}
			return false;
		}

		internal void GetDnsRecords()
		{
			Exception ex = null;
			try
			{
				if (MachineName.Comparer.Equals(this.Name, Environment.MachineName))
				{
					List<IPAddress> list = ExchangeNetworkNode.FindCandidateDnsAddrs();
					this.m_dnsAddresses = list.ToArray();
				}
				else
				{
					this.m_dnsAddresses = Dns.GetHostAddresses(this.Name);
				}
				foreach (IPAddress ipaddress in this.m_dnsAddresses)
				{
					NetworkManager.TraceDebug("Node {0} has DNS for {1}", new object[]
					{
						this.Name,
						ipaddress
					});
				}
			}
			catch (SocketException ex2)
			{
				ex = ex2;
			}
			catch (NetworkInformationException ex3)
			{
				ex = ex3;
			}
			finally
			{
				if (ex != null)
				{
					NetworkManager.TraceError("NetworkMap.GetDnsRecords failed: {0}", new object[]
					{
						ex
					});
				}
				this.m_hasDnsBeenChecked = true;
			}
		}

		internal bool IsEndPointMappedByDNS(IPAddress ipAddr)
		{
			return ExchangeNetworkNode.IsAddressPresent(this.m_dnsAddresses, ipAddr);
		}

		private string m_name;

		private AmNodeState m_clusterState = AmNodeState.Unknown;

		private bool m_hasDnsBeenChecked;

		private IPAddress[] m_dnsAddresses;
	}
}
