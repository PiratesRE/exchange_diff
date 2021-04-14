using System;
using System.Collections.Generic;
using Microsoft.Exchange.Transport.Scheduler.Contracts;

namespace Microsoft.Exchange.Transport.Scheduler.Processing
{
	internal interface ISchedulerDiagnostics
	{
		void Received();

		void Dispatched();

		void Throttled();

		void Processed();

		void ScopedQueueCreated(int count);

		void ScopedQueueDestroyed(int count);

		void VisitCurrentScopedQueues(IDictionary<IMessageScope, ScopedQueue> currentQueues);

		void RegisterQueueLogging(IQueueLogProvider logProvider);

		SchedulerDiagnosticsInfo GetDiagnosticsInfo();
	}
}
