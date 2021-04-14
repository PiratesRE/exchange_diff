using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.MailboxLoadBalance.QueueProcessing
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ProcessingQueueCounters : IQueueCounters
	{
		public ProcessingQueueCounters(string queueName)
		{
			MailboxLoadBalanceMultiInstancePerformanceCountersInstance instance = MailboxLoadBalanceMultiInstancePerformanceCounters.GetInstance(queueName);
			this.ExecutionRateCounter = new PerfCounterWithAverageRate(null, instance.ProcessingRate, instance.ProcessingRateBase, 1, TimeSpan.FromHours(1.0));
			this.IncomingRequestRateCounter = new PerfCounterWithAverageRate(null, instance.ProcessingRequestRate, instance.ProcessingRequestRateBase, 1, TimeSpan.FromHours(1.0));
			this.QueueLengthCounter = instance.ProcessingQueueLength;
		}

		public PerfCounterWithAverageRate IncomingRequestRateCounter { get; private set; }

		public PerfCounterWithAverageRate ExecutionRateCounter { get; private set; }

		public ExPerformanceCounter QueueLengthCounter { get; private set; }
	}
}
