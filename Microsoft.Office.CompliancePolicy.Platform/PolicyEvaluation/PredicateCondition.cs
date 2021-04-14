using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Office.CompliancePolicy.PolicyEvaluation
{
	public abstract class PredicateCondition : Condition
	{
		protected PredicateCondition()
		{
		}

		protected PredicateCondition(Property property, List<string> entries)
		{
			if (property == null)
			{
				throw new ArgumentNullException("property");
			}
			if (entries != null)
			{
				if (!entries.Any((string entry) => entry == null))
				{
					this.property = property;
					this.value = (this.BuildValue(entries) ?? Value.Empty);
					return;
				}
			}
			throw new ArgumentNullException("entries");
		}

		protected PredicateCondition(Property property, List<List<KeyValuePair<string, string>>> entries)
		{
			if (property == null)
			{
				throw new ArgumentNullException("property");
			}
			if (entries != null)
			{
				if (!entries.Any((List<KeyValuePair<string, string>> entry) => entry == null))
				{
					this.property = property;
					this.value = this.BuildValue(entries);
					return;
				}
			}
			throw new ArgumentNullException("entries");
		}

		public override ConditionType ConditionType
		{
			get
			{
				return ConditionType.Predicate;
			}
		}

		public Property Property
		{
			get
			{
				return this.property;
			}
			protected set
			{
				this.property = value;
			}
		}

		public Value Value
		{
			get
			{
				return this.value;
			}
			protected set
			{
				this.value = value;
			}
		}

		public abstract string Name { get; }

		protected virtual Value BuildValue(List<string> entries)
		{
			return Value.CreateValue((this.property == null) ? typeof(string) : this.property.Type, entries);
		}

		protected virtual Value BuildValue(List<List<KeyValuePair<string, string>>> entries)
		{
			return Value.CreateValue(entries);
		}

		protected int CompareComparablePropertyAndValue(PolicyEvaluationContext context)
		{
			if (!this.Property.IsComparableTo(this.Value))
			{
				throw new CompliancePolicyValidationException(string.Format("Rule '{0}' is in inconsitent state due to unknown property '{1}'", context.CurrentRule.Name, this.property.Name));
			}
			object obj = this.Property.GetValue(context);
			object obj2 = this.Value.GetValue(context);
			if (!(obj2 is IComparable))
			{
				throw new CompliancePolicyValidationException(string.Format("Rule '{0}' contains an invalid property '{1}'", context.CurrentRule.Name, this.Property.Name));
			}
			return ((IComparable)obj).CompareTo(obj2);
		}

		protected bool CompareEquatablePropertyAndValue(PolicyEvaluationContext context)
		{
			if (!this.Property.IsEquatableTo(this.Value))
			{
				throw new CompliancePolicyValidationException(string.Format("Rule '{0}' is in inconsitent state due to unknown property '{1}'", context.CurrentRule.Name, this.property.Name));
			}
			object obj = this.Property.GetValue(context);
			object obj2 = this.Value.GetValue(context);
			if (obj == null && obj2 == null)
			{
				return true;
			}
			if (obj == null || obj2 == null)
			{
				return false;
			}
			if (!Argument.IsTypeEquatable(obj2.GetType()))
			{
				throw new CompliancePolicyValidationException(string.Format("Rule '{0}' contains an invalid property '{1}'", context.CurrentRule.Name, this.Property.Name));
			}
			return obj.Equals(obj2);
		}

		private Property property;

		private Value value;
	}
}
