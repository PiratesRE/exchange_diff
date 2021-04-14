using System;

namespace Microsoft.Exchange.Configuration.Tasks
{
	public enum TaskModuleKey
	{
		Logging,
		LatencyTracker,
		Rbac,
		CmdletIterationEvent,
		RunspaceServerSettingsInit,
		PiiRedaction,
		ReportException,
		SetErrorExecutionContext,
		Throttling,
		TaskFaultInjection,
		Sqm,
		CollectCmdletLogEntries,
		PswsPropertyConverter,
		PswsErrorHandling,
		SetupLogging,
		CmdletHealthCounters,
		AutoReportProgress,
		RunspaceServerSettingsFinalize,
		CmdletProxy
	}
}
