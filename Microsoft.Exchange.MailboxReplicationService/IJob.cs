using System;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal interface IJob
	{
		JobState State { get; }

		bool ShouldWakeup { get; }

		JobSortKey JobSortKey { get; }

		MrsSystemTask GetTask(SystemWorkloadBase systemWorkload, ResourceReservationContext context);

		void ProcessTaskExecutionResult(MrsSystemTask systemTask);

		void RevertToPreviousUnthrottledState();

		void WaitForJobToBeDone();

		void ResetJob();
	}
}
