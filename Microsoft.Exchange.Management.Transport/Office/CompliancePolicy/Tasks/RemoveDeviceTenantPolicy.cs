using System;
using System.Management.Automation;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	[Cmdlet("Remove", "DeviceTenantPolicy", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveDeviceTenantPolicy : RemoveDevicePolicyBase
	{
		public RemoveDeviceTenantPolicy() : base(PolicyScenario.DeviceTenantConditionalAccess)
		{
		}
	}
}
