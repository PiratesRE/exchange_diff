using System;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.TransportSync
{
	internal class DeltaSyncServiceEndpointsLoadFailed : IWorkItem
	{
		public void Initialize(MaintenanceDefinition discoveryDefinition, IMaintenanceWorkBroker broker, TracingContext traceContext)
		{
			this.InitializeMonitor(discoveryDefinition, broker, traceContext);
			this.InitializeResponder(discoveryDefinition, broker, traceContext);
		}

		private void InitializeMonitor(MaintenanceDefinition discoveryDefinition, IMaintenanceWorkBroker broker, TracingContext traceContext)
		{
			Configurations configurations = Configurations.CreateFromWorkDefinition(discoveryDefinition);
			MonitorDefinition monitorDefinition = OverallXFailuresMonitor.CreateDefinition("DeltaSync.ServiceEndpointsLoad.Failed.Monitor", NotificationItem.GenerateResultName(ExchangeComponent.MailboxMigration.Name, TransportSyncNotificationEvent.DeltaSyncServiceEndpointsLoadFailed.ToString(), null), ExchangeComponent.MailboxMigration.Name, ExchangeComponent.MailboxMigration, (int)DeltaSyncServiceEndpointsLoadFailed.DeltaSyncServiceEndpointsLoadFailedMonitoringInterval.TotalSeconds, (int)DeltaSyncServiceEndpointsLoadFailed.DeltaSyncServiceEndpointsLoadFailedRecurrenceInterval.TotalSeconds, 10, true);
			monitorDefinition.Enabled = configurations.DeltaSyncServiceEndpointsLoadFailedMonitorAndResponderEnabled;
			monitorDefinition.ServicePriority = 2;
			monitorDefinition.ScenarioDescription = "Valdiate TransportSync is not impacted by DeltaSync service endpoint failure issues";
			broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, traceContext);
		}

		private void InitializeResponder(MaintenanceDefinition discoveryDefinition, IMaintenanceWorkBroker broker, TracingContext traceContext)
		{
			Configurations configurations = Configurations.CreateFromWorkDefinition(discoveryDefinition);
			ResponderDefinition definition = EscalateResponder.CreateDefinition("DeltaSync.ServiceEndpointsLoad.Failed.Escalate", ExchangeComponent.MailboxMigration.Name, "DeltaSync.ServiceEndpointsLoad.Failed.Monitor", string.Format("{0}/{1}", "DeltaSync.ServiceEndpointsLoad.Failed.Monitor", ExchangeComponent.MailboxMigration.Name), Environment.MachineName, ServiceHealthStatus.None, ExchangeComponent.MailboxMigration.EscalationTeam, Strings.EscalationSubjectUnhealthy, Strings.DeltaSyncServiceEndpointsLoadFailedEscalationMessage, configurations.DeltaSyncServiceEndpointsLoadFailedMonitorAndResponderEnabled, NotificationServiceClass.UrgentInTraining, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
			broker.AddWorkDefinition<ResponderDefinition>(definition, traceContext);
		}

		private const int DeltaSyncServiceEndpointsLoadFailedMonitorThreshold = 10;

		private static readonly TimeSpan DeltaSyncServiceEndpointsLoadFailedMonitoringInterval = TimeSpan.FromSeconds(3600.0);

		private static readonly TimeSpan DeltaSyncServiceEndpointsLoadFailedRecurrenceInterval = TimeSpan.FromSeconds(3600.0);
	}
}
