using System;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Responders;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring
{
	internal class ResetIISAppPoolResponderHelper : ResponderDefinitionHelper
	{
		internal override ResponderDefinition CreateDefinition()
		{
			string text = (!string.IsNullOrWhiteSpace(base.ServiceName)) ? base.ServiceName : "Exchange";
			string mandatoryValue = base.GetMandatoryValue<string>("AppPoolName");
			DumpMode optionalEnumValue = base.GetOptionalEnumValue<DumpMode>("DumpOnRestart", DumpMode.None);
			string optionalValue = base.GetOptionalValue<string>("DumpPath", null);
			double optionalValue2 = base.GetOptionalValue<double>("MinimumFreeDiskPercent", 15.0);
			int optionalValue3 = base.GetOptionalValue<int>("MaximumDumpDurationInSeconds", 0);
			string name = base.Name;
			string alertMask = base.AlertMask;
			ServiceHealthStatus targetHealthState = base.TargetHealthState;
			bool enabled = base.Enabled;
			string serviceName = text;
			return ResetIISAppPoolResponder.CreateDefinition(name, alertMask, mandatoryValue, targetHealthState, optionalEnumValue, optionalValue, optionalValue2, optionalValue3, serviceName, enabled, null);
		}
	}
}
