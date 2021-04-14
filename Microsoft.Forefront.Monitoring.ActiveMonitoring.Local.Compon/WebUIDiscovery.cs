using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ForefrontActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring
{
	public sealed class WebUIDiscovery : MaintenanceWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			if (!FfoLocalEndpointManager.IsWebServiceInstalled)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.HTTPTracer, WebUIDiscovery.traceContext, "[WebUIDiscovery.DoWork]: WebService role is not installed on this server.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\WebUI\\WebUIDiscovery.cs", 42);
				return;
			}
			GenericWorkItemHelper.CreateAllDefinitions(new List<string>
			{
				"WebUI.xml"
			}, base.Broker, WebUIDiscovery.traceContext, base.Result);
		}

		private static TracingContext traceContext = new TracingContext();
	}
}
