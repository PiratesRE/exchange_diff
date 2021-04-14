using System;

namespace Microsoft.Exchange.Hygiene.Data.AsyncQueue
{
	public enum AsyncQueueStatus : short
	{
		None,
		Pending = 5,
		Paused = 10,
		NotStarted = 30,
		InProgress = 40,
		ErrorRetry = 50,
		ErrorRetryTransient = 60,
		AutoPaused = 70,
		Continuous = 90,
		Completed = 100,
		CompletedWithError = 110,
		Failed = 120,
		Cancelled = 130,
		SystemCancelled = 140,
		Skipped = 150
	}
}
