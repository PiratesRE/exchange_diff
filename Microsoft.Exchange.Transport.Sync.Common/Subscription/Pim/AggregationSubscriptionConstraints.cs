using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Sync.Common.Subscription.Pim
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class AggregationSubscriptionConstraints
	{
		public static readonly StringLengthConstraint NameLengthConstraint = new StringLengthConstraint(1, 256);

		public static readonly RangedValueConstraint<int> PortRangeConstraint = new RangedValueConstraint<int>(1, 65535);

		public static readonly RangedValueConstraint<int> PasswordRangeConstraint = new RangedValueConstraint<int>(1, 256);
	}
}
