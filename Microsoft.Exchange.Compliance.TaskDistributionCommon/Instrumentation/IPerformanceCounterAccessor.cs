using System;

namespace Microsoft.Exchange.Compliance.TaskDistributionCommon.Instrumentation
{
	internal interface IPerformanceCounterAccessor
	{
		void AddDequeueLatency(long latency);

		void AddProcessingCompletionEvent(ProcessingCompletionEvent pEvent, long latency);

		void AddProcessorEvent(ProcessorEvent pEvent);

		void AddQueueEvent(QueueEvent qEvent);

		void UpdateCounters();
	}
}
