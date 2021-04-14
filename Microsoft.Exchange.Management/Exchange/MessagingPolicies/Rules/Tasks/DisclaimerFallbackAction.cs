using System;
using Microsoft.Exchange.Core.RuleTasks;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	public enum DisclaimerFallbackAction
	{
		[LocDescription(RulesTasksStrings.IDs.FallbackWrap)]
		Wrap,
		[LocDescription(RulesTasksStrings.IDs.FallbackIgnore)]
		Ignore,
		[LocDescription(RulesTasksStrings.IDs.FallbackReject)]
		Reject
	}
}
