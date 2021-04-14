using System;

namespace Microsoft.Exchange.MessagingPolicies.Rules
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

		public override bool Evaluate(RulesEvaluationContext context)
		{
			return false;
		}
	}
}
