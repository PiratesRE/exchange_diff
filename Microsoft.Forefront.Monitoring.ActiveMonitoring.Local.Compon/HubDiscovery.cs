using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring
{
	public sealed class HubDiscovery : MaintenanceWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			WTFDiagnostics.TraceInformation(ExTraceGlobals.TransportTracer, base.TraceContext, "HubTransportDiscovery.DoWork", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\Transport\\Discovery\\HubDiscovery.cs", 45);
			if (!DiscoveryUtils.IsHubTransportRoleInstalled())
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.TransportTracer, base.TraceContext, "HubTransportDiscovery.DoWork(): Hub role not installed. Skip.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\Transport\\Discovery\\HubDiscovery.cs", 49);
				base.Result.StateAttribute1 = "HubTransportDiscovery: Hub role not installed. Skip.";
				return;
			}
			GenericWorkItemHelper.CreateAllDefinitions(new List<string>
			{
				"AgentLatency_Hub.xml",
				"Attribution_Hub.xml",
				"Core_Hub.xml",
				"DlpEtr_Hub.xml",
				"E4E_Hub.xml",
				"IRM_Hub.xml",
				"Latency_Hub.xml",
				"Quarantine_Hub.xml",
				"SmtpProbes_Hub.xml",
				"TransportHA_Hub.xml",
				"HubTransport_Agents_ControlPoint.xml",
				"HubTransport_Categorizer.xml",
				"HubTransport_OfflineRMS.xml",
				"Common_BridgeheadEdge_Transport.xml",
				"Common_BridgeheadEdge_TransportLogSearch.xml",
				"Queuing_Hub.xml"
			}, base.Broker, base.TraceContext, base.Result);
		}

		private const string ServiceName = "Transport";
	}
}
