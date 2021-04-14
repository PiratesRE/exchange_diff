using System;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring
{
	internal class RestartServiceResponderHelper : ResponderDefinitionHelper
	{
		internal override ResponderDefinition CreateDefinition()
		{
			string text = (!string.IsNullOrWhiteSpace(base.ServiceName)) ? base.ServiceName : "Exchange";
			string mandatoryValue = base.GetMandatoryValue<string>("WindowsServiceName");
			int optionalValue = base.GetOptionalValue<int>("ServiceStopTimeout", 15);
			int optionalValue2 = base.GetOptionalValue<int>("ServiceStartTimeout", 120);
			int optionalValue3 = base.GetOptionalValue<int>("ServiceStartDelay", 0);
			bool optionalEnumValue = base.GetOptionalEnumValue<bool>("IsMasterAndWorker", false);
			DumpMode optionalEnumValue2 = base.GetOptionalEnumValue<DumpMode>("DumpOnRestart", DumpMode.None);
			string optionalValue4 = base.GetOptionalValue<string>("DumpPath", null);
			double optionalValue5 = base.GetOptionalValue<double>("MinimumFreeDiskPercent", 15.0);
			int optionalValue6 = base.GetOptionalValue<int>("MaximumDumpDurationInSeconds", 0);
			string name = base.Name;
			string alertMask = base.AlertMask;
			ServiceHealthStatus targetHealthState = base.TargetHealthState;
			bool enabled = base.Enabled;
			string serviceName = text;
			return RestartServiceResponder.CreateDefinition(name, alertMask, mandatoryValue, targetHealthState, optionalValue, optionalValue2, optionalValue3, optionalEnumValue, optionalEnumValue2, optionalValue4, optionalValue5, optionalValue6, serviceName, null, true, enabled, null, false);
		}
	}
}
