using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ForefrontActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring
{
	public sealed class Migration1415Discovery : MaintenanceWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			if (!FfoLocalEndpointManager.IsBackgroundRoleInstalled)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.FFOMigration1415Tracer, Migration1415Discovery.traceContext, "[FFO Migration1415Discovery.DoWork]: Background role is not installed on this server.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\Migration1415\\Migration1415Discovery.cs", 42);
				base.Result.StateAttribute1 = "FFO Migration1415Discovery: Background role is not installed on this server.";
				return;
			}
			try
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.FFOMigration1415Tracer, Migration1415Discovery.traceContext, "[FFO Migration1415Discovery.DoWork]: Discovery Started.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\Migration1415\\Migration1415Discovery.cs", 54);
				GenericWorkItemHelper.CreateAllDefinitions(new List<string>
				{
					"FFOMigration1415.xml"
				}, base.Broker, Migration1415Discovery.traceContext, base.Result);
				WTFDiagnostics.TraceInformation(ExTraceGlobals.FFOMigration1415Tracer, Migration1415Discovery.traceContext, "[FFO Migration1415Discovery.DoWork]: Discovery Completed.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\Migration1415\\Migration1415Discovery.cs", 68);
			}
			catch (Exception ex)
			{
				WTFDiagnostics.TraceError<string>(ExTraceGlobals.FFOMigration1415Tracer, Migration1415Discovery.traceContext, "[FFO Migration1415Discovery.DoWork]: Exception occurred during discovery. {0}", ex.ToString(), null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\Migration1415\\Migration1415Discovery.cs", 75);
				throw;
			}
		}

		private static TracingContext traceContext = new TracingContext();
	}
}
