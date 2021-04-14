using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ForefrontActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring
{
	public sealed class MessageTracingHubDiscovery : MaintenanceWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			WTFDiagnostics.TraceInformation(ExTraceGlobals.MessageTracingTracer, base.TraceContext, "MessageTracingHubDiscovery.DoWork", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\MessageTracing\\Discovery\\MessageTracingHubDiscovery.cs", 31);
			if (!DiscoveryUtils.IsHubTransportRoleInstalled())
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.MessageTracingTracer, base.TraceContext, "MessageTracingHubDiscovery.DoWork(): Hub role not installed. Skip.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\MessageTracing\\Discovery\\MessageTracingHubDiscovery.cs", 35);
				base.Result.StateAttribute1 = "MessageTracingHubDiscovery: Hub role not installed. Skip.";
				return;
			}
			GenericWorkItemHelper.CreateAllDefinitions(new List<string>
			{
				"MessageTracing_Hub.xml"
			}, base.Broker, base.TraceContext, base.Result);
		}
	}
}
