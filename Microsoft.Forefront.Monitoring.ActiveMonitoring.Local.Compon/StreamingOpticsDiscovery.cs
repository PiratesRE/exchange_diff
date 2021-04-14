using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ForefrontActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring
{
	public sealed class StreamingOpticsDiscovery : MaintenanceWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			if (FfoLocalEndpointManager.IsHubTransportRoleInstalled)
			{
				GenericWorkItemHelper.CreateAllDefinitions(new List<string>
				{
					"StreamingOptics.xml"
				}, base.Broker, base.TraceContext, base.Result);
			}
			if (FfoLocalEndpointManager.IsWebServiceInstalled)
			{
				GenericWorkItemHelper.CreateAllDefinitions(new List<string>
				{
					"StreamingOptics.xml",
					"WS_TblApp.xml"
				}, base.Broker, base.TraceContext, base.Result);
			}
			if (!FfoLocalEndpointManager.IsHubTransportRoleInstalled && !FfoLocalEndpointManager.IsWebServiceInstalled)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.AntiSpamTracer, StreamingOpticsDiscovery.traceContext, "[StreamingOpticsDiscovery.DoWork]: Neither HubTransport nor WebService role is installed on this server.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\StreamingOptics\\StreamingOpticsDiscovery.cs", 58);
				base.Result.StateAttribute1 = "StreamingOpticsDiscovery: Neither HubTransport nor WebService role is installed on this server.";
			}
		}

		private static readonly TracingContext traceContext = new TracingContext();
	}
}
