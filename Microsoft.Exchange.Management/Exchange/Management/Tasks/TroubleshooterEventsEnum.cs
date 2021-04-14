using System;

namespace Microsoft.Exchange.Management.Tasks
{
	public enum TroubleshooterEventsEnum
	{
		TSNoProblemsDetected = 5000,
		MailboxAssistantsServiceStopped,
		MailboxAssistantsServiceStarted,
		TSMDBperformanceCounterNotLoaded = 5100,
		TSMinServerVersion,
		TSNotAMailboxServer = 5200,
		MailboxAssistantsServiceNotRunning,
		MailboxAssistantsServiceCouldNotBeStopped,
		MailboxAssistantsServiceCouldNotBeStarted,
		TSResolutionFailed,
		AIMDBLastEventPollingThreadHung,
		AIDatabaseStatusPollThreadHung,
		AIMDBWatermarkTooLow
	}
}
