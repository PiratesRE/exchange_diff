using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class FromSubscriptionCondition : GuidCondition
	{
		private FromSubscriptionCondition(Rule rule, Guid[] subscriptionGuids) : base(ConditionType.FromSubscriptionCondition, rule, subscriptionGuids)
		{
		}

		public static FromSubscriptionCondition Create(Rule rule, params Guid[] subscriptionGuids)
		{
			return new FromSubscriptionCondition(rule, subscriptionGuids);
		}

		public override Rule.ProviderIdEnum ProviderId
		{
			get
			{
				return Rule.ProviderIdEnum.Exchange14;
			}
		}

		internal override Restriction BuildRestriction()
		{
			PropTag propertyTag = base.Rule.PropertyDefinitionToPropTagFromCache(InternalSchema.SharingInstanceGuid);
			return Condition.CreateORGuidContentRestriction(base.Guids, propertyTag);
		}
	}
}
