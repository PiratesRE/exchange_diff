using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks.ConfigurationSettings
{
	public abstract class RemoveOverrideBase : RemoveSystemConfigurationObjectTask<SettingOverrideIdParameter, SettingOverride>
	{
		protected abstract bool IsFlight { get; }

		protected override ObjectId RootId
		{
			get
			{
				return SettingOverride.GetContainerId(this.IsFlight);
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageRemoveExchangeSettings(this.Identity.ToString());
			}
		}
	}
}
