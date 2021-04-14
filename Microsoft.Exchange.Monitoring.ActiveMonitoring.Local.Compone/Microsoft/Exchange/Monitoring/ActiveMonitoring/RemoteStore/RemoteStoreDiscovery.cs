using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.RemoteStore.Monitors;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.RemoteStore.Responders;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.RemoteStore
{
	public sealed class RemoteStoreDiscovery : MaintenanceWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			try
			{
				LocalEndpointManager instance = LocalEndpointManager.Instance;
				if (instance.ExchangeServerRoleEndpoint == null || !instance.ExchangeServerRoleEndpoint.IsMailboxRoleInstalled)
				{
					WTFDiagnostics.TraceWarning(ExTraceGlobals.RemoteStoreTracer, base.TraceContext, "RemoteStoreDiscovery.DoWork: Mailbox role is not installed on this server, no need to create database space related work items", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RemoteStore\\RemoteStoreDiscovery.cs", 95);
				}
				else
				{
					RemoteStoreDiscovery.isDatacenter = LocalEndpointManager.IsDataCenter;
					this.CreateRemoteStoreAdminRPCInterfaceContext();
				}
			}
			catch (EndpointManagerEndpointUninitializedException)
			{
				WTFDiagnostics.TraceError(ExTraceGlobals.RemoteStoreTracer, base.TraceContext, "RemoteStoreDiscovery.DoWork: EndpointManagerEndpointUninitializedException is caught.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RemoteStore\\RemoteStoreDiscovery.cs", 115);
			}
		}

		private void CreateRemoteStoreAdminRPCInterfaceContext()
		{
			string sampleMask = NotificationItem.GenerateResultName(ExchangeComponent.RemoteStore.Name, "ListMDBStatusNotification", null);
			MonitorDefinition monitorDefinition = RemoteStoreAdminRPCInterfaceMonitor.CreateDefinition("RemoteStoreAdminRPCInterfaceMonitor", sampleMask, ExchangeComponent.RemoteStore.Name, ExchangeComponent.RemoteStore, "MSExchangeIS", 9, RemoteStoreDiscovery.remoteStoreAdminRPCInterfaceRecurrenceInterval, RemoteStoreDiscovery.remoteStoreAdminRPCInterfaceMonitoringInterval, true);
			monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Unhealthy, 0),
				new MonitorStateTransition(ServiceHealthStatus.Unrecoverable, RemoteStoreDiscovery.remoteStoreAdminRPCInterfaceEscalationInterval)
			};
			monitorDefinition.SecondaryMonitoringThreshold = 5.0;
			monitorDefinition.ServicePriority = 0;
			monitorDefinition.ScenarioDescription = "Validate Remote Store health is not impacted by any issues";
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
			ResponderDefinition responderDefinition = RemoteStoreAdminRPCInterfaceForceRebootResponder.CreateDefinition("RemoteStoreAdminRPCInterfaceKillServer", monitorDefinition.Name, monitorDefinition.ConstructWorkItemResultName(), TimeSpan.Zero, ServiceHealthStatus.Unhealthy, ExchangeComponent.RemoteStore.Name, ExchangeComponent.RemoteStore.Name, 1, true);
			responderDefinition.Attributes[typeof(TimeoutException).FullName] = bool.TrueString;
			base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition, base.TraceContext);
			TimeSpan duration = RemoteStoreDiscovery.remoteStoreAdminRPCInterfaceEscalationInterval + RemoteStoreDiscovery.remoteStoreAdminRPCInterfaceMonitoringInterval;
			string escalationMessage;
			if (RemoteStoreDiscovery.isDatacenter)
			{
				escalationMessage = Strings.RemoteStoreAdminRPCInterfaceEscalationEscalationMessageDc(duration);
			}
			else
			{
				escalationMessage = Strings.RemoteStoreAdminRPCInterfaceEscalationEscalationMessageEnt(duration);
			}
			ResponderDefinition responderDefinition2 = RemoteStoreAdminRPCInterfaceEscalateResponder.CreateDefinition("RemoteStoreAdminRPCInterfaceEscalate", ExchangeComponent.RemoteStore.Name, monitorDefinition.Name, monitorDefinition.ConstructWorkItemResultName(), "MSExchangeIS", ServiceHealthStatus.Unrecoverable, ExchangeComponent.RemoteStore.EscalationTeam, Strings.RemoteStoreAdminRPCInterfaceEscalationSubject(duration), escalationMessage, NotificationServiceClass.Scheduled, true, 0);
			responderDefinition2.Attributes[typeof(TimeoutException).FullName] = bool.TrueString;
			base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition2, base.TraceContext);
		}

		internal const string ListMDBStatusNotificationTriggerString = "ListMDBStatusNotification";

		internal const int AmPeriodicDatabaseAnalyzerIntervalInMinutes = 2;

		private const int RemoteStoreAdminRPCInterfaceMonitoringThreshold = 9;

		private const int CountOfSpecificExceptionTypeThreshold = 5;

		private const string StoreServiceName = "MSExchangeIS";

		private static TimeSpan remoteStoreAdminRPCInterfaceRecurrenceInterval = TimeSpan.FromMinutes(2.0);

		private static TimeSpan remoteStoreAdminRPCInterfaceEscalationInterval = TimeSpan.FromMinutes(20.0);

		private static TimeSpan remoteStoreAdminRPCInterfaceMonitoringInterval = new TimeSpan(TimeSpan.FromMinutes(2.0).Ticks * 9L + TimeSpan.FromMinutes(2.0).Ticks * 2L);

		private static bool isDatacenter;
	}
}
