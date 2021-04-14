using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.PublicFolders
{
	internal class PFMailboxQuotaWorkItem
	{
		public static IEnumerable<MonitorDefinition> GenerateMonitorDefinitions(MaintenanceDefinition discoveryDefinition)
		{
			yield return PFMailboxQuotaWorkItem.CreateMonitorDefinition(discoveryDefinition);
			yield break;
		}

		public static IEnumerable<ResponderDefinition> GenerateResponderDefinitions(MaintenanceDefinition discoveryDefinition)
		{
			yield return PFMailboxQuotaWorkItem.CreatePFMailboxQuotaResponderDefinition();
			yield break;
		}

		public static MonitorDefinition CreateMonitorDefinition(MaintenanceDefinition discoveryDefinition)
		{
			bool enabled = bool.Parse(discoveryDefinition.Attributes["PFMailboxQuotaWarningMonitorEnabled"]);
			int monitoringInterval = (int)TimeSpan.Parse(discoveryDefinition.Attributes["PFMailboxQuotaWarningMonitorInterval"]).TotalSeconds;
			int recurrenceInterval = (int)TimeSpan.Parse(discoveryDefinition.Attributes["PFMailboxQuotaWarningMonitorRecurranceInterval"]).TotalSeconds;
			int numberOfFailures = int.Parse(discoveryDefinition.Attributes["PFMailboxQuotaWarningMonitorMinErrorCount"]);
			TimeSpan timeSpan = TimeSpan.Parse(discoveryDefinition.Attributes["PFMailboxQuotaWarningTimeToEscalate"]);
			if (ExEnvironment.IsTest)
			{
				recurrenceInterval = (int)TimeSpan.FromSeconds(10.0).TotalSeconds;
				timeSpan = TimeSpan.FromSeconds(5.0);
				monitoringInterval = (int)TimeSpan.FromSeconds(120.0).TotalSeconds;
			}
			string name = ExchangeComponent.PublicFolders.Name;
			string sampleMask = NotificationItem.GenerateResultName(name, "PublicFolderMailboxQuota", null);
			MonitorDefinition monitorDefinition = OverallXFailuresMonitor.CreateDefinition("PublicFolderMailboxQuotaMonitor", sampleMask, ExchangeComponent.PublicFolders.Name, ExchangeComponent.PublicFolders, monitoringInterval, recurrenceInterval, numberOfFailures, true);
			monitorDefinition.Enabled = enabled;
			monitorDefinition.TargetResource = monitorDefinition.ServiceName;
			monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Unrecoverable, (int)timeSpan.TotalSeconds)
			};
			monitorDefinition.ServicePriority = 2;
			monitorDefinition.ScenarioDescription = "Validate PublicFolders health is not impacted by mailbox quota issues";
			return monitorDefinition;
		}

		public static ResponderDefinition CreatePFMailboxQuotaResponderDefinition()
		{
			ResponderDefinition mailboxQuotaResponderWorkItemDefiniteion = PFMailboxQuotaWorkItem.GetMailboxQuotaResponderWorkItemDefiniteion();
			if (ExEnvironment.IsTest)
			{
				mailboxQuotaResponderWorkItemDefiniteion.TargetHealthState = ServiceHealthStatus.None;
				mailboxQuotaResponderWorkItemDefiniteion.RecurrenceIntervalSeconds = (int)TimeSpan.FromSeconds(200.0).TotalSeconds;
				mailboxQuotaResponderWorkItemDefiniteion.MinimumSecondsBetweenEscalates = (int)TimeSpan.FromSeconds(5.0).TotalSeconds;
				mailboxQuotaResponderWorkItemDefiniteion.WaitIntervalSeconds = (int)TimeSpan.FromSeconds(5.0).TotalSeconds;
			}
			else
			{
				mailboxQuotaResponderWorkItemDefiniteion.RecurrenceIntervalSeconds = (int)TimeSpan.FromMinutes(30.0).TotalSeconds;
			}
			return mailboxQuotaResponderWorkItemDefiniteion;
		}

		public static ResponderDefinition GetMailboxQuotaResponderWorkItemDefiniteion()
		{
			ResponderDefinition result;
			if (LocalEndpointManager.IsDataCenter)
			{
				result = PublicFolderMailboxResponder.CreateDefinition("PublicFolderMailboxQuotaResponder", ExchangeComponent.PublicFolders.Name, "PublicFolderMailboxQuotaMonitor", string.Format("{0}/{1}", "PublicFolderMailboxQuotaMonitor", ExchangeComponent.PublicFolders.Name), "PublicFolderMailbox.Quota", ServiceHealthStatus.Unrecoverable, true, NotificationServiceClass.Scheduled);
			}
			else
			{
				result = EscalateResponder.CreateDefinition("PublicFolderMailboxQuotaResponder", ExchangeComponent.PublicFolders.Name, "PublicFolderMailboxQuotaMonitor", string.Format("{0}/{1}", "PublicFolderMailboxQuotaMonitor", ExchangeComponent.PublicFolders.Name), "PublicFolderMailbox.Quota", ServiceHealthStatus.Unrecoverable, ExchangeComponent.PublicFolders.EscalationTeam, Strings.PublicFolderMailboxQuotaEscalationSubject, Strings.PublicFolderMailboxQuotaEscalationMessage, true, NotificationServiceClass.Scheduled, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
			}
			return result;
		}

		private const string MonitorEnabledAttributeName = "PFMailboxQuotaWarningMonitorEnabled";

		private const string MonitorIntervalAttributeName = "PFMailboxQuotaWarningMonitorInterval";

		private const string MonitorRecurranceIntervalAttributeName = "PFMailboxQuotaWarningMonitorRecurranceInterval";

		private const string MonitorMinErrorCountAttributeName = "PFMailboxQuotaWarningMonitorMinErrorCount";

		private const string EscalateTimeAttributeName = "PFMailboxQuotaWarningTimeToEscalate";

		private const string MonitorName = "PublicFolderMailboxQuotaMonitor";

		private const string PFMailboxQuotaResponderName = "PublicFolderMailboxQuotaResponder";

		private const string PublicFolderMailboxQuotaNameMask = "PublicFolderMailboxQuota";
	}
}
