using System;
using System.Collections.Generic;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.PublicFolders
{
	internal class PublicFolderSyncSuccessWorkItem
	{
		public static IEnumerable<MonitorDefinition> GenerateMonitorDefinitions(MaintenanceDefinition discoveryDefinition)
		{
			yield return PublicFolderSyncSuccessWorkItem.CreateMonitorDefinition(discoveryDefinition);
			yield break;
		}

		public static IEnumerable<ResponderDefinition> GenerateResponderDefinitions(MaintenanceDefinition discoveryDefinition)
		{
			yield return PublicFolderSyncSuccessWorkItem.CreateEscalateResponderDefinition(discoveryDefinition);
			yield break;
		}

		public static MonitorDefinition CreateMonitorDefinition(MaintenanceDefinition discoveryDefinition)
		{
			bool enabled = bool.Parse(discoveryDefinition.Attributes["PublicFolderSyncSuccessMonitorEnabled"]);
			int monitoringInterval = (int)TimeSpan.Parse(discoveryDefinition.Attributes["PublicFolderSyncSuccessMonitorInterval"]).TotalSeconds;
			int recurrenceInterval = (int)TimeSpan.Parse(discoveryDefinition.Attributes["PublicFolderSyncSuccessRecurrenceInterval"]).TotalSeconds;
			int numberOfFailures = int.Parse(discoveryDefinition.Attributes["PublicFolderSyncSuccessMonitorMinErrorCount"]);
			TimeSpan timeSpan = TimeSpan.Parse(discoveryDefinition.Attributes["PublicFolderSyncSuccessTimeToEscalate"]);
			string name = ExchangeComponent.PublicFolders.Name;
			string component = "PublicFolderMailboxSync";
			string sampleMask = NotificationItem.GenerateResultName(name, component, null);
			MonitorDefinition monitorDefinition = OverallXFailuresMonitor.CreateDefinition("PublicFolderSyncFailureMonitor", sampleMask, ExchangeComponent.PublicFolders.Name, ExchangeComponent.PublicFolders, monitoringInterval, recurrenceInterval, numberOfFailures, true);
			monitorDefinition.Enabled = enabled;
			monitorDefinition.TargetResource = monitorDefinition.ServiceName;
			monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Unrecoverable, (int)timeSpan.TotalSeconds)
			};
			monitorDefinition.ServicePriority = 2;
			monitorDefinition.ScenarioDescription = "Validate PublicFolder health is not impacted by sync issues";
			return monitorDefinition;
		}

		public static ResponderDefinition CreateEscalateResponderDefinition(MaintenanceDefinition discoveryDefinition)
		{
			string targetResource = "PublicFolderMailboxSync";
			int durationMinutes = (int)TimeSpan.Parse(discoveryDefinition.Attributes["PublicFolderSyncSuccessMonitorInterval"]).TotalMinutes;
			int minCount = int.Parse(discoveryDefinition.Attributes["PublicFolderSyncSuccessMonitorMinErrorCount"]);
			string alertMask = string.Format("{0}/{1}", "PublicFolderSyncFailureMonitor", ExchangeComponent.PublicFolders.Name);
			string escalationMessageUnhealthy = Strings.PublicFolderSyncEscalationMessage(minCount, durationMinutes);
			return EscalateResponder.CreateDefinition("PublicFolderSyncFailureEscalate", ExchangeComponent.PublicFolders.Name, "PublicFolderSyncFailureMonitor", alertMask, targetResource, ServiceHealthStatus.Unrecoverable, ExchangeComponent.PublicFolders.EscalationTeam, Strings.PublicFolderSyncEscalationSubject, escalationMessageUnhealthy, true, NotificationServiceClass.Scheduled, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
		}

		private const string MonitorEnabledAttributeName = "PublicFolderSyncSuccessMonitorEnabled";

		private const string MonitorIntervalAttributeName = "PublicFolderSyncSuccessMonitorInterval";

		private const string RecurrenceIntervalAttributeName = "PublicFolderSyncSuccessRecurrenceInterval";

		private const string MonitorMinErrorCountAttributeName = "PublicFolderSyncSuccessMonitorMinErrorCount";

		private const string EscalateTimeAttributeName = "PublicFolderSyncSuccessTimeToEscalate";

		private const string MonitorName = "PublicFolderSyncFailureMonitor";

		private const string EscalateResponderName = "PublicFolderSyncFailureEscalate";
	}
}
