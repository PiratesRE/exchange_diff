using System;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.TransportSync
{
	internal class RegistryAccessDenied : IWorkItem
	{
		public void Initialize(MaintenanceDefinition discoveryDefinition, IMaintenanceWorkBroker broker, TracingContext traceContext)
		{
			this.InitializeMonitor(discoveryDefinition, broker, traceContext);
			this.InitializeResponder(discoveryDefinition, broker, traceContext);
		}

		private void InitializeMonitor(MaintenanceDefinition discoveryDefinition, IMaintenanceWorkBroker broker, TracingContext traceContext)
		{
			Configurations configurations = Configurations.CreateFromWorkDefinition(discoveryDefinition);
			MonitorDefinition monitorDefinition = OverallXFailuresMonitor.CreateDefinition("Registry.AccessDenied.Monitor", NotificationItem.GenerateResultName(ExchangeComponent.MailboxMigration.Name, TransportSyncNotificationEvent.RegistryAccessDenied.ToString(), null), ExchangeComponent.MailboxMigration.Name, ExchangeComponent.MailboxMigration, (int)RegistryAccessDenied.RegistryAccessDeniedMonitoringInterval.TotalSeconds, (int)RegistryAccessDenied.RegistryAccessDeniedRecurrenceInterval.TotalSeconds, 10, true);
			monitorDefinition.Enabled = configurations.RegistryAccessDeniedMonitorAndResponderEnabled;
			monitorDefinition.ServicePriority = 2;
			monitorDefinition.ScenarioDescription = "Validate TransportSync health is not impacted by Registry access issues";
			broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, traceContext);
		}

		private void InitializeResponder(MaintenanceDefinition discoveryDefinition, IMaintenanceWorkBroker broker, TracingContext traceContext)
		{
			Configurations configurations = Configurations.CreateFromWorkDefinition(discoveryDefinition);
			ResponderDefinition definition = EscalateResponder.CreateDefinition("Registry.AccessDenied.Escalate", ExchangeComponent.MailboxMigration.Name, "Registry.AccessDenied.Monitor", string.Format("{0}/{1}", "Registry.AccessDenied.Monitor", ExchangeComponent.MailboxMigration.Name), Environment.MachineName, ServiceHealthStatus.None, ExchangeComponent.MailboxMigration.EscalationTeam, Strings.EscalationSubjectUnhealthy, Strings.RegistryAccessDeniedEscalationMessage, configurations.RegistryAccessDeniedMonitorAndResponderEnabled, NotificationServiceClass.UrgentInTraining, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
			broker.AddWorkDefinition<ResponderDefinition>(definition, traceContext);
		}

		private const int RegistryAccessDeniedMonitorThreshold = 10;

		private static readonly TimeSpan RegistryAccessDeniedMonitoringInterval = TimeSpan.FromSeconds(3600.0);

		private static readonly TimeSpan RegistryAccessDeniedRecurrenceInterval = TimeSpan.FromSeconds(3600.0);
	}
}
