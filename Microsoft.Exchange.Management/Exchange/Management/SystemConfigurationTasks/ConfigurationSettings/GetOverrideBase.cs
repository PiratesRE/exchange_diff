using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks.ConfigurationSettings
{
	public abstract class GetOverrideBase : GetSystemConfigurationObjectTask<SettingOverrideIdParameter, SettingOverride>
	{
		protected abstract bool IsFlight { get; }

		protected override ObjectId RootId
		{
			get
			{
				return SettingOverride.GetContainerId(this.IsFlight);
			}
		}

		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception) || typeof(ConfigurationSettingsException).IsInstanceOfType(exception);
		}
	}
}
