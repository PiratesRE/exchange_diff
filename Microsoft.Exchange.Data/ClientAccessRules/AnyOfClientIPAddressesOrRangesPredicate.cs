using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.Exchange.Core.RuleTasks;
using Microsoft.Exchange.MessagingPolicies.Rules;

namespace Microsoft.Exchange.Data.ClientAccessRules
{
	internal class AnyOfClientIPAddressesOrRangesPredicate : PredicateCondition
	{
		public AnyOfClientIPAddressesOrRangesPredicate(Property property, ShortList<string> valueEntries, RulesCreationContext creationContext) : base(property, valueEntries, creationContext)
		{
			if (!base.Property.IsString && !typeof(IPAddress).IsAssignableFrom(base.Property.Type))
			{
				throw new RulesValidationException(RulesTasksStrings.ClientAccessRulesIpPropertyRequired(this.Name));
			}
		}

		public override string Name
		{
			get
			{
				return "anyOfClientIPAddressesOrRangesPredicate";
			}
		}

		public override Version MinimumVersion
		{
			get
			{
				return AnyOfClientIPAddressesOrRangesPredicate.PredicateBaseVersion;
			}
		}

		public IEnumerable<IPRange> TargetIpRanges
		{
			get
			{
				return (IEnumerable<IPRange>)base.Value.ParsedValue;
			}
		}

		public override bool Evaluate(RulesEvaluationContext baseContext)
		{
			ClientAccessRulesEvaluationContext clientAccessRulesEvaluationContext = (ClientAccessRulesEvaluationContext)baseContext;
			IPAddress discoveredValue = clientAccessRulesEvaluationContext.RemoteEndpoint.Address;
			return this.TargetIpRanges.Any((IPRange target) => target.Contains(discoveredValue));
		}

		protected override Value BuildValue(ShortList<string> entries, RulesCreationContext creationContext)
		{
			return Value.CreateValue(entries.Select(new Func<string, IPRange>(IPRange.Parse)));
		}

		public const string Tag = "anyOfClientIPAddressesOrRangesPredicate";

		private static readonly Version PredicateBaseVersion = new Version("15.00.0008.00");
	}
}
