using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.LogAnalyzer.Analyzers.CmdletInfraLog;
using Microsoft.Exchange.LogAnalyzer.Extensions.CmdletInfraLog;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Responders;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Rps
{
	public sealed class RpsEdsAlertDiscovery : MaintenanceWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			WTFDiagnostics.TraceFunction(ExTraceGlobals.RPSTracer, base.TraceContext, "Entering DoWork", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\EDS\\RpsEdsAlertDiscovery.cs", 44);
			List<CmdletInfraVirtualDirectory> list = new List<CmdletInfraVirtualDirectory>();
			list.Add(0);
			list.Add(1);
			if (!LocalEndpointManager.Instance.ExchangeServerRoleEndpoint.IsCafeRoleInstalled)
			{
				list.Add(2);
			}
			this.AddMonitorItemsForTimeoutAuthzRequestEDSAlert(list);
			this.AddMonitorItemsForUnhandledCmdletExceptionEDSAlert(list);
			this.AddMonitorItemsForAuthzErrorEDSAlert(list);
			this.AddMonitorItemsForLongLatencyCmdletEDSAlert(list);
			this.AddMonitoringItemsForHttpGenericErrorAlert(list);
			this.AddMonitoringItemsForHttpErrorResponseAlert(list);
			WTFDiagnostics.TraceFunction(ExTraceGlobals.RPSTracer, base.TraceContext, "Leaving DoWork", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\EDS\\RpsEdsAlertDiscovery.cs", 59);
		}

		private void AddMonitorItemsForTimeoutAuthzRequestEDSAlert(List<CmdletInfraVirtualDirectory> monitoringVdirs)
		{
			this.AddMonitorItemForEdsAlertWithRecycleAppPoolResponder(monitoringVdirs, CmdletInfraTimeoutAuthzRequestAnalyzer.TimeoutAuthzRequestAboveThresholdEventNameTemplate, "Time out authz requests were found at {0} app pool");
		}

		private void AddMonitorItemForEdsAlert(List<CmdletInfraVirtualDirectory> monitoringVdirs, string eventNameTemplate, string escalationMessageTempalte)
		{
			WTFDiagnostics.TraceFunction(ExTraceGlobals.RPSTracer, base.TraceContext, "Entering AddMonitorItemForEdsAlert: " + eventNameTemplate, null, "AddMonitorItemForEdsAlert", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\EDS\\RpsEdsAlertDiscovery.cs", 79);
			if (!LocalEndpointManager.Instance.ExchangeServerRoleEndpoint.IsMailboxRoleInstalled)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.RPSTracer, base.TraceContext, "Skip AddMonitorItemForEdsAlert, Mailbox role is not installed!", null, "AddMonitorItemForEdsAlert", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\EDS\\RpsEdsAlertDiscovery.cs", 82);
			}
			else
			{
				foreach (CmdletInfraVirtualDirectory cmdletInfraVirtualDirectory in monitoringVdirs)
				{
					string text = string.Format(eventNameTemplate, cmdletInfraVirtualDirectory);
					string text2 = text + "Monitor";
					string name = text + "Escalation";
					MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition(text2, string.Format("{0}/{1}", ExchangeComponent.Eds.Name, text), ExchangeComponent.Eds.Name, ExchangeComponent.Eds, 1, true, 300);
					monitorDefinition.MonitoringIntervalSeconds = 300;
					monitorDefinition.RecurrenceIntervalSeconds = 300;
					monitorDefinition.ServicePriority = 1;
					monitorDefinition.ScenarioDescription = "Validate RPS health is not impacted by any issues";
					base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
					ResponderDefinition responderDefinition = EscalateResponder.CreateDefinition(name, ExchangeComponent.Eds.Name, text2, text2, Environment.MachineName, ServiceHealthStatus.None, ExchangeComponent.Rps.EscalationTeam, string.Format(escalationMessageTempalte, this.GetAppPoolName(cmdletInfraVirtualDirectory)), RpsEdsAlertDiscovery.EscalationMessage, true, NotificationServiceClass.UrgentInTraining, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
					responderDefinition.RecurrenceIntervalSeconds = 300;
					responderDefinition.MinimumSecondsBetweenEscalates = 3600;
					base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition, base.TraceContext);
					WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.RPSTracer, base.TraceContext, "Monitor & Escalation responder were created for eds alert '{0}'", text, null, "AddMonitorItemForEdsAlert", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\EDS\\RpsEdsAlertDiscovery.cs", 119);
				}
			}
			WTFDiagnostics.TraceFunction(ExTraceGlobals.RPSTracer, base.TraceContext, "Leaving AddMonitorItemForEdsAlert: " + eventNameTemplate, null, "AddMonitorItemForEdsAlert", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\EDS\\RpsEdsAlertDiscovery.cs", 123);
		}

		private void AddMonitorItemsForUnhandledCmdletExceptionEDSAlert(List<CmdletInfraVirtualDirectory> monitoringVdirs)
		{
			this.AddMonitorItemForEdsAlert(monitoringVdirs, CmdletInfraUnhandledCmdletExceptionAnalyzer.UnhandledCmdletExceptionEventName, "Unhandled cmdlet exceptions were found at {0} app pool");
		}

		private void AddMonitorItemsForLongLatencyCmdletEDSAlert(List<CmdletInfraVirtualDirectory> monitoringVdirs)
		{
			this.AddMonitorItemForEdsAlertWithRecycleAppPoolResponder(monitoringVdirs, CmdletInfraCmdletLatencyAnalyzer.LongLatecyCmdletEventNameTemplate, "long latency cmdlets were found at {0} app pool");
		}

		private void AddMonitorItemForEdsAlertWithRecycleAppPoolResponder(List<CmdletInfraVirtualDirectory> monitoringVdirs, string eventNameTemplate, string escalationMessageTempalte)
		{
			WTFDiagnostics.TraceFunction(ExTraceGlobals.RPSTracer, base.TraceContext, "Entering AddMonitorItemForEdsAlertWithRecycleAppPoolResponder: " + eventNameTemplate, null, "AddMonitorItemForEdsAlertWithRecycleAppPoolResponder", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\EDS\\RpsEdsAlertDiscovery.cs", 152);
			if (!LocalEndpointManager.Instance.ExchangeServerRoleEndpoint.IsMailboxRoleInstalled)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.RPSTracer, base.TraceContext, string.Format("Skip AddMonitorItemForEdsAlertWithRecycleAppPoolResponder: {0}, Mailbox role is not installed!", eventNameTemplate), null, "AddMonitorItemForEdsAlertWithRecycleAppPoolResponder", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\EDS\\RpsEdsAlertDiscovery.cs", 155);
			}
			else
			{
				MonitorStateTransition[] monitorStateTransitions = new MonitorStateTransition[]
				{
					new MonitorStateTransition(ServiceHealthStatus.Degraded, 0),
					new MonitorStateTransition(ServiceHealthStatus.Unrecoverable, 3600)
				};
				foreach (CmdletInfraVirtualDirectory cmdletInfraVirtualDirectory in monitoringVdirs)
				{
					string text = string.Format(eventNameTemplate, cmdletInfraVirtualDirectory);
					string text2 = text + "Monitor";
					string responderName = text + "RecycleAppPool";
					string name = text + "Escalation";
					MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition(text2, string.Format("{0}/{1}", ExchangeComponent.Eds.Name, text), ExchangeComponent.Eds.Name, ExchangeComponent.Eds, 1, true, 300);
					monitorDefinition.MonitoringIntervalSeconds = 1200;
					monitorDefinition.RecurrenceIntervalSeconds = 300;
					monitorDefinition.MonitorStateTransitions = monitorStateTransitions;
					monitorDefinition.ServicePriority = 1;
					monitorDefinition.ScenarioDescription = "Validate RPS health is not impacted by apppool issues";
					base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
					ResponderDefinition responderDefinition = ResetIISAppPoolResponder.CreateDefinition(responderName, text2, this.GetAppPoolName(cmdletInfraVirtualDirectory), ServiceHealthStatus.Degraded, DumpMode.MiniDump, null, 15.0, 0, "Exchange", true, null);
					responderDefinition.ServiceName = ExchangeComponent.Eds.Name;
					base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition, base.TraceContext);
					ResponderDefinition responderDefinition2 = EscalateResponder.CreateDefinition(name, ExchangeComponent.Eds.Name, text2, text2, Environment.MachineName, ServiceHealthStatus.Unrecoverable, ExchangeComponent.Rps.EscalationTeam, string.Format(escalationMessageTempalte, this.GetAppPoolName(cmdletInfraVirtualDirectory)), RpsEdsAlertDiscovery.EscalationMessage, true, NotificationServiceClass.UrgentInTraining, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
					responderDefinition2.RecurrenceIntervalSeconds = 300;
					responderDefinition2.MinimumSecondsBetweenEscalates = 3600;
					base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition2, base.TraceContext);
					WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.RPSTracer, base.TraceContext, "Monitor & Escalation responder were created for eds alert '{0}'", text, null, "AddMonitorItemForEdsAlertWithRecycleAppPoolResponder", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\EDS\\RpsEdsAlertDiscovery.cs", 210);
				}
			}
			WTFDiagnostics.TraceFunction(ExTraceGlobals.RPSTracer, base.TraceContext, "Leaving AddMonitorItemForEdsAlertWithRecycleAppPoolResponder: " + eventNameTemplate, null, "AddMonitorItemForEdsAlertWithRecycleAppPoolResponder", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\EDS\\RpsEdsAlertDiscovery.cs", 214);
		}

		private void AddMonitorItemsForAuthzErrorEDSAlert(List<CmdletInfraVirtualDirectory> monitoringVdirs)
		{
			this.AddMonitorItemForEdsAlert(monitoringVdirs, CmdletInfraAuthzErrorAnalyzer.AuthzErrorAlertTemplate, "Authz errors were found at {0} app pool");
		}

		private void AddMonitoringItemsForHttpErrorResponseAlert(List<CmdletInfraVirtualDirectory> monitoringVdirs)
		{
			this.AddMonitorItemForEdsAlert(monitoringVdirs, CmdletInfraHttpErrorResponseAnalyzer.HttpErrorEventNameTemplate, "Http error responses were found at {0} app pool");
		}

		private void AddMonitoringItemsForHttpGenericErrorAlert(List<CmdletInfraVirtualDirectory> monitoringVdirs)
		{
			this.AddMonitorItemForEdsAlert(monitoringVdirs, CmdletInfraHttpGenericErrorAnalyzer.HttpErrorAlertTemplate, "Http generic errors were found at {0} app pool");
		}

		private string GetAppPoolName(CmdletInfraVirtualDirectory vdir)
		{
			string result = null;
			switch (vdir)
			{
			case 0:
				result = "MSExchangePowerShellAppPool";
				break;
			case 1:
				result = "MSExchangePowerShellLiveIDAppPool";
				break;
			case 2:
				result = "MSExchangePowerShellFrontEndAppPool";
				break;
			}
			return result;
		}

		internal static readonly string EscalationMessage = "\r\nMonitor Failure Count : {{Monitor.TotalValue}} <br/>\r\nMonitor Total Sample Count : {{Monitor.TotalSampleCount}} <br/>\r\nMonitor Total Failed Count : {{Monitor.TotalFailedCount}} <br/>\r\nMonitor First Alert Observed Time: {{Monitor.FirstAlertObservedTime}} <br/>\r\nLast Failed Probe Result Name : {{Probe.ResultName}} <br/>\r\nLast Failed Probe Error : {{Probe.Error}} <br/>\r\nFailure Context : {{Probe.FailureContext}} <br/>";
	}
}
