using System;

namespace Microsoft.Exchange.Hygiene.Data.BackgroundJobBackend
{
	public enum TaskCompletionStatusType : byte
	{
		None,
		Success,
		Timedout,
		Failed,
		NonConformingJobDef,
		ScheduleDeactivated,
		ScheduleMissing,
		ExecutableMissing
	}
}
