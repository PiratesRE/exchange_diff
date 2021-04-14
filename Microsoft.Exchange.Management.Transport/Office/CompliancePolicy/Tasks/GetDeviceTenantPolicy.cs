using System;
using System.Management.Automation;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	[Cmdlet("Get", "DeviceTenantPolicy", DefaultParameterSetName = "Identity")]
	public sealed class GetDeviceTenantPolicy : GetDevicePolicyBase
	{
		public GetDeviceTenantPolicy() : base(PolicyScenario.DeviceTenantConditionalAccess)
		{
		}
	}
}
