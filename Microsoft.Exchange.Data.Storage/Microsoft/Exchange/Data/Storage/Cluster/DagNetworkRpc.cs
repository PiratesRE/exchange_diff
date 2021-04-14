using System;
using System.Linq;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Data.Serialization;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.Rpc.ActiveManager;
using Microsoft.Exchange.Rpc.Cluster;

namespace Microsoft.Exchange.Data.Storage.Cluster
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class DagNetworkRpc
	{
		public static DagNetworkConfiguration GetDagNetworkConfig(string serverName)
		{
			byte[] configAsBytes = null;
			DagNetworkRpc.RunRpcOperation(serverName, delegate(ReplayRpcClient rpcClient)
			{
				ExTraceGlobals.DatabaseAvailabilityGroupTracer.TraceDebug<string>(0L, "GetDagNetworkConfig sending RPC to {0}", serverName);
				return rpcClient.GetDagNetworkConfig(ref configAsBytes);
			});
			return (DagNetworkConfiguration)Serialization.BytesToObject(configAsBytes);
		}

		public static DagNetworkConfiguration GetDagNetworkConfig(DatabaseAvailabilityGroup dag)
		{
			DagNetworkConfiguration dagNetworkConfiguration = null;
			if (dag.Servers.Count == 0)
			{
				return null;
			}
			Exception ex = null;
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 85, "GetDagNetworkConfig", "f:\\15.00.1497\\sources\\dev\\data\\src\\storage\\Cluster\\DagNetworkClientRpc.cs");
			topologyConfigurationSession.UseConfigNC = false;
			topologyConfigurationSession.UseGlobalCatalog = true;
			foreach (ADObjectId adobjectId in dag.Servers)
			{
				try
				{
					ADComputer adcomputer = topologyConfigurationSession.FindComputerByHostName(adobjectId.Name);
					if (adcomputer != null && !string.IsNullOrEmpty(adcomputer.DnsHostName))
					{
						if (dag.DatacenterActivationMode != DatacenterActivationModeOption.DagOnly || !dag.StoppedMailboxServers.Contains(adcomputer.DnsHostName, StringComparer.OrdinalIgnoreCase))
						{
							dagNetworkConfiguration = DagNetworkRpc.GetDagNetworkConfig(adcomputer.DnsHostName);
							if (dagNetworkConfiguration != null)
							{
								break;
							}
						}
					}
				}
				catch (TaskServerTransientException ex2)
				{
					ex = ex2;
				}
				catch (TaskServerException ex3)
				{
					ex = ex3;
				}
				catch (ADTransientException ex4)
				{
					ex = ex4;
				}
				catch (ADExternalException ex5)
				{
					ex = ex5;
				}
				catch (ADOperationException ex6)
				{
					ex = ex6;
				}
			}
			if (dagNetworkConfiguration == null && ex != null)
			{
				throw ex;
			}
			return dagNetworkConfiguration;
		}

		public static void SetDagNetwork(DatabaseAvailabilityGroup dag, SetDagNetworkRequest change)
		{
			if (dag.Servers.Count == 0)
			{
				return;
			}
			AmPamInfo primaryActiveManager = AmRpcClientHelper.GetPrimaryActiveManager(ADObjectWrapperFactory.CreateWrapper(dag));
			string targetServerName = primaryActiveManager.ServerName;
			byte[] changeAsBytes = Serialization.ObjectToBytes(change);
			DagNetworkRpc.RunRpcOperation(targetServerName, delegate(ReplayRpcClient rpcClient)
			{
				ExTraceGlobals.DatabaseAvailabilityGroupTracer.TraceDebug<string>(0L, "SetDagNetwork sending RPC to {0}", targetServerName);
				return rpcClient.SetDagNetwork(changeAsBytes);
			});
		}

		public static void RemoveDagNetwork(DatabaseAvailabilityGroup dag, RemoveDagNetworkRequest change)
		{
			if (dag.Servers.Count == 0)
			{
				return;
			}
			AmPamInfo primaryActiveManager = AmRpcClientHelper.GetPrimaryActiveManager(ADObjectWrapperFactory.CreateWrapper(dag));
			string targetServerName = primaryActiveManager.ServerName;
			byte[] changeAsBytes = Serialization.ObjectToBytes(change);
			DagNetworkRpc.RunRpcOperation(targetServerName, delegate(ReplayRpcClient rpcClient)
			{
				ExTraceGlobals.DatabaseAvailabilityGroupTracer.TraceDebug<string>(0L, "RemoveDagNetwork sending RPC to {0}", targetServerName);
				return rpcClient.RemoveDagNetwork(changeAsBytes);
			});
		}

		public static void SetDagNetworkConfig(DatabaseAvailabilityGroup dag, SetDagNetworkConfigRequest change)
		{
			if (dag.Servers.Count == 0)
			{
				return;
			}
			AmPamInfo primaryActiveManager = AmRpcClientHelper.GetPrimaryActiveManager(ADObjectWrapperFactory.CreateWrapper(dag));
			string targetServerName = primaryActiveManager.ServerName;
			byte[] changeAsBytes = Serialization.ObjectToBytes(change);
			DagNetworkRpc.RunRpcOperation(targetServerName, delegate(ReplayRpcClient rpcClient)
			{
				ExTraceGlobals.DatabaseAvailabilityGroupTracer.TraceDebug<string>(0L, "SetDagNetworkConfig sending RPC to {0}", targetServerName);
				return rpcClient.SetDagNetworkConfig(changeAsBytes);
			});
		}

		private static void RunRpcOperation(string serverName, DagNetworkRpc.InternalRpcOperation rpcOperation)
		{
			DagNetworkRpc.RunRpcOperation(serverName, null, rpcOperation);
		}

		private static void RunRpcOperation(string serverName, int? timeoutMs, DagNetworkRpc.InternalRpcOperation rpcOperation)
		{
			RpcErrorExceptionInfo errorInfo = null;
			TasksRpcExceptionWrapper.Instance.ClientRetryableOperation(serverName, delegate
			{
				using (ReplayRpcClient replayRpcClient = DagNetworkRpc.RpcClientFactory(serverName, timeoutMs))
				{
					errorInfo = rpcOperation(replayRpcClient);
				}
			});
			TasksRpcExceptionWrapper.Instance.ClientRethrowIfFailed(serverName, errorInfo);
		}

		private static ReplayRpcClient RpcClientFactory(string serverName)
		{
			return DagNetworkRpc.RpcClientFactory(serverName, null);
		}

		private static ReplayRpcClient RpcClientFactory(string serverName, int? timeoutMs)
		{
			ReplayRpcClient replayRpcClient = new ReplayRpcClient(serverName);
			if (timeoutMs != null)
			{
				replayRpcClient.SetTimeOut(timeoutMs.Value);
			}
			return replayRpcClient;
		}

		private const int ShortRpcTimeoutMs = 10000;

		private delegate RpcErrorExceptionInfo InternalRpcOperation(ReplayRpcClient rpcClient);
	}
}
