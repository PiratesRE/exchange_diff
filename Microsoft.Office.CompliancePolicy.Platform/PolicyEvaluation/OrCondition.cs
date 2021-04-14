using System;
using System.Collections.Generic;

namespace Microsoft.Office.CompliancePolicy.PolicyEvaluation
{
	public sealed class OrCondition : Condition
	{
		public override ConditionType ConditionType
		{
			get
			{
				return ConditionType.Or;
			}
		}

		public List<Condition> SubConditions
		{
			get
			{
				return this.subConditions;
			}
		}

		public override Version MinimumVersion
		{
			get
			{
				Version version = Condition.BaseVersion;
				foreach (Condition condition in this.SubConditions)
				{
					Version minimumVersion = condition.MinimumVersion;
					if (version < minimumVersion)
					{
						version = minimumVersion;
					}
				}
				return version;
			}
		}

		public override bool Evaluate(PolicyEvaluationContext context)
		{
			bool result = false;
			foreach (Condition condition in this.subConditions)
			{
				if (condition.Evaluate(context))
				{
					result = true;
					if (base.EvaluationMode == ConditionEvaluationMode.Optimized)
					{
						return true;
					}
				}
			}
			return result;
		}

		public override void GetSupplementalData(SupplementalData data)
		{
			foreach (Condition condition in this.SubConditions)
			{
				condition.GetSupplementalData(data);
			}
		}

		private List<Condition> subConditions = new List<Condition>();
	}
}
