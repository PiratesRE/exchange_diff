using System;
using Microsoft.Exchange.Core.RuleTasks;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	public enum ADAttributeEvaluationType
	{
		[LocDescription(RulesTasksStrings.IDs.ADAttributeEvaluationTypeEquals)]
		Equals,
		[LocDescription(RulesTasksStrings.IDs.ADAttributeEvaluationTypeContains)]
		Contains
	}
}
