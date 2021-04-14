using System;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	public enum RuleErrorAction
	{
		[LocDescription(RulesStrings.IDs.RuleErrorActionIgnore)]
		Ignore,
		[LocDescription(RulesStrings.IDs.RuleErrorActionDefer)]
		Defer
	}
}
