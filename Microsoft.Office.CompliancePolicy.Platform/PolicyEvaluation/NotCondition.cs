using System;

namespace Microsoft.Office.CompliancePolicy.PolicyEvaluation
{
	public sealed class NotCondition : Condition
	{
		public NotCondition(Condition subCondition)
		{
			this.subCondition = subCondition;
		}

		public override ConditionType ConditionType
		{
			get
			{
				return ConditionType.Not;
			}
		}

		public Condition SubCondition
		{
			get
			{
				return this.subCondition;
			}
		}

		public override Version MinimumVersion
		{
			get
			{
				return this.subCondition.MinimumVersion;
			}
		}

		public override bool Evaluate(PolicyEvaluationContext context)
		{
			return !this.subCondition.Evaluate(context);
		}

		public override void GetSupplementalData(SupplementalData data)
		{
			this.SubCondition.GetSupplementalData(data);
		}

		private Condition subCondition;
	}
}
