using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class GetMobileDeviceStatisticsCommand : SyntheticCommandWithPipelineInput<MobileDevice, MobileDevice>
	{
		private GetMobileDeviceStatisticsCommand() : base("Get-MobileDeviceStatistics")
		{
		}

		public GetMobileDeviceStatisticsCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual GetMobileDeviceStatisticsCommand SetParameters(GetMobileDeviceStatisticsCommand.MailboxParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual GetMobileDeviceStatisticsCommand SetParameters(GetMobileDeviceStatisticsCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual GetMobileDeviceStatisticsCommand SetParameters(GetMobileDeviceStatisticsCommand.IdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class MailboxParameters : ParametersBase
		{
			public virtual string Mailbox
			{
				set
				{
					base.PowerSharpParameters["Mailbox"] = ((value != null) ? new MailboxIdParameter(value) : null);
				}
			}

			public virtual SwitchParameter GetMailboxLog
			{
				set
				{
					base.PowerSharpParameters["GetMailboxLog"] = value;
				}
			}

			public virtual MultiValuedProperty<string> NotificationEmailAddresses
			{
				set
				{
					base.PowerSharpParameters["NotificationEmailAddresses"] = value;
				}
			}

			public virtual SwitchParameter ActiveSync
			{
				set
				{
					base.PowerSharpParameters["ActiveSync"] = value;
				}
			}

			public virtual SwitchParameter OWAforDevices
			{
				set
				{
					base.PowerSharpParameters["OWAforDevices"] = value;
				}
			}

			public virtual SwitchParameter ShowRecoveryPassword
			{
				set
				{
					base.PowerSharpParameters["ShowRecoveryPassword"] = value;
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
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual SwitchParameter GetMailboxLog
			{
				set
				{
					base.PowerSharpParameters["GetMailboxLog"] = value;
				}
			}

			public virtual MultiValuedProperty<string> NotificationEmailAddresses
			{
				set
				{
					base.PowerSharpParameters["NotificationEmailAddresses"] = value;
				}
			}

			public virtual SwitchParameter ActiveSync
			{
				set
				{
					base.PowerSharpParameters["ActiveSync"] = value;
				}
			}

			public virtual SwitchParameter OWAforDevices
			{
				set
				{
					base.PowerSharpParameters["OWAforDevices"] = value;
				}
			}

			public virtual SwitchParameter ShowRecoveryPassword
			{
				set
				{
					base.PowerSharpParameters["ShowRecoveryPassword"] = value;
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
		}

		public class IdentityParameters : ParametersBase
		{
			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new MobileDeviceIdParameter(value) : null);
				}
			}

			public virtual SwitchParameter GetMailboxLog
			{
				set
				{
					base.PowerSharpParameters["GetMailboxLog"] = value;
				}
			}

			public virtual MultiValuedProperty<string> NotificationEmailAddresses
			{
				set
				{
					base.PowerSharpParameters["NotificationEmailAddresses"] = value;
				}
			}

			public virtual SwitchParameter ActiveSync
			{
				set
				{
					base.PowerSharpParameters["ActiveSync"] = value;
				}
			}

			public virtual SwitchParameter OWAforDevices
			{
				set
				{
					base.PowerSharpParameters["OWAforDevices"] = value;
				}
			}

			public virtual SwitchParameter ShowRecoveryPassword
			{
				set
				{
					base.PowerSharpParameters["ShowRecoveryPassword"] = value;
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
		}
	}
}
