using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.Sync.CookieManager
{
	internal abstract class MsoFullSyncPoller : FullSyncPoller
	{
		protected MsoFullSyncPoller(string serviceInstanceName)
		{
			this.msoServiceInstanceName = serviceInstanceName;
		}

		public override IEnumerable<string> GetFullSyncTenants()
		{
			QueryFilter filter = this.RetrieveFullSyncTenantsSearchFilter();
			return from cu in PartitionDataAggregator.FindTenantCookieContainers(filter)
			select cu.ExternalDirectoryOrganizationId;
		}

		protected virtual QueryFilter RetrieveFullSyncTenantsSearchFilter()
		{
			return new OrFilter(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, ExchangeConfigurationUnitSchema.DirSyncServiceInstance, this.msoServiceInstanceName),
				new NotFilter(new ExistsFilter(ExchangeConfigurationUnitSchema.DirSyncServiceInstance))
			});
		}

		private readonly string msoServiceInstanceName;
	}
}
