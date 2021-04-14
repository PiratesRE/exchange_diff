using System;

namespace Microsoft.Exchange.Transport.LoggingCommon
{
	internal enum ThrottlingResource
	{
		SubmissionQueue,
		SubmissionQueueQuota,
		TotalQueueQuota,
		FreeDiskSpace,
		Memory,
		VersionBuckets,
		SmtpIn,
		MaxLinesReached,
		Threads,
		Threads_MaxPerHub,
		Threads_PendingConnectionTimedOut
	}
}
