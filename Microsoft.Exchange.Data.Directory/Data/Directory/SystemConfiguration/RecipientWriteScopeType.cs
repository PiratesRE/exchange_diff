using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public enum RecipientWriteScopeType
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
		ExclusiveRecipientScope = 13,
		MailboxICanDelegate = 15
	}
}
