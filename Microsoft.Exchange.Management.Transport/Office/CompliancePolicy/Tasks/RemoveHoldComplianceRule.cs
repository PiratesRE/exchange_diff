using System;
using System.Management.Automation;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	[Cmdlet("Remove", "HoldComplianceRule", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveHoldComplianceRule : RemoveComplianceRuleBase
	{
		public RemoveHoldComplianceRule() : base(PolicyScenario.Hold)
		{
		}
	}
}
