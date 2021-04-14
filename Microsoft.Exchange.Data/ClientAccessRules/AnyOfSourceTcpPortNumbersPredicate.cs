using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Core.RuleTasks;
using Microsoft.Exchange.MessagingPolicies.Rules;

namespace Microsoft.Exchange.Data.ClientAccessRules
{
	internal class AnyOfSourceTcpPortNumbersPredicate : PredicateCondition
	{
		public AnyOfSourceTcpPortNumbersPredicate(Property property, ShortList<string> valueEntries, RulesCreationContext creationContext) : base(property, valueEntries, creationContext)
		{
			if (!base.Property.IsString && !typeof(IntRange).IsAssignableFrom(base.Property.Type))
			{
				throw new RulesValidationException(RulesTasksStrings.ClientAccessRulesPortRangePropertyRequired(this.Name));
			}
		}

		public override string Name
		{
			get
			{
				return "anyOfSourceTcpPortNumbersPredicate";
			}
		}

		public override Version MinimumVersion
		{
			get
			{
				return AnyOfSourceTcpPortNumbersPredicate.PredicateBaseVersion;
			}
		}

		public IEnumerable<IntRange> TargetPortRanges
		{
			get
			{
				return (IEnumerable<IntRange>)base.Value.ParsedValue;
			}
		}

		public override bool Evaluate(RulesEvaluationContext baseContext)
		{
			ClientAccessRulesEvaluationContext clientAccessRulesEvaluationContext = (ClientAccessRulesEvaluationContext)baseContext;
			int discoveredValue = clientAccessRulesEvaluationContext.RemoteEndpoint.Port;
			return this.TargetPortRanges.Any((IntRange target) => target.Contains(discoveredValue));
		}

		protected override Value BuildValue(ShortList<string> entries, RulesCreationContext creationContext)
		{
			return Value.CreateValue(entries.Select(new Func<string, IntRange>(IntRange.Parse)));
		}

		public const string Tag = "anyOfSourceTcpPortNumbersPredicate";

		private static readonly Version PredicateBaseVersion = new Version("15.00.0008.00");
	}
}
