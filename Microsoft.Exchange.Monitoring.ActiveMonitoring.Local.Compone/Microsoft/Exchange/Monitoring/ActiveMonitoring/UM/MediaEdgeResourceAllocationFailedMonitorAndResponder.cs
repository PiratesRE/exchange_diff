using System;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.UM
{
	internal class MediaEdgeResourceAllocationFailedMonitorAndResponder : IUMLocalMonitoringMonitorAndResponder
	{
		public void InitializeMonitorAndResponder(IMaintenanceWorkBroker broker, TracingContext traceContext)
		{
			UMNotificationEventUtils.InstantiateUMNotificationEventBasedUrgentAlerts(MediaEdgeResourceAllocationFailedMonitorAndResponder.MediaEdgeResourceAllocationFailedMonitorName, MediaEdgeResourceAllocationFailedMonitorAndResponder.MediaEdgeResourceAllocationFailedResponderName, ExchangeComponent.UMProtocol, 3600, 0, 3, Strings.MediaEdgeResourceAllocationFailedEscalationMessage, 0, UMNotificationEvent.MediaEdgeResourceAllocation, broker, traceContext, NotificationServiceClass.Scheduled);
		}

		private const int MediaEdgeResourceAllocationFailedMonitorRecurrenceIntervalInSecs = 0;

		private const int MediaEdgeResourceAllocationFailedMonitorMonitoringIntervalInSecs = 3600;

		private const int MediaEdgeResourceAllocationFailedMonitorNumberOfFailures = 3;

		private const int MediaEdgeResourceAllocationFailedMonitorTransitionToUnhealthySecs = 0;

		private static readonly string MediaEdgeResourceAllocationFailedMonitorName = "MediaEdgeResourceAllocationFailedMonitor";

		private static readonly string MediaEdgeResourceAllocationFailedResponderName = "MediaEdgeResourceAllocationFailedEscalate";
	}
}
