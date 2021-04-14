using System;
using System.Threading;
using System.Xml;
using Microsoft.Exchange.Diagnostics.Components.ForefrontActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring
{
	public sealed class DataminingDiscovery : MaintenanceWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			if (!FfoLocalEndpointManager.IsBackgroundRoleInstalled && !FfoLocalEndpointManager.IsHubTransportRoleInstalled && !FfoLocalEndpointManager.IsFrontendTransportRoleInstalled)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.DataminingTracer, DataminingDiscovery.traceContext, "[FFO DataminingDiscovery.DoWork]: None of the roles: Background, HubTransport and FrontendTransport is installed on this server.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\Datamining\\DataminingDiscovery.cs", 43);
				base.Result.StateAttribute1 = "FFO DataminingDiscovery: None of the roles: Background, HubTransport and FrontendTransport is installed on this server.";
				return;
			}
			try
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.DataminingTracer, DataminingDiscovery.traceContext, "[FFO DataminingDiscovery.DoWork]: Discovery Started.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\Datamining\\DataminingDiscovery.cs", 55);
				XmlNode definitionNode = GenericWorkItemHelper.GetDefinitionNode("FfoDatamining.xml", DataminingDiscovery.traceContext);
				GenericWorkItemHelper.CreatePerfCounterDefinitions(definitionNode, base.Broker, DataminingDiscovery.traceContext, base.Result);
				GenericWorkItemHelper.CreateNTEventDefinitions(definitionNode, base.Broker, DataminingDiscovery.traceContext, base.Result);
				GenericWorkItemHelper.CreateCustomDefinitions(definitionNode, base.Broker, DataminingDiscovery.traceContext, base.Result);
				GenericWorkItemHelper.CompleteDiscovery(DataminingDiscovery.traceContext);
				WTFDiagnostics.TraceInformation(ExTraceGlobals.DataminingTracer, DataminingDiscovery.traceContext, "[FFO DataminingDiscovery.DoWork]: Discovery Completed.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\Datamining\\DataminingDiscovery.cs", 75);
			}
			catch (Exception ex)
			{
				WTFDiagnostics.TraceError<string>(ExTraceGlobals.DataminingTracer, DataminingDiscovery.traceContext, "[FFO DataminingDiscovery.DoWork]: Exception occurred during discovery. {0}", ex.ToString(), null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\Datamining\\DataminingDiscovery.cs", 82);
				throw;
			}
		}

		private static TracingContext traceContext = new TracingContext();
	}
}
