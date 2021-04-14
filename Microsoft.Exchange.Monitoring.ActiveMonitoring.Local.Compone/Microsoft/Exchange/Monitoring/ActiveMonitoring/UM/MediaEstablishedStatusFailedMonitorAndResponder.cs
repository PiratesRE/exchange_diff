using System;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.UM
{
	internal class MediaEstablishedStatusFailedMonitorAndResponder : IUMLocalMonitoringMonitorAndResponder
	{
		public void InitializeMonitorAndResponder(IMaintenanceWorkBroker broker, TracingContext traceContext)
		{
			UMNotificationEventUtils.InstantiateUMNotificationEventBasedUrgentAlerts(MediaEstablishedStatusFailedMonitorAndResponder.MediaEstablishedStatusFailedMonitorName, MediaEstablishedStatusFailedMonitorAndResponder.MediaEstablishedStatusFailedResponderName, ExchangeComponent.UMProtocol, 3600, 0, 3, Strings.MediaEstablishedFailedEscalationMessage, 0, UMNotificationEvent.MediaEstablishedStatus, broker, traceContext, NotificationServiceClass.Scheduled);
		}

		private const int MediaEstablishedStatusFailedMonitorRecurrenceIntervalInSecs = 0;

		private const int MediaEstablishedStatusFailedMonitorMonitoringIntervalInSecs = 3600;

		private const int MediaEstablishedStatusFailedMonitorNumberOfFailures = 3;

		private const int MediaEstablishedStatusFailedMonitorTransitionToUnhealthySecs = 0;

		private static readonly string MediaEstablishedStatusFailedMonitorName = "MediaEstablishedFailedMonitor";

		private static readonly string MediaEstablishedStatusFailedResponderName = "MediaEstablishedFailedEscalate";
	}
}
