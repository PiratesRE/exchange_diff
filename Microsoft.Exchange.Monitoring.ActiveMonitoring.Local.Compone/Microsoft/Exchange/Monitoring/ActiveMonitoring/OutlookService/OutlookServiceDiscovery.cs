using System;
using System.Reflection;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.OutlookService.Service;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.OutlookService.Probes;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Responders;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.OutlookService
{
	public class OutlookServiceDiscovery : MaintenanceWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			if (!LocalEndpointManager.IsDataCenter)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.FeaturesTracer, base.TraceContext, "OutlookServiceDiscovery::DoWork(): OutlookService doesn't run on-premises", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OutlookService\\OutlookServiceDiscovery.cs", 143);
				return;
			}
			try
			{
				if (OutlookServiceDiscovery.endpointManager.ExchangeServerRoleEndpoint == null)
				{
					WTFDiagnostics.TraceInformation(ExTraceGlobals.FeaturesTracer, base.TraceContext, "OutlookServiceDiscovery::DoWork(): Could not find ExchangeServerRoleEndpoint", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OutlookService\\OutlookServiceDiscovery.cs", 152);
					return;
				}
			}
			catch (Exception ex)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.FeaturesTracer, base.TraceContext, string.Format("OutlookServiceDiscovery:: DoWork(): ExchangeServerRoleEndpoint object threw exception.  Exception:{0}", ex.ToString()), null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OutlookService\\OutlookServiceDiscovery.cs", 158);
				return;
			}
			if (OutlookServiceDiscovery.endpointManager.ExchangeServerRoleEndpoint.IsCafeRoleInstalled)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.FeaturesTracer, base.TraceContext, "OutlookServiceDiscovery:: DoWork(): Found that CafeRole is installed", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OutlookService\\OutlookServiceDiscovery.cs", 164);
				try
				{
					if (OutlookServiceDiscovery.endpointManager.MailboxDatabaseEndpoint == null)
					{
						WTFDiagnostics.TraceInformation(ExTraceGlobals.FeaturesTracer, base.TraceContext, "OutlookServiceDiscovery:: DoWork(): Could not find MailboxDatabaseEndpoint", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OutlookService\\OutlookServiceDiscovery.cs", 170);
						return;
					}
				}
				catch (Exception ex2)
				{
					WTFDiagnostics.TraceInformation(ExTraceGlobals.FeaturesTracer, base.TraceContext, string.Format("OutlookServiceDiscovery:: DoWork(): MailboxDatabaseEndpoint object threw exception.  Exception:{0}", ex2.ToString()), null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OutlookService\\OutlookServiceDiscovery.cs", 176);
					return;
				}
				WTFDiagnostics.TraceInformation(ExTraceGlobals.FeaturesTracer, base.TraceContext, "OutlookServiceDiscovery:: DoWork(): Found Cafe Role", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OutlookService\\OutlookServiceDiscovery.cs", 180);
				WTFDiagnostics.TraceInformation(ExTraceGlobals.FeaturesTracer, base.TraceContext, "OutlookServiceDiscovery:: DoWork(): Creating Cafe/FrontEnd probe", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OutlookService\\OutlookServiceDiscovery.cs", 181);
				this.CreateFrontendSelfTestProbe(base.TraceContext);
			}
			if (OutlookServiceDiscovery.endpointManager.ExchangeServerRoleEndpoint.IsMailboxRoleInstalled)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.FeaturesTracer, base.TraceContext, "OutlookServiceDiscovery:: DoWork(): Found that MailboxRole is installed", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OutlookService\\OutlookServiceDiscovery.cs", 188);
				try
				{
					if (OutlookServiceDiscovery.endpointManager.MailboxDatabaseEndpoint == null)
					{
						WTFDiagnostics.TraceInformation(ExTraceGlobals.FeaturesTracer, base.TraceContext, "OutlookServiceDiscovery:: DoWork(): Could not find MailboxDatabaseEndpoint", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OutlookService\\OutlookServiceDiscovery.cs", 194);
						return;
					}
				}
				catch (Exception ex3)
				{
					WTFDiagnostics.TraceInformation(ExTraceGlobals.FeaturesTracer, base.TraceContext, string.Format("OutlookServiceDiscovery:: DoWork(): MailboxDatabaseEndpoint object threw exception.  Exception:{0}", ex3.ToString()), null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OutlookService\\OutlookServiceDiscovery.cs", 200);
					return;
				}
				WTFDiagnostics.TraceInformation(ExTraceGlobals.FeaturesTracer, base.TraceContext, "OutlookServiceDiscovery:: DoWork(): Found Mailbox Role", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OutlookService\\OutlookServiceDiscovery.cs", 204);
				WTFDiagnostics.TraceInformation(ExTraceGlobals.FeaturesTracer, base.TraceContext, "OutlookServiceDiscovery:: DoWork(): Creating BackEnd probe", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OutlookService\\OutlookServiceDiscovery.cs", 205);
				this.CreateBackendSelfTestProbe(base.TraceContext);
				this.CreateOutlookServiceLocalPingProbe(base.TraceContext);
				this.CreateOutlookServiceLocalSyncProbe(base.TraceContext);
			}
		}

		private void CreateOutlookServiceLocalSyncProbe(TracingContext traceContext)
		{
			WTFDiagnostics.TraceInformation(ExTraceGlobals.FeaturesTracer, base.TraceContext, "OutlookServiceDiscovery:: CreateOutlookServiceLocalSyncProbe(): Creating Local Sync probe", null, "CreateOutlookServiceLocalSyncProbe", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OutlookService\\OutlookServiceDiscovery.cs", 218);
			ProbeDefinition probeDefinition = OutlookServiceLocalSyncProbe.CreateDefinition(OutlookServiceDiscovery.AssemblyPath, OutlookServiceDiscovery.LocalSyncProbeName, "https://localhost:444/outlookservice");
			base.Broker.AddWorkDefinition<ProbeDefinition>(probeDefinition, base.TraceContext);
			WTFDiagnostics.TraceInformation(ExTraceGlobals.FeaturesTracer, base.TraceContext, "OutlookServiceDiscovery:: CreateOutlookServiceLocalSyncProbe(): Creating Local Sync Probe Monitor", null, "CreateOutlookServiceLocalSyncProbe", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OutlookService\\OutlookServiceDiscovery.cs", 223);
			MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition("LocalSyncProbe_ConsecutiveFailuresMonitor", probeDefinition.Name, ExchangeComponent.HxServiceMail.Name, ExchangeComponent.HxServiceMail, 4, true, OutlookServiceLocalSyncProbe.ProbeRecurrenceIntervalSeconds * 4);
			monitorDefinition.RecurrenceIntervalSeconds = 0;
			monitorDefinition.TargetResource = "MSExchangeOutlookServiceAppPool";
			monitorDefinition.IsHaImpacting = false;
			monitorDefinition.ServicePriority = 1;
			WTFDiagnostics.TraceInformation(ExTraceGlobals.FeaturesTracer, base.TraceContext, "OutlookServiceDiscovery:: CreateOutlookServiceLocalSyncProbe(): Creating Local Sync Probe responder chain", null, "CreateOutlookServiceLocalSyncProbe", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OutlookService\\OutlookServiceDiscovery.cs", 240);
			monitorDefinition.MonitorStateTransitions = this.CreateResponderChain(monitorDefinition.Name, "MSExchangeOutlookServiceAppPool");
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
		}

		private void CreateOutlookServiceLocalPingProbe(TracingContext traceContext)
		{
			WTFDiagnostics.TraceInformation(ExTraceGlobals.FeaturesTracer, base.TraceContext, "OutlookServiceDiscovery:: CreateOutlookServiceLocalPingProbe(): Creating Local Ping probe", null, "CreateOutlookServiceLocalPingProbe", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OutlookService\\OutlookServiceDiscovery.cs", 251);
			ProbeDefinition probeDefinition = OutlookServiceLocalPingProbe.CreateDefinition(OutlookServiceDiscovery.AssemblyPath, OutlookServiceDiscovery.LocalPingProbeName, "https://localhost:444/outlookservice");
			base.Broker.AddWorkDefinition<ProbeDefinition>(probeDefinition, base.TraceContext);
			WTFDiagnostics.TraceInformation(ExTraceGlobals.FeaturesTracer, base.TraceContext, "OutlookServiceDiscovery:: CreateOutlookServiceLocalPingProbe(): Creating Local Ping Probe Monitor", null, "CreateOutlookServiceLocalPingProbe", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OutlookService\\OutlookServiceDiscovery.cs", 256);
			MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition("LocalPingProbe_ConsecutiveFailuresMonitor", probeDefinition.Name, ExchangeComponent.HxServiceMail.Name, ExchangeComponent.HxServiceMail, 4, true, OutlookServiceLocalPingProbe.PingProbeRecurrenceIntervalSeconds * 4);
			monitorDefinition.RecurrenceIntervalSeconds = 60;
			monitorDefinition.TargetResource = "MSExchangeOutlookServiceAppPool";
			monitorDefinition.IsHaImpacting = false;
			monitorDefinition.ServicePriority = 1;
			WTFDiagnostics.TraceInformation(ExTraceGlobals.FeaturesTracer, base.TraceContext, "OutlookServiceDiscovery:: CreateOutlookServiceLocalPingProbe(): Creating Local Ping Probe responder chain", null, "CreateOutlookServiceLocalPingProbe", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OutlookService\\OutlookServiceDiscovery.cs", 273);
			monitorDefinition.MonitorStateTransitions = this.CreateResponderChain(monitorDefinition.Name, "MSExchangeOutlookServiceAppPool");
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
		}

		private void CreateBackendSelfTestProbe(TracingContext traceContext)
		{
			WTFDiagnostics.TraceInformation(ExTraceGlobals.FeaturesTracer, base.TraceContext, "OutlookServiceDiscovery:: CreateBackendSelfTestProbe(): Creating Backend Selftest probe", null, "CreateBackendSelfTestProbe", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OutlookService\\OutlookServiceDiscovery.cs", 285);
			ProbeDefinition probeDefinition = OutlookServiceSelfTestProbe.CreateDefinition(OutlookServiceDiscovery.AssemblyPath, OutlookServiceDiscovery.BackendSelfTestProbeName, "https://localhost:444/outlookservice/exhealth.check");
			base.Broker.AddWorkDefinition<ProbeDefinition>(probeDefinition, base.TraceContext);
			WTFDiagnostics.TraceInformation(ExTraceGlobals.FeaturesTracer, base.TraceContext, "OutlookServiceDiscovery:: CreateBackendSelfTestProbe(): Creating Backend Selftest Monitor", null, "CreateBackendSelfTestProbe", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OutlookService\\OutlookServiceDiscovery.cs", 290);
			MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition("ConsecutiveFailuresMonitor", probeDefinition.Name, ExchangeComponent.HxServiceMail.Name, ExchangeComponent.HxServiceMail, 4, true, OutlookServiceSelfTestProbe.PingProbeRecurrenceInterval * 4);
			monitorDefinition.RecurrenceIntervalSeconds = 0;
			monitorDefinition.TargetResource = "MSExchangeOutlookServiceAppPool";
			monitorDefinition.IsHaImpacting = false;
			monitorDefinition.ServicePriority = 1;
			WTFDiagnostics.TraceInformation(ExTraceGlobals.FeaturesTracer, base.TraceContext, "OutlookServiceDiscovery:: CreateBackendSelfTestProbe(): Creating Backend Selftest responder chain", null, "CreateBackendSelfTestProbe", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OutlookService\\OutlookServiceDiscovery.cs", 307);
			monitorDefinition.MonitorStateTransitions = this.CreateResponderChain(monitorDefinition.Name, "MSExchangeOutlookServiceAppPool");
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
		}

		private void CreateFrontendSelfTestProbe(TracingContext traceContext)
		{
			WTFDiagnostics.TraceInformation(ExTraceGlobals.FeaturesTracer, base.TraceContext, "OutlookServiceDiscovery:: CreateFrontendSelfTestProbe(): Creating CAFE/Frontend Selftest probe", null, "CreateFrontendSelfTestProbe", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OutlookService\\OutlookServiceDiscovery.cs", 319);
			ProbeDefinition probeDefinition = OutlookServiceSelfTestProbe.CreateDefinition(OutlookServiceDiscovery.AssemblyPath, OutlookServiceDiscovery.FrontendSelfTestProbeName, "https://localhost:443/outlookservice/exhealth.check");
			base.Broker.AddWorkDefinition<ProbeDefinition>(probeDefinition, base.TraceContext);
			WTFDiagnostics.TraceInformation(ExTraceGlobals.FeaturesTracer, base.TraceContext, "OutlookServiceDiscovery:: CreateFrontendSelfTestProbe(): Creating CAFE/Frontend Selftest Monitor", null, "CreateFrontendSelfTestProbe", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OutlookService\\OutlookServiceDiscovery.cs", 324);
			MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition("Frontend_ConsecutiveFailuresMonitor", probeDefinition.Name, ExchangeComponent.HxServiceMail.Name, ExchangeComponent.HxServiceMail, 4, true, OutlookServiceSelfTestProbe.PingProbeRecurrenceInterval * 4);
			monitorDefinition.RecurrenceIntervalSeconds = 0;
			monitorDefinition.TargetResource = "MSExchangeOutlookServiceAppPool";
			monitorDefinition.IsHaImpacting = false;
			monitorDefinition.ServicePriority = 1;
			WTFDiagnostics.TraceInformation(ExTraceGlobals.FeaturesTracer, base.TraceContext, "OutlookServiceDiscovery:: CreateFrontendSelfTestProbe(): Creating CAFE/Frontend Selftest responder chain", null, "CreateFrontendSelfTestProbe", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OutlookService\\OutlookServiceDiscovery.cs", 341);
			monitorDefinition.MonitorStateTransitions = this.CreateResponderChain(monitorDefinition.Name, "MSExchangeOutlookServiceAppPool");
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
		}

		private MonitorStateTransition[] CreateResponderChain(string monitorName, string targetResource)
		{
			WTFDiagnostics.TraceInformation(ExTraceGlobals.FeaturesTracer, base.TraceContext, "OutlookServiceDiscovery:: CreateResponderChain(): Creating responder chain", null, "CreateResponderChain", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OutlookService\\OutlookServiceDiscovery.cs", 354);
			MonitorStateTransition[] result = new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Degraded1, 0),
				new MonitorStateTransition(ServiceHealthStatus.Unrecoverable, 1200)
			};
			WTFDiagnostics.TraceInformation(ExTraceGlobals.FeaturesTracer, base.TraceContext, "OutlookServiceDiscovery:: CreateResponderChain(): Creating the restartAppPool responder", null, "CreateResponderChain", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OutlookService\\OutlookServiceDiscovery.cs", 366);
			ResponderDefinition responderDefinition = ResetIISAppPoolResponder.CreateDefinition(string.Format(OutlookServiceDiscovery.restartServiceResponderName, monitorName), monitorName, "MSExchangeOutlookServiceAppPool", ServiceHealthStatus.Degraded1, DumpMode.FullDump, null, 25.0, 90, "Exchange", true, "Dag");
			responderDefinition.ServiceName = ExchangeComponent.HxServiceMail.Name;
			responderDefinition.TargetResource = targetResource;
			responderDefinition.RecurrenceIntervalSeconds = 0;
			base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition, base.TraceContext);
			WTFDiagnostics.TraceInformation(ExTraceGlobals.FeaturesTracer, base.TraceContext, "OutlookServiceDiscovery:: CreateResponderChain(): Creating the escalate responder for the unrecoverable state", null, "CreateResponderChain", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OutlookService\\OutlookServiceDiscovery.cs", 386);
			ResponderDefinition responderDefinition2 = ExtraDetailsAlertResponder.CreateDefinition(string.Format(OutlookServiceDiscovery.escalateResponderName, monitorName), ExchangeComponent.HxServiceMail.Name, monitorName, monitorName, targetResource, ServiceHealthStatus.Unrecoverable, ExchangeComponent.HxServiceMail.EscalationTeam, OutlookServiceDiscovery.EscalateMailSubject, OutlookServiceDiscovery.EscalateMailBody, OutlookServiceSelfTestProbe.PingProbeRecurrenceInterval, "OutlookServiceRequest", "MSExchangeOutlookServiceAppPool", true, NotificationServiceClass.Scheduled, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59");
			responderDefinition2.RecurrenceIntervalSeconds = 0;
			base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition2, base.TraceContext);
			return result;
		}

		private const string ServiceAppPool = "MSExchangeOutlookServiceAppPool";

		protected const string BackendEndpoint = "https://localhost:444/outlookservice/exhealth.check";

		protected const string FrontendEndpoint = "https://localhost:443/outlookservice/exhealth.check";

		protected const string OutlookServiceEndpoint = "https://localhost:444/outlookservice";

		private const int MaxRetryAttempts = 3;

		private const int ConsecutiveFailureThreshold = 4;

		private const double SuccessPercentThreshold = 60.0;

		private const int UnrecoverableStateSeconds = 1200;

		public static readonly int SelfTestProbeRecurrenceIntervalSeconds = 60;

		public static readonly int SelfTestProbeTimeoutSeconds = OutlookServiceDiscovery.SelfTestProbeRecurrenceIntervalSeconds - 2;

		public static readonly string BackendSelfTestProbeName = "OutlookServiceSelfTestProbe";

		public static readonly string FrontendSelfTestProbeName = "OutlookService_FrontEnd_SelfTestProbe";

		public static readonly string LocalPingProbeName = "OutlookServiceLocalPingProbe";

		public static readonly string LocalSyncProbeName = "OutlookServiceLocalSyncProbe";

		internal static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		private static LocalEndpointManager endpointManager = LocalEndpointManager.Instance;

		private static string EscalateMailSubject = string.Format(Strings.HxServiceEscalationMessageUnhealthy, ExchangeComponent.HxServiceMail.Name, ExchangeComponent.HxServiceMail.Name);

		private static string EscalateMailBody = Strings.HxServiceEscalationMessageUnhealthy;

		private static string restartServiceResponderName = "{0}_OutlookServiceRestartWebAppPool";

		private static string escalateResponderName = "{0}_OutlookServiceEscalate";
	}
}
