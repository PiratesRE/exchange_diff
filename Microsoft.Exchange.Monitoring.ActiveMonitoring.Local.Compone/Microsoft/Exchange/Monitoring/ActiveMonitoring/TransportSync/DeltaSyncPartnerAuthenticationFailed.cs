using System;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.TransportSync
{
	internal class DeltaSyncPartnerAuthenticationFailed : IWorkItem
	{
		public void Initialize(MaintenanceDefinition discoveryDefinition, IMaintenanceWorkBroker broker, TracingContext traceContext)
		{
			this.InitializeMonitor(discoveryDefinition, broker, traceContext);
			this.InitializeResponder(discoveryDefinition, broker, traceContext);
		}

		private void InitializeMonitor(MaintenanceDefinition discoveryDefinition, IMaintenanceWorkBroker broker, TracingContext traceContext)
		{
			Configurations configurations = Configurations.CreateFromWorkDefinition(discoveryDefinition);
			MonitorDefinition monitorDefinition = OverallXFailuresMonitor.CreateDefinition("DeltaSync.PartnerAuthentication.Failed.Monitor", NotificationItem.GenerateResultName(ExchangeComponent.MailboxMigration.Name, TransportSyncNotificationEvent.DeltaSyncPartnerAuthenticationFailed.ToString(), null), ExchangeComponent.MailboxMigration.Name, ExchangeComponent.MailboxMigration, (int)DeltaSyncPartnerAuthenticationFailed.DeltaSyncPartnerAuthenticationFailedMonitoringInterval.TotalSeconds, (int)DeltaSyncPartnerAuthenticationFailed.DeltaSyncPartnerAuthenticationFailedRecurrenceInterval.TotalSeconds, 10, true);
			monitorDefinition.Enabled = configurations.DeltaSyncPartnerAuthenticationFailedMonitorAndResponderEnabled;
			monitorDefinition.ServicePriority = 2;
			monitorDefinition.ScenarioDescription = "Validate TransportSync is not impacted by DeltaSync partner authentication issues";
			broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, traceContext);
		}

		private void InitializeResponder(MaintenanceDefinition discoveryDefinition, IMaintenanceWorkBroker broker, TracingContext traceContext)
		{
			Configurations configurations = Configurations.CreateFromWorkDefinition(discoveryDefinition);
			ResponderDefinition definition = EscalateResponder.CreateDefinition("DeltaSync.PartnerAuthentication.Failed.Escalate", ExchangeComponent.MailboxMigration.Name, "DeltaSync.PartnerAuthentication.Failed.Monitor", string.Format("{0}/{1}", "DeltaSync.PartnerAuthentication.Failed.Monitor", ExchangeComponent.MailboxMigration.Name), Environment.MachineName, ServiceHealthStatus.None, ExchangeComponent.MailboxMigration.EscalationTeam, Strings.EscalationSubjectUnhealthy, Strings.DeltaSyncPartnerAuthenticationFailedEscalationMessage, configurations.DeltaSyncPartnerAuthenticationFailedMonitorAndResponderEnabled, NotificationServiceClass.UrgentInTraining, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
			broker.AddWorkDefinition<ResponderDefinition>(definition, traceContext);
		}

		private const int DeltaSyncPartnerAuthenticationFailedMonitorThreshold = 10;

		private static readonly TimeSpan DeltaSyncPartnerAuthenticationFailedMonitoringInterval = TimeSpan.FromSeconds(3600.0);

		private static readonly TimeSpan DeltaSyncPartnerAuthenticationFailedRecurrenceInterval = TimeSpan.FromSeconds(3600.0);
	}
}
