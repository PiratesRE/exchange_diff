using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Net.JobQueues
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class Configuration
	{
		public int MaxAllowedQueueLength { get; private set; }

		public int MaxAllowedPendingJobCount { get; private set; }

		public TimeSpan DispatcherWakeUpInterval { get; private set; }

		public Configuration(int maxAllowedQueueLength, int maxAllowedPendingJobCount, TimeSpan dispatcherWakeUpInterval)
		{
			this.MaxAllowedPendingJobCount = maxAllowedPendingJobCount;
			this.MaxAllowedQueueLength = maxAllowedQueueLength;
			this.DispatcherWakeUpInterval = dispatcherWakeUpInterval;
		}
	}
}
