using System;

namespace Microsoft.Exchange.MessagingPolicies.Journaling
{
	public enum JournalNdrValidationCheckResult
	{
		JournalNdrValidationPassed,
		JournalNdrCannotBeNullReversePath,
		JournalNdrExistInJournalRuleRecipient,
		JournalNdrExistInJournalRuleJournalEmailAddress
	}
}
