using System;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	public enum RuleMode
	{
		[LocDescription(RulesStrings.IDs.ModeAudit)]
		Audit = 1,
		[LocDescription(RulesStrings.IDs.ModeAuditAndNotify)]
		AuditAndNotify,
		[LocDescription(RulesStrings.IDs.ModeEnforce)]
		Enforce
	}
}
