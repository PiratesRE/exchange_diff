using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.TimeBasedAssistants
{
	public static class InfrastructureValidationDiscovery
	{
		public static void ConfigureForTest()
		{
			InfrastructureValidationDiscovery.InfrastructureValidationProbeMaxAttempts = 1;
			InfrastructureValidationDiscovery.InfrastructureValidationMonitorThreshold = 1;
			InfrastructureValidationDiscovery.InfrastructureValidationProbeRecurrence = (int)TimeSpan.FromSeconds(30.0).TotalSeconds;
			InfrastructureValidationDiscovery.InfrastructureValidationMonitoringInterval = (int)TimeSpan.FromSeconds(30.0).TotalSeconds;
			InfrastructureValidationDiscovery.InfrastructureValidationEscalateResponderWaitInterval = (int)TimeSpan.FromSeconds(10.0).TotalSeconds;
		}

		public static ProbeDefinition CreateInfrastructureValidationProbeDefinition(TracingContext tracingContext)
		{
			ArgumentValidator.ThrowIfNull("tracingContext", tracingContext);
			return TimeBasedAssistantsDiscoveryHelpers.CreateProbe(ExchangeComponent.TimeBasedAssistants, "MSExchangeMailboxAssistants", LocalServer.GetServer().Fqdn, typeof(InfrastructureValidationProbe), InfrastructureValidationDiscovery.InfrastructureValidationFailureProbeName, InfrastructureValidationDiscovery.InfrastructureValidationProbeRecurrence, InfrastructureValidationDiscovery.InfrastructureValidationProbeMaxAttempts, tracingContext);
		}

		public static MonitorDefinition CreateInfrastructureValidationMonitorDefinition(string mask, TracingContext tracingContext)
		{
			ArgumentValidator.ThrowIfNullOrWhiteSpace("mask", mask);
			ArgumentValidator.ThrowIfNull("tracingContext", tracingContext);
			MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition(InfrastructureValidationDiscovery.InfrastructureValidationMonitorName, mask, ExchangeComponent.TimeBasedAssistants.Name, ExchangeComponent.TimeBasedAssistants, InfrastructureValidationDiscovery.InfrastructureValidationMonitorThreshold, true, InfrastructureValidationDiscovery.InfrastructureValidationMonitoringInterval);
			monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Unhealthy, 0),
				new MonitorStateTransition(ServiceHealthStatus.Unrecoverable, monitorDefinition.RecurrenceIntervalSeconds / 2)
			};
			monitorDefinition.ServicePriority = 1;
			monitorDefinition.ScenarioDescription = "Find Time Based Assistants Infrastructure is not functional.";
			return monitorDefinition;
		}

		public static ResponderDefinition CreateInfrastructureValidationEscalateResponderDefinition(string monitorName, string alertMask)
		{
			ArgumentValidator.ThrowIfNullOrWhiteSpace("monitorName", monitorName);
			ArgumentValidator.ThrowIfNullOrWhiteSpace("alertMask", alertMask);
			ResponderDefinition responderDefinition = EscalateResponder.CreateDefinition(InfrastructureValidationDiscovery.InfrastructureValidationEscalateResponderName, ExchangeComponent.TimeBasedAssistants.Name, monitorName, alertMask, "MSExchangeMailboxAssistants", ServiceHealthStatus.Unrecoverable, ExchangeComponent.TimeBasedAssistants.EscalationTeam, Strings.InfrastructureValidationSubject, Strings.InfrastructureValidationMessage, true, NotificationServiceClass.UrgentInTraining, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
			responderDefinition.WaitIntervalSeconds = InfrastructureValidationDiscovery.InfrastructureValidationEscalateResponderWaitInterval;
			return responderDefinition;
		}

		private const string InfrastructureValidationWorkItemName = "InfrastructureValidation";

		private static int InfrastructureValidationProbeMaxAttempts = 3;

		private static int InfrastructureValidationMonitorThreshold = 10;

		private static readonly string InfrastructureValidationFailureProbeName = TimeBasedAssistantsDiscoveryHelpers.GenerateProbeName("InfrastructureValidation");

		private static readonly string InfrastructureValidationMonitorName = TimeBasedAssistantsDiscoveryHelpers.GenerateMonitorName("InfrastructureValidation");

		private static readonly string InfrastructureValidationEscalateResponderName = TimeBasedAssistantsDiscoveryHelpers.GenerateResponderName("InfrastructureValidation", "Escalate");

		private static int InfrastructureValidationProbeRecurrence = (int)TimeSpan.FromMinutes(30.0).TotalSeconds;

		private static int InfrastructureValidationMonitoringInterval = (int)TimeSpan.FromHours(6.0).TotalSeconds;

		private static int InfrastructureValidationEscalateResponderWaitInterval = (int)TimeSpan.FromHours(12.0).TotalSeconds;
	}
}
