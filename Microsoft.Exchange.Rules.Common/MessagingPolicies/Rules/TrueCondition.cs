using System;

namespace Microsoft.Exchange.MessagingPolicies.Rules
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

		public override bool Evaluate(RulesEvaluationContext context)
		{
			return true;
		}
	}
}
