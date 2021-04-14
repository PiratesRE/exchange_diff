using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ForefrontActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring
{
	public sealed class WebStoreDiscovery : MaintenanceWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			if (!FfoLocalEndpointManager.IsWebstoreInstalled)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.WebStoreTracer, WebStoreDiscovery.traceContext, "[WebStoreDiscovery.DoWork]: Webstore is not installed on this server.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\WebStore\\WebStoreDiscovery.cs", 43);
				base.Result.StateAttribute1 = "WebStoreDiscovery: Webstore is not installed on this server.";
				return;
			}
			GenericWorkItemHelper.CreateAllDefinitions(new List<string>
			{
				"WebStore.xml",
				"FfoWebStore.xml"
			}, base.Broker, base.TraceContext, base.Result);
			WTFDiagnostics.TraceInformation(ExTraceGlobals.WebStoreTracer, WebStoreDiscovery.traceContext, "[WebStoreDiscovery.DoWork]: Webstore work item definitions created.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\WebStore\\WebStoreDiscovery.cs", 63);
		}

		private static TracingContext traceContext = new TracingContext();
	}
}
