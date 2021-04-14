using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.Sync.CookieManager
{
	internal sealed class MsoRecipientFullSyncPoller : MsoFullSyncPoller
	{
		public MsoRecipientFullSyncPoller(string serviceInstanceName) : base(serviceInstanceName)
		{
		}

		protected override QueryFilter RetrieveFullSyncTenantsSearchFilter()
		{
			return new AndFilter(new QueryFilter[]
			{
				new ExistsFilter(MsoTenantCookieContainerSchema.MsoForwardSyncRecipientCookie),
				base.RetrieveFullSyncTenantsSearchFilter(),
				new ComparisonFilter(ComparisonOperator.NotEqual, ExchangeConfigurationUnitSchema.OrganizationStatus, OrganizationStatus.ReadyForRemoval),
				new ComparisonFilter(ComparisonOperator.NotEqual, ExchangeConfigurationUnitSchema.OrganizationStatus, OrganizationStatus.SoftDeleted),
				new ComparisonFilter(ComparisonOperator.NotEqual, ExchangeConfigurationUnitSchema.OrganizationStatus, OrganizationStatus.PendingRemoval)
			});
		}
	}
}
