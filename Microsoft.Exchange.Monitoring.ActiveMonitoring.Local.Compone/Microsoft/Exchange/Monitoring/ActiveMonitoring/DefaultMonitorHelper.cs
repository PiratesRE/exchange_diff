using System;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring
{
	internal class DefaultMonitorHelper : MonitorDefinitionHelper
	{
		internal override MonitorDefinition CreateDefinition()
		{
			return base.CreateMonitorDefinition();
		}
	}
}
