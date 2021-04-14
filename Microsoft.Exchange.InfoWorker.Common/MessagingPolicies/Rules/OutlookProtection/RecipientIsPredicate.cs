using System;

namespace Microsoft.Exchange.MessagingPolicies.Rules.OutlookProtection
{
	internal sealed class RecipientIsPredicate : PredicateCondition
	{
		public RecipientIsPredicate(ShortList<string> entries, RulesCreationContext creationContext) : base(new StringProperty("Message.ToCcBcc"), entries, creationContext)
		{
		}

		public override string Name
		{
			get
			{
				return "recipientIs";
			}
		}

		public override bool Evaluate(RulesEvaluationContext context)
		{
			throw new NotSupportedException("Outlook Protection rules are only evaluated on Outlook.");
		}
	}
}
