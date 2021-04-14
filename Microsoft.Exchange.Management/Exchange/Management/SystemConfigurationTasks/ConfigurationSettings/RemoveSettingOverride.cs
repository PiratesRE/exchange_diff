using System;
using System.Management.Automation;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks.ConfigurationSettings
{
	[Cmdlet("Remove", "SettingOverride", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveSettingOverride : RemoveOverrideBase
	{
		protected override bool IsFlight
		{
			get
			{
				return false;
			}
		}
	}
}
