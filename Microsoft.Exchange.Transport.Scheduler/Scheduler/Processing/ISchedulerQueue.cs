using System;
using Microsoft.Exchange.Transport.Scheduler.Contracts;

namespace Microsoft.Exchange.Transport.Scheduler.Processing
{
	internal interface ISchedulerQueue
	{
		bool IsEmpty { get; }

		long Count { get; }

		void Enqueue(ISchedulableMessage message);

		bool TryDequeue(out ISchedulableMessage message);

		bool TryPeek(out ISchedulableMessage message);
	}
}
