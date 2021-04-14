using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ForefrontActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.WebService
{
	public sealed class ExtendedReportWebDiscovery : MaintenanceWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			if (!FfoLocalEndpointManager.IsWebServiceInstalled)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.HTTPTracer, ExtendedReportWebDiscovery.traceContext, "[ExtendedReportWebDiscovery.DoWork]: WebService role is not installed on this server.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\WebService\\ExtendedReportWebDiscovery.cs", 41);
				return;
			}
			GenericWorkItemHelper.CreateAllDefinitions(new List<string>
			{
				"ExtendedReportWeb.xml"
			}, base.Broker, base.TraceContext, base.Result);
		}

		private static TracingContext traceContext = new TracingContext();
	}
}
