using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Management.Automation;
using System.Security.Principal;
using System.ServiceProcess;
using Microsoft.Exchange.Cluster.ActiveManagerServer;
using Microsoft.Exchange.Cluster.ClusApi;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Restore", "DatabaseAvailabilityGroup", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High, DefaultParameterSetName = "Identity")]
	public sealed class RestoreDatabaseAvailabilityGroup : SystemConfigurationObjectActionTask<DatabaseAvailabilityGroupIdParameter, DatabaseAvailabilityGroup>
	{
		[Parameter(Mandatory = false)]
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

		[Parameter(Mandatory = false)]
		public SwitchParameter UsePrimaryWitnessServer
		{
			get
			{
				return (SwitchParameter)(base.Fields["UsePrimaryWitnessServer"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["UsePrimaryWitnessServer"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public FileShareWitnessServerName AlternateWitnessServer
		{
			get
			{
				return (FileShareWitnessServerName)base.Fields["AlternateWitnessServer"];
			}
			set
			{
				base.Fields["AlternateWitnessServer"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public NonRootLocalLongFullPath AlternateWitnessDirectory
		{
			get
			{
				return (NonRootLocalLongFullPath)base.Fields["AlternateWitnessDirectory"];
			}
			set
			{
				base.Fields["AlternateWitnessDirectory"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				if (this.ActiveDirectorySite != null)
				{
					return Strings.ConfirmationMessageRestoreDatabaseAvailabilityGroupADSite(this.m_dag.Name, this.ActiveDirectorySite.ToString());
				}
				return Strings.ConfirmationMessageRestoreDatabaseAvailabilityGroup(this.m_dag.Name);
			}
		}

		private void PrepareServersInDagIfRequired()
		{
			if (this.m_serverNamesInDag == null)
			{
				this.m_serverNamesInDag = new HashSet<AmServerName>();
				if (this.m_dag != null && this.m_dag.Servers != null)
				{
					foreach (ADObjectId serverId in this.m_dag.Servers)
					{
						this.m_serverNamesInDag.Add(new AmServerName(serverId));
					}
				}
			}
		}

		private void CheckFsw(string fswComputerNameFqdn)
		{
			this.m_output.AppendLogMessage("Checking that the file share witness server ({0}) isn't one of the servers in the DAG.", new object[]
			{
				fswComputerNameFqdn
			});
			this.PrepareServersInDagIfRequired();
			AmServerName amServerName = new AmServerName(fswComputerNameFqdn);
			if (this.m_serverNamesInDag.Contains(amServerName))
			{
				this.m_output.WriteErrorSimple(new DagTaskServerMailboxServerAlsoServesAsFswNodeException(fswComputerNameFqdn));
			}
			this.m_output.AppendLogMessage("Checking if the FSW server can be queried with WMI.", new object[0]);
			DateTime bootTime = AmHelper.GetBootTime(amServerName);
			this.m_output.AppendLogMessage("The boot time of the FSW was '{0}' (MaxValue if the call failed).", new object[]
			{
				bootTime
			});
			if (bootTime == DateTime.MaxValue)
			{
				this.m_output.WriteErrorSimple(new DagTaskServerFswServerNotAccessibleWithWmiException(amServerName.NetbiosName));
			}
		}

		private void LogCommandLineParameters()
		{
			string[] parametersToLog = new string[]
			{
				"Identity",
				"ActiveDirectorySite",
				"AlternateWitnessServer",
				"AlternateWitnessDirectory",
				"DomainController",
				"WhatIf"
			};
			DagTaskHelper.LogCommandLineParameters(this.m_output, base.MyInvocation.Line, parametersToLog, base.Fields);
		}

		protected override void InternalValidate()
		{
			this.m_output = new HaTaskOutputHelper("restore-databaseavailabilitygroup", new Task.TaskErrorLoggingDelegate(base.WriteError), new Task.TaskWarningLoggingDelegate(this.WriteWarning), new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.TaskProgressLoggingDelegate(base.WriteProgress), this.GetHashCode());
			this.m_output.CreateTempLogFile();
			this.LogCommandLineParameters();
			this.m_output.WriteProgressIncrementalSimple(Strings.ProgressValidation, 5);
			this.m_dag = DagTaskHelper.DagIdParameterToDag(this.Identity, base.DataSession);
			DagTaskHelper.VerifyDagAndServersAreWithinScopes<DatabaseAvailabilityGroup>(this, this.m_dag, true);
			if (this.m_dag.DatacenterActivationMode != DatacenterActivationModeOption.DagOnly)
			{
				this.m_output.WriteErrorSimple(new TaskCanOnlyRunOnDacException(this.m_dag.Name));
			}
			DatabaseAvailabilityGroupAction.ResolveServersBasedOnStoppedList(this.m_output, this.m_dag, this.m_servers, this.m_startedServers, this.m_stoppedServers);
			if (this.ActiveDirectorySite != null)
			{
				List<ADObjectId> sitesForDag = DatabaseAvailabilityGroupAction.GetSitesForDag(this.m_dag);
				ADObjectId adobjectId = null;
				foreach (ADObjectId adobjectId2 in sitesForDag)
				{
					if (DatabaseAvailabilityGroupAction.SiteEquals(adobjectId2, this.ActiveDirectorySite))
					{
						adobjectId = adobjectId2;
					}
				}
				if (adobjectId == null)
				{
					this.m_output.WriteErrorSimple(new InvalidAdSiteException(this.ActiveDirectorySite.ToString()));
				}
			}
			this.m_isQuorumPresent = this.CheckClussvcRunningOnStartedServers();
			if (this.UsePrimaryWitnessServer)
			{
				if (this.m_dag.WitnessServer == null)
				{
					this.m_output.WriteErrorSimple(new RestoreNeedsWitnessServerException(this.m_dag.Name));
				}
			}
			else if (this.m_dag.AlternateWitnessServer == null && this.AlternateWitnessServer == null)
			{
				this.m_output.WriteErrorSimple(new RestoreNeedsAlternateWitnessServerException(this.m_dag.Name));
			}
			base.InternalValidate();
		}

		private void ProcessFileShareWitness()
		{
			FileShareWitnessServerName fileShareWitnessServerName = this.AlternateWitnessServer ?? this.m_dag.AlternateWitnessServer;
			NonRootLocalLongFullPath nonRootLocalLongFullPath = this.AlternateWitnessDirectory ?? this.m_dag.AlternateWitnessDirectory;
			if (this.UsePrimaryWitnessServer)
			{
				fileShareWitnessServerName = this.m_dag.WitnessServer;
				nonRootLocalLongFullPath = this.m_dag.WitnessDirectory;
			}
			string serverName = (fileShareWitnessServerName != null) ? fileShareWitnessServerName.ToString() : Strings.CreateAltFswWillAutomaticallyCalculateLater;
			string filePath = (nonRootLocalLongFullPath != null) ? nonRootLocalLongFullPath.ToString() : Strings.CreateAltFswWillAutomaticallyCalculateLater;
			this.m_output.WriteProgressIncrementalSimple(Strings.ProgressCreateAltFsw(serverName, filePath), 10);
			this.m_afsw = new FileShareWitness((ITopologyConfigurationSession)base.DataSession, this.m_dag.Name, fileShareWitnessServerName, nonRootLocalLongFullPath);
			try
			{
				this.m_afsw.Initialize();
			}
			catch (LocalizedException error)
			{
				this.m_output.WriteErrorSimple(error);
			}
			this.CheckFsw(this.m_afsw.WitnessServerFqdn);
			if (SharedHelper.StringIEquals(this.m_afsw.WitnessServer.ToString(), this.m_dag.WitnessServer.ToString()) && this.m_dag.WitnessDirectory != this.m_afsw.WitnessDirectory)
			{
				this.m_output.WriteErrorSimple(new DagFswAndAlternateFswOnSameWitnessServerButPointToDifferentDirectoriesException(this.m_afsw.WitnessServer.ToString(), this.m_dag.WitnessDirectory.ToString(), this.m_afsw.WitnessDirectory.ToString()));
			}
			if (!this.UsePrimaryWitnessServer)
			{
				this.m_dag.SetAlternateWitnessServer(this.m_afsw.FileShareWitnessShare, this.m_afsw.WitnessDirectory);
			}
			else
			{
				this.m_dag.SetWitnessServer(this.m_afsw.FileShareWitnessShare, this.m_afsw.WitnessDirectory);
			}
			base.DataSession.Save(this.m_dag);
			try
			{
				this.m_output.AppendLogMessage("Creating the file share of the FSW...", new object[0]);
				this.m_afsw.Create();
				if (this.m_dag.Servers.Count == 0 && this.m_afsw.IsJustCreated)
				{
					this.m_output.AppendLogMessage("Because there are no servers in the DAG, and the file share witness was just created, then the permissions may be incorrect (the CNO account might not yet exist). Deleting the FSW.", new object[0]);
					this.m_afsw.Delete();
				}
			}
			catch (LocalizedException ex)
			{
				this.m_output.AppendLogMessage("There was an exception encountered during creation and/or deletion of the FSW ('{0}'). If it was an error during deletion, then we'll just ignore it.", new object[]
				{
					ex.Message
				});
				if (this.m_afsw.GetExceptionType(ex) != FileShareWitnessExceptionType.FswDeleteError)
				{
					this.m_output.WriteWarning(ex.LocalizedString);
				}
				else
				{
					this.m_output.AppendLogMessage("Yes, it was a an error related to deleting the FSW. Continuing on.", new object[0]);
				}
			}
		}

		protected override bool IsKnownException(Exception exception)
		{
			return exception is IdentityNotMappedException || exception is AmClusterException || exception is ADTransientException || exception is ADExternalException || exception is ADOperationException || exception is AmServerException || exception is ObjectNotFoundException || AmExceptionHelper.IsKnownClusterException(this, exception) || base.IsKnownException(exception);
		}

		protected override void InternalProcessRecord()
		{
			this.m_output.WriteProgressIncrementalSimple(Strings.ProgressForceQuorum, 10);
			if (!this.m_isQuorumPresent && !this.ForceQuorumIfNecessary())
			{
				this.m_output.WriteErrorSimple(new DagTaskQuorumNotAchievedException(this.m_dag.Name));
			}
			this.m_output.WriteProgressIncrementalSimple(Strings.ProgressChangeFsw, 20);
			DatabaseAvailabilityGroupAction.EvictStoppedNodes(this.m_dag, this.m_stoppedServers.Values, this.m_output);
			this.ProcessFileShareWitness();
			IEnumerable<AmServerName> amServerNamesFromServers = RestoreDatabaseAvailabilityGroup.GetAmServerNamesFromServers(this.m_startedServers.Values);
			using (AmCluster amCluster = AmCluster.OpenByNames(amServerNamesFromServers))
			{
				bool shouldBeFsw = DagTaskHelper.ShouldBeFileShareWitness(this.m_output, this.m_dag, amCluster, false);
				DagTaskHelper.ChangeQuorumToMnsOrFswAsAppropriate(this.m_output, this, this.m_dag, amCluster, this.m_afsw, this.m_afsw, shouldBeFsw, !this.UsePrimaryWitnessServer);
				this.DeleteLastLogGenerationTimeStamps(amCluster, this.m_stoppedServers.Values, this.m_output);
			}
			this.m_output.WriteProgressIncrementalSimple(Strings.ProgressTaskComplete, 100);
			base.InternalProcessRecord();
		}

		internal static string[] GetNodeNamesFromServers(Dictionary<string, Server> servers)
		{
			if (servers == null)
			{
				throw new ArgumentNullException("servers");
			}
			return (from server in servers.Values
			select server.Name).ToArray<string>();
		}

		internal static IEnumerable<AmServerName> GetAmServerNamesFromServers(IEnumerable<Server> servers)
		{
			if (servers == null)
			{
				throw new ArgumentNullException("servers");
			}
			return (from server in servers
			select new AmServerName(server.Fqdn)).ToList<AmServerName>();
		}

		private void DeleteLastLogGenerationTimeStamps(AmCluster cluster, IEnumerable<Server> stoppedServers, HaTaskOutputHelper output)
		{
			output.AppendLogMessage("Deleting last log generation time stamps of all databases on stopped servers...", new object[0]);
			List<Database> list = new List<Database>(100);
			foreach (Server server in stoppedServers)
			{
				output.AppendLogMessage("Finding all databases on stopped server '{0}'...", new object[]
				{
					server.Fqdn
				});
				new AmServerName(server);
				Database[] databases = server.GetDatabases();
				list.AddRange(databases);
			}
			if (list.Count == 0)
			{
				output.AppendLogMessage("No databases were found on the stopped servers. Skipping time stamp deletion.", new object[0]);
				return;
			}
			output.AppendLogMessage("Found {0} databases on the stopped servers.", new object[]
			{
				list.Count
			});
			ClusterBatchLastLogGenDeleter clusterBatchLastLogGenDeleter = new ClusterBatchLastLogGenDeleter(cluster, list, output);
			clusterBatchLastLogGenDeleter.DeleteTimeStamps();
			output.AppendLogMessage("Finished deleting last log generation time stamps on stopped servers.", new object[0]);
		}

		private AmServerName GetCurrentActiveServer(Guid dbGuid)
		{
			ActiveManager noncachingActiveManagerInstance = ActiveManager.GetNoncachingActiveManagerInstance();
			DatabaseLocationInfo serverForDatabase = noncachingActiveManagerInstance.GetServerForDatabase(dbGuid);
			return new AmServerName(serverForDatabase.ServerFqdn);
		}

		private bool ForceQuorumIfNecessary()
		{
			bool flag = true;
			bool result = false;
			foreach (Server server in this.m_startedServers.Values)
			{
				this.m_output.AppendLogMessage("ForceQuorumIfNecessary: Checking if clussvc is running on {0}...", new object[]
				{
					server.Name
				});
				using (ServiceController serviceController = new ServiceController("clussvc", server.Name))
				{
					ServiceControllerStatus serviceControllerStatus = DatabaseAvailabilityGroupAction.GetServiceControllerStatus(serviceController, server.Name, this.m_output);
					if (serviceControllerStatus == ServiceControllerStatus.Stopped)
					{
						this.m_output.AppendLogMessage("ForceQuorum: clussvc is stopped on node {0}. Starting it.", new object[]
						{
							server.Name
						});
						try
						{
							if (flag)
							{
								this.m_output.AppendLogMessage("Starting cluster service with /fq (force quorum).", new object[0]);
								serviceController.Start(new string[]
								{
									"/fq"
								});
								flag = false;
							}
							else
							{
								this.m_output.AppendLogMessage("Starting cluster service in normal mode.", new object[0]);
								serviceController.Start();
							}
							try
							{
								this.m_output.AppendLogMessage("Waiting up to {0} for clussvc to be in the Running state.", new object[]
								{
									RestoreDatabaseAvailabilityGroup.ServiceTimeOut
								});
								serviceController.WaitForStatus(ServiceControllerStatus.Running, RestoreDatabaseAvailabilityGroup.ServiceTimeOut);
								result = true;
							}
							catch (System.ServiceProcess.TimeoutException)
							{
								this.m_output.WriteErrorSimple(new FailedToStartClusSvcException(server.Name, serviceControllerStatus.ToString()));
							}
							continue;
						}
						catch (Win32Exception ex)
						{
							this.m_output.AppendLogMessage("ForceQuorum: cluster service on {0}Failed to start with fq {1}", new object[]
							{
								server.Name,
								ex.Message
							});
							this.m_output.WriteErrorSimple(new FailedToStartClusSvcException(server.Name, serviceControllerStatus.ToString()));
							continue;
						}
						catch (InvalidOperationException ex2)
						{
							this.m_output.AppendLogMessage("ForceQuorum: cluster service on {0}Failed to start with fq {1}", new object[]
							{
								server.Name,
								ex2.Message
							});
							this.m_output.WriteErrorSimple(new FailedToStartClusSvcException(server.Name, serviceControllerStatus.ToString()));
							continue;
						}
					}
					this.m_output.AppendLogMessage("ForceQuorum: cluster service on {0} is in state {1}, we will try another node (if there is one) for forcing quorum.", new object[]
					{
						server.Name,
						serviceControllerStatus
					});
					flag = false;
				}
			}
			return result;
		}

		private bool CheckClussvcRunningOnStartedServers()
		{
			bool result = false;
			List<AmServerName> list = new List<AmServerName>(16);
			List<AmServerName> list2 = new List<AmServerName>(16);
			new List<AmServerName>(16);
			List<string> list3 = new List<string>(16);
			foreach (Server server in this.m_startedServers.Values)
			{
				this.m_output.AppendLogMessage("Checking if clussvc is running on {0}...", new object[]
				{
					server.Name
				});
				using (ServiceController serviceController = new ServiceController("clussvc", server.Name))
				{
					ServiceControllerStatus serviceControllerStatus = DatabaseAvailabilityGroupAction.GetServiceControllerStatus(serviceController, server.Name, this.m_output);
					this.m_output.AppendLogMessage("Clussvc is {0} on {1}.", new object[]
					{
						serviceControllerStatus,
						server.Name
					});
					if (serviceControllerStatus == ServiceControllerStatus.Running)
					{
						list.Add(new AmServerName(server));
					}
					else
					{
						list2.Add(new AmServerName(server));
					}
				}
			}
			if (list2.Count == this.m_startedServers.Values.Count)
			{
				this.m_output.AppendLogMessage("None of the {0} servers in the started list had clussvc running, so it is safe to continue with restore-dag, but forcequorum will be necessary.", new object[]
				{
					list2.Count
				});
			}
			else
			{
				int num = 0;
				List<AmServerName> list4 = new List<AmServerName>(1);
				try
				{
					using (AmCluster amCluster = AmCluster.OpenByNames(list))
					{
						IEnumerable<IAmClusterNode> enumerable = amCluster.EnumerateNodes();
						foreach (IAmClusterNode amClusterNode in enumerable)
						{
							AmNodeState state = amClusterNode.State;
							this.m_output.AppendLogMessage("The node {0} has cluster state = {1}.", new object[]
							{
								amClusterNode.Name,
								state
							});
							if (state == AmNodeState.Joining)
							{
								this.m_output.AppendLogMessage("  Node {0} is joining! That means there is some difficulty establishing quorum.", new object[]
								{
									amClusterNode.Name
								});
								list3.Add(amClusterNode.Name.NetbiosName);
							}
							else if (AmClusterNode.IsNodeUp(state))
							{
								num++;
							}
							if (state == AmNodeState.Paused)
							{
								list4.Add(amClusterNode.Name);
							}
						}
						foreach (IAmClusterNode amClusterNode2 in enumerable)
						{
							amClusterNode2.Dispose();
						}
					}
				}
				catch (ClusterException)
				{
					this.m_output.WriteErrorSimple(new DagTaskRestoreDagCouldNotOpenClusterException());
				}
				this.m_output.AppendLogMessage("There were {0} total (Stopped&Started) servers that were Up, out of {1} servers in the Started list that had clussvc running.", new object[]
				{
					num,
					list.Count
				});
				if (num == list.Count)
				{
					this.m_output.AppendLogMessage("Because the two values are the same, that means quorum is healthy (even if not all of the servers are Up), so it is safe to continue with restore-dag.", new object[0]);
					result = true;
				}
				else if (num > list.Count)
				{
					this.m_output.AppendLogMessage("Because there are more servers that are up than there are in the Started list, it is safe to continue with restore-dag.", new object[0]);
					result = true;
				}
				else
				{
					this.m_output.AppendLogMessage("Because the two values are different, that means quorum is contested/in flux, so it is NOT safe to continue with restore-dag.", new object[0]);
					this.m_output.WriteErrorSimple(new RestoreFailedDagUpException(string.Join(",", list3.ToArray())));
				}
			}
			return result;
		}

		protected override void InternalEndProcessing()
		{
			TaskLogger.LogEnter();
			if (this.m_output != null)
			{
				this.m_output.CloseTempLogFile();
			}
			TaskLogger.LogExit();
			base.InternalEndProcessing();
		}

		private FileShareWitness m_afsw;

		private HashSet<AmServerName> m_serverNamesInDag;

		private HaTaskOutputHelper m_output;

		private Dictionary<AmServerName, Server> m_servers = new Dictionary<AmServerName, Server>(16);

		private Dictionary<AmServerName, Server> m_startedServers = new Dictionary<AmServerName, Server>(16);

		private Dictionary<AmServerName, Server> m_stoppedServers = new Dictionary<AmServerName, Server>(16);

		private DatabaseAvailabilityGroup m_dag;

		private static readonly TimeSpan ServiceTimeOut = new TimeSpan(0, 1, 0);

		private bool m_isQuorumPresent;
	}
}
