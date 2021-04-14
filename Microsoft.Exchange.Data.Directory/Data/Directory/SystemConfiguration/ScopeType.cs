using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public enum ScopeType
	{
		None,
		NotApplicable,
		Organization,
		MyGAL,
		Self,
		MyDirectReports,
		OU,
		CustomRecipientScope,
		MyDistributionGroups,
		MyExecutive,
		OrganizationConfig,
		CustomConfigScope,
		PartnerDelegatedTenantScope,
		ExclusiveRecipientScope,
		ExclusiveConfigScope,
		MailboxICanDelegate
	}
}
