using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Exchange.Cluster.ActiveManagerServer;
using Microsoft.Exchange.Cluster.ClusApi;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.HA;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Data.Storage.Cluster;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal static class DagHelper
	{
		internal static void CreateFileShareWitnessQuorum(ITaskOutputHelper output, IAmCluster cluster, string fswShare)
		{
			output = (output ?? NullTaskOutputHelper.GetNullLogger());
			IAmClusterGroup amClusterGroup = cluster.FindCoreClusterGroup();
			if (amClusterGroup == null)
			{
				Thread.Sleep(10000);
				amClusterGroup = cluster.FindCoreClusterGroup();
				if (amClusterGroup == null)
				{
					throw new FailedToGetClusterCoreGroupException();
				}
			}
			using (amClusterGroup)
			{
				IEnumerable<AmClusterResource> source = amClusterGroup.EnumerateResourcesOfType("File Share Witness");
				IAmClusterResource amClusterResource = source.ElementAtOrDefault(0);
				try
				{
					bool flag = false;
					if (amClusterResource == null)
					{
						output.AppendLogMessage("CreateFileShareWitnessQuorum: Could not find an existing FSW resource. A new one will be created.", new object[0]);
						flag = true;
					}
					else if (amClusterResource.GetState() == AmResourceState.Failed)
					{
						output.AppendLogMessage("CreateFileShareWitnessQuorum: The existing FSW resource is in a Failed state. It should be deleted and recreated.", new object[0]);
						flag = true;
					}
					else
					{
						string privateProperty = amClusterResource.GetPrivateProperty<string>("SharePath");
						if (!SharedHelper.StringIEquals(privateProperty, fswShare))
						{
							output.AppendLogMessage("CreateFileShareWitnessQuorum: There is already a FSW, but the current share path ({0}) is not what's desired ({1}). Will try to fix it.", new object[]
							{
								privateProperty,
								fswShare
							});
							List<string> list = new List<string>(4);
							foreach (IAmClusterNode amClusterNode in cluster.EnumerateNodes())
							{
								using (amClusterNode)
								{
									if (!AmClusterNode.IsNodeUp(amClusterNode.State))
									{
										list.Add(amClusterNode.Name.NetbiosName);
									}
								}
							}
							if (list.Count > 0)
							{
								output.WriteErrorSimple(new DagTaskSetDagNeedsAllNodesUpToChangeQuorumException(string.Join(",", list.ToArray())));
							}
							DagHelper.SetFswSharePath(output, cluster, amClusterResource, fswShare);
						}
					}
					AmResourceState state;
					if (!flag && amClusterResource != null)
					{
						try
						{
							state = amClusterResource.GetState();
							if (state != AmResourceState.Online)
							{
								output.AppendLogMessage("The FSW is not online (it is {0}). Attempting to bring online.", new object[]
								{
									state
								});
								amClusterResource.OnlineResource();
							}
							state = amClusterResource.GetState();
							output.AppendLogMessage("The fsw resource is now in state {0}.", new object[]
							{
								state
							});
							if (state != AmResourceState.Online)
							{
								flag = true;
							}
						}
						catch (ClusterException ex)
						{
							output.AppendLogMessage("Bringing the FSW resource online failed, so it will be deleted and recreated. For the record, the error was {0}", new object[]
							{
								ex
							});
							flag = true;
						}
					}
					if (flag)
					{
						if (amClusterResource != null)
						{
							amClusterResource.Dispose();
							amClusterResource = null;
						}
						output.AppendLogMessage("CreateFileShareWitnessQuorum: Calling RevertToMnsQuorum to clean everything up first.", new object[0]);
						DagHelper.RevertToMnsQuorum(output, cluster);
						string text = string.Format("File Share Witness ({0})", fswShare);
						output.AppendLogMessage("Creating a new file share witness resource named '{0}'.", new object[]
						{
							text
						});
						amClusterResource = amClusterGroup.CreateResource(text, "File Share Witness");
						DagHelper.SetFswSharePath(output, cluster, amClusterResource, fswShare);
					}
					output.AppendLogMessage("The FSW resource is now in state {0}.", new object[]
					{
						amClusterResource.GetState()
					});
					string text2;
					uint maxLogSize;
					string quorumResourceInformation = cluster.GetQuorumResourceInformation(out text2, out maxLogSize);
					output.AppendLogMessage("The current quorum resource is '{0}'. About to set it to the FSW.", new object[]
					{
						quorumResourceInformation
					});
					state = amClusterResource.GetState();
					if (state != AmResourceState.Online)
					{
						output.WriteErrorSimple(new DagTaskFileShareWitnessResourceIsStillNotOnlineException(fswShare, state.ToString()));
					}
					cluster.SetQuorumResource(amClusterResource, null, maxLogSize);
					quorumResourceInformation = cluster.GetQuorumResourceInformation(out text2, out maxLogSize);
					output.AppendLogMessage("The quorum resource is now '{0}'.", new object[]
					{
						quorumResourceInformation
					});
					output.AppendLogMessage("Bringing the quorum resource online...", new object[0]);
					amClusterResource.OnlineResource();
				}
				finally
				{
					if (amClusterResource != null)
					{
						amClusterResource.Dispose();
						amClusterResource = null;
					}
				}
			}
		}

		private static void SetFswSharePath(ITaskOutputHelper output, IAmCluster cluster, IAmClusterResource fsw, string fswShare)
		{
			try
			{
				fsw.OfflineResource();
				fsw.SetPrivateProperty<string>("SharePath", fswShare);
				fsw.OnlineResource();
			}
			catch (ClusterException ex)
			{
				Win32Exception ex2 = null;
				if (ex.TryGetTypedInnerException(out ex2))
				{
					output.AppendLogMessage("SetFswSharePath() caught an AmClusterApiException with errorcode={0} and NativeErrorCode={1}. ex = {2}", new object[]
					{
						ex2.ErrorCode,
						ex2.NativeErrorCode,
						ex2
					});
					if (ex2.NativeErrorCode == 5)
					{
						string text = cluster.CnoName;
						if (text == string.Empty)
						{
							using (IAmClusterGroup amClusterGroup = cluster.FindCoreClusterGroup())
							{
								if (amClusterGroup != null && amClusterGroup.OwnerNode != null)
								{
									text = amClusterGroup.OwnerNode.NetbiosName;
								}
							}
						}
						output.WriteErrorSimple(new DagTaskFswNeedsCnoPermissionException(fswShare, text));
					}
				}
				throw;
			}
		}

		internal static void RevertToMnsQuorum(ILogTraceHelper output, IAmCluster cluster)
		{
			using (IAmClusterGroup amClusterGroup = cluster.FindCoreClusterGroup())
			{
				string text;
				uint maxLogSize;
				string quorumResourceInformation = cluster.GetQuorumResourceInformation(out text, out maxLogSize);
				if (string.IsNullOrEmpty(quorumResourceInformation))
				{
					output.AppendLogMessage("RevertToMnsQuorum: It's already using MNS!", new object[0]);
					using (IAmClusterResource amClusterResource = amClusterGroup.FindResourceByTypeName("File Share Witness"))
					{
						if (amClusterResource != null)
						{
							output.AppendLogMessage("Even though the quorum is set to MNS, there is a FSW resource present named '{0}', which will be deleted shortly.", new object[]
							{
								amClusterResource.Name
							});
							amClusterResource.DeleteResource();
							output.AppendLogMessage("The resource has been deleted!", new object[0]);
						}
						goto IL_13D;
					}
				}
				using (AmClusterResource amClusterResource2 = cluster.OpenResource(quorumResourceInformation))
				{
					output.AppendLogMessage("Setting cluster quorum to MNS", new object[0]);
					if (cluster.CnoName == string.Empty)
					{
						cluster.ClearQuorumResource();
					}
					else
					{
						using (IAmClusterResource amClusterResource3 = amClusterGroup.FindResourceByTypeName("Network Name"))
						{
							output.AppendLogMessage("Setting cluster quorum resource to the netname resource (i.e. MNS quorum).", new object[0]);
							cluster.SetQuorumResource(amClusterResource3, null, maxLogSize);
							if (amClusterResource2 != null && amClusterResource2.GetTypeName() == "File Share Witness")
							{
								output.AppendLogMessage("Offlining and deleting the old FSW resource '{0}'.", new object[]
								{
									quorumResourceInformation
								});
								amClusterResource2.OfflineResource();
								amClusterResource2.DeleteResource();
							}
						}
					}
				}
				IL_13D:;
			}
		}

		internal static bool IsQuorumTypeFileShareWitness(IAmCluster cluster)
		{
			using (AmClusterResource amClusterResource = cluster.OpenQuorumResource())
			{
				if (amClusterResource != null && amClusterResource.GetTypeName() == "File Share Witness")
				{
					return true;
				}
			}
			return false;
		}

		public static bool IsLocalNodeClustered()
		{
			return DagHelper.IsNodeClustered(AmServerName.LocalComputerName);
		}

		public static bool IsNodeClustered(AmServerName serverName)
		{
			string nodeName = serverName.Fqdn;
			if (serverName.IsLocalComputerName)
			{
				nodeName = null;
			}
			AmNodeClusterState dwClusStatus = AmNodeClusterState.NotInstalled;
			int dwError = -1;
			try
			{
				Action invokableAction = delegate()
				{
					dwError = ClusapiMethods.GetNodeClusterState(nodeName, ref dwClusStatus);
				};
				InvokeWithTimeout.Invoke(invokableAction, TimeSpan.FromSeconds((double)RegistryParameters.RemoteClusterCallTimeoutInSec));
			}
			catch (TimeoutException)
			{
				dwError = 1460;
			}
			if (dwError != 0)
			{
				throw new ExClusTransientException("IsNodeClustered", new Win32Exception(dwError));
			}
			return dwClusStatus == AmNodeClusterState.Running || dwClusStatus == AmNodeClusterState.NotRunning;
		}

		public static IEnumerable<AmServerName> EnumeratePausedNodes(IAmCluster cluster)
		{
			IList<AmServerName> list = new List<AmServerName>(8);
			foreach (IAmClusterNode amClusterNode in cluster.EnumerateNodes())
			{
				using (amClusterNode)
				{
					if (amClusterNode.State == AmNodeState.Paused)
					{
						list.Add(amClusterNode.Name);
					}
				}
			}
			return list;
		}

		internal static IAmCluster CreateDagCluster(string clusterName, AmServerName firstNodeName, string[] ipAddresses, uint[] netmasks, out string verboseLog)
		{
			IAmCluster result = null;
			HaTaskStringBuilderOutputHelper haTaskStringBuilderOutputHelper = new HaTaskStringBuilderOutputHelper("m.e.cluster.replay.dll!CreateDagCluster");
			GCHandle value = GCHandle.Alloc(haTaskStringBuilderOutputHelper);
			Exception ex = null;
			try
			{
				haTaskStringBuilderOutputHelper.WriteProgressSimple(ReplayStrings.DagTaskFormingClusterProgress(clusterName, firstNodeName.NetbiosName));
				haTaskStringBuilderOutputHelper.LastException = null;
				haTaskStringBuilderOutputHelper.MaxPercentageDuringCallback = 0;
				result = ClusterFactory.Instance.CreateExchangeCluster(clusterName, firstNodeName, ipAddresses, netmasks, haTaskStringBuilderOutputHelper, GCHandle.ToIntPtr(value), out ex, true);
				if (ex != null)
				{
					haTaskStringBuilderOutputHelper.WriteErrorSimple(ex);
				}
			}
			catch (LocalizedException exception)
			{
				DagHelper.ThrowDagTaskOperationWrapper(exception);
			}
			finally
			{
				verboseLog = haTaskStringBuilderOutputHelper.ToString();
				value.Free();
			}
			return result;
		}

		internal static void FollowBestPractices(ITaskOutputHelper output, IAmCluster cluster)
		{
			if (string.Empty != cluster.CnoName)
			{
				using (IAmClusterGroup amClusterGroup = cluster.FindCoreClusterGroup())
				{
					using (IAmClusterResource amClusterResource = amClusterGroup.FindResourceByTypeName("Network Name"))
					{
						output.AppendLogMessage("Setting the DNS TTL to 300", new object[0]);
						amClusterResource.SetPrivateProperty<int>("HostRecordTTL", 300);
					}
				}
			}
		}

		internal static void DestroyDagCluster(string clusterName, out string verboseLog)
		{
			HaTaskStringBuilderOutputHelper haTaskStringBuilderOutputHelper = new HaTaskStringBuilderOutputHelper("m.e.cluster.replay.dll!DestroyDagCluster");
			ExTraceGlobals.ClusterTracer.TraceDebug<string>(0L, "Going to DestroyCluster( {0} ) on this machine.", clusterName);
			GCHandle value = GCHandle.Alloc(haTaskStringBuilderOutputHelper);
			Exception ex = null;
			try
			{
				haTaskStringBuilderOutputHelper.LastException = null;
				haTaskStringBuilderOutputHelper.MaxPercentageDuringCallback = 0;
				using (IAmCluster amCluster = ClusterFactory.Instance.Open())
				{
					amCluster.DestroyExchangeCluster(haTaskStringBuilderOutputHelper, GCHandle.ToIntPtr(value), out ex, true);
				}
				if (ex != null)
				{
					haTaskStringBuilderOutputHelper.WriteErrorSimple(ex);
				}
			}
			catch (LocalizedException exception)
			{
				DagHelper.ThrowDagTaskOperationWrapper(exception);
			}
			finally
			{
				verboseLog = haTaskStringBuilderOutputHelper.ToString();
				value.Free();
			}
		}

		internal static void InstallFailoverClustering(out string verboseLog)
		{
			HaTaskStringBuilderOutputHelper haTaskStringBuilderOutputHelper = new HaTaskStringBuilderOutputHelper("m.e.cluster.replay.dll!InstallFailoverClustering");
			ExTraceGlobals.ClusterTracer.TraceDebug(0L, "Going to InstallFailoverClustering() on this machine.");
			haTaskStringBuilderOutputHelper.WriteProgressSimple(ReplayStrings.DagTaskInstallingFailoverClustering(AmServerName.LocalComputerName.NetbiosName));
			try
			{
				OsComponentManager osComponentManager = new OsComponentManager(null, haTaskStringBuilderOutputHelper);
				osComponentManager.AddWindowsFeature(OsComponent.ComponentRSATClustering);
				osComponentManager.AddWindowsFeature(OsComponent.ComponentFailoverClustering);
			}
			catch (DagTaskServerException ex)
			{
				ExTraceGlobals.ClusterTracer.TraceError<DagTaskServerException>(0L, "InstallFailoverClustering() failed: {0}", ex);
				haTaskStringBuilderOutputHelper.WriteErrorSimple(ex);
				throw ex;
			}
			catch (DagTaskServerTransientException ex2)
			{
				ExTraceGlobals.ClusterTracer.TraceError<DagTaskServerTransientException>(0L, "InstallFailoverClustering() failed: {0}", ex2);
				haTaskStringBuilderOutputHelper.WriteErrorSimple(ex2);
				throw ex2;
			}
			haTaskStringBuilderOutputHelper.WriteProgressSimple(ReplayStrings.DagTaskInstalledFailoverClustering);
			verboseLog = haTaskStringBuilderOutputHelper.ToString();
		}

		internal static void AddDagClusterNode(AmServerName mailboxServerName, out string verboseLog)
		{
			string tmpVerboseLog = string.Empty;
			HaTaskStringBuilderOutputHelper output = new HaTaskStringBuilderOutputHelper("m.e.cluster.replay.dll!AddDagClusterNode");
			DagHelper.nodeActionTracker.PerformNodeAction(output, mailboxServerName, NodeAction.Add, delegate
			{
				DagHelper.AddDagClusterNodeInternal(output, mailboxServerName, out tmpVerboseLog);
			});
			verboseLog = tmpVerboseLog;
		}

		private static void AddDagClusterNodeInternal(HaTaskStringBuilderOutputHelper output, AmServerName mailboxServerName, out string verboseLog)
		{
			try
			{
				Exception ex = null;
				output.AppendLogMessage("Opening a local AmCluster handle.", new object[0]);
				using (IAmCluster amCluster = ClusterFactory.Instance.Open())
				{
					output.WriteProgressSimple(ReplayStrings.DagTaskAddingServerToDag(mailboxServerName.NetbiosName, amCluster.Name));
					GCHandle value = GCHandle.Alloc(output);
					try
					{
						output.LastException = null;
						output.MaxPercentageDuringCallback = 0;
						amCluster.AddNodeToCluster(mailboxServerName, output, GCHandle.ToIntPtr(value), out ex, true);
					}
					finally
					{
						value.Free();
					}
				}
				if (output.LastException != null)
				{
					output.WriteErrorSimple(output.LastException);
				}
				if (ex != null)
				{
					output.WriteErrorSimple(ex);
				}
				output.WriteProgressSimple(ReplayStrings.DagTaskJoinedNodeToCluster(mailboxServerName.NetbiosName));
			}
			catch (LocalizedException exception)
			{
				DagHelper.ThrowDagTaskOperationWrapper(exception);
			}
			finally
			{
				verboseLog = output.ToString();
			}
		}

		internal static bool EvictDagClusterNode(AmServerName convictedNode, out string verboseLog)
		{
			string tmpVerboseLog = string.Empty;
			bool isSuccess = false;
			HaTaskStringBuilderOutputHelper output = new HaTaskStringBuilderOutputHelper("m.e.cluster.replay.dll!EvictDagClusterNode");
			DagHelper.nodeActionTracker.PerformNodeAction(output, convictedNode, NodeAction.Evict, delegate
			{
				isSuccess = DagHelper.EvictDagClusterNodeInternal(output, convictedNode, out tmpVerboseLog);
			});
			verboseLog = tmpVerboseLog;
			return isSuccess;
		}

		private static bool EvictDagClusterNodeInternal(HaTaskStringBuilderOutputHelper output, AmServerName convictedNode, out string verboseLog)
		{
			ExTraceGlobals.ClusterTracer.TraceDebug<AmServerName>(0L, "Going to EvictClusterNode( {0} ) on this machine.", convictedNode);
			try
			{
				using (IAmCluster amCluster = ClusterFactory.Instance.Open())
				{
					amCluster.EvictNodeFromCluster(convictedNode);
				}
			}
			catch (ClusterException ex)
			{
				output.AppendLogMessage("EvictDagClusterNode got exception {0}", new object[]
				{
					ex
				});
				AmClusterException exception = DagHelper.TranslateClusterExceptionForClient(ex);
				DagHelper.ThrowDagTaskOperationWrapper(exception);
			}
			catch (LocalizedException ex2)
			{
				output.AppendLogMessage("EvictDagClusterNode got exception {0}", new object[]
				{
					ex2
				});
				DagHelper.ThrowDagTaskOperationWrapper(ex2);
			}
			finally
			{
				verboseLog = output.ToString();
			}
			return true;
		}

		private static AmClusterException TranslateClusterExceptionForClient(ClusterException ex)
		{
			ClusterEvictWithoutCleanupException ex2 = ex as ClusterEvictWithoutCleanupException;
			if (ex2 != null)
			{
				return new AmClusterEvictWithoutCleanupException(ex2.NodeName);
			}
			ClusterNodeNotFoundException ex3 = ex as ClusterNodeNotFoundException;
			if (ex3 != null)
			{
				return new AmClusterNodeNotFoundException(ex3.NodeName);
			}
			ClusterNodeJoinedException ex4 = ex as ClusterNodeJoinedException;
			if (ex4 != null)
			{
				return new AmClusterNodeJoinedException(ex4.NodeName);
			}
			ClusterFileNotFoundException ex5 = ex as ClusterFileNotFoundException;
			if (ex5 != null)
			{
				return new AmClusterFileNotFoundException(ex5.NodeName);
			}
			return new AmClusterException(ex.Message);
		}

		internal static void ForceCleanupNode(out string verboseLog)
		{
			ExTraceGlobals.ClusterTracer.TraceDebug(0L, "ForceCleanupNode called");
			HaTaskStringBuilderOutputHelper haTaskStringBuilderOutputHelper = new HaTaskStringBuilderOutputHelper("m.e.cluster.replay.dll!ForceCleanupNode");
			try
			{
				ManagementScope managementScope = new ManagementScope("\\\\.\\root\\MSCluster");
				managementScope.Connect();
				string path = "MSCluster_Cluster";
				using (ManagementClass managementClass = new ManagementClass(managementScope, new ManagementPath(path), null))
				{
					managementClass.Get();
					using (ManagementObjectCollection instances = managementClass.GetInstances())
					{
						if (instances == null)
						{
							throw new ManagementApiException("mgmtClass.GetInstances()");
						}
						foreach (ManagementBaseObject managementBaseObject in instances)
						{
							ManagementObject managementObject = (ManagementObject)managementBaseObject;
							using (managementObject)
							{
								using (ManagementBaseObject methodParameters = managementObject.GetMethodParameters("ForceCleanup"))
								{
									if (methodParameters == null)
									{
										throw new ManagementApiException("clus.GetMethodParameters(\"ForceCleanup\")");
									}
									methodParameters.SetPropertyValue("Timeout", 10000);
									methodParameters.SetPropertyValue("NodeName", Environment.MachineName);
									try
									{
										using (managementObject.InvokeMethod("ForceCleanup", methodParameters, null))
										{
										}
									}
									catch (ManagementException ex)
									{
										ExTraceGlobals.ClusterTracer.TraceDebug<string>(0L, "ForceCleanup throws {0}", ex.Message);
									}
								}
							}
						}
					}
				}
			}
			catch (COMException exception)
			{
				DagHelper.ThrowDagTaskOperationWrapper(exception);
			}
			catch (ManagementApiException exception2)
			{
				DagHelper.ThrowDagTaskOperationWrapper(exception2);
			}
			catch (ManagementException exception3)
			{
				DagHelper.ThrowDagTaskOperationWrapper(exception3);
			}
			finally
			{
				verboseLog = haTaskStringBuilderOutputHelper.ToString();
			}
		}

		public static void ThrowDagTaskOperationWrapper(Exception exception)
		{
			if (exception == null)
			{
				return;
			}
			string text = string.Empty;
			IHaRpcServerBaseException ex = exception as IHaRpcServerBaseException;
			if (ex != null)
			{
				text = ex.ErrorMessage;
			}
			else
			{
				text = exception.Message;
			}
			if (exception is ClusterException)
			{
				throw new DagTaskOperationFailedException(text, exception);
			}
			if (exception is TransientException)
			{
				throw new DagTaskServerTransientException(text, exception);
			}
			throw new DagTaskOperationFailedException(text, exception);
		}

		public static IADDatabaseAvailabilityGroup GetLocalServerDatabaseAvailabilityGroup(IADToplogyConfigurationSession adSession, out Exception exception)
		{
			return DirectoryHelper.GetLocalServerDatabaseAvailabilityGroup(adSession, out exception);
		}

		private static NodeActionTracker nodeActionTracker = new NodeActionTracker();
	}
}
