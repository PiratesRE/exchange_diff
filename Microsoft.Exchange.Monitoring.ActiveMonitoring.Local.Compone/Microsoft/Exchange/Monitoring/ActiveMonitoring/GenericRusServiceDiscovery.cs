using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring
{
	public sealed class GenericRusServiceDiscovery : MaintenanceWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			try
			{
				if (!FfoLocalEndpointManager.IsWebServiceInstalled)
				{
					string text = "GenericRusServiceDiscovery: FFO Web service role is not installed on this server.";
					WTFDiagnostics.TraceInformation(ExTraceGlobals.GenericRusTracer, GenericRusServiceDiscovery.traceContext, text, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\GenericRus\\GenericRusServiceDiscovery.cs", 45);
					base.Result.StateAttribute1 = text;
				}
				else
				{
					GenericWorkItemHelper.CreateAllDefinitions(new List<string>
					{
						"GenericRus_Server.xml"
					}, base.Broker, base.TraceContext, base.Result);
				}
			}
			catch (EndpointManagerEndpointUninitializedException)
			{
				WTFDiagnostics.TraceDebug(ExTraceGlobals.GenericRusTracer, base.TraceContext, "[GenericRusServiceDiscovery.DoWork]: EndpointException occurred, ignoring exception and treating as transient.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\GenericRus\\GenericRusServiceDiscovery.cs", 64);
			}
		}

		private static TracingContext traceContext = new TracingContext();
	}
}
