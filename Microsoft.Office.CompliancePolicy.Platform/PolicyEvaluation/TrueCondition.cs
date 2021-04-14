using System;

namespace Microsoft.Office.CompliancePolicy.PolicyEvaluation
{
	public sealed class TrueCondition : Condition
	{
		public override ConditionType ConditionType
		{
			get
			{
				return ConditionType.True;
			}
		}

		public override bool Evaluate(PolicyEvaluationContext context)
		{
			return true;
		}
	}
}
