using System;
using System.Threading;
using Microsoft.Exchange.Monitoring.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring
{
	public sealed class NtpDelayDetectionDiscovery : MaintenanceWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			if (!FfoLocalEndpointManager.IsForefrontForOfficeDatacenter)
			{
				base.Result.StateAttribute1 = "NtpDelayDetectionDiscovery: This is not a FFO datacenter machine.";
				return;
			}
			base.Result.StateAttribute1 = "NTPDelayDetectionDiscovery: install NTP probes.";
			GenericWorkItemHelper.CreateAllDefinitions(new string[]
			{
				"NTPEventMonitor.xml"
			}, base.Broker, base.TraceContext, base.Result);
		}
	}
}
