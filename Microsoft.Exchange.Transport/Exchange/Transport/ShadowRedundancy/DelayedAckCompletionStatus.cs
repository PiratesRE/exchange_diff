using System;

namespace Microsoft.Exchange.Transport.ShadowRedundancy
{
	internal enum DelayedAckCompletionStatus
	{
		Delivered,
		Expired,
		Skipped
	}
}
