using System;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.UM
{
	internal class UMCertificateNearExpiryMonitorAndResponder : IUMLocalMonitoringMonitorAndResponder
	{
		public void InitializeMonitorAndResponder(IMaintenanceWorkBroker broker, TracingContext traceContext)
		{
			UMNotificationEventUtils.InstantiateUMNotificationEventBasedUrgentAlerts(UMCertificateNearExpiryMonitorAndResponder.UMCertificateNearExpiryMonitorName, UMCertificateNearExpiryMonitorAndResponder.UMCertificateNearExpiryResponderName, ExchangeComponent.UMProtocol, 3600, 0, 1, Strings.UMCertificateNearExpiryEscalationMessage, 0, UMNotificationEvent.CertificateNearExpiry, broker, traceContext, NotificationServiceClass.Scheduled);
		}

		private const int UMCertificateNearExpiryMonitorRecurrenceIntervalInSecs = 0;

		private const int UMCertificateNearExpiryMonitorMonitoringIntervalInSecs = 3600;

		private const int UMCertificateNearExpiryNumberOfFailures = 1;

		private const int UMCertificateNearExpiryMonitorTransitionToUnhealthySecs = 0;

		private static readonly string UMCertificateNearExpiryMonitorName = "UMCertificateNearExpiryMonitor";

		private static readonly string UMCertificateNearExpiryResponderName = "UMCertificateNearExpiryEscalate";
	}
}
