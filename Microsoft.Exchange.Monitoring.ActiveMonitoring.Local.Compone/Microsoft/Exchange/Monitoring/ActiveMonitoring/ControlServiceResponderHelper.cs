using System;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Transport.Responders;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring
{
	internal class ControlServiceResponderHelper : ResponderDefinitionHelper
	{
		internal override ResponderDefinition CreateDefinition()
		{
			string mandatoryValue = base.GetMandatoryValue<string>("WindowsServiceName");
			int mandatoryValue2 = base.GetMandatoryValue<int>("ControlCode");
			string optionalValue = base.GetOptionalValue<string>("ThrottleGroupName", "");
			string name = base.Name;
			string alertMask = base.AlertMask;
			ServiceHealthStatus targetHealthState = base.TargetHealthState;
			return ControlServiceResponder.CreateDefinition(name, alertMask, mandatoryValue, targetHealthState, mandatoryValue2, optionalValue);
		}
	}
}
