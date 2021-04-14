using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Journaling
{
	internal class JournalFilterAgentWorkItem
	{
		public static MonitorDefinition CreateMonitorDefinition(MaintenanceDefinition discoveryDefinition)
		{
			bool enabled = bool.Parse(discoveryDefinition.Attributes["JournalFilterAgentMonitorEnabled"]);
			int monitoringInterval = (int)TimeSpan.Parse(discoveryDefinition.Attributes["JournalFilterAgentMonitorInterval"]).TotalSeconds;
			int recurrenceInterval = (int)TimeSpan.Parse(discoveryDefinition.Attributes["JournalFilterAgentMonitorRecurranceInterval"]).TotalSeconds;
			if (ExEnvironment.IsTest)
			{
				recurrenceInterval = (int)TimeSpan.FromSeconds(60.0).TotalSeconds;
			}
			int numberOfFailures = int.Parse(discoveryDefinition.Attributes["JournalFilterAgentMonitorMinErrorCount"]);
			TimeSpan timeSpan = TimeSpan.Parse(discoveryDefinition.Attributes["JournalFilterAgentTimeToEscalate"]);
			string name = ExchangeComponent.Compliance.Name;
			string sampleMask = NotificationItem.GenerateResultName(name, "JournalFilterAgentComponent", null);
			MonitorDefinition monitorDefinition = OverallXFailuresMonitor.CreateDefinition("JournalFilterAgentMonitor", sampleMask, ExchangeComponent.Compliance.Name, ExchangeComponent.Compliance, monitoringInterval, recurrenceInterval, numberOfFailures, enabled);
			monitorDefinition.TargetResource = monitorDefinition.ServiceName;
			monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Unrecoverable, (int)timeSpan.TotalSeconds)
			};
			return monitorDefinition;
		}

		public static ResponderDefinition CreateEscalateResponderDefinition(MaintenanceDefinition discoveryDefinition)
		{
			string alertMask = string.Format("{0}/{1}", "JournalFilterAgentMonitor", ExchangeComponent.Compliance.Name);
			return EscalateResponder.CreateDefinition("JournalFilterAgentResponder", ExchangeComponent.Compliance.Name, "JournalFilterAgentMonitor", alertMask, "Compliance", ServiceHealthStatus.Unrecoverable, ExchangeComponent.Compliance.EscalationTeam, Strings.JournalFilterAgentEscalationSubject, Strings.JournalFilterAgentEscalationMessage, true, NotificationServiceClass.UrgentInTraining, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
		}

		private const string MonitorEnabledAttributeName = "JournalFilterAgentMonitorEnabled";

		private const string MonitorIntervalAttributeName = "JournalFilterAgentMonitorInterval";

		private const string MonitorRecurranceIntervalAttributeName = "JournalFilterAgentMonitorRecurranceInterval";

		private const string MonitorMinErrorCountAttributeName = "JournalFilterAgentMonitorMinErrorCount";

		private const string EscalateTimeAttributeName = "JournalFilterAgentTimeToEscalate";

		private const string MonitorName = "JournalFilterAgentMonitor";

		private const string ResponderName = "JournalFilterAgentResponder";
	}
}
