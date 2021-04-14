using System;
using Microsoft.Exchange.MessagingPolicies.Rules;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class SentToPredicate : IsSameUserPredicate
	{
		public SentToPredicate(Property property, ShortList<string> addressesToCompare, RulesCreationContext creationContext) : base(property, addressesToCompare, creationContext)
		{
		}

		public override string Name
		{
			get
			{
				return "SentTo";
			}
		}

		public void SetValue(ShortList<string> addressesToCompare)
		{
			if (addressesToCompare != null && addressesToCompare.Count > 0)
			{
				this.value = this.BuildValue(addressesToCompare, null);
			}
		}

		protected override object GetConditionValue(BaseTransportRulesEvaluationContext context)
		{
			if (!this.areAdressesExpanded)
			{
				OwaRulesEvaluationContext owaRulesEvaluationContext = context as OwaRulesEvaluationContext;
				this.SetValue(ADUtils.GetAllEmailAddresses(base.Value.RawValues, owaRulesEvaluationContext.OrganizationId));
				this.areAdressesExpanded = true;
			}
			return base.Value.GetValue(context);
		}

		private bool areAdressesExpanded;
	}
}
