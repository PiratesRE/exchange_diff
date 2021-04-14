using System;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.UM
{
	internal class UMPipelineFullMonitorAndResponder : IUMLocalMonitoringMonitorAndResponder
	{
		public void InitializeMonitorAndResponder(IMaintenanceWorkBroker broker, TracingContext traceContext)
		{
			MonitorDefinition monitorDefinition = OverallXFailuresMonitor.CreateDefinition(UMPipelineFullMonitorAndResponder.UMPipelineFullMonitorName, NotificationItem.GenerateResultName(ExchangeComponent.UMProtocol.Name, UMNotificationEvent.UMPipelineFull.ToString(), null), UMMonitoringConstants.UMProtocolHealthSet, ExchangeComponent.UMProtocol, 60, 60, 1, true);
			monitorDefinition.TargetResource = string.Empty;
			monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Unhealthy, 3600)
			};
			broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, traceContext);
			ResponderDefinition definition = EscalateResponder.CreateDefinition("UMPipelineFullEscalate", UMMonitoringConstants.UMProtocolHealthSet, UMPipelineFullMonitorAndResponder.UMPipelineFullMonitorName, UMPipelineFullMonitorAndResponder.UMPipelineFullMonitorName, string.Empty, ServiceHealthStatus.Unhealthy, UMMonitoringConstants.UmEscalationTeam, Strings.EscalationSubjectUnhealthy, Strings.UMPipelineFullEscalationMessageString, true, NotificationServiceClass.Urgent, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
			broker.AddWorkDefinition<ResponderDefinition>(definition, traceContext);
		}

		private const int UMPipelineFullMonitorMonitoringIntervalSecs = 60;

		private const int UMPipelineFullMonitorRecurrenceIntervalSecs = 60;

		private const int UMPipelineFullMonitorNumberOfFailures = 1;

		private const int UMPipelineFullMonitorTransitionToDegradedSecs = 0;

		private const int UMPipelineFullMonitorTransitionToUnhealthySecs = 3600;

		private static readonly string UMPipelineFullMonitorName = "UMPipelineFullMonitor";
	}
}
