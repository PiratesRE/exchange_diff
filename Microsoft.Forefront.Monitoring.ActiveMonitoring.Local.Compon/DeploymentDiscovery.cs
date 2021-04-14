using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ForefrontActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring
{
	public sealed class DeploymentDiscovery : MaintenanceWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			if (!FfoLocalEndpointManager.IsForefrontForOfficeDatacenter)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.DeploymentTracer, DeploymentDiscovery.traceContext, "[DeploymentDiscovery.DoWork]: This is not a FFO datacenter machine.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\Deployment\\DeploymentDiscovery.cs", 43);
				base.Result.StateAttribute1 = "DeploymentDiscovery: This is not a FFO datacenter machine.";
				return;
			}
			GenericWorkItemHelper.CreateAllDefinitions(new List<string>
			{
				"Deployment.xml"
			}, base.Broker, base.TraceContext, base.Result);
			WTFDiagnostics.TraceInformation(ExTraceGlobals.DeploymentTracer, DeploymentDiscovery.traceContext, "[DeploymentDiscovery.DoWork]: DeploymentDiscovery work item definitions created.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\Deployment\\DeploymentDiscovery.cs", 58);
		}

		private static TracingContext traceContext = new TracingContext();
	}
}
