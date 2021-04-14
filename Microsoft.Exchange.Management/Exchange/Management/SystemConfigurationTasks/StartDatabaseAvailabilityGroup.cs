using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Microsoft.Exchange.Cluster.ClusApi;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Start", "DatabaseAvailabilityGroup", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class StartDatabaseAvailabilityGroup : DatabaseAvailabilityGroupAction
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				if (base.ActiveDirectorySite == null)
				{
					return Strings.ConfirmationMessageStartDatabaseAvailabilityGroupServer(this.m_dag.Name, base.MailboxServer.ToString());
				}
				return Strings.ConfirmationMessageStartDatabaseAvailabilityGroupADSite(this.m_dag.Name, base.ActiveDirectorySite.ToString());
			}
		}

		protected override string TaskName
		{
			get
			{
				return "start-databaseavailabilitygroup";
			}
		}

		private void LogCommandLineParameters()
		{
			string[] parametersToLog = new string[]
			{
				"Identity",
				"MailboxServer",
				"ActiveDirectorySite",
				"ConfigurationOnly",
				"QuorumOnly",
				"WhatIf"
			};
			DagTaskHelper.LogCommandLineParameters(this.m_output, base.MyInvocation.Line, parametersToLog, base.Fields);
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			this.LogCommandLineParameters();
		}

		protected override void InternalProcessRecord()
		{
			Dictionary<AmServerName, Server> dictionary = new Dictionary<AmServerName, Server>(16);
			if (this.m_mailboxServer != null)
			{
				AmServerName serverName = new AmServerName(this.m_mailboxServer.Fqdn);
				if (!this.m_serversInDag.Keys.Any((AmServerName serverInDag) => serverInDag.Equals(serverName)))
				{
					this.m_output.WriteErrorSimple(new DagTaskServerIsNotInDagException(serverName.Fqdn, this.m_dag.Name));
				}
				dictionary.Add(serverName, this.m_mailboxServer);
				if (base.NeedToUpdateAD)
				{
					if (this.m_stoppedServers.ContainsKey(serverName))
					{
						this.m_stoppedServers.Remove(serverName);
						this.m_output.AppendLogMessage("{0} removed from stopped list", new object[]
						{
							serverName.NetbiosName
						});
					}
					if (!this.m_startedServers.ContainsKey(serverName))
					{
						this.m_startedServers.Add(serverName, this.m_mailboxServer);
						this.m_output.AppendLogMessage("{0} added to started list", new object[]
						{
							serverName.NetbiosName
						});
					}
				}
			}
			else
			{
				dictionary = DatabaseAvailabilityGroupAction.GetServersInSite(this.m_output, this.m_serversInDag.Values, base.ActiveDirectorySite);
				if (base.NeedToUpdateAD)
				{
					foreach (AmServerName amServerName in dictionary.Keys)
					{
						if (this.m_stoppedServers.ContainsKey(amServerName))
						{
							this.m_output.AppendLogMessage("Remove {0} from stopped list", new object[]
							{
								amServerName.NetbiosName
							});
							this.m_stoppedServers.Remove(amServerName);
						}
						if (!this.m_startedServers.ContainsKey(amServerName))
						{
							this.m_output.AppendLogMessage("Add {0} to started list", new object[]
							{
								amServerName.NetbiosName
							});
							this.m_startedServers.Add(amServerName, this.m_serversInDag[amServerName]);
						}
					}
				}
			}
			if (base.NeedToUpdateAD)
			{
				this.m_output.WriteProgressSimple(Strings.ProgressStopUpdateAD);
				this.m_dag.StoppedMailboxServers = DatabaseAvailabilityGroupAction.ServerListToFqdnList(this.m_stoppedServers.Keys);
				this.m_adSession.Save(this.m_dag);
				this.m_output.AppendLogMessage("Updated stopped list with :", new object[0]);
				foreach (AmServerName amServerName2 in this.m_stoppedServers.Keys)
				{
					this.m_output.AppendLogMessage("\t{0}", new object[]
					{
						amServerName2.NetbiosName
					});
				}
				this.m_output.WriteProgressSimple(Strings.ProgressStopUpdateOtherAD);
				base.ForceADReplication();
			}
			List<string> errorServers = null;
			if (base.NeedToUpdateCluster)
			{
				errorServers = this.JoinStartedNodes(dictionary.Values);
				if (base.NeedToUpdateAD)
				{
					IEnumerable<AmServerName> enumerable = new List<AmServerName>(1);
					enumerable = from server in this.m_startedServers.Keys
					where !errorServers.Contains(server.NetbiosName, StringComparer.OrdinalIgnoreCase)
					select server;
					this.m_output.WriteProgressSimple(Strings.ProgressStartUpdateAD);
					this.m_dag.StartedMailboxServers = DatabaseAvailabilityGroupAction.ServerListToFqdnList(enumerable);
					this.m_adSession.Save(this.m_dag);
					this.m_output.AppendLogMessage("Updated started list with :", new object[0]);
					foreach (AmServerName amServerName3 in enumerable)
					{
						this.m_output.AppendLogMessage("\t{0}", new object[]
						{
							amServerName3.NetbiosName
						});
					}
					this.m_output.WriteProgressSimple(Strings.ProgressStartUpdateOtherAD);
					base.ForceADReplication();
					if (errorServers.Count != 0)
					{
						this.m_output.WriteErrorSimple(new FailedToStartNodeException(string.Join(",", errorServers.ToArray<string>()), this.m_dag.Name));
					}
				}
				this.SetAutomountConsensusOnStartedServers(dictionary);
			}
			base.InternalProcessRecord();
			this.m_output.WriteProgressSimple(Strings.ProgressTaskComplete);
		}

		private void SetAutomountConsensusOnStartedServers(Dictionary<AmServerName, Server> serversToStart)
		{
			TimeSpan timeout = TimeSpan.FromSeconds(17.0);
			foreach (AmServerName amServerName in serversToStart.Keys)
			{
				this.m_output.AppendLogMessage("notify replayservice on {0} with consensus state 1", new object[]
				{
					amServerName.NetbiosName
				});
				AmRpcClientHelper.RpcchSetAutomountConsensusStateBestEffort(amServerName.Fqdn, 1, timeout);
			}
		}

		private List<string> JoinStartedNodes(IEnumerable<Server> serversToStart)
		{
			if (serversToStart == null)
			{
				throw new ArgumentNullException("serversToStart");
			}
			List<string> list = new List<string>(1);
			using (AmCluster amCluster = AmCluster.OpenDagClus(this.m_dag))
			{
				using (IAmClusterGroup amClusterGroup = amCluster.FindCoreClusterGroup())
				{
					try
					{
						using (DumpClusterTopology dumpClusterTopology = new DumpClusterTopology(amCluster, this.m_output))
						{
							dumpClusterTopology.Dump();
						}
					}
					catch (ClusterException ex)
					{
						this.m_output.AppendLogMessage("DumpClusterTopology( {0} ) failed with exception = {1}. This is OK.", new object[]
						{
							this.m_dag.Name,
							ex.Message
						});
						this.m_output.AppendLogMessage("Ignoring previous error, as it is acceptable if the cluster does not exist yet.", new object[0]);
					}
					AmServerName ownerNode = amClusterGroup.OwnerNode;
					DatabaseTasksHelper.CheckReplayServiceRunningOnNode(ownerNode, new Task.TaskErrorLoggingDelegate(base.WriteError));
					if (amCluster.CnoName != string.Empty)
					{
						using (IAmClusterResource amClusterResource = amClusterGroup.FindResourceByTypeName("Network Name"))
						{
							DagTaskHelper.LogCnoState(this.m_output, this.m_dag.Name, amClusterResource);
						}
					}
					foreach (Server server in serversToStart)
					{
						bool flag = false;
						AmServerName amServerName = new AmServerName(server);
						try
						{
							if (amCluster.IsEvictedBasedOnMemberShip(amServerName))
							{
								this.m_output.WriteProgressSimple(Strings.ProgressJoinNode(amServerName.NetbiosName));
								this.m_output.AppendLogMessage("ForceCleanup the Node {0}", new object[]
								{
									server.Name
								});
								DatabaseAvailabilityGroupAction.ForceCleanupOneNodeLocally(this.m_dag.Name, server, TimeSpan.FromSeconds(15.0), this.m_output);
								this.m_output.AppendLogMessage("Join the node {0} to the cluster", new object[]
								{
									server.Name
								});
								DatabaseAvailabilityGroupAction.JoinOneNode(ownerNode, amServerName, this.m_output);
								flag = true;
							}
							else
							{
								this.m_output.AppendLogMessage("{0} is not evicted", new object[]
								{
									amServerName
								});
							}
							if (!AmCluster.IsRunning(amServerName))
							{
								try
								{
									this.m_output.AppendLogMessage("{0} cluster service is not running, try to start the service", new object[]
									{
										server.Name
									});
									this.m_output.WriteProgressSimple(Strings.ProgressStartClussvc(amServerName.NetbiosName));
									DatabaseAvailabilityGroupAction.TryStartClussvcOnNode(amServerName, this.m_output);
								}
								catch (InvalidOperationException)
								{
									this.m_output.AppendLogMessage("Got an invalidOperationException, most likely the node {0} is force cleanedup", new object[]
									{
										amServerName
									});
									if (flag)
									{
										this.m_output.AppendLogMessage("STRANGE! we joined {0} but cannot start the service!", new object[]
										{
											amServerName
										});
										throw;
									}
									this.m_output.WriteProgressSimple(Strings.ProgressJoinNode(amServerName.NetbiosName));
									DatabaseAvailabilityGroupAction.JoinForceCleanupNode(ownerNode, amServerName, this.m_output);
									flag = true;
								}
							}
							if (flag)
							{
								ICollection<AmServerName> startedMailboxServers = from server1 in this.m_dag.StartedMailboxServers
								select new AmServerName(server1);
								DatabaseAvailabilityGroupAction.FixIPAddress(new AmServerName(server.Fqdn), this.m_dag, startedMailboxServers, this.m_output);
							}
						}
						catch (LocalizedException ex2)
						{
							this.m_output.WriteWarning(Strings.FailedToJoinNode(server.Name, this.m_dag.Name, ex2.Message));
							list.Add(server.Name);
						}
						catch (InvalidOperationException ex3)
						{
							this.m_output.WriteWarning(Strings.FailedToJoinNode(server.Name, this.m_dag.Name, ex3.Message));
							list.Add(server.Name);
						}
					}
				}
			}
			return list;
		}
	}
}
