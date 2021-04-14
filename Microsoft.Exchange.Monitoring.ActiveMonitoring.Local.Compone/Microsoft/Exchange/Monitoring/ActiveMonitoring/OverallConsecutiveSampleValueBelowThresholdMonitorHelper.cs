using System;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring
{
	internal class OverallConsecutiveSampleValueBelowThresholdMonitorHelper : MonitorDefinitionHelper
	{
		internal override MonitorDefinition CreateDefinition()
		{
			MonitorDefinition monitorDefinition = OverallConsecutiveSampleValueBelowThresholdMonitor.CreateDefinition(base.Name, base.SampleMask, base.ServiceName, base.Component, base.MonitoringThreshold, (int)base.SecondaryMonitoringThreshold, base.Enabled);
			base.GetAdditionalProperties(monitorDefinition);
			monitorDefinition.RecurrenceIntervalSeconds = base.RecurrenceIntervalSeconds;
			return monitorDefinition;
		}
	}
}
