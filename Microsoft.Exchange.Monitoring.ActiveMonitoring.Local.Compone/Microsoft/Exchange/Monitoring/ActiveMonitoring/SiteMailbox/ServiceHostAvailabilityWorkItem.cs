using System;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.SiteMailbox
{
	internal class ServiceHostAvailabilityWorkItem
	{
		public static ProbeDefinition CreateProbeDefinition(MaintenanceDefinition discoveryDefinition)
		{
			TimeSpan timeSpan = TimeSpan.Parse(discoveryDefinition.Attributes["ServiceHostServiceProbeFrequency"]);
			bool enabled = bool.Parse(discoveryDefinition.Attributes["ServiceHostServiceProbeEnabled"]);
			return new ProbeDefinition
			{
				TypeName = typeof(GenericServiceProbe).FullName,
				AssemblyPath = typeof(GenericServiceProbe).Assembly.Location,
				Enabled = enabled,
				MaxRetryAttempts = 3,
				Name = "ExchangeServiceHostServiceRunningProbe",
				RecurrenceIntervalSeconds = (int)timeSpan.TotalSeconds,
				ServiceName = ExchangeComponent.SiteMailbox.Name,
				TargetResource = "MSExchangeServiceHost",
				TimeoutSeconds = (int)timeSpan.TotalSeconds
			};
		}

		public static MonitorDefinition CreateMonitorDefinition(MaintenanceDefinition discoveryDefinition)
		{
			int num = (int)TimeSpan.Parse(discoveryDefinition.Attributes["ServiceHostServiceProbeFrequency"]).TotalSeconds;
			int num2 = int.Parse(discoveryDefinition.Attributes["ServiceHostServiceMonitorThreshold"]);
			int monitoringInterval = num * num2;
			TimeSpan t = TimeSpan.Parse(discoveryDefinition.Attributes["ServiceHostServiceTimeToRestart"]);
			TimeSpan timeSpan = t + TimeSpan.FromMinutes(30.0);
			TimeSpan timeSpan2 = TimeSpan.Parse(discoveryDefinition.Attributes["ServiceHostServiceTimeToEscalate"]);
			MonitorDefinition monitorDefinition = OverallXFailuresMonitor.CreateDefinition("ExchangeServiceHostServiceRunningMonitor", "ExchangeServiceHostServiceRunningProbe", ExchangeComponent.SiteMailbox.Name, ExchangeComponent.SiteMailbox, monitoringInterval, num, num2, true);
			monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Degraded, (int)t.TotalSeconds),
				new MonitorStateTransition(ServiceHealthStatus.Unhealthy, (int)timeSpan.TotalSeconds),
				new MonitorStateTransition(ServiceHealthStatus.Unrecoverable, (int)timeSpan2.TotalSeconds)
			};
			monitorDefinition.ServicePriority = 2;
			monitorDefinition.ScenarioDescription = "Validate SiteMailbox health is not impacted by service host availability issues";
			return monitorDefinition;
		}

		public static ResponderDefinition CreateRecoveryResponderDefinition()
		{
			string monitorName = string.Format("{0}/{1}", "ExchangeServiceHostServiceRunningMonitor", ExchangeComponent.SiteMailbox.Name);
			ResponderDefinition responderDefinition = RestartServiceResponder.CreateDefinition("ExchangeServiceHostServiceRestart", monitorName, "MSExchangeServiceHost", ServiceHealthStatus.Degraded, 15, 120, 0, false, DumpMode.None, null, 15.0, 0, "Exchange", null, true, true, "Dag", false);
			responderDefinition.ServiceName = ExchangeComponent.SiteMailbox.Name;
			return responderDefinition;
		}

		public static ResponderDefinition CreateRecovery2ResponderDefinition()
		{
			string monitorName = string.Format("{0}/{1}", "ExchangeServiceHostServiceRunningMonitor", ExchangeComponent.SiteMailbox.Name);
			ResponderDefinition responderDefinition = RestartServiceResponder.CreateDefinition("ExchangeServiceHostServiceRestart2", monitorName, "MSExchangeServiceHost", ServiceHealthStatus.Unhealthy, 15, 120, 0, false, DumpMode.None, null, 15.0, 0, "Exchange", null, true, true, "Dag", false);
			responderDefinition.ServiceName = ExchangeComponent.SiteMailbox.Name;
			return responderDefinition;
		}

		public static ResponderDefinition CreateEscalateResponderDefinition()
		{
			string alertMask = string.Format("{0}/{1}", "ExchangeServiceHostServiceRunningMonitor", ExchangeComponent.SiteMailbox.Name);
			string escalationSubjectUnhealthy = Strings.ServiceNotRunningEscalationSubject("MSExchangeServiceHost");
			string escalationMessageUnhealthy;
			if (Datacenter.IsMicrosoftHostedOnly(true))
			{
				escalationMessageUnhealthy = Strings.ServiceNotRunningEscalationMessageDc("MSExchangeServiceHost");
			}
			else
			{
				escalationMessageUnhealthy = Strings.ServiceNotRunningEscalationMessageEnt("MSExchangeServiceHost");
			}
			return EscalateResponder.CreateDefinition("ExchangeServiceHostRunningEscalate", ExchangeComponent.SiteMailbox.Name, "ExchangeServiceHostServiceRunningMonitor", alertMask, "MSExchangeServiceHost", ServiceHealthStatus.Unrecoverable, ExchangeComponent.SiteMailbox.EscalationTeam, escalationSubjectUnhealthy, escalationMessageUnhealthy, true, NotificationServiceClass.Urgent, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
		}

		private const string ServiceHostServiceName = "MSExchangeServiceHost";

		private const string ServiceHostServiceProbeEnabledName = "ServiceHostServiceProbeEnabled";

		private const string ServiceHostServiceProbeFrequencyName = "ServiceHostServiceProbeFrequency";

		private const string ServiceHostServiceMonitorThresholdName = "ServiceHostServiceMonitorThreshold";

		private const string RestartTransitionSpanName = "ServiceHostServiceTimeToRestart";

		private const string EscalateTransitionSpanName = "ServiceHostServiceTimeToEscalate";

		private const string ServiceHostServiceProbeName = "ExchangeServiceHostServiceRunningProbe";

		private const string ServiceHostServiceMonitorName = "ExchangeServiceHostServiceRunningMonitor";

		private const string ServiceHostServiceRestartResponderName = "ExchangeServiceHostServiceRestart";

		private const string ServiceHostEscalateResponderName = "ExchangeServiceHostRunningEscalate";
	}
}
