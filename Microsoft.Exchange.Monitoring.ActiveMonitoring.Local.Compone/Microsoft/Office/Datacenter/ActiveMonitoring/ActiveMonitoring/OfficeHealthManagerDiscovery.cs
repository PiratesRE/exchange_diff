using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Monitors;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Office.Datacenter.ActiveMonitoring.ActiveMonitoring
{
	public sealed class OfficeHealthManagerDiscovery : MaintenanceWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			WTFDiagnostics.TraceInformation(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "Adding healthstate collection monitor", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HM\\OfficeHealthManagerDiscovery.cs", 34);
			MonitorDefinition definition = HealthStateCollectionMonitor.CreateDefinition("ServerHealthStateCollectionMonitor");
			base.Broker.AddWorkDefinition<MonitorDefinition>(definition, base.TraceContext);
		}

		private const string HealthStateCollectionMonitorName = "ServerHealthStateCollectionMonitor";
	}
}
