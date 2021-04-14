using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.PublicFolders
{
	internal class PublicFolderMoveJobStuckWorkItem
	{
		public static IEnumerable<MonitorDefinition> GenerateMonitorDefinitions(MaintenanceDefinition discoveryDefinition)
		{
			yield return PublicFolderMoveJobStuckWorkItem.CreateMonitorDefinition(discoveryDefinition);
			yield break;
		}

		public static IEnumerable<ResponderDefinition> GenerateResponderDefinitions(MaintenanceDefinition discoveryDefinition)
		{
			yield return PublicFolderMoveJobStuckWorkItem.CreateEscalateResponderDefinition(discoveryDefinition);
			yield break;
		}

		public static MonitorDefinition CreateMonitorDefinition(MaintenanceDefinition discoveryDefinition)
		{
			bool enabled = bool.Parse(discoveryDefinition.Attributes["PublicFolderMoveJobStuckMonitorEnabled"]);
			int monitoringInterval = (int)TimeSpan.Parse(discoveryDefinition.Attributes["PublicFolderMoveJobStuckMonitorInterval"]).TotalSeconds;
			int recurrenceInterval = (int)TimeSpan.Parse(discoveryDefinition.Attributes["PublicFolderMoveJobStuckRecurrenceInterval"]).TotalSeconds;
			int numberOfFailures = int.Parse(discoveryDefinition.Attributes["PublicFolderMoveJobStuckMonitorMinErrorCount"]);
			TimeSpan timeSpan = TimeSpan.Parse(discoveryDefinition.Attributes["PublicFolderMoveJobStuckTimeToEscalate"]);
			string name = ExchangeComponent.PublicFolders.Name;
			string component = "PFMoveJobStuck";
			string sampleMask = NotificationItem.GenerateResultName(name, component, null);
			if (ExEnvironment.IsTest)
			{
				recurrenceInterval = (int)TimeSpan.FromSeconds(10.0).TotalSeconds;
				timeSpan = TimeSpan.FromSeconds(5.0);
				monitoringInterval = (int)TimeSpan.FromSeconds(60.0).TotalSeconds;
			}
			MonitorDefinition monitorDefinition = OverallXFailuresMonitor.CreateDefinition("PublicFolderMoveJobStuckMonitor", sampleMask, ExchangeComponent.PublicFolders.Name, ExchangeComponent.PublicFolders, monitoringInterval, recurrenceInterval, numberOfFailures, true);
			monitorDefinition.Enabled = enabled;
			monitorDefinition.TargetResource = monitorDefinition.ServiceName;
			monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Unrecoverable, (int)timeSpan.TotalSeconds)
			};
			monitorDefinition.ServicePriority = 2;
			monitorDefinition.ScenarioDescription = "Validate PublicFolder health is not impacted by move job issues";
			return monitorDefinition;
		}

		public static ResponderDefinition CreateEscalateResponderDefinition(MaintenanceDefinition discoveryDefinition)
		{
			string targetResource = "PFMoveJobStuck";
			double totalMinutes = TimeSpan.Parse(discoveryDefinition.Attributes["PublicFolderMoveJobStuckMonitorInterval"]).TotalMinutes;
			int.Parse(discoveryDefinition.Attributes["PublicFolderMoveJobStuckMonitorMinErrorCount"]);
			string alertMask = string.Format("{0}/{1}", "PublicFolderMoveJobStuckMonitor", ExchangeComponent.PublicFolders.Name);
			ResponderDefinition responderDefinition = EscalateResponder.CreateDefinition("PublicFolderMoveJobStuckEscalate", ExchangeComponent.PublicFolders.Name, "PublicFolderMoveJobStuckMonitor", alertMask, targetResource, ServiceHealthStatus.Unrecoverable, ExchangeComponent.PublicFolders.EscalationTeam, Strings.PublicFolderMoveJobStuckEscalationSubject, Strings.PublicFolderMoveJobStuckEscalationMessage, true, NotificationServiceClass.Scheduled, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
			if (ExEnvironment.IsTest)
			{
				responderDefinition.TargetHealthState = ServiceHealthStatus.None;
				responderDefinition.RecurrenceIntervalSeconds = (int)TimeSpan.FromSeconds(20.0).TotalSeconds;
			}
			return responderDefinition;
		}

		private const string MonitorEnabledAttributeName = "PublicFolderMoveJobStuckMonitorEnabled";

		private const string MonitorIntervalAttributeName = "PublicFolderMoveJobStuckMonitorInterval";

		private const string RecurrenceIntervalAttributeName = "PublicFolderMoveJobStuckRecurrenceInterval";

		private const string MonitorMinErrorCountAttributeName = "PublicFolderMoveJobStuckMonitorMinErrorCount";

		private const string EscalateTimeAttributeName = "PublicFolderMoveJobStuckTimeToEscalate";

		private const string MonitorName = "PublicFolderMoveJobStuckMonitor";

		private const string EscalateResponderName = "PublicFolderMoveJobStuckEscalate";
	}
}
