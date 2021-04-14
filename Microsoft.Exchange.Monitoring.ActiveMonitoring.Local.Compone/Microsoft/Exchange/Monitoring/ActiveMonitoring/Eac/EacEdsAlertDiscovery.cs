using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Responders;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Eac
{
	public sealed class EacEdsAlertDiscovery : MaintenanceWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			WTFDiagnostics.TraceFunction(ExTraceGlobals.ECPTracer, base.TraceContext, "Entering DoWork", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Eac\\Eds\\EacEdsAlertDiscovery.cs", 45);
			this.AddMonitorResponderItemsForEcpEventLogError();
			this.AddMonitorResponderItemsForEcpLandingDefaultPageError();
			WTFDiagnostics.TraceFunction(ExTraceGlobals.ECPTracer, base.TraceContext, "Leaving DoWork", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Eac\\Eds\\EacEdsAlertDiscovery.cs", 48);
		}

		private void AddMonitorResponderItemsForEcpEventLogError()
		{
			WTFDiagnostics.TraceFunction(ExTraceGlobals.ECPTracer, base.TraceContext, "Entering AddEcpEventLogErrorMonitorItems", null, "AddMonitorResponderItemsForEcpEventLogError", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Eac\\Eds\\EacEdsAlertDiscovery.cs", 56);
			if (!LocalEndpointManager.Instance.ExchangeServerRoleEndpoint.IsMailboxRoleInstalled)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.ECPTracer, base.TraceContext, "Skip AddEcpEventLogErrorMonitorItems, Mailbox role is not installed!", null, "AddMonitorResponderItemsForEcpEventLogError", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Eac\\Eds\\EacEdsAlertDiscovery.cs", 59);
			}
			else
			{
				string text = "Ecp_EventLog_HttpUnhandledExceptionReachedThreshold";
				string str = text.Replace('_', '.');
				string text2 = str + "Monitor";
				string responderName = str + "ResetEcpAppPoolResponder";
				string name = str + "EscalationResponder";
				MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition(text2, string.Format("{0}/{1}", ExchangeComponent.Eds.Name, text), ExchangeComponent.Ecp.Name, ExchangeComponent.Ecp, 1, true, 300);
				monitorDefinition.MonitoringIntervalSeconds = 300;
				monitorDefinition.RecurrenceIntervalSeconds = 300;
				monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
				{
					new MonitorStateTransition(ServiceHealthStatus.Degraded, 0),
					new MonitorStateTransition(ServiceHealthStatus.Unrecoverable, 1200)
				};
				monitorDefinition.ServicePriority = 2;
				monitorDefinition.ScenarioDescription = "Validate EAC health based on event log entries";
				base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
				ResponderDefinition responderDefinition = ResetIISAppPoolResponder.CreateDefinition(responderName, monitorDefinition.Name, "MSExchangeECPAppPool", ServiceHealthStatus.Degraded, DumpMode.None, null, 15.0, 0, "Exchange", true, null);
				responderDefinition.ServiceName = ExchangeComponent.Ecp.Name;
				responderDefinition.RecurrenceIntervalSeconds = 300;
				responderDefinition.MinimumSecondsBetweenEscalates = 3600;
				ResponderDefinition responderDefinition2 = EscalateResponder.CreateDefinition(name, ExchangeComponent.Ecp.Name, text2, text2, Environment.MachineName, ServiceHealthStatus.Unrecoverable, ExchangeComponent.Ecp.EscalationTeam, "Ecp EventLog HttpUnhandledException found and not recoverable", EacEdsAlertDiscovery.EscalationMessage, true, NotificationServiceClass.UrgentInTraining, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
				responderDefinition2.RecurrenceIntervalSeconds = 300;
				responderDefinition2.MinimumSecondsBetweenEscalates = 3600;
				base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition, base.TraceContext);
				base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition2, base.TraceContext);
				WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.ECPTracer, base.TraceContext, "Monitor & ResetAppPool/Escalation responder were created for eds alert '{0}'", text, null, "AddMonitorResponderItemsForEcpEventLogError", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Eac\\Eds\\EacEdsAlertDiscovery.cs", 121);
			}
			WTFDiagnostics.TraceFunction(ExTraceGlobals.ECPTracer, base.TraceContext, "Leaving AddEcpEventLogErrorMonitorItems", null, "AddMonitorResponderItemsForEcpEventLogError", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Eac\\Eds\\EacEdsAlertDiscovery.cs", 124);
		}

		private void AddMonitorResponderItemsForEcpLandingDefaultPageError()
		{
			WTFDiagnostics.TraceFunction(ExTraceGlobals.ECPTracer, base.TraceContext, "Entering AddEcpLandingDefaultPageErrorMonitorItems", null, "AddMonitorResponderItemsForEcpLandingDefaultPageError", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Eac\\Eds\\EacEdsAlertDiscovery.cs", 134);
			if (!LocalEndpointManager.Instance.ExchangeServerRoleEndpoint.IsMailboxRoleInstalled)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.ECPTracer, base.TraceContext, "Skip AddEcpLandingDefaultPageErrorMonitorItems, Mailbox role is not installed!", null, "AddMonitorResponderItemsForEcpLandingDefaultPageError", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Eac\\Eds\\EacEdsAlertDiscovery.cs", 137);
			}
			else
			{
				string text = "Ecp_EventLog_LandingDefaultPageErrorReachedThreshold";
				string str = text.Replace('_', '.');
				string text2 = str + "Monitor";
				string name = str + "Escalation";
				MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition(text2, string.Format("{0}/{1}", ExchangeComponent.Eds.Name, text), ExchangeComponent.Ecp.Name, ExchangeComponent.Ecp, 1, true, 300);
				monitorDefinition.MonitoringIntervalSeconds = 300;
				monitorDefinition.RecurrenceIntervalSeconds = 300;
				monitorDefinition.ServicePriority = 2;
				monitorDefinition.ScenarioDescription = "Validate EAC landing page health based on event log entries";
				base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
				ResponderDefinition responderDefinition = EscalateResponder.CreateDefinition(name, ExchangeComponent.Ecp.Name, text2, text2, Environment.MachineName, ServiceHealthStatus.Unhealthy, ExchangeComponent.Ecp.EscalationTeam, "Ecp EventLog LandingDefaultPageError found and unhealthy", EacEdsAlertDiscovery.EscalationMessage, true, NotificationServiceClass.UrgentInTraining, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
				responderDefinition.RecurrenceIntervalSeconds = 300;
				responderDefinition.MinimumSecondsBetweenEscalates = 3600;
				base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition, base.TraceContext);
				WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.ECPTracer, base.TraceContext, "Monitor & Escalation responder were created for eds alert '{0}'", text, null, "AddMonitorResponderItemsForEcpLandingDefaultPageError", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Eac\\Eds\\EacEdsAlertDiscovery.cs", 174);
			}
			WTFDiagnostics.TraceFunction(ExTraceGlobals.ECPTracer, base.TraceContext, "Leaving AddEcpLandingDefaultPageErrorMonitorItems", null, "AddMonitorResponderItemsForEcpLandingDefaultPageError", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Eac\\Eds\\EacEdsAlertDiscovery.cs", 177);
		}

		private const string EacAppPoolName = "MSExchangeECPAppPool";

		internal static readonly string EscalationMessage = "\r\nMonitor Failure Count : {{Monitor.TotalValue}} <br/>\r\nMonitor Total Sample Count : {{Monitor.TotalSampleCount}} <br/>\r\nMonitor Total Failed Count : {{Monitor.TotalFailedCount}} <br/>\r\nMonitor First Alert Observed Time: {{Monitor.FirstAlertObservedTime}} <br/>\r\nLast Failed Probe Result Name : {{Probe.ResultName}} <br/>\r\nLast Failed Probe Error : {{Probe.Error}} <br/>\r\nFailure Context : {{Probe.FailureContext}} <br/>";
	}
}
