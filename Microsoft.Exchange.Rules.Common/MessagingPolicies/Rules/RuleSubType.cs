using System;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	public enum RuleSubType
	{
		[LocDescription(RulesStrings.IDs.RuleSubtypeNone)]
		None = 1,
		[LocDescription(RulesStrings.IDs.RuleSubtypeDlp)]
		Dlp
	}
}
