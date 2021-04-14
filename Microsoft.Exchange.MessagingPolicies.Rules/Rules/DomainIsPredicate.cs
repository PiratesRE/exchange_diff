using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal class DomainIsPredicate : PredicateCondition
	{
		public DomainIsPredicate(Property property, ShortList<string> valueEntries, RulesCreationContext creationContext) : base(property, valueEntries, creationContext)
		{
			if (!base.Property.IsString || !base.Value.IsString)
			{
				throw new RulesValidationException(RulesStrings.StringPropertyOrValueRequired(this.Name));
			}
		}

		public override string Name
		{
			get
			{
				return "domainIs";
			}
		}

		public override Version MinimumVersion
		{
			get
			{
				return DomainIsPredicate.DomainIsBaseVersion;
			}
		}

		public override bool Evaluate(RulesEvaluationContext context)
		{
			TransportRulesEvaluationContext transportRulesEvaluationContext = context as TransportRulesEvaluationContext;
			if (transportRulesEvaluationContext != null)
			{
				transportRulesEvaluationContext.PredicateName = this.Name;
			}
			object value = base.Property.GetValue(context);
			object predicateValue = base.Value.GetValue(context);
			IEnumerable<string> enumerable = value as IEnumerable<string>;
			if (enumerable != null)
			{
				return enumerable.Any((string s) => DomainIsPredicate.DomainIs(predicateValue, s, this.Name));
			}
			string text = value as string;
			return !string.IsNullOrEmpty(text) && DomainIsPredicate.DomainIs(predicateValue, text, this.Name);
		}

		internal static bool DomainIs(object domainsFromThePredicate, string domainFromTheMessage, string predicateName)
		{
			string text = domainsFromThePredicate as string;
			if (text != null)
			{
				return DomainIsPredicate.IsSubdomainOf(text, domainFromTheMessage);
			}
			IEnumerable<string> enumerable = domainsFromThePredicate as IEnumerable<string>;
			if (enumerable != null)
			{
				return enumerable.Any((string s) => DomainIsPredicate.IsSubdomainOf(s, domainFromTheMessage));
			}
			throw new RulesValidationException(TransportRulesStrings.InvalidPropertyValueType(predicateName));
		}

		internal static bool IsSubdomainOf(string domain, string subDomain)
		{
			return domain != null && subDomain != null && (subDomain.Equals(domain, StringComparison.InvariantCultureIgnoreCase) || subDomain.EndsWith("." + domain, StringComparison.InvariantCultureIgnoreCase));
		}

		internal static readonly Version DomainIsBaseVersion = new Version("15.00.0005.02");
	}
}
