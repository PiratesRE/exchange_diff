using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	public class OrCondition : Condition
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
				Version version = Rule.BaseVersion;
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

		public override bool Evaluate(RulesEvaluationContext context)
		{
			bool result = false;
			foreach (Condition condition in this.subConditions)
			{
				if (condition.Evaluate(context))
				{
					context.Trace("Subcondition '{0}' evaluated as Match", new object[]
					{
						condition.ConditionType
					});
					result = true;
					if (base.EvaluationMode == ConditionEvaluationMode.Optimized)
					{
						return true;
					}
				}
				else
				{
					context.Trace("Subcondition '{0}' evaluated as Not Match", new object[]
					{
						condition.ConditionType
					});
				}
			}
			return result;
		}

		public override void GetSupplementalData(SupplementalData data)
		{
			foreach (Condition condition in this.subConditions)
			{
				condition.GetSupplementalData(data);
			}
		}

		private List<Condition> subConditions = new List<Condition>();
	}
}
