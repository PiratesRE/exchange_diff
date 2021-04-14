using System;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.MessagingPolicies.Journaling
{
	public enum JournalRuleScope
	{
		[LocDescription(Strings.IDs.InternalJournalRuleScope)]
		Internal,
		[LocDescription(Strings.IDs.ExternalJournalRuleScope)]
		External,
		[LocDescription(Strings.IDs.GlobalJournalRuleScope)]
		Global
	}
}
