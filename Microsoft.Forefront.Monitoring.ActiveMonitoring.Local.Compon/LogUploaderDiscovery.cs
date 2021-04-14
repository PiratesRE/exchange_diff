using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ForefrontActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring
{
	public sealed class LogUploaderDiscovery : MaintenanceWorkItem
	{
		internal static bool DoesLogUploaderServiceExist()
		{
			return ServiceController.GetServices().Any((ServiceController s) => s.ServiceName == "MSComplianceAudit");
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			WTFDiagnostics.TraceInformation(ExTraceGlobals.ProvisioningTracer, LogUploaderDiscovery.traceContext, "[LogUploaderDiscovery.DoWork]: Started Execution.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\LogUploader\\LogUploaderDiscovery.cs", 48);
			if (!LogUploaderDiscovery.DoesLogUploaderServiceExist())
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.MessageTracingTracer, LogUploaderDiscovery.traceContext, "[LogUploaderDiscovery.DoWork]: ComplianceAuditService is not installed on this server.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\LogUploader\\LogUploaderDiscovery.cs", 52);
				base.Result.StateAttribute1 = "LogUploaderDiscovery: ComplianceAuditService is not installed on this server.";
				return;
			}
			GenericWorkItemHelper.CreateAllDefinitions(new List<string>
			{
				"LogUploader.xml",
				"LogUploaderDefinitions.xml"
			}, base.Broker, base.TraceContext, base.Result);
			WTFDiagnostics.TraceInformation(ExTraceGlobals.ProvisioningTracer, LogUploaderDiscovery.traceContext, "[LogUploaderDiscovery.DoWork]: Ended Execution.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\LogUploader\\LogUploaderDiscovery.cs", 72);
		}

		public const string UploaderServiceName = "MSComplianceAudit";

		private static TracingContext traceContext = new TracingContext();
	}
}
