using System;
using Microsoft.Exchange.Core.RuleTasks;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	public enum Evaluation
	{
		[LocDescription(RulesTasksStrings.IDs.EvaluationEqual)]
		Equal,
		[LocDescription(RulesTasksStrings.IDs.EvaluationNotEqual)]
		NotEqual
	}
}
