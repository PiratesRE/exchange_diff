using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal class SenderAttributeMatchesPredicate : TextMatchingPredicate
	{
		public SenderAttributeMatchesPredicate(ShortList<string> entries, RulesCreationContext creationContext) : base(null, entries, creationContext)
		{
		}

		public override string Name
		{
			get
			{
				return "senderAttributeMatches";
			}
		}

		public override Version MinimumVersion
		{
			get
			{
				return TransportRuleConstants.VersionedContainerBaseVersion;
			}
		}

		protected override Value BuildValue(ShortList<string> entries, RulesCreationContext creationContext)
		{
			return Value.CreateValue(typeof(string[]), TransportUtils.BuildPatternListForUserAttributeMatchesPredicate(entries));
		}

		public override bool Evaluate(RulesEvaluationContext baseContext)
		{
			TransportRulesEvaluationContext context = (TransportRulesEvaluationContext)baseContext;
			context.PredicateName = this.Name;
			object value = base.Value.GetValue(context);
			List<string> attributes = new List<string>();
			string text = value as string;
			if (text != null)
			{
				attributes.Add(text);
			}
			else
			{
				attributes = (List<string>)value;
			}
			if (!attributes.Any<string>())
			{
				return false;
			}
			IEnumerable<string> source = (IEnumerable<string>)MessageProperty.GetMessageFromValue(context);
			return source.Any((string fromAddress) => TransportUtils.UserAttributeMatchesPatterns(context, fromAddress, attributes.ToArray(), this.Name));
		}
	}
}
