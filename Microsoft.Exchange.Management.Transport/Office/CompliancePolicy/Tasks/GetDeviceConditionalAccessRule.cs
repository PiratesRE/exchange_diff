using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.UnifiedPolicy;
using Microsoft.Exchange.Management.Transport;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	[Cmdlet("Get", "DeviceConditionalAccessRule", DefaultParameterSetName = "Identity")]
	public sealed class GetDeviceConditionalAccessRule : GetDeviceRuleBase
	{
		public GetDeviceConditionalAccessRule() : base(PolicyScenario.DeviceConditionalAccess)
		{
		}

		protected override bool IsDeviceRule(string identity)
		{
			return DevicePolicyUtility.IsDeviceConditionalAccessRule(identity);
		}

		protected override DeviceRuleBase CreateDeviceRuleObject(RuleStorage ruleStorage)
		{
			return new DeviceConditionalAccessRule(ruleStorage);
		}

		protected override string GetCanOnlyManipulateErrorString()
		{
			return Strings.CanOnlyManipulateDeviceConditionalAccessRule;
		}
	}
}
