using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal sealed class AndCondition : Condition
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
			foreach (Condition condition in this.subConditions)
			{
				if (!condition.Evaluate(context))
				{
					context.Trace("Condition '{0}' evaluated as Not Match", new object[]
					{
						condition.ConditionType
					});
					return false;
				}
				context.Trace("Condition '{0}' evaluated as Match", new object[]
				{
					condition.ConditionType
				});
			}
			return true;
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
