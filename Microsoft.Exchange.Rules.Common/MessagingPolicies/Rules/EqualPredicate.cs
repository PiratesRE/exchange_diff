﻿using System;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	public class EqualPredicate : PredicateCondition
	{
		public EqualPredicate(Property property, ShortList<string> entries, RulesCreationContext creationContext) : base(property, entries, creationContext)
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
				return "equal";
			}
		}

		public override bool Evaluate(RulesEvaluationContext context)
		{
			bool flag = base.ComparePropertyAndValue(context) == 0;
			base.UpdateEvaluationHistory(context, flag, null, 0);
			return flag;
		}
	}
}
