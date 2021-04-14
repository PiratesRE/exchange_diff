using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Net;
using System.Net.Sockets;
using Microsoft.Exchange.Cluster.ClusApi;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage.Cluster;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Set", "DatabaseAvailabilityGroup", DefaultParameterSetName = "Identity", SupportsShouldProcess = true)]
	public sealed class SetDatabaseAvailabilityGroup : SetTopologySystemConfigurationObjectTask<DatabaseAvailabilityGroupIdParameter, DatabaseAvailabilityGroup>
	{
		[Parameter(Mandatory = false)]
		public FileShareWitnessServerName WitnessServer
		{
			get
			{
				return (FileShareWitnessServerName)base.Fields["WitnessServer"];
			}
			set
			{
				base.Fields["WitnessServer"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ushort ReplicationPort
		{
			get
			{
				return (ushort)base.Fields["ReplicationPort"];
			}
			set
			{
				base.Fields["ReplicationPort"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public DatabaseAvailabilityGroup.NetworkOption NetworkCompression
		{
			get
			{
				return (DatabaseAvailabilityGroup.NetworkOption)base.Fields["NetworkCompression"];
			}
			set
			{
				base.Fields["NetworkCompression"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public DatabaseAvailabilityGroup.NetworkOption NetworkEncryption
		{
			get
			{
				return (DatabaseAvailabilityGroup.NetworkOption)base.Fields["NetworkEncryption"];
			}
			set
			{
				base.Fields["NetworkEncryption"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool ManualDagNetworkConfiguration
		{
			get
			{
				return (bool)base.Fields["ManualDagNetworkConfiguration"];
			}
			set
			{
				base.Fields["ManualDagNetworkConfiguration"] = value;
			}
		}

		[Parameter]
		public SwitchParameter DiscoverNetworks
		{
			get
			{
				return (SwitchParameter)(base.Fields["DiscoverNetworks"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["DiscoverNetworks"] = value;
			}
		}

		[Parameter]
		public SwitchParameter AllowCrossSiteRpcClientAccess
		{
			get
			{
				return (SwitchParameter)(base.Fields["AllowCrossSiteRpcClientAccess"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["AllowCrossSiteRpcClientAccess"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public NonRootLocalLongFullPath WitnessDirectory
		{
			get
			{
				return (NonRootLocalLongFullPath)base.Fields["WitnessDirectory"];
			}
			set
			{
				base.Fields["WitnessDirectory"] = value;
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
				this.m_alternateWitnessServerParameterSpecified = true;
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

		[Parameter(Mandatory = false)]
		public DatacenterActivationModeOption DatacenterActivationMode
		{
			get
			{
				return (DatacenterActivationModeOption)base.Fields["DatacenterActivationMode"];
			}
			set
			{
				base.Fields["DatacenterActivationMode"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public IPAddress[] DatabaseAvailabilityGroupIpAddresses
		{
			get
			{
				return (IPAddress[])base.Fields["DatabaseAvailabilityGroupIpAddresses"];
			}
			set
			{
				base.Fields["DatabaseAvailabilityGroupIpAddresses"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public DatabaseAvailabilityGroupConfigurationIdParameter DagConfiguration
		{
			get
			{
				return (DatabaseAvailabilityGroupConfigurationIdParameter)base.Fields["DagConfiguration"];
			}
			set
			{
				base.Fields["DagConfiguration"] = value;
				this.m_dagConfigParameterSpecified = true;
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
			return DagTaskHelper.IsKnownException(this, e) || base.IsKnownException(e);
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetDatabaseAvailabilityGroup(this.Identity.ToString());
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			this.m_output = new HaTaskOutputHelper("set-databaseavailabilitygroup", new Task.TaskErrorLoggingDelegate(base.WriteError), new Task.TaskWarningLoggingDelegate(this.WriteWarning), new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.TaskProgressLoggingDelegate(base.WriteProgress), this.GetHashCode());
			this.m_dag = this.DataObject;
			this.m_skipDagValidation = this.SkipDagValidation;
			DagTaskHelper.VerifyDagAndServersAreWithinScopes<DatabaseAvailabilityGroup>(this, this.m_dag, true);
			if (this.DatabaseAvailabilityGroupIpAddresses != null)
			{
				if (!this.m_dag.IsDagEmpty())
				{
					this.PrepareServersInDagIfRequired();
					foreach (AmServerName amServerName in this.m_serverNamesInDag)
					{
						DagTaskHelper.ValidateIPAddressList(this.m_output, amServerName.NetbiosName, this.DatabaseAvailabilityGroupIpAddresses, this.m_dag.Name);
						DagTaskHelper.ValidateIPAddressList(this.m_output, amServerName.Fqdn, this.DatabaseAvailabilityGroupIpAddresses, this.m_dag.Name);
					}
				}
				foreach (IPAddress ipaddress in this.DatabaseAvailabilityGroupIpAddresses)
				{
					if (ipaddress.AddressFamily == AddressFamily.InterNetworkV6)
					{
						this.m_output.WriteErrorSimple(new DagTaskDagIpAddressesMustBeIpv4Exception());
					}
				}
			}
			this.m_useAlternateWitnessServer = (this.m_dag.AlternateWitnessServer != null && DagTaskHelper.IsDagFailedOverToOtherSite(this.m_output, this.m_dag));
			if (!this.m_useAlternateWitnessServer)
			{
				this.m_fsw = new FileShareWitness((ITopologyConfigurationSession)base.DataSession, this.m_dag.Name, this.WitnessServer ?? this.m_dag.WitnessServer, this.WitnessDirectory ?? this.m_dag.WitnessDirectory);
				try
				{
					this.m_fsw.Initialize();
				}
				catch (LocalizedException error)
				{
					this.m_output.WriteErrorSimple(error);
				}
				this.CheckFsw(this.m_fsw.WitnessServerFqdn);
			}
			if (this.AlternateWitnessServer != null || this.AlternateWitnessDirectory != null || (this.m_dag.AlternateWitnessServer != null && this.m_dag.AlternateWitnessDirectory != null && !this.m_alternateWitnessServerParameterSpecified) || this.m_useAlternateWitnessServer)
			{
				this.m_afsw = new FileShareWitness((ITopologyConfigurationSession)base.DataSession, this.m_dag.Name, this.AlternateWitnessServer ?? this.m_dag.AlternateWitnessServer, this.AlternateWitnessDirectory ?? this.m_dag.AlternateWitnessDirectory);
				try
				{
					this.m_afsw.Initialize();
				}
				catch (LocalizedException error2)
				{
					this.m_output.WriteErrorSimple(error2);
				}
				if (this.m_fsw != null && SharedHelper.StringIEquals(this.m_afsw.WitnessServerFqdn, this.m_fsw.WitnessServerFqdn) && this.m_fsw.WitnessDirectory != this.m_afsw.WitnessDirectory)
				{
					this.m_output.WriteErrorSimple(new DagFswAndAlternateFswOnSameWitnessServerButPointToDifferentDirectoriesException(this.m_fsw.WitnessServer.ToString(), this.m_fsw.WitnessDirectory.ToString(), this.m_afsw.WitnessDirectory.ToString()));
				}
				this.CheckFsw(this.m_afsw.WitnessServerFqdn);
			}
			Dictionary<AmServerName, Server> startedServers = new Dictionary<AmServerName, Server>();
			DatabaseAvailabilityGroupAction.ResolveServers(this.m_output, this.m_dag, this.m_allServers, startedServers, this.m_stoppedServers);
			if (!this.m_dag.IsDagEmpty())
			{
				this.PrepareServersInDagIfRequired();
				List<AmServerName> list = new List<AmServerName>(this.m_stoppedServers.Count);
				IEnumerable<AmServerName> serversInCluster = null;
				foreach (Server server in this.m_stoppedServers.Values)
				{
					list.Add(new AmServerName(server.Id));
				}
				using (AmCluster amCluster = AmCluster.OpenDagClus(this.m_dag))
				{
					serversInCluster = amCluster.EnumerateNodeNames();
				}
				if (!this.m_skipDagValidation)
				{
					DagTaskHelper.CompareDagClusterMembership(this.m_output, this.m_dag.Name, this.m_serverNamesInDag, serversInCluster, list);
				}
			}
			if (base.Fields["DatacenterActivationMode"] != null && this.DatacenterActivationMode != DatacenterActivationModeOption.Off)
			{
				DagTaskHelper.CheckDagCanBeActivatedInDatacenter(this.m_output, this.m_dag, null, (ITopologyConfigurationSession)base.DataSession);
			}
			if (base.Fields["DatacenterActivationMode"] != null && this.DatacenterActivationMode == DatacenterActivationModeOption.Off)
			{
				this.DataObject.StartedMailboxServers = null;
				this.DataObject.StoppedMailboxServers = null;
			}
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			bool flag = false;
			if ((base.Fields["ReplicationPort"] != null || base.Fields["NetworkCompression"] != null || base.Fields["NetworkEncryption"] != null || base.Fields["ManualDagNetworkConfiguration"] != null || base.Fields["DiscoverNetworks"] != null) && !this.m_dag.IsDagEmpty())
			{
				flag = true;
				this.m_IsObjectChanged = true;
			}
			if (this.DataObject.AllowCrossSiteRpcClientAccess != this.AllowCrossSiteRpcClientAccess)
			{
				if (base.Fields["AllowCrossSiteRpcClientAccess"] != null)
				{
					this.DataObject.AllowCrossSiteRpcClientAccess = this.AllowCrossSiteRpcClientAccess;
					this.m_IsObjectChanged = true;
				}
				else
				{
					this.AllowCrossSiteRpcClientAccess = this.DataObject.AllowCrossSiteRpcClientAccess;
				}
			}
			if (this.m_fsw != null)
			{
				this.m_dag.SetWitnessServer(this.m_fsw.FileShareWitnessShare, this.m_fsw.WitnessDirectory);
				this.m_IsObjectChanged = true;
			}
			if (this.m_afsw != null)
			{
				this.m_dag.SetAlternateWitnessServer(this.m_afsw.FileShareWitnessShare, this.m_afsw.WitnessDirectory);
				this.m_IsObjectChanged = true;
			}
			else if (this.AlternateWitnessServer == null && this.m_alternateWitnessServerParameterSpecified)
			{
				this.m_dag.SetAlternateWitnessServer(null, null);
			}
			base.InternalProcessRecord();
			if (flag && !this.m_dag.IsDagEmpty())
			{
				SetDagNetworkConfigRequest setDagNetworkConfigRequest = new SetDagNetworkConfigRequest();
				if (base.Fields["ReplicationPort"] != null)
				{
					setDagNetworkConfigRequest.ReplicationPort = this.ReplicationPort;
				}
				setDagNetworkConfigRequest.NetworkCompression = this.m_dag.NetworkCompression;
				setDagNetworkConfigRequest.NetworkEncryption = this.m_dag.NetworkEncryption;
				setDagNetworkConfigRequest.ManualDagNetworkConfiguration = this.m_dag.ManualDagNetworkConfiguration;
				if (base.Fields["DiscoverNetworks"] != null)
				{
					setDagNetworkConfigRequest.DiscoverNetworks = true;
				}
				DagNetworkRpc.SetDagNetworkConfig(this.m_dag, setDagNetworkConfigRequest);
			}
			if (!this.m_dag.IsDagEmpty())
			{
				using (AmCluster amCluster = AmCluster.OpenDagClus(this.m_dag))
				{
					if (amCluster.CnoName != string.Empty)
					{
						using (IAmClusterGroup amClusterGroup = amCluster.FindCoreClusterGroup())
						{
							using (IAmClusterResource amClusterResource = amClusterGroup.FindResourceByTypeName("Network Name"))
							{
								IPAddress[] dagIpAddressesFromAd = this.GetDagIpAddressesFromAd(this.m_output, this.m_dag);
								AmClusterResourceHelper.FixUpIpAddressesForNetName(this.m_output, amCluster, (AmClusterGroup)amClusterGroup, (AmClusterResource)amClusterResource, dagIpAddressesFromAd);
								DagTaskHelper.LogCnoState(this.m_output, this.m_dag.Name, amClusterResource);
							}
						}
					}
				}
				this.UpdateFileShareWitness();
				DagTaskHelper.NotifyServersOfConfigChange(this.m_allServers.Keys);
			}
			TaskLogger.LogExit();
		}

		private IPAddress[] GetDagIpAddressesFromAd(ITaskOutputHelper output, DatabaseAvailabilityGroup dag)
		{
			MultiValuedProperty<IPAddress> databaseAvailabilityGroupIpv4Addresses = dag.DatabaseAvailabilityGroupIpv4Addresses;
			IPAddress[] array = new IPAddress[0];
			if (databaseAvailabilityGroupIpv4Addresses.Count > 0)
			{
				array = databaseAvailabilityGroupIpv4Addresses.ToArray();
			}
			string[] value = (from addr in array
			select addr.ToString()).ToArray<string>();
			output.AppendLogMessage("Got the following IP addresses for the DAG (blank means DHCP): {0}", new object[]
			{
				string.Join(",", value)
			});
			return array;
		}

		protected override void InternalStateReset()
		{
			base.InternalStateReset();
			this.m_allServers.Clear();
			this.m_serverNamesInDag = null;
		}

		protected override bool IsObjectStateChanged()
		{
			return this.m_IsObjectChanged || base.IsObjectStateChanged();
		}

		protected override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			DatabaseAvailabilityGroup databaseAvailabilityGroup = (DatabaseAvailabilityGroup)base.PrepareDataObject();
			if (base.Fields["NetworkCompression"] != null)
			{
				databaseAvailabilityGroup.NetworkCompression = this.NetworkCompression;
			}
			if (base.Fields["NetworkEncryption"] != null)
			{
				databaseAvailabilityGroup.NetworkEncryption = this.NetworkEncryption;
			}
			if (base.Fields["ManualDagNetworkConfiguration"] != null)
			{
				databaseAvailabilityGroup.ManualDagNetworkConfiguration = this.ManualDagNetworkConfiguration;
			}
			if (base.Fields["DatacenterActivationMode"] != null)
			{
				databaseAvailabilityGroup.DatacenterActivationMode = this.DatacenterActivationMode;
			}
			if (base.Fields.IsChanged("DatabaseAvailabilityGroupIpAddresses"))
			{
				if (base.Fields["DatabaseAvailabilityGroupIpAddresses"] == null)
				{
					databaseAvailabilityGroup.DatabaseAvailabilityGroupIpv4Addresses = new MultiValuedProperty<IPAddress>
					{
						IPAddress.Any
					};
				}
				else
				{
					databaseAvailabilityGroup.DatabaseAvailabilityGroupIpv4Addresses = this.DatabaseAvailabilityGroupIpAddresses;
				}
			}
			if (base.Fields["DagConfiguration"] != null)
			{
				DatabaseAvailabilityGroupConfiguration databaseAvailabilityGroupConfiguration = DagConfigurationHelper.DagConfigIdParameterToDagConfig(this.DagConfiguration, this.ConfigurationSession);
				databaseAvailabilityGroup.DatabaseAvailabilityGroupConfiguration = databaseAvailabilityGroupConfiguration.Id;
			}
			else if (this.m_dagConfigParameterSpecified)
			{
				databaseAvailabilityGroup.DatabaseAvailabilityGroupConfiguration = null;
			}
			TaskLogger.LogExit();
			return databaseAvailabilityGroup;
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
			this.PrepareServersInDagIfRequired();
			AmServerName item = new AmServerName(fswComputerNameFqdn);
			if (this.m_serverNamesInDag.Contains(item))
			{
				this.m_output.WriteErrorSimple(new DagTaskServerMailboxServerAlsoServesAsFswNodeException(fswComputerNameFqdn));
			}
		}

		private void UpdateFileShareWitness()
		{
			if (this.m_allServers.Count == 0)
			{
				this.m_output.WriteErrorSimple(new DagTaskNoServersAreStartedException(this.m_dag.Name));
			}
			IEnumerable<AmServerName> amServerNamesFromServers = RestoreDatabaseAvailabilityGroup.GetAmServerNamesFromServers(this.m_allServers.Values);
			using (AmCluster amCluster = AmCluster.OpenByNames(amServerNamesFromServers))
			{
				bool flag = DagTaskHelper.IsQuorumTypeFileShareWitness(this.m_output, amCluster);
				bool flag2 = DagTaskHelper.ShouldBeFileShareWitness(this.m_output, this.m_dag, amCluster, false);
				string fswShareCurrent = string.Empty;
				if (flag)
				{
					using (AmClusterResource amClusterResource = amCluster.OpenQuorumResource())
					{
						if (amClusterResource != null)
						{
							fswShareCurrent = amClusterResource.GetPrivateProperty<string>("SharePath");
						}
					}
				}
				if (flag2)
				{
					if (this.m_fsw != null)
					{
						try
						{
							this.m_output.AppendLogMessage("Creating/modififying the primary FSW, if needed.", new object[0]);
							this.m_fsw.Create();
							if (this.m_dag.Servers.Count == 0 && this.m_fsw.IsJustCreated)
							{
								this.m_fsw.Delete();
							}
						}
						catch (LocalizedException ex)
						{
							if (this.m_fsw.GetExceptionType(ex) != FileShareWitnessExceptionType.FswDeleteError)
							{
								this.m_output.WriteWarning(ex.LocalizedString);
							}
						}
					}
					if (this.m_afsw != null && !this.m_afsw.Equals(this.m_fsw))
					{
						try
						{
							this.m_output.AppendLogMessage("Creating/modififying the alternate FSW, if needed.", new object[0]);
							this.m_afsw.Create();
							if (this.m_dag.Servers.Count == 0 && this.m_afsw.IsJustCreated)
							{
								this.m_afsw.Delete();
							}
						}
						catch (LocalizedException ex2)
						{
							if (this.m_afsw.GetExceptionType(ex2) != FileShareWitnessExceptionType.FswDeleteError)
							{
								this.m_output.WriteWarning(ex2.LocalizedString);
							}
						}
					}
					bool useAlternateWitnessServer = this.m_useAlternateWitnessServer;
					if (!this.m_skipDagValidation || (flag2 && !flag))
					{
						DagTaskHelper.ChangeQuorumToMnsOrFswAsAppropriate(this.m_output, this, this.m_dag, amCluster, this.m_fsw, this.m_afsw, flag2, this.m_useAlternateWitnessServer);
					}
				}
				else if (!this.m_skipDagValidation && flag)
				{
					DagTaskHelper.RevertToMnsQuorum(this.m_output, amCluster, fswShareCurrent);
				}
			}
		}

		private FileShareWitness m_fsw;

		private FileShareWitness m_afsw;

		private bool m_useAlternateWitnessServer;

		private bool m_alternateWitnessServerParameterSpecified;

		private bool m_dagConfigParameterSpecified;

		private DatabaseAvailabilityGroup m_dag;

		private bool m_skipDagValidation;

		private HaTaskOutputHelper m_output;

		private bool m_IsObjectChanged;

		private HashSet<AmServerName> m_serverNamesInDag;

		private Dictionary<AmServerName, Server> m_allServers = new Dictionary<AmServerName, Server>(16);

		private Dictionary<AmServerName, Server> m_stoppedServers = new Dictionary<AmServerName, Server>(16);

		private static class ParameterNames
		{
			public const string ReplicationPort = "ReplicationPort";

			public const string NetworkCompression = "NetworkCompression";

			public const string NetworkEncryption = "NetworkEncryption";

			public const string ManualDagNetworkConfiguration = "ManualDagNetworkConfiguration";

			public const string DiscoverNetworks = "DiscoverNetworks";

			public const string DatacenterActivationMode = "DatacenterActivationMode";

			public const string WitnessServer = "WitnessServer";

			public const string AlternateWitnessServer = "AlternateWitnessServer";

			public const string WitnessDirectory = "WitnessDirectory";

			public const string AlternateWitnessDirectory = "AlternateWitnessDirectory";

			public const string DatabaseAvailabilityGroupIpAddresses = "DatabaseAvailabilityGroupIpAddresses";

			public const string AllowCrossSiteRpcClientAccess = "AllowCrossSiteRpcClientAccess";

			public const string DagConfiguration = "DagConfiguration";
		}
	}
}
