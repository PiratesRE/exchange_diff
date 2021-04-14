using System;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.UM
{
	internal class PerfCounterUMPipelineSLAMonitorAndResponder : IUMLocalMonitoringMonitorAndResponder
	{
		public void InitializeMonitorAndResponder(IMaintenanceWorkBroker broker, TracingContext traceContext)
		{
			MonitorDefinition monitorDefinition = OverallConsecutiveSampleValueBelowThresholdMonitor.CreateDefinition(PerfCounterUMPipelineSLAMonitorAndResponder.UMPipelineSLAMonitorName, PerformanceCounterNotificationItem.GenerateResultName("MSExchangeUMAvailability\\% of Messages Successfully Processed Over the Last Hour"), UMMonitoringConstants.UMProtocolHealthSet, ExchangeComponent.UMProtocol, 50.0, 2, true);
			monitorDefinition.RecurrenceIntervalSeconds = 0;
			monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Unhealthy, 0)
			};
			monitorDefinition.ServicePriority = 1;
			monitorDefinition.ScenarioDescription = "Validate UM health is not impacted by SLA impacting issues";
			broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, traceContext);
			ResponderDefinition definition = EscalateResponder.CreateDefinition(PerfCounterUMPipelineSLAMonitorAndResponder.UMPipelineSLAResponderName, UMMonitoringConstants.UMProtocolHealthSet, PerfCounterUMPipelineSLAMonitorAndResponder.UMPipelineSLAMonitorName, PerfCounterUMPipelineSLAMonitorAndResponder.UMPipelineSLAMonitorName, string.Empty, ServiceHealthStatus.None, UMMonitoringConstants.UmEscalationTeam, Strings.EscalationSubjectUnhealthy, PerfCounterUMPipelineSLAMonitorAndResponder.UMPipelineSLAEscalationMessageString, true, NotificationServiceClass.Scheduled, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
			broker.AddWorkDefinition<ResponderDefinition>(definition, traceContext);
		}

		private const int UMPipelineSLAPercentThreshold = 50;

		private const int UMPipelineSLANumberOfSamples = 2;

		private const string UMPipelineSLAPerfCounterName = "MSExchangeUMAvailability\\% of Messages Successfully Processed Over the Last Hour";

		private const int UMPipelineSLATransitionToUnhealthySecs = 0;

		private static readonly string UMPipelineSLAMonitorName = "UMPipelineSLAMonitor";

		private static readonly string UMPipelineSLAResponderName = "UMPipelineSLAEscalate";

		private static readonly string UMPipelineSLAEscalationMessageString = Strings.UMPipelineSLAEscalationMessageString(50);
	}
}
