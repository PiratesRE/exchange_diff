using System;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.ProcessIsolation.Monitors
{
	public enum ProcessTrigger
	{
		PrivateWorkingSetTrigger_Warning,
		PrivateWorkingSetTrigger_Error,
		ProcessProcessorTimeTrigger_Warning,
		ProcessProcessorTimeTrigger_Error,
		ExchangeCrashEventTrigger_Error,
		LongRunningWatsonTrigger_Warning,
		LongRunningWerMgrTrigger_Warning
	}
}
