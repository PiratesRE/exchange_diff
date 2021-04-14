using System;
using System.Threading;
using System.Xml;
using Microsoft.Exchange.Diagnostics.Components.FfoSelfRecoveryFx;
using Microsoft.Exchange.Monitoring.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.RaaService
{
	public sealed class RaaServiceDiscovery : MaintenanceWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			if (!FfoLocalEndpointManager.IsCentralAdminRoleInstalled)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.RAAServiceTracer, RaaServiceDiscovery.traceContext, "[RaaServiceDiscovery.DoWork]: RaaService role is not installed on this server.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\RaaService\\RaaServiceDiscovery.cs", 38);
				base.Result.StateAttribute1 = "RaaServiceDiscovery: RaaService role is not installed on this server.";
				return;
			}
			XmlNode definitionNode = GenericWorkItemHelper.GetDefinitionNode("RaaService.xml", RaaServiceDiscovery.traceContext);
			GenericWorkItemHelper.CreateCustomDefinitions(definitionNode, base.Broker, RaaServiceDiscovery.traceContext, base.Result);
			GenericWorkItemHelper.CompleteDiscovery(RaaServiceDiscovery.traceContext);
			WTFDiagnostics.TraceInformation(ExTraceGlobals.RAAServiceTracer, RaaServiceDiscovery.traceContext, "[RaaServiceDiscovery.DoWork]: work item definitions created", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\RaaService\\RaaServiceDiscovery.cs", 57);
		}

		private static TracingContext traceContext = new TracingContext();
	}
}
