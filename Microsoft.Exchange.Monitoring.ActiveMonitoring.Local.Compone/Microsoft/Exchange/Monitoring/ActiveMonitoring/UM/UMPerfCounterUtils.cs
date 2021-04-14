using System;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.UM
{
	internal class UMPerfCounterUtils
	{
		public static void InstantiatePerfCountersBasedUrgentAlerts(int percentThreshold, int numberOfSamples, string perfCounterName, string monitorName, string responderName, Component exchangeComponent, string escalationMessageString, int transitionToUnhealthySecs, IMaintenanceWorkBroker broker, TracingContext traceContext, NotificationServiceClass notificationServiceClass = NotificationServiceClass.Scheduled)
		{
			MonitorDefinition monitorDefinition = OverallConsecutiveSampleValueAboveThresholdMonitor.CreateDefinition(monitorName, PerformanceCounterNotificationItem.GenerateResultName(perfCounterName), exchangeComponent.Name, exchangeComponent, (double)percentThreshold, numberOfSamples, true);
			monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Unhealthy, transitionToUnhealthySecs)
			};
			monitorDefinition.RecurrenceIntervalSeconds = 0;
			monitorDefinition.ServicePriority = 1;
			monitorDefinition.ScenarioDescription = "Validate UM health is not impacted by any issues";
			broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, traceContext);
			ResponderDefinition definition = EscalateResponder.CreateDefinition(responderName, exchangeComponent.Name, monitorDefinition.Name, monitorDefinition.Name, string.Empty, ServiceHealthStatus.None, UMMonitoringConstants.UmEscalationTeam, Strings.EscalationSubjectUnhealthy, escalationMessageString, true, notificationServiceClass, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
			broker.AddWorkDefinition<ResponderDefinition>(definition, traceContext);
		}
	}
}
