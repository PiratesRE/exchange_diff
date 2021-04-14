using System;
using Microsoft.Exchange.TextProcessing;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	public class MatchesRegexPredicate : TextMatchingPredicate
	{
		public MatchesRegexPredicate(Property property, ShortList<string> entries, RulesCreationContext creationContext) : base(property, entries, creationContext)
		{
		}

		public override string Name
		{
			get
			{
				return "matchesRegex";
			}
		}

		public override Version MinimumVersion
		{
			get
			{
				return Rule.BaseVersion15;
			}
		}

		protected override Value BuildValue(ShortList<string> entries, RulesCreationContext creationContext)
		{
			return MatchesRegexPredicate.BuildValueFromList(entries, creationContext);
		}

		public static Value BuildValueFromList(ShortList<string> entries, RulesCreationContext creationContext)
		{
			Value result;
			try
			{
				MultiMatcher multiMatcher = new MultiMatcher();
				foreach (string pattern in entries)
				{
					multiMatcher.Add(creationContext.MatchFactory.CreateRegex(pattern, CaseSensitivityMode.Insensitive, MatchRegexOptions.ExplicitCaptures, MatchesRegexPredicate.RegexMatchTimeout));
				}
				result = Value.CreateValue(multiMatcher, entries);
			}
			catch (ArgumentException innerException)
			{
				throw new RulesValidationException(TextMatchingStrings.RegexInternalParsingError, innerException);
			}
			return result;
		}

		public static readonly TimeSpan RegexMatchTimeout = new TimeSpan(0, 0, 30);
	}
}
