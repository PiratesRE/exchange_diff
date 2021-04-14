using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("install", "ConfigurationUnitsContainer")]
	public sealed class InstallConfigurationUnitsContainerTask : NewFixedNameSystemConfigurationObjectTask<Container>
	{
		protected override IConfigDataProvider CreateSession()
		{
			List<ADServer> list = ADForest.GetLocalForest().FindAllGlobalCatalogsInLocalSite();
			if (list.Count == 0)
			{
				throw new CannotFindGlobalCatalogsInForest(ADForest.GetLocalForest().Fqdn);
			}
			return DirectorySessionFactory.Default.CreateTenantConfigurationSession(base.DomainController, false, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromAllTenantsPartitionId(list[0].Id.GetPartitionId()), 34, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\DirectorySetup\\InstallConfigurationUnitsContainer.cs");
		}

		protected override IConfigurable PrepareDataObject()
		{
			Container container = (Container)base.PrepareDataObject();
			container.SetId(ADSession.GetConfigurationUnitsRootForLocalForest());
			return container;
		}

		protected override void InternalProcessRecord()
		{
			try
			{
				base.InternalProcessRecord();
			}
			catch (ADObjectAlreadyExistsException)
			{
			}
		}
	}
}
