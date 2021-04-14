using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data.Reporting
{
	internal interface ITenantThrottlingSession
	{
		void SetThrottleState(TenantThrottleInfo throttleInfo);

		TenantThrottleInfo GetThrottleState(Guid tenantId);

		void SaveTenantThrottleInfo(List<TenantThrottleInfo> throttleInfoList, int partitionId = 0);

		List<TenantThrottleInfo> GetTenantThrottlingDigest(int partitionId = 0, Guid? tenantId = null, bool overriddenOnly = false, int tenantCount = 5000, bool throttledOnly = true);

		int GetPhysicalPartitionCount();

		TransportProcessingQuotaConfig GetTransportThrottlingConfig();

		void SetTransportThrottlingConfig(TransportProcessingQuotaConfig config);
	}
}
