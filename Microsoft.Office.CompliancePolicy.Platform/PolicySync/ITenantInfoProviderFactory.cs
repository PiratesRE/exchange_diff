using System;

namespace Microsoft.Office.CompliancePolicy.PolicySync
{
	public interface ITenantInfoProviderFactory
	{
		ITenantInfoProvider CreateTenantInfoProvider(TenantContext tenantContext);
	}
}
