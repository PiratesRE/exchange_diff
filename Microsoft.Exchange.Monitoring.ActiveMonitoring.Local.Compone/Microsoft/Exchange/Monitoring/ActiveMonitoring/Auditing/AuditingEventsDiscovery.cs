using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Monitors;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Auditing
{
	public sealed class AuditingEventsDiscovery : MaintenanceWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			WTFDiagnostics.TraceInformation(ExTraceGlobals.ProvisioningTracer, AuditingEventsDiscovery.traceContext, "AuditingEventsDiscovery:: DoWork(): Started Execution.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Auditing\\AuditingEventsDiscovery.cs", 73);
			LocalEndpointManager instance = LocalEndpointManager.Instance;
			this.CreateNotificationEventAlert(AuditingEventsDiscovery.SearchErrorEventName, this.SixtyMinutesInSeconds, this.SixtyMinutesInSeconds, 10, NotificationServiceClass.UrgentInTraining, false);
			this.CreateNotificationEventAlert(AuditingEventsDiscovery.AsyncSearchServiceletStartingEventName, this.SixtyMinutesInSeconds, this.SixtyMinutesInSeconds, 10, NotificationServiceClass.UrgentInTraining, false);
			this.CreateHeartbeatEventAlert(AuditingEventsDiscovery.HeartbeatEventName, this.ThirtyMinutesInSeconds, this.ThirtyMinutesInSeconds, NotificationServiceClass.UrgentInTraining, true);
			WTFDiagnostics.TraceInformation(ExTraceGlobals.ProvisioningTracer, AuditingEventsDiscovery.traceContext, "AuditingEventsDiscovery:: DoWork(): Completed Execution.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Auditing\\AuditingEventsDiscovery.cs", 83);
		}

		private void CreateNotificationEventAlert(string notification, int recurrenceIntervalSeconds, int monitoringIntervalSeconds, int numFailures, NotificationServiceClass urgency, bool roleFlag)
		{
			if (roleFlag && !LocalEndpointManager.IsDataCenter)
			{
				return;
			}
			if (roleFlag && !LocalEndpointManager.Instance.ExchangeServerRoleEndpoint.IsMailboxRoleInstalled)
			{
				return;
			}
			string sampleMask = NotificationItem.GenerateResultName(ExchangeComponent.Compliance.Name, notification, null);
			string text = notification + "Monitor";
			string name = notification + "Responder";
			MonitorDefinition monitorDefinition = OverallXFailuresMonitor.CreateDefinition(text, sampleMask, ExchangeComponent.Compliance.Name, ExchangeComponent.Compliance, monitoringIntervalSeconds, recurrenceIntervalSeconds, numFailures, true);
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, AuditingEventsDiscovery.traceContext);
			ResponderDefinition definition = EscalateResponder.CreateDefinition(name, ExchangeComponent.Compliance.Name, text, monitorDefinition.ConstructWorkItemResultName(), ExchangeComponent.Compliance.Name, ServiceHealthStatus.Unhealthy, ExchangeComponent.Auditing.EscalationTeam, Strings.AsyncAuditLogSearchEscalationSubject, Strings.AsyncAuditLogSearchEscalationMessage(Environment.MachineName, notification), true, urgency, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
			base.Broker.AddWorkDefinition<ResponderDefinition>(definition, AuditingEventsDiscovery.traceContext);
		}

		private void CreateHeartbeatEventAlert(string notification, int recurrenceIntervalSeconds, int monitoringIntervalSeconds, NotificationServiceClass urgency, bool roleFlag)
		{
			if (roleFlag && !LocalEndpointManager.IsDataCenter)
			{
				return;
			}
			if (roleFlag && !LocalEndpointManager.Instance.ExchangeServerRoleEndpoint.IsMailboxRoleInstalled)
			{
				return;
			}
			string notificationMask = NotificationItem.GenerateResultName(ExchangeComponent.Compliance.Name, notification, null);
			string text = notification + "HBMonitor";
			string name = notification + "HBResponder";
			MonitorDefinition monitorDefinition = NotificationHeartbeatMonitor.CreateDefinition(text, ExchangeComponent.Compliance.Name, ExchangeComponent.Compliance, notificationMask, recurrenceIntervalSeconds, monitoringIntervalSeconds, true);
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, AuditingEventsDiscovery.traceContext);
			ResponderDefinition definition = EscalateResponder.CreateDefinition(name, ExchangeComponent.Compliance.Name, text, monitorDefinition.ConstructWorkItemResultName(), ExchangeComponent.Compliance.Name, ServiceHealthStatus.Unhealthy, ExchangeComponent.Auditing.EscalationTeam, Strings.AsyncAuditLogSearchEscalationSubject, Strings.AsyncAuditLogSearchEscalationMessage(Environment.MachineName, notification), true, urgency, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
			base.Broker.AddWorkDefinition<ResponderDefinition>(definition, AuditingEventsDiscovery.traceContext);
		}

		private static TracingContext traceContext = new TracingContext();

		private static readonly Type OverallXFailuresMonitorType = typeof(OverallXFailuresMonitor);

		private static readonly string SearchErrorEventName = "AuditLogSearchCompletedWithErrors";

		private static readonly string HeartbeatEventName = "AsyncSearchServiceletRunning";

		private static readonly string AsyncSearchServiceletStartingEventName = "AsyncSearchServiceletStarting";

		private readonly int FiveMinutesInSeconds = (int)TimeSpan.FromMinutes(5.0).TotalSeconds;

		private readonly int ThirtyMinutesInSeconds = (int)TimeSpan.FromMinutes(30.0).TotalSeconds;

		private readonly int SixtyMinutesInSeconds = (int)TimeSpan.FromMinutes(60.0).TotalSeconds;
	}
}
