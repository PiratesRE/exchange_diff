using System;
using Microsoft.Exchange.Core.RuleTasks;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	public enum AddedRecipientType
	{
		[LocDescription(RulesTasksStrings.IDs.ToRecipientType)]
		To,
		[LocDescription(RulesTasksStrings.IDs.CcRecipientType)]
		Cc,
		[LocDescription(RulesTasksStrings.IDs.BccRecipientType)]
		Bcc,
		[LocDescription(RulesTasksStrings.IDs.RedirectRecipientType)]
		Redirect
	}
}
