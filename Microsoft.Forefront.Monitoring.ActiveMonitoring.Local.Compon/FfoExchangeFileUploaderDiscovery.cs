using System;
using System.Collections.Generic;
using System.ServiceProcess;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ForefrontActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring
{
	public sealed class FfoExchangeFileUploaderDiscovery : MaintenanceWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			if (!FfoLocalEndpointManager.IsForefrontForOfficeDatacenter && !FfoLocalEndpointManager.IsCentralAdminRoleInstalled)
			{
				return;
			}
			bool flag = false;
			foreach (ServiceController serviceController in ServiceController.GetServices())
			{
				if (serviceController.ServiceName == "MSExchangeFileUpload")
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.DataminingTracer, FfoExchangeFileUploaderDiscovery.traceContext, "[DataminingTracer.DoWork]: MSExchangeFileUpload service is not installed on the box.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\Datamining\\FfoExchangeFileUploaderDiscovery.cs", 60);
				base.Result.StateAttribute1 = "DataminingTracer: MSExchangeFileUpload service is not installed on the box.";
				return;
			}
			GenericWorkItemHelper.CreateAllDefinitions(new List<string>
			{
				"FfoExchangeFileUploader.xml"
			}, base.Broker, base.TraceContext, base.Result);
			WTFDiagnostics.TraceInformation(ExTraceGlobals.DataminingTracer, FfoExchangeFileUploaderDiscovery.traceContext, "[DataminingTracer.DoWork]: DataminingTracer work item definitions created.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\Datamining\\FfoExchangeFileUploaderDiscovery.cs", 79);
		}

		private static TracingContext traceContext = new TracingContext();
	}
}
