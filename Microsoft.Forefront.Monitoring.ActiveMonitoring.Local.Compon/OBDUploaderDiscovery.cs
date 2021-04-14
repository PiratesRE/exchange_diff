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
	public sealed class OBDUploaderDiscovery : MaintenanceWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			LocalEndpointManager instance = LocalEndpointManager.Instance;
			if (!instance.ExchangeServerRoleEndpoint.IsMailboxRoleInstalled || FfoLocalEndpointManager.IsForefrontForOfficeDatacenter)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.DataminingTracer, OBDUploaderDiscovery.traceContext, "[OBDUploaderDiscovery.DoWork]: only enable on EXO BE box for now. Later need to enable it in EOP", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\Datamining\\OBDUploaderDiscovery.cs", 37);
				base.Result.StateAttribute1 = "OBD file uploader: only enable on EXO BE box for now.";
				return;
			}
			GenericWorkItemHelper.CreateAllDefinitions(new List<string>
			{
				"OBDUploader.xml"
			}, base.Broker, OBDUploaderDiscovery.traceContext, base.Result);
			WTFDiagnostics.TraceInformation(ExTraceGlobals.DataminingTracer, OBDUploaderDiscovery.traceContext, "OBD file uploader:  work item definitions created", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\Datamining\\OBDUploaderDiscovery.cs", 56);
		}

		private static TracingContext traceContext = new TracingContext();
	}
}
