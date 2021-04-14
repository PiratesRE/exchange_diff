using System;
using System.Management;
using System.Management.Automation;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("New", "DatabaseAvailabilityGroup", SupportsShouldProcess = true)]
	public sealed class NewDatabaseAvailabilityGroup : NewSystemConfigurationObjectTask<DatabaseAvailabilityGroup>
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
		public ThirdPartyReplicationMode ThirdPartyReplication
		{
			get
			{
				if (base.Fields["ThirdPartyReplication"] == null)
				{
					return ThirdPartyReplicationMode.Disabled;
				}
				return (ThirdPartyReplicationMode)base.Fields["ThirdPartyReplication"];
			}
			set
			{
				base.Fields["ThirdPartyReplication"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		[ValidateNotNullOrEmpty]
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
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewDatabaseAvailabilityGroup(base.Name.ToString());
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			return DirectorySessionFactory.Default.CreateTopologyConfigurationSession(base.DomainController, false, ConsistencyMode.FullyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 152, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\Cluster\\NewDatabaseAvailabilityGroup.cs");
		}

		private void ValidateDagNameIsValidComputerName(string dagName)
		{
			bool flag = true;
			Regex regex = new Regex("^[-A-Za-z0-9]+$", RegexOptions.CultureInvariant);
			if (dagName.Length > 15)
			{
				flag = false;
			}
			else
			{
				Match match = regex.Match(dagName);
				if (!match.Success)
				{
					flag = false;
				}
			}
			if (!flag)
			{
				this.m_output.WriteError(new DagTaskDagNameMustBeComputerNameExceptionM1(dagName), ErrorCategory.InvalidArgument, null);
			}
			ITopologyConfigurationSession configSession = (ITopologyConfigurationSession)base.DataSession;
			bool flag3;
			bool flag2 = DagTaskHelper.DoesComputerAccountExist(configSession, dagName, out flag3);
			if (!flag2)
			{
				ExTraceGlobals.ClusterTracer.TraceDebug<string>((long)this.GetHashCode(), "The computer account {0} does not exist.", dagName);
				return;
			}
			if (flag3)
			{
				this.m_output.WriteError(new DagTaskComputerAccountExistsAndIsEnabledException(dagName), ErrorCategory.InvalidArgument, null);
				return;
			}
			ExTraceGlobals.ClusterTracer.TraceDebug<string>((long)this.GetHashCode(), "The computer account {0} exists, but is disabled.", dagName);
		}

		private void LogCommandLineParameters()
		{
			this.m_output.AppendLogMessage("commandline: {0}", new object[]
			{
				base.MyInvocation.Line
			});
			string[] array = new string[]
			{
				"Name",
				"WitnessServer",
				"WitnessDirectory",
				"WhatIf"
			};
			foreach (string text in array)
			{
				this.m_output.AppendLogMessage("Option '{0}' = '{1}'.", new object[]
				{
					text,
					base.Fields[text]
				});
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			this.m_output = new HaTaskOutputHelper("new-databaseavailabiltygroup", new Task.TaskErrorLoggingDelegate(base.WriteError), new Task.TaskWarningLoggingDelegate(this.WriteWarning), new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.TaskProgressLoggingDelegate(base.WriteProgress), this.GetHashCode());
			this.m_output.CreateTempLogFile();
			this.m_output.AppendLogMessage("new-dag started", new object[0]);
			this.LogCommandLineParameters();
			if (this.DatabaseAvailabilityGroupIpAddresses != null)
			{
				foreach (IPAddress ipaddress in this.DatabaseAvailabilityGroupIpAddresses)
				{
					if (ipaddress.AddressFamily == AddressFamily.InterNetworkV6)
					{
						this.m_output.WriteErrorSimple(new DagTaskDagIpAddressesMustBeIpv4Exception());
					}
				}
			}
			this.m_dagName = base.Name;
			QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, DatabaseAvailabilityGroupSchema.Name, this.m_dagName);
			DatabaseAvailabilityGroup[] array = this.ConfigurationSession.Find<DatabaseAvailabilityGroup>(null, QueryScope.SubTree, filter, null, 1);
			if (array != null && array.Length > 0)
			{
				base.WriteError(new ADObjectAlreadyExistsException(Strings.NewDagErrorDuplicateName(this.m_dagName)), ErrorCategory.InvalidArgument, this.m_dagName);
			}
			this.ValidateDagNameIsValidComputerName(this.m_dagName);
			this.m_fsw = new FileShareWitness((ITopologyConfigurationSession)base.DataSession, this.m_dagName, this.WitnessServer, this.WitnessDirectory);
			try
			{
				this.m_fsw.Initialize();
			}
			catch (LocalizedException error)
			{
				this.m_output.WriteErrorSimple(error);
			}
			catch (ManagementException error2)
			{
				this.m_output.WriteErrorSimple(error2);
			}
			try
			{
				this.m_fsw.Create();
				if (this.m_fsw.IsJustCreated)
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
			base.InternalValidate();
			DagTaskHelper.VerifyDagIsWithinScopes<DatabaseAvailabilityGroup>(this, this.DataObject, false);
			this.m_output.WriteVerbose(Strings.NewDagPassedChecks);
			TaskLogger.LogExit();
		}

		private void InitializeDagAdObject()
		{
			foreach (ADObjectId identity in this.m_newDag.Servers)
			{
				Server dagMemberServer = (Server)base.DataSession.Read<Server>(identity);
				DagTaskHelper.RevertDagServersDatabasesToStandalone(this.ConfigurationSession, this.m_output, dagMemberServer);
			}
			if (base.Fields["DatabaseAvailabilityGroupIpAddresses"] != null)
			{
				MultiValuedProperty<IPAddress> databaseAvailabilityGroupIpv4Addresses = new MultiValuedProperty<IPAddress>(this.DatabaseAvailabilityGroupIpAddresses);
				this.m_newDag.DatabaseAvailabilityGroupIpv4Addresses = databaseAvailabilityGroupIpv4Addresses;
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			if (this.ThirdPartyReplication == ThirdPartyReplicationMode.Enabled)
			{
				this.m_output.WriteWarning(Strings.WarningMessageNewTPRDag);
			}
			this.InitializeDagAdObject();
			base.InternalProcessRecord();
			this.m_output.WriteVerbose(Strings.NewDagCompletedSuccessfully);
			this.m_output.CloseTempLogFile();
			TaskLogger.LogExit();
		}

		protected override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			DatabaseAvailabilityGroup databaseAvailabilityGroup = new DatabaseAvailabilityGroup();
			databaseAvailabilityGroup.SetId(((ITopologyConfigurationSession)this.ConfigurationSession).GetDatabaseAvailabilityGroupContainerId().GetChildId(base.Name));
			databaseAvailabilityGroup.Name = this.m_dagName;
			databaseAvailabilityGroup.SetWitnessServer(this.m_fsw.FileShareWitnessShare, this.m_fsw.WitnessDirectory);
			databaseAvailabilityGroup.ThirdPartyReplication = this.ThirdPartyReplication;
			if (this.DagConfiguration != null)
			{
				DatabaseAvailabilityGroupConfiguration databaseAvailabilityGroupConfiguration = DagConfigurationHelper.DagConfigIdParameterToDagConfig(this.DagConfiguration, this.ConfigurationSession);
				databaseAvailabilityGroup.DatabaseAvailabilityGroupConfiguration = databaseAvailabilityGroupConfiguration.Id;
			}
			this.m_newDag = databaseAvailabilityGroup;
			TaskLogger.LogExit();
			return databaseAvailabilityGroup;
		}

		private FileShareWitness m_fsw;

		private DatabaseAvailabilityGroup m_newDag;

		private string m_dagName;

		private HaTaskOutputHelper m_output;

		private static class ParameterNames
		{
			public const string DatabaseAvailabilityGroupIpAddresses = "DatabaseAvailabilityGroupIpAddresses";

			public const string ThirdPartyReplication = "ThirdPartyReplication";
		}
	}
}
