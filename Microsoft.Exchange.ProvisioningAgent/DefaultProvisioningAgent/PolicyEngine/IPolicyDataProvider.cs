using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Provisioning;
using Microsoft.Exchange.Data.Directory.ProvisioningCache;

namespace Microsoft.Exchange.DefaultProvisioningAgent.PolicyEngine
{
	internal interface IPolicyDataProvider
	{
		IEnumerable<ADProvisioningPolicy> GetEffectiveProvisioningPolicy(OrganizationId organizationId, Type poType, ProvisioningPolicyType policyType, int maxResults, ProvisioningCache provisioningCache);

		string Source { get; }
	}
}
