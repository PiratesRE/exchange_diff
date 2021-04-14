using System;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local.Components.Common.Probes;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.SiteMailbox
{
	internal class ServiceHostProcessCrashDetectionWorkItem
	{
		public static ProbeDefinition CreateProbeDefinition(MaintenanceDefinition discoveryDefinition)
		{
			int recurrenceInterval = (int)TimeSpan.Parse(discoveryDefinition.Attributes["ServiceHostCrashDetectionProbeFrequency"]).TotalSeconds;
			bool enabled = bool.Parse(discoveryDefinition.Attributes["ServiceHostCrashDetectionProbeEnabled"]);
			ProbeDefinition probeDefinition = GenericProcessCrashDetectionProbe.CreateDefinition("ExchangeServiceHostProcessRepeatedlyCrashingProbe", "Microsoft.Exchange.ServiceHost", recurrenceInterval, null, false);
			probeDefinition.Enabled = enabled;
			probeDefinition.MaxRetryAttempts = 3;
			probeDefinition.ServiceName = ExchangeComponent.SiteMailbox.Name;
			return probeDefinition;
		}

		public static MonitorDefinition CreateMonitorDefinition(MaintenanceDefinition discoveryDefinition)
		{
			int recurrenceInterval = (int)TimeSpan.Parse(discoveryDefinition.Attributes["ServiceHostCrashDetectionProbeFrequency"]).TotalSeconds;
			int numberOfFailures = int.Parse(discoveryDefinition.Attributes["ServiceHostCrashDetectionMonitorThreshold"]);
			int monitoringInterval = (int)TimeSpan.Parse(discoveryDefinition.Attributes["ServiceHostCrashDetectionMonitorInterval"]).TotalSeconds;
			TimeSpan zero = TimeSpan.Zero;
			TimeSpan timeSpan = TimeSpan.Parse(discoveryDefinition.Attributes["ServiceHostCrashTimeToEscalate"]);
			MonitorDefinition monitorDefinition = OverallXFailuresMonitor.CreateDefinition("ExchangeServiceHostProcessRepeatedlyCrashingMonitor", string.Format("{0}/{1}", "ExchangeServiceHostProcessRepeatedlyCrashingProbe", "Microsoft.Exchange.ServiceHost"), ExchangeComponent.SiteMailbox.Name, ExchangeComponent.SiteMailbox, monitoringInterval, recurrenceInterval, numberOfFailures, true);
			monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Degraded, (int)zero.TotalSeconds),
				new MonitorStateTransition(ServiceHealthStatus.Unhealthy, (int)timeSpan.TotalSeconds)
			};
			monitorDefinition.ServicePriority = 2;
			monitorDefinition.ScenarioDescription = "Validate SiteMailbox health is not impacted by service host process crashe issues";
			return monitorDefinition;
		}

		public static ResponderDefinition CreateEscalateResponderDefinition(MaintenanceDefinition discoveryDefinition)
		{
			int minCount = int.Parse(discoveryDefinition.Attributes["ServiceHostCrashDetectionMonitorThreshold"]);
			int durationMinutes = (int)TimeSpan.Parse(discoveryDefinition.Attributes["ServiceHostCrashDetectionMonitorInterval"]).TotalMinutes;
			string escalationMessageUnhealthy = Strings.ProcessRepeatedlyCrashingEscalationMessage("Microsoft.Exchange.ServiceHost", minCount, durationMinutes);
			string escalationSubjectUnhealthy = Strings.ProcessRepeatedlyCrashingEscalationSubject("Microsoft.Exchange.ServiceHost");
			return EscalateResponder.CreateDefinition("ExchangeServiceHostProcessRepeatedlyCrashingEscalate", ExchangeComponent.SiteMailbox.Name, "ExchangeServiceHostProcessRepeatedlyCrashingMonitor", string.Format("{0}/{1}", "ExchangeServiceHostProcessRepeatedlyCrashingMonitor", ExchangeComponent.SiteMailbox.Name), "Microsoft.Exchange.ServiceHost", ServiceHealthStatus.Unhealthy, ExchangeComponent.SiteMailbox.EscalationTeam, escalationSubjectUnhealthy, escalationMessageUnhealthy, true, NotificationServiceClass.Urgent, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
		}

		private const string ServiceHostProcessName = "Microsoft.Exchange.ServiceHost";

		private const string ServiceHostCrashDetectionProbeEnabledName = "ServiceHostCrashDetectionProbeEnabled";

		private const string ServiceHostCrashDetectionProbeFrequencyName = "ServiceHostCrashDetectionProbeFrequency";

		private const string ServiceHostCrashDetectionMonitorThresholdName = "ServiceHostCrashDetectionMonitorThreshold";

		private const string ServiceHostCrashDetectionMonitorIntervalName = "ServiceHostCrashDetectionMonitorInterval";

		private const string EscalateTransitionSpanName = "ServiceHostCrashTimeToEscalate";

		private const string ServiceHostProcessRepeatedlyCrashingProbeName = "ExchangeServiceHostProcessRepeatedlyCrashingProbe";

		private const string ServiceHostProcessRepeatedlyCrashingMonitorName = "ExchangeServiceHostProcessRepeatedlyCrashingMonitor";

		private const string ServiceHostProcessRepeatedlyCrashingEscalateResponderName = "ExchangeServiceHostProcessRepeatedlyCrashingEscalate";
	}
}
