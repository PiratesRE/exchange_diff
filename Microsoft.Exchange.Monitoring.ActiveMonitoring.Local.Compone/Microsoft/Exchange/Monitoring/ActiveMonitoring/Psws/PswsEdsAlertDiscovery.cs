using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.LogAnalyzer.Analyzers.CmdletInfraLog;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Responders;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Psws
{
	public sealed class PswsEdsAlertDiscovery : MaintenanceWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			WTFDiagnostics.TraceFunction(ExTraceGlobals.PswsTracer, base.TraceContext, "Entering DoWork", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PSWS\\EDS\\PswsEdsAlertDiscovery.cs", 44);
			this.AddMonitorItemsForUnhandledCmdletExceptionEDSAlert();
			this.AddMonitorItemsForLongLatencyCmdletEDSAlert();
			WTFDiagnostics.TraceFunction(ExTraceGlobals.PswsTracer, base.TraceContext, "Leaving DoWork", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PSWS\\EDS\\PswsEdsAlertDiscovery.cs", 47);
		}

		private void AddMonitorItemsForUnhandledCmdletExceptionEDSAlert()
		{
			WTFDiagnostics.TraceFunction(ExTraceGlobals.PswsTracer, base.TraceContext, "Entering AddMonitorItemsForUnhandledCmdletExceptionEDSAlert", null, "AddMonitorItemsForUnhandledCmdletExceptionEDSAlert", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PSWS\\EDS\\PswsEdsAlertDiscovery.cs", 55);
			if (!LocalEndpointManager.Instance.ExchangeServerRoleEndpoint.IsMailboxRoleInstalled)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.PswsTracer, base.TraceContext, "Skip AddMonitorItemsForUnhandledCmdletExceptionEDSAlert, Mailbox role is not installed!", null, "AddMonitorItemsForUnhandledCmdletExceptionEDSAlert", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PSWS\\EDS\\PswsEdsAlertDiscovery.cs", 58);
			}
			else
			{
				string text = string.Format(CmdletInfraUnhandledCmdletExceptionAnalyzer.UnhandledCmdletExceptionEventName, 3);
				string text2 = text + "Monitor";
				string name = text + "Escalation";
				MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition(text2, string.Format("{0}/{1}", ExchangeComponent.Eds.Name, text), ExchangeComponent.Eds.Name, ExchangeComponent.Eds, 1, true, 300);
				monitorDefinition.MonitoringIntervalSeconds = 300;
				monitorDefinition.RecurrenceIntervalSeconds = 300;
				monitorDefinition.ServicePriority = 1;
				monitorDefinition.ScenarioDescription = "Validate PSWS health is not impacted by any issues";
				base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
				ResponderDefinition responderDefinition = EscalateResponder.CreateDefinition(name, ExchangeComponent.Eds.Name, text2, text2, Environment.MachineName, ServiceHealthStatus.None, ExchangeComponent.Psws.EscalationTeam, "Unhandled cmdlet exceptions were found at MSExchangePswsAppPool app pool", PswsEdsAlertDiscovery.EscalationMessage, true, NotificationServiceClass.UrgentInTraining, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
				responderDefinition.RecurrenceIntervalSeconds = 300;
				responderDefinition.MinimumSecondsBetweenEscalates = 3600;
				base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition, base.TraceContext);
				WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.PswsTracer, base.TraceContext, "Monitor & Escalation responder were created for eds alert '{0}'", text, null, "AddMonitorItemsForUnhandledCmdletExceptionEDSAlert", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PSWS\\EDS\\PswsEdsAlertDiscovery.cs", 93);
			}
			WTFDiagnostics.TraceFunction(ExTraceGlobals.PswsTracer, base.TraceContext, "Leaving AddMonitorItemsForUnhandledCmdletExceptionEDSAlert", null, "AddMonitorItemsForUnhandledCmdletExceptionEDSAlert", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PSWS\\EDS\\PswsEdsAlertDiscovery.cs", 96);
		}

		private void AddMonitorItemsForLongLatencyCmdletEDSAlert()
		{
			WTFDiagnostics.TraceFunction(ExTraceGlobals.PswsTracer, base.TraceContext, "Entering AddMonitorItemsForLongLatencyCmdletEDSAlert", null, "AddMonitorItemsForLongLatencyCmdletEDSAlert", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PSWS\\EDS\\PswsEdsAlertDiscovery.cs", 104);
			if (!LocalEndpointManager.Instance.ExchangeServerRoleEndpoint.IsMailboxRoleInstalled)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.PswsTracer, base.TraceContext, "Skip AddMonitorItemsForLongLatencyCmdletEDSAlert, Mailbox role is not installed!", null, "AddMonitorItemsForLongLatencyCmdletEDSAlert", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PSWS\\EDS\\PswsEdsAlertDiscovery.cs", 107);
			}
			else
			{
				MonitorStateTransition[] monitorStateTransitions = new MonitorStateTransition[]
				{
					new MonitorStateTransition(ServiceHealthStatus.Degraded, 0),
					new MonitorStateTransition(ServiceHealthStatus.Unrecoverable, 3600)
				};
				string text = string.Format(CmdletInfraCmdletLatencyAnalyzer.LongLatecyCmdletEventNameTemplate, 3);
				string text2 = text + "Monitor";
				string responderName = text + "RecycleAppPool";
				string name = text + "Escalation";
				MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition(text2, string.Format("{0}/{1}", ExchangeComponent.Eds.Name, text), ExchangeComponent.Eds.Name, ExchangeComponent.Eds, 1, true, 300);
				monitorDefinition.MonitoringIntervalSeconds = 1800;
				monitorDefinition.RecurrenceIntervalSeconds = 300;
				monitorDefinition.MonitorStateTransitions = monitorStateTransitions;
				monitorDefinition.ServicePriority = 1;
				monitorDefinition.ScenarioDescription = "Validate PSWS health is not impacted by latency issues";
				base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
				ResponderDefinition responderDefinition = ResetIISAppPoolResponder.CreateDefinition(responderName, text2, "MSExchangePswsAppPool", ServiceHealthStatus.Degraded, DumpMode.MiniDump, null, 15.0, 0, "Exchange", true, null);
				responderDefinition.ServiceName = ExchangeComponent.Eds.Name;
				base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition, base.TraceContext);
				ResponderDefinition responderDefinition2 = EscalateResponder.CreateDefinition(name, ExchangeComponent.Eds.Name, text2, text2, Environment.MachineName, ServiceHealthStatus.Unrecoverable, ExchangeComponent.Psws.EscalationTeam, "long latency cmdlets were found at MSExchangePswsAppPool app pool", PswsEdsAlertDiscovery.EscalationMessage, true, NotificationServiceClass.UrgentInTraining, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
				responderDefinition2.RecurrenceIntervalSeconds = 300;
				responderDefinition2.MinimumSecondsBetweenEscalates = 3600;
				base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition2, base.TraceContext);
				WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.PswsTracer, base.TraceContext, "Monitor & Escalation responder were created for eds alert '{0}'", text, null, "AddMonitorItemsForLongLatencyCmdletEDSAlert", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PSWS\\EDS\\PswsEdsAlertDiscovery.cs", 161);
			}
			WTFDiagnostics.TraceFunction(ExTraceGlobals.PswsTracer, base.TraceContext, "Leaving AddMonitorItemsForLongLatencyCmdletEDSAlert", null, "AddMonitorItemsForLongLatencyCmdletEDSAlert", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PSWS\\EDS\\PswsEdsAlertDiscovery.cs", 164);
		}

		internal static readonly string EscalationMessage = "\r\nMonitor Failure Count : {{Monitor.TotalValue}} <br/>\r\nMonitor Total Sample Count : {{Monitor.TotalSampleCount}} <br/>\r\nMonitor Total Failed Count : {{Monitor.TotalFailedCount}} <br/>\r\nMonitor First Alert Observed Time: {{Monitor.FirstAlertObservedTime}} <br/>\r\nLast Failed Probe Result Name : {{Probe.ResultName}} <br/>\r\nLast Failed Probe Error : {{Probe.Error}} <br/>\r\nFailure Context : {{Probe.FailureContext}} <br/>";
	}
}
