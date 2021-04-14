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
	public sealed class MtrtDiscovery : MaintenanceWorkItem
	{
		internal static bool DoesMTRTServiceExist()
		{
			ServiceController[] services = ServiceController.GetServices();
			ServiceController serviceController = services.FirstOrDefault((ServiceController s) => s.ServiceName.Equals("MSMessageTracingClient", StringComparison.InvariantCultureIgnoreCase));
			return serviceController != null;
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			if (!MtrtDiscovery.DoesMTRTServiceExist())
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.MessageTracingTracer, MtrtDiscovery.traceContext, "[MtrtDiscovery.DoWork]: MessageTracing role is not installed on this server.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\MessageTracing\\MTRTDiscovery.cs", 56);
				base.Result.StateAttribute1 = "MtrtDiscovery: MessageTracing role is not installed on this server.";
				return;
			}
			GenericWorkItemHelper.CreateAllDefinitions(new List<string>
			{
				"MTRT.xml",
				"MessageTracing.xml"
			}, base.Broker, base.TraceContext, base.Result);
		}

		public const string MessageTracingServiceName = "MSMessageTracingClient";

		private static TracingContext traceContext = new TracingContext();
	}
}
