using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.TimeBasedAssistants
{
	public static class TimeBasedAssistantsOutOfSlaMonitor
	{
		public static void ConfigureForTest()
		{
			TimeBasedAssistantsOutOfSlaMonitor.AssistantsOutOfSlaProbeMaxAttempts = 1;
			TimeBasedAssistantsOutOfSlaMonitor.AssistantsOutOfSlaMonitorThreshold = 2;
			TimeBasedAssistantsOutOfSlaMonitor.AssistantsOutOfSlaProbeRecurrence = (int)TimeSpan.FromSeconds(30.0).TotalSeconds;
			TimeBasedAssistantsOutOfSlaMonitor.AssistantsOutOfSlaMonitoringInterval = (int)TimeSpan.FromSeconds(60.0).TotalSeconds;
			TimeBasedAssistantsOutOfSlaMonitor.AssistantsOutOfSlaEscalateResponderWaitInterval = (int)TimeSpan.FromSeconds(10.0).TotalSeconds;
		}

		public static ProbeDefinition CreateOutOfSlaProbeDefinition(TracingContext tracingContext, TbaOutOfSlaAlertType alertType)
		{
			ArgumentValidator.ThrowIfNull("tracingContext", tracingContext);
			Type typeFromHandle;
			string probeName;
			if (alertType == TbaOutOfSlaAlertType.Urgent)
			{
				typeFromHandle = typeof(TimeBasedAssistantsOutOfSlaProbeUrgent);
				probeName = TimeBasedAssistantsOutOfSlaMonitor.AssistantsOutOfSlaUrgentProbeName;
			}
			else
			{
				typeFromHandle = typeof(TimeBasedAssistantsOutOfSlaProbeScheduled);
				probeName = TimeBasedAssistantsOutOfSlaMonitor.AssistantsOutOfSlaScheduledProbeName;
			}
			return TimeBasedAssistantsDiscoveryHelpers.CreateProbe(ExchangeComponent.TimeBasedAssistants, "MSExchangeMailboxAssistants", LocalServer.GetServer().Fqdn, typeFromHandle, probeName, TimeBasedAssistantsOutOfSlaMonitor.AssistantsOutOfSlaProbeRecurrence, TimeBasedAssistantsOutOfSlaMonitor.AssistantsOutOfSlaProbeMaxAttempts, tracingContext);
		}

		public static MonitorDefinition CreateOutOfSlaMonitorDefinition(string mask, TracingContext tracingContext, TbaOutOfSlaAlertType alertType)
		{
			ArgumentValidator.ThrowIfNullOrWhiteSpace("mask", mask);
			ArgumentValidator.ThrowIfNull("tracingContext", tracingContext);
			string name;
			if (alertType == TbaOutOfSlaAlertType.Urgent)
			{
				name = TimeBasedAssistantsOutOfSlaMonitor.AssistantsOutOfSlaUrgentMonitorName;
			}
			else
			{
				name = TimeBasedAssistantsOutOfSlaMonitor.AssistantsOutOfSlaScheduledMonitorName;
			}
			MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition(name, mask, ExchangeComponent.TimeBasedAssistants.Name, ExchangeComponent.TimeBasedAssistants, TimeBasedAssistantsOutOfSlaMonitor.AssistantsOutOfSlaMonitorThreshold, true, TimeBasedAssistantsOutOfSlaMonitor.AssistantsOutOfSlaMonitoringInterval);
			monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Unhealthy, 0),
				new MonitorStateTransition(ServiceHealthStatus.Unrecoverable, monitorDefinition.RecurrenceIntervalSeconds / 2)
			};
			monitorDefinition.ServicePriority = 1;
			monitorDefinition.ScenarioDescription = "Validate Time Based Assistatns are meeting their goal.";
			return monitorDefinition;
		}

		public static ResponderDefinition CreateOutOfSlaEscalateResponderDefinition(string monitorName, string alertMask, TbaOutOfSlaAlertType alertType)
		{
			ArgumentValidator.ThrowIfNullOrWhiteSpace("monitorName", monitorName);
			ArgumentValidator.ThrowIfNullOrWhiteSpace("alertMask", alertMask);
			string name;
			if (alertType == TbaOutOfSlaAlertType.Urgent)
			{
				name = TimeBasedAssistantsOutOfSlaMonitor.AssistantsOutOfSlaUrgentEscalateResponderName;
			}
			else
			{
				name = TimeBasedAssistantsOutOfSlaMonitor.AssistantsOutOfSlaScheduledEscalateResponderName;
			}
			ResponderDefinition responderDefinition = EscalateResponder.CreateDefinition(name, ExchangeComponent.TimeBasedAssistants.Name, monitorName, alertMask, "MSExchangeMailboxAssistants", ServiceHealthStatus.Unrecoverable, ExchangeComponent.TimeBasedAssistants.EscalationTeam, Strings.AssistantsOutOfSlaSubject, Strings.AssistantsOutOfSlaMessage, true, NotificationServiceClass.UrgentInTraining, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
			responderDefinition.WaitIntervalSeconds = TimeBasedAssistantsOutOfSlaMonitor.AssistantsOutOfSlaEscalateResponderWaitInterval;
			return responderDefinition;
		}

		private const string AssistantsOutOfSlaUrgentWorkItemName = "TbaOutOfSlaUrgent";

		private const string AssistantsOutOfSlaScheduledWorkItemName = "TbaOutOfSlaScheduled";

		private static readonly string AssistantsOutOfSlaUrgentProbeName = TimeBasedAssistantsDiscoveryHelpers.GenerateProbeName("TbaOutOfSlaUrgent");

		private static readonly string AssistantsOutOfSlaUrgentMonitorName = TimeBasedAssistantsDiscoveryHelpers.GenerateMonitorName("TbaOutOfSlaUrgent");

		private static readonly string AssistantsOutOfSlaUrgentEscalateResponderName = TimeBasedAssistantsDiscoveryHelpers.GenerateResponderName("TbaOutOfSlaUrgent", "Escalate");

		private static readonly string AssistantsOutOfSlaScheduledProbeName = TimeBasedAssistantsDiscoveryHelpers.GenerateProbeName("TbaOutOfSlaScheduled");

		private static readonly string AssistantsOutOfSlaScheduledMonitorName = TimeBasedAssistantsDiscoveryHelpers.GenerateMonitorName("TbaOutOfSlaScheduled");

		private static readonly string AssistantsOutOfSlaScheduledEscalateResponderName = TimeBasedAssistantsDiscoveryHelpers.GenerateResponderName("TbaOutOfSlaScheduled", "Escalate");

		private static int AssistantsOutOfSlaProbeMaxAttempts = 3;

		private static int AssistantsOutOfSlaMonitorThreshold = 4;

		private static int AssistantsOutOfSlaProbeRecurrence = (int)TimeSpan.FromMinutes(20.0).TotalSeconds;

		private static int AssistantsOutOfSlaMonitoringInterval = (int)TimeSpan.FromHours(2.0).TotalSeconds;

		private static int AssistantsOutOfSlaEscalateResponderWaitInterval = (int)TimeSpan.FromHours(8.0).TotalSeconds;
	}
}
