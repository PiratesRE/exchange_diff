using System;

namespace Microsoft.Exchange.Transport.Scheduler.Contracts
{
	internal delegate void SchedulingEventHandler(SchedulingState state, ISchedulableMessage message);
}
