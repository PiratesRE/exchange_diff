using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.UnifiedPolicy;
using Microsoft.Exchange.Management.Transport;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	[Cmdlet("Remove", "DeviceConditionalAccessRule", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveDeviceConditionalAccessRule : RemoveDeviceRuleBase
	{
		public RemoveDeviceConditionalAccessRule() : base(PolicyScenario.DeviceConditionalAccess)
		{
		}

		protected override DeviceRuleBase CreateDeviceRule(RuleStorage ruleStorage)
		{
			return new DeviceConditionalAccessRule(base.DataObject);
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.RemoveDeviceConditionalAccessRuleConfirmation(this.Identity.ToString());
			}
		}
	}
}
