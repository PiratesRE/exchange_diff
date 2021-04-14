using System;
using Microsoft.Exchange.Core.RuleTasks;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	public enum DisclaimerLocation
	{
		[LocDescription(RulesTasksStrings.IDs.AppendDisclaimer)]
		Append,
		[LocDescription(RulesTasksStrings.IDs.PrependDisclaimer)]
		Prepend
	}
}
