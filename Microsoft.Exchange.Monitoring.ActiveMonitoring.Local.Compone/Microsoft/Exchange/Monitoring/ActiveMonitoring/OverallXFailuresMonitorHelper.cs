using System;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring
{
	internal class OverallXFailuresMonitorHelper : MonitorDefinitionHelper
	{
		internal override MonitorDefinition CreateDefinition()
		{
			MonitorDefinition monitorDefinition = OverallXFailuresMonitor.CreateDefinition(base.Name, base.SampleMask, base.ServiceName, base.Component, base.MonitoringIntervalSeconds, base.RecurrenceIntervalSeconds, (int)base.MonitoringThreshold, base.Enabled);
			base.GetAdditionalProperties(monitorDefinition);
			return monitorDefinition;
		}
	}
}
