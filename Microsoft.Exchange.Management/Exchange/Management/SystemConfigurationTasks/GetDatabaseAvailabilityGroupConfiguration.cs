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
	[OutputType(new Type[]
	{
		typeof(DagConfigurationEntry)
	})]
	[Cmdlet("Get", "DatabaseAvailabilityGroupConfiguration")]
	public sealed class GetDatabaseAvailabilityGroupConfiguration : GetSystemConfigurationObjectTask<DatabaseAvailabilityGroupConfigurationIdParameter, DatabaseAvailabilityGroupConfiguration>
	{
		protected override IConfigDataProvider CreateSession()
		{
			return DirectorySessionFactory.Default.CreateTopologyConfigurationSession(base.DomainController, false, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 52, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\Cluster\\GetDatabaseAvailabilityGroupConfiguration.cs");
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			TaskLogger.LogEnter(new object[]
			{
				dataObject.Identity,
				dataObject
			});
			DatabaseAvailabilityGroupConfiguration dagConfig = (DatabaseAvailabilityGroupConfiguration)dataObject;
			DagConfigurationEntry dagConfigurationEntry = this.ConstructNewDagConfigEntry(dagConfig);
			if (dagConfigurationEntry != null)
			{
				base.WriteResult(dagConfigurationEntry);
			}
			TaskLogger.LogExit();
		}

		private DagConfigurationEntry ConstructNewDagConfigEntry(DatabaseAvailabilityGroupConfiguration dagConfig)
		{
			DagConfigurationHelper dagConfigurationHelper = DagConfigurationHelper.Deserialize(dagConfig.ConfigurationXML);
			if (dagConfigurationHelper.Version <= 1)
			{
				return new DagConfigurationEntry
				{
					Name = dagConfig.Name,
					Identity = dagConfig.Identity,
					ServersPerDag = dagConfigurationHelper.ServersPerDag,
					DatabasesPerServer = dagConfigurationHelper.DatabasesPerServer,
					DatabasesPerVolume = dagConfigurationHelper.DatabasesPerVolume,
					CopiesPerDatabase = dagConfigurationHelper.CopiesPerDatabase,
					MinCopiesPerDatabaseForMonitoring = dagConfigurationHelper.MinCopiesPerDatabaseForMonitoring
				};
			}
			throw new DagConfigVersionConflictException(dagConfig.Name, 1, dagConfigurationHelper.Version);
		}

		protected override bool DeepSearch
		{
			get
			{
				return true;
			}
		}
	}
}
