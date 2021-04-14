using System;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.UM
{
	internal class UMNotificationEventUtils
	{
		public static void InstantiateUMNotificationEventBasedUrgentAlerts(string monitorName, string responderName, Component exchangeComponent, int monitorMonitoringIntervalInSecs, int monitorRecurrenceIntervalInSecs, int monitorNumberOfFailures, string escalationMessageString, int transitionToUnhealthySecs, UMNotificationEvent umNotificationEvent, IMaintenanceWorkBroker broker, TracingContext traceContext, NotificationServiceClass notificationServiceClass = NotificationServiceClass.Scheduled)
		{
			MonitorDefinition monitorDefinition = OverallXFailuresMonitor.CreateDefinition(monitorName, NotificationItem.GenerateResultName(exchangeComponent.Name, umNotificationEvent.ToString(), null), exchangeComponent.Name, exchangeComponent, monitorMonitoringIntervalInSecs, monitorRecurrenceIntervalInSecs, monitorNumberOfFailures, true);
			monitorDefinition.TargetResource = string.Empty;
			monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Unhealthy, transitionToUnhealthySecs)
			};
			monitorDefinition.ServicePriority = 1;
			monitorDefinition.ScenarioDescription = "Validate UM health is not impacted by any issues";
			broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, traceContext);
			ResponderDefinition definition = EscalateResponder.CreateDefinition(responderName, exchangeComponent.Name, monitorName, monitorName, string.Empty, ServiceHealthStatus.Unhealthy, UMMonitoringConstants.UmEscalationTeam, Strings.EscalationSubjectUnhealthy, escalationMessageString, true, notificationServiceClass, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
			broker.AddWorkDefinition<ResponderDefinition>(definition, traceContext);
		}

		public static void InitializeMonitorAndResponderBasedOnOverallPercentSuccessMonitor(string monitorName, string responderName, Component exchangeComponent, int monitorMonitoringIntervalInSecs, int monitorRecurrenceIntervalInSecs, int monitorThreshold, string escalationMessageString, int transitionToUnhealthySecs, string healthSetName, UMNotificationEvent umNotificationEvent, IMaintenanceWorkBroker broker, TracingContext traceContext, NotificationServiceClass notificationServiceClass = NotificationServiceClass.Scheduled)
		{
			MonitorDefinition monitorDefinition = OverallPercentSuccessMonitor.CreateDefinition(monitorName, NotificationItem.GenerateResultName(exchangeComponent.Name, umNotificationEvent.ToString(), null), healthSetName, ExchangeComponent.UMProtocol, (double)monitorThreshold, TimeSpan.FromSeconds((double)monitorMonitoringIntervalInSecs), true);
			monitorDefinition.RecurrenceIntervalSeconds = monitorRecurrenceIntervalInSecs;
			monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Unhealthy, transitionToUnhealthySecs)
			};
			monitorDefinition.ServicePriority = 1;
			monitorDefinition.ScenarioDescription = "Validate UM health is not impacted by any issues";
			broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, traceContext);
			ResponderDefinition definition = EscalateResponder.CreateDefinition(responderName, healthSetName, monitorName, monitorName, string.Empty, ServiceHealthStatus.Unhealthy, UMMonitoringConstants.UmEscalationTeam, Strings.EscalationSubjectUnhealthy, escalationMessageString, true, NotificationServiceClass.Scheduled, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
			broker.AddWorkDefinition<ResponderDefinition>(definition, traceContext);
		}
	}
}
