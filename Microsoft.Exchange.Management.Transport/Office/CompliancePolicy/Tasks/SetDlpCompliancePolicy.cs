using System;
using System.Management.Automation;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	[Cmdlet("Set", "DlpCompliancePolicy", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetDlpCompliancePolicy : SetCompliancePolicyBase
	{
		public SetDlpCompliancePolicy() : base(PolicyScenario.Dlp)
		{
		}
	}
}
