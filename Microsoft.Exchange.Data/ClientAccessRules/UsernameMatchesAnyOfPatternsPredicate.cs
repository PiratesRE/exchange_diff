using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Core.RuleTasks;
using Microsoft.Exchange.MessagingPolicies.Rules;

namespace Microsoft.Exchange.Data.ClientAccessRules
{
	internal class UsernameMatchesAnyOfPatternsPredicate : PredicateCondition
	{
		public UsernameMatchesAnyOfPatternsPredicate(Property property, ShortList<string> valueEntries, RulesCreationContext creationContext) : base(property, valueEntries, creationContext)
		{
			if (!base.Property.IsString && !typeof(string).IsAssignableFrom(base.Property.Type))
			{
				throw new RulesValidationException(RulesTasksStrings.ClientAccessRulesUsernamePatternRequired(this.Name));
			}
		}

		public override string Name
		{
			get
			{
				return "usernameMatchesAnyOfPatternsPredicate";
			}
		}

		public override Version MinimumVersion
		{
			get
			{
				return UsernameMatchesAnyOfPatternsPredicate.PredicateBaseVersion;
			}
		}

		public IEnumerable<Regex> RegexPatterns
		{
			get
			{
				return (IEnumerable<Regex>)base.Value.ParsedValue;
			}
		}

		public override bool Evaluate(RulesEvaluationContext baseContext)
		{
			ClientAccessRulesEvaluationContext clientAccessRulesEvaluationContext = (ClientAccessRulesEvaluationContext)baseContext;
			string username = clientAccessRulesEvaluationContext.UserName;
			return this.RegexPatterns.Any((Regex target) => target.IsMatch(username));
		}

		protected override Value BuildValue(ShortList<string> entries, RulesCreationContext creationContext)
		{
			return Value.CreateValue(entries.Select(new Func<string, Regex>(ClientAccessRulesUsernamePatternProperty.GetWildcardPatternRegex)));
		}

		public const string Tag = "usernameMatchesAnyOfPatternsPredicate";

		private static readonly Version PredicateBaseVersion = new Version("15.00.0011.00");
	}
}
