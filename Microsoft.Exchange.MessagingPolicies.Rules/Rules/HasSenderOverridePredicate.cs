using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal class HasSenderOverridePredicate : PredicateCondition
	{
		public HasSenderOverridePredicate(Property property, ShortList<string> entries, RulesCreationContext creationContext) : base(property, entries, creationContext)
		{
			if (!base.Property.IsString)
			{
				throw new RulesValidationException(RulesStrings.StringPropertyOrValueRequired(this.Name));
			}
		}

		public override string Name
		{
			get
			{
				return "hasSenderOverride";
			}
		}

		public override Version MinimumVersion
		{
			get
			{
				return HasSenderOverridePredicate.ComplianceProgramsBaseVersion;
			}
		}

		public override bool Evaluate(RulesEvaluationContext baseContext)
		{
			TransportRulesEvaluationContext transportRulesEvaluationContext = (TransportRulesEvaluationContext)baseContext;
			transportRulesEvaluationContext.PredicateName = this.Name;
			if (!TransportUtils.IsInternalMail(transportRulesEvaluationContext))
			{
				return false;
			}
			IEnumerable<string> canonicalizedStringProperty = TransportUtils.GetCanonicalizedStringProperty(base.Property, transportRulesEvaluationContext);
			if (canonicalizedStringProperty.Any((string thisValue) => !string.IsNullOrWhiteSpace(thisValue)))
			{
				transportRulesEvaluationContext.SenderOverridden = true;
				return true;
			}
			return false;
		}

		protected override Value BuildValue(ShortList<string> entries, RulesCreationContext creationContext)
		{
			return Value.Empty;
		}

		internal static readonly Version ComplianceProgramsBaseVersion = new Version("15.00.0001.00");
	}
}
