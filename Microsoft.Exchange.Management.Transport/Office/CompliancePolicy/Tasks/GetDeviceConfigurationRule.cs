using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.UnifiedPolicy;
using Microsoft.Exchange.Management.Transport;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	[Cmdlet("Get", "DeviceConfigurationRule", DefaultParameterSetName = "Identity")]
	public sealed class GetDeviceConfigurationRule : GetDeviceRuleBase
	{
		public GetDeviceConfigurationRule() : base(PolicyScenario.DeviceSettings)
		{
		}

		protected override bool IsDeviceRule(string identity)
		{
			return DevicePolicyUtility.IsDeviceConfigurationRule(identity);
		}

		protected override DeviceRuleBase CreateDeviceRuleObject(RuleStorage ruleStorage)
		{
			return new DeviceConfigurationRule(ruleStorage);
		}

		protected override string GetCanOnlyManipulateErrorString()
		{
			return Strings.CanOnlyManipulateDeviceConfigurationRule;
		}
	}
}
