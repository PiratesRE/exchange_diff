using System;

namespace Microsoft.Exchange.Data.Directory.ProvisioningCache
{
	internal interface IProvisioningCacheInvalidation
	{
		bool ShouldInvalidProvisioningCache(out OrganizationId orgId, out Guid[] keys);
	}
}
