using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Sync.Common.Subscription.Pim
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class PimSubscriptionValidator
	{
		public static ICollection<ValidationError> Validate(PimSubscriptionProxy subscription)
		{
			List<ValidationError> list = new List<ValidationError>();
			ValidationError validationError = AggregationSubscriptionConstraints.NameLengthConstraint.Validate(subscription.Name, new SubscriptionProxyPropertyDefinition("Name", typeof(string)), null);
			if (validationError != null)
			{
				list.Add(validationError);
			}
			ValidationError validationError2 = AggregationSubscriptionConstraints.NameLengthConstraint.Validate(subscription.DisplayName, new SubscriptionProxyPropertyDefinition("DisplayName", typeof(string)), null);
			if (validationError2 != null)
			{
				list.Add(validationError2);
			}
			return list;
		}
	}
}
