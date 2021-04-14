using System;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	public class GreaterThanOrEqualPredicate : PredicateCondition
	{
		public GreaterThanOrEqualPredicate(Property property, ShortList<string> entries, RulesCreationContext creationContext) : base(property, entries, creationContext)
		{
			if (!base.Property.IsNumerical)
			{
				throw new RulesValidationException(RulesStrings.NumericalPropertyRequiredForPredicate(this.Name));
			}
		}

		public override string Name
		{
			get
			{
				return "greaterThanOrEqual";
			}
		}

		public override Version MinimumVersion
		{
			get
			{
				if (string.CompareOrdinal(base.Property.Name, "Message.Size") == 0)
				{
					return Rule.BaseVersion15;
				}
				return base.MinimumVersion;
			}
		}

		public override bool Evaluate(RulesEvaluationContext context)
		{
			bool flag = base.ComparePropertyAndValue(context) >= 0;
			base.UpdateEvaluationHistory(context, flag, null, 0);
			return flag;
		}
	}
}
