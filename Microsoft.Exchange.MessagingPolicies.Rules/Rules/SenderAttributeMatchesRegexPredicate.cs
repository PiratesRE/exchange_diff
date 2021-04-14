using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal class SenderAttributeMatchesRegexPredicate : PredicateCondition
	{
		public SenderAttributeMatchesRegexPredicate(ShortList<string> entries, RulesCreationContext creationContext) : base(null, entries, creationContext)
		{
		}

		public override string Name
		{
			get
			{
				return "senderAttributeMatchesRegex";
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
			return Value.CreateValue(typeof(string[]), entries);
		}

		public override bool Evaluate(RulesEvaluationContext baseContext)
		{
			TransportRulesEvaluationContext transportRulesEvaluationContext = (TransportRulesEvaluationContext)baseContext;
			transportRulesEvaluationContext.PredicateName = this.Name;
			object value = base.Value.GetValue(transportRulesEvaluationContext);
			List<string> list = new List<string>();
			string text = value as string;
			if (text != null)
			{
				list.Add(text);
			}
			else
			{
				list = (List<string>)value;
			}
			return transportRulesEvaluationContext.MailItem.FromAddress.IsValid && list.Count != 0 && TransportUtils.UserAttributeMatchesPatterns(transportRulesEvaluationContext, transportRulesEvaluationContext.MailItem.FromAddress.ToString(), list.ToArray(), this.Name);
		}
	}
}
