using System;
using System.Management.Automation;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Management.PushNotifications;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class GetPushNotificationAppCommand : SyntheticCommandWithPipelineInput<PushNotificationAppPresentationObject, PushNotificationAppPresentationObject>
	{
		private GetPushNotificationAppCommand() : base("Get-PushNotificationApp")
		{
		}

		public GetPushNotificationAppCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual GetPushNotificationAppCommand SetParameters(GetPushNotificationAppCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual GetPushNotificationAppCommand SetParameters(GetPushNotificationAppCommand.IdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual PushNotificationPlatform Platform
			{
				set
				{
					base.PowerSharpParameters["Platform"] = value;
				}
			}

			public virtual bool Enabled
			{
				set
				{
					base.PowerSharpParameters["Enabled"] = value;
				}
			}

			public virtual SwitchParameter UseClearTextAuthenticationKeys
			{
				set
				{
					base.PowerSharpParameters["UseClearTextAuthenticationKeys"] = value;
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
					base.PowerSharpParameters["Identity"] = ((value != null) ? new PushNotificationAppIdParameter(value) : null);
				}
			}

			public virtual PushNotificationPlatform Platform
			{
				set
				{
					base.PowerSharpParameters["Platform"] = value;
				}
			}

			public virtual bool Enabled
			{
				set
				{
					base.PowerSharpParameters["Enabled"] = value;
				}
			}

			public virtual SwitchParameter UseClearTextAuthenticationKeys
			{
				set
				{
					base.PowerSharpParameters["UseClearTextAuthenticationKeys"] = value;
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
