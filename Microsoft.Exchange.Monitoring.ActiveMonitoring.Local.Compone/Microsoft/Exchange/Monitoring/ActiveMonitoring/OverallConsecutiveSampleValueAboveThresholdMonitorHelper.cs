using System;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring
{
	internal class OverallConsecutiveSampleValueAboveThresholdMonitorHelper : MonitorDefinitionHelper
	{
		internal override MonitorDefinition CreateDefinition()
		{
			MonitorDefinition monitorDefinition = OverallConsecutiveSampleValueAboveThresholdMonitor.CreateDefinition(base.Name, base.SampleMask, base.ServiceName, base.Component, base.MonitoringThreshold, (int)base.SecondaryMonitoringThreshold, base.Enabled);
			base.GetAdditionalProperties(monitorDefinition);
			monitorDefinition.RecurrenceIntervalSeconds = base.RecurrenceIntervalSeconds;
			return monitorDefinition;
		}
	}
}
