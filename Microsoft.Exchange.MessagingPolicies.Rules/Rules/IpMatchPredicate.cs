using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal class IpMatchPredicate : PredicateCondition
	{
		public IpMatchPredicate(Property property, ShortList<string> valueEntries, RulesCreationContext creationContext) : base(property, valueEntries, creationContext)
		{
			if (!base.Property.IsString && !typeof(IPAddress).IsAssignableFrom(base.Property.Type))
			{
				throw new RulesValidationException(TransportRulesStrings.IpMatchPropertyRequired(this.Name));
			}
		}

		public override string Name
		{
			get
			{
				return "ipMatch";
			}
		}

		public override Version MinimumVersion
		{
			get
			{
				return IpMatchPredicate.IpMatchBaseVersion;
			}
		}

		protected override Value BuildValue(ShortList<string> entries, RulesCreationContext creationContext)
		{
			if (entries.Count == 0)
			{
				throw new RulesValidationException(RulesStrings.StringPropertyOrValueRequired(this.Name));
			}
			this.targetIpRanges = entries.Select(new Func<string, IPRange>(IPRange.Parse)).ToList<IPRange>();
			return Value.CreateValue(typeof(string), entries);
		}

		public override bool Evaluate(RulesEvaluationContext baseContext)
		{
			TransportRulesEvaluationContext transportRulesEvaluationContext = (TransportRulesEvaluationContext)baseContext;
			transportRulesEvaluationContext.PredicateName = this.Name;
			object value = base.Property.GetValue(transportRulesEvaluationContext);
			if (value == null)
			{
				return false;
			}
			IPAddress discoveredValue = (IPAddress)value;
			return this.targetIpRanges.Any((IPRange target) => target.Contains(discoveredValue));
		}

		internal static readonly Version IpMatchBaseVersion = new Version("15.00.0002.00");

		private List<IPRange> targetIpRanges = new List<IPRange>();
	}
}
