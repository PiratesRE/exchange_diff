using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Flags]
	public enum ReportEntryFlags
	{
		None = 0,
		BadItem = 1,
		Failure = 2,
		ConfigObject = 4,
		MailboxSize = 8,
		Fatal = 16,
		Cleanup = 32,
		Primary = 256,
		Archive = 512,
		Source = 1024,
		Target = 2048,
		Before = 4096,
		After = 8192,
		MailboxVerificationResults = 16384,
		TargetThrottleDurations = 32768,
		SourceThrottleDurations = 65536,
		SessionStatistics = 131072
	}
}
