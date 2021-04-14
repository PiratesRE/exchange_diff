using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.UnifiedPolicy;
using Microsoft.Exchange.Management.Transport;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	[Cmdlet("Get", "DeviceTenantRule", DefaultParameterSetName = "Identity")]
	public sealed class GetDeviceTenantRule : GetDeviceRuleBase
	{
		public GetDeviceTenantRule() : base(PolicyScenario.DeviceTenantConditionalAccess)
		{
		}

		protected override bool IsDeviceRule(string identity)
		{
			return DevicePolicyUtility.IsDeviceTenantRule(identity);
		}

		protected override DeviceRuleBase CreateDeviceRuleObject(RuleStorage ruleStorage)
		{
			return new DeviceTenantRule(ruleStorage);
		}

		protected override string GetCanOnlyManipulateErrorString()
		{
			return Strings.CanOnlyManipulateDeviceTenantRule;
		}
	}
}
