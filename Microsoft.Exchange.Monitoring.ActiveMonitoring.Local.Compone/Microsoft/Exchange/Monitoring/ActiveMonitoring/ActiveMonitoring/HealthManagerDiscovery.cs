using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Monitors;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring
{
	public sealed class HealthManagerDiscovery : MaintenanceWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			WTFDiagnostics.TraceFunction(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "Start discovery for Health Manager.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HM\\HealthManagerDiscovery.cs", 101);
			if (!FfoLocalEndpointManager.IsForefrontForOfficeDatacenter)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "Adding heartbeat probe", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HM\\HealthManagerDiscovery.cs", 109);
				ProbeDefinition definition = TestActiveMonitoringProbe.CreateDefinition("HealthManagerHeartbeatProbe");
				base.Broker.AddWorkDefinition<ProbeDefinition>(definition, base.TraceContext);
				WTFDiagnostics.TraceInformation(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "Adding heartbeat monitor", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HM\\HealthManagerDiscovery.cs", 113);
				MonitorDefinition monitorDefinition = HeartbeatMonitor.CreateDefinition("HealthManagerHeartbeatMonitor", "HealthManagerHeartbeatProbe");
				monitorDefinition.ServicePriority = 0;
				monitorDefinition.ScenarioDescription = "Validate Monitoring health is not impacted by any issues";
				base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
				WTFDiagnostics.TraceInformation(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "Adding heartbeat responder", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HM\\HealthManagerDiscovery.cs", 119);
				ResponderDefinition definition2 = HeartbeatResponder.CreateDefinition("HealthManagerHeartbeatResponder", "HealthManagerHeartbeatMonitor");
				base.Broker.AddWorkDefinition<ResponderDefinition>(definition2, base.TraceContext);
			}
			WTFDiagnostics.TraceInformation(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "Adding healthstate collection monitor", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HM\\HealthManagerDiscovery.cs", 125);
			MonitorDefinition definition3 = HealthStateCollectionMonitor.CreateDefinition("ServerHealthStateCollectionMonitor");
			base.Broker.AddWorkDefinition<MonitorDefinition>(definition3, base.TraceContext);
			if (DirectoryAccessor.Instance.Server.IsMailboxServer)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "Adding MonitoringMailboxCleaner", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HM\\HealthManagerDiscovery.cs", 131);
				ProbeDefinition definition4 = MonitoringMailboxCleaner.CreateDefinition();
				base.Broker.AddWorkDefinition<ProbeDefinition>(definition4, base.TraceContext);
			}
			WTFDiagnostics.TraceInformation(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "Adding quarantine monitor", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HM\\HealthManagerDiscovery.cs", 138);
			MonitorDefinition monitorDefinition2 = QuarantineMonitor.CreateDefinition();
			monitorDefinition2.ServicePriority = 0;
			monitorDefinition2.ScenarioDescription = "Validate Monitoring health is not impacted by poisened workitems issues";
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition2, base.TraceContext);
			WTFDiagnostics.TraceInformation(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "Adding quarantine escalate responder", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HM\\HealthManagerDiscovery.cs", 145);
			ResponderDefinition responderDefinition = EscalateResponder.CreateDefinition("HealthManagerWorkItemQuarantineEscalate", ExchangeComponent.Monitoring.Name, monitorDefinition2.Name, monitorDefinition2.ConstructWorkItemResultName(), Environment.MachineName, ServiceHealthStatus.None, null, Strings.QuarantineEscalationSubject, Strings.QuarantineEscalationMessage, true, NotificationServiceClass.Scheduled, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
			responderDefinition.WaitIntervalSeconds = 90000;
			base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition, base.TraceContext);
			WTFDiagnostics.TraceInformation(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "Loading all maintenance definitions during running time", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HM\\HealthManagerDiscovery.cs", 168);
			IDataAccessQuery<MaintenanceDefinition> allDefinitions = LocalDataAccess.GetAllDefinitions<MaintenanceDefinition>();
			HashSet<string> hashSet = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
			WTFDiagnostics.TraceInformation(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "Loading component names from maintenance definitions", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HM\\HealthManagerDiscovery.cs", 172);
			foreach (MaintenanceDefinition maintenanceDefinition in allDefinitions)
			{
				if (!string.IsNullOrEmpty(maintenanceDefinition.ServiceName) && ExchangeComponent.WellKnownComponents.ContainsKey(maintenanceDefinition.ServiceName))
				{
					hashSet.Add(maintenanceDefinition.ServiceName);
				}
				else
				{
					hashSet.Add(ExchangeComponent.Monitoring.Name);
				}
			}
			WTFDiagnostics.TraceInformation(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, string.Format("Loaded {0} component names from {1} maintenance definitions", hashSet.Count, allDefinitions.Count<MaintenanceDefinition>()), null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HM\\HealthManagerDiscovery.cs", 186);
			foreach (string componentName in hashSet)
			{
				this.CreateMaintenanceMonitors(componentName);
			}
			MonitorDefinition monitorDefinition3 = OverallConsecutiveProbeFailuresMonitor.CreateDefinition("HealthManagerSchedulingLatencyMonitor", NotificationItem.GenerateResultName(ExchangeComponent.Eds.Name, "ActiveMonitoringSchedulingLatencyTrigger_Error", null), base.Definition.ServiceName, ExchangeComponent.Monitoring, 1, true, 600);
			monitorDefinition3.RecurrenceIntervalSeconds = 0;
			monitorDefinition3.ServicePriority = 0;
			monitorDefinition3.ScenarioDescription = "Validate Monitoring health is not impacted by latencies";
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition3, base.TraceContext);
			ResponderDefinition responderDefinition2 = EscalateResponder.CreateDefinition("HealthManagerSchedulingLatencyEscalate", base.Definition.ServiceName, "HealthManagerSchedulingLatencyMonitor", "HealthManagerSchedulingLatencyMonitor", string.Empty, ServiceHealthStatus.Unhealthy, ExchangeComponent.Monitoring.EscalationTeam, Strings.SchedulingLatencyEscalateResponderSubject, Strings.SchedulingLatencyEscalateResponderMessage, true, NotificationServiceClass.Urgent, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
			responderDefinition2.RecurrenceIntervalSeconds = 0;
			base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition2, base.TraceContext);
			LocalEndpointManager instance = LocalEndpointManager.Instance;
			if (instance.ExchangeServerRoleEndpoint != null && instance.ExchangeServerRoleEndpoint.IsCafeRoleInstalled)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "Adding KeepAliveMaintenance definition.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HM\\HealthManagerDiscovery.cs", 236);
				base.Broker.AddWorkDefinition<MaintenanceDefinition>(KeepAliveMaintenance.CreateDefinition(), base.TraceContext);
			}
			WTFDiagnostics.TraceFunction(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "Finished discovery for Health Manager.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HM\\HealthManagerDiscovery.cs", 241);
		}

		private void CreateMaintenanceMonitors(string componentName)
		{
			Component component = null;
			if (!ExchangeComponent.WellKnownComponents.TryGetValue(componentName, out component))
			{
				throw new InvalidOperationException(string.Format("The component '{0}' does not exist in Exchange Components.", componentName));
			}
			WTFDiagnostics.TraceInformation(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, string.Format("Adding {0} maintenance failure and timeout monitors", component.Name), null, "CreateMaintenanceMonitors", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HM\\HealthManagerDiscovery.cs", 258);
			MonitorDefinition monitorDefinition = MaintenanceFailureMonitor.CreateDefinition(component);
			monitorDefinition.ServicePriority = 0;
			monitorDefinition.ScenarioDescription = "Validate Monitoring health is not impacted by maintenance workitem failures";
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
			MonitorDefinition monitorDefinition2 = MaintenanceTimeoutMonitor.CreateDefinition(component);
			monitorDefinition2.ServicePriority = 0;
			monitorDefinition2.ScenarioDescription = "Validate Monitoring health is not impacted by maintenance workitem timeouts";
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition2, base.TraceContext);
			WTFDiagnostics.TraceInformation(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, string.Format("Adding {0} maintenance failure and timeout escalate responders", component.Name), null, "CreateMaintenanceMonitors", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HM\\HealthManagerDiscovery.cs", 271);
			ResponderDefinition responderDefinition = EscalateResponder.CreateDefinition(string.Format("{0}.{1}", "MaintenanceFailureEscalate", component.Name), component.Name, string.Format("{0}.{1}", "MaintenanceFailureMonitor", component.Name), monitorDefinition.ConstructWorkItemResultName(), Environment.MachineName, ServiceHealthStatus.None, null, Strings.MaintenanceFailureEscalationSubject, Strings.MaintenanceFailureEscalationMessage, true, NotificationServiceClass.Scheduled, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
			responderDefinition.RecurrenceIntervalSeconds = 0;
			responderDefinition.StartTime = DateTime.UtcNow.AddSeconds((double)(monitorDefinition.RecurrenceIntervalSeconds + 1));
			base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition, base.TraceContext);
			ResponderDefinition responderDefinition2 = EscalateResponder.CreateDefinition(string.Format("{0}.{1}", "MaintenanceTimeoutEscalate", component.Name), component.Name, string.Format("{0}.{1}", "MaintenanceTimeoutMonitor", component.Name), monitorDefinition2.ConstructWorkItemResultName(), Environment.MachineName, ServiceHealthStatus.None, null, Strings.MaintenanceTimeoutEscalationSubject, Strings.MaintenanceTimeoutEscalationMessage, true, NotificationServiceClass.Scheduled, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
			responderDefinition2.RecurrenceIntervalSeconds = 0;
			responderDefinition2.StartTime = DateTime.UtcNow.AddSeconds((double)(monitorDefinition2.RecurrenceIntervalSeconds + 1));
			base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition2, base.TraceContext);
		}

		internal const string HeartbeatProbeName = "HealthManagerHeartbeatProbe";

		internal const string HeartbeatMonitorName = "HealthManagerHeartbeatMonitor";

		internal const string HeartbeatResponderName = "HealthManagerHeartbeatResponder";

		internal const string SchedulingLatencyTriggerName = "ActiveMonitoringSchedulingLatencyTrigger_Error";

		internal const string SchedulingLatencyMonitorName = "HealthManagerSchedulingLatencyMonitor";

		internal const string SchedulingLatencyEscalateResponderName = "HealthManagerSchedulingLatencyEscalate";

		private const string QuarantineEscalateResponderName = "HealthManagerWorkItemQuarantineEscalate";

		private const string MaintenanceFailureEscalateResponderName = "MaintenanceFailureEscalate";

		private const string MaintenanceTimeoutEscalateResponderName = "MaintenanceTimeoutEscalate";

		private const string HealthStateCollectionMonitorName = "ServerHealthStateCollectionMonitor";

		private const string MonitoringScopeNotificationMonitorName = "MonitoringScopeNotificationMonitor";

		private const string HealthSetScopeNotificationMonitorName = "HealthSetScopeNotificationMonitor";

		private const string ScopeNotificationCollectionMonitorName = "ScopeNotificationCollectionMonitor";
	}
}
