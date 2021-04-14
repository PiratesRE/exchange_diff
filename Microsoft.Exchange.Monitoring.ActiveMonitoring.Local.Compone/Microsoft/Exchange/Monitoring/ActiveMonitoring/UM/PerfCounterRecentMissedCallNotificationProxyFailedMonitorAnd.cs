using System;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.UM
{
	internal class PerfCounterRecentMissedCallNotificationProxyFailedMonitorAndResponder : IUMLocalMonitoringMonitorAndResponder
	{
		public void InitializeMonitorAndResponder(IMaintenanceWorkBroker broker, TracingContext traceContext)
		{
			UMPerfCounterUtils.InstantiatePerfCountersBasedUrgentAlerts(50, 2, "MSExchangeUMCallRouterAvailability\\% of Missed Call Notification Proxy Failed at UM Call Router Over the Last Hour", PerfCounterRecentMissedCallNotificationProxyFailedMonitorAndResponder.RecentUMCallRouterMissedCallNotificationProxyFailedMonitorName, PerfCounterRecentMissedCallNotificationProxyFailedMonitorAndResponder.RecentUMCallRouterMissedCallNotificationProxyFailedResponderName, ExchangeComponent.UMCallRouter, PerfCounterRecentMissedCallNotificationProxyFailedMonitorAndResponder.RecentUMCallRouterMissedCallNotificationProxyFailedEscalationMessageString, 0, broker, traceContext, NotificationServiceClass.Scheduled);
		}

		private const int RecentUMCallRouterMissedCallNotificationProxyFailedPercentThreshold = 50;

		private const int RecentUMCallRouterMissedCallNotificationProxyFailedNumberOfSamples = 2;

		private const string RecentUMCallRouterMissedCallNotificationProxyFailedPerfCounterName = "MSExchangeUMCallRouterAvailability\\% of Missed Call Notification Proxy Failed at UM Call Router Over the Last Hour";

		private const int RecentUMCallRouterMissedCallNotificationProxyFailedTransitionToUnhealthySecs = 0;

		private static readonly string RecentUMCallRouterMissedCallNotificationProxyFailedMonitorName = "UMCallRouterRecentMissedCallNotificationProxyFailedMonitor";

		private static readonly string RecentUMCallRouterMissedCallNotificationProxyFailedResponderName = "UMCallRouterRecentMissedCallNotificationProxyFailedEscalate";

		private static readonly string RecentUMCallRouterMissedCallNotificationProxyFailedEscalationMessageString = Strings.UMCallRouterRecentMissedCallNotificationProxyFailedEscalationMessageString(50);
	}
}
