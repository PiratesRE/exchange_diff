using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Core.RuleTasks;
using Microsoft.Exchange.MessagingPolicies.Rules;

namespace Microsoft.Exchange.Data.ClientAccessRules
{
	internal class AnyOfAuthenticationTypesPredicate : PredicateCondition
	{
		public AnyOfAuthenticationTypesPredicate(Property property, ShortList<string> valueEntries, RulesCreationContext creationContext) : base(property, valueEntries, creationContext)
		{
			if (!base.Property.IsString && !typeof(ClientAccessAuthenticationMethod).IsAssignableFrom(base.Property.Type))
			{
				throw new RulesValidationException(RulesTasksStrings.ClientAccessRulesAuthenticationTypeRequired(this.Name));
			}
		}

		public override string Name
		{
			get
			{
				return "anyOfAuthenticationTypesPredicate";
			}
		}

		public override Version MinimumVersion
		{
			get
			{
				return AnyOfAuthenticationTypesPredicate.PredicateBaseVersion;
			}
		}

		public IEnumerable<ClientAccessAuthenticationMethod> AuthenticationTypeList
		{
			get
			{
				return (IEnumerable<ClientAccessAuthenticationMethod>)base.Value.ParsedValue;
			}
		}

		public override bool Evaluate(RulesEvaluationContext baseContext)
		{
			ClientAccessRulesEvaluationContext clientAccessRulesEvaluationContext = (ClientAccessRulesEvaluationContext)baseContext;
			ClientAccessAuthenticationMethod authenticationType = clientAccessRulesEvaluationContext.AuthenticationType;
			return this.AuthenticationTypeList.Contains(authenticationType);
		}

		protected override Value BuildValue(ShortList<string> entries, RulesCreationContext creationContext)
		{
			return Value.CreateValue(entries.Select(delegate(string authenticationTypeString)
			{
				int result;
				if (int.TryParse(authenticationTypeString, out result))
				{
					return (ClientAccessAuthenticationMethod)result;
				}
				return (ClientAccessAuthenticationMethod)Enum.Parse(typeof(ClientAccessAuthenticationMethod), authenticationTypeString);
			}));
		}

		public const string Tag = "anyOfAuthenticationTypesPredicate";

		private static readonly Version PredicateBaseVersion = new Version("15.00.0012.00");
	}
}
