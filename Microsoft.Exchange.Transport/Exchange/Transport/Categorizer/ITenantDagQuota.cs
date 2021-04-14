using System;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal interface ITenantDagQuota
	{
		int GetDagCountForTenant(Guid externalOrganizationId);

		void IncrementMessagesDeliveredToTenant(Guid externalOrganizationId);

		void RefreshDagCount(int dagsAvailable);
	}
}
