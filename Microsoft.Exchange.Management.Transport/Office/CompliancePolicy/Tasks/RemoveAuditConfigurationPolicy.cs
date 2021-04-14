using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Storage.UnifiedPolicy;
using Microsoft.Exchange.Management.Transport;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	[Cmdlet("Remove", "AuditConfigurationPolicy", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveAuditConfigurationPolicy : RemoveCompliancePolicyBase
	{
		public RemoveAuditConfigurationPolicy() : base(PolicyScenario.AuditSettings)
		{
		}

		protected override void InternalValidate()
		{
			this.ValidateAuditConfigurationPolicyParameter();
			base.InternalValidate();
		}

		private void ValidateAuditConfigurationPolicyParameter()
		{
			if (this.Identity != null && !AuditPolicyUtility.IsAuditConfigurationPolicy(this.Identity.ToString()))
			{
				base.WriteError(new ArgumentException(Strings.CanOnlyManipulateAuditConfigurationPolicy), ErrorCategory.InvalidArgument, null);
			}
		}
	}
}
