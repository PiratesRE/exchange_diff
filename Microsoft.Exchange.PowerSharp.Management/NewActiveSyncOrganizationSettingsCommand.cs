using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class NewActiveSyncOrganizationSettingsCommand : SyntheticCommandWithPipelineInput<ActiveSyncOrganizationSettings, ActiveSyncOrganizationSettings>
	{
		private NewActiveSyncOrganizationSettingsCommand() : base("New-ActiveSyncOrganizationSettings")
		{
		}

		public NewActiveSyncOrganizationSettingsCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual NewActiveSyncOrganizationSettingsCommand SetParameters(NewActiveSyncOrganizationSettingsCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
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

			public virtual bool AllowAccessForUnSupportedPlatform
			{
				set
				{
					base.PowerSharpParameters["AllowAccessForUnSupportedPlatform"] = value;
				}
			}

			public virtual string Organization
			{
				set
				{
					base.PowerSharpParameters["Organization"] = ((value != null) ? new OrganizationIdParameter(value) : null);
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
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
