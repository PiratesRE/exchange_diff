using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Management.Automation;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
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
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Monitoring;
using Microsoft.Exchange.Rpc.ActiveManager;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Add", "DatabaseAvailabilityGroupServer", SupportsShouldProcess = true)]
	public sealed class AddDatabaseAvailabilityGroupServer : SystemConfigurationObjectActionTask<DatabaseAvailabilityGroupIdParameter, DatabaseAvailabilityGroup>, IDisposable
	{
		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = true, ParameterSetName = "Identity", ValueFromPipelineByPropertyName = true, ValueFromPipeline = true, Position = 1)]
		public ServerIdParameter MailboxServer
		{
			get
			{
				return (ServerIdParameter)base.Fields["MailboxServer"];
			}
			set
			{
				base.Fields["MailboxServer"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter SkipDagValidation
		{
			get
			{
				return (SwitchParameter)(base.Fields["SkipDagValidation"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["SkipDagValidation"] = value;
			}
		}

		protected override bool IsKnownException(Exception e)
		{
			return AmExceptionHelper.IsKnownClusterException(this, e) || base.IsKnownException(e);
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageAddDatabaseAvailabilityGroupServer(this.m_mailboxServerName, this.m_dagName);
			}
		}

		public AddDatabaseAvailabilityGroupServer()
		{
			this.m_serversInDag = new List<Server>(8);
			this.m_output = new HaTaskOutputHelper("add-databaseavailabiltygroupserver", new Task.TaskErrorLoggingDelegate(base.WriteError), new Task.TaskWarningLoggingDelegate(this.WriteWarning), new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.TaskProgressLoggingDelegate(base.WriteProgress), this.GetHashCode());
			this.m_output.CreateTempLogFile();
			this.m_output.AppendLogMessage("add-dagserver started", new object[0]);
		}

		private void FetchOrMakeUpSecretClusterName()
		{
			this.CheckServerDagClusterMembership();
			if (this.m_clusDag == null)
			{
				this.m_secretClusterName = this.m_dagName;
				return;
			}
			this.m_secretClusterName = this.m_clusDag.Name;
		}

		private void ResolveParameters()
		{
			this.m_output.WriteProgressSimple(Strings.DagTaskValidatingParameters);
			this.m_dag = DagTaskHelper.DagIdParameterToDag(this.Identity, this.ConfigurationSession);
			this.m_dagName = this.m_dag.Name;
			this.m_mailboxServer = (Server)base.GetDataObject<Server>(this.MailboxServer, base.DataSession, null, new LocalizedString?(Strings.ErrorServerNotFound(this.MailboxServer.ToString())), new LocalizedString?(Strings.ErrorServerNotUnique(this.MailboxServer.ToString())));
			this.m_skipDagValidation = this.SkipDagValidation;
			DagTaskHelper.ServerToMailboxServer(new Task.TaskErrorLoggingDelegate(this.m_output.WriteError), this.m_mailboxServer);
			this.m_mailboxAmServerName = new AmServerName(this.m_mailboxServer.Fqdn);
			this.m_mailboxServerName = this.m_mailboxServer.Name;
			this.m_output.AppendLogMessage("Mailbox server: value passed in = {0}, mailboxServer.Name = {1}, mailboxServer.Fqdn = {2}", new object[]
			{
				this.MailboxServer,
				this.m_mailboxServerName,
				this.m_mailboxAmServerName.Fqdn
			});
			if (!this.m_mailboxServer.IsMailboxServer)
			{
				this.m_output.WriteErrorSimple(new OperationOnlyOnMailboxServerException(this.m_mailboxServer.Name));
			}
			if (this.m_mailboxServer.MajorVersion != Server.CurrentExchangeMajorVersion)
			{
				this.m_output.WriteErrorSimple(new DagTaskErrorServerWrongVersion(this.m_mailboxServer.Name));
			}
			DagTaskHelper.LogClussvcState(this.m_output, this.m_mailboxAmServerName);
			MultiValuedProperty<IPAddress> databaseAvailabilityGroupIpv4Addresses = this.m_dag.DatabaseAvailabilityGroupIpv4Addresses;
			IPAddress[] array = new IPAddress[0];
			if (databaseAvailabilityGroupIpv4Addresses.Count > 0)
			{
				array = databaseAvailabilityGroupIpv4Addresses.ToArray();
			}
			string[] value = (from addr in array
			select addr.ToString()).ToArray<string>();
			this.m_output.AppendLogMessage("The IP addresses for the DAG are (blank means DHCP): {0}", new object[]
			{
				string.Join(",", value)
			});
			if (array != null && array.Length > 0)
			{
				this.m_staticAddresses = array;
				DagTaskHelper.ValidateIPAddressList(this.m_output, this.m_mailboxAmServerName.NetbiosName, this.m_staticAddresses, this.m_dag.Name);
				DagTaskHelper.ValidateIPAddressList(this.m_output, this.m_mailboxAmServerName.Fqdn, this.m_staticAddresses, this.m_dag.Name);
				AmClusterResourceHelper.ValidateIpv4Addresses(null, this.m_staticAddresses);
			}
			DagTaskHelper.LogMachineIpAddresses(this.m_output, this.m_dagName);
			DagTaskHelper.LogMachineIpAddresses(this.m_output, this.m_mailboxAmServerName.NetbiosName);
			DagTaskHelper.LogMachineIpAddresses(this.m_output, this.m_mailboxAmServerName.Fqdn);
		}

		private void CheckThereIsNoClusterNamed(string dagName)
		{
			ITopologyConfigurationSession configSession = (ITopologyConfigurationSession)base.DataSession;
			bool flag2;
			bool flag = DagTaskHelper.DoesComputerAccountExist(configSession, dagName, out flag2);
			if (!flag)
			{
				this.DagTrace("The computer account {0} does not exist.", new object[]
				{
					dagName
				});
				return;
			}
			if (flag2)
			{
				this.DagTrace("The computer account {0} exists and is enabled.", new object[]
				{
					dagName
				});
				try
				{
					using (AmCluster.OpenByName(new AmServerName(dagName)))
					{
						this.m_output.WriteErrorSimple(new DagTaskClusterWithDagNameIsSquattingException(dagName));
					}
				}
				catch (ClusterException)
				{
					this.DagTrace("As expected, GetRemoteCluster( {0} ) was unable to open a cluster.", new object[]
					{
						dagName
					});
				}
				this.m_output.WriteErrorSimple(new DagTaskComputerAccountExistsAndIsEnabledException(dagName));
				return;
			}
			this.DagTrace("The computer account {0} exists, but is disabled.", new object[]
			{
				dagName
			});
		}

		private void FindClusterForDag()
		{
			AmCluster amCluster = null;
			this.m_output.WriteProgressSimple(Strings.DagTaskCheckingClusterMembershipOfServer(this.m_mailboxServerName));
			IEnumerable<AmServerName> enumerable = from server in this.m_serversInDag
			where !SharedHelper.StringIEquals(server.Fqdn, this.m_mailboxAmServerName.Fqdn)
			select new AmServerName(server.Fqdn);
			try
			{
				try
				{
					amCluster = AmCluster.OpenByName(this.m_mailboxAmServerName);
					this.DagTrace("Mailbox server '{0}' is already a member of cluster '{1}'.", new object[]
					{
						this.m_mailboxServerName,
						amCluster.Name
					});
					this.m_clusteringConfiguredOnServer = true;
					this.m_clusteringInstalledOnServer = true;
				}
				catch (ClusterException ex)
				{
					this.DagTrace("GetRemoteCluster() for the mailbox server failed with exception = {0}. This is OK.", new object[]
					{
						ex.Message
					});
					this.DagTrace("Ignoring previous error, as it is acceptable if the cluster does not exist yet.");
					this.m_clusteringConfiguredOnServer = false;
				}
				try
				{
					bool flag = true;
					try
					{
						new AmServerName(this.m_dagName);
					}
					catch (AmGetFqdnFailedNotFoundException)
					{
						flag = false;
						this.DagTrace("Unable to find an FQDN for {0}.", new object[]
						{
							this.m_dagName
						});
					}
					if (flag)
					{
						int num = 0;
						using (DumpClusterTopology dumpClusterTopology = new DumpClusterTopology(this.m_dagName, this.m_output))
						{
							num = dumpClusterTopology.Dump();
						}
						if (num == 53 && DagHelper.IsLocalNodeClustered())
						{
							using (DumpClusterTopology dumpClusterTopology2 = new DumpClusterTopology(string.Empty, this.m_output))
							{
								dumpClusterTopology2.Dump();
							}
						}
					}
				}
				catch (ClusterException ex2)
				{
					this.DagTrace("DumpClusterTopology( {0} ) failed with exception = {1}. This is OK.", new object[]
					{
						this.m_dagName,
						ex2.Message
					});
					this.DagTrace("Ignoring previous error, as it is acceptable if the cluster does not exist yet.");
				}
				IEnumerable<string> source = from serverName in enumerable
				select serverName.NetbiosName;
				if (enumerable.Count<AmServerName>() > 0)
				{
					try
					{
						this.DagTrace("Opening the cluster on nodes [{0}].", new object[]
						{
							string.Join(", ", source.ToArray<string>())
						});
						this.m_clusDag = AmCluster.OpenByNames(enumerable);
						this.DagTrace("Other mailbox servers in the DAG are already members of cluster '{0}'", new object[]
						{
							this.m_clusDag.Name
						});
					}
					catch (ClusterException ex3)
					{
						this.DagTrace("GetRemoteCluster() failed possibly with error {0}, ex = {1}", new object[]
						{
							LocalizedException.GenerateErrorCode(ex3.InnerException).ToString(),
							ex3
						});
						this.m_clusteringConfiguredOnServer = false;
					}
				}
				if (this.m_clusDag == null && enumerable.Count<AmServerName>() != 0)
				{
					this.m_output.WriteErrorSimple(new DagTaskServerCanNotContactClusterException(enumerable.Count<AmServerName>(), string.Join(", ", source.ToArray<string>())));
				}
				if (amCluster == null)
				{
					if (this.m_clusDag == null)
					{
						this.DagTrace("The new server ({0}) is not a member of a cluster, nor are the other servers (if there are any other servers).", new object[]
						{
							this.m_mailboxServerName
						});
						this.CheckThereIsNoClusterNamed(this.m_dagName);
					}
					else
					{
						this.DagTrace("The server {0} does not belong to a cluster, and the other servers belong to {1}.", new object[]
						{
							this.m_mailboxServerName,
							this.m_clusDag.Name
						});
					}
				}
				else if (this.m_clusDag == null)
				{
					this.DagTrace("The server {0} does belong to a cluster ({1}), but there are no other servers in the DAG yet.", new object[]
					{
						this.m_mailboxServerName,
						amCluster.Name
					});
					this.m_clusDag = amCluster;
					amCluster = null;
				}
				else
				{
					if (SharedHelper.StringIEquals(this.m_clusDag.Name, amCluster.Name))
					{
						this.DagTrace("The server {0} belongs to the same cluster as the other servers ({1}).", new object[]
						{
							this.m_mailboxServerName,
							this.m_clusDag.Name
						});
					}
					else
					{
						this.m_output.WriteErrorSimple(new AddDagServerMailboxServerIsInDifferentClusterException(this.m_mailboxServerName, amCluster.Name, this.m_clusDag.Name));
					}
					if (!SharedHelper.StringIEquals(this.m_clusDag.Name, this.m_dagName))
					{
						this.m_output.WriteErrorSimple(new DagTaskClusterNameIsNotDagNameException(this.m_mailboxServerName, this.m_clusDag.Name, this.m_dagName));
					}
				}
			}
			finally
			{
				if (amCluster != null)
				{
					amCluster.Dispose();
					amCluster = null;
				}
			}
			if (!this.m_skipDagValidation && this.m_clusDag != null)
			{
				DagTaskHelper.ValidateDagClusterMembership(this.m_output, this.m_dag, this.m_clusDag, this.m_mailboxAmServerName);
			}
		}

		private void DetermineClusterStateOfRemoteMachine()
		{
			AmNodeClusterState amNodeClusterState = AmNodeClusterState.NotInstalled;
			int nodeClusterState = ClusapiMethods.GetNodeClusterState(this.m_mailboxAmServerName.Fqdn, ref amNodeClusterState);
			if (nodeClusterState != 0)
			{
				this.DagTrace("GetNodeClusterState( {0} ) failed with {1}. Continuing anyway (assuming clustering is still installed though).", new object[]
				{
					this.m_mailboxServerName,
					nodeClusterState
				});
				this.m_clusteringInstalledOnServer = true;
				this.m_clusteringConfiguredOnServer = false;
				return;
			}
			this.DagTrace("According to GetNodeClusterState(), the server {0} is {1}.", new object[]
			{
				this.m_mailboxServerName,
				amNodeClusterState
			});
			AmNodeClusterState amNodeClusterState2 = amNodeClusterState;
			switch (amNodeClusterState2)
			{
			case AmNodeClusterState.NotInstalled:
				this.m_clusteringInstalledOnServer = false;
				this.m_clusteringConfiguredOnServer = false;
				return;
			case AmNodeClusterState.NotConfigured:
				this.m_clusteringInstalledOnServer = true;
				this.m_clusteringConfiguredOnServer = false;
				return;
			case (AmNodeClusterState)2:
				goto IL_C0;
			case AmNodeClusterState.NotRunning:
				break;
			default:
				if (amNodeClusterState2 != AmNodeClusterState.Running)
				{
					goto IL_C0;
				}
				break;
			}
			this.m_clusteringInstalledOnServer = true;
			this.m_clusteringConfiguredOnServer = true;
			return;
			IL_C0:
			this.m_clusteringInstalledOnServer = true;
			this.m_clusteringConfiguredOnServer = false;
		}

		private void CheckServerDagClusterMembership()
		{
			this.FindClusterForDag();
			if (this.m_serversInDag.Count == 0 || (this.m_serversInDag.Count == 1 && SharedHelper.StringIEquals(this.m_serversInDag[0].Name, this.m_mailboxAmServerName.NetbiosName)))
			{
				if (this.m_clusteringInstalledOnServer && this.m_clusteringConfiguredOnServer)
				{
					if (this.m_clusDag == null)
					{
						this.m_output.WriteErrorSimple(new DagTaskClusteringShouldBeDisabledException(this.m_mailboxServerName));
					}
				}
				else if (this.m_clusDag != null)
				{
					this.m_output.WriteErrorSimple(new DagTaskClusteringShouldBeEnabledException(this.m_mailboxServerName));
				}
			}
			if (this.m_clusteringInstalledOnServer)
			{
				if (this.m_clusteringConfiguredOnServer)
				{
					this.m_joinCluster = false;
					try
					{
						using (this.m_clusDag.OpenNode(this.m_mailboxAmServerName))
						{
						}
						goto IL_EF;
					}
					catch (ClusterException)
					{
						this.m_output.WriteErrorSimple(new NewDagServerIsAlreadyManuallyConfiguredForClusteringButIsNotInDagException(this.m_mailboxServerName, this.m_dagName));
						goto IL_EF;
					}
				}
				this.m_joinCluster = true;
			}
			IL_EF:
			ADObjectId databaseAvailabilityGroup = this.m_mailboxServer.DatabaseAvailabilityGroup;
			if (databaseAvailabilityGroup != null)
			{
				DatabaseAvailabilityGroup databaseAvailabilityGroup2 = (DatabaseAvailabilityGroup)base.DataSession.Read<DatabaseAvailabilityGroup>(this.m_mailboxServer.DatabaseAvailabilityGroup);
				if (!SharedHelper.StringIEquals(databaseAvailabilityGroup2.Name, this.m_dag.Name))
				{
					this.m_output.WriteErrorSimple(new NewDagServerIsAlreadyInDifferentDagException(this.m_mailboxServerName, databaseAvailabilityGroup2.Name, this.m_dagName));
				}
			}
		}

		private void CheckServerDagAdSettings()
		{
			if (this.m_dag.Servers.Count >= 16)
			{
				this.m_output.WriteErrorSimple(new DagTaskErrorTooManyServers(this.m_dagName, 16));
			}
			this.DagTrace("DAG {0} has {1} servers:", new object[]
			{
				this.m_dagName,
				this.m_dag.Servers.Count
			});
			foreach (ADObjectId identity in this.m_dag.Servers)
			{
				Server server = (Server)base.DataSession.Read<Server>(identity);
				this.DagTrace("DAG {0} contains server {1}.", new object[]
				{
					this.m_dagName,
					server.Name
				});
				this.m_serversInDag.Add(server);
			}
			ServerVersion adminDisplayVersion = this.m_mailboxServer.AdminDisplayVersion;
			if (adminDisplayVersion.Major > Server.Exchange2011MajorVersion || (adminDisplayVersion.Major == Server.Exchange2011MajorVersion && adminDisplayVersion.Minor >= 1))
			{
				foreach (Server server2 in this.m_serversInDag)
				{
					ServerVersion adminDisplayVersion2 = server2.AdminDisplayVersion;
					if (adminDisplayVersion2.Major < Server.Exchange2011MajorVersion || (adminDisplayVersion2.Major == Server.Exchange2011MajorVersion && adminDisplayVersion2.Minor == 0))
					{
						this.m_output.WriteErrorSimple(new DagTaskServerDifferentExchangeVersionException(this.m_dagName, server2.Name, adminDisplayVersion2, this.m_mailboxServerName, adminDisplayVersion));
					}
				}
			}
			if (adminDisplayVersion.Major < Server.Exchange2011MajorVersion || (adminDisplayVersion.Major == Server.Exchange2011MajorVersion && adminDisplayVersion.Minor == 0))
			{
				foreach (Server server3 in this.m_serversInDag)
				{
					ServerVersion adminDisplayVersion3 = server3.AdminDisplayVersion;
					if (adminDisplayVersion3.Major > Server.Exchange2011MajorVersion || (adminDisplayVersion3.Major == Server.Exchange2011MajorVersion && adminDisplayVersion3.Minor >= 1))
					{
						this.m_output.WriteErrorSimple(new DagTaskServerDifferentExchangeVersionException(this.m_dagName, server3.Name, adminDisplayVersion3, this.m_mailboxServerName, adminDisplayVersion));
					}
				}
			}
		}

		private void CheckFswSettings()
		{
			this.m_fsw = new FileShareWitness((ITopologyConfigurationSession)base.DataSession, this.m_dag.Name, this.m_dag.WitnessServer, this.m_dag.WitnessDirectory, this.m_clusDag);
			try
			{
				this.m_fsw.Initialize();
			}
			catch (LocalizedException error)
			{
				this.m_output.WriteErrorSimple(error);
			}
			if (this.m_dag.AlternateWitnessServer != null && this.m_dag.AlternateWitnessDirectory != null)
			{
				this.m_afsw = new FileShareWitness((ITopologyConfigurationSession)base.DataSession, this.m_dag.Name, this.m_dag.AlternateWitnessServer, this.m_dag.AlternateWitnessDirectory, this.m_clusDag);
				try
				{
					this.m_afsw.Initialize();
				}
				catch (LocalizedException ex)
				{
					this.DagTrace("Unable to initialize Alternate Witness Server for dag {0} (AlternateWitnessServer: {1}, AlternateWitnessDirectory: {2}). Specific error: {3}", new object[]
					{
						this.m_dag.Name,
						this.m_dag.AlternateWitnessServer,
						this.m_dag.AlternateWitnessDirectory,
						ex
					});
				}
			}
		}

		private void InternalValidateClusterChecks()
		{
			this.DetermineClusterStateOfRemoteMachine();
			this.DetermineResourcesToCreate();
			this.FetchOrMakeUpSecretClusterName();
			this.CheckFswSettings();
		}

		private void DetermineResourcesToCreate()
		{
			this.m_createIPAddressResources = true;
			if (this.m_dag.DatabaseAvailabilityGroupIpAddresses != null && this.m_dag.DatabaseAvailabilityGroupIpAddresses.Count == 1 && IPAddress.None.Equals(this.m_dag.DatabaseAvailabilityGroupIpAddresses[0]))
			{
				this.m_createIPAddressResources = false;
			}
		}

		private void LogCommandLineParameters()
		{
			string[] parametersToLog = new string[]
			{
				"Identity",
				"MailboxServer",
				"DatabaseAvailabilityGroupIpAddresses",
				"WhatIf"
			};
			DagTaskHelper.LogCommandLineParameters(this.m_output, base.MyInvocation.Line, parametersToLog, base.Fields);
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			this.LogCommandLineParameters();
			try
			{
				this.ResolveParameters();
				base.VerifyIsWithinScopes((IConfigurationSession)base.DataSession, this.m_mailboxServer, true, new DataAccessTask<DatabaseAvailabilityGroup>.ADObjectOutOfScopeString(Strings.ErrorServerOutOfScope));
				DagTaskHelper.VerifyDagAndServersAreWithinScopes<DatabaseAvailabilityGroup>(this, this.m_dag, true);
				DagTaskHelper.CheckServerDoesNotBelongToDifferentDag(new Task.TaskErrorLoggingDelegate(this.m_output.WriteError), base.DataSession, this.m_mailboxServer, this.m_dagName);
				this.CheckServerDagAdSettings();
				this.InternalValidateClusterChecks();
				DatabaseTasksHelper.CheckReplayServiceRunningOnNode(this.m_mailboxAmServerName, new Task.TaskErrorLoggingDelegate(base.WriteError));
				if (this.m_joinCluster && this.m_clusDag != null)
				{
					using (IAmClusterGroup amClusterGroup = this.m_clusDag.FindCoreClusterGroup())
					{
						AmServerName ownerNode = amClusterGroup.OwnerNode;
						if (this.m_mailboxAmServerName != ownerNode)
						{
							DatabaseTasksHelper.CheckReplayServiceRunningOnNode(ownerNode, new Task.TaskErrorLoggingDelegate(base.WriteError));
						}
						if (this.m_clusDag.CnoName != string.Empty)
						{
							using (IAmClusterResource amClusterResource = amClusterGroup.FindResourceByTypeName("Network Name"))
							{
								DagTaskHelper.LogCnoState(this.m_output, this.m_dagName, amClusterResource);
							}
						}
					}
				}
				string serverName = this.m_fsw.WitnessServer.isFqdn ? this.m_fsw.WitnessServer.Fqdn : this.m_fsw.WitnessServer.HostName;
				AmServerName fswNodeName = new AmServerName(serverName);
				DagTaskHelper.CheckNodeIsNotFswNode(this.m_mailboxAmServerName, fswNodeName, new Task.TaskErrorLoggingDelegate(base.WriteError));
				base.InternalValidate();
			}
			catch (Exception ex)
			{
				this.DagTrace("InternalValidate() hit: {0}", new object[]
				{
					ex
				});
				throw;
			}
			this.DagTrace("InternalValidate() done.");
			TaskLogger.LogExit();
		}

		private void InstallFailoverClustering()
		{
			this.m_output.LastException = null;
			string verboseData = null;
			try
			{
				ReplayRpcClientWrapper.RunInstallFailoverClustering(this.m_mailboxAmServerName, out verboseData);
			}
			catch (LocalizedException error)
			{
				this.m_output.AppendLogMessage(ReplayStrings.DagTaskRemoteOperationLogBegin(this.m_mailboxServerName));
				this.m_output.AppendLogMessage(ReplayStrings.DagTaskRemoteOperationLogData(verboseData));
				this.m_output.WriteErrorSimple(error);
			}
			this.m_output.AppendLogMessage(ReplayStrings.DagTaskRemoteOperationLogBegin(this.m_mailboxServerName));
			this.m_output.AppendLogMessage(ReplayStrings.DagTaskRemoteOperationLogData(verboseData));
			this.m_output.AppendLogMessage(ReplayStrings.DagTaskRemoteOperationLogEnd(this.m_mailboxServerName));
		}

		private static uint NumberOfBitsSet(uint test)
		{
			uint num = 0U;
			while (test != 0U)
			{
				test &= test - 1U;
				num += 1U;
			}
			return num;
		}

		private static IPAddress GenerateNetmaskIpAddressFromNetmaskBits(uint bitsInNetMask)
		{
			byte[] array;
			if (bitsInNetMask > 32U)
			{
				array = new byte[16];
			}
			else
			{
				array = new byte[4];
			}
			int num = 0;
			while (num < array.Length && bitsInNetMask > 0U)
			{
				int num2 = Math.Min(8, (int)bitsInNetMask);
				bitsInNetMask -= (uint)num2;
				byte b = byte.MaxValue;
				b = (byte)(b >> 8 - num2);
				b = (byte)(b << 8 - num2);
				array[num] = b;
				num++;
			}
			return new IPAddress(array);
		}

		private static bool FindNetworkAndMaskForIpv4Address(HaTaskOutputHelper output, AmServerName serverName, IPAddress ipaddr, AddDagServerWmiIpInformation[] addrs, out string addrNetwork, out uint netmask)
		{
			addrNetwork = string.Empty;
			netmask = 0U;
			foreach (AddDagServerWmiIpInformation addDagServerWmiIpInformation in addrs)
			{
				IPAddress ipaddress = AddDatabaseAvailabilityGroupServer.GenerateNetmaskIpAddressFromNetmaskBits(addDagServerWmiIpInformation.m_netmask);
				if (AmClusterNetwork.IsIPInNetwork(ipaddr, addDagServerWmiIpInformation.m_ipAddress.ToString(), ipaddress.ToString()))
				{
					addrNetwork = addDagServerWmiIpInformation.m_ipAddress.ToString();
					netmask = addDagServerWmiIpInformation.m_netmask;
					output.AppendLogMessage("The address {0} falls under the network ({1}/{2}).", new object[]
					{
						ipaddr,
						addrNetwork,
						netmask
					});
					break;
				}
			}
			if (netmask == 0U)
			{
				output.AppendLogMessage("Could not find any suitable networks for the address {0}!", new object[]
				{
					ipaddr
				});
			}
			return netmask != 0U;
		}

		private static uint ConvertUnicastIpv6AddressToClusterNetworkAddress(IPAddress realAddr, out IPAddress clusterNetworkAddress)
		{
			byte[] addressBytes = realAddr.GetAddressBytes();
			byte[] array = new byte[realAddr.GetAddressBytes().Length];
			for (int i = 0; i < 8; i++)
			{
				array[i] = addressBytes[i];
			}
			clusterNetworkAddress = new IPAddress(array);
			return 64U;
		}

		private static uint GetNumberOfBitsSetInAddressByteArray(byte[] ipaddrAsBytes)
		{
			uint num = 0U;
			for (int i = 0; i < ipaddrAsBytes.Length; i++)
			{
				num += AddDatabaseAvailabilityGroupServer.NumberOfBitsSet((uint)ipaddrAsBytes[i]);
			}
			return num;
		}

		internal static uint ConvertUnicastIpAddressToClusterNetworkAddress(IPAddress realAddr, IPAddress netmaskAddr, out IPAddress clusterNetworkAddress)
		{
			if (realAddr.AddressFamily == AddressFamily.InterNetworkV6)
			{
				return AddDatabaseAvailabilityGroupServer.ConvertUnicastIpv6AddressToClusterNetworkAddress(realAddr, out clusterNetworkAddress);
			}
			if (netmaskAddr == null)
			{
				clusterNetworkAddress = null;
				return 0U;
			}
			byte[] addressBytes = realAddr.GetAddressBytes();
			byte[] addressBytes2 = netmaskAddr.GetAddressBytes();
			byte[] array = new byte[realAddr.GetAddressBytes().Length];
			if (addressBytes.Length != addressBytes2.Length)
			{
				clusterNetworkAddress = null;
				return 0U;
			}
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = (addressBytes[i] & addressBytes2[i]);
			}
			clusterNetworkAddress = new IPAddress(array);
			return AddDatabaseAvailabilityGroupServer.GetNumberOfBitsSetInAddressByteArray(addressBytes2);
		}

		private static void FetchNetworksForDhcpClustering(HaTaskOutputHelper output, AmServerName mailboxServerName, out string[] ipaddrs, out uint[] netmasks)
		{
			bool flag = false;
			List<string> list = new List<string>(4);
			List<uint> list2 = new List<uint>(4);
			AddDagServerWmiIpInformation[] array = AddDatabaseAvailabilityGroupServer.FetchNetworksUsingWmi(output, mailboxServerName, AddDatabaseAvailabilityGroupServer.RequireGateway.Yes);
			foreach (AddDagServerWmiIpInformation addDagServerWmiIpInformation in array)
			{
				list.Add(addDagServerWmiIpInformation.m_ipAddress.ToString());
				list2.Add(addDagServerWmiIpInformation.m_netmask);
				if (addDagServerWmiIpInformation.m_fDhcpEnabled)
				{
					flag = true;
				}
			}
			if (!flag)
			{
				output.AppendLogMessage(Strings.DagTaskNoNetworksRunningDhcp(mailboxServerName.NetbiosName));
			}
			ipaddrs = list.ToArray();
			netmasks = list2.ToArray();
		}

		private static void FetchNetworksForStaticAddresses(HaTaskOutputHelper output, AmServerName serverName, IPAddress[] staticAddresses, out string[] ipaddrs, out uint[] netmasks)
		{
			List<string> list = new List<string>(staticAddresses.Length);
			List<uint> list2 = new List<uint>(staticAddresses.Length);
			AddDagServerWmiIpInformation[] addrs = AddDatabaseAvailabilityGroupServer.FetchNetworksUsingWmi(output, serverName, AddDatabaseAvailabilityGroupServer.RequireGateway.No);
			for (int i = 0; i < staticAddresses.Length; i++)
			{
				uint num = 0U;
				if (staticAddresses[i].AddressFamily == AddressFamily.InterNetwork)
				{
					string text;
					if (!AddDatabaseAvailabilityGroupServer.FindNetworkAndMaskForIpv4Address(output, serverName, staticAddresses[i], addrs, out text, out num))
					{
						output.AppendLogMessage(Strings.DagTaskCouldNotFindMatchingNetwork(staticAddresses[i].ToString(), serverName.NetbiosName));
					}
				}
				else if (staticAddresses[i].AddressFamily == AddressFamily.InterNetworkV6)
				{
					num = 64U;
				}
				if (num != 0U)
				{
					list.Add(staticAddresses[i].ToString());
					list2.Add(num);
				}
			}
			if (list.Count == 0)
			{
				output.WriteErrorSimple(new DagTaskCouldNotFindAnyMatchingNetworkException(serverName.NetbiosName));
			}
			ipaddrs = list.ToArray();
			netmasks = list2.ToArray();
		}

		private static System.Management.ManagementScope GetManagementScope(AmServerName machineName)
		{
			ManagementPath path = new ManagementPath(string.Format("\\\\{0}\\root\\cimv2", machineName.Fqdn));
			AmServerName amServerName = new AmServerName(Environment.MachineName);
			ConnectionOptions connectionOptions = new ConnectionOptions();
			if (!amServerName.Equals(machineName))
			{
				connectionOptions.Authority = string.Format("Kerberos:host/{0}", machineName.Fqdn);
			}
			return new System.Management.ManagementScope(path, connectionOptions);
		}

		internal static AddDagServerWmiIpInformation[] FetchNetworksUsingWmi(HaTaskOutputHelper output, AmServerName machineName, AddDatabaseAvailabilityGroupServer.RequireGateway defaultGatewayRequired)
		{
			AddDagServerWmiIpInformation[] result = null;
			string query = "select * from Win32_NetworkAdapterConfiguration where IPEnabled = true";
			int num = -2147023174;
			int num2 = 0;
			output.AppendLogMessage("Connecting to server '{0}' via WMI...", new object[]
			{
				machineName
			});
			System.Management.ManagementScope managementScope = AddDatabaseAvailabilityGroupServer.GetManagementScope(machineName);
			using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(managementScope, new ObjectQuery(query)))
			{
				ManagementObjectCollection managementObjectCollection = null;
				try
				{
					managementObjectCollection = managementObjectSearcher.Get();
				}
				catch (COMException ex)
				{
					if (ex.ErrorCode == num)
					{
						output.WriteErrorSimple(new ServerNotAvailableException());
					}
					output.WriteErrorSimple(ex);
				}
				output.AppendLogMessage("Fetching the network adapters and {0} the ones without default gateways.", new object[]
				{
					(defaultGatewayRequired == AddDatabaseAvailabilityGroupServer.RequireGateway.Yes) ? "ignoring" : "including"
				});
				try
				{
					List<AddDagServerWmiIpInformation> list = new List<AddDagServerWmiIpInformation>(managementObjectCollection.Count);
					foreach (ManagementBaseObject managementBaseObject in managementObjectCollection)
					{
						ManagementObject managementObject = (ManagementObject)managementBaseObject;
						string text = (string)managementObject["Description"];
						bool dhcpEnabled = (bool)managementObject["DHCPEnabled"];
						string[] array = (string[])managementObject["IPAddress"];
						string[] array2 = (string[])managementObject["IPSubnet"];
						string[] array3 = (string[])managementObject["DefaultIPGateway"];
						if (array != null)
						{
							string text2 = "<none>";
							if (array3 != null)
							{
								num2++;
								text2 = string.Join(",", array3);
							}
							for (int i = 0; i < array.Length; i++)
							{
								IPAddress ipaddress = IPAddress.Parse(array[i]);
								uint num3 = AddDatabaseAvailabilityGroupServer.WmiSubnetStringToNetmask(output, array2[i]);
								bool flag = true;
								if (num3 == 0U)
								{
									flag = false;
								}
								else if (ipaddress.IsIPv6LinkLocal || IPAddress.IsLoopback(ipaddress))
								{
									flag = false;
								}
								if (defaultGatewayRequired == AddDatabaseAvailabilityGroupServer.RequireGateway.Yes && array3 == null)
								{
									flag = false;
								}
								output.AppendLogMessage("{0} has an address: {1}/{2} default gateway(s)={3} [{4}]", new object[]
								{
									machineName,
									ipaddress,
									num3,
									text2,
									flag ? "valid" : "invalid"
								});
								if (flag)
								{
									IPAddress netmaskAddr = null;
									if (ipaddress.AddressFamily == AddressFamily.InterNetwork)
									{
										netmaskAddr = IPAddress.Parse(array2[i]);
									}
									IPAddress ipaddress2;
									AddDatabaseAvailabilityGroupServer.ConvertUnicastIpAddressToClusterNetworkAddress(ipaddress, netmaskAddr, out ipaddress2);
									output.AppendLogMessage("IP address derived that's suitable for clustering DHCP/IPv6: {0}/{1}.", new object[]
									{
										ipaddress2,
										num3
									});
									list.Add(new AddDagServerWmiIpInformation(ipaddress2, num3, dhcpEnabled));
								}
							}
						}
					}
					result = list.ToArray();
				}
				catch (ManagementException ex2)
				{
					output.WriteError(new ReadNetworkAdapterInfoException(ex2.Message), ErrorCategory.ReadError, null);
				}
				finally
				{
					if (managementObjectCollection != null)
					{
						managementObjectCollection.Dispose();
					}
				}
			}
			if (num2 > 1)
			{
				output.WriteWarning(Strings.DagTaskMultipleDefaultGatewaysFound(machineName.NetbiosName));
			}
			return result;
		}

		private static uint WmiSubnetStringToNetmask(HaTaskOutputHelper output, string wmiSubnet)
		{
			uint result = 0U;
			Match match = AddDatabaseAvailabilityGroupServer.s_regexNetmask.Match(wmiSubnet);
			try
			{
				if (match.Success)
				{
					byte[] ipaddrAsBytes = new byte[]
					{
						byte.Parse(match.Groups[1].Value),
						byte.Parse(match.Groups[2].Value),
						byte.Parse(match.Groups[3].Value),
						byte.Parse(match.Groups[4].Value)
					};
					result = AddDatabaseAvailabilityGroupServer.GetNumberOfBitsSetInAddressByteArray(ipaddrAsBytes);
				}
				else
				{
					result = uint.Parse(wmiSubnet);
				}
			}
			catch (FormatException)
			{
				output.AppendLogMessage("The subnet '{0}' was not in a valid format.", new object[]
				{
					wmiSubnet
				});
				result = 0U;
			}
			catch (OverflowException)
			{
				output.AppendLogMessage("The subnet '{0}' was not in a valid format.", new object[]
				{
					wmiSubnet
				});
				result = 0U;
			}
			return result;
		}

		private void FormCluster()
		{
			if (this.m_clusDag != null)
			{
				this.m_output.WriteVerbose(Strings.DagTaskSkippingFormCluster(this.m_secretClusterName, this.m_dagName));
				this.DagTrace("Cluster named '{0}' for dag '{1}' already exists.", new object[]
				{
					this.m_secretClusterName,
					this.m_dagName
				});
				return;
			}
			string[] array;
			uint[] array2;
			if (this.m_createIPAddressResources)
			{
				if (this.m_staticAddresses == null || (this.m_staticAddresses.Length == 1 && IPAddress.Any.Equals(this.m_staticAddresses[0])))
				{
					AddDatabaseAvailabilityGroupServer.FetchNetworksForDhcpClustering(this.m_output, this.m_mailboxAmServerName, out array, out array2);
				}
				else
				{
					AddDatabaseAvailabilityGroupServer.FetchNetworksForStaticAddresses(this.m_output, this.m_mailboxAmServerName, this.m_staticAddresses, out array, out array2);
				}
			}
			else
			{
				array = new string[]
				{
					IPAddress.None.ToString()
				};
				uint[] array3 = new uint[1];
				array2 = array3;
			}
			string[] array4 = new string[array2.Length];
			for (int i = 0; i < array2.Length; i++)
			{
				array4[i] = array2[i].ToString();
			}
			this.m_output.WriteProgressSimple(ReplayStrings.DagTaskFormingClusterProgress(this.m_secretClusterName, this.m_mailboxServerName));
			this.m_output.AppendLogMessage(ReplayStrings.DagTaskFormingClusterToLog(this.m_secretClusterName, this.m_mailboxServerName, string.Join(",", array), string.Join(",", array4)));
			this.m_output.LastException = null;
			string verboseData = null;
			try
			{
				ReplayRpcClientWrapper.RunCreateCluster(this.m_mailboxAmServerName, this.m_secretClusterName, this.m_mailboxAmServerName, array, array2, out verboseData);
			}
			catch (LocalizedException error)
			{
				this.m_output.AppendLogMessage(ReplayStrings.DagTaskRemoteOperationLogBegin(this.m_mailboxServerName));
				this.m_output.AppendLogMessage(ReplayStrings.DagTaskRemoteOperationLogData(verboseData));
				this.m_output.WriteErrorSimple(error);
			}
			this.m_output.AppendLogMessage(ReplayStrings.DagTaskRemoteOperationLogBegin(this.m_mailboxServerName));
			this.m_output.AppendLogMessage(ReplayStrings.DagTaskRemoteOperationLogData(verboseData));
			this.m_output.AppendLogMessage(ReplayStrings.DagTaskRemoteOperationLogEnd(this.m_mailboxServerName));
			if (this.m_output.LastException != null)
			{
				this.m_output.WriteErrorSimple(this.m_output.LastException);
			}
			this.m_clusDag = AmCluster.OpenByName(this.m_mailboxAmServerName);
			this.CheckFswSettings();
			this.m_clusteringConfiguredOnServer = true;
			this.m_output.WriteProgressSimple(Strings.DagTaskFormedCluster);
		}

		private void JoinNodeToCluster()
		{
			this.m_output.WriteProgressSimple(Strings.DagTaskJoiningNodeToCluster(this.m_mailboxServerName));
			this.m_output.LastException = null;
			string verboseLog = null;
			string remoteServerName = "<unknown>";
			try
			{
				using (IAmClusterGroup amClusterGroup = this.m_clusDag.FindCoreClusterGroup())
				{
					AmServerName ownerNode = amClusterGroup.OwnerNode;
					remoteServerName = ownerNode.Fqdn;
					ReplayRpcClientWrapper.RunAddNodeToCluster(ownerNode, this.m_mailboxAmServerName, out verboseLog);
				}
			}
			catch (LocalizedException error)
			{
				DagTaskHelper.LogRemoteVerboseLog(this.m_output, remoteServerName, verboseLog);
				this.m_output.WriteErrorSimple(error);
			}
			DagTaskHelper.LogRemoteVerboseLog(this.m_output, remoteServerName, verboseLog);
			this.m_output.WriteProgressSimple(Strings.DagTaskJoinedNodeToCluster(this.m_mailboxServerName));
			try
			{
				using (this.m_clusDag.OpenNode(this.m_mailboxAmServerName))
				{
					this.m_output.AppendLogMessage("Successfully able to OpenNode('{0}')!", new object[]
					{
						this.m_mailboxAmServerName.NetbiosName
					});
				}
			}
			catch (ClusterException)
			{
				this.m_output.WriteErrorSimple(new DagTaskAddClusterNodeUnexpectedlyFailedException(this.m_mailboxServerName, this.m_dagName));
			}
			if (this.m_clusDag.CnoName != string.Empty)
			{
				this.m_output.WriteProgressSimple(Strings.DagTaskFixingUpIpResources);
				using (IAmClusterGroup amClusterGroup2 = this.m_clusDag.FindCoreClusterGroup())
				{
					using (IAmClusterResource amClusterResource = amClusterGroup2.FindResourceByTypeName("Network Name"))
					{
						this.m_output.AppendLogMessage("Cluster group net name = '{0}'.", new object[]
						{
							amClusterResource.Name
						});
						LocalizedString value = AmClusterResourceHelper.FixUpIpAddressesForNetName(this.m_output, this.m_clusDag, (AmClusterGroup)amClusterGroup2, (AmClusterResource)amClusterResource, this.m_staticAddresses);
						this.m_output.WriteProgressSimple(Strings.DagTaskFixedUpIpResources(value));
						DagTaskHelper.LogCnoState(this.m_output, this.m_dagName, amClusterResource);
					}
				}
			}
			DagTaskHelper.NotifyServerOfConfigChange(this.m_mailboxAmServerName);
		}

		protected override void InternalProcessRecord()
		{
			bool flag = false;
			TaskLogger.LogEnter();
			try
			{
				this.m_output.WriteProgressSimple(ReplayStrings.DagTaskAddingServerToDag(this.m_mailboxServerName, this.m_dagName));
				if (!this.m_clusteringInstalledOnServer)
				{
					this.m_output.WriteProgressSimple(ReplayStrings.DagTaskInstallingFailoverClustering(this.m_mailboxServerName));
					this.InstallFailoverClustering();
					this.m_output.WriteProgressSimple(ReplayStrings.DagTaskInstalledFailoverClustering);
					using (DumpClusterTopology dumpClusterTopology = new DumpClusterTopology(this.m_dagName, this.m_output))
					{
						dumpClusterTopology.Dump();
					}
					this.m_output.WriteProgressSimple(Strings.NewDagClusteringInstalledLaterRunningChecksLaterVerbose);
					this.InternalValidateClusterChecks();
				}
				if (this.m_clusDag == null)
				{
					this.FormCluster();
					flag = false;
					this.m_joinCluster = false;
					this.FollowBestPractices();
					this.m_output.WriteProgressSimple(Strings.DagTaskSleepAfterClusterFormation(45, this.m_dagName, this.m_mailboxServerName));
					Thread.Sleep(45000);
				}
				if (this.m_joinCluster)
				{
					this.JoinNodeToCluster();
					flag = DagTaskHelper.ShouldBeFileShareWitness(this.m_output, this.m_dag, this.m_clusDag, false);
				}
				this.m_output.WriteProgressSimple(Strings.DagTaskUpdatingAdDagMembership(this.m_mailboxServerName, this.m_dagName));
				base.InternalProcessRecord();
				this.UpdateAdSettings();
				this.m_output.WriteProgressSimple(Strings.DagTaskUpdatedAdDagMembership(this.m_mailboxServerName, this.m_dagName));
				if (flag)
				{
					try
					{
						this.m_fsw.Create();
					}
					catch (LocalizedException ex)
					{
						this.m_output.WriteWarning(ex.LocalizedString);
					}
					if (this.m_afsw != null && this.m_afsw.IsInitialized && !this.m_afsw.Equals(this.m_fsw))
					{
						try
						{
							this.m_afsw.Create();
						}
						catch (LocalizedException ex2)
						{
							this.m_output.WriteWarning(ex2.LocalizedString);
						}
					}
				}
				if (this.m_joinCluster)
				{
					bool flag2 = DagTaskHelper.IsQuorumTypeFileShareWitness(this.m_output, this.m_clusDag);
					this.m_output.AppendLogMessage("SkipDagValidation = {0}; IsQuorumTypeFSW = {1}; CreateFSW = {2}", new object[]
					{
						this.m_skipDagValidation,
						flag2,
						flag
					});
					if (!this.m_skipDagValidation || (flag && !flag2))
					{
						DagTaskHelper.ChangeQuorumToMnsOrFswAsAppropriate(this.m_output, this, this.m_dag, this.m_clusDag, this.m_fsw, this.m_afsw, flag, false);
					}
					else
					{
						this.m_output.AppendLogMessage("Skip setting Quorum type", new object[0]);
					}
				}
				if (this.m_clusDag != null)
				{
					this.m_clusDag.Dispose();
					this.m_clusDag = null;
				}
				if (AmRpcVersionControl.IsAmRefreshConfigurationSupported(this.m_mailboxServer.AdminDisplayVersion))
				{
					int maxSecondsToWait = 120;
					Exception ex3 = null;
					try
					{
						AmRpcClientHelper.AmRefreshConfiguration(this.m_mailboxAmServerName.Fqdn, AmRefreshConfigurationFlags.Wait, maxSecondsToWait);
					}
					catch (AmServerTransientException ex4)
					{
						ex3 = ex4;
					}
					catch (AmServerException ex5)
					{
						ex3 = ex5;
					}
					if (ex3 != null)
					{
						this.m_output.WriteErrorSimple(ex3);
					}
				}
				this.m_output.WriteProgressSimple(Strings.DagTaskAddedServerToDag(this.m_mailboxServerName, this.m_dagName));
			}
			catch (Exception ex6)
			{
				this.DagTrace("InternalProcessRecord() hit: {0}", new object[]
				{
					ex6
				});
				throw;
			}
			TaskLogger.LogExit();
		}

		protected override void InternalEndProcessing()
		{
			TaskLogger.LogEnter();
			if (this.m_output != null)
			{
				this.m_output.WriteProgress(Strings.ProgressStatusCompleted, Strings.DagTaskDone, 100);
				this.m_output.CloseTempLogFile();
			}
			TaskLogger.LogExit();
		}

		private void UpdateAdSettings()
		{
			this.m_mailboxServer.DatabaseAvailabilityGroup = (ADObjectId)this.m_dag.Identity;
			base.DataSession.Save(this.m_mailboxServer);
			ExTraceGlobals.ClusterTracer.TraceDebug<string, string>((long)this.GetHashCode(), "PrepareDataObject() called on dag={0} and server={1}.", this.m_dagName, this.m_mailboxServerName);
			DagTaskHelper.PromoteDagServersDatabasesToDag(this.ConfigurationSession, this.m_output, this.m_dag, this.m_mailboxServer);
		}

		private void FollowBestPractices()
		{
			DagHelper.FollowBestPractices(this.m_output, this.m_clusDag);
		}

		private void DagTrace(string format, params object[] args)
		{
			this.m_output.AppendLogMessage(format, args);
		}

		private void DagTrace(string message)
		{
			this.m_output.AppendLogMessage(message, new object[0]);
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				if (!this.m_fDisposed)
				{
					if (disposing && this.m_clusDag != null)
					{
						this.m_clusDag.Dispose();
						this.m_clusDag = null;
					}
					this.m_fDisposed = true;
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		private const string WmiStringDescription = "Description";

		private const string WmiStringDhcpEnabled = "DHCPEnabled";

		private const string WmiStringIPAddress = "IPAddress";

		private const string WmiStringIPSubnet = "IPSubnet";

		private const string WmiStringDefaultIPGateway = "DefaultIPGateway";

		private const int MaxServersInDag = 16;

		private static Regex s_regexNetmask = new Regex("^(\\d+)\\.(\\d+)\\.(\\d+)\\.(\\d+)$", RegexOptions.CultureInvariant);

		private FileShareWitness m_fsw;

		private FileShareWitness m_afsw;

		private DatabaseAvailabilityGroup m_dag;

		private string m_dagName;

		private string m_secretClusterName;

		private AmCluster m_clusDag;

		private IPAddress[] m_staticAddresses;

		private HaTaskOutputHelper m_output;

		private Server m_mailboxServer;

		private List<Server> m_serversInDag;

		private AmServerName m_mailboxAmServerName;

		private string m_mailboxServerName;

		private bool m_clusteringInstalledOnServer;

		private bool m_clusteringConfiguredOnServer;

		private bool m_skipDagValidation;

		private bool m_createIPAddressResources;

		private bool m_joinCluster;

		private bool m_fDisposed;

		private static Regex m_regexUncShare = new Regex("^\\\\\\\\([^\\\\]+)\\\\([^\\\\]+)$", RegexOptions.CultureInvariant);

		internal enum RequireGateway
		{
			No,
			Yes
		}
	}
}
