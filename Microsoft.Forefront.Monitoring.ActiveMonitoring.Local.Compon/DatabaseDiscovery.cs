using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ForefrontActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring
{
	public sealed class DatabaseDiscovery : MaintenanceWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			if (!FfoLocalEndpointManager.IsDatabaseRoleInstalled && !FfoLocalEndpointManager.IsInfraDatabaseRoleInstalled)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.DALTracer, DatabaseDiscovery.traceContext, "[DatabaseDiscovery.DoWork]: Databases are not installed on this server.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\Database\\DatabaseDiscovery.cs", 36);
				base.Result.StateAttribute1 = "DatabaseDiscovery: Databases are not installed on this server.";
				return;
			}
			WTFDiagnostics.TraceInformation(ExTraceGlobals.DALTracer, DatabaseDiscovery.traceContext, "[DatabaseDiscovery.DoWork]: Databases are installed on this server.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\Database\\DatabaseDiscovery.cs", 45);
			GenericWorkItemHelper.CreateAllDefinitions(new string[]
			{
				"FFODatabase.xml"
			}, base.Broker, base.TraceContext, base.Result);
		}

		private static TracingContext traceContext = new TracingContext();
	}
}
