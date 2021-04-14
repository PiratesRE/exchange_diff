using System;
using System.Threading;
using System.Xml;
using Microsoft.Exchange.Diagnostics.Components.ForefrontActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring
{
	public sealed class ProvisioningDiscovery : MaintenanceWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			if (!FfoLocalEndpointManager.IsWebServiceInstalled)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.ProvisioningTracer, ProvisioningDiscovery.traceContext, "[ProvisioningDiscovery.DoWork]: WebService role is not installed on this server.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\Provisioning\\ProvisioningDiscovery.cs", 42);
				base.Result.StateAttribute1 = "ProvisioningDiscovery: WebService role is not installed on this server.";
				return;
			}
			XmlNode definitionNode = GenericWorkItemHelper.GetDefinitionNode("FfoProvisioning.xml", ProvisioningDiscovery.traceContext);
			GenericWorkItemHelper.CreatePerfCounterDefinitions(definitionNode, base.Broker, ProvisioningDiscovery.traceContext, base.Result);
			GenericWorkItemHelper.CreateNTEventDefinitions(definitionNode, base.Broker, ProvisioningDiscovery.traceContext, base.Result);
			GenericWorkItemHelper.CreateCustomDefinitions(definitionNode, base.Broker, ProvisioningDiscovery.traceContext, base.Result);
			GenericWorkItemHelper.CompleteDiscovery(ProvisioningDiscovery.traceContext);
		}

		private static TracingContext traceContext = new TracingContext();
	}
}
