using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	public abstract class PredicateCondition : Condition
	{
		protected PredicateCondition()
		{
		}

		protected PredicateCondition(Property property, ShortList<string> entries, RulesCreationContext context)
		{
			this.property = property;
			this.value = this.BuildValue(entries, context);
			if (this.value == null)
			{
				this.value = Value.Empty;
			}
			this.UpdateSize(context);
		}

		protected PredicateCondition(Property property, ShortList<ShortList<KeyValuePair<string, string>>> entries, RulesCreationContext creationContext)
		{
			this.property = property;
			this.value = this.BuildValue(entries, creationContext);
			this.UpdateSize(creationContext);
		}

		protected void UpdateSize(RulesCreationContext creationContext)
		{
			creationContext.ConditionAndActionSize += 18;
			if (this.property != null)
			{
				creationContext.ConditionAndActionSize += this.property.GetEstimatedSize();
			}
			if (this.value != null)
			{
				creationContext.ConditionAndActionSize += this.value.GetEstimatedSize();
			}
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
		}

		public Value Value
		{
			get
			{
				return this.value;
			}
		}

		public abstract string Name { get; }

		protected virtual Value BuildValue(ShortList<string> entries, RulesCreationContext creationContext)
		{
			return Value.CreateValue(this.property.Type, entries);
		}

		protected virtual Value BuildValue(ShortList<ShortList<KeyValuePair<string, string>>> entries, RulesCreationContext creationContext)
		{
			return Value.CreateValue(entries);
		}

		protected int ComparePropertyAndValue(RulesEvaluationContext context)
		{
			if (!this.property.IsNumerical || !this.Value.IsNumerical)
			{
				throw new InvalidOperationException(RulesStrings.InvalidPropertyType);
			}
			object obj = this.property.GetValue(context);
			object obj2 = this.Value.GetValue(context);
			IComparable comparable = obj as IComparable;
			if (comparable == null)
			{
				throw new RuleInvalidOperationException(RulesStrings.InvalidPropertyForRule(this.property.Name, context.CurrentRule.Name));
			}
			context.Trace("Comparing '{0}' with '{1}'", new object[]
			{
				obj.ToString(),
				(obj2 != null) ? obj2.ToString() : "null"
			});
			return comparable.CompareTo(obj2);
		}

		protected void UpdateEvaluationHistory(RulesEvaluationContext context, bool isMatch, List<string> matches, int supplementalInfo = 0)
		{
			if (context != null)
			{
				List<string> list = matches ?? new List<string>
				{
					this.Value.GetValue(context).ToString()
				};
				context.RulesEvaluationHistory.AddPredicateEvaluationResult(context, base.GetType(), isMatch, list, supplementalInfo);
				context.Trace("Condition '{0}' evaluated as '{1}'. {2}", new object[]
				{
					base.GetType().ToString(),
					isMatch ? "Match" : "Not Match",
					isMatch ? string.Join(",", list) : string.Empty
				});
			}
		}

		protected Property property;

		protected Value value;
	}
}
