using System;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.UM
{
	internal class UMTranscriptionThrottledMonitorAndResponder : IUMLocalMonitoringMonitorAndResponder
	{
		public void InitializeMonitorAndResponder(IMaintenanceWorkBroker broker, TracingContext traceContext)
		{
			UMNotificationEventUtils.InitializeMonitorAndResponderBasedOnOverallPercentSuccessMonitor(UMTranscriptionThrottledMonitorAndResponder.UMTranscriptionThrottledMonitorName, UMTranscriptionThrottledMonitorAndResponder.UMTranscriptionThrottledResponderName, ExchangeComponent.UMProtocol, 86400, 0, 50, Strings.UMTranscriptionThrottledEscalationMessage(50), 86400, UMMonitoringConstants.UMProtocolHealthSet, UMNotificationEvent.UMTranscriptionThrottling, broker, traceContext, NotificationServiceClass.Scheduled);
		}

		private const int UMTranscriptionThrottledMonitorRecurrenceIntervalInSecs = 0;

		private const int UMTranscriptionThrottledMonitorMonitoringIntervalInSecs = 86400;

		private const int UMTranscriptionThrottledMonitorMonitoringThreshold = 50;

		private const int UMTranscriptionThrottledMonitorTransitionToUnhealthySecs = 86400;

		private static readonly string UMTranscriptionThrottledMonitorName = "UMTranscriptionThrottledMonitor";

		private static readonly string UMTranscriptionThrottledResponderName = "UMTranscriptionThrottledEscalate";
	}
}
