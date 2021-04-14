using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal class LegacyMatchesPredicate : PredicateCondition
	{
		public List<string> Patterns
		{
			get
			{
				return this.patterns;
			}
		}

		public LegacyMatchesPredicate(Property property, ShortList<string> entries, RulesCreationContext creationContext) : base(property, entries, creationContext)
		{
			this.patterns = new List<string>(entries);
			this.matchesRegexPredicate = new MatchesRegexPredicate(property, RegexUtils.ConvertLegacyRegexToTpl(entries), creationContext);
		}

		public override string Name
		{
			get
			{
				return "matches";
			}
		}

		public override bool Evaluate(RulesEvaluationContext context)
		{
			return this.matchesRegexPredicate.Evaluate(context);
		}

		protected override Value BuildValue(ShortList<string> entries, RulesCreationContext creationContext)
		{
			return Value.CreateValue(string.Empty.GetType(), entries);
		}

		private MatchesRegexPredicate matchesRegexPredicate;

		private List<string> patterns;
	}
}
