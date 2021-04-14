using System;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Monitors;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring
{
	internal class ComponentHealthPercentFailureMonitorHelper : MonitorDefinitionHelper
	{
		internal override MonitorDefinition CreateDefinition()
		{
			MonitorDefinition monitorDefinition = ComponentHealthPercentFailureMonitor.CreateDefinition(base.Component, base.RecurrenceIntervalSeconds, base.TimeoutSeconds, base.MaxRetryAttempts, base.MonitoringIntervalSeconds, base.MonitoringThreshold);
			base.GetAdditionalProperties(monitorDefinition);
			return monitorDefinition;
		}
	}
}
