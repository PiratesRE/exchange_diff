using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.Sync.CookieManager
{
	internal sealed class MsoCompanyFullSyncPoller : MsoFullSyncPoller
	{
		public MsoCompanyFullSyncPoller(string serviceInstanceName) : base(serviceInstanceName)
		{
		}

		protected override QueryFilter RetrieveFullSyncTenantsSearchFilter()
		{
			return new AndFilter(new QueryFilter[]
			{
				new ExistsFilter(MsoTenantCookieContainerSchema.MsoForwardSyncNonRecipientCookie),
				base.RetrieveFullSyncTenantsSearchFilter()
			});
		}
	}
}
