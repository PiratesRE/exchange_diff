using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public enum ConfigWriteScopeType
	{
		None,
		NotApplicable,
		OrganizationConfig = 10,
		CustomConfigScope,
		PartnerDelegatedTenantScope,
		ExclusiveConfigScope = 14
	}
}
