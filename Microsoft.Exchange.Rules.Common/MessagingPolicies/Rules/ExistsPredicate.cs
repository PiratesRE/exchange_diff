using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	public class ExistsPredicate : PredicateCondition
	{
		public ExistsPredicate(Property property, ShortList<string> entries, RulesCreationContext creationContext) : base(property, entries, creationContext)
		{
		}

		public override string Name
		{
			get
			{
				return "exists";
			}
		}

		public override bool Evaluate(RulesEvaluationContext context)
		{
			object value = base.Property.GetValue(context);
			if (value == null)
			{
				return false;
			}
			IEnumerable<string> enumerable = value as IEnumerable<string>;
			if (enumerable != null)
			{
				bool flag = enumerable.Any<string>();
				base.UpdateEvaluationHistory(context, flag, enumerable.ToList<string>(), 0);
				return flag;
			}
			base.UpdateEvaluationHistory(context, true, null, 0);
			return true;
		}

		protected override Value BuildValue(ShortList<string> entries, RulesCreationContext creationContext)
		{
			if (entries.Count != 0)
			{
				throw new RulesValidationException(RulesStrings.ValueIsNotAllowed(this.Name));
			}
			return null;
		}
	}
}
