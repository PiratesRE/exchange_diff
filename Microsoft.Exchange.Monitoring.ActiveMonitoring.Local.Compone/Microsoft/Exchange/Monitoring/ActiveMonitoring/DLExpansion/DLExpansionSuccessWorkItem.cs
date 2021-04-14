using System;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.DLExpansion
{
	internal class DLExpansionSuccessWorkItem
	{
		public static MonitorDefinition CreateMonitorDefinition(MaintenanceDefinition discoveryDefinition)
		{
			bool enabled = bool.Parse(discoveryDefinition.Attributes["DLExpansionMonitorEnabled"]);
			double availabilityPercentage = double.Parse(discoveryDefinition.Attributes["DLExpansionMonitorThreshold"]);
			TimeSpan monitoringInterval = TimeSpan.Parse(discoveryDefinition.Attributes["DLExpansionMonitorInterval"]);
			int minimumErrorCount = int.Parse(discoveryDefinition.Attributes["DLExpansionMonitorMinErrorCount"]);
			TimeSpan zero = TimeSpan.Zero;
			TimeSpan timeSpan = TimeSpan.Parse(discoveryDefinition.Attributes["DLExpansionTimeToEscalate"]);
			string name = ExchangeComponent.DLExpansion.Name;
			string sampleMask = NotificationItem.GenerateResultName(name, "DLExpansionComponent", null);
			MonitorDefinition monitorDefinition = OverallPercentSuccessMonitor.CreateDefinition("DLExpansionPercentMonitor", sampleMask, ExchangeComponent.DLExpansion.Name, ExchangeComponent.DLExpansion, availabilityPercentage, monitoringInterval, minimumErrorCount, true);
			monitorDefinition.Enabled = enabled;
			monitorDefinition.TargetResource = monitorDefinition.ServiceName;
			monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Degraded, (int)zero.TotalSeconds),
				new MonitorStateTransition(ServiceHealthStatus.Unrecoverable, (int)timeSpan.TotalSeconds)
			};
			return monitorDefinition;
		}

		public static ResponderDefinition CreateEscalateResponderDefinition(MaintenanceDefinition discoveryDefinition)
		{
			string alertMask = string.Format("{0}/{1}", "DLExpansionPercentMonitor", ExchangeComponent.DLExpansion.Name);
			return EscalateResponder.CreateDefinition("DLExpansionEscalateResponder", ExchangeComponent.DLExpansion.Name, "DLExpansionPercentMonitor", alertMask, "DLExpansion", ServiceHealthStatus.Unrecoverable, ExchangeComponent.DLExpansion.EscalationTeam, Strings.DLExpansionEscalationSubject, Strings.DLExpansionEscalationMessage, true, NotificationServiceClass.Scheduled, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
		}

		private const string MonitorEnabledAttributeName = "DLExpansionMonitorEnabled";

		private const string MonitorThresholdAttributeName = "DLExpansionMonitorThreshold";

		private const string MonitorIntervalAttributeName = "DLExpansionMonitorInterval";

		private const string MonitorMinErrorCountAttributeName = "DLExpansionMonitorMinErrorCount";

		private const string EscalateTimeAttributeName = "DLExpansionTimeToEscalate";

		private const string MonitorName = "DLExpansionPercentMonitor";

		private const string ResponderName = "DLExpansionEscalateResponder";
	}
}
