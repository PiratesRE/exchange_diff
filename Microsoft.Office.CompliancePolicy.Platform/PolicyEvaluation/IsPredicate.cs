using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Office.CompliancePolicy.PolicyEvaluation
{
	public class IsPredicate : PredicateCondition
	{
		public IsPredicate(Property property, List<string> entries) : base(property, entries)
		{
			if (!entries.Any<string>())
			{
				throw new ArgumentException("entries can not be empty");
			}
			if (!base.Property.IsEquatableType && !base.Property.IsEquatableCollectionType)
			{
				throw new CompliancePolicyValidationException(string.Format("Property type {0} is not supported by Predicate '{1}'", base.Property.Type, this.Name));
			}
		}

		public override string Name
		{
			get
			{
				return "is";
			}
		}

		public override Version MinimumVersion
		{
			get
			{
				if (base.Property.Type == typeof(string) || base.Property.IsCollectionOfType(typeof(string)) || base.Property.Type == typeof(Guid))
				{
					return base.MinimumVersion;
				}
				return IsPredicate.minVersion;
			}
		}

		public override bool Evaluate(PolicyEvaluationContext context)
		{
			if (!base.Property.IsEquatableTo(base.Value))
			{
				throw new CompliancePolicyValidationException(string.Format("Rule '{0}' is in inconsitent state due to unknown property '{1}'", context.CurrentRule.Name, base.Property.Name));
			}
			object value = base.Property.GetValue(context);
			object value2 = base.Value.GetValue(context);
			if (value == null && value2 == null)
			{
				return true;
			}
			if (value == null || value2 == null)
			{
				return false;
			}
			if (!Argument.IsTypeEquatable(value2.GetType()) && !Argument.IsTypeEquatableCollection(value2.GetType()))
			{
				throw new CompliancePolicyValidationException(string.Format("Rule '{0}' contains an invalid property '{1}'", context.CurrentRule.Name, base.Property.Name));
			}
			if (value.GetType() == typeof(string) || Argument.IsTypeCollectionOfType(value.GetType(), typeof(string)))
			{
				return PolicyUtils.CompareStringValues(value, value2, CaseInsensitiveStringComparer.Instance);
			}
			if (value.GetType() == typeof(Guid) || Argument.IsTypeCollectionOfType(value.GetType(), typeof(Guid)))
			{
				return PolicyUtils.CompareGuidValues(value, value2);
			}
			return PolicyUtils.CompareValues(value, value2);
		}

		private static readonly Version minVersion = new Version("1.00.0002.000");
	}
}
