using System;

namespace Microsoft.Office.CompliancePolicy.PolicyConfiguration
{
	public interface IPolicyConfigProviderManager
	{
		PolicyConfigProvider CreateForSyncEngine(Guid organizationId, string auxiliaryStore, bool enablePolicyApplication = true, ExecutionLog logProvider = null);
	}
}
