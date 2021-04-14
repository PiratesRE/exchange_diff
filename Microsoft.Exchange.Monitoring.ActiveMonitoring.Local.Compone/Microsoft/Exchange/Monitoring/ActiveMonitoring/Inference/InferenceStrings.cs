using System;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Inference
{
	public static class InferenceStrings
	{
		internal static string DisableInferenceComponentResponderName(string monitorName)
		{
			return monitorName.Replace("Monitor", "DisableInferenceComponent");
		}

		internal static string InferenceEscalateResponderName(string monitorName)
		{
			return monitorName.Replace("Monitor", "Escalate");
		}

		internal const string Monitor = "Monitor";

		internal const string InferenceClassificationSLAMonitorName = "InferenceClassificationSLAMonitor";

		internal const string InferenceTrainingFailurePercentageMonitorName = "InferenceTrainingFailurePercentageMonitor";

		internal const string InferenceMailboxAssistantsCrashProbeName = "InferenceMailboxAssistantsCrashProbe";

		internal const string InferenceMailboxAssistantsCrashMonitorName = "InferenceMailboxAssistantsCrashMonitor";

		internal const string InferenceDeliveryCrashProbeName = "InferenceDeliveryCrashProbe";

		internal const string InferenceDeliveryCrashMonitorName = "InferenceDeliveryCrashMonitor";

		internal const string InferenceComponentDisabledProbeName = "InferenceComponentDisabledProbe";

		internal const string InferenceComponentDisabledMonitorName = "InferenceComponentDisabledMonitor";

		internal const string InferenceActivityLogSyntheticProbeName = "InferenceActivityLogSyntheticProbe";
	}
}
