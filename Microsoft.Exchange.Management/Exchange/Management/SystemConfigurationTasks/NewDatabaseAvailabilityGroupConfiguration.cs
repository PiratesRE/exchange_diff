using System;
using System.Management.Automation;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("New", "DatabaseAvailabilityGroupConfiguration", SupportsShouldProcess = true)]
	public sealed class NewDatabaseAvailabilityGroupConfiguration : NewSystemConfigurationObjectTask<DatabaseAvailabilityGroupConfiguration>
	{
		[Parameter(Mandatory = true)]
		public int ServersPerDag
		{
			get
			{
				return (int)base.Fields["ServersPerDag"];
			}
			set
			{
				base.Fields["ServersPerDag"] = value;
			}
		}

		[Parameter(Mandatory = true)]
		public int DatabasesPerServer
		{
			get
			{
				return (int)base.Fields["DatabasesPerServer"];
			}
			set
			{
				base.Fields["DatabasesPerServer"] = value;
			}
		}

		[Parameter(Mandatory = true)]
		public int DatabasesPerVolume
		{
			get
			{
				return (int)base.Fields["DatabasesPerVolume"];
			}
			set
			{
				base.Fields["DatabasesPerVolume"] = value;
			}
		}

		[Parameter(Mandatory = true)]
		public int CopiesPerDatabase
		{
			get
			{
				return (int)base.Fields["CopiesPerDatabase"];
			}
			set
			{
				base.Fields["CopiesPerDatabase"] = value;
			}
		}

		[Parameter(Mandatory = true)]
		public int MinCopiesPerDatabaseForMonitoring
		{
			get
			{
				return (int)base.Fields["MinCopiesPerDatabaseForMonitoring"];
			}
			set
			{
				base.Fields["MinCopiesPerDatabaseForMonitoring"] = value;
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			this.m_output = new HaTaskOutputHelper("new-databaseavailabiltygroupconfiguration", new Task.TaskErrorLoggingDelegate(base.WriteError), new Task.TaskWarningLoggingDelegate(this.WriteWarning), new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.TaskProgressLoggingDelegate(base.WriteProgress), this.GetHashCode());
			this.m_output.CreateTempLogFile();
			this.m_output.AppendLogMessage("new-dagconfiguration started", new object[0]);
			this.LogCommandLineParameters();
			this.m_dagConfigName = base.Name;
			DagConfigurationHelper dagConfigurationHelper = new DagConfigurationHelper(this.ServersPerDag, this.DatabasesPerServer, this.DatabasesPerVolume, this.CopiesPerDatabase, this.MinCopiesPerDatabaseForMonitoring);
			this.m_dagConfigXML = dagConfigurationHelper.Serialize();
			QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, DatabaseAvailabilityGroupConfigurationSchema.Name, this.m_dagConfigName);
			DatabaseAvailabilityGroupConfiguration[] array = this.ConfigurationSession.Find<DatabaseAvailabilityGroupConfiguration>(null, QueryScope.SubTree, filter, null, 1);
			if (array != null && array.Length > 0)
			{
				base.WriteError(new ADObjectAlreadyExistsException(Strings.NewDagConfigurationErrorDuplicateName(this.m_dagConfigName)), ErrorCategory.InvalidArgument, this.m_dagConfigName);
			}
			base.InternalValidate();
			this.m_output.WriteVerbose(Strings.NewDagConfigurationPassedChecks);
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.InternalProcessRecord();
			this.m_output.WriteVerbose(Strings.NewDagConfigurationCompletedSuccessfully);
			this.m_output.CloseTempLogFile();
			TaskLogger.LogExit();
		}

		protected override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			DatabaseAvailabilityGroupConfiguration databaseAvailabilityGroupConfiguration = new DatabaseAvailabilityGroupConfiguration();
			databaseAvailabilityGroupConfiguration.SetId(((ITopologyConfigurationSession)this.ConfigurationSession).GetDatabaseAvailabilityGroupContainerId().GetChildId(this.m_dagConfigName));
			databaseAvailabilityGroupConfiguration.Name = this.m_dagConfigName;
			databaseAvailabilityGroupConfiguration.ConfigurationXML = this.m_dagConfigXML;
			TaskLogger.LogExit();
			return databaseAvailabilityGroupConfiguration;
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
				"ServersPerDag",
				"DatabasesPerServer",
				"DatabasesPerVolume",
				"CopiesPerDatabase",
				"MinCopiesPerDatabaseForMonitoring"
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

		private string m_dagConfigName;

		private string m_dagConfigXML;

		private HaTaskOutputHelper m_output;
	}
}
