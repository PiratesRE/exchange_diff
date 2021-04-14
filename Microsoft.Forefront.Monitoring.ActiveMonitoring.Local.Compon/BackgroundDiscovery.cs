using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ForefrontActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring
{
	public sealed class BackgroundDiscovery : MaintenanceWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			if (!FfoLocalEndpointManager.IsBackgroundRoleInstalled)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.BackgroundTracer, BackgroundDiscovery.traceContext, "[BackgroundDiscovery.DoWork]: Background role is not installed on this server.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\Background\\BackgroundDiscovery.cs", 49);
				base.Result.StateAttribute1 = "BackgroundDiscovery: Background role is not installed on this server.";
				return;
			}
			GenericWorkItemHelper.CreateAllDefinitions(new List<string>
			{
				"Background.xml",
				"BGD_ForwardSync.xml",
				"BGD_IPListGenerators.xml",
				"BGD_IPUriPuller.xml",
				"BGD_LatencyBucket.xml",
				"BGD_OutboundIpReputation.xml",
				"BGD_OutboundSpamAlerting.xml",
				"BGD_RulesDataBlobGenerator.xml",
				"BGD_PackageManager.xml",
				"BGD_RulesPublisher.xml",
				"BGD_InterServiceSpamDataSync.xml",
				"BGD_UriGenerator.xml",
				"FfoBackground.xml",
				"BGD_EngineUpdatePublisher.xml",
				"BGD_ESNJob.xml",
				"BGD_AnalystAlerting.xml",
				"BGD_AnalystRulesPublishing.xml"
			}, base.Broker, base.TraceContext, base.Result);
		}

		private static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		private static TracingContext traceContext = new TracingContext();
	}
}
