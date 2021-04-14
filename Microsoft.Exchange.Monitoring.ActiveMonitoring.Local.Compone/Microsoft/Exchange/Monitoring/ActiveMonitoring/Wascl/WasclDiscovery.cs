using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Wascl
{
	public sealed class WasclDiscovery : MaintenanceWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			LocalEndpointManager instance = LocalEndpointManager.Instance;
			if (instance.ExchangeServerRoleEndpoint == null || !instance.ExchangeServerRoleEndpoint.IsMailboxRoleInstalled)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.WasclTracer, base.TraceContext, "WasclDiscovery.DoWork: Mailbox role is not installed on this server. Wascl maintenance items would not be loaded", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Wascl\\WasclDiscovery.cs", 35);
				return;
			}
			if (!LocalEndpointManager.IsDataCenter)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.WasclTracer, base.TraceContext, "WasclDiscovery.DoWork: Wascl cannot run on non-Datacenter deployments", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Wascl\\WasclDiscovery.cs", 41);
				return;
			}
			WTFDiagnostics.TraceInformation(ExTraceGlobals.WasclTracer, base.TraceContext, "Wascl.DoWork Discovery Started.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Wascl\\WasclDiscovery.cs", 45);
			GenericWorkItemHelper.CreateAllDefinitions(new string[]
			{
				"Wascl.xml"
			}, base.Broker, base.TraceContext, base.Result);
		}
	}
}
