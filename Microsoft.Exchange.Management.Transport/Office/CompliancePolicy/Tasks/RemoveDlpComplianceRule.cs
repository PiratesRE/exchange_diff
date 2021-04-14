using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Transport;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	[Cmdlet("Remove", "DlpComplianceRule", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveDlpComplianceRule : RemoveComplianceRuleBase
	{
		public RemoveDlpComplianceRule() : base(PolicyScenario.Dlp)
		{
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			PsDlpComplianceRule psDlpComplianceRule = new PsDlpComplianceRule(base.DataObject);
			psDlpComplianceRule.PopulateTaskProperties(this, base.DataSession as IConfigurationSession);
			if (psDlpComplianceRule.ReadOnly && !base.ForceDeletion)
			{
				throw new TaskRuleIsTooAdvancedToModifyException(psDlpComplianceRule.Name);
			}
		}
	}
}
