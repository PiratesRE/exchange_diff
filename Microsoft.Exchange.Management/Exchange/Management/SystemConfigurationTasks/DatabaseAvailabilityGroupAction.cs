using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Management.Automation;
using System.Net;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.Cluster.ClusApi;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage.Cluster;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Monitoring;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	public class DatabaseAvailabilityGroupAction : SystemConfigurationObjectActionTask<DatabaseAvailabilityGroupIdParameter, DatabaseAvailabilityGroup>
	{
		private protected bool NeedToUpdateAD { protected get; private set; }

		private protected bool NeedToUpdateCluster { protected get; private set; }

		[Parameter(Mandatory = true, ParameterSetName = "MailboxSet", ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 0)]
		[Parameter(Mandatory = true, ParameterSetName = "Identity", ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 0)]
		public override DatabaseAvailabilityGroupIdParameter Identity
		{
			get
			{
				return (DatabaseAvailabilityGroupIdParameter)base.Fields["Identity"];
			}
			set
			{
				base.Fields["Identity"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "MailboxSet", ValueFromPipelineByPropertyName = true, ValueFromPipeline = true)]
		public MailboxServerIdParameter MailboxServer
		{
			get
			{
				return (MailboxServerIdParameter)base.Fields["MailboxServer"];
			}
			set
			{
				base.Fields["MailboxServer"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "Identity", ValueFromPipelineByPropertyName = true, ValueFromPipeline = true)]
		public AdSiteIdParameter ActiveDirectorySite
		{
			get
			{
				return (AdSiteIdParameter)base.Fields["ActiveDirectorySite"];
			}
			set
			{
				base.Fields["ActiveDirectorySite"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		[Parameter(Mandatory = false, ParameterSetName = "MailboxSet")]
		public SwitchParameter ConfigurationOnly
		{
			get
			{
				return (SwitchParameter)(base.Fields["ConfigurationOnly"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["ConfigurationOnly"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "MailboxSet")]
		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public SwitchParameter QuorumOnly
		{
			get
			{
				return (SwitchParameter)(base.Fields["QuorumOnly"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["QuorumOnly"] = value;
			}
		}

		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception) || exception is ClusterException || exception is DagTaskServerException || exception is DagTaskServerTransientException || exception is ADTransientException || exception is ADExternalException || exception is ADOperationException;
		}

		protected virtual string TaskName
		{
			get
			{
				return "databaseavailabilitygroupaction";
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			this.m_adSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(false, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 146, "InternalValidate", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\Cluster\\DatabaseAvailabilityGroupAction.cs");
			this.m_output = new HaTaskOutputHelper(this.TaskName, new Microsoft.Exchange.Configuration.Tasks.Task.TaskErrorLoggingDelegate(base.WriteError), new Microsoft.Exchange.Configuration.Tasks.Task.TaskWarningLoggingDelegate(this.WriteWarning), new Microsoft.Exchange.Configuration.Tasks.Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Microsoft.Exchange.Configuration.Tasks.Task.TaskProgressLoggingDelegate(base.WriteProgress), this.GetHashCode());
			this.m_output.CreateTempLogFile();
			this.NeedToUpdateAD = true;
			this.NeedToUpdateCluster = true;
			if (this.ConfigurationOnly && this.QuorumOnly)
			{
				this.m_output.WriteErrorSimple(new TaskBothConfigurationOnlyAndQuorumOnlySpecifiedException());
			}
			if (this.ConfigurationOnly)
			{
				this.NeedToUpdateCluster = false;
			}
			if (this.QuorumOnly)
			{
				this.NeedToUpdateAD = false;
			}
			this.m_dag = DagTaskHelper.DagIdParameterToDag(this.Identity, base.DataSession);
			DagTaskHelper.VerifyDagAndServersAreWithinScopes<DatabaseAvailabilityGroup>(this, this.m_dag, true);
			if (this.m_dag.DatacenterActivationMode != DatacenterActivationModeOption.DagOnly)
			{
				this.m_output.WriteErrorSimple(new TaskCanOnlyRunOnDacException(this.m_dag.Name));
			}
			DatabaseAvailabilityGroupAction.ResolveServers(this.m_output, this.m_dag, this.m_serversInDag, this.m_startedServers, this.m_stoppedServers);
			if (this.MailboxServer != null)
			{
				this.m_mailboxServer = (Server)base.GetDataObject<Server>(this.MailboxServer, base.DataSession, null, new LocalizedString?(Strings.ErrorServerNotFound(this.MailboxServer.ToString())), new LocalizedString?(Strings.ErrorServerNotUnique(this.MailboxServer.ToString())));
				if (!this.m_mailboxServer.IsMailboxServer)
				{
					this.m_output.WriteErrorSimple(new OperationOnlyOnMailboxServerException(this.m_mailboxServer.Name));
				}
				if (this.m_mailboxServer.MajorVersion != Server.CurrentExchangeMajorVersion)
				{
					this.m_output.WriteErrorSimple(new DagTaskErrorServerWrongVersion(this.m_mailboxServer.Name));
				}
			}
			if (this.ActiveDirectorySite != null)
			{
				IConfigurationSession configurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(false, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 225, "InternalValidate", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\Cluster\\DatabaseAvailabilityGroupAction.cs");
				configurationSession.UseConfigNC = false;
				configurationSession.UseGlobalCatalog = true;
				ADSite adsite = (ADSite)base.GetDataObject<ADSite>(this.ActiveDirectorySite, configurationSession, null, new LocalizedString?(Strings.ErrorSiteNotFound(this.ActiveDirectorySite.ToString())), new LocalizedString?(Strings.ErrorSiteNotUnique(this.ActiveDirectorySite.ToString())));
			}
			base.InternalValidate();
			TaskLogger.LogExit();
		}

		internal static void ResolveServersBasedOnStoppedList(ILogTraceHelper output, DatabaseAvailabilityGroup dag, Dictionary<AmServerName, Server> servers, Dictionary<AmServerName, Server> startedServers, Dictionary<AmServerName, Server> stoppedServers)
		{
			DatabaseAvailabilityGroupAction.ResolveServersInternal(output, dag, servers, startedServers, stoppedServers, false);
		}

		internal static void ResolveServers(ILogTraceHelper output, DatabaseAvailabilityGroup dag, Dictionary<AmServerName, Server> servers, Dictionary<AmServerName, Server> startedServers, Dictionary<AmServerName, Server> stoppedServers)
		{
			DatabaseAvailabilityGroupAction.ResolveServersInternal(output, dag, servers, startedServers, stoppedServers, true);
		}

		private static void ResolveServersInternal(ILogTraceHelper output, DatabaseAvailabilityGroup dag, Dictionary<AmServerName, Server> servers, Dictionary<AmServerName, Server> startedServers, Dictionary<AmServerName, Server> stoppedServers, bool startedListIsReadFromAd)
		{
			if (dag == null)
			{
				throw new ArgumentNullException("dag");
			}
			if (servers == null)
			{
				throw new ArgumentNullException("servers");
			}
			if (startedServers == null)
			{
				throw new ArgumentNullException("startedServers");
			}
			if (stoppedServers == null)
			{
				throw new ArgumentNullException("stoppedServers");
			}
			output = (output ?? NullLogTraceHelper.GetNullLogger());
			foreach (ADObjectId entryId in dag.Servers)
			{
				Server server = dag.Session.Read<Server>(entryId);
				AmServerName amServerName = new AmServerName(server.Fqdn);
				if (!server.IsMailboxServer)
				{
					throw new OperationOnlyOnMailboxServerException(server.Name);
				}
				if (server.MajorVersion != Server.CurrentExchangeMajorVersion)
				{
					throw new DagTaskErrorServerWrongVersion(server.Name);
				}
				servers.Add(amServerName, server);
				bool flag = false;
				if (dag.StoppedMailboxServers.Contains(amServerName.Fqdn))
				{
					stoppedServers.Add(amServerName, server);
					flag = true;
				}
				if (startedListIsReadFromAd)
				{
					if (dag.StartedMailboxServers.Contains(amServerName.Fqdn))
					{
						startedServers.Add(amServerName, server);
					}
				}
				else if (!flag)
				{
					startedServers.Add(amServerName, server);
				}
			}
			output.AppendLogMessage("Successfully resolved the servers based on the stopped servers list.", new object[0]);
			string text;
			if (startedListIsReadFromAd)
			{
				text = "The list is the StartedServers property of the DAG in AD";
			}
			else
			{
				text = "The list is all of the servers that are not in the StoppedServers list.";
			}
			output.AppendLogMessage("The following servers are in the StartedServers list ({0}):", new object[]
			{
				text
			});
			foreach (AmServerName amServerName2 in startedServers.Keys)
			{
				output.AppendLogMessage("\t{0}", new object[]
				{
					amServerName2.NetbiosName
				});
			}
			output.AppendLogMessage("The following servers are in the StoppedServers list:", new object[0]);
			foreach (AmServerName amServerName3 in stoppedServers.Keys)
			{
				output.AppendLogMessage("\t{0}", new object[]
				{
					amServerName3.NetbiosName
				});
			}
		}

		internal static Dictionary<AmServerName, Server> GetServersInSite(ILogTraceHelper output, IEnumerable<Server> servers, AdSiteIdParameter adSite)
		{
			if (servers == null)
			{
				throw new ArgumentNullException("servers");
			}
			if (adSite == null)
			{
				throw new ArgumentNullException("adSite");
			}
			Dictionary<AmServerName, Server> dictionary = new Dictionary<AmServerName, Server>(16);
			foreach (Server server in servers)
			{
				if (DatabaseAvailabilityGroupAction.SiteEquals(server.ServerSite, adSite))
				{
					dictionary.Add(new AmServerName(server.Fqdn), server);
				}
			}
			output.AppendLogMessage("The following servers are in the site {0}:", new object[]
			{
				adSite
			});
			foreach (AmServerName amServerName in dictionary.Keys)
			{
				output.AppendLogMessage("\t{0}", new object[]
				{
					amServerName.NetbiosName
				});
			}
			return dictionary;
		}

		internal static MultiValuedProperty<string> ServerListToFqdnList(IEnumerable<AmServerName> servers)
		{
			if (servers == null)
			{
				throw new ArgumentNullException("servers");
			}
			IEnumerable<string> source = from server in servers
			select server.Fqdn;
			return new MultiValuedProperty<string>(source.ToList<string>());
		}

		internal static List<ADObjectId> GetSitesForDag(DatabaseAvailabilityGroup dag)
		{
			if (dag == null)
			{
				throw new ArgumentNullException("dag");
			}
			HashSet<ADObjectId> hashSet = new HashSet<ADObjectId>();
			foreach (ADObjectId entryId in dag.Servers)
			{
				MiniServer miniServer = ((ITopologyConfigurationSession)dag.Session).ReadMiniServer(entryId, DatabaseAvailabilityGroupAction.PropertiesNeededFromServer);
				ADObjectId serverSite = miniServer.ServerSite;
				hashSet.Add(serverSite);
			}
			return hashSet.ToList<ADObjectId>();
		}

		internal static bool IsClusterUp(DatabaseAvailabilityGroup dag, HaTaskOutputHelper output)
		{
			try
			{
				using (AmCluster amCluster = AmCluster.OpenDagClus(dag))
				{
					using (IAmClusterGroup amClusterGroup = amCluster.FindCoreClusterGroup())
					{
						IEnumerable<AmServerName> source = amCluster.EnumerateNodeNames();
						output.AppendLogMessage("Cluster is up, there are {0} nodes in the cluster, group owner:{1}", new object[]
						{
							source.Count<AmServerName>(),
							amClusterGroup.OwnerNode.NetbiosName
						});
					}
				}
			}
			catch (AmCoreGroupRegNotFound amCoreGroupRegNotFound)
			{
				output.AppendLogMessage("Cluster is down, got AmCoreGroupRegNotFound:{0} when trying to open the cluster", new object[]
				{
					amCoreGroupRegNotFound.ToString()
				});
				return false;
			}
			return true;
		}

		internal static void EvictStoppedNodes(DatabaseAvailabilityGroup dag, IEnumerable<Server> stoppedServers, HaTaskOutputHelper output)
		{
			if (dag == null)
			{
				throw new ArgumentNullException("dag");
			}
			if (stoppedServers == null)
			{
				throw new ArgumentNullException("stoppedServers");
			}
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			string error = null;
			List<string> list = new List<string>(1);
			string verboseLog = null;
			using (AmCluster amCluster = AmCluster.OpenDagClus(dag))
			{
				using (IAmClusterGroup amClusterGroup = amCluster.FindCoreClusterGroup())
				{
					output.AppendLogMessage("EvictStoppedNodes has been called. Dumping current cluster state.", new object[0]);
					try
					{
						using (DumpClusterTopology dumpClusterTopology = new DumpClusterTopology(amCluster, output))
						{
							dumpClusterTopology.Dump();
						}
					}
					catch (ClusterException ex)
					{
						output.AppendLogMessage("DumpClusterTopology( {0} ) failed with exception = {1}. This is OK.", new object[]
						{
							dag.Name,
							ex.Message
						});
						output.AppendLogMessage("Ignoring previous error, as it is acceptable if the cluster does not exist yet.", new object[0]);
					}
					IEnumerable<AmServerName> source = amCluster.EnumerateNodeNames();
					AmServerName ownerNode = amClusterGroup.OwnerNode;
					int num = stoppedServers.Count<Server>();
					foreach (Server server in stoppedServers)
					{
						AmServerName amServerName = new AmServerName(server);
						if (source.Contains(amServerName))
						{
							output.AppendLogMessage("Server '{0}' is still a node in the cluster, and will have to be evicted.", new object[]
							{
								amServerName.NetbiosName
							});
							try
							{
								try
								{
									output.WriteProgressIncrementalSimple(Strings.ProgressForceCleanupNode(server.Name), 20 / num);
									output.AppendLogMessage("Running the eviction operation by issuing an RPC to the replay service on '{0}'...", new object[]
									{
										ownerNode.Fqdn
									});
									ReplayRpcClientWrapper.RunEvictNodeFromCluster(ownerNode, amServerName, out verboseLog);
								}
								finally
								{
									DagTaskHelper.LogRemoteVerboseLog(output, ownerNode.Fqdn, verboseLog);
								}
							}
							catch (DagTaskOperationFailedException ex2)
							{
								output.AppendLogMessage("An exception was thrown! ex={0}", new object[]
								{
									ex2.Message
								});
								Exception ex3;
								if (ex2.TryGetInnerExceptionOfType(out ex3))
								{
									output.AppendLogMessage("Ignore it. It was AmClusterEvictWithoutCleanupException, which is acceptable. It could be completed with cluster node /forcecleanp, but that isn't necessary.", new object[0]);
								}
								else if (ex2.TryGetInnerExceptionOfType(out ex3))
								{
									output.AppendLogMessage("That exception is fine. It means that the server has already been evicted from the cluster.", new object[0]);
								}
								else
								{
									error = ex2.Message;
									output.WriteWarning(Strings.FailedToEvictNode(server.Name, dag.Name, error));
									list.Add(server.Name);
								}
							}
							catch (LocalizedException ex4)
							{
								error = ex4.Message;
								output.WriteWarning(Strings.FailedToEvictNode(server.Name, dag.Name, error));
								list.Add(server.Name);
							}
						}
						else
						{
							output.AppendLogMessage("Server '{0}' is not in the cluster anymore. It must have already been evicted.", new object[]
							{
								amServerName.NetbiosName
							});
						}
					}
				}
			}
			if (list.Count != 0)
			{
				output.WriteErrorSimple(new FailedToEvictNodeException(string.Join(",", list.ToArray<string>()), dag.Name, error));
			}
		}

		internal static void ForceCleanupStoppedNodes(DatabaseAvailabilityGroup dag, IEnumerable<Server> shouldStopServers, TimeSpan maxTimeToWaitForOneNode, HaTaskOutputHelper output)
		{
			if (dag == null)
			{
				throw new ArgumentNullException("dag");
			}
			if (shouldStopServers == null)
			{
				throw new ArgumentNullException("shouldStopServers");
			}
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			string empty = string.Empty;
			List<string> list = new List<string>(1);
			int num = shouldStopServers.Count<Server>();
			foreach (Server server in shouldStopServers)
			{
				new AmServerName(server);
				output.WriteProgressIncrementalSimple(Strings.ProgressEvictNode(server.Name), 20 / num);
				if (!DatabaseAvailabilityGroupAction.ForceCleanupOneNodeLocally(dag.Name, server, maxTimeToWaitForOneNode, output))
				{
					list.Add(server.Name);
				}
			}
			if (list.Count != 0)
			{
				output.WriteErrorSimple(new StopDagFailedException(string.Join(",", list.ToArray<string>()), dag.Name));
			}
		}

		internal static bool ForceCleanupOneNodeLocally(string dagName, Server nodeToForceCleanup, TimeSpan maxTimeToWait, HaTaskOutputHelper output)
		{
			if (dagName == null)
			{
				throw new ArgumentNullException("dagName");
			}
			if (nodeToForceCleanup == null)
			{
				throw new ArgumentNullException("nodeToForceCleanup");
			}
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			bool result = false;
			string verboseLog = string.Empty;
			AmServerName serverName = new AmServerName(nodeToForceCleanup);
			output.AppendLogMessage("Attempting to run cluster node <LocalNodeName> /forcecleanup on {0}...", new object[]
			{
				serverName.NetbiosName
			});
			string error = null;
			System.Threading.Tasks.Task task = System.Threading.Tasks.Task.Factory.StartNew(delegate()
			{
				try
				{
					ReplayRpcClientWrapper.RunForceCleanupNode(serverName, out verboseLog);
				}
				catch (LocalizedException ex)
				{
					error = ex.Message;
				}
			});
			if (!task.Wait(maxTimeToWait))
			{
				error = string.Format("The operation didn't complete in {0} seconds", maxTimeToWait.TotalSeconds);
				output.WriteWarning(Strings.FailedToForceCleanupNode(nodeToForceCleanup.Name, dagName, error));
			}
			else
			{
				DagTaskHelper.LogRemoteVerboseLog(output, serverName.Fqdn, verboseLog);
				if (!string.IsNullOrEmpty(error))
				{
					output.WriteWarning(Strings.FailedToForceCleanupNode(nodeToForceCleanup.Name, dagName, error));
				}
				else
				{
					result = true;
				}
			}
			return result;
		}

		internal bool ForceADReplication()
		{
			Server server = this.m_adSession.FindLocalServer();
			if (server == null)
			{
				return false;
			}
			ADObjectId localServerSite = server.ServerSite;
			ADObjectId[] array = (from s in DatabaseAvailabilityGroupAction.GetSitesForDag(this.m_dag)
			where s != null && !s.Equals(localServerSite)
			select s).Distinct<ADObjectId>().ToArray<ADObjectId>();
			if (array != null && array.Length > 0)
			{
				this.m_output.AppendLogMessage("forcing AD replication to site {0}", new object[]
				{
					array[0].Name
				});
				DagTaskHelper.ForceReplication(this.m_adSession, this.m_dag, array, this.m_dag.Name, new Microsoft.Exchange.Configuration.Tasks.Task.TaskWarningLoggingDelegate(this.WriteWarning), new Microsoft.Exchange.Configuration.Tasks.Task.TaskVerboseLoggingDelegate(base.WriteVerbose));
				this.m_output.AppendLogMessage("forcing replication succeeded", new object[0]);
				return true;
			}
			return false;
		}

		internal static void JoinOneNode(AmServerName pamServer, AmServerName serverName, HaTaskOutputHelper output)
		{
			string verboseLog = null;
			try
			{
				output.AppendLogMessage("joining {0}", new object[]
				{
					serverName.NetbiosName
				});
				ReplayRpcClientWrapper.RunAddNodeToCluster(pamServer, serverName, out verboseLog);
			}
			catch (DagTaskServerException ex)
			{
				Exception ex2;
				if (!ex.TryGetInnerExceptionOfType(out ex2) && !ex.TryGetInnerExceptionOfType(out ex2))
				{
					throw;
				}
				DagTaskHelper.LogRemoteVerboseLog(output, pamServer.Fqdn, verboseLog);
				output.AppendLogMessage("{0} is probably just starting up, retry add after 1 minute", new object[]
				{
					serverName.NetbiosName
				});
				Thread.Sleep(DatabaseAvailabilityGroupAction.WaitBetweenOps);
				ReplayRpcClientWrapper.RunAddNodeToCluster(pamServer, serverName, out verboseLog);
			}
			finally
			{
				DagTaskHelper.LogRemoteVerboseLog(output, pamServer.Fqdn, verboseLog);
			}
		}

		internal static void TryStartClussvcOnNode(AmServerName serverName, HaTaskOutputHelper output)
		{
			using (ServiceController serviceController = new ServiceController("clussvc", serverName.Fqdn))
			{
				try
				{
					if (DatabaseAvailabilityGroupAction.GetServiceControllerStatus(serviceController, serverName.NetbiosName, output) == ServiceControllerStatus.StopPending)
					{
						output.AppendLogMessage("Service is in stop pending, wait for it to be stopped", new object[0]);
						serviceController.WaitForStatus(ServiceControllerStatus.Stopped, DatabaseAvailabilityGroupAction.ServiceTimeout);
					}
					if (DatabaseAvailabilityGroupAction.GetServiceControllerStatus(serviceController, serverName.NetbiosName, output) == ServiceControllerStatus.Stopped)
					{
						serviceController.Start();
						output.WriteProgressSimple(Strings.WaitingForClusterServiceToStart(serverName.NetbiosName));
						serviceController.WaitForStatus(ServiceControllerStatus.Running, DatabaseAvailabilityGroupAction.ServiceTimeout);
					}
				}
				catch (System.ServiceProcess.TimeoutException)
				{
					output.WriteErrorSimple(new FailedToStartClusSvcException(serverName.NetbiosName, DatabaseAvailabilityGroupAction.GetServiceControllerStatus(serviceController, serverName.NetbiosName, output).ToString()));
				}
			}
		}

		internal static void JoinForceCleanupNode(AmServerName pamServer, AmServerName serverName, HaTaskOutputHelper output)
		{
			string verboseLog = null;
			output.AppendLogMessage("{0} is probably cleaned up, try to evict it and join it back", new object[]
			{
				serverName.NetbiosName
			});
			try
			{
				ReplayRpcClientWrapper.RunEvictNodeFromCluster(pamServer, serverName, out verboseLog);
				DagTaskHelper.LogRemoteVerboseLog(output, pamServer.Fqdn, verboseLog);
				output.AppendLogMessage("Sleep one minute before we issue add", new object[0]);
				Thread.Sleep(DatabaseAvailabilityGroupAction.WaitBetweenOps);
				output.AppendLogMessage("joining {0}", new object[]
				{
					serverName.NetbiosName
				});
				ReplayRpcClientWrapper.RunAddNodeToCluster(pamServer, serverName, out verboseLog);
			}
			finally
			{
				DagTaskHelper.LogRemoteVerboseLog(output, pamServer.Fqdn, verboseLog);
			}
		}

		internal static ServiceControllerStatus GetServiceControllerStatus(ServiceController sc, string serverName, HaTaskOutputHelper output)
		{
			try
			{
				return sc.Status;
			}
			catch (Win32Exception ex)
			{
				output.AppendLogMessage("Failed to get status for server: {0} error {1}", new object[]
				{
					serverName,
					ex.Message
				});
				output.WriteErrorSimple(new FailedToGetServiceStatusForNodeException(serverName, ex.Message));
			}
			catch (InvalidOperationException ex2)
			{
				output.AppendLogMessage("Failed to get status for server: {0} error {1}", new object[]
				{
					serverName,
					ex2.Message
				});
				output.WriteErrorSimple(new FailedToGetServiceStatusForNodeException(serverName, ex2.Message));
			}
			return ServiceControllerStatus.StopPending;
		}

		internal static void FixIPAddress(AmServerName nodeName, DatabaseAvailabilityGroup dag, IEnumerable<AmServerName> startedMailboxServers, ITaskOutputHelper output)
		{
			output.WriteProgressSimple(Strings.DagTaskFixingUpIpResources);
			MultiValuedProperty<IPAddress> databaseAvailabilityGroupIpv4Addresses = dag.DatabaseAvailabilityGroupIpv4Addresses;
			IPAddress[] array = new IPAddress[0];
			if (databaseAvailabilityGroupIpv4Addresses.Count > 0)
			{
				array = databaseAvailabilityGroupIpv4Addresses.ToArray();
			}
			string[] value = (from addr in array
			select addr.ToString()).ToArray<string>();
			output.AppendLogMessage("Got the following IP addresses for the DAG (blank means DHCP):{0}", new object[]
			{
				string.Join(",", value)
			});
			using (AmCluster amCluster = AmCluster.OpenByNames(startedMailboxServers))
			{
				if (amCluster.CnoName != string.Empty)
				{
					using (IAmClusterGroup amClusterGroup = amCluster.FindCoreClusterGroup())
					{
						using (IAmClusterResource amClusterResource = amClusterGroup.FindResourceByTypeName("Network Name"))
						{
							LocalizedString value2 = AmClusterResourceHelper.FixUpIpAddressesForNetName(output, amCluster, (AmClusterGroup)amClusterGroup, (AmClusterResource)amClusterResource, array);
							output.WriteProgressSimple(Strings.DagTaskFixedUpIpResources(value2));
							DagTaskHelper.LogCnoState(output, dag.Name, amClusterResource);
						}
					}
				}
			}
		}

		protected override void InternalEndProcessing()
		{
			TaskLogger.LogEnter();
			if (this.m_output != null)
			{
				this.m_output.CloseTempLogFile();
			}
			TaskLogger.LogExit();
		}

		protected override IConfigDataProvider CreateSession()
		{
			return DirectorySessionFactory.Default.CreateTopologyConfigurationSession(base.DomainController, false, ConsistencyMode.FullyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 983, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\Cluster\\DatabaseAvailabilityGroupAction.cs");
		}

		internal static bool SubnetEquals(IPAddress addr1, IPAddress addr2, IPAddress subnet)
		{
			if (addr1.AddressFamily != addr2.AddressFamily)
			{
				return false;
			}
			IPAddress ipaddress;
			AddDatabaseAvailabilityGroupServer.ConvertUnicastIpAddressToClusterNetworkAddress(addr1, subnet, out ipaddress);
			IPAddress obj;
			AddDatabaseAvailabilityGroupServer.ConvertUnicastIpAddressToClusterNetworkAddress(addr2, subnet, out obj);
			return ipaddress.Equals(obj);
		}

		internal static bool SiteEquals(ADObjectId siteId, ADIdParameter inputSite)
		{
			return siteId != null && inputSite != null && (siteId.Name.Equals(inputSite.ToString(), StringComparison.OrdinalIgnoreCase) || siteId.ToString().Equals(inputSite.ToString(), StringComparison.OrdinalIgnoreCase) || (inputSite.InternalADObjectId != null && inputSite.InternalADObjectId.Equals(siteId)));
		}

		private const string Clussvc = "clussvc";

		internal static readonly ADPropertyDefinition[] PropertiesNeededFromServer = new ADPropertyDefinition[]
		{
			ServerSchema.Fqdn,
			ServerSchema.ServerSite,
			ServerSchema.IsMailboxServer,
			ServerSchema.VersionNumber
		};

		protected AdSiteIdParameter m_otherSiteId;

		internal HaTaskOutputHelper m_output;

		internal Dictionary<AmServerName, Server> m_serversInDag = new Dictionary<AmServerName, Server>(16);

		internal Dictionary<AmServerName, Server> m_startedServers = new Dictionary<AmServerName, Server>(16);

		internal Dictionary<AmServerName, Server> m_stoppedServers = new Dictionary<AmServerName, Server>(16);

		protected Server m_mailboxServer;

		internal ITopologyConfigurationSession m_adSession;

		protected DatabaseAvailabilityGroup m_dag;

		private static readonly TimeSpan ServiceTimeout = new TimeSpan(0, 1, 0);

		private static readonly TimeSpan WaitBetweenOps = new TimeSpan(0, 1, 0);
	}
}
