using System;
using Microsoft.Exchange.Core.RuleTasks;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	public enum ToUserScope
	{
		[LocDescription(RulesTasksStrings.IDs.InternalUser)]
		InOrganization,
		[LocDescription(RulesTasksStrings.IDs.ExternalUser)]
		NotInOrganization,
		[LocDescription(RulesTasksStrings.IDs.ExternalPartner)]
		ExternalPartner,
		[LocDescription(RulesTasksStrings.IDs.ExternalNonPartner)]
		ExternalNonPartner
	}
}
