using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.MailboxLoadBalance.QueueProcessing
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class InjectionQueueCounters : IQueueCounters
	{
		public InjectionQueueCounters(string queueName)
		{
			MailboxLoadBalanceMultiInstancePerformanceCountersInstance instance = MailboxLoadBalanceMultiInstancePerformanceCounters.GetInstance(queueName);
			this.ExecutionRateCounter = new PerfCounterWithAverageRate(null, instance.InjectionRate, instance.InjectionRateBase, 1, TimeSpan.FromHours(1.0));
			this.IncomingRequestRateCounter = new PerfCounterWithAverageRate(null, instance.InjectionRequestRate, instance.InjectionRequestRateBase, 1, TimeSpan.FromHours(1.0));
			this.QueueLengthCounter = instance.InjectionQueueLength;
		}

		public PerfCounterWithAverageRate IncomingRequestRateCounter { get; private set; }

		public PerfCounterWithAverageRate ExecutionRateCounter { get; private set; }

		public ExPerformanceCounter QueueLengthCounter { get; private set; }
	}
}
