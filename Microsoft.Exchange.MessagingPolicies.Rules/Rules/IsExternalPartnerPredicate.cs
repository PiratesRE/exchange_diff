using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Transport.Internal;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal class IsExternalPartnerPredicate : PredicateCondition
	{
		public IsExternalPartnerPredicate(Property property, ShortList<string> entries, RulesCreationContext creationContext) : base(property, entries, creationContext)
		{
		}

		public override string Name
		{
			get
			{
				return "isExternalPartner";
			}
		}

		public override bool Evaluate(RulesEvaluationContext baseContext)
		{
			BaseTransportRulesEvaluationContext baseTransportRulesEvaluationContext = (BaseTransportRulesEvaluationContext)baseContext;
			baseTransportRulesEvaluationContext.PredicateName = this.Name;
			object value = base.Property.GetValue(baseTransportRulesEvaluationContext);
			return IsExternalPartnerPredicate.IsExternalPartner((string)value);
		}

		protected override Value BuildValue(ShortList<string> entries, RulesCreationContext creationContext)
		{
			if (entries.Count != 0)
			{
				throw new RulesValidationException(RulesStrings.ValueIsNotAllowed(this.Name));
			}
			return null;
		}

		internal static bool IsExternalPartner(string recipient)
		{
			if (string.IsNullOrEmpty(recipient))
			{
				return false;
			}
			string domainPart = ((RoutingAddress)recipient).DomainPart;
			return !string.IsNullOrEmpty(domainPart) && SmtpAddress.IsValidDomain(domainPart) && Configuration.TransportConfigObject.IsTLSSendSecureDomain(domainPart);
		}
	}
}
