using System;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.JournalArchive
{
	internal class JournalArchiveWorkItem
	{
		public static MonitorDefinition CreateMonitorDefinition(MaintenanceDefinition discoveryDefinition)
		{
			bool enabled = bool.Parse(discoveryDefinition.Attributes["JournalArchiveMonitorEnabled"]);
			int monitoringInterval = (int)TimeSpan.Parse(discoveryDefinition.Attributes["JournalArchiveMonitorInterval"]).TotalSeconds;
			int recurrenceInterval = (int)TimeSpan.Parse(discoveryDefinition.Attributes["JournalArchiveMonitorRecurrenceInterval"]).TotalSeconds;
			int numberOfFailures = int.Parse(discoveryDefinition.Attributes["JournalArchiveMonitorMinErrorCount"]);
			TimeSpan timeSpan = TimeSpan.Parse(discoveryDefinition.Attributes["JournalingTimeToEscalate"]);
			string name = ExchangeComponent.JournalArchive.Name;
			string sampleMask = NotificationItem.GenerateResultName(name, "JournalArchiveComponent", null);
			MonitorDefinition monitorDefinition = OverallXFailuresMonitor.CreateDefinition("JournalArchiveMonitor", sampleMask, ExchangeComponent.JournalArchive.Name, ExchangeComponent.JournalArchive, monitoringInterval, recurrenceInterval, numberOfFailures, true);
			monitorDefinition.Enabled = enabled;
			monitorDefinition.TargetResource = monitorDefinition.ServiceName;
			monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Unrecoverable, (int)timeSpan.TotalSeconds)
			};
			return monitorDefinition;
		}

		public static ResponderDefinition CreateEscalateResponderDefinition(MaintenanceDefinition discoveryDefinition)
		{
			string alertMask = string.Format("{0}/{1}", "JournalArchiveMonitor", ExchangeComponent.JournalArchive.Name);
			return JournalArchiveResponder.CreateDefinition("JournalArchiveResponder", ExchangeComponent.JournalArchive.Name, "JournalArchiveMonitor", alertMask, "JournalArchive", ServiceHealthStatus.Unrecoverable, ExchangeComponent.JournalArchive.EscalationTeam, Strings.JournalArchiveEscalationSubject, Strings.JournalArchiveEscalationMessage, NotificationServiceClass.UrgentInTraining, true);
		}

		private const string MonitorEnabledAttributeName = "JournalArchiveMonitorEnabled";

		private const string MonitorIntervalAttributeName = "JournalArchiveMonitorInterval";

		private const string MonitorRecurranceIntervalAttributeName = "JournalArchiveMonitorRecurrenceInterval";

		private const string MonitorMinErrorCountAttributeName = "JournalArchiveMonitorMinErrorCount";

		private const string EscalateTimeAttributeName = "JournalingTimeToEscalate";

		private const string MonitorName = "JournalArchiveMonitor";

		private const string ResponderName = "JournalArchiveResponder";
	}
}
