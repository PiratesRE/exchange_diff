using System;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SymphonyProxy : ISymphonyProxy
	{
		public Uri WorkloadUri { get; set; }

		public X509Certificate2 Cert { get; set; }

		public WorkItemQueryResult QueryWorkItems(string groupName, string tenantTier, string workItemType, WorkItemStatus status, int pageSize, byte[] bookmark)
		{
			WorkItemQueryResult result2;
			using (ProxyWrapper<UpgradeHandlerClient, IUpgradeHandler> workloadClient = new ProxyWrapper<UpgradeHandlerClient, IUpgradeHandler>(this.WorkloadUri, this.Cert))
			{
				WorkItemQueryResult result = null;
				workloadClient.CallSymphony(delegate
				{
					result = workloadClient.Proxy.QueryWorkItems(groupName, tenantTier, workItemType, status, pageSize, bookmark);
				}, this.WorkloadUri.ToString());
				result2 = result;
			}
			return result2;
		}

		public WorkItemInfo[] QueryTenantWorkItems(Guid tenantId)
		{
			WorkItemInfo[] result2;
			using (ProxyWrapper<UpgradeHandlerClient, IUpgradeHandler> workloadClient = new ProxyWrapper<UpgradeHandlerClient, IUpgradeHandler>(this.WorkloadUri, this.Cert))
			{
				WorkItemInfo[] result = null;
				workloadClient.CallSymphony(delegate
				{
					result = workloadClient.Proxy.QueryTenantWorkItems(tenantId);
				}, this.WorkloadUri.ToString());
				result2 = result;
			}
			return result2;
		}

		public void UpdateWorkItem(string workItemId, WorkItemStatusInfo status)
		{
			SymphonyProxy.<>c__DisplayClassf CS$<>8__locals1 = new SymphonyProxy.<>c__DisplayClassf();
			CS$<>8__locals1.workItemId = workItemId;
			CS$<>8__locals1.status = status;
			using (ProxyWrapper<UpgradeHandlerClient, IUpgradeHandler> workloadClient = new ProxyWrapper<UpgradeHandlerClient, IUpgradeHandler>(this.WorkloadUri, this.Cert))
			{
				workloadClient.CallSymphony(delegate
				{
					workloadClient.Proxy.UpdateWorkItem(CS$<>8__locals1.workItemId, CS$<>8__locals1.status);
				}, this.WorkloadUri.ToString());
			}
		}

		public void UpdateTenantReadiness(TenantReadiness[] data)
		{
			SymphonyProxy.<>c__DisplayClass15 CS$<>8__locals1 = new SymphonyProxy.<>c__DisplayClass15();
			CS$<>8__locals1.data = data;
			using (ProxyWrapper<UpgradeSchedulingConstraintsClient, IUpgradeSchedulingConstraints> workloadClient = new ProxyWrapper<UpgradeSchedulingConstraintsClient, IUpgradeSchedulingConstraints>(this.WorkloadUri, this.Cert))
			{
				workloadClient.CallSymphony(delegate
				{
					workloadClient.Proxy.UpdateTenantReadiness(CS$<>8__locals1.data);
				}, this.WorkloadUri.ToString());
			}
		}
	}
}
