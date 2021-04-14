using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.UnifiedPolicy;
using Microsoft.Exchange.Management.Transport;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	[Cmdlet("Remove", "DeviceConfigurationRule", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveDeviceConfigurationRule : RemoveDeviceRuleBase
	{
		public RemoveDeviceConfigurationRule() : base(PolicyScenario.DeviceSettings)
		{
		}

		protected override DeviceRuleBase CreateDeviceRule(RuleStorage ruleStorage)
		{
			return new DeviceConfigurationRule(base.DataObject);
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.RemoveDeviceConfiguationRuleConfirmation(this.Identity.ToString());
			}
		}
	}
}
