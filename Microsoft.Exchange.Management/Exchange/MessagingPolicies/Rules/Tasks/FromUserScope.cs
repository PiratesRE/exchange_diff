using System;
using Microsoft.Exchange.Core.RuleTasks;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	public enum FromUserScope
	{
		[LocDescription(RulesTasksStrings.IDs.InternalUser)]
		InOrganization,
		[LocDescription(RulesTasksStrings.IDs.ExternalUser)]
		NotInOrganization
	}
}
