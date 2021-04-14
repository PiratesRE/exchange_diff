using System;
using Microsoft.Exchange.Diagnostics.Components.MessagingPolicies;
using Microsoft.Exchange.TextProcessing;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal class AttachmentMatchesRegexPredicate : PredicateCondition
	{
		public AttachmentMatchesRegexPredicate(ShortList<string> entries, RulesCreationContext creationContext) : base(null, entries, creationContext)
		{
		}

		public override string Name
		{
			get
			{
				return "attachmentMatchesRegexPatterns";
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

		public override bool Evaluate(RulesEvaluationContext baseContext)
		{
			TransportRulesEvaluationContext transportRulesEvaluationContext = (TransportRulesEvaluationContext)baseContext;
			transportRulesEvaluationContext.PredicateName = this.Name;
			return AttachmentMatcher.AttachmentMatches(base.Value, baseContext, new AttachmentMatcher.TracingDelegate(ExTraceGlobals.TransportRulesEngineTracer.TraceDebug));
		}
	}
}
