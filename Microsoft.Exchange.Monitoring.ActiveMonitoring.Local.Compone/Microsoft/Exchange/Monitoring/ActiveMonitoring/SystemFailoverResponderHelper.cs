using System;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Responders;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring
{
	internal class SystemFailoverResponderHelper : ResponderDefinitionHelper
	{
		internal override ResponderDefinition CreateDefinition()
		{
			string text = (!string.IsNullOrWhiteSpace(base.ServiceName)) ? base.ServiceName : "Exchange";
			string mandatoryValue = base.GetMandatoryValue<string>("ComponentName");
			string name = base.Name;
			string alertMask = base.AlertMask;
			ServiceHealthStatus targetHealthState = base.TargetHealthState;
			bool enabled = base.Enabled;
			string serviceName = text;
			return SystemFailoverResponder.CreateDefinition(name, alertMask, targetHealthState, mandatoryValue, serviceName, enabled);
		}
	}
}
