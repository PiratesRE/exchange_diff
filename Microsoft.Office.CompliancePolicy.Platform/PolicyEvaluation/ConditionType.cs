using System;

namespace Microsoft.Office.CompliancePolicy.PolicyEvaluation
{
	public enum ConditionType
	{
		And,
		Or,
		Not,
		True,
		False,
		Predicate,
		DynamicQuery
	}
}
