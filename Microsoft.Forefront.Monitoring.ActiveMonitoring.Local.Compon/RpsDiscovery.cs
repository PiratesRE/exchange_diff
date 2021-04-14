using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ForefrontActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring
{
	public sealed class RpsDiscovery : MaintenanceWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			LocalEndpointManager instance = LocalEndpointManager.Instance;
			try
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.RPSTracer, base.TraceContext, "[FFO RPS.DoWork]: Discovery Started.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\RPS\\RpsDiscovery.cs", 41);
				GenericWorkItemHelper.CreateAllDefinitions(new List<string>
				{
					"FFORpsDiscovery.xml"
				}, base.Broker, base.TraceContext, base.Result);
			}
			catch (Exception ex)
			{
				WTFDiagnostics.TraceError<string>(ExTraceGlobals.RPSTracer, base.TraceContext, "[FFO RPS.DoWork]: Exception occurred during discovery. {0}", ex.ToString(), null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\RPS\\RpsDiscovery.cs", 57);
				throw;
			}
		}

		private const string ServiceName = "RPS";
	}
}
