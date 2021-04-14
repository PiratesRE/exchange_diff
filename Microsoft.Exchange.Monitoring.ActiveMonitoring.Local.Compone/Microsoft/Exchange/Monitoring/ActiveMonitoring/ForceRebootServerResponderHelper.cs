using System;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Responders;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring
{
	internal class ForceRebootServerResponderHelper : ResponderDefinitionHelper
	{
		internal override ResponderDefinition CreateDefinition()
		{
			string text = (!string.IsNullOrWhiteSpace(base.ServiceName)) ? base.ServiceName : "Exchange";
			string optionalValue = base.GetOptionalValue<string>("ServersInGroup", null);
			string[] serversInGroup = (optionalValue == null) ? null : optionalValue.Split(new char[]
			{
				',',
				';'
			});
			int optionalValue2 = base.GetOptionalValue<int>("MinimumRequiredServers", -1);
			string optionalValue3 = base.GetOptionalValue<string>("RecoveryId", "");
			string optionalValue4 = base.GetOptionalValue<string>("FailureReason", "");
			string optionalValue5 = base.GetOptionalValue<string>("ArbitrationScope", "Datacenter, Stamp");
			string optionalValue6 = base.GetOptionalValue<string>("ArbitrationSource", "RecoveryData");
			string optionalValue7 = base.GetOptionalValue<string>("RequestedAction", "ArbitrationOnly");
			string name = base.Name;
			string alertMask = base.AlertMask;
			ServiceHealthStatus targetHealthState = base.TargetHealthState;
			bool enabled = base.Enabled;
			string serviceName = text;
			return ForceRebootServerResponder.CreateDefinition(name, alertMask, targetHealthState, serversInGroup, optionalValue2, optionalValue3, optionalValue4, optionalValue5, optionalValue6, optionalValue7, serviceName, enabled, null, false);
		}
	}
}
