using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.UnifiedPolicy;
using Microsoft.Exchange.Management.Transport;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	[Cmdlet("Remove", "DeviceTenantRule", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveDeviceTenantRule : RemoveDeviceRuleBase
	{
		public RemoveDeviceTenantRule() : base(PolicyScenario.DeviceTenantConditionalAccess)
		{
		}

		protected override DeviceRuleBase CreateDeviceRule(RuleStorage ruleStorage)
		{
			return new DeviceTenantRule(base.DataObject);
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.RemoveDeviceTenantRuleConfirmation(this.Identity.ToString());
			}
		}
	}
}
