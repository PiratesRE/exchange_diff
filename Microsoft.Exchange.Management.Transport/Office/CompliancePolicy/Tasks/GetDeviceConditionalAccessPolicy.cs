using System;
using System.Management.Automation;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	[Cmdlet("Get", "DeviceConditionalAccessPolicy", DefaultParameterSetName = "Identity")]
	public sealed class GetDeviceConditionalAccessPolicy : GetDevicePolicyBase
	{
		public GetDeviceConditionalAccessPolicy() : base(PolicyScenario.DeviceConditionalAccess)
		{
		}
	}
}
