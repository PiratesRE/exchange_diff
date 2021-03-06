using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pim;

namespace Microsoft.Exchange.Transport.Sync.Common.Subscription.Pop
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class PopSubscriptionValidator
	{
		public static ICollection<ValidationError> Validate(PopSubscriptionProxy subscription)
		{
			List<ValidationError> list = new List<ValidationError>();
			list.AddRange(PimSubscriptionValidator.Validate(subscription));
			ValidationError validationError = AggregationSubscriptionConstraints.NameLengthConstraint.Validate(subscription.IncomingUserName, new SubscriptionProxyPropertyDefinition("IncomingUserName", typeof(string)), null);
			if (validationError != null)
			{
				list.Add(validationError);
			}
			ValidationError validationError2 = AggregationSubscriptionConstraints.PortRangeConstraint.Validate(subscription.IncomingPort, new SubscriptionProxyPropertyDefinition("IncomingPort", typeof(int)), null);
			if (validationError2 != null)
			{
				list.Add(validationError2);
			}
			return list;
		}
	}
}
