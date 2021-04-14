using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.InfoWorker.Common.MessageTracking
{
	internal delegate bool TryEnqueueNextHopDelegate(MessageTrackingLogEntry terminalEvent, TrackingContext context, Queue<MailItemTracker> remainingTrackers);
}
