using System;
using Microsoft.Exchange.Data.Storage.LinkedFolder;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.SiteMailbox
{
	internal class SiteMailboxSyncSuccessWorkItem
	{
		public static MonitorDefinition CreateMonitorDefinition(MaintenanceDefinition discoveryDefinition)
		{
			bool enabled = bool.Parse(discoveryDefinition.Attributes["SiteMailboxSyncSuccessMonitorEnabled"]);
			double availabilityPercentage = double.Parse(discoveryDefinition.Attributes["SiteMailboxSyncSuccessMonitorThreshold"]);
			TimeSpan monitoringInterval = TimeSpan.Parse(discoveryDefinition.Attributes["SiteMailboxSyncSuccessMonitorInterval"]);
			int minimumErrorCount = int.Parse(discoveryDefinition.Attributes["SiteMailboxSyncSuccessMonitorMinErrorCount"]);
			TimeSpan zero = TimeSpan.Zero;
			TimeSpan timeSpan = TimeSpan.Parse(discoveryDefinition.Attributes["SiteMailboxSyncSuccessTimeToEscalate"]);
			string name = ExchangeComponent.SiteMailbox.Name;
			string name2 = typeof(SiteSynchronizer).Name;
			string sampleMask = NotificationItem.GenerateResultName(name, name2, null);
			MonitorDefinition monitorDefinition = OverallPercentSuccessMonitor.CreateDefinition("SiteMailboxDocumentSyncPercentSuccessMonitor", sampleMask, ExchangeComponent.SiteMailbox.Name, ExchangeComponent.SiteMailbox, availabilityPercentage, monitoringInterval, minimumErrorCount, true);
			monitorDefinition.Enabled = enabled;
			monitorDefinition.TargetResource = monitorDefinition.ServiceName;
			monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Degraded, (int)zero.TotalSeconds),
				new MonitorStateTransition(ServiceHealthStatus.Unrecoverable, (int)timeSpan.TotalSeconds)
			};
			monitorDefinition.ServicePriority = 2;
			monitorDefinition.ScenarioDescription = "Validate SiteMailbox health is not impacted by document sync issues";
			return monitorDefinition;
		}

		public static ResponderDefinition CreateEscalateResponderDefinition(MaintenanceDefinition discoveryDefinition)
		{
			string name = typeof(DocumentLibSynchronizer).Name;
			double num = double.Parse(discoveryDefinition.Attributes["SiteMailboxSyncSuccessMonitorThreshold"]);
			string alertMask = string.Format("{0}/{1}", "SiteMailboxDocumentSyncPercentSuccessMonitor", ExchangeComponent.SiteMailbox.Name);
			string escalationMessageUnhealthy = Strings.SiteMailboxDocumentSyncEscalationMessage(Convert.ToInt32(100.0 - num));
			return EscalateResponder.CreateDefinition("SiteMailboxDocumentSyncEscalate", ExchangeComponent.SiteMailbox.Name, "SiteMailboxDocumentSyncPercentSuccessMonitor", alertMask, name, ServiceHealthStatus.Unrecoverable, ExchangeComponent.SiteMailbox.EscalationTeam, Strings.SiteMailboxDocumentSyncEscalationSubject, escalationMessageUnhealthy, true, NotificationServiceClass.Urgent, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
		}

		private const string MonitorEnabledAttributeName = "SiteMailboxSyncSuccessMonitorEnabled";

		private const string MonitorThresholdAttributeName = "SiteMailboxSyncSuccessMonitorThreshold";

		private const string MonitorIntervalAttributeName = "SiteMailboxSyncSuccessMonitorInterval";

		private const string MonitorMinErrorCountAttributeName = "SiteMailboxSyncSuccessMonitorMinErrorCount";

		private const string EscalateTimeAttributeName = "SiteMailboxSyncSuccessTimeToEscalate";

		private const string SiteMailboxDocSyncPercentMonitorName = "SiteMailboxDocumentSyncPercentSuccessMonitor";

		private const string SiteMailboxDocSyncEscalateResponderName = "SiteMailboxDocumentSyncEscalate";
	}
}
