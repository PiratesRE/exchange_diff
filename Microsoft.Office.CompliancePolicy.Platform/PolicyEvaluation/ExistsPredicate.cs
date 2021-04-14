using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Office.CompliancePolicy.PolicyEvaluation
{
	public class ExistsPredicate : PredicateCondition
	{
		public ExistsPredicate(Property property, List<string> entries) : base(property, entries)
		{
		}

		public override string Name
		{
			get
			{
				return "exists";
			}
		}

		public override bool Evaluate(PolicyEvaluationContext context)
		{
			object value = base.Property.GetValue(context);
			if (value == null)
			{
				return false;
			}
			IEnumerable<string> enumerable = value as IEnumerable<string>;
			return enumerable == null || enumerable.Any<string>();
		}

		protected override Value BuildValue(List<string> entries)
		{
			if (entries.Count != 0)
			{
				throw new CompliancePolicyValidationException(string.Format("Predicate '{0}' does not support values", this.Name));
			}
			return null;
		}
	}
}
