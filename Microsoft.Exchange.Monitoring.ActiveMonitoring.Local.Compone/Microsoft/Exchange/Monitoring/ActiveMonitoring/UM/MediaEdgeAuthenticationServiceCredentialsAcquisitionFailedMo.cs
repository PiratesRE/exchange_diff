using System;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.UM
{
	internal class MediaEdgeAuthenticationServiceCredentialsAcquisitionFailedMonitorAndResponder : IUMLocalMonitoringMonitorAndResponder
	{
		public void InitializeMonitorAndResponder(IMaintenanceWorkBroker broker, TracingContext traceContext)
		{
			UMNotificationEventUtils.InstantiateUMNotificationEventBasedUrgentAlerts(MediaEdgeAuthenticationServiceCredentialsAcquisitionFailedMonitorAndResponder.MediaEdgeAuthenticationServiceCredentialsAcquisitionFailedMonitorName, MediaEdgeAuthenticationServiceCredentialsAcquisitionFailedMonitorAndResponder.MediaEdgeAuthenticationServiceCredentialsAcquisitionFailedResponderName, ExchangeComponent.UMProtocol, 3600, 0, 3, Strings.MediaEdgeAuthenticationServiceCredentialsAcquisitionFailedEscalationMessage, 0, UMNotificationEvent.MediaEdgeAuthenticationServiceCredentialsAcquisition, broker, traceContext, NotificationServiceClass.Scheduled);
		}

		private const int MediaEdgeAuthenticationServiceCredentialsAcquisitionFailedMonitorRecurrenceIntervalInSecs = 0;

		private const int MediaEdgeAuthenticationServiceCredentialsAcquisitionFailedMonitorMonitoringIntervalInSecs = 3600;

		private const int MediaEdgeAuthenticationServiceCredentialsAcquisitionFailedMonitorNumberOfFailures = 3;

		private const int MediaEdgeAuthenticationServiceCredentialsAcquisitionFailedMonitorTransitionToUnhealthySecs = 0;

		private static readonly string MediaEdgeAuthenticationServiceCredentialsAcquisitionFailedMonitorName = "MediaEdgeAuthenticationServiceCredentialsAcquisitionFailedMonitor";

		private static readonly string MediaEdgeAuthenticationServiceCredentialsAcquisitionFailedResponderName = "MediaEdgeAuthenticationServiceCredentialsAcquisitionFailedEscalate";
	}
}
