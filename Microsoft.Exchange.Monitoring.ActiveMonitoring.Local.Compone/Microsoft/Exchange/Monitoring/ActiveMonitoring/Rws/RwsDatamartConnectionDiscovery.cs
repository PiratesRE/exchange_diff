using System;
using System.Reflection;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Rws.Probes;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Rws
{
	public sealed class RwsDatamartConnectionDiscovery : MaintenanceWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			WTFDiagnostics.TraceInformation(ExTraceGlobals.RWSTracer, base.TraceContext, "RwsDatamartConnectionDiscovery.DoWork: enter", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Rws\\RwsDatamartConnectionDiscovery.cs", 99);
			LocalEndpointManager instance = LocalEndpointManager.Instance;
			if (!LocalEndpointManager.IsDataCenter)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.RWSTracer, base.TraceContext, "Skip RWS monitoring on non datacenter environment", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Rws\\RwsDatamartConnectionDiscovery.cs", 106);
				return;
			}
			if (instance.MailboxDatabaseEndpoint == null || instance.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForBackend.Count == 0)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.RWSTracer, base.TraceContext, "RwsDatamartConnectionDiscovery.DoWork: no mailbox database found on this server", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Rws\\RwsDatamartConnectionDiscovery.cs", 112);
				return;
			}
			ProbeDefinition definition = this.CreateProbe(RwsDatamartConnectionDiscovery.ProbeName, RwsDatamartConnectionDiscovery.RwsDatamartConnectionProbeTypeName, RwsDatamartConnectionDiscovery.ProbeRecurrenceIntervalSeconds, RwsDatamartConnectionDiscovery.ProbeTimeoutSeconds, RwsDatamartConnectionDiscovery.ProbeRetryTimes);
			base.Broker.AddWorkDefinition<ProbeDefinition>(definition, base.TraceContext);
			MonitorDefinition monitorDefinition = this.CreateMonitor(RwsDatamartConnectionDiscovery.MonitorName, RwsDatamartConnectionDiscovery.ProbeName);
			monitorDefinition.ServicePriority = 2;
			monitorDefinition.ScenarioDescription = "Validate RWS health is not impacted by Datamart connectivity issues";
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
			ResponderDefinition definition2 = this.CreateResponder(RwsDatamartConnectionDiscovery.ResponderName, RwsDatamartConnectionDiscovery.AssemblyPath, RwsDatamartConnectionDiscovery.EscalateResponderTypeName, RwsDatamartConnectionDiscovery.AlertTypeId, monitorDefinition.Name);
			base.Broker.AddWorkDefinition<ResponderDefinition>(definition2, base.TraceContext);
		}

		private ProbeDefinition CreateProbe(string probeName, string probeTypeName, int recurrenceIntervalSeconds, int timeoutSeconds, int maxRetryAttempts)
		{
			WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.RWSTracer, base.TraceContext, "RwsDatamartConnectionDiscovery.CreateProbe: Creating probe {0}", probeName, null, "CreateProbe", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Rws\\RwsDatamartConnectionDiscovery.cs", 156);
			ProbeDefinition probeDefinition = new ProbeDefinition();
			probeDefinition.AssemblyPath = RwsDatamartConnectionDiscovery.AssemblyPath;
			probeDefinition.TypeName = RwsDatamartConnectionDiscovery.RwsDatamartConnectionProbeTypeName;
			probeDefinition.Name = probeName;
			probeDefinition.ServiceName = ExchangeComponent.Rws.Service;
			probeDefinition.RecurrenceIntervalSeconds = recurrenceIntervalSeconds;
			probeDefinition.TimeoutSeconds = timeoutSeconds;
			probeDefinition.MaxRetryAttempts = maxRetryAttempts;
			WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.RWSTracer, base.TraceContext, "RwsDatamartConnectionDiscovery.CreateProbe: Created probe {0}", probeName, null, "CreateProbe", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Rws\\RwsDatamartConnectionDiscovery.cs", 171);
			return probeDefinition;
		}

		private MonitorDefinition CreateMonitor(string monitorName, string sampleMask)
		{
			MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition(monitorName, sampleMask, ExchangeComponent.Rws.Service, ExchangeComponent.Rws, RwsDatamartConnectionDiscovery.FailureCountThreshold, true, RwsDatamartConnectionDiscovery.MonitorIntervalSeconds);
			monitorDefinition.TargetResource = Environment.MachineName;
			WTFDiagnostics.TraceInformation<string, string>(ExTraceGlobals.RWSTracer, base.TraceContext, "RwsDatamartConnectionDiscovery.CreateMonitor: Creating monitor {0} of type {1}", monitorName, RwsDatamartConnectionDiscovery.OverallConsecutiveProbeFailuresMonitorTypeName, null, "CreateMonitor", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Rws\\RwsDatamartConnectionDiscovery.cs", 201);
			return monitorDefinition;
		}

		private ResponderDefinition CreateResponder(string responderName, string assemblyPath, string responderTypeName, string alertTypeId, string alertMask)
		{
			ResponderDefinition result = OBDEscalateResponder.CreateDefinition(responderName, ExchangeComponent.Rws.Service, alertTypeId, alertMask, Environment.MachineName, ServiceHealthStatus.Unrecoverable, ExchangeComponent.Rws.EscalationTeam, Strings.RwsDatamartConnectionEscalationSubject(Environment.MachineName), Strings.RwsDatamartConnectionEscalationBody(Environment.MachineName), true, NotificationServiceClass.Urgent, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
			WTFDiagnostics.TraceInformation(ExTraceGlobals.RWSTracer, base.TraceContext, string.Format("RwsDatamartConnectionDiscovery.CreateResponder: Created responder {0} of type {1}", responderName, responderTypeName), null, "CreateResponder", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Rws\\RwsDatamartConnectionDiscovery.cs", 240);
			return result;
		}

		internal static readonly string ProbeName = "RwsDatamartConnectionProbe";

		internal static readonly string MonitorName = "RwsDatamartConnectionMonitor";

		internal static readonly string ResponderName = "RwsDatamartConnectionEscalateResponder";

		internal static readonly string AlertTypeId = "Exchange/Rws/DatamartProbe/Responder";

		internal static int FailureCountThreshold = 3;

		internal static int ProbeRecurrenceIntervalSeconds = 600;

		internal static int MonitorIntervalSeconds = 2400;

		internal static int ProbeTimeoutSeconds = 180;

		internal static int ProbeRetryTimes = 0;

		private static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		private static readonly string RwsDatamartConnectionProbeTypeName = typeof(RwsDatamartConnectionProbe).FullName;

		private static readonly string OverallConsecutiveProbeFailuresMonitorTypeName = typeof(OverallConsecutiveProbeFailuresMonitor).FullName;

		private static readonly string EscalateResponderTypeName = typeof(OBDEscalateResponder).FullName;
	}
}
