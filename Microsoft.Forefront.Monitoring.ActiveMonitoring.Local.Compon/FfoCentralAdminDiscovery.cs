using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ForefrontActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring
{
	public sealed class FfoCentralAdminDiscovery : MaintenanceWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			if (!FfoLocalEndpointManager.IsCentralAdminRoleInstalled)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.CentralAdminTracer, FfoCentralAdminDiscovery.traceContext, "[FfoCentralAdminDiscovery.DoWork]: This is not a FFO CentralAdmin machine.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\FfoCentralAdmin\\FfoCentralAdminDiscovery.cs", 43);
				base.Result.StateAttribute1 = "FfoCentralAdminDiscovery: This is not a FFO CentralAdmin machine.";
				return;
			}
			GenericWorkItemHelper.CreateAllDefinitions(new List<string>
			{
				"FfoCentralAdmin.xml",
				"FfoRecoveryActionArbiter.xml"
			}, base.Broker, base.TraceContext, base.Result);
			WTFDiagnostics.TraceInformation(ExTraceGlobals.CentralAdminTracer, FfoCentralAdminDiscovery.traceContext, "[FfoCentralAdminDiscovery.DoWork]: FfoCentralAdminDiscovery work item definitions created.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\FfoCentralAdmin\\FfoCentralAdminDiscovery.cs", 63);
		}

		private static TracingContext traceContext = new TracingContext();
	}
}
