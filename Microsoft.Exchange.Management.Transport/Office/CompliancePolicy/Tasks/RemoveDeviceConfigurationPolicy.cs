using System;
using System.Management.Automation;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	[Cmdlet("Remove", "DeviceConfigurationPolicy", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveDeviceConfigurationPolicy : RemoveDevicePolicyBase
	{
		public RemoveDeviceConfigurationPolicy() : base(PolicyScenario.DeviceSettings)
		{
		}
	}
}
