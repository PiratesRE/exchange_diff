using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ForefrontActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.WebService
{
	public sealed class WebServiceDiscovery : MaintenanceWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			if (!FfoLocalEndpointManager.IsWebServiceInstalled)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.WebServiceTracer, WebServiceDiscovery.traceContext, "[WebServiceDiscovery.DoWork]: WebService role is not installed on this server.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\WebService\\WebServiceDiscovery.cs", 44);
				base.Result.StateAttribute1 = "WebServiceDiscovery: WebService role is not installed on this server.";
				return;
			}
			GenericWorkItemHelper.CreateAllDefinitions(new List<string>
			{
				"UMCWebService.xml",
				"DALWebService.xml",
				"RusPublisherWeb.xml"
			}, base.Broker, base.TraceContext, base.Result);
		}

		private const string EscalationTeam = "FFO Web Service";

		private static TracingContext traceContext = new TracingContext();
	}
}
