using System;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	public enum RuleState
	{
		[LocDescription(RulesStrings.IDs.StateEnabled)]
		Enabled,
		[LocDescription(RulesStrings.IDs.StateDisabled)]
		Disabled
	}
}
