using System;

namespace Microsoft.Exchange.MessagingPolicies.Rules.OutlookProtection
{
	internal sealed class AllInternalPredicate : PredicateCondition
	{
		public AllInternalPredicate() : base(new StringProperty("Message.ToCcBcc"), new ShortList<string>(), new RulesCreationContext())
		{
		}

		public override string Name
		{
			get
			{
				return "allInternal";
			}
		}

		public override bool Evaluate(RulesEvaluationContext context)
		{
			throw new NotSupportedException("Outlook Protection rules are only evaluated on Outlook.");
		}

		protected override Value BuildValue(ShortList<string> entries, RulesCreationContext creationContext)
		{
			return null;
		}
	}
}
