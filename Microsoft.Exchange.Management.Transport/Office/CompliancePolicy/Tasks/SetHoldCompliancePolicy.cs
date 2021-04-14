using System;
using System.Management.Automation;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	[Cmdlet("Set", "HoldCompliancePolicy", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetHoldCompliancePolicy : SetCompliancePolicyBase
	{
		public SetHoldCompliancePolicy() : base(PolicyScenario.Hold)
		{
		}
	}
}
