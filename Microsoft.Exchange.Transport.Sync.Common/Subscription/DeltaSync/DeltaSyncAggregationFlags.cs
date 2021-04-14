using System;

namespace Microsoft.Exchange.Transport.Sync.Common.Subscription.DeltaSync
{
	[Flags]
	internal enum DeltaSyncAggregationFlags
	{
		AggregationTypeMask = 48,
		AccountStatusBlocked = 256,
		AccountStatusHipped = 512,
		AccountStatusMask = 768
	}
}
