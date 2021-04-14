using System;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.UM
{
	internal class UMGrammarUsageMonitorAndResponder : IUMLocalMonitoringMonitorAndResponder
	{
		public void InitializeMonitorAndResponder(IMaintenanceWorkBroker broker, TracingContext traceContext)
		{
			UMNotificationEventUtils.InitializeMonitorAndResponderBasedOnOverallPercentSuccessMonitor(UMGrammarUsageMonitorAndResponder.UMGrammarUsageMonitorName, UMGrammarUsageMonitorAndResponder.UMGrammarUsageResponderName, ExchangeComponent.UMProtocol, 86400, 86400, 50, Strings.UMGrammarUsageEscalationMessage(50), 86400, UMMonitoringConstants.UMProtocolHealthSet, UMNotificationEvent.UMGrammarUsage, broker, traceContext, NotificationServiceClass.Scheduled);
		}

		private const int UMGrammarUsageMonitorRecurrenceIntervalInSecs = 0;

		private const int UMGrammarUsageMonitorMonitoringIntervalInSecs = 86400;

		private const int UMGrammarUsageMonitorMonitoringThreshold = 50;

		private const int UMGrammarUsageMonitorTransitionToUnhealthySecs = 86400;

		private static readonly string UMGrammarUsageMonitorName = "UMGrammarUsageMonitor";

		private static readonly string UMGrammarUsageResponderName = "UMGrammarUsageEscalate";
	}
}
