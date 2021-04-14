using System;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	internal interface ISymphonyProxy
	{
		WorkItemQueryResult QueryWorkItems(string groupName, string tenantTier, string workItemType, WorkItemStatus status, int pageSize, byte[] bookmark);

		WorkItemInfo[] QueryTenantWorkItems(Guid tenantId);

		void UpdateWorkItem(string workItemId, WorkItemStatusInfo status);

		void UpdateTenantReadiness(TenantReadiness[] data);
	}
}
