using System;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.TransportSync.Probes;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.TransportSync
{
	internal class DatabaseConsistency : IWorkItem
	{
		public void Initialize(MaintenanceDefinition discoveryDefinition, IMaintenanceWorkBroker broker, TracingContext traceContext)
		{
			string databaseName = discoveryDefinition.Attributes["DatabaseName"];
			this.InitializeProbe(discoveryDefinition, broker, traceContext, databaseName);
			this.InitializeMonitor(discoveryDefinition, broker, traceContext, databaseName);
			this.InitializeResponder(discoveryDefinition, broker, traceContext, databaseName);
		}

		private void InitializeProbe(MaintenanceDefinition discoveryDefinition, IMaintenanceWorkBroker broker, TracingContext traceContext, string databaseName)
		{
			Configurations configurations = Configurations.CreateFromWorkDefinition(discoveryDefinition);
			ProbeDefinition probeDefinition = WorkDefinitionHelper.CreateProbeDefinition("TransportSyncManager.DatabaseConsistency.Probe", typeof(DatabaseConsistencyProbe), databaseName, ExchangeComponent.MailboxMigration.Name, configurations.DatabaseConsistencyRecurrenceInterval, configurations.DatabaseConsistencyEnabled);
			probeDefinition.TargetGroup = configurations.Server.Name;
			broker.AddWorkDefinition<ProbeDefinition>(probeDefinition, traceContext);
		}

		private void InitializeMonitor(MaintenanceDefinition discoveryDefinition, IMaintenanceWorkBroker broker, TracingContext traceContext, string databaseName)
		{
			Configurations configurations = Configurations.CreateFromWorkDefinition(discoveryDefinition);
			MonitorDefinition monitorDefinition = OverallXFailuresMonitor.CreateDefinition("TransportSyncManager.DatabaseConsistency.Monitor", string.Format("{0}/{1}", "TransportSyncManager.DatabaseConsistency.Probe", databaseName), ExchangeComponent.MailboxMigration.Name, ExchangeComponent.MailboxMigration, (int)DatabaseConsistency.DatabaseConsistencyMonitoringInterval.TotalSeconds, (int)DatabaseConsistency.DatabaseConsistencyMonitoringRecurrenceInterval.TotalSeconds, configurations.DatabaseConsistencyMonitorThreshold, true);
			monitorDefinition.TargetResource = databaseName;
			monitorDefinition.Enabled = configurations.DatabaseConsistencyEnabled;
			monitorDefinition.ServicePriority = 2;
			monitorDefinition.ScenarioDescription = "Validate TransportSync health is not impacted by database consistency issues";
			broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, traceContext);
			monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Degraded, 0),
				new MonitorStateTransition(ServiceHealthStatus.Unrecoverable, (int)configurations.DatabaseConsistencyRecurrenceInterval.TotalSeconds * 3)
			};
		}

		private void InitializeResponder(MaintenanceDefinition discoveryDefinition, IMaintenanceWorkBroker broker, TracingContext traceContext, string databaseName)
		{
			Configurations configurations = Configurations.CreateFromWorkDefinition(discoveryDefinition);
			ResponderDefinition definition = EscalateResponder.CreateDefinition("TransportSyncManager.DatabaseConsistency.Escalate", ExchangeComponent.MailboxMigration.Name, "TransportSyncManager.DatabaseConsistency.Monitor", string.Format("{0}/{1}", "TransportSyncManager.DatabaseConsistency.Monitor", databaseName), databaseName, ServiceHealthStatus.Unrecoverable, ExchangeComponent.MailboxMigration.EscalationTeam, Strings.EscalationSubjectUnhealthy, Strings.DatabaseConsistencyEscalationMessage(databaseName), configurations.DatabaseConsistencyEnabled, NotificationServiceClass.UrgentInTraining, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
			broker.AddWorkDefinition<ResponderDefinition>(definition, traceContext);
		}

		private static readonly TimeSpan DatabaseConsistencyMonitoringInterval = TimeSpan.FromSeconds(3600.0);

		private static readonly TimeSpan DatabaseConsistencyMonitoringRecurrenceInterval = TimeSpan.FromSeconds(3600.0);
	}
}
