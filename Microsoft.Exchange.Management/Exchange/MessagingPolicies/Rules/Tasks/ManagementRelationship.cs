using System;
using Microsoft.Exchange.Core.RuleTasks;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	public enum ManagementRelationship
	{
		[LocDescription(RulesTasksStrings.IDs.ManagementRelationshipManager)]
		Manager,
		[LocDescription(RulesTasksStrings.IDs.ManagementRelationshipDirectReport)]
		DirectReport
	}
}
