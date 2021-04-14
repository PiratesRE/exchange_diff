using System;
using System.Collections.Generic;

namespace Microsoft.Office.CompliancePolicy.PolicyEvaluation
{
	public class LessThanPredicate : PredicateCondition
	{
		public LessThanPredicate(Property property, List<string> entries) : base(property, entries)
		{
			if (!base.Property.IsComparableType)
			{
				throw new CompliancePolicyValidationException(string.Format("Type {0} is not supported by Predicate '{1}'", base.Property.Type, this.Name));
			}
		}

		public override string Name
		{
			get
			{
				return "lessThan";
			}
		}

		public override Version MinimumVersion
		{
			get
			{
				if (base.Property.Type == typeof(long))
				{
					return LessThanPredicate.minVersion;
				}
				return base.MinimumVersion;
			}
		}

		public override bool Evaluate(PolicyEvaluationContext context)
		{
			return base.CompareComparablePropertyAndValue(context) < 0;
		}

		private static readonly Version minVersion = new Version("1.00.0002.000");
	}
}
