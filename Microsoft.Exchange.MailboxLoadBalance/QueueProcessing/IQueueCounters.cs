using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.MailboxLoadBalance.QueueProcessing
{
	internal interface IQueueCounters
	{
		PerfCounterWithAverageRate IncomingRequestRateCounter { get; }

		PerfCounterWithAverageRate ExecutionRateCounter { get; }

		ExPerformanceCounter QueueLengthCounter { get; }
	}
}
