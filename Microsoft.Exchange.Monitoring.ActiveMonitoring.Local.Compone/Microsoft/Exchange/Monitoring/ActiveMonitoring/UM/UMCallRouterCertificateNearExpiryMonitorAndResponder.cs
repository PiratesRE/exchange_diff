using System;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.UM
{
	internal class UMCallRouterCertificateNearExpiryMonitorAndResponder : IUMLocalMonitoringMonitorAndResponder
	{
		public void InitializeMonitorAndResponder(IMaintenanceWorkBroker broker, TracingContext traceContext)
		{
			UMNotificationEventUtils.InstantiateUMNotificationEventBasedUrgentAlerts(UMCallRouterCertificateNearExpiryMonitorAndResponder.UMCallRouterCertificateNearExpiryMonitorName, UMCallRouterCertificateNearExpiryMonitorAndResponder.UMCallRouterCertificateNearExpiryResponderName, ExchangeComponent.UMCallRouter, 3600, 0, 1, Strings.UMCallRouterCertificateNearExpiryEscalationMessage, 0, UMNotificationEvent.CallRouterCertificateNearExpiry, broker, traceContext, NotificationServiceClass.Scheduled);
		}

		private const int UMCallRouterCertificateNearExpiryMonitorRecurrenceIntervalInSecs = 0;

		private const int UMCallRouterCertificateNearExpiryMonitorMonitoringIntervalInSecs = 3600;

		private const int UMCallRouterCertificateNearExpiryNumberOfFailures = 1;

		private const int UMCallRouterCertificateNearExpiryMonitorTransitionToUnhealthySecs = 0;

		private static readonly string UMCallRouterCertificateNearExpiryMonitorName = "UMCallRouterCertificateNearExpiryMonitor";

		private static readonly string UMCallRouterCertificateNearExpiryResponderName = "UMCallRouterCertificateNearExpiryEscalate";
	}
}
