using System;
using System.Management.Automation;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks.ConfigurationSettings
{
	[Cmdlet("Set", "SettingOverride", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class SetSettingOverride : SetOverrideBase
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
