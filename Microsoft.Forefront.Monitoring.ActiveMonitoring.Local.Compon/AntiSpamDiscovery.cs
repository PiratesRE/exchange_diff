using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ForefrontActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring
{
	public sealed class AntiSpamDiscovery : MaintenanceWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			if (FfoLocalEndpointManager.IsHubTransportRoleInstalled)
			{
				GenericWorkItemHelper.CreateAllDefinitions(new string[]
				{
					"AntiSpam.xml",
					"Common_Agents_SpamEngine.xml",
					"Hub_Agents_SpamAnalysis.xml",
					"Hub_Agents_SpamFilter.xml",
					"Hub_Agents_SpamFeedDelivery.xml"
				}, base.Broker, base.TraceContext, base.Result);
			}
			if (FfoLocalEndpointManager.IsFrontendTransportRoleInstalled)
			{
				GenericWorkItemHelper.CreateAllDefinitions(new string[]
				{
					"AntiSpam.xml",
					"Common_Agents_SpamEngine.xml",
					"Frontdoor_Agents_EnvelopeFilter.xml",
					"Frontdoor_Agents_IPFilter.xml"
				}, base.Broker, base.TraceContext, base.Result);
			}
			if (DiscoveryUtils.IsMailboxRoleInstalled())
			{
				GenericWorkItemHelper.CreateAllDefinitions(new string[]
				{
					"Common_Agents_SpamEngine.xml",
					"Mailbox_Agents_SpamFilter.xml"
				}, base.Broker, base.TraceContext, base.Result);
			}
			if (!FfoLocalEndpointManager.IsHubTransportRoleInstalled && !FfoLocalEndpointManager.IsFrontendTransportRoleInstalled && !DiscoveryUtils.IsMailboxRoleInstalled())
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.AntiSpamTracer, AntiSpamDiscovery.traceContext, "[AntiSpamDiscovery.DoWork]: Neither HubTransport, FrontendTransport, or Mailbox roles are installed on this server.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\AntiSpam\\AntiSpamDiscovery.cs", 88);
				base.Result.StateAttribute1 = "AntiSpamDiscovery: Neither HubTransport, FrontendTransport, or Mailbox roles are installed on this server.";
			}
		}

		private static TracingContext traceContext = new TracingContext();
	}
}
