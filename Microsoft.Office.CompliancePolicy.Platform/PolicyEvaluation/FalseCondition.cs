using System;

namespace Microsoft.Office.CompliancePolicy.PolicyEvaluation
{
	public sealed class FalseCondition : Condition
	{
		public override ConditionType ConditionType
		{
			get
			{
				return ConditionType.False;
			}
		}

		public override bool Evaluate(PolicyEvaluationContext context)
		{
			return false;
		}
	}
}
