using System;
using System.Threading;
using System.Xml;
using Microsoft.Exchange.Diagnostics.Components.ForefrontActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.WebService
{
	public sealed class SpamDigestWebServiceDiscovery : MaintenanceWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			if (!FfoLocalEndpointManager.IsWebServiceInstalled)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.WebServiceTracer, SpamDigestWebServiceDiscovery.traceContext, "[WebServiceDiscovery.DoWork]: FfoWebService role is not installed on this server.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\WebService\\SpamDigestWebServiceDiscovery.cs", 44);
				base.Result.StateAttribute1 = "WebServiceDiscovery: FfoWebService role is not installed on this server.";
				return;
			}
			XmlNode definitionNode = GenericWorkItemHelper.GetDefinitionNode("SpamDigestWebService.xml", SpamDigestWebServiceDiscovery.traceContext);
			GenericWorkItemHelper.CreatePerfCounterDefinitions(definitionNode, base.Broker, SpamDigestWebServiceDiscovery.traceContext, base.Result);
			GenericWorkItemHelper.CreateCustomDefinitions(definitionNode, base.Broker, SpamDigestWebServiceDiscovery.traceContext, base.Result);
			GenericWorkItemHelper.CompleteDiscovery(SpamDigestWebServiceDiscovery.traceContext);
		}

		private const string EscalationTeam = "FFO Web Service";

		private static TracingContext traceContext = new TracingContext();
	}
}
