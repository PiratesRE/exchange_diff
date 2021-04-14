using System;
using System.Management.Automation;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	[Cmdlet("Get", "DeviceConfigurationPolicy", DefaultParameterSetName = "Identity")]
	public sealed class GetDeviceConfigurationPolicy : GetDevicePolicyBase
	{
		public GetDeviceConfigurationPolicy() : base(PolicyScenario.DeviceSettings)
		{
		}
	}
}
