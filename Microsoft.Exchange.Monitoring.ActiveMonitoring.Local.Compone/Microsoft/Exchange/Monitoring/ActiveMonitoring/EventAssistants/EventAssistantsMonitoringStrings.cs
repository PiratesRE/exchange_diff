using System;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.EventAssistants
{
	public static class EventAssistantsMonitoringStrings
	{
		public const string EventAssistantsServiceProbeName = "EventAssistantsServiceProbe";

		public const string EventAssistantsServiceMonitorName = "EventAssistantsServiceMonitor";

		public const string EventAssistantsServiceRestartResponderName = "EventAssistantsServiceRestart";

		public const string EventAssistantsServiceEscalateResponderName = "EventAssistantsServiceEscalate";

		public const string EventAssistantsProcessRepeatedlyCrashingProbeName = "EventAssistantsProcessRepeatedlyCrashingProbe";

		public const string EventAssistantsProcessRepeatedlyCrashingMonitorName = "EventAssistantsProcessRepeatedlyCrashingMonitor";

		public const string EventAssistantsProcessRepeatedlyCrashingEscalateResponderName = "EventAssistantsProcessRepeatedlyCrashingEscalate";

		public const string MailboxAssistantsWatermarksProbeName = "MailboxAssistantsWatermarksProbe";

		public const string MailboxAssistantsWatermarksMonitorName = "MailboxAssistantsWatermarksMonitor";

		public const string MailboxAssistantsWatermarksEscalationProcessingMonitorName = "MailboxAssistantsWatermarksEscalationProcessingMonitor";

		public const string MailboxAssistantsWatermarksWatsonResponderName = "MailboxAssistantsWatermarksWatsonResponder";

		public const string MailboxAssistantsWatermarksRestartResponderName = "MailboxAssistantsWatermarksRestart";

		public const string MailboxAssistantsWatermarksEscalationNotificationResponderName = "MailboxAssistantsWatermarksEscalationNotification";

		public const string MailboxAssistantsWatermarksEscalateResponderName = "MailboxAssistantsWatermarksEscalate";

		public const string MailSubmissionWatermarksProbeName = "MailSubmissionWatermarksProbe";

		public const string MailSubmissionWatermarksMonitorName = "MailSubmissionWatermarksMonitor";

		public const string MailSubmissionWatermarksRestartResponderName = "MailSubmissionWatermarksRestart";

		public const string MailSubmissionWatermarksEscalateResponderName = "MailSubmissionWatermarksEscalate";

		public const string InvokeMonitoringProbeCommand = "Invoke-MonitoringProbe -Identity '{0}\\{1}\\{{Probe.StateAttribute1}}' -Server {2}";

		public const string GetAllUnhealthyMonitors = "Get-ServerHealth -Identity '{0}' -HealthSet '{1}' | ?{{$_.Name -match '{2}' -and $_.AlertValue -ne 'Healthy'}}";
	}
}
