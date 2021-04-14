using System;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring
{
	internal class OverallPercentSuccessMonitorHelper : MonitorDefinitionHelper
	{
		internal override MonitorDefinition CreateDefinition()
		{
			MonitorDefinition monitorDefinition = OverallPercentSuccessMonitor.CreateDefinition(base.Name, base.SampleMask, base.ServiceName, base.Component, base.MonitoringThreshold, TimeSpan.FromSeconds((double)base.MonitoringIntervalSeconds), base.MinimumErrorCount, base.Enabled);
			base.GetAdditionalProperties(monitorDefinition);
			return monitorDefinition;
		}
	}
}
