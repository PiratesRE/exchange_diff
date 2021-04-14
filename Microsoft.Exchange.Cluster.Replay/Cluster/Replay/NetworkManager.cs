using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Cluster.ActiveManagerServer;
using Microsoft.Exchange.Cluster.ClusApi;
using Microsoft.Exchange.Cluster.Common;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.HA;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Cluster;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.EseRepl;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class NetworkManager
	{
		public static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.NetworkManagerTracer;
			}
		}

		private static NetworkOption MapESEReplNetworkOption(DatabaseAvailabilityGroup.NetworkOption option)
		{
			switch (option)
			{
			case DatabaseAvailabilityGroup.NetworkOption.Disabled:
				return NetworkOption.Disabled;
			case DatabaseAvailabilityGroup.NetworkOption.Enabled:
				return NetworkOption.Enabled;
			case DatabaseAvailabilityGroup.NetworkOption.InterSubnetOnly:
				return NetworkOption.InterSubnetOnly;
			case DatabaseAvailabilityGroup.NetworkOption.SeedOnly:
				return NetworkOption.SeedOnly;
			default:
				return NetworkOption.Disabled;
			}
		}

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

		public bool ManualDagNetworkConfiguration { get; private set; }

		private AmClusterHandle ClusterHandle
		{
			get
			{
				return this.m_hCluster;
			}
		}

		private bool EseReplDagNetConfigIsStale { get; set; }

		public static NetworkPath ChooseNetworkPath(string targetName, string networkName, NetworkPath.ConnectionPurpose purpose)
		{
			ITcpConnector tcpConnector = Dependencies.TcpConnector;
			return tcpConnector.ChooseDagNetworkPath(targetName, networkName, purpose);
		}

		internal static NetworkPath InternalChooseDagNetworkPath(string targetName, string networkName, NetworkPath.ConnectionPurpose purpose)
		{
			string nodeNameFromFqdn = MachineName.GetNodeNameFromFqdn(targetName);
			NetworkManager manager = NetworkManager.GetManager();
			NetworkPath networkPath = null;
			DagNetConfig dagNetConfig = null;
			manager.TryWaitForInitialization();
			if (manager.m_netMap != null)
			{
				networkPath = manager.m_netMap.ChoosePath(nodeNameFromFqdn, networkName);
			}
			else
			{
				DagNetRoute dagNetRoute = null;
				DagNetRoute[] array = DagNetChooser.ProposeRoutes(targetName, out dagNetConfig);
				if (networkName != null)
				{
					foreach (DagNetRoute dagNetRoute2 in array)
					{
						if (StringUtil.IsEqualIgnoreCase(dagNetRoute2.NetworkName, networkName))
						{
							dagNetRoute = dagNetRoute2;
							break;
						}
					}
				}
				else if (array != null && array.Length > 0)
				{
					dagNetRoute = array[0];
				}
				if (dagNetRoute != null)
				{
					networkPath = new NetworkPath(targetName, dagNetRoute.TargetIPAddr, dagNetRoute.TargetPort, dagNetRoute.SourceIPAddr);
					networkPath.NetworkName = dagNetRoute.NetworkName;
					networkPath.CrossSubnet = dagNetRoute.IsCrossSubnet;
				}
			}
			if (networkPath == null)
			{
				networkPath = NetworkManager.BuildDnsNetworkPath(targetName, NetworkManager.GetReplicationPort());
			}
			networkPath.Purpose = purpose;
			if (dagNetConfig == null)
			{
				dagNetConfig = DagNetEnvironment.FetchNetConfig();
			}
			networkPath.ApplyNetworkPolicy(dagNetConfig);
			return networkPath;
		}

		private static NetworkPath BuildDnsNetworkPath(string targetName, ushort replicationPort)
		{
			try
			{
				IPAddress ipaddress = NetworkManager.ChooseAddressFromDNS(targetName);
				if (ipaddress != null)
				{
					return new NetworkPath(targetName, ipaddress, (int)replicationPort, null)
					{
						NetworkChoiceIsMandatory = true
					};
				}
			}
			catch (SocketException ex)
			{
				throw new NetworkTransportException(ReplayStrings.NetworkAddressResolutionFailed(targetName, ex.Message), ex);
			}
			throw new NetworkTransportException(ReplayStrings.NetworkAddressResolutionFailedNoDnsEntry(targetName));
		}

		public static IPAddress ChooseAddressFromDNS(string targetName)
		{
			Exception ex;
			IPAddress[] dnsAddresses = NetworkUtil.GetDnsAddresses(targetName, ref ex);
			foreach (IPAddress ipaddress in dnsAddresses)
			{
				if (ipaddress.AddressFamily == AddressFamily.InterNetwork)
				{
					return ipaddress;
				}
			}
			foreach (IPAddress ipaddress2 in dnsAddresses)
			{
				if (ipaddress2.AddressFamily == AddressFamily.InterNetworkV6)
				{
					return ipaddress2;
				}
			}
			return null;
		}

		public static NetworkManager GetManager()
		{
			return NetworkManager.s_mgr;
		}

		public static ExchangeNetworkMap GetMap()
		{
			return NetworkManager.s_mgr.m_netMap;
		}

		public static void Start()
		{
			lock (NetworkManager.s_mgrLock)
			{
				if (!NetworkManager.s_initialized)
				{
					NetworkManager.s_mgr.Initialize();
					NetworkManager.s_initialized = true;
				}
			}
		}

		public static void ReportError(NetworkPath path, Exception e)
		{
		}

		public static void HandleUnexpectedException(Exception e)
		{
			ExTraceGlobals.NetworkManagerTracer.TraceError<Exception>(0L, "UnexpectedException {0}", e);
			throw e;
		}

		internal static ushort GetReplicationPort()
		{
			NetworkManager manager = NetworkManager.GetManager();
			if (manager == null)
			{
				return 64327;
			}
			return manager.ReplicationPort;
		}

		internal static void TraceDebug(string format, params object[] args)
		{
			ExTraceGlobals.NetworkManagerTracer.TraceDebug(0L, format, args);
		}

		internal static void TraceError(string format, params object[] args)
		{
			ExTraceGlobals.NetworkManagerTracer.TraceError(0L, format, args);
		}

		internal static void ThrowException(Exception e)
		{
			NetworkManager.TraceError("Throwing exception: {0}", new object[]
			{
				e
			});
			throw e;
		}

		internal static TcpClientChannel OpenConnection(ref NetworkPath actualPath, int timeoutInMsec, bool ignoreNodeDown)
		{
			NetworkPath networkPath = actualPath;
			NetworkTransportException ex = null;
			ITcpConnector tcpConnector = Dependencies.TcpConnector;
			TcpClientChannel tcpClientChannel = tcpConnector.TryConnect(networkPath, timeoutInMsec, out ex);
			if (tcpClientChannel != null)
			{
				return tcpClientChannel;
			}
			if (!networkPath.NetworkChoiceIsMandatory)
			{
				NetworkManager.TraceError("Attempting alternate routes", new object[0]);
				List<NetworkPath> list = null;
				ExchangeNetworkMap map = NetworkManager.GetMap();
				if (map != null)
				{
					list = map.EnumeratePaths(networkPath.TargetNodeName, ignoreNodeDown);
					if (list != null)
					{
						NetworkPath networkPath2 = null;
						foreach (NetworkPath networkPath3 in list)
						{
							if (string.Equals(networkPath3.NetworkName, networkPath.NetworkName, DatabaseAvailabilityGroupNetwork.NameComparison))
							{
								networkPath2 = networkPath3;
								break;
							}
						}
						if (networkPath2 != null)
						{
							list.Remove(networkPath2);
						}
						DagNetConfig dagConfig = DagNetEnvironment.FetchNetConfig();
						foreach (NetworkPath networkPath4 in list)
						{
							networkPath4.Purpose = networkPath.Purpose;
							networkPath4.ApplyNetworkPolicy(dagConfig);
							tcpClientChannel = tcpConnector.TryConnect(networkPath, timeoutInMsec, out ex);
							if (tcpClientChannel != null)
							{
								actualPath = networkPath4;
								return tcpClientChannel;
							}
						}
					}
				}
			}
			throw ex;
		}

		internal static ExchangeNetwork GetNetwork(string netName)
		{
			ExchangeNetworkMap map = NetworkManager.GetMap();
			if (map != null)
			{
				return map.GetNetwork(netName);
			}
			return null;
		}

		internal static ExchangeNetwork LookupEndPoint(IPAddress ipAddr, out NetworkEndPoint endPoint)
		{
			endPoint = null;
			ExchangeNetworkMap map = NetworkManager.GetMap();
			if (map != null)
			{
				return map.LookupEndPoint(ipAddr, out endPoint);
			}
			return null;
		}

		internal static ExchangeNetworkPerfmonCounters GetPerfCounters(string netName)
		{
			ExchangeNetwork network = NetworkManager.GetNetwork(netName);
			if (network != null)
			{
				return network.PerfCounters;
			}
			return null;
		}

		internal static void Shutdown()
		{
			lock (NetworkManager.s_mgrLock)
			{
				NetworkManager.TraceDebug("Shutdown initiated.", new object[0]);
				NetworkManager.s_mgr.m_shutdown = true;
				NetworkManager.s_mgr.m_firstDriveMapRefreshCompleted.Close();
			}
		}

		internal static DagNetworkConfiguration GetDagNetworkConfig()
		{
			DagNetworkConfiguration config = null;
			NetworkManager.RunRpcOperation("GetDagNetworkConfig", delegate(object param0, EventArgs param1)
			{
				ExchangeNetworkMap exchangeNetworkMap = NetworkManager.FetchInitializedMap();
				config = new DagNetworkConfiguration();
				config.NetworkCompression = exchangeNetworkMap.NetworkManager.NetworkCompression;
				config.NetworkEncryption = exchangeNetworkMap.NetworkManager.NetworkEncryption;
				config.ReplicationPort = exchangeNetworkMap.NetworkManager.ReplicationPort;
				config.Networks = exchangeNetworkMap.GetDagNets();
			});
			return config;
		}

		internal static void SetDagNetworkConfig(SetDagNetworkConfigRequest configChange)
		{
			NetworkManager.RunRpcOperation("SetDagNetworkConfig", delegate(object param0, EventArgs param1)
			{
				NetworkManager manager = NetworkManager.GetManager();
				if (manager == null)
				{
					throw new DagNetworkManagementException(ReplayStrings.NetworkManagerInitError);
				}
				lock (manager.m_mapRefreshLock)
				{
					using (IAmCluster amCluster = ClusterFactory.Instance.Open())
					{
						using (DagConfigurationStore dagConfigurationStore = new DagConfigurationStore())
						{
							dagConfigurationStore.Open();
							PersistentDagNetworkConfig persistentDagNetworkConfig = dagConfigurationStore.LoadNetworkConfig();
							if (persistentDagNetworkConfig == null)
							{
								persistentDagNetworkConfig = new PersistentDagNetworkConfig();
							}
							else
							{
								string text = persistentDagNetworkConfig.Serialize();
								ReplayEventLogConstants.Tuple_DagNetworkConfigOld.LogEvent(DateTime.UtcNow.ToString(), new object[]
								{
									text
								});
							}
							if (configChange.SetPort)
							{
								persistentDagNetworkConfig.ReplicationPort = configChange.ReplicationPort;
								manager.ReplicationPort = configChange.ReplicationPort;
							}
							manager.NetworkCompression = configChange.NetworkCompression;
							persistentDagNetworkConfig.NetworkCompression = configChange.NetworkCompression;
							manager.NetworkEncryption = configChange.NetworkEncryption;
							persistentDagNetworkConfig.NetworkEncryption = configChange.NetworkEncryption;
							manager.ManualDagNetworkConfiguration = configChange.ManualDagNetworkConfiguration;
							persistentDagNetworkConfig.ManualDagNetworkConfiguration = configChange.ManualDagNetworkConfiguration;
							if (configChange.DiscoverNetworks)
							{
								NetworkDiscovery networkDiscovery = new NetworkDiscovery();
								networkDiscovery.LoadClusterObjects(amCluster);
								networkDiscovery.DetermineDnsStatus();
								networkDiscovery.AggregateNetworks(true);
								ExchangeNetworkMap exchangeNetworkMap = new ExchangeNetworkMap(manager);
								exchangeNetworkMap.Load(networkDiscovery);
								persistentDagNetworkConfig = exchangeNetworkMap.BuildPersistentDagNetworkConfig();
							}
							manager.UpdateNetworkConfig(persistentDagNetworkConfig);
						}
					}
				}
			});
		}

		internal static void SetDagNetwork(SetDagNetworkRequest changeReq)
		{
			NetworkManager.RunRpcOperation("SetDagNetwork", delegate(object param0, EventArgs param1)
			{
				NetworkManager manager = NetworkManager.GetManager();
				if (manager == null)
				{
					throw new DagNetworkManagementException(ReplayStrings.NetworkManagerInitError);
				}
				lock (manager.m_mapRefreshLock)
				{
					using (DagConfigurationStore dagConfigurationStore = new DagConfigurationStore())
					{
						dagConfigurationStore.Open();
						PersistentDagNetworkConfig persistentDagNetworkConfig = dagConfigurationStore.LoadNetworkConfig();
						if (persistentDagNetworkConfig != null)
						{
							string text = persistentDagNetworkConfig.Serialize();
							ReplayEventLogConstants.Tuple_DagNetworkConfigOld.LogEvent(DateTime.UtcNow.ToString(), new object[]
							{
								text
							});
						}
					}
					ExchangeNetworkMap exchangeNetworkMap = NetworkManager.FetchInitializedMap();
					PersistentDagNetworkConfig netConfig = exchangeNetworkMap.UpdateNetConfig(changeReq);
					manager.UpdateNetworkConfig(netConfig);
				}
			});
		}

		internal static void RemoveDagNetwork(RemoveDagNetworkRequest req)
		{
			NetworkManager.RunRpcOperation("RemoveDagNetwork", delegate(object param0, EventArgs param1)
			{
				NetworkManager manager = NetworkManager.GetManager();
				if (manager == null)
				{
					throw new DagNetworkManagementException(ReplayStrings.NetworkManagerInitError);
				}
				lock (manager.m_mapRefreshLock)
				{
					using (DagConfigurationStore dagConfigurationStore = new DagConfigurationStore())
					{
						dagConfigurationStore.Open();
						PersistentDagNetworkConfig persistentDagNetworkConfig = dagConfigurationStore.LoadNetworkConfig();
						if (persistentDagNetworkConfig != null)
						{
							string text = persistentDagNetworkConfig.Serialize();
							ReplayEventLogConstants.Tuple_DagNetworkConfigOld.LogEvent(DateTime.UtcNow.ToString(), new object[]
							{
								text
							});
						}
					}
					ExchangeNetworkMap exchangeNetworkMap = NetworkManager.FetchInitializedMap();
					PersistentDagNetworkConfig persistentDagNetworkConfig2 = exchangeNetworkMap.BuildPersistentDagNetworkConfig();
					if (!persistentDagNetworkConfig2.RemoveNetwork(req.Name))
					{
						NetworkManager.TraceError("RemoveDagNetwork {0} not found", new object[]
						{
							req.Name
						});
						throw new DagNetworkManagementException(ReplayStrings.NetworkNameNotFound(req.Name));
					}
					manager.UpdateNetworkConfig(persistentDagNetworkConfig2);
				}
			});
		}

		protected static void RunRpcOperation(string rpcName, EventHandler ev)
		{
			Exception ex = null;
			NetworkManager.TraceDebug("RunRpcOperation({0})", new object[]
			{
				rpcName
			});
			try
			{
				ev(null, null);
				return;
			}
			catch (ClusterNetworkDeletedException ex2)
			{
				ex = ex2;
			}
			catch (ClusterException ex3)
			{
				ex = ex3;
			}
			catch (DagNetworkManagementException ex4)
			{
				ex = ex4;
			}
			catch (COMException ex5)
			{
				ex = ex5;
			}
			catch (IOException ex6)
			{
				ex = ex6;
			}
			catch (UnauthorizedAccessException ex7)
			{
				ex = ex7;
			}
			catch (TransientException ex8)
			{
				ex = ex8;
			}
			catch (Win32Exception ex9)
			{
				ex = ex9;
			}
			if (ex != null)
			{
				NetworkManager.TraceError("RunRpcOperation({0}) hit exception {1}", new object[]
				{
					rpcName,
					ex
				});
				throw new DagNetworkRpcServerException(rpcName, ex.Message, ex);
			}
		}

		private static ExchangeNetworkMap FetchInitializedMap()
		{
			ExchangeNetworkMap map = NetworkManager.GetMap();
			if (map == null)
			{
				NetworkManager.TraceError("NetworkMap has not yet been initialized. Sleeping for {0}ms", new object[]
				{
					NetworkManager.GetInitializationTimeoutInMsec()
				});
				Thread.Sleep(NetworkManager.GetInitializationTimeoutInMsec());
				map = NetworkManager.GetMap();
				if (map == null)
				{
					throw new DagNetworkManagementException(ReplayStrings.NetworkManagerInitError);
				}
			}
			return map;
		}

		private static int GetInitializationTimeoutInMsec()
		{
			return 1000 * RegistryParameters.NetworkManagerStartupTimeoutInSec;
		}

		private bool IsPAM()
		{
			AmConfig config = AmSystemManager.Instance.Config;
			if (!config.IsPAM && !config.IsSAM)
			{
				NetworkManager.TraceDebug("NetworkManager startup skipped.  Not running in DAG role", new object[0]);
				return false;
			}
			return config.IsPAM;
		}

		private void Initialize()
		{
			TcpPortFallback.LoadPortNumber(out this.m_replicationPort);
			this.m_threadClusterNotification = new Thread(new ThreadStart(this.ClusterNotificationThread));
			this.m_threadClusterNotification.Start();
		}

		private void EnumerateNetworkMap()
		{
			NetworkManager.TraceDebug("EnumerateNetworkMap: attempting to reload", new object[0]);
			using (IAmCluster amCluster = ClusterFactory.Instance.Open())
			{
				PersistentDagNetworkConfig persistentDagNetworkConfig = null;
				string text = null;
				Exception ex = null;
				using (DagConfigurationStore dagConfigurationStore = new DagConfigurationStore())
				{
					dagConfigurationStore.Open();
					persistentDagNetworkConfig = dagConfigurationStore.LoadNetworkConfig(out text);
					PersistentDagNetworkConfig persistentDagNetworkConfig2;
					if (persistentDagNetworkConfig == null)
					{
						persistentDagNetworkConfig2 = new PersistentDagNetworkConfig();
					}
					else
					{
						persistentDagNetworkConfig2 = persistentDagNetworkConfig.Copy();
					}
					IADDatabaseAvailabilityGroup localDag = Dependencies.ADConfig.GetLocalDag();
					if (localDag == null)
					{
						NetworkManager.TraceError("EnumerateNetworkMap can't get the DAG!", new object[0]);
					}
					else
					{
						if (persistentDagNetworkConfig2.NetworkCompression != localDag.NetworkCompression)
						{
							persistentDagNetworkConfig2.NetworkCompression = localDag.NetworkCompression;
						}
						if (persistentDagNetworkConfig2.NetworkEncryption != localDag.NetworkEncryption)
						{
							persistentDagNetworkConfig2.NetworkEncryption = localDag.NetworkEncryption;
						}
						if (persistentDagNetworkConfig2.ManualDagNetworkConfiguration != localDag.ManualDagNetworkConfiguration)
						{
							persistentDagNetworkConfig2.ManualDagNetworkConfiguration = localDag.ManualDagNetworkConfiguration;
						}
					}
					this.NetworkCompression = persistentDagNetworkConfig2.NetworkCompression;
					this.NetworkEncryption = persistentDagNetworkConfig2.NetworkEncryption;
					this.ReplicationPort = persistentDagNetworkConfig2.ReplicationPort;
					this.ManualDagNetworkConfiguration = persistentDagNetworkConfig2.ManualDagNetworkConfiguration;
					if (this.m_portInLocalRegistry != this.ReplicationPort && TcpPortFallback.StorePortNumber(this.ReplicationPort))
					{
						this.m_portInLocalRegistry = this.ReplicationPort;
					}
					NetworkDiscovery networkDiscovery = new NetworkDiscovery();
					networkDiscovery.LoadClusterObjects(amCluster);
					if (this.ManualDagNetworkConfiguration)
					{
						networkDiscovery.LoadExistingConfiguration(persistentDagNetworkConfig2);
					}
					networkDiscovery.DetermineDnsStatus();
					networkDiscovery.AggregateNetworks(true);
					if (!this.ManualDagNetworkConfiguration)
					{
						networkDiscovery.RemoveEmptyNets();
					}
					ExchangeNetworkMap exchangeNetworkMap = new ExchangeNetworkMap(this);
					exchangeNetworkMap.Load(networkDiscovery);
					AmConfig config = AmSystemManager.Instance.Config;
					if (config.IsPAM)
					{
						try
						{
							exchangeNetworkMap.SynchronizeClusterNetworkRoles(amCluster);
						}
						catch (ClusCommonFailException ex2)
						{
							NetworkManager.TraceError("SynchronizeClusterNetworkRoles threw: {0}", new object[]
							{
								ex2
							});
							ex = ex2;
						}
					}
					exchangeNetworkMap.SetupPerfmon();
					persistentDagNetworkConfig2 = exchangeNetworkMap.BuildPersistentDagNetworkConfig();
					string text2 = persistentDagNetworkConfig2.Serialize();
					bool flag = false;
					if (config.IsPAM)
					{
						if (persistentDagNetworkConfig == null || text != text2)
						{
							flag = true;
							Interlocked.Exchange(ref this.m_skipNextClusterRegistryEvent, 1);
							dagConfigurationStore.StoreNetworkConfig(text2);
							if (persistentDagNetworkConfig != null)
							{
								ReplayEventLogConstants.Tuple_DagNetworkConfigOld.LogEvent("DAGNET", new object[]
								{
									text
								});
							}
						}
					}
					else if (this.m_lastWrittenClusterNetConfigXML != null && this.m_lastWrittenClusterNetConfigXML != text2)
					{
						flag = true;
					}
					if (flag)
					{
						ReplayEventLogConstants.Tuple_DagNetworkConfigNew.LogEvent("DAGNET", new object[]
						{
							text2
						});
					}
					this.m_lastWrittenClusterNetConfigXML = text2;
					DagNetConfig dagNetConfig = this.Convert2DagNetConfig(networkDiscovery);
					string text3 = dagNetConfig.Serialize();
					if (this.m_lastWrittenEseReplNetConfigXML == null || this.EseReplDagNetConfigIsStale || text3 != this.m_lastWrittenEseReplNetConfigXML)
					{
						DagNetEnvironment.PublishDagNetConfig(text3);
						this.EseReplDagNetConfigIsStale = false;
					}
					this.m_lastWrittenEseReplNetConfigXML = text3;
					this.m_mapLoadTime = ExDateTime.Now;
					this.m_netMap = exchangeNetworkMap;
					NetworkManager.TraceDebug("EnumerateNetworkMap: completed reload", new object[0]);
					AmSystemManager instance = AmSystemManager.Instance;
					if (instance != null)
					{
						AmNetworkMonitor networkMonitor = instance.NetworkMonitor;
						if (networkMonitor != null)
						{
							networkMonitor.RefreshMapiNetwork();
						}
					}
					if (ex != null)
					{
						throw ex;
					}
				}
			}
		}

		private DagNetConfig Convert2DagNetConfig(NetworkDiscovery map)
		{
			DagNetConfig dagNetConfig = new DagNetConfig();
			dagNetConfig.ReplicationPort = (int)this.ReplicationPort;
			dagNetConfig.NetworkCompression = NetworkManager.MapESEReplNetworkOption(this.NetworkCompression);
			dagNetConfig.NetworkEncryption = NetworkManager.MapESEReplNetworkOption(this.NetworkEncryption);
			foreach (LogicalNetwork logicalNetwork in map.LogicalNetworks)
			{
				DagNetwork dagNetwork = new DagNetwork();
				dagNetwork.Name = logicalNetwork.Name;
				dagNetwork.Description = logicalNetwork.Description;
				dagNetwork.ReplicationEnabled = logicalNetwork.ReplicationEnabled;
				dagNetwork.IsDnsMapped = logicalNetwork.HasDnsNic();
				foreach (Subnet subnet in logicalNetwork.Subnets)
				{
					dagNetwork.Subnets.Add(subnet.SubnetId.ToString());
				}
				dagNetConfig.Networks.Add(dagNetwork);
			}
			foreach (ClusterNode clusterNode in map.Nodes)
			{
				DagNode dagNode = new DagNode();
				dagNode.Name = clusterNode.Name.NetbiosName;
				foreach (ClusterNic clusterNic in clusterNode.Nics)
				{
					if (clusterNic.IPAddress != null)
					{
						DagNode.Nic nic = new DagNode.Nic();
						nic.IpAddress = clusterNic.IPAddress.ToString();
						nic.NetworkName = clusterNic.ClusterNetwork.LogicalNetwork.Name;
						dagNode.Nics.Add(nic);
					}
				}
				dagNetConfig.Nodes.Add(dagNode);
			}
			return dagNetConfig;
		}

		private void UpdateNetworkConfig(PersistentDagNetworkConfig netConfig)
		{
			using (DagConfigurationStore dagConfigurationStore = new DagConfigurationStore())
			{
				dagConfigurationStore.Open();
				Interlocked.Exchange(ref this.m_skipNextClusterRegistryEvent, 1);
				string text = dagConfigurationStore.StoreNetworkConfig(netConfig);
				this.m_lastWrittenClusterNetConfigXML = text;
				ReplayEventLogConstants.Tuple_DagNetworkConfigNew.LogEvent(DateTime.UtcNow.ToString(), new object[]
				{
					text
				});
				this.EseReplDagNetConfigIsStale = true;
				this.DriveMapRefresh();
			}
		}

		private void DriveMapRefresh()
		{
			try
			{
				this.DriveMapRefreshInternal();
			}
			finally
			{
				this.m_firstDriveMapRefreshCompleted.Set();
			}
		}

		private void DriveMapRefreshInternal()
		{
			lock (this.m_mapRefreshLock)
			{
				bool flag2 = false;
				Exception ex = null;
				int num = 4;
				for (int i = 1; i <= num; i++)
				{
					try
					{
						this.EnumerateNetworkMap();
						flag2 = true;
						break;
					}
					catch (ClusterNetworkDeletedException ex2)
					{
						ex = ex2;
					}
					catch (ClusterException ex3)
					{
						ex = ex3;
					}
					if (i < num)
					{
						NetworkManager.TraceError("DriveMapRefresh hit an exception during EnumerateNetworkMap, sleeping for 1 second and re-trying: {0}", new object[]
						{
							ex
						});
						Thread.Sleep(1000);
					}
				}
				if (!flag2)
				{
					throw ex;
				}
				if (this.m_netMap != null)
				{
					ExchangeNetwork exchangeNetwork = null;
					foreach (KeyValuePair<string, ExchangeNetwork> keyValuePair in this.m_netMap.Networks)
					{
						ExchangeNetwork value = keyValuePair.Value;
						if (value.ReplicationEnabled)
						{
							exchangeNetwork = value;
							break;
						}
					}
					if (exchangeNetwork == null)
					{
						ReplayEventLogConstants.Tuple_NetworkReplicationDisabled.LogEvent("AllNetsDisabled", new object[0]);
					}
				}
			}
		}

		private bool TryDriveMapRefresh()
		{
			Exception ex = null;
			try
			{
				this.DriveMapRefresh();
			}
			catch (ClusterException ex2)
			{
				ex = ex2;
			}
			catch (DataSourceTransientException ex3)
			{
				ex = ex3;
			}
			catch (DataSourceOperationException ex4)
			{
				ex = ex4;
			}
			catch (TransientException ex5)
			{
				ex = ex5;
			}
			catch (Win32Exception ex6)
			{
				ex = ex6;
			}
			catch (SerializationException ex7)
			{
				ex = ex7;
				ReplayCrimsonEvents.GeneralSerializationError.LogPeriodic<string>("NetworkManager", DiagCore.DefaultEventSuppressionInterval, ex7.ToString());
			}
			if (ex != null)
			{
				NetworkManager.TraceError("TryDriveMapRefresh hit exception: {0}", new object[]
				{
					ex
				});
				ReplayEventLogConstants.Tuple_NetworkMonitoringError.LogEvent(ex.Message.GetHashCode().ToString(), new object[]
				{
					ex.ToString()
				});
				return false;
			}
			return true;
		}

		private void ClusterNotificationThread()
		{
			if (this.RefreshClusterHandles())
			{
				this.TryDriveMapRefresh();
			}
			while (!this.m_shutdown)
			{
				Exception ex = null;
				try
				{
					AmConfig config = AmSystemManager.Instance.Config;
					if (!config.IsPAM && !config.IsSAM)
					{
						NetworkManager.TraceDebug("NetworkManager sleeping.  Not running in DAG role", new object[0]);
						Thread.Sleep(NetworkManager.GetInitializationTimeoutInMsec());
					}
					else if (this.RefreshClusterHandles())
					{
						this.MonitorEvents();
					}
					else
					{
						Thread.Sleep(NetworkManager.GetInitializationTimeoutInMsec());
					}
				}
				catch (ClusterException ex2)
				{
					ex = ex2;
				}
				catch (DataSourceTransientException ex3)
				{
					ex = ex3;
				}
				catch (DataSourceOperationException ex4)
				{
					ex = ex4;
				}
				catch (TransientException ex5)
				{
					ex = ex5;
				}
				catch (COMException ex6)
				{
					ex = ex6;
				}
				catch (Win32Exception ex7)
				{
					ex = ex7;
				}
				if (ex != null)
				{
					NetworkManager.TraceError("ClusterNotificationThread monitoring encountered an exception: {0}", new object[]
					{
						ex
					});
					ReplayEventLogConstants.Tuple_NetworkMonitoringError.LogEvent(ex.Message.GetHashCode().ToString(), new object[]
					{
						ex.ToString()
					});
				}
				if (!this.m_shutdown)
				{
					Thread.Sleep(1000);
				}
			}
			NetworkManager.TraceDebug("ClusterNotificationThread exiting", new object[0]);
		}

		private bool TryWaitForInitialization()
		{
			TimeSpan timeout = TimeSpan.FromSeconds((double)RegistryParameters.NetworkManagerStartupTimeoutInSec);
			return this.m_firstDriveMapRefreshCompleted.WaitOne(timeout) == ManualOneShotEvent.Result.Success;
		}

		private void CloseClusterHandles()
		{
			this.m_clusterHandlesAreValid = false;
			if (this.m_hCluster != null && !this.m_hCluster.IsInvalid)
			{
				this.m_hCluster.Dispose();
				this.m_hCluster = null;
			}
		}

		private bool RefreshClusterHandles()
		{
			this.CloseClusterHandles();
			Exception ex = null;
			try
			{
				this.m_hCluster = ClusapiMethods.OpenCluster(null);
				if (this.m_hCluster == null || this.m_hCluster.IsInvalid)
				{
					int lastWin32Error = Marshal.GetLastWin32Error();
					NetworkManager.TraceError("OpenCluster() failed with error {0}.", new object[]
					{
						lastWin32Error
					});
					return false;
				}
				this.m_clusterHandlesAreValid = true;
				return true;
			}
			catch (ClusterException ex2)
			{
				ex = ex2;
			}
			catch (TransientException ex3)
			{
				ex = ex3;
			}
			if (ex != null)
			{
				NetworkManager.TraceError("RefreshClusterHandles() failed with error {0}.", new object[]
				{
					ex
				});
			}
			return false;
		}

		private AmClusterRegkeyHandle GetClusdbKeyHandle(IDistributedStoreKey key)
		{
			DistributedStoreKey distributedStoreKey = key as DistributedStoreKey;
			ClusterDbKey clusterDbKey;
			if (distributedStoreKey != null)
			{
				clusterDbKey = (distributedStoreKey.PrimaryStoreKey as ClusterDbKey);
			}
			else
			{
				clusterDbKey = (key as ClusterDbKey);
			}
			if (clusterDbKey != null)
			{
				return clusterDbKey.KeyHandle;
			}
			return null;
		}

		private void RegisterForChangeNotification(IDistributedStoreKey dsKey, AmClusterNotifyHandle hChange)
		{
			AmClusterRegkeyHandle clusdbKeyHandle = this.GetClusdbKeyHandle(dsKey);
			if (clusdbKeyHandle != null && !clusdbKeyHandle.IsInvalid)
			{
				ClusterNotifyFlags dwFilter = ClusterNotifyFlags.CLUSTER_CHANGE_REGISTRY_VALUE;
				IntPtr dwNotifyKey = (IntPtr)1;
				int num = ClusapiMethods.RegisterClusterNotify(hChange, dwFilter, clusdbKeyHandle, dwNotifyKey);
				if (num != 0)
				{
					NetworkManager.TraceError("RegisterClusterNotify for reg notification 0x{0:X8}", new object[]
					{
						num
					});
					throw AmExceptionHelper.ConstructClusterApiException(num, "RegisterClusterNotify(Network Registry)", new object[0]);
				}
			}
		}

		private void MonitorEvents()
		{
			AmClusterNotifyHandle amClusterNotifyHandle = null;
			IDistributedStoreKey distributedStoreKey = null;
			IDistributedStoreChangeNotify distributedStoreChangeNotify = null;
			try
			{
				ClusterNotifyFlags networkClusterNotificationMask = RegistryParameters.NetworkClusterNotificationMask;
				NetworkManager.TraceDebug("SettingClusterMask as 0x{0:x}", new object[]
				{
					networkClusterNotificationMask
				});
				amClusterNotifyHandle = ClusapiMethods.CreateClusterNotifyPort(AmClusterNotifyHandle.InvalidHandle, this.ClusterHandle, networkClusterNotificationMask, IntPtr.Zero);
				if (amClusterNotifyHandle == null || amClusterNotifyHandle.IsInvalid)
				{
					int lastWin32Error = Marshal.GetLastWin32Error();
					NetworkManager.TraceError("CreateClusterNotifyPort failed. Error code 0x{0:X8}", new object[]
					{
						lastWin32Error
					});
					throw new ClusCommonTransientException("CreateClusterNotifyPort", new Win32Exception(lastWin32Error));
				}
				using (IDistributedStoreKey clusterKey = DistributedStore.Instance.GetClusterKey(this.ClusterHandle, null, null, DxStoreKeyAccessMode.Write, false))
				{
					distributedStoreKey = clusterKey.OpenKey("Exchange\\DagNetwork", DxStoreKeyAccessMode.CreateIfNotExist, false, null);
				}
				this.RegisterForChangeNotification(distributedStoreKey, amClusterNotifyHandle);
				TimeSpan t = new TimeSpan(0, 0, RegistryParameters.NetworkStatusPollingPeriodInSecs);
				while (this.m_clusterHandlesAreValid && !this.m_shutdown)
				{
					StringBuilder stringBuilder = new StringBuilder(256);
					uint num = Convert.ToUInt32(stringBuilder.Capacity);
					IntPtr zero = IntPtr.Zero;
					ClusterNotifyFlags clusterNotifyFlags;
					int clusterNotify = ClusapiMethods.GetClusterNotify(amClusterNotifyHandle, out zero, out clusterNotifyFlags, stringBuilder, ref num, 3000U);
					if (this.m_shutdown)
					{
						break;
					}
					if (this.m_netMap == null)
					{
						if (!this.TryDriveMapRefresh())
						{
							break;
						}
					}
					else if (clusterNotify == 258)
					{
						if (t < ExDateTime.TimeDiff(ExDateTime.Now, this.m_mapLoadTime) && !this.TryDriveMapRefresh())
						{
							break;
						}
					}
					else if (clusterNotify != 0)
					{
						NetworkManager.TraceDebug("GetClusterNotify() returned unexpected status code 0x{0:X)", new object[]
						{
							clusterNotify
						});
					}
					else
					{
						string text = stringBuilder.ToString();
						NetworkManager.TraceDebug("GetClusterNotify() returned notifyKey={0}, filterType=0x{1:x}, resName={2}", new object[]
						{
							zero,
							clusterNotifyFlags,
							text
						});
						if ((clusterNotifyFlags & ~(ClusterNotifyFlags.CLUSTER_CHANGE_NODE_PROPERTY | ClusterNotifyFlags.CLUSTER_CHANGE_REGISTRY_NAME | ClusterNotifyFlags.CLUSTER_CHANGE_REGISTRY_ATTRIBUTES | ClusterNotifyFlags.CLUSTER_CHANGE_REGISTRY_VALUE | ClusterNotifyFlags.CLUSTER_CHANGE_REGISTRY_SUBTREE | ClusterNotifyFlags.CLUSTER_CHANGE_RESOURCE_STATE | ClusterNotifyFlags.CLUSTER_CHANGE_RESOURCE_DELETED | ClusterNotifyFlags.CLUSTER_CHANGE_RESOURCE_ADDED | ClusterNotifyFlags.CLUSTER_CHANGE_RESOURCE_PROPERTY | ClusterNotifyFlags.CLUSTER_CHANGE_GROUP_STATE | ClusterNotifyFlags.CLUSTER_CHANGE_GROUP_DELETED | ClusterNotifyFlags.CLUSTER_CHANGE_GROUP_ADDED | ClusterNotifyFlags.CLUSTER_CHANGE_GROUP_PROPERTY | ClusterNotifyFlags.CLUSTER_CHANGE_RESOURCE_TYPE_DELETED | ClusterNotifyFlags.CLUSTER_CHANGE_RESOURCE_TYPE_ADDED | ClusterNotifyFlags.CLUSTER_CHANGE_RESOURCE_TYPE_PROPERTY | ClusterNotifyFlags.CLUSTER_CHANGE_CLUSTER_RECONNECT | ClusterNotifyFlags.CLUSTER_CHANGE_QUORUM_STATE | ClusterNotifyFlags.CLUSTER_CHANGE_CLUSTER_PROPERTY)) != ~(ClusterNotifyFlags.CLUSTER_CHANGE_NODE_STATE | ClusterNotifyFlags.CLUSTER_CHANGE_NODE_DELETED | ClusterNotifyFlags.CLUSTER_CHANGE_NODE_ADDED | ClusterNotifyFlags.CLUSTER_CHANGE_NODE_PROPERTY | ClusterNotifyFlags.CLUSTER_CHANGE_REGISTRY_NAME | ClusterNotifyFlags.CLUSTER_CHANGE_REGISTRY_ATTRIBUTES | ClusterNotifyFlags.CLUSTER_CHANGE_REGISTRY_VALUE | ClusterNotifyFlags.CLUSTER_CHANGE_REGISTRY_SUBTREE | ClusterNotifyFlags.CLUSTER_CHANGE_RESOURCE_STATE | ClusterNotifyFlags.CLUSTER_CHANGE_RESOURCE_DELETED | ClusterNotifyFlags.CLUSTER_CHANGE_RESOURCE_ADDED | ClusterNotifyFlags.CLUSTER_CHANGE_RESOURCE_PROPERTY | ClusterNotifyFlags.CLUSTER_CHANGE_GROUP_STATE | ClusterNotifyFlags.CLUSTER_CHANGE_GROUP_DELETED | ClusterNotifyFlags.CLUSTER_CHANGE_GROUP_ADDED | ClusterNotifyFlags.CLUSTER_CHANGE_GROUP_PROPERTY | ClusterNotifyFlags.CLUSTER_CHANGE_RESOURCE_TYPE_DELETED | ClusterNotifyFlags.CLUSTER_CHANGE_RESOURCE_TYPE_ADDED | ClusterNotifyFlags.CLUSTER_CHANGE_RESOURCE_TYPE_PROPERTY | ClusterNotifyFlags.CLUSTER_CHANGE_CLUSTER_RECONNECT | ClusterNotifyFlags.CLUSTER_CHANGE_NETWORK_STATE | ClusterNotifyFlags.CLUSTER_CHANGE_NETWORK_DELETED | ClusterNotifyFlags.CLUSTER_CHANGE_NETWORK_ADDED | ClusterNotifyFlags.CLUSTER_CHANGE_NETWORK_PROPERTY | ClusterNotifyFlags.CLUSTER_CHANGE_NETINTERFACE_STATE | ClusterNotifyFlags.CLUSTER_CHANGE_NETINTERFACE_DELETED | ClusterNotifyFlags.CLUSTER_CHANGE_NETINTERFACE_ADDED | ClusterNotifyFlags.CLUSTER_CHANGE_NETINTERFACE_PROPERTY | ClusterNotifyFlags.CLUSTER_CHANGE_QUORUM_STATE | ClusterNotifyFlags.CLUSTER_CHANGE_CLUSTER_STATE | ClusterNotifyFlags.CLUSTER_CHANGE_CLUSTER_PROPERTY | ClusterNotifyFlags.CLUSTER_CHANGE_HANDLE_CLOSE) && !this.TryDriveMapRefresh())
						{
							break;
						}
					}
				}
			}
			finally
			{
				if (amClusterNotifyHandle != null)
				{
					amClusterNotifyHandle.Dispose();
					amClusterNotifyHandle = null;
				}
				if (distributedStoreChangeNotify != null)
				{
					distributedStoreChangeNotify.Dispose();
				}
				if (distributedStoreKey != null)
				{
					distributedStoreKey.Dispose();
				}
			}
		}

		private const int SleepTransientException = 1000;

		private const uint ClusterNotifyTimeoutMilliseconds = 3000U;

		private const string FirstDriveMapRefreshCompletedEventName = "FirstDriveMapRefreshCompletedEvent";

		private static object s_mgrLock = new object();

		private static NetworkManager s_mgr = new NetworkManager();

		private static bool s_initialized = false;

		private ManualOneShotEvent m_firstDriveMapRefreshCompleted = new ManualOneShotEvent("FirstDriveMapRefreshCompletedEvent");

		private ExchangeNetworkMap m_netMap;

		private object m_mapRefreshLock = new object();

		private ExDateTime m_mapLoadTime;

		private AmClusterHandle m_hCluster;

		private bool m_clusterHandlesAreValid;

		private bool m_shutdown;

		private Thread m_threadClusterNotification;

		private int m_skipNextClusterRegistryEvent;

		private ushort m_replicationPort = 64327;

		private ushort m_portInLocalRegistry;

		private DatabaseAvailabilityGroup.NetworkOption m_networkCompression = DatabaseAvailabilityGroup.NetworkOption.InterSubnetOnly;

		private DatabaseAvailabilityGroup.NetworkOption m_networkEncryption = DatabaseAvailabilityGroup.NetworkOption.InterSubnetOnly;

		private string m_lastWrittenClusterNetConfigXML;

		private string m_lastWrittenEseReplNetConfigXML;
	}
}
