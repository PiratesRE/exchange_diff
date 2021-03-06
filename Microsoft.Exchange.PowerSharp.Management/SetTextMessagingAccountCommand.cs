using System;
using System.Globalization;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetTextMessagingAccountCommand : SyntheticCommandWithPipelineInputNoOutput<TextMessagingAccount>
	{
		private SetTextMessagingAccountCommand() : base("Set-TextMessagingAccount")
		{
		}

		public SetTextMessagingAccountCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetTextMessagingAccountCommand SetParameters(SetTextMessagingAccountCommand.IdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetTextMessagingAccountCommand SetParameters(SetTextMessagingAccountCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class IdentityParameters : ParametersBase
		{
			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new MailboxIdParameter(value) : null);
				}
			}

			public virtual SwitchParameter IgnoreDefaultScope
			{
				set
				{
					base.PowerSharpParameters["IgnoreDefaultScope"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual RegionInfo CountryRegionId
			{
				set
				{
					base.PowerSharpParameters["CountryRegionId"] = value;
				}
			}

			public virtual int MobileOperatorId
			{
				set
				{
					base.PowerSharpParameters["MobileOperatorId"] = value;
				}
			}

			public virtual E164Number NotificationPhoneNumber
			{
				set
				{
					base.PowerSharpParameters["NotificationPhoneNumber"] = value;
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
			public virtual SwitchParameter IgnoreDefaultScope
			{
				set
				{
					base.PowerSharpParameters["IgnoreDefaultScope"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual RegionInfo CountryRegionId
			{
				set
				{
					base.PowerSharpParameters["CountryRegionId"] = value;
				}
			}

			public virtual int MobileOperatorId
			{
				set
				{
					base.PowerSharpParameters["MobileOperatorId"] = value;
				}
			}

			public virtual E164Number NotificationPhoneNumber
			{
				set
				{
					base.PowerSharpParameters["NotificationPhoneNumber"] = value;
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
