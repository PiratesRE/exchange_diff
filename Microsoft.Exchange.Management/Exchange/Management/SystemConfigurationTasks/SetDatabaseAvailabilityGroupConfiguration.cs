using System;
using System.Management.Automation;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Set", "DatabaseAvailabilityGroupConfiguration", DefaultParameterSetName = "Identity", SupportsShouldProcess = true)]
	public sealed class SetDatabaseAvailabilityGroupConfiguration : SetTopologySystemConfigurationObjectTask<DatabaseAvailabilityGroupConfigurationIdParameter, DatabaseAvailabilityGroupConfiguration>
	{
		[Parameter(Mandatory = false)]
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

		[Parameter(Mandatory = false)]
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

		[Parameter(Mandatory = false)]
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

		[Parameter(Mandatory = false)]
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

		[Parameter(Mandatory = false)]
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

		protected override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			DatabaseAvailabilityGroupConfiguration databaseAvailabilityGroupConfiguration = (DatabaseAvailabilityGroupConfiguration)base.PrepareDataObject();
			DagConfigurationHelper dagConfigurationHelper = DagConfigurationHelper.Deserialize(databaseAvailabilityGroupConfiguration.ConfigurationXML);
			if (dagConfigurationHelper.Version <= 1)
			{
				if (base.Fields["ServersPerDag"] != null)
				{
					dagConfigurationHelper.ServersPerDag = this.ServersPerDag;
				}
				if (base.Fields["DatabasesPerServer"] != null)
				{
					dagConfigurationHelper.DatabasesPerServer = this.DatabasesPerServer;
				}
				if (base.Fields["DatabasesPerVolume"] != null)
				{
					dagConfigurationHelper.DatabasesPerVolume = this.DatabasesPerVolume;
				}
				if (base.Fields["CopiesPerDatabase"] != null)
				{
					dagConfigurationHelper.CopiesPerDatabase = this.CopiesPerDatabase;
				}
				if (base.Fields["MinCopiesPerDatabaseForMonitoring"] != null)
				{
					dagConfigurationHelper.MinCopiesPerDatabaseForMonitoring = this.MinCopiesPerDatabaseForMonitoring;
				}
				this.m_configXML = dagConfigurationHelper.Serialize();
				databaseAvailabilityGroupConfiguration.ConfigurationXML = this.m_configXML;
				TaskLogger.LogExit();
				return databaseAvailabilityGroupConfiguration;
			}
			throw new DagConfigVersionConflictException(databaseAvailabilityGroupConfiguration.Name, 1, dagConfigurationHelper.Version);
		}

		private string m_configXML;
	}
}
