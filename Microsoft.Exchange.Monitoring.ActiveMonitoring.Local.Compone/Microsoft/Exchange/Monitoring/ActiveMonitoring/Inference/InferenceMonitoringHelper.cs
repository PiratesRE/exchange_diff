using System;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Inference
{
	internal static class InferenceMonitoringHelper
	{
		static InferenceMonitoringHelper()
		{
			MonitoringLogConfiguration configuration = new MonitoringLogConfiguration(ExchangeComponent.Inference.Name);
			InferenceMonitoringHelper.monitoringLogger = new MonitoringLogger(configuration);
		}

		internal static void LogInfo(string message, params object[] messageArgs)
		{
			InferenceMonitoringHelper.monitoringLogger.LogEvent(DateTime.UtcNow, message, messageArgs);
		}

		internal static void LogInfo(WorkItem workItem, string message, params object[] messageArgs)
		{
			InferenceMonitoringHelper.monitoringLogger.LogEvent(DateTime.UtcNow, string.Format("{0}/{1}: ", workItem.Definition.Name, workItem.Definition.TargetResource) + message, messageArgs);
		}

		internal const string MailboxAssistantsServiceName = "msexchangemailboxassistants";

		internal const string DeliveryServiceName = "msexchangedelivery";

		internal const string MailboxAssistantsRegKeyName = "SYSTEM\\CurrentControlSet\\Services\\MSExchangeMailboxAssistants\\Parameters";

		internal const string DeliveryAgentRegKeyName = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Inference";

		internal const string TrainingRegKeyValueName = "InferenceTrainingAssistantEnabledOverride";

		internal const string DataCollectionRegKeyValueName = "InferenceDataCollectionAssistantEnabledOverride";

		internal const string ClassificationRegKeyValueName = "ClassificationPipelineEnabled";

		internal const string DeliveryAppConfigFileName = "MSExchangeDelivery.exe.config";

		internal const string ClassificationAppConfigOverrideName = "InferenceClassificationAgentEnabledOverride";

		private static MonitoringLogger monitoringLogger;
	}
}
