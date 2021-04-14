using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetActiveSyncOrganizationSettingsCommand : SyntheticCommandWithPipelineInputNoOutput<ActiveSyncOrganizationSettings>
	{
		private SetActiveSyncOrganizationSettingsCommand() : base("Set-ActiveSyncOrganizationSettings")
		{
		}

		public SetActiveSyncOrganizationSettingsCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetActiveSyncOrganizationSettingsCommand SetParameters(SetActiveSyncOrganizationSettingsCommand.ParameterSetAddDeviceFilterRuleForAllDevicesParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetActiveSyncOrganizationSettingsCommand SetParameters(SetActiveSyncOrganizationSettingsCommand.ParameterSetRemoveDeviceFilterParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetActiveSyncOrganizationSettingsCommand SetParameters(SetActiveSyncOrganizationSettingsCommand.ParameterSetRemoveDeviceFilterRuleParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetActiveSyncOrganizationSettingsCommand SetParameters(SetActiveSyncOrganizationSettingsCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetActiveSyncOrganizationSettingsCommand SetParameters(SetActiveSyncOrganizationSettingsCommand.ParameterSetAddDeviceFilterRuleParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class ParameterSetAddDeviceFilterRuleForAllDevicesParameters : ParametersBase
		{
			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new ActiveSyncOrganizationSettingsIdParameter(value) : null);
				}
			}

			public virtual SwitchParameter AddDeviceFilterRuleForAllDevices
			{
				set
				{
					base.PowerSharpParameters["AddDeviceFilterRuleForAllDevices"] = value;
				}
			}

			public virtual string DeviceFilterName
			{
				set
				{
					base.PowerSharpParameters["DeviceFilterName"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual DeviceAccessLevel DefaultAccessLevel
			{
				set
				{
					base.PowerSharpParameters["DefaultAccessLevel"] = value;
				}
			}

			public virtual string UserMailInsert
			{
				set
				{
					base.PowerSharpParameters["UserMailInsert"] = value;
				}
			}

			public virtual bool AllowAccessForUnSupportedPlatform
			{
				set
				{
					base.PowerSharpParameters["AllowAccessForUnSupportedPlatform"] = value;
				}
			}

			public virtual MultiValuedProperty<SmtpAddress> AdminMailRecipients
			{
				set
				{
					base.PowerSharpParameters["AdminMailRecipients"] = value;
				}
			}

			public virtual string OtaNotificationMailInsert
			{
				set
				{
					base.PowerSharpParameters["OtaNotificationMailInsert"] = value;
				}
			}

			public virtual ActiveSyncDeviceFilterArray DeviceFiltering
			{
				set
				{
					base.PowerSharpParameters["DeviceFiltering"] = value;
				}
			}

			public virtual SwitchParameter Verbose
			{
				set
				{
					base.PowerSharpParameters["Verbose"] = value;
				}
			}

			public virtual SwitchParameter Debug
			{
				set
				{
					base.PowerSharpParameters["Debug"] = value;
				}
			}

			public virtual ActionPreference ErrorAction
			{
				set
				{
					base.PowerSharpParameters["ErrorAction"] = value;
				}
			}

			public virtual ActionPreference WarningAction
			{
				set
				{
					base.PowerSharpParameters["WarningAction"] = value;
				}
			}

			public virtual SwitchParameter WhatIf
			{
				set
				{
					base.PowerSharpParameters["WhatIf"] = value;
				}
			}
		}

		public class ParameterSetRemoveDeviceFilterParameters : ParametersBase
		{
			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new ActiveSyncOrganizationSettingsIdParameter(value) : null);
				}
			}

			public virtual SwitchParameter RemoveDeviceFilter
			{
				set
				{
					base.PowerSharpParameters["RemoveDeviceFilter"] = value;
				}
			}

			public virtual string DeviceFilterName
			{
				set
				{
					base.PowerSharpParameters["DeviceFilterName"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual DeviceAccessLevel DefaultAccessLevel
			{
				set
				{
					base.PowerSharpParameters["DefaultAccessLevel"] = value;
				}
			}

			public virtual string UserMailInsert
			{
				set
				{
					base.PowerSharpParameters["UserMailInsert"] = value;
				}
			}

			public virtual bool AllowAccessForUnSupportedPlatform
			{
				set
				{
					base.PowerSharpParameters["AllowAccessForUnSupportedPlatform"] = value;
				}
			}

			public virtual MultiValuedProperty<SmtpAddress> AdminMailRecipients
			{
				set
				{
					base.PowerSharpParameters["AdminMailRecipients"] = value;
				}
			}

			public virtual string OtaNotificationMailInsert
			{
				set
				{
					base.PowerSharpParameters["OtaNotificationMailInsert"] = value;
				}
			}

			public virtual ActiveSyncDeviceFilterArray DeviceFiltering
			{
				set
				{
					base.PowerSharpParameters["DeviceFiltering"] = value;
				}
			}

			public virtual SwitchParameter Verbose
			{
				set
				{
					base.PowerSharpParameters["Verbose"] = value;
				}
			}

			public virtual SwitchParameter Debug
			{
				set
				{
					base.PowerSharpParameters["Debug"] = value;
				}
			}

			public virtual ActionPreference ErrorAction
			{
				set
				{
					base.PowerSharpParameters["ErrorAction"] = value;
				}
			}

			public virtual ActionPreference WarningAction
			{
				set
				{
					base.PowerSharpParameters["WarningAction"] = value;
				}
			}

			public virtual SwitchParameter WhatIf
			{
				set
				{
					base.PowerSharpParameters["WhatIf"] = value;
				}
			}
		}

		public class ParameterSetRemoveDeviceFilterRuleParameters : ParametersBase
		{
			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new ActiveSyncOrganizationSettingsIdParameter(value) : null);
				}
			}

			public virtual SwitchParameter RemoveDeviceFilterRule
			{
				set
				{
					base.PowerSharpParameters["RemoveDeviceFilterRule"] = value;
				}
			}

			public virtual string DeviceFilterName
			{
				set
				{
					base.PowerSharpParameters["DeviceFilterName"] = value;
				}
			}

			public virtual string DeviceFilterRuleName
			{
				set
				{
					base.PowerSharpParameters["DeviceFilterRuleName"] = value;
				}
			}

			public virtual DeviceAccessCharacteristic DeviceFilterCharacteristic
			{
				set
				{
					base.PowerSharpParameters["DeviceFilterCharacteristic"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual DeviceAccessLevel DefaultAccessLevel
			{
				set
				{
					base.PowerSharpParameters["DefaultAccessLevel"] = value;
				}
			}

			public virtual string UserMailInsert
			{
				set
				{
					base.PowerSharpParameters["UserMailInsert"] = value;
				}
			}

			public virtual bool AllowAccessForUnSupportedPlatform
			{
				set
				{
					base.PowerSharpParameters["AllowAccessForUnSupportedPlatform"] = value;
				}
			}

			public virtual MultiValuedProperty<SmtpAddress> AdminMailRecipients
			{
				set
				{
					base.PowerSharpParameters["AdminMailRecipients"] = value;
				}
			}

			public virtual string OtaNotificationMailInsert
			{
				set
				{
					base.PowerSharpParameters["OtaNotificationMailInsert"] = value;
				}
			}

			public virtual ActiveSyncDeviceFilterArray DeviceFiltering
			{
				set
				{
					base.PowerSharpParameters["DeviceFiltering"] = value;
				}
			}

			public virtual SwitchParameter Verbose
			{
				set
				{
					base.PowerSharpParameters["Verbose"] = value;
				}
			}

			public virtual SwitchParameter Debug
			{
				set
				{
					base.PowerSharpParameters["Debug"] = value;
				}
			}

			public virtual ActionPreference ErrorAction
			{
				set
				{
					base.PowerSharpParameters["ErrorAction"] = value;
				}
			}

			public virtual ActionPreference WarningAction
			{
				set
				{
					base.PowerSharpParameters["WarningAction"] = value;
				}
			}

			public virtual SwitchParameter WhatIf
			{
				set
				{
					base.PowerSharpParameters["WhatIf"] = value;
				}
			}
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new ActiveSyncOrganizationSettingsIdParameter(value) : null);
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual DeviceAccessLevel DefaultAccessLevel
			{
				set
				{
					base.PowerSharpParameters["DefaultAccessLevel"] = value;
				}
			}

			public virtual string UserMailInsert
			{
				set
				{
					base.PowerSharpParameters["UserMailInsert"] = value;
				}
			}

			public virtual bool AllowAccessForUnSupportedPlatform
			{
				set
				{
					base.PowerSharpParameters["AllowAccessForUnSupportedPlatform"] = value;
				}
			}

			public virtual MultiValuedProperty<SmtpAddress> AdminMailRecipients
			{
				set
				{
					base.PowerSharpParameters["AdminMailRecipients"] = value;
				}
			}

			public virtual string OtaNotificationMailInsert
			{
				set
				{
					base.PowerSharpParameters["OtaNotificationMailInsert"] = value;
				}
			}

			public virtual ActiveSyncDeviceFilterArray DeviceFiltering
			{
				set
				{
					base.PowerSharpParameters["DeviceFiltering"] = value;
				}
			}

			public virtual SwitchParameter Verbose
			{
				set
				{
					base.PowerSharpParameters["Verbose"] = value;
				}
			}

			public virtual SwitchParameter Debug
			{
				set
				{
					base.PowerSharpParameters["Debug"] = value;
				}
			}

			public virtual ActionPreference ErrorAction
			{
				set
				{
					base.PowerSharpParameters["ErrorAction"] = value;
				}
			}

			public virtual ActionPreference WarningAction
			{
				set
				{
					base.PowerSharpParameters["WarningAction"] = value;
				}
			}

			public virtual SwitchParameter WhatIf
			{
				set
				{
					base.PowerSharpParameters["WhatIf"] = value;
				}
			}
		}

		public class ParameterSetAddDeviceFilterRuleParameters : ParametersBase
		{
			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new ActiveSyncOrganizationSettingsIdParameter(value) : null);
				}
			}

			public virtual SwitchParameter AddDeviceFilterRule
			{
				set
				{
					base.PowerSharpParameters["AddDeviceFilterRule"] = value;
				}
			}

			public virtual string DeviceFilterName
			{
				set
				{
					base.PowerSharpParameters["DeviceFilterName"] = value;
				}
			}

			public virtual string DeviceFilterRuleName
			{
				set
				{
					base.PowerSharpParameters["DeviceFilterRuleName"] = value;
				}
			}

			public virtual DeviceAccessCharacteristic DeviceFilterCharacteristic
			{
				set
				{
					base.PowerSharpParameters["DeviceFilterCharacteristic"] = value;
				}
			}

			public virtual DeviceFilterOperator DeviceFilterOperator
			{
				set
				{
					base.PowerSharpParameters["DeviceFilterOperator"] = value;
				}
			}

			public virtual string DeviceFilterValue
			{
				set
				{
					base.PowerSharpParameters["DeviceFilterValue"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual DeviceAccessLevel DefaultAccessLevel
			{
				set
				{
					base.PowerSharpParameters["DefaultAccessLevel"] = value;
				}
			}

			public virtual string UserMailInsert
			{
				set
				{
					base.PowerSharpParameters["UserMailInsert"] = value;
				}
			}

			public virtual bool AllowAccessForUnSupportedPlatform
			{
				set
				{
					base.PowerSharpParameters["AllowAccessForUnSupportedPlatform"] = value;
				}
			}

			public virtual MultiValuedProperty<SmtpAddress> AdminMailRecipients
			{
				set
				{
					base.PowerSharpParameters["AdminMailRecipients"] = value;
				}
			}

			public virtual string OtaNotificationMailInsert
			{
				set
				{
					base.PowerSharpParameters["OtaNotificationMailInsert"] = value;
				}
			}

			public virtual ActiveSyncDeviceFilterArray DeviceFiltering
			{
				set
				{
					base.PowerSharpParameters["DeviceFiltering"] = value;
				}
			}

			public virtual SwitchParameter Verbose
			{
				set
				{
					base.PowerSharpParameters["Verbose"] = value;
				}
			}

			public virtual SwitchParameter Debug
			{
				set
				{
					base.PowerSharpParameters["Debug"] = value;
				}
			}

			public virtual ActionPreference ErrorAction
			{
				set
				{
					base.PowerSharpParameters["ErrorAction"] = value;
				}
			}

			public virtual ActionPreference WarningAction
			{
				set
				{
					base.PowerSharpParameters["WarningAction"] = value;
				}
			}

			public virtual SwitchParameter WhatIf
			{
				set
				{
					base.PowerSharpParameters["WhatIf"] = value;
				}
			}
		}
	}
}
