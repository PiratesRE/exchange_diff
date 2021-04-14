using System;
using System.Collections.Generic;

namespace Microsoft.Office.CompliancePolicy.PolicyEvaluation
{
	public class NotEqualPredicate : PredicateCondition
	{
		public NotEqualPredicate(Property property, List<string> entries) : base(property, entries)
		{
			if (!base.Property.IsEquatableType)
			{
				throw new CompliancePolicyValidationException(string.Format("Type {0} is not supported by Predicate '{1}'", base.Property.Type, this.Name));
			}
		}

		public override string Name
		{
			get
			{
				return "notEqual";
			}
		}

		public override Version MinimumVersion
		{
			get
			{
				if (base.Property.IsEnumType || base.Property.Type == typeof(long) || base.Property.Type == typeof(bool) || base.Property.Type == typeof(string) || base.Property.Type == typeof(Guid))
				{
					return NotEqualPredicate.minVersion;
				}
				return base.MinimumVersion;
			}
		}

		public override bool Evaluate(PolicyEvaluationContext context)
		{
			return !base.CompareEquatablePropertyAndValue(context);
		}

		private static readonly Version minVersion = new Version("1.00.0002.000");
	}
}
