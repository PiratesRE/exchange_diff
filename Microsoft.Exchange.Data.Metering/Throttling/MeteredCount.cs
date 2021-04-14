using System;

namespace Microsoft.Exchange.Data.Metering.Throttling
{
	internal enum MeteredCount
	{
		AllQueue,
		AcceptedSubmissionQueue,
		AcceptedTotalQueue,
		CurrentRejectedSubmissionQueue,
		CurrentRejectedTotalQueue,
		RejectedSubmissionQueue,
		RejectedTotalQueue,
		TempRejected,
		PermanentRejected,
		Accepted,
		Deferred,
		Deprioritized,
		OutstandingJobs,
		Memory,
		ProcessingTicks
	}
}
