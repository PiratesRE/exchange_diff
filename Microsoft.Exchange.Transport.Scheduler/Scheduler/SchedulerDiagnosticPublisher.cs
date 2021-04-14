using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Scheduler.Contracts;
using Microsoft.Exchange.Transport.Scheduler.Processing;

namespace Microsoft.Exchange.Transport.Scheduler
{
	internal class SchedulerDiagnosticPublisher
	{
		public SchedulerDiagnosticPublisher(IProcessingSchedulerAdmin processingSchedulerAdmin)
		{
			ArgumentValidator.ThrowIfNull("processingSchedulerAdmin", processingSchedulerAdmin);
			this.processingSchedulerAdmin = processingSchedulerAdmin;
		}

		public void Publish()
		{
			SchedulerDiagnosticsInfo diagnosticsInfo = this.processingSchedulerAdmin.GetDiagnosticsInfo();
			this.perfCountersInstance.OldestLockAge.RawValue = this.GetAgeTicks(diagnosticsInfo.OldestLockTimeStamp);
			this.perfCountersInstance.OldestScopedQueueAge.RawValue = this.GetAgeTicks(diagnosticsInfo.OldestScopedQueueCreateTime);
			this.perfCountersInstance.TotalScopedQueues.RawValue = diagnosticsInfo.TotalScopedQueues;
			this.perfCountersInstance.TotalReceived.RawValue = diagnosticsInfo.Received;
			this.perfCountersInstance.TotalScheduled.RawValue = diagnosticsInfo.Dispatched;
			this.perfCountersInstance.ThrottlingRate.RawValue = diagnosticsInfo.ThrottleRate;
			this.perfCountersInstance.ScopedQueuesCreationRate.RawValue = diagnosticsInfo.ScopedQueuesCreatedRate;
			this.perfCountersInstance.ScopedQueuesRemovalRate.RawValue = diagnosticsInfo.ScopedQueuesDestroyedRate;
			this.perfCountersInstance.ProcessingVelocity.RawValue = diagnosticsInfo.ProcessRate - diagnosticsInfo.ReceiveRate;
		}

		private long GetAgeTicks(DateTime timeStamp)
		{
			if (!(timeStamp == DateTime.MaxValue))
			{
				return timeStamp.Ticks;
			}
			return DateTime.UtcNow.Ticks;
		}

		private readonly IProcessingSchedulerAdmin processingSchedulerAdmin;

		private SchedulerPerfCountersInstance perfCountersInstance = SchedulerPerfCounters.GetInstance("other");
	}
}
