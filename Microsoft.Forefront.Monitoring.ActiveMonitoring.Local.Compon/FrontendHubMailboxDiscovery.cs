using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring
{
	public sealed class FrontendHubMailboxDiscovery : MaintenanceWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			WTFDiagnostics.TraceInformation(ExTraceGlobals.TransportTracer, base.TraceContext, "FrontendHubMailboxDiscovery.DoWork", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\Transport\\Discovery\\FrontEndHubMailboxDiscovery.cs", 31);
			if (!DiscoveryUtils.IsFrontendTransportRoleInstalled() && !DiscoveryUtils.IsHubTransportRoleInstalled() && !DiscoveryUtils.IsMailboxRoleInstalled())
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.TransportTracer, base.TraceContext, "FrontendHubMailboxDiscovery.DoWork(): None of Frontend, hub or mailbox roles are installed. Skip.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\Transport\\Discovery\\FrontEndHubMailboxDiscovery.cs", 37);
				base.Result.StateAttribute1 = "FrontendHubMailboxDiscovery: None of Frontend, hub or mailbox roles are installed. Skip.";
				return;
			}
			GenericWorkItemHelper.CreateAllDefinitions(new List<string>
			{
				"Interceptor_FrontendHubMailbox.xml",
				"Common_Transport.xml"
			}, base.Broker, base.TraceContext, base.Result);
		}
	}
}
