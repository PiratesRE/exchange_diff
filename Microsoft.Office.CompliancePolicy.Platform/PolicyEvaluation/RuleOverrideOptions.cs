using System;

namespace Microsoft.Office.CompliancePolicy.PolicyEvaluation
{
	[Flags]
	public enum RuleOverrideOptions
	{
		None = 0,
		AllowFalsePositiveOverride = 1,
		AllowOverrideWithoutJustification = 2,
		AllowOverrideWithJustification = 4
	}
}
