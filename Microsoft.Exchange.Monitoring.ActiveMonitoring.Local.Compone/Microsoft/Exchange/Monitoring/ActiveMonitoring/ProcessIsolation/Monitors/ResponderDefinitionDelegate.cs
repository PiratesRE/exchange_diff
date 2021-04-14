using System;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.ProcessIsolation.Monitors
{
	public delegate ResponderDefinition ResponderDefinitionDelegate(MonitorDefinition monitor, ServiceHealthStatus status);
}
