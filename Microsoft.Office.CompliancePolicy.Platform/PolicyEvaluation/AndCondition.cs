using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Office.CompliancePolicy.PolicyEvaluation
{
	public sealed class AndCondition : Condition
	{
		public override ConditionType ConditionType
		{
			get
			{
				return ConditionType.And;
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
			return this.subConditions.All((Condition condition) => condition.Evaluate(context));
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
