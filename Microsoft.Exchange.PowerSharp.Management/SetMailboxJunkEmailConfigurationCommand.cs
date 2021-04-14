using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetMailboxJunkEmailConfigurationCommand : SyntheticCommandWithPipelineInputNoOutput<MailboxJunkEmailConfiguration>
	{
		private SetMailboxJunkEmailConfigurationCommand() : base("Set-MailboxJunkEmailConfiguration")
		{
		}

		public SetMailboxJunkEmailConfigurationCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetMailboxJunkEmailConfigurationCommand SetParameters(SetMailboxJunkEmailConfigurationCommand.IdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetMailboxJunkEmailConfigurationCommand SetParameters(SetMailboxJunkEmailConfigurationCommand.DefaultParameters parameters)
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

			public virtual bool Enabled
			{
				set
				{
					base.PowerSharpParameters["Enabled"] = value;
				}
			}

			public virtual bool TrustedListsOnly
			{
				set
				{
					base.PowerSharpParameters["TrustedListsOnly"] = value;
				}
			}

			public virtual bool ContactsTrusted
			{
				set
				{
					base.PowerSharpParameters["ContactsTrusted"] = value;
				}
			}

			public virtual MultiValuedProperty<string> TrustedSendersAndDomains
			{
				set
				{
					base.PowerSharpParameters["TrustedSendersAndDomains"] = value;
				}
			}

			public virtual MultiValuedProperty<string> BlockedSendersAndDomains
			{
				set
				{
					base.PowerSharpParameters["BlockedSendersAndDomains"] = value;
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

			public virtual bool Enabled
			{
				set
				{
					base.PowerSharpParameters["Enabled"] = value;
				}
			}

			public virtual bool TrustedListsOnly
			{
				set
				{
					base.PowerSharpParameters["TrustedListsOnly"] = value;
				}
			}

			public virtual bool ContactsTrusted
			{
				set
				{
					base.PowerSharpParameters["ContactsTrusted"] = value;
				}
			}

			public virtual MultiValuedProperty<string> TrustedSendersAndDomains
			{
				set
				{
					base.PowerSharpParameters["TrustedSendersAndDomains"] = value;
				}
			}

			public virtual MultiValuedProperty<string> BlockedSendersAndDomains
			{
				set
				{
					base.PowerSharpParameters["BlockedSendersAndDomains"] = value;
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
