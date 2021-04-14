using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	public abstract class TextMatchingPredicate : PredicateCondition
	{
		public List<string> Patterns { get; set; }

		public TextMatchingPredicate(Property property, ShortList<string> entries, RulesCreationContext creationContext) : base(property, entries, creationContext)
		{
			if (base.Property != null && !base.Property.IsString && base.Property.Type != typeof(IContent))
			{
				throw new RulesValidationException(RulesStrings.SearchablePropertyRequired);
			}
			this.Patterns = new List<string>(entries);
		}

		public override bool Evaluate(RulesEvaluationContext context)
		{
			if (this.property == null)
			{
				throw new InvalidOperationException(RulesStrings.InvalidPropertyType);
			}
			object value = base.Property.GetValue(context);
			MultiMatcher multiMatcher = base.Value.GetValue(context) as MultiMatcher;
			if (multiMatcher == null)
			{
				throw new InvalidOperationException(RulesStrings.InvalidValue("MatchAnyIMatch"));
			}
			bool flag;
			if (base.Property.Type == typeof(string))
			{
				flag = TextMatchingPredicate.IsMatch((string)value, base.Property.Name, multiMatcher, context);
			}
			else if (base.Property.Type == typeof(string[]) || base.Property.Type == typeof(List<string>))
			{
				flag = TextMatchingPredicate.IsMatch((IEnumerable<string>)value, base.Property.Name, multiMatcher, context);
			}
			else
			{
				if (!(base.Property.Type == typeof(IContent)))
				{
					throw new InvalidOperationException(RulesStrings.InvalidPropertyType);
				}
				flag = ((IContent)value).Matches(multiMatcher, context);
			}
			context.Trace("Text match condition evaluated as {0}Match", new object[]
			{
				flag ? string.Empty : "Not "
			});
			return flag;
		}

		private static bool IsMatch(string text, string propertyName, MultiMatcher matcher, RulesEvaluationContext rulesEvaluationContext)
		{
			return text != null && matcher.IsMatch(text, propertyName, rulesEvaluationContext);
		}

		private static bool IsMatch(IEnumerable<string> value, string propertyName, MultiMatcher matcher, RulesEvaluationContext rulesEvaluationContext)
		{
			int num = 0;
			foreach (string text in value)
			{
				if (TextMatchingPredicate.IsMatch(text, propertyName + num, matcher, rulesEvaluationContext))
				{
					return true;
				}
				num++;
			}
			return false;
		}
	}
}
