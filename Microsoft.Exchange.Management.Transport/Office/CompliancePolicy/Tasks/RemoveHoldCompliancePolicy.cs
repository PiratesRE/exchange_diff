using System;
using System.Management.Automation;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	[Cmdlet("Remove", "HoldCompliancePolicy", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveHoldCompliancePolicy : RemoveCompliancePolicyBase
	{
		public RemoveHoldCompliancePolicy() : base(PolicyScenario.Hold)
		{
		}
	}
}
