using System;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring
{
	internal class OverallConsecutiveProbeFailuresMonitorHelper : MonitorDefinitionHelper
	{
		internal override MonitorDefinition CreateDefinition()
		{
			MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition(base.Name, base.SampleMask, base.ServiceName, base.Component, (int)base.MonitoringThreshold, base.Enabled, (base.MonitoringIntervalSeconds > 0) ? base.MonitoringIntervalSeconds : 300);
			base.GetAdditionalProperties(monitorDefinition);
			return monitorDefinition;
		}
	}
}
