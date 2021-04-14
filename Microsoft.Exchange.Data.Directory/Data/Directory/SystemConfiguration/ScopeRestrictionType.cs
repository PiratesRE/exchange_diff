using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public enum ScopeRestrictionType
	{
		NotApplicable = 1,
		DomainScope_Obsolete,
		RecipientScope,
		ServerScope,
		PartnerDelegatedTenantScope,
		DatabaseScope
	}
}
