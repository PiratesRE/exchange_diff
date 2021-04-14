using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Management.Automation;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.ServiceProcess;
using Microsoft.Exchange.Cluster.ClusApi;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Configuration.Common;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Data.Storage.Cluster;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.EventMessages;
using Microsoft.Exchange.Management.SystemConfigurationTasks;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Rpc.ActiveManager;
using Microsoft.Exchange.Rpc.Cluster;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal static class DagTaskHelper
	{
		internal static void CheckServerDoesNotBelongToDifferentDag(Task.TaskErrorLoggingDelegate writeError, IConfigDataProvider dataSession, Server mailboxServer, string dagName)
		{
			ADObjectId databaseAvailabilityGroup = mailboxServer.DatabaseAvailabilityGroup;
			if (databaseAvailabilityGroup != null)
			{
				DatabaseAvailabilityGroup databaseAvailabilityGroup2 = (DatabaseAvailabilityGroup)dataSession.Read<DatabaseAvailabilityGroup>(databaseAvailabilityGroup);
				if (databaseAvailabilityGroup2 != null && databaseAvailabilityGroup2.Name != dagName)
				{
					writeError(new DagTaskServerMailboxServerIsInDifferentDagException(mailboxServer.Name, databaseAvailabilityGroup2.Name), ErrorCategory.InvalidArgument, null);
				}
			}
		}

		internal static DatabaseAvailabilityGroup DagIdParameterToDag(DatabaseAvailabilityGroupIdParameter dagIdParam, IConfigDataProvider configSession)
		{
			IEnumerable<DatabaseAvailabilityGroup> objects = dagIdParam.GetObjects<DatabaseAvailabilityGroup>(null, configSession);
			DatabaseAvailabilityGroup result;
			using (IEnumerator<DatabaseAvailabilityGroup> enumerator = objects.GetEnumerator())
			{
				if (!enumerator.MoveNext())
				{
					throw new ManagementObjectNotFoundException(Strings.ErrorDagNotFound(dagIdParam.ToString()));
				}
				DatabaseAvailabilityGroup databaseAvailabilityGroup = enumerator.Current;
				if (enumerator.MoveNext())
				{
					throw new ManagementObjectAmbiguousException(Strings.ErrorDagNotUnique(dagIdParam.ToString()));
				}
				result = databaseAvailabilityGroup;
			}
			return result;
		}

		internal static DatabaseAvailabilityGroup ReadDagByName(string dagName, IConfigDataProvider configSession)
		{
			DatabaseAvailabilityGroupIdParameter dagIdParam = DatabaseAvailabilityGroupIdParameter.Parse(dagName);
			return DagTaskHelper.DagIdParameterToDag(dagIdParam, configSession);
		}

		internal static DatabaseAvailabilityGroup ReadDag(ADObjectId dagId, IConfigDataProvider configSession)
		{
			DatabaseAvailabilityGroup result = null;
			if (!ADObjectId.IsNullOrEmpty(dagId))
			{
				result = (DatabaseAvailabilityGroup)configSession.Read<DatabaseAvailabilityGroup>(dagId);
			}
			return result;
		}

		internal static DatabaseAvailabilityGroup GetDagForDatabase(Database database, IConfigDataProvider configSession, Task.TaskErrorLoggingDelegate writeError)
		{
			DatabaseAvailabilityGroup databaseAvailabilityGroup = DagTaskHelper.ReadDag(database.MasterServerOrAvailabilityGroup, configSession);
			if (databaseAvailabilityGroup == null)
			{
				writeError(new InconsistentADException(Strings.InconsistentADDbMasterServerNotADag(database.Name, database.MasterServerOrAvailabilityGroup.ToString())), ErrorCategory.InvalidOperation, database.Identity);
			}
			return databaseAvailabilityGroup;
		}

		internal static void PreventTaskWhenTPREnabled(DatabaseAvailabilityGroup dag, Task task)
		{
			if (dag.ThirdPartyReplication == ThirdPartyReplicationMode.Enabled)
			{
				task.WriteError(new InvalidTPRTaskException(task.MyInvocation.MyCommand.Name), ErrorCategory.InvalidOperation, dag.Identity);
			}
		}

		internal static void PreventTaskWhenAutoNetConfigIsEnabled(DatabaseAvailabilityGroup dag, Task task)
		{
			if (!dag.ManualDagNetworkConfiguration)
			{
				task.WriteError(new DagNetTaskIsManualOnlyException(task.MyInvocation.MyCommand.Name, dag.Name), ErrorCategory.InvalidOperation, dag.Identity);
			}
		}

		internal static AmCluster OpenClusterFromDag(DatabaseAvailabilityGroup dag)
		{
			List<AmServerName> list = (from adServerId in dag.Servers
			select new AmServerName(adServerId.Name)).ToList<AmServerName>();
			if (list.Count == 0)
			{
				return null;
			}
			return AmCluster.OpenByNames(list);
		}

		internal static bool IsKnownException(object task, Exception e)
		{
			if (AmExceptionHelper.IsKnownClusterException(task, e))
			{
				return true;
			}
			if (e is AmServerException || e is RpcException || e is DagNetworkManagementException || e is ClusterException || e is HaRpcServerBaseException || e is HaRpcServerTransientBaseException)
			{
				ExTraceGlobals.CmdletsTracer.TraceError<string>((long)task.GetHashCode(), task.ToString() + " got exception : {0}", e.Message);
				return true;
			}
			return false;
		}

		internal static bool LogClussvcState(ITaskOutputHelper output, AmServerName mailboxServerName)
		{
			string text = "not installed";
			string text2 = "none";
			bool result = false;
			try
			{
				using (ServiceController serviceController = new ServiceController("clussvc", mailboxServerName.Fqdn))
				{
					ServiceControllerStatus status = serviceController.Status;
					text = status.ToString();
					if (status == ServiceControllerStatus.Running)
					{
						result = true;
					}
				}
			}
			catch (Win32Exception ex)
			{
				output.AppendLogMessage("LogClussvcState: Received a Win32Exception for server {0}.", new object[]
				{
					mailboxServerName.NetbiosName
				});
				if (!DagTaskHelper.IsServiceNotInstalled(ex))
				{
					text = "unknown";
					text2 = ex.ToString();
				}
			}
			catch (InvalidOperationException ex2)
			{
				output.AppendLogMessage("LogClussvcState: Received an InvalidOperationException for server {0}.", new object[]
				{
					mailboxServerName.NetbiosName
				});
				if (ex2.InnerException == null)
				{
					text2 = ex2.ToString();
				}
				else
				{
					Win32Exception ex3 = ex2.InnerException as Win32Exception;
					if (!DagTaskHelper.IsServiceNotInstalled(ex3))
					{
						text = "unknown";
						text2 = ex3.ToString();
					}
				}
			}
			output.AppendLogMessage("LogClussvcState: clussvc is {0} on {1}. Exception (if any) = {2}", new object[]
			{
				text,
				mailboxServerName,
				text2
			});
			return result;
		}

		internal static void LogCommandLineParameters(ITaskOutputHelper output, string commandLine, string[] parametersToLog, PropertyBag allParameters)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			output.AppendLogMessage("commandline: {0}", new object[]
			{
				commandLine
			});
			foreach (string text in parametersToLog)
			{
				output.AppendLogMessage("Option '{0}' = '{1}'.", new object[]
				{
					text,
					allParameters[text]
				});
			}
			using (Process currentProcess = Process.GetCurrentProcess())
			{
				output.AppendLogMessage("Process: {0} {1}:{2}.", new object[]
				{
					currentProcess.ProcessName,
					currentProcess.MainModule.ModuleName,
					currentProcess.Id
				});
			}
			using (WindowsIdentity current = WindowsIdentity.GetCurrent())
			{
				output.AppendLogMessage("User context = '{0}'.", new object[]
				{
					current.Name
				});
				foreach (IdentityReference identityReference in current.Groups)
				{
					try
					{
						NTAccount ntaccount = identityReference.Translate(typeof(NTAccount)) as NTAccount;
						if (ntaccount != null)
						{
							output.AppendLogMessage("  Member of group '{0}'.", new object[]
							{
								ntaccount.ToString()
							});
						}
					}
					catch (IdentityNotMappedException)
					{
						output.AppendLogMessage("  Member of group (unprintable, IdentityNotMappedException encountered).", new object[0]);
					}
				}
			}
		}

		internal static bool IsServiceNotInstalled(Win32Exception ex)
		{
			uint nativeErrorCode = (uint)ex.NativeErrorCode;
			return nativeErrorCode == 1060U;
		}

		internal static ADObjectId FindServerAdObjectIdInDag(DatabaseAvailabilityGroup dag, AmServerName serverName)
		{
			foreach (ADObjectId adobjectId in dag.Servers)
			{
				if (SharedHelper.StringIEquals(adobjectId.Name, serverName.NetbiosName))
				{
					return adobjectId;
				}
			}
			return null;
		}

		public static AmServerName GetPrimaryActiveManagerNode(DatabaseAvailabilityGroup dag)
		{
			return DagTaskHelper.GetPrimaryActiveManagerNode(ADObjectWrapperFactory.CreateWrapper(dag));
		}

		public static AmServerName GetPrimaryActiveManagerNode(IADDatabaseAvailabilityGroup dag)
		{
			AmServerName result = null;
			AmPamInfo primaryActiveManager = AmRpcClientHelper.GetPrimaryActiveManager(dag);
			if (primaryActiveManager != null)
			{
				result = new AmServerName(primaryActiveManager.ServerName);
			}
			return result;
		}

		public static DeferredFailoverEntry[] GetServersInDeferredRecovery(DatabaseAvailabilityGroup dag)
		{
			List<DeferredFailoverEntry> list = new List<DeferredFailoverEntry>();
			if (dag.PrimaryActiveManager != null)
			{
				List<AmDeferredRecoveryEntry> deferredRecoveryEntries = AmRpcClientHelper.GetDeferredRecoveryEntries(dag.PrimaryActiveManager.Name);
				if (deferredRecoveryEntries != null)
				{
					foreach (AmDeferredRecoveryEntry amDeferredRecoveryEntry in deferredRecoveryEntries)
					{
						ADObjectId adobjectId = DagTaskHelper.FindServerAdObjectIdInDag(dag, new AmServerName(amDeferredRecoveryEntry.ServerFqdn));
						if (adobjectId != null)
						{
							list.Add(new DeferredFailoverEntry(adobjectId, DateTime.Parse(amDeferredRecoveryEntry.RecoveryDueTimeInUtc)));
						}
					}
				}
			}
			return list.ToArray();
		}

		internal static void CompareDagClusterMembership(ITaskOutputHelper output, string dagName, IEnumerable<AmServerName> serversInAd, IEnumerable<AmServerName> serversInCluster, IEnumerable<AmServerName> serverNamesToIgnore)
		{
			output.AppendLogMessage(Strings.DagTaskCheckingDagServersAreClustered(dagName));
			IEnumerable<AmServerName> first = serversInAd.Except(serverNamesToIgnore);
			IEnumerable<AmServerName> source = first.Except(serversInCluster);
			if (source.Count<AmServerName>() != 0)
			{
				IEnumerable<string> source2 = from name in source
				select name.NetbiosName;
				output.WriteErrorSimple(new DagTaskServersInAdNotInCluster(string.Join(",", source2.ToArray<string>())));
			}
			output.AppendLogMessage(Strings.DagTaskCheckingClusterNodesForDagMembership(dagName));
			IEnumerable<AmServerName> first2 = serversInCluster.Except(serverNamesToIgnore);
			IEnumerable<AmServerName> source3 = first2.Except(serversInAd);
			if (source3.Count<AmServerName>() != 0)
			{
				IEnumerable<string> source4 = from name in source3
				select name.NetbiosName;
				output.WriteErrorSimple(new DagTaskServersInClusterNotInAd(string.Join(",", source4.ToArray<string>())));
			}
		}

		internal static void ValidateDagClusterMembership(ITaskOutputHelper output, DatabaseAvailabilityGroup dag, AmCluster clusDag, AmServerName ignoreThisServer)
		{
			Dictionary<AmServerName, Server> servers = new Dictionary<AmServerName, Server>(10);
			Dictionary<AmServerName, Server> startedServers = new Dictionary<AmServerName, Server>(10);
			Dictionary<AmServerName, Server> dictionary = new Dictionary<AmServerName, Server>(10);
			DatabaseAvailabilityGroupAction.ResolveServers(output, dag, servers, startedServers, dictionary);
			if (!dag.IsDagEmpty())
			{
				IEnumerable<AmServerName> serversInAd = from ADObjectId memberServerId in dag.Servers
				select new AmServerName(memberServerId);
				List<AmServerName> list = new List<AmServerName>(dictionary.Count);
				foreach (Server server in dictionary.Values)
				{
					list.Add(new AmServerName(server.Id));
				}
				if (ignoreThisServer != null && !list.Contains(ignoreThisServer))
				{
					list.Add(ignoreThisServer);
				}
				IEnumerable<AmServerName> serversInCluster = clusDag.EnumerateNodeNames();
				DagTaskHelper.CompareDagClusterMembership(output, dag.Name, serversInAd, serversInCluster, list);
			}
		}

		internal static void ValidateIPAddressList(ITaskOutputHelper output, string machineName, IPAddress[] ipList, string dagName)
		{
			try
			{
				IPHostEntry hostEntry = Dns.GetHostEntry(machineName);
				IEnumerable<IPAddress> source = ipList.Intersect(hostEntry.AddressList);
				if (source.Count<IPAddress>() > 0)
				{
					string[] value = (from addr in source
					select addr.ToString()).ToArray<string>();
					output.WriteErrorSimple(new DagTaskServerIPAddressSameAsStaticIPAddressException(machineName, string.Join(",", value), dagName));
				}
			}
			catch (SocketException ex)
			{
				output.AppendLogMessage("Failure while trying to resolve {0}: threw a SocketException: {1}.", new object[]
				{
					machineName,
					ex.Message
				});
			}
		}

		internal static void LogMachineIpAddresses(ITaskOutputHelper output, string machineName)
		{
			try
			{
				output.AppendLogMessage("Looking up IP addresses for {0}.", new object[]
				{
					machineName
				});
				IPHostEntry hostEntry = Dns.GetHostEntry(machineName);
				string[] array = new string[hostEntry.AddressList.Length];
				for (int i = 0; i < hostEntry.AddressList.Length; i++)
				{
					array[i] = hostEntry.AddressList[i].ToString();
				}
				output.AppendLogMessage("  {0} = [ {1} ].", new object[]
				{
					machineName,
					string.Join(", ", array)
				});
			}
			catch (SocketException ex)
			{
				output.AppendLogMessage("Failure while trying to resolve {0}: threw a SocketException: {1}.", new object[]
				{
					machineName,
					ex.Message
				});
			}
		}

		internal static void LogFileShareSecurity(ITaskOutputHelper output, byte[] binarySecurityDescriptor)
		{
			FileSystemSecurity fileSystemSecurity = new DirectorySecurity();
			fileSystemSecurity.SetSecurityDescriptorBinaryForm(binarySecurityDescriptor);
			foreach (object obj in fileSystemSecurity.GetAccessRules(true, true, typeof(NTAccount)))
			{
				AuthorizationRule authorizationRule = (AuthorizationRule)obj;
				FileSystemAccessRule fileSystemAccessRule = authorizationRule as FileSystemAccessRule;
				if (fileSystemAccessRule != null)
				{
					output.AppendLogMessage("  Rule: {0} : {1}", new object[]
					{
						authorizationRule.IdentityReference,
						fileSystemAccessRule.FileSystemRights
					});
				}
			}
		}

		internal static void LogCnoState(ITaskOutputHelper output, string dagName, IAmClusterResource netName)
		{
			AmResourceState state = netName.GetState();
			output.AppendLogMessage("The CNO is currently {0}.", new object[]
			{
				state
			});
			if (state != AmResourceState.Online && state != AmResourceState.OnlinePending)
			{
				output.WriteWarning(Strings.DagTaskCnoNotOnline(dagName));
			}
		}

		internal static void LogRemoteVerboseLog(ITaskOutputHelper output, string remoteServerName, string verboseLog)
		{
			if (!string.IsNullOrEmpty(verboseLog))
			{
				output.AppendLogMessage(ReplayStrings.DagTaskRemoteOperationLogBegin(remoteServerName));
				output.AppendLogMessage(ReplayStrings.DagTaskRemoteOperationLogData(verboseLog));
				output.AppendLogMessage(ReplayStrings.DagTaskRemoteOperationLogEnd(remoteServerName));
			}
		}

		private static void CreateFileShareWitnessQuorum(ITaskOutputHelper output, AmCluster cluster, string fswShare)
		{
			try
			{
				output.WriteProgressSimple(Strings.DagTaskCreatingFsw(fswShare));
				DagHelper.CreateFileShareWitnessQuorum(output, cluster, fswShare);
				DagTaskHelper.RecreateFswQuorumIfItIsOffline(output, cluster, fswShare);
			}
			catch (ClusterException ex)
			{
				if (ex.InnerException != null && ex.InnerException is Win32Exception)
				{
					int nativeErrorCode = ((Win32Exception)ex.InnerException).NativeErrorCode;
					if (nativeErrorCode == 53)
					{
						output.WriteErrorSimple(new DagTaskProblemChangingQuorumExceptionBadNetPath(cluster.Name, fswShare, ex));
					}
					else if ((long)nativeErrorCode == 67L)
					{
						output.WriteErrorSimple(new DagTaskProblemChangingQuorumExceptionBadNetName(cluster.Name, fswShare, ex));
					}
					else if (nativeErrorCode == 5)
					{
						output.WriteErrorSimple(new DagTaskProblemChangingQuorumExceptionAccessDenied(cluster.Name, fswShare, ex));
					}
					else if (nativeErrorCode == 3)
					{
						output.WriteErrorSimple(new DagTaskProblemChangingQuorumExceptionPathNotFound(cluster.Name, fswShare, ex));
					}
					else
					{
						output.WriteErrorSimple(new DagTaskProblemChangingQuorumException(cluster.Name, ex));
					}
				}
				else
				{
					output.WriteErrorSimple(new DagTaskProblemChangingQuorumException(cluster.Name, ex));
				}
			}
		}

		internal static void RevertToMnsQuorum(ITaskOutputHelper output, AmCluster cluster, string fswShareCurrent)
		{
			try
			{
				output.WriteProgressSimple(Strings.DagTaskRevertingFsw(fswShareCurrent));
				DagHelper.RevertToMnsQuorum(output, cluster);
			}
			catch (ClusterException ex)
			{
				output.WriteErrorSimple(new DagTaskProblemChangingQuorumException(cluster.Name, ex));
			}
		}

		internal static bool IsQuorumTypeFileShareWitness(ITaskOutputHelper output, AmCluster cluster)
		{
			bool result;
			try
			{
				result = DagHelper.IsQuorumTypeFileShareWitness(cluster);
			}
			catch (ClusterException)
			{
				result = false;
			}
			return result;
		}

		internal static bool ShouldBeFileShareWitness(ITaskOutputHelper output, DatabaseAvailabilityGroup dag, AmCluster clusDag, bool removeNode)
		{
			bool result = false;
			int num = clusDag.EnumerateNodeNames().Count<AmServerName>();
			if (num > 1 && removeNode)
			{
				output.AppendLogMessage("ShouldBeFileShareWitness has been called anticipating a node removal. Decrementing the server count from {0}.", new object[]
				{
					num
				});
				num--;
			}
			if (num % 2 == 0)
			{
				output.AppendLogMessage("There are {0} started servers in the cluster, which is an even number. That requires a file share witness!", new object[]
				{
					num
				});
				result = true;
			}
			else if (num == 1)
			{
				MultiValuedProperty<string> stoppedMailboxServers = dag.StoppedMailboxServers;
				if (stoppedMailboxServers != null && stoppedMailboxServers.Count > 0)
				{
					output.AppendLogMessage("There is 1 server in the cluster, and StoppedMailboxServers is not empty. That requires a file share witness!", new object[0]);
					result = true;
				}
				else
				{
					output.AppendLogMessage("There is a single server in the cluster, and StoppedMailboxServers is empty. No file share witness needed.", new object[0]);
				}
			}
			else
			{
				output.AppendLogMessage("There are {0} servers in the cluster, which is an odd number. That does not require a file share witness.", new object[]
				{
					num
				});
			}
			return result;
		}

		internal static bool IsDagFailedOverToOtherSite(ITaskOutputHelper output, DatabaseAvailabilityGroup dag)
		{
			bool result = false;
			if (dag.DatacenterActivationMode == DatacenterActivationModeOption.DagOnly)
			{
				MultiValuedProperty<string> stoppedMailboxServers = dag.StoppedMailboxServers;
				if (stoppedMailboxServers != null && stoppedMailboxServers.Count > 0)
				{
					output.AppendLogMessage("IsDagFailedOverToOtherSite: The DAG is in DagOnly mode, and there are servers in the StoppedMailboxServers list. Therefore, the DAG is active in the alternate site.", new object[0]);
					result = true;
				}
				else
				{
					output.AppendLogMessage("IsDagFailedOverToOtherSite: The DAG is in DagOnly mode, but StoppedMailboxServers list is empty. Therefore, the DAG is active in both site.", new object[0]);
				}
			}
			else
			{
				output.AppendLogMessage("IsDagFailedOverToOtherSite: The DAG is in DAC mode of 'off'.", new object[0]);
			}
			return result;
		}

		internal static int DagTaskClusterSetupProgressCallbackWrapper(IntPtr pvCallbackArg, ClusapiMethods.CLUSTER_SETUP_PHASE eSetupPhase, ClusapiMethods.CLUSTER_SETUP_PHASE_TYPE ePhaseType, ClusapiMethods.CLUSTER_SETUP_PHASE_SEVERITY ePhaseSeverity, uint dwPercentComplete, string lpszObjectName, uint dwStatus)
		{
			bool flag = true;
			int result = 1;
			if (pvCallbackArg != IntPtr.Zero)
			{
				object target = GCHandle.FromIntPtr(pvCallbackArg).Target;
				IClusterSetupProgress clusterSetupProgress = target as IClusterSetupProgress;
				if (clusterSetupProgress == null)
				{
					ExTraceGlobals.ClusterTracer.TraceDebug<Type>(0L, "DagTaskClusterSetupProgressCallbackWrapper had a callback object of an unexpected type ({0}).", target.GetType());
				}
				else
				{
					result = clusterSetupProgress.ClusterSetupProgressCallback(pvCallbackArg, eSetupPhase, ePhaseType, ePhaseSeverity, dwPercentComplete, lpszObjectName, dwStatus);
					flag = false;
				}
			}
			if (flag)
			{
				ExTraceGlobals.ClusterTracer.TraceDebug(0L, "GenericClusterSetupProgressCallback( \tcallbackArg = {0},\teSetupPhase = {1},\tePhaseType = {2},\tePhaseSeverity = {3},\tdwPercentComplete = {4},\tszObjectName = {5},\tdwStatus = {6} )", new object[]
				{
					pvCallbackArg,
					eSetupPhase,
					ePhaseType,
					ePhaseSeverity,
					dwPercentComplete,
					lpszObjectName,
					dwStatus
				});
				result = 1;
			}
			return result;
		}

		internal static void CheckStoreIsRunning(Task.TaskErrorLoggingDelegate writeErrorDelegate, AmServerName machineName)
		{
			if (!DagTaskHelper.IsStoreRunningOnNode(machineName))
			{
				writeErrorDelegate(new DagTaskStoreMustBeRunningOnMachineException(machineName.NetbiosName), ErrorCategory.InvalidArgument, null);
			}
		}

		internal static bool DoesComputerAccountExist(ITopologyConfigurationSession configSession, string cnoName, out bool accountEnabled)
		{
			if (configSession == null)
			{
				configSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(false, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 1008, "DoesComputerAccountExist", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\Cluster\\dagtaskhelper.cs");
			}
			bool useConfigNC = configSession.UseConfigNC;
			bool useGlobalCatalog = configSession.UseGlobalCatalog;
			bool flag = false;
			accountEnabled = false;
			configSession.UseConfigNC = false;
			configSession.UseGlobalCatalog = true;
			try
			{
				ADComputer adcomputer = configSession.FindComputerByHostName(cnoName);
				if (adcomputer != null)
				{
					flag = true;
					accountEnabled = true;
				}
				ExTraceGlobals.ClusterTracer.TraceDebug<string, bool, bool>(0L, "Computer account {0} exists: {1}. Enabled: {2}.", cnoName, flag, accountEnabled);
				ExTraceGlobals.ClusterTracer.TraceDebug<string, string>(0L, "Computer account {0} is {1}.", cnoName, (adcomputer == null) ? "<null>" : adcomputer.ToString());
			}
			finally
			{
				configSession.UseConfigNC = useConfigNC;
				configSession.UseGlobalCatalog = useGlobalCatalog;
			}
			return flag;
		}

		internal static void DisableComputerAccount(ITaskOutputHelper output, ITopologyConfigurationSession configSession, string cnoName)
		{
			if (configSession == null)
			{
				configSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(false, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 1070, "DisableComputerAccount", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\Cluster\\dagtaskhelper.cs");
			}
			bool useConfigNC = configSession.UseConfigNC;
			bool useGlobalCatalog = configSession.UseGlobalCatalog;
			configSession.UseConfigNC = false;
			configSession.UseGlobalCatalog = true;
			try
			{
				ADComputer adcomputer = configSession.FindComputerByHostName(cnoName);
				if (adcomputer != null)
				{
					ExTraceGlobals.ClusterTracer.TraceDebug<string>(0L, "Computer account {0} exists and is enabled.", cnoName);
					adcomputer.DisableComputerAccount();
					configSession.Save(adcomputer);
					ExTraceGlobals.ClusterTracer.TraceDebug<string>(0L, "Computer account {0} has been disabled", cnoName);
				}
			}
			catch (ADTransientException ex)
			{
				output.WriteWarning(Strings.DagTaskCouldNotDisableAccountName(cnoName, ex));
			}
			catch (ADExternalException ex2)
			{
				output.WriteWarning(Strings.DagTaskCouldNotDisableAccountName(cnoName, ex2));
			}
			catch (ADOperationException ex3)
			{
				output.WriteWarning(Strings.DagTaskCouldNotDisableAccountName(cnoName, ex3));
			}
			finally
			{
				configSession.UseConfigNC = useConfigNC;
				configSession.UseGlobalCatalog = useGlobalCatalog;
			}
		}

		internal static bool IsStoreRunningOnNode(AmServerName nodeName)
		{
			ServiceController serviceController = new ServiceController("msexchangeis", nodeName.Fqdn);
			using (serviceController)
			{
				try
				{
					ExTraceGlobals.ClusterTracer.TraceDebug<string, ServiceControllerStatus, AmServerName>(0L, "IsStoreRunningOnNode(): {0} is {1} on {2}.", serviceController.ServiceName, serviceController.Status, nodeName);
					if (serviceController.Status == ServiceControllerStatus.Running)
					{
						return true;
					}
				}
				catch (Win32Exception arg)
				{
					ExTraceGlobals.ClusterTracer.TraceDebug<Win32Exception>(0L, "IsStoreRunningOnNode: Machine {0}, error {1}", arg);
				}
				catch (InvalidOperationException arg2)
				{
					ExTraceGlobals.ClusterTracer.TraceDebug<InvalidOperationException>(0L, "IsStoreRunningOnNode: Machine {0}, error {1}", arg2);
				}
			}
			return false;
		}

		internal static MailboxServer ServerToMailboxServer(Task.TaskErrorLoggingDelegate writeErrorDelegate, Server server)
		{
			if (!server.IsMailboxServer)
			{
				writeErrorDelegate(new NewDagServerIsNotMailboxServerException(server.Name), ErrorCategory.InvalidArgument, null);
			}
			return new MailboxServer(server);
		}

		private static void PromoteSingleDatabaseMasterServer(IConfigDataProvider dataSession, HaTaskOutputHelper output, Database mdb, DatabaseAvailabilityGroup dag, Server currentServer)
		{
			ADObjectId server = mdb.Server;
			output.AppendLogMessage("Database '{0}' is hosted on server '{1}'. Updating MasterServerOrAvailabilityGroup", new object[]
			{
				mdb,
				currentServer.Identity
			});
			output.WriteProgressIncrementalSimple(Strings.DagTaskUpdatingMasterServerOnDatabase(mdb.Name, dag.Identity.ToString()), 1);
			output.AppendLogMessage("Updating '{0}'.MasterServerOrAvailabilityGroup to '{1}.", new object[]
			{
				mdb.Name,
				dag.Identity.ToString()
			});
			mdb.MasterServerOrAvailabilityGroup = (ADObjectId)dag.Identity;
			mdb.InvalidDatabaseCopiesAllowed = true;
			dataSession.Save(mdb);
		}

		internal static void PromoteDagServersDatabasesToDag(IConfigurationSession dataSession, HaTaskOutputHelper output, DatabaseAvailabilityGroup dag, Server dagMemberServer)
		{
			dataSession = DagTaskHelper.CreateCustomConfigSessionIfNecessary(dataSession, ConsistencyMode.PartiallyConsistent, false);
			Database[] databases = dagMemberServer.GetDatabases(true);
			foreach (Database mdb in databases)
			{
				DagTaskHelper.PromoteSingleDatabaseMasterServer(dataSession, output, mdb, dag, dagMemberServer);
			}
			for (int j = 0; j < databases.Length; j++)
			{
				output.AppendLogMessage("database[ {0} ] ({1})'s MsOrDag={2}.", new object[]
				{
					j.ToString(),
					databases[j].Name,
					databases[j].MasterServerOrAvailabilityGroup
				});
			}
		}

		private static void RevertSingleDatabase(IConfigDataProvider dataSession, HaTaskOutputHelper output, Database mdb, Server dagMemberServer)
		{
			output.WriteProgressIncrementalSimple(Strings.DagTaskUpdatingMasterServerOnDatabase(mdb.Name, dagMemberServer.Name), 1);
			output.AppendLogMessage("Updating '{0}'.MasterServerOrAvailabilityGroup to '{1}.", new object[]
			{
				mdb.Name,
				dagMemberServer
			});
			mdb.MasterServerOrAvailabilityGroup = (ADObjectId)dagMemberServer.Identity;
			mdb.InvalidDatabaseCopiesAllowed = true;
			dataSession.Save(mdb);
		}

		internal static void RevertDagServersDatabasesToStandalone(IConfigurationSession dataSession, HaTaskOutputHelper output, Server dagMemberServer)
		{
			dataSession = DagTaskHelper.CreateCustomConfigSessionIfNecessary(dataSession, ConsistencyMode.PartiallyConsistent, false);
			Database[] databases = dagMemberServer.GetDatabases(true);
			foreach (Database mdb in databases)
			{
				DagTaskHelper.RevertSingleDatabase(dataSession, output, mdb, dagMemberServer);
			}
			for (int j = 0; j < databases.Length; j++)
			{
				output.AppendLogMessage("database[ {0} ] ({1})'s MasterServerOrDag={2}.", new object[]
				{
					j.ToString(),
					databases[j].Name,
					databases[j].MasterServerOrAvailabilityGroup
				});
			}
		}

		internal static IConfigurationSession CreateCustomConfigSessionIfNecessary(IConfigurationSession session, ConsistencyMode intendedConsistency, bool intendedReadonly)
		{
			if (session == null)
			{
				session = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(intendedReadonly, intendedConsistency, ADSessionSettings.FromRootOrgScopeSet(), 1343, "CreateCustomConfigSessionIfNecessary", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\Cluster\\dagtaskhelper.cs");
			}
			else if (session.ConsistencyMode != intendedConsistency || session.ReadOnly != intendedReadonly)
			{
				session = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(session.DomainController, intendedReadonly, intendedConsistency, session.NetworkCredential, session.SessionSettings, 1351, "CreateCustomConfigSessionIfNecessary", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\Cluster\\dagtaskhelper.cs");
			}
			return session;
		}

		internal static ADObjectId[] DetermineRemoteSites(ITopologyConfigurationSession taskSession, string dcFqdn, Server targetServer)
		{
			ADObjectId adobjectId = null;
			bool flag = false;
			try
			{
				if (taskSession.DomainController == null)
				{
					flag = true;
					Fqdn fqdn = Fqdn.Parse(dcFqdn);
					taskSession.DomainController = fqdn;
				}
				RootDse rootDse = taskSession.GetRootDse();
				adobjectId = rootDse.Site;
			}
			finally
			{
				if (flag)
				{
					taskSession.DomainController = null;
				}
			}
			ExTraceGlobals.CmdletsTracer.TraceDebug<ADObjectId>(0L, "DetermineRemoteSites found the task running against site {0}", adobjectId);
			if (targetServer.DatabaseAvailabilityGroup != null)
			{
				DatabaseAvailabilityGroup databaseAvailabilityGroup = taskSession.Read<DatabaseAvailabilityGroup>(targetServer.DatabaseAvailabilityGroup);
				HashSet<ADObjectId> hashSet = new HashSet<ADObjectId>();
				foreach (ADObjectId entryId in databaseAvailabilityGroup.Servers)
				{
					MiniServer miniServer = taskSession.ReadMiniServer(entryId, DagTaskHelper.s_propertiesNeededFromServer);
					if (miniServer.ServerSite != null && !miniServer.ServerSite.Equals(adobjectId))
					{
						hashSet.Add(miniServer.ServerSite);
					}
				}
				if (hashSet.Count > 0)
				{
					return hashSet.ToArray<ADObjectId>();
				}
				return null;
			}
			else
			{
				if (targetServer.ServerSite != null && !targetServer.ServerSite.Equals(adobjectId))
				{
					return new ADObjectId[]
					{
						targetServer.ServerSite
					};
				}
				return null;
			}
		}

		internal static bool ForceReplication(IDirectorySession session, ADObject instanceToReplicate, ADObjectId[] sites, string objectIdentity, Task.TaskWarningLoggingDelegate writeWarning, Task.TaskVerboseLoggingDelegate writeVerbose)
		{
			bool result = false;
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			if (instanceToReplicate == null)
			{
				throw new ArgumentNullException("instanceToReplicate");
			}
			if (sites == null)
			{
				throw new ArgumentNullException("sites");
			}
			if (objectIdentity == null)
			{
				throw new ArgumentNullException("objectIdentity");
			}
			if (writeWarning == null)
			{
				throw new ArgumentNullException("writeWarning");
			}
			if (writeVerbose == null)
			{
				throw new ArgumentNullException("writeVerbose");
			}
			string[] array = session.ReplicateSingleObject(instanceToReplicate, sites);
			if (array == null || (array != null && array.Length > 0 && string.IsNullOrEmpty(array[0])))
			{
				writeWarning(Strings.WarningMultiSiteReplicationFailed(objectIdentity, string.Join(", ", (from site in sites
				select site.Name).ToArray<string>())));
			}
			else
			{
				writeVerbose(Strings.VerboseMultiSiteDCContact(array));
				result = true;
			}
			return result;
		}

		internal static RpcDatabaseCopyStatus2 GetRpcDatabaseCopyStatus(Task.TaskErrorLoggingDelegate writeError, Task.TaskWarningLoggingDelegate writeWarning, Task.TaskVerboseLoggingDelegate writeVerbose, Server server, Database database, bool ignoreInstanceNotFound, bool shouldWriteError, bool shouldWriteWarning, out bool instanceWasNotFound)
		{
			instanceWasNotFound = false;
			RpcDatabaseCopyStatus2 result = null;
			string name = server.Name;
			try
			{
				writeVerbose(Strings.GetCopyStatusRpcQuery(database.Name, name));
				RpcDatabaseCopyStatus2[] copyStatus = ReplayRpcClientHelper.GetCopyStatus(name, RpcGetDatabaseCopyStatusFlags2.ReadThrough, new Guid[]
				{
					database.Guid
				});
				result = copyStatus[0];
			}
			catch (TaskServerException ex)
			{
				if (ex is ReplayServiceRpcUnknownInstanceException)
				{
					instanceWasNotFound = true;
					if (ignoreInstanceNotFound)
					{
						return null;
					}
				}
				GetCopyStatusRpcException ex2 = new GetCopyStatusRpcException(name, database.Name, ex.Message);
				if (shouldWriteError)
				{
					writeError(ex2, ErrorCategory.InvalidResult, null);
				}
				else if (shouldWriteWarning)
				{
					writeWarning(ex2.LocalizedString);
				}
			}
			return result;
		}

		public static void CheckNodeIsNotFswNode(AmServerName nodeName, AmServerName fswNodeName, Task.TaskErrorLoggingDelegate writeError)
		{
			if (nodeName.Equals(fswNodeName))
			{
				writeError(new DagTaskServerMailboxServerAlsoServesAsFswNodeException(nodeName.NetbiosName), ErrorCategory.InvalidOperation, null);
			}
		}

		public static void CheckDagCanBeActivatedInDatacenter(HaTaskOutputHelper output, DatabaseAvailabilityGroup dag, ADObjectId serverToRemove, ITopologyConfigurationSession dataSession)
		{
			if (dag != null && dag.Servers != null && ((serverToRemove == null) ? dag.Servers.Count : (dag.Servers.Count - 1)) >= 2)
			{
				HashSet<ADObjectId> hashSet = new HashSet<ADObjectId>();
				foreach (ADObjectId adobjectId in dag.Servers)
				{
					bool flag = false;
					if (serverToRemove != null)
					{
						if (!adobjectId.Equals(serverToRemove))
						{
							flag = true;
						}
					}
					else
					{
						flag = true;
					}
					if (flag)
					{
						MiniServer miniServer = dataSession.ReadMiniServer(adobjectId, DagTaskHelper.s_propertiesNeededFromServer);
						if (miniServer.ServerSite != null)
						{
							hashSet.Add(miniServer.ServerSite);
						}
					}
				}
				return;
			}
			if (serverToRemove == null)
			{
				output.WriteErrorSimple(new DagCantBeSetIntoDatacenterActivationModeException(dag.Name));
				return;
			}
			output.WriteErrorSimple(new DagServerCantBeRemovedInDatacenterActivationMode(serverToRemove.Name, dag.Name));
		}

		internal static void RecreateFswQuorumIfItIsOffline(ITaskOutputHelper output, AmCluster cluster, string fswShare)
		{
			using (AmClusterResource amClusterResource = cluster.OpenQuorumResource())
			{
				if (amClusterResource != null && amClusterResource.GetTypeName() == "File Share Witness")
				{
					output.AppendLogMessage("RecreateFswQuorumIfItIsOffline: The quorum type is {0}. Doing nothing.", new object[]
					{
						amClusterResource.GetTypeName()
					});
				}
				else
				{
					AmResourceState state = amClusterResource.GetState();
					if (state == AmResourceState.Online)
					{
						output.AppendLogMessage("RecreateFswQuorumIfItIsOffline: The quorum resource is online. Doing nothing.", new object[0]);
					}
					else
					{
						output.AppendLogMessage("RecreateFswQuorumIfItIsOffline: The quorum resource '{0}' is in state '{1}'.", new object[]
						{
							amClusterResource.Name,
							state
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
						output.AppendLogMessage("All of the machines are up, but the FSW quorum resource was not. The task will now delete it and create a new one.", new object[0]);
						string privateProperty = amClusterResource.GetPrivateProperty<string>("SharePath");
						DagTaskHelper.RevertToMnsQuorum(output, cluster, privateProperty);
						output.AppendLogMessage("The quorum type has been changed back to MNS quorum. Now to create a new FSW resource.", new object[0]);
						DagTaskHelper.CreateFileShareWitnessQuorum(output, cluster, fswShare);
						output.AppendLogMessage("A new file share fswShare resource has been created.", new object[0]);
					}
				}
			}
		}

		public static void ChangeQuorumToMnsOrFswAsAppropriate(ITaskOutputHelper output, Task task, DatabaseAvailabilityGroup dag, AmCluster clusDag, FileShareWitness primaryFsw, FileShareWitness alternateFsw, bool shouldBeFsw, bool forceAlternateFsw)
		{
			bool primary = true;
			string text;
			if (forceAlternateFsw)
			{
				text = alternateFsw.FileShareWitnessShare.ToString();
				primary = false;
			}
			else
			{
				bool flag = DagTaskHelper.IsDagFailedOverToOtherSite(output, dag);
				if (flag && alternateFsw != null && alternateFsw.IsInitialized)
				{
					text = alternateFsw.FileShareWitnessShare.ToString();
					primary = false;
				}
				else
				{
					text = primaryFsw.FileShareWitnessShare.ToString();
				}
			}
			if (shouldBeFsw)
			{
				DagTaskHelper.CreateFileShareWitnessQuorum(output, clusDag, text);
				DagTaskHelper.TryLogFswChangeEvent(task, dag.Name, text, primary);
				return;
			}
			DagTaskHelper.RevertToMnsQuorum(output, clusDag, text);
		}

		internal static bool TryLogFswChangeEvent(Task task, string dagName, string fsw, bool primary)
		{
			bool result = false;
			try
			{
				ExManagementApplicationLogger.LogEvent(primary ? ManagementEventLogConstants.Tuple_FswChangedToPrimary : ManagementEventLogConstants.Tuple_FswChangedToAlternate, new string[]
				{
					dagName,
					fsw
				});
				result = true;
			}
			catch (ArgumentException ex)
			{
				ExTraceGlobals.CmdletsTracer.TraceError<string>((long)task.GetHashCode(), task.ToString() + " was trying to raise an app log event and got an exception : {0}", ex.Message);
			}
			catch (InvalidOperationException ex2)
			{
				ExTraceGlobals.CmdletsTracer.TraceError<string>((long)task.GetHashCode(), task.ToString() + " was trying to raise an app log event and got an exception : {0}", ex2.Message);
			}
			catch (Win32Exception ex3)
			{
				ExTraceGlobals.CmdletsTracer.TraceError<string>((long)task.GetHashCode(), task.ToString() + " was trying to raise an app log event and got an exception : {0}", ex3.Message);
			}
			return result;
		}

		private static IConfigurationSession CreateScopedADSession<TDataObject>(DataAccessTask<TDataObject> task) where TDataObject : IConfigurable, new()
		{
			ADSessionSettings sessionSettings = ADSessionSettings.FromCustomScopeSet(task.ScopeSet, ADSystemConfigurationSession.GetRootOrgContainerId(task.DomainController, null), task.CurrentOrganizationId, task.ExecutingUserOrganizationId, true);
			return DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.PartiallyConsistent, sessionSettings, 1807, "CreateScopedADSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\Cluster\\dagtaskhelper.cs");
		}

		public static void VerifyDagIsWithinScopes<TDataObject>(DataAccessTask<TDataObject> task, DatabaseAvailabilityGroup dag, bool isModification) where TDataObject : IConfigurable, new()
		{
			task.VerifyIsWithinScopes(DagTaskHelper.CreateScopedADSession<TDataObject>(task), dag, isModification, new DataAccessTask<TDataObject>.ADObjectOutOfScopeString(Strings.ErrorCannotChangeObjectOutOfWriteScope));
		}

		public static void VerifyDagAndServersAreWithinScopes<TDataObject>(DataAccessTask<TDataObject> task, DatabaseAvailabilityGroup dag, bool isModification) where TDataObject : IConfigurable, new()
		{
			IConfigurationSession session = DagTaskHelper.CreateScopedADSession<TDataObject>(task);
			task.VerifyIsWithinScopes(session, dag, isModification, new DataAccessTask<TDataObject>.ADObjectOutOfScopeString(Strings.ErrorCannotChangeObjectOutOfWriteScope));
			if (!dag.IsDagEmpty())
			{
				Dictionary<AmServerName, Server> dictionary = new Dictionary<AmServerName, Server>();
				Dictionary<AmServerName, Server> startedServers = new Dictionary<AmServerName, Server>();
				Dictionary<AmServerName, Server> stoppedServers = new Dictionary<AmServerName, Server>();
				DatabaseAvailabilityGroupAction.ResolveServers(null, dag, dictionary, startedServers, stoppedServers);
				foreach (ADObjectId serverId in dag.Servers)
				{
					AmServerName key = new AmServerName(serverId);
					task.VerifyIsWithinScopes(session, dictionary[key], true, new DataAccessTask<TDataObject>.ADObjectOutOfScopeString(Strings.ErrorCannotChangeObjectOutOfWriteScope));
				}
			}
		}

		public static void NotifyServersOfConfigChange(IEnumerable<AmServerName> servers)
		{
			foreach (AmServerName server in servers)
			{
				DagTaskHelper.NotifyServerOfConfigChange(server);
			}
		}

		public static void NotifyServerOfConfigChange(AmServerName server)
		{
			Exception ex = null;
			try
			{
				AmRpcClientHelper.AmRefreshConfiguration(server.Fqdn, AmRefreshConfigurationFlags.None, 0);
			}
			catch (AmServerTransientException ex2)
			{
				ex = ex2;
			}
			catch (AmServerException ex3)
			{
				ex = ex3;
			}
			if (ex != null)
			{
				ExTraceGlobals.CmdletsTracer.TraceError<string, Exception>(0L, "NotifyDagServersOfConfigChange failed to server {0} : {1}", server.Fqdn, ex);
			}
		}

		private const string MsExchangeIs = "msexchangeis";

		private const string ClusSvc = "clussvc";

		private const uint ERROR_SERVICE_DOES_NOT_EXIST = 1060U;

		private const int m_MinimumNumberOfServersInDagForDatacenterActivationMode = 2;

		private static readonly PropertyDefinition[] s_propertiesNeededFromServer = new PropertyDefinition[]
		{
			ServerSchema.ServerSite
		};
	}
}
