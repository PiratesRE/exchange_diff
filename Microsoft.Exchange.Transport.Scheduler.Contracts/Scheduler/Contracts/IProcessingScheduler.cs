using System;

namespace Microsoft.Exchange.Transport.Scheduler.Contracts
{
	internal interface IProcessingScheduler
	{
		void Submit(ISchedulableMessage message);

		bool TryGetNext(out ISchedulableMessage message);

		void SubscribeToEvent(SchedulingState state, SchedulingEventHandler handler);

		void UnsubscribeFromEvent(SchedulingState state, SchedulingEventHandler handler);
	}
}
