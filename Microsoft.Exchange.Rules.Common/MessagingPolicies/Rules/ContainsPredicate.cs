using System;
using Microsoft.Exchange.TextProcessing;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	public class ContainsPredicate : TextMatchingPredicate
	{
		public ContainsPredicate(Property property, ShortList<string> entries, RulesCreationContext creationContext) : base(property, entries, creationContext)
		{
		}

		public override string Name
		{
			get
			{
				return "contains";
			}
		}

		public override Version MinimumVersion
		{
			get
			{
				if (string.CompareOrdinal(base.Property.Name, "Message.SenderDomain") == 0)
				{
					return ContainsPredicate.SenderDomainContainsWordsVersion;
				}
				if (string.CompareOrdinal(base.Property.Name, "Message.ContentCharacterSets") == 0)
				{
					return ContainsPredicate.ContentCharacterSetContainsWordsVersion;
				}
				return base.MinimumVersion;
			}
		}

		protected override Value BuildValue(ShortList<string> entries, RulesCreationContext creationContext)
		{
			Value result;
			try
			{
				MultiMatcher multiMatcher = new MultiMatcher();
				IMatch matcher = creationContext.MatchFactory.CreateSingleExecutionTermSet(entries);
				multiMatcher.Add(matcher);
				result = Value.CreateValue(multiMatcher, entries);
			}
			catch (ArgumentException innerException)
			{
				throw new RulesValidationException(TextMatchingStrings.KeywordInternalParsingError, innerException);
			}
			return result;
		}

		public static readonly Version SenderDomainContainsWordsVersion = new Version("15.00.0005.00");

		public static readonly Version ContentCharacterSetContainsWordsVersion = new Version("15.00.0005.01");
	}
}
