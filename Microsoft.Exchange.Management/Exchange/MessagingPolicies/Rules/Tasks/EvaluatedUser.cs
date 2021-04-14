using System;
using Microsoft.Exchange.Core.RuleTasks;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	public enum EvaluatedUser
	{
		[LocDescription(RulesTasksStrings.IDs.EvaluatedUserSender)]
		Sender,
		[LocDescription(RulesTasksStrings.IDs.EvaluatedUserRecipient)]
		Recipient
	}
}
