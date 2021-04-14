using System;

namespace Microsoft.Office.CompliancePolicy.PolicySync
{
	public interface ITenantInfoProvider : IDisposable
	{
		void Save(TenantInfo tenantInfo);

		TenantInfo Load();
	}
}
