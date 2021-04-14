using System;

namespace Microsoft.Exchange.Transport.ShadowRedundancy
{
	internal delegate bool DelayedAckItemHandler(object state, DelayedAckCompletionStatus status, TimeSpan delay, string context);
}
