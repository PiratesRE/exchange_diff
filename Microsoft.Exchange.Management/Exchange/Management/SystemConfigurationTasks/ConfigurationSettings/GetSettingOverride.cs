using System;
using System.Management.Automation;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks.ConfigurationSettings
{
	[Cmdlet("Get", "SettingOverride", DefaultParameterSetName = "Identity")]
	public sealed class GetSettingOverride : GetOverrideBase
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
