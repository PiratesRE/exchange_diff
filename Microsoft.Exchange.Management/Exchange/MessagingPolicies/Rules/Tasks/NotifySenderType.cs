using System;
using Microsoft.Exchange.Core.RuleTasks;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	public enum NotifySenderType
	{
		[LocDescription(RulesTasksStrings.IDs.NotifyOnlyActionType)]
		NotifyOnly = 1,
		[LocDescription(RulesTasksStrings.IDs.RejectMessageActionType)]
		RejectMessage,
		[LocDescription(RulesTasksStrings.IDs.RejectUnlessFalsePositiveOverrideActionType)]
		RejectUnlessFalsePositiveOverride,
		[LocDescription(RulesTasksStrings.IDs.RejectUnlessSilentOverrideActionType)]
		RejectUnlessSilentOverride,
		[LocDescription(RulesTasksStrings.IDs.RejectUnlessExplicitOverrideActionType)]
		RejectUnlessExplicitOverride
	}
}
