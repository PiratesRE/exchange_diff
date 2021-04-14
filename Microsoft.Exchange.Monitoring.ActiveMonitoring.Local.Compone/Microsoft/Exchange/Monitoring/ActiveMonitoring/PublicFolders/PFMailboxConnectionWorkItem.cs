using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.PublicFolders
{
	internal class PFMailboxConnectionWorkItem
	{
		public static IEnumerable<MonitorDefinition> GenerateMonitorDefinitions(MaintenanceDefinition discoveryDefinition)
		{
			yield return PFMailboxConnectionWorkItem.CreateMonitorDefinition(discoveryDefinition);
			yield break;
		}

		public static IEnumerable<ResponderDefinition> GenerateResponderDefinitions(MaintenanceDefinition discoveryDefinition)
		{
			yield return PFMailboxConnectionWorkItem.CreatePFConnectionResponderDefinition();
			yield break;
		}

		public static MonitorDefinition CreateMonitorDefinition(MaintenanceDefinition discoveryDefinition)
		{
			bool enabled = bool.Parse(discoveryDefinition.Attributes["PFMailboxConnectionCountMonitorEnabled"]);
			int monitoringInterval = (int)TimeSpan.Parse(discoveryDefinition.Attributes["PFMailboxConnectionCountMonitorInterval"]).TotalSeconds;
			int recurrenceInterval = (int)TimeSpan.Parse(discoveryDefinition.Attributes["PFMailboxConnectionCountMonitorRecurranceInterval"]).TotalSeconds;
			int numberOfFailures = int.Parse(discoveryDefinition.Attributes["PFMailboxConnectionCountMonitorMinErrorCount"]);
			TimeSpan timeSpan = TimeSpan.Parse(discoveryDefinition.Attributes["PFMailboxConnectionCountTimeToEscalate"]);
			if (ExEnvironment.IsTest)
			{
				recurrenceInterval = (int)TimeSpan.FromSeconds(10.0).TotalSeconds;
				timeSpan = TimeSpan.FromSeconds(5.0);
				monitoringInterval = (int)TimeSpan.FromSeconds(60.0).TotalSeconds;
			}
			string name = ExchangeComponent.PublicFolders.Name;
			string sampleMask = NotificationItem.GenerateResultName(name, "PublicFolderMailboxConnectionCount", null);
			MonitorDefinition monitorDefinition = OverallXFailuresMonitor.CreateDefinition("PFMailboxConnectionCountMonitor", sampleMask, ExchangeComponent.PublicFolders.Name, ExchangeComponent.PublicFolders, monitoringInterval, recurrenceInterval, numberOfFailures, true);
			monitorDefinition.Enabled = enabled;
			monitorDefinition.TargetResource = monitorDefinition.ServiceName;
			monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Unrecoverable, (int)timeSpan.TotalSeconds)
			};
			monitorDefinition.ServicePriority = 2;
			monitorDefinition.ScenarioDescription = "Validate PublicFolders health is not impacted by mailbox connectivity issues";
			return monitorDefinition;
		}

		public static ResponderDefinition CreatePFConnectionResponderDefinition()
		{
			ResponderDefinition connectionResponderWorkItemDefinition = PFMailboxConnectionWorkItem.GetConnectionResponderWorkItemDefinition();
			if (ExEnvironment.IsTest)
			{
				connectionResponderWorkItemDefinition.TargetHealthState = ServiceHealthStatus.None;
				connectionResponderWorkItemDefinition.RecurrenceIntervalSeconds = (int)TimeSpan.FromSeconds(20.0).TotalSeconds;
			}
			return connectionResponderWorkItemDefinition;
		}

		public static ResponderDefinition GetConnectionResponderWorkItemDefinition()
		{
			ResponderDefinition result;
			if (LocalEndpointManager.IsDataCenter)
			{
				result = PublicFolderConnectionResponder.CreateDefinition("PFMailboxConnectionCountResponder", ExchangeComponent.PublicFolders.Name, "PFMailboxConnectionCountMonitor", string.Format("{0}/{1}", "PFMailboxConnectionCountMonitor", ExchangeComponent.PublicFolders.Name), "PublicFolderMailbox.ConnectionCount", ServiceHealthStatus.Unrecoverable, true);
			}
			else
			{
				result = EscalateResponder.CreateDefinition("PFMailboxConnectionCountResponder", ExchangeComponent.PublicFolders.Name, "PFMailboxConnectionCountMonitor", string.Format("{0}/{1}", "PFMailboxConnectionCountMonitor", ExchangeComponent.PublicFolders.Name), "PublicFolderMailbox.ConnectionCount", ServiceHealthStatus.Unrecoverable, ExchangeComponent.PublicFolders.EscalationTeam, Strings.PublicFolderConnectionCountEscalationSubject, Strings.EscalationMessageFailuresUnhealthy(Strings.PublicFolderConnectionCountEscalationMessage), true, NotificationServiceClass.Scheduled, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
			}
			return result;
		}

		private const string MonitorEnabledAttributeName = "PFMailboxConnectionCountMonitorEnabled";

		private const string MonitorIntervalAttributeName = "PFMailboxConnectionCountMonitorInterval";

		private const string MonitorRecurranceIntervalAttributeName = "PFMailboxConnectionCountMonitorRecurranceInterval";

		private const string MonitorMinErrorCountAttributeName = "PFMailboxConnectionCountMonitorMinErrorCount";

		private const string EscalateTimeAttributeName = "PFMailboxConnectionCountTimeToEscalate";

		private const string ConnectionCountMonitorName = "PFMailboxConnectionCountMonitor";

		private const string ConnectionCountResponderName = "PFMailboxConnectionCountResponder";

		private const string ConnectionCountNameMask = "PublicFolderMailboxConnectionCount";
	}
}
