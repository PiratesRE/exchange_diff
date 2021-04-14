using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Journaling
{
	internal class JournalingWorkItem
	{
		public static MonitorDefinition CreateMonitorDefinition(MaintenanceDefinition discoveryDefinition)
		{
			bool enabled = bool.Parse(discoveryDefinition.Attributes["JournalingMonitorEnabled"]);
			int monitoringInterval = (int)TimeSpan.Parse(discoveryDefinition.Attributes["JournalingMonitorInterval"]).TotalSeconds;
			int recurrenceInterval = (int)TimeSpan.Parse(discoveryDefinition.Attributes["JournalingMonitorRecurranceInterval"]).TotalSeconds;
			if (ExEnvironment.IsTest)
			{
				recurrenceInterval = (int)TimeSpan.FromSeconds(60.0).TotalSeconds;
			}
			int numberOfFailures = int.Parse(discoveryDefinition.Attributes["JournalingMonitorMinErrorCount"]);
			TimeSpan zero = TimeSpan.Zero;
			TimeSpan timeSpan = TimeSpan.Parse(discoveryDefinition.Attributes["JournalingTimeToEscalate"]);
			string name = ExchangeComponent.Compliance.Name;
			string sampleMask = NotificationItem.GenerateResultName(name, "JournalAgent", null);
			MonitorDefinition monitorDefinition = OverallXFailuresMonitor.CreateDefinition("JournanlingMonitor", sampleMask, ExchangeComponent.Compliance.Name, ExchangeComponent.Compliance, monitoringInterval, recurrenceInterval, numberOfFailures, enabled);
			monitorDefinition.TargetResource = monitorDefinition.ServiceName;
			monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Unrecoverable, (int)timeSpan.TotalSeconds)
			};
			return monitorDefinition;
		}

		public static ResponderDefinition CreateEscalateResponderDefinition(MaintenanceDefinition discoveryDefinition)
		{
			string alertMask = string.Format("{0}/{1}", "JournanlingMonitor", ExchangeComponent.Compliance.Name);
			ResponderDefinition responderDefinition = EscalateResponder.CreateDefinition("JournalingEscalateResponder", ExchangeComponent.Compliance.Name, "JournanlingMonitor", alertMask, "Compliance", ServiceHealthStatus.Unrecoverable, ExchangeComponent.Compliance.EscalationTeam, Strings.JournalingEscalationSubject, Strings.JournalingEscalationMessage, true, NotificationServiceClass.Urgent, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
			responderDefinition.Enabled = true;
			responderDefinition.NotificationServiceClass = NotificationServiceClass.UrgentInTraining;
			return responderDefinition;
		}

		private const string MonitorEnabledAttributeName = "JournalingMonitorEnabled";

		private const string MonitorIntervalAttributeName = "JournalingMonitorInterval";

		private const string MonitorRecurranceIntervalAttributeName = "JournalingMonitorRecurranceInterval";

		private const string MonitorMinErrorCountAttributeName = "JournalingMonitorMinErrorCount";

		private const string EscalateTimeAttributeName = "JournalingTimeToEscalate";

		private const string MonitorName = "JournanlingMonitor";

		private const string ResponderName = "JournalingEscalateResponder";
	}
}
