using System;

namespace Microsoft.Exchange.Hygiene.Data.Directory
{
	public enum ReportExecutionStatusType : byte
	{
		None,
		Queued,
		Scheduled,
		Running,
		Pausing,
		Paused,
		Resuming,
		Completed,
		Failed,
		Cancelling,
		Cancelled
	}
}
