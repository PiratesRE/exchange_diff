using System;

namespace Microsoft.Office.CompliancePolicy.PolicyEvaluation
{
	public enum RuleMode
	{
		Disabled,
		Audit,
		AuditAndNotify,
		Enforce,
		PendingDeletion
	}
}
