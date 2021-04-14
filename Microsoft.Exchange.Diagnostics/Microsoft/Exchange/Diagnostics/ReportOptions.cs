using System;

namespace Microsoft.Exchange.Diagnostics
{
	[Flags]
	public enum ReportOptions
	{
		None = 0,
		ReportTerminateAfterSend = 1,
		DoNotCollectDumps = 2,
		DeepStackTraceHash = 4,
		DoNotLogProcessAndThreadIds = 8,
		DoNotFreezeThreads = 16
	}
}
