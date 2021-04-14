using System;

namespace Microsoft.Exchange.MessagingPolicies.UnJournalAgent
{
	internal enum UnjournalRecipientType
	{
		Unknown,
		Mailbox,
		DistributionGroup,
		ResolvedOther,
		External,
		Empty
	}
}
