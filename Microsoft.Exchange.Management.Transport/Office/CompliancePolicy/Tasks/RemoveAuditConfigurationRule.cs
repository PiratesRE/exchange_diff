using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Storage.UnifiedPolicy;
using Microsoft.Exchange.Management.Transport;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	[Cmdlet("Remove", "AuditConfigurationRule", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveAuditConfigurationRule : RemoveComplianceRuleBase
	{
		public RemoveAuditConfigurationRule() : base(PolicyScenario.AuditSettings)
		{
		}

		protected override void InternalValidate()
		{
			this.ValidateAuditConfigurationRuleParameter();
			base.InternalValidate();
		}

		private void ValidateAuditConfigurationRuleParameter()
		{
			if (this.Identity != null && !AuditPolicyUtility.IsAuditConfigurationRule(this.Identity.ToString()))
			{
				base.WriteError(new ArgumentException(Strings.CanOnlyManipulateAuditConfigurationRule), ErrorCategory.InvalidArgument, null);
			}
		}
	}
}
