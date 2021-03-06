using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Management.RecipientTasks;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class UpdateSafeListCommand : SyntheticCommandWithPipelineInput<ADRecipient, ADRecipient>
	{
		private UpdateSafeListCommand() : base("Update-SafeList")
		{
		}

		public UpdateSafeListCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual UpdateSafeListCommand SetParameters(UpdateSafeListCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual UpdateSafeListCommand SetParameters(UpdateSafeListCommand.IdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual UpdateType Type
			{
				set
				{
					base.PowerSharpParameters["Type"] = value;
				}
			}

			public virtual SwitchParameter IncludeDomains
			{
				set
				{
					base.PowerSharpParameters["IncludeDomains"] = value;
				}
			}

			public virtual SwitchParameter EnsureJunkEmailRule
			{
				set
				{
					base.PowerSharpParameters["EnsureJunkEmailRule"] = value;
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

		public class IdentityParameters : ParametersBase
		{
			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new MailboxIdParameter(value) : null);
				}
			}

			public virtual UpdateType Type
			{
				set
				{
					base.PowerSharpParameters["Type"] = value;
				}
			}

			public virtual SwitchParameter IncludeDomains
			{
				set
				{
					base.PowerSharpParameters["IncludeDomains"] = value;
				}
			}

			public virtual SwitchParameter EnsureJunkEmailRule
			{
				set
				{
					base.PowerSharpParameters["EnsureJunkEmailRule"] = value;
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
