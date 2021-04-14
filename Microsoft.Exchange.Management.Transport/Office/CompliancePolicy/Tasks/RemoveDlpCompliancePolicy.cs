using System;
using System.Management.Automation;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	[Cmdlet("Remove", "DlpCompliancePolicy", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveDlpCompliancePolicy : RemoveCompliancePolicyBase
	{
		public RemoveDlpCompliancePolicy() : base(PolicyScenario.Dlp)
		{
		}
	}
}
