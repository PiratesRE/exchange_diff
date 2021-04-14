using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.TimeBasedAssistants
{
	public static class TimeBasedAssistantsActiveDatabaseDiscovery
	{
		public static void ConfigureForTest()
		{
			TimeBasedAssistantsActiveDatabaseDiscovery.AssistantsActiveDatabaseProbeMaxAttempts = 1;
			TimeBasedAssistantsActiveDatabaseDiscovery.AssistantsActiveDatabaseMonitorThreshold = 2;
			TimeBasedAssistantsActiveDatabaseDiscovery.AssistantsActiveDatabaseProbeRecurrenceSeconds = (int)TimeSpan.FromSeconds(30.0).TotalSeconds;
			TimeBasedAssistantsActiveDatabaseDiscovery.AssistantsActiveDatabaseMonitoringIntervalSeconds = (int)TimeSpan.FromSeconds(60.0).TotalSeconds;
			TimeBasedAssistantsActiveDatabaseDiscovery.AssistantsActiveDatabaseEscalateResponderWaitIntervalSeconds = (int)TimeSpan.FromSeconds(10.0).TotalSeconds;
		}

		public static ProbeDefinition CreateActiveDatabaseProbeDefinition(TracingContext tracingContext)
		{
			ArgumentValidator.ThrowIfNull("tracingContext", tracingContext);
			return TimeBasedAssistantsDiscoveryHelpers.CreateProbe(ExchangeComponent.TimeBasedAssistants, "MSExchangeMailboxAssistants", LocalServer.GetServer().Fqdn, typeof(TimeBasedAssistantsActiveDatabaseProbe), TimeBasedAssistantsActiveDatabaseDiscovery.AssistantsActiveDatabaseProbeName, TimeBasedAssistantsActiveDatabaseDiscovery.AssistantsActiveDatabaseProbeRecurrenceSeconds, TimeBasedAssistantsActiveDatabaseDiscovery.AssistantsActiveDatabaseProbeMaxAttempts, tracingContext);
		}

		public static MonitorDefinition CreateActiveDatabaseMonitorDefinition(string mask, TracingContext tracingContext)
		{
			ArgumentValidator.ThrowIfNullOrWhiteSpace("mask", mask);
			ArgumentValidator.ThrowIfNull("tracingContext", tracingContext);
			MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition(TimeBasedAssistantsActiveDatabaseDiscovery.AssistantsActiveDatabaseMonitorName, mask, ExchangeComponent.TimeBasedAssistants.Name, ExchangeComponent.TimeBasedAssistants, TimeBasedAssistantsActiveDatabaseDiscovery.AssistantsActiveDatabaseMonitorThreshold, true, TimeBasedAssistantsActiveDatabaseDiscovery.AssistantsActiveDatabaseMonitoringIntervalSeconds);
			monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Unhealthy, 0),
				new MonitorStateTransition(ServiceHealthStatus.Unrecoverable, monitorDefinition.RecurrenceIntervalSeconds / 2)
			};
			monitorDefinition.ServicePriority = 1;
			monitorDefinition.ScenarioDescription = "Validate Time Based Assistatns are running on active databases.";
			return monitorDefinition;
		}

		public static ResponderDefinition CreateActiveDatabaseEscalateResponderDefinition(string monitorName, string alertMask)
		{
			ArgumentValidator.ThrowIfNullOrWhiteSpace("monitorName", monitorName);
			ArgumentValidator.ThrowIfNullOrWhiteSpace("alertMask", alertMask);
			ResponderDefinition responderDefinition = EscalateResponder.CreateDefinition(TimeBasedAssistantsActiveDatabaseDiscovery.AssistantsActiveDatabaseEscalateResponderName, ExchangeComponent.TimeBasedAssistants.Name, monitorName, alertMask, "MSExchangeMailboxAssistants", ServiceHealthStatus.Unrecoverable, ExchangeComponent.TimeBasedAssistants.EscalationTeam, Strings.AssistantsActiveDatabaseSubject, Strings.AssistantsActiveDatabaseMessage, true, NotificationServiceClass.UrgentInTraining, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
			responderDefinition.WaitIntervalSeconds = TimeBasedAssistantsActiveDatabaseDiscovery.AssistantsActiveDatabaseEscalateResponderWaitIntervalSeconds;
			return responderDefinition;
		}

		private const string AssistantsActiveDatabaseWorkItemName = "TbaActiveDatabase";

		private static readonly string AssistantsActiveDatabaseProbeName = TimeBasedAssistantsDiscoveryHelpers.GenerateProbeName("TbaActiveDatabase");

		private static readonly string AssistantsActiveDatabaseMonitorName = TimeBasedAssistantsDiscoveryHelpers.GenerateMonitorName("TbaActiveDatabase");

		private static readonly string AssistantsActiveDatabaseEscalateResponderName = TimeBasedAssistantsDiscoveryHelpers.GenerateResponderName("TbaActiveDatabase", "Escalate");

		private static int AssistantsActiveDatabaseProbeMaxAttempts = 3;

		private static int AssistantsActiveDatabaseMonitorThreshold = 4;

		private static int AssistantsActiveDatabaseProbeRecurrenceSeconds = (int)TimeSpan.FromMinutes(80.0).TotalSeconds;

		private static int AssistantsActiveDatabaseMonitoringIntervalSeconds = (int)TimeSpan.FromHours(8.0).TotalSeconds;

		private static int AssistantsActiveDatabaseEscalateResponderWaitIntervalSeconds = (int)TimeSpan.FromHours(8.0).TotalSeconds;
	}
}
