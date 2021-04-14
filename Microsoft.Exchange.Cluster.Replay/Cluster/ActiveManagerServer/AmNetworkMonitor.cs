using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Exchange.Cluster.ClusApi;
using Microsoft.Exchange.Cluster.Common;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.HA;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class AmNetworkMonitor
	{
		public static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.AmNetworkMonitorTracer;
			}
		}

		public void UseCluster(IAmCluster cluster)
		{
			this.m_sharedCluster = cluster;
		}

		private IAmCluster GetCluster()
		{
			return this.m_sharedCluster;
		}

		private void TriggerClusterRefresh(string reason)
		{
			AmNetworkMonitor.Tracer.TraceError<string>(0L, "TriggerClusterRefresh because {0}. Not yet implemented", reason);
		}

		public void RefreshMapiNetwork()
		{
			Exception ex = null;
			try
			{
				Dictionary<AmServerName, AmNetworkMonitor.Node> dictionary = new Dictionary<AmServerName, AmNetworkMonitor.Node>();
				Dictionary<string, AmNetworkMonitor.Nic> dictionary2 = new Dictionary<string, AmNetworkMonitor.Nic>();
				using (IAmCluster amCluster = ClusterFactory.Instance.Open())
				{
					lock (this)
					{
						ExTraceGlobals.AmNetworkMonitorTracer.TraceDebug((long)this.GetHashCode(), "RefreshMapiNetwork running");
						foreach (IAmClusterNode amClusterNode in amCluster.EnumerateNodes())
						{
							using (amClusterNode)
							{
								AmNetworkMonitor.Node node = new AmNetworkMonitor.Node(amClusterNode.Name);
								dictionary.Add(node.Name, node);
								IPAddress[] dnsAddresses = NetworkUtil.GetDnsAddresses(amClusterNode.Name.Fqdn, ref ex);
								foreach (AmClusterNetInterface amClusterNetInterface in amClusterNode.EnumerateNetInterfaces())
								{
									using (amClusterNetInterface)
									{
										bool flag2 = false;
										IPAddress ipaddress = NetworkUtil.ConvertStringToIpAddress(amClusterNetInterface.GetAddress());
										if (ipaddress != null && NetworkUtil.IsAddressPresent(dnsAddresses, ipaddress))
										{
											flag2 = true;
											AmNetworkMonitor.Tracer.TraceDebug<string, IPAddress>((long)this.GetHashCode(), "NIC '{0}' on DNS at {1}", amClusterNetInterface.Name, ipaddress);
										}
										if (!flag2)
										{
											string[] ipv6Addresses = amClusterNetInterface.GetIPv6Addresses();
											if (ipv6Addresses != null && ipv6Addresses.Length > 0)
											{
												foreach (string text in ipv6Addresses)
												{
													ipaddress = NetworkUtil.ConvertStringToIpAddress(text);
													if (ipaddress != null && NetworkUtil.IsAddressPresent(dnsAddresses, ipaddress))
													{
														flag2 = true;
														AmNetworkMonitor.Tracer.TraceDebug<string, string>((long)this.GetHashCode(), "NIC '{0}' on DNS at {1}", amClusterNetInterface.Name, text);
														break;
													}
												}
											}
										}
										if (flag2)
										{
											AmNetworkMonitor.Nic nic = new AmNetworkMonitor.Nic(amClusterNetInterface.Name, node);
											node.MapiNics.Add(nic);
											dictionary2.Add(nic.Name, nic);
										}
									}
								}
							}
						}
						this.m_nodeTable = dictionary;
						this.m_nicTable = dictionary2;
					}
				}
			}
			catch (ClusterException ex2)
			{
				ex = ex2;
			}
			finally
			{
				this.m_firstRefreshCompleted.Set();
				this.m_firstRefreshCompleted.Close();
			}
			if (ex != null)
			{
				AmNetworkMonitor.Tracer.TraceError<Exception>(0L, "RefreshMapiNetwork fails:{0}", ex);
			}
		}

		public bool TryWaitForInitialization()
		{
			TimeSpan timeout = TimeSpan.FromSeconds((double)RegistryParameters.NetworkManagerStartupTimeoutInSec);
			return this.m_firstRefreshCompleted.WaitOne(timeout) == ManualOneShotEvent.Result.Success;
		}

		public bool IsNodePubliclyUp(AmServerName nodeName)
		{
			this.TryWaitForInitialization();
			Dictionary<AmServerName, AmNetworkMonitor.Node> nodeTable = this.m_nodeTable;
			if (nodeTable == null || this.GetCluster() == null)
			{
				AmNetworkMonitor.Tracer.TraceError<string>(0L, "Not yet initialized. Assuming {0} is Down", nodeName.NetbiosName);
				return false;
			}
			AmNetworkMonitor.Node node;
			if (!nodeTable.TryGetValue(nodeName, out node))
			{
				AmNetworkMonitor.Tracer.TraceError<string>((long)nodeTable.GetHashCode(), "{0} is not known. Assuming Down", nodeName.NetbiosName);
				return false;
			}
			return this.IsNodePubliclyUp(node);
		}

		private bool IsNodePubliclyUp(AmNetworkMonitor.Node node)
		{
			IAmCluster cluster = this.GetCluster();
			if (cluster == null)
			{
				AmNetworkMonitor.Tracer.TraceError<AmServerName>(0L, "If cluster object is not valid, then assume node {0} is up", node.Name);
				return true;
			}
			Exception ex;
			AmNodeState nodeState = cluster.GetNodeState(node.Name, out ex);
			if (ex != null)
			{
				return false;
			}
			if (!AmClusterNode.IsNodeUp(nodeState))
			{
				return false;
			}
			AmClusterNodeNetworkStatus amClusterNodeNetworkStatus = AmClusterNodeStatusAccessor.Read(cluster, node.Name, out ex);
			return amClusterNodeNetworkStatus == null || amClusterNodeNetworkStatus.HasADAccess;
		}

		public List<AmServerName> GetServersReportedAsPubliclyUp()
		{
			List<AmServerName> list = new List<AmServerName>();
			Dictionary<AmServerName, AmNetworkMonitor.Node> nodeTable = this.m_nodeTable;
			if (nodeTable != null)
			{
				foreach (KeyValuePair<AmServerName, AmNetworkMonitor.Node> keyValuePair in nodeTable)
				{
					AmNetworkMonitor.Node value = keyValuePair.Value;
					if (this.IsNodePubliclyUp(value))
					{
						list.Add(value.Name);
					}
				}
			}
			return list;
		}

		public bool AreAnyMapiNicsUp(AmServerName nodeName)
		{
			Dictionary<AmServerName, AmNetworkMonitor.Node> nodeTable = this.m_nodeTable;
			if (nodeTable == null || this.GetCluster() == null)
			{
				AmNetworkMonitor.Tracer.TraceError<string>(0L, "Not yet initialized. Assuming {0} is Up", nodeName.NetbiosName);
				return true;
			}
			AmNetworkMonitor.Node node;
			if (!nodeTable.TryGetValue(nodeName, out node))
			{
				AmNetworkMonitor.Tracer.TraceError<string>((long)nodeTable.GetHashCode(), "{0} is not known. Assuming Up", nodeName.NetbiosName);
				return true;
			}
			return this.AreAnyMapiNicsUp(node);
		}

		private bool AreAnyMapiNicsUp(AmNetworkMonitor.Node node)
		{
			int num = 0;
			foreach (AmNetworkMonitor.Nic nic in node.MapiNics)
			{
				num++;
				AmNetInterfaceState nicState = this.GetNicState(nic.Name);
				switch (nicState)
				{
				case AmNetInterfaceState.Unavailable:
				case AmNetInterfaceState.Failed:
				case AmNetInterfaceState.Unreachable:
					AmNetworkMonitor.Tracer.TraceError<string, AmNetInterfaceState>(0L, "Nic '{0}' is {1}.", nic.Name, nicState);
					break;
				case AmNetInterfaceState.Up:
					return true;
				default:
					AmNetworkMonitor.Tracer.TraceError<string, AmNetInterfaceState>(0L, "Nic '{0}' is {1}.", nic.Name, nicState);
					return true;
				}
			}
			AmNetworkMonitor.Tracer.TraceError<AmServerName, int>(0L, "Node {0} has {1} MAPI nics. None appear usable.", node.Name, num);
			return false;
		}

		private AmNetInterfaceState GetNicState(string nicName)
		{
			AmNetInterfaceState result = AmNetInterfaceState.Unknown;
			IAmCluster cluster = this.GetCluster();
			if (cluster != null)
			{
				Exception ex;
				result = cluster.GetNetInterfaceState(nicName, out ex);
				if (ex != null)
				{
					AmNetworkMonitor.Tracer.TraceError<string, Exception>(0L, "Failed to get state for nic '{0}': {1}", nicName, ex);
					this.TriggerClusterRefresh("GetNicState failed");
				}
			}
			return result;
		}

		public void ProcessEvent(AmClusterEventInfo cei)
		{
			if (cei.IsNetInterfaceStateChanged)
			{
				this.ProcessNicEvent(cei);
			}
		}

		private void ProcessNicEvent(AmClusterEventInfo cei)
		{
			Dictionary<string, AmNetworkMonitor.Nic> nicTable = this.m_nicTable;
			IAmCluster cluster = this.GetCluster();
			if (nicTable == null || cluster == null)
			{
				AmNetworkMonitor.Tracer.TraceError(0L, "Not yet initialized. Ignoring event");
				return;
			}
			AmNetworkMonitor.Nic nic;
			if (!nicTable.TryGetValue(cei.ObjectName, out nic))
			{
				this.TriggerClusterRefresh("nic not found");
				return;
			}
			AmNetInterfaceState nicState = this.GetNicState(cei.ObjectName);
			switch (nicState)
			{
			case AmNetInterfaceState.Unavailable:
				AmNetworkMonitor.Tracer.TraceError<string, AmNetInterfaceState>(0L, "MAPI NIC '{0}' is {1}.", cei.ObjectName, nicState);
				return;
			case AmNetInterfaceState.Failed:
			case AmNetInterfaceState.Unreachable:
			{
				AmNetworkMonitor.Tracer.TraceError<string, AmNetInterfaceState>(0L, "MAPI NIC '{0}' is {1}.", cei.ObjectName, nicState);
				AmEvtMapiNetworkFailure amEvtMapiNetworkFailure = new AmEvtMapiNetworkFailure(nic.Node.Name);
				amEvtMapiNetworkFailure.Notify(true);
				return;
			}
			case AmNetInterfaceState.Up:
				AmNetworkMonitor.Tracer.TraceDebug<string>(0L, "MAPI NIC '{0}' is Up.", cei.ObjectName);
				if (nic.Node.Name.IsLocalComputerName)
				{
					AmClusterNodeNetworkStatus amClusterNodeNetworkStatus = new AmClusterNodeNetworkStatus();
					Exception ex = AmClusterNodeStatusAccessor.Write(cluster, nic.Node.Name, amClusterNodeNetworkStatus);
					if (ex != null)
					{
						ReplayCrimsonEvents.AmNodeStatusUpdateFailed.Log<string, string>(amClusterNodeNetworkStatus.ToString(), ex.Message);
						return;
					}
				}
				break;
			default:
				AmNetworkMonitor.Tracer.TraceError<AmNetInterfaceState, string>(0L, "Unexpected NIC state {0} for {1}", nicState, cei.ObjectName);
				break;
			}
		}

		private const string FirstRefreshCompletedEventName = "FirstRefreshCompletedEvent";

		private Dictionary<AmServerName, AmNetworkMonitor.Node> m_nodeTable;

		private Dictionary<string, AmNetworkMonitor.Nic> m_nicTable;

		private IAmCluster m_sharedCluster;

		private ManualOneShotEvent m_firstRefreshCompleted = new ManualOneShotEvent("FirstRefreshCompletedEvent");

		private class Node
		{
			public AmServerName Name { get; set; }

			public List<AmNetworkMonitor.Nic> MapiNics { get; set; }

			public Node(AmServerName name)
			{
				this.Name = name;
				this.MapiNics = new List<AmNetworkMonitor.Nic>();
			}
		}

		private class Nic
		{
			public string Name { get; set; }

			public AmNetworkMonitor.Node Node { get; set; }

			public Nic(string name, AmNetworkMonitor.Node node)
			{
				this.Name = name;
				this.Node = node;
			}
		}
	}
}
