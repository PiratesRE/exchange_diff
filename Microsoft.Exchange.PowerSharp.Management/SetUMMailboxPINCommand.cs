using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetUMMailboxPINCommand : SyntheticCommandWithPipelineInputNoOutput<MailboxIdParameter>
	{
		private SetUMMailboxPINCommand() : base("Set-UMMailboxPIN")
		{
		}

		public SetUMMailboxPINCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetUMMailboxPINCommand SetParameters(SetUMMailboxPINCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetUMMailboxPINCommand SetParameters(SetUMMailboxPINCommand.IdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual string Pin
			{
				set
				{
					base.PowerSharpParameters["Pin"] = value;
				}
			}

			public virtual bool PinExpired
			{
				set
				{
					base.PowerSharpParameters["PinExpired"] = value;
				}
			}

			public virtual bool LockedOut
			{
				set
				{
					base.PowerSharpParameters["LockedOut"] = value;
				}
			}

			public virtual string NotifyEmail
			{
				set
				{
					base.PowerSharpParameters["NotifyEmail"] = value;
				}
			}

			public virtual bool SendEmail
			{
				set
				{
					base.PowerSharpParameters["SendEmail"] = value;
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

			public virtual string Pin
			{
				set
				{
					base.PowerSharpParameters["Pin"] = value;
				}
			}

			public virtual bool PinExpired
			{
				set
				{
					base.PowerSharpParameters["PinExpired"] = value;
				}
			}

			public virtual bool LockedOut
			{
				set
				{
					base.PowerSharpParameters["LockedOut"] = value;
				}
			}

			public virtual string NotifyEmail
			{
				set
				{
					base.PowerSharpParameters["NotifyEmail"] = value;
				}
			}

			public virtual bool SendEmail
			{
				set
				{
					base.PowerSharpParameters["SendEmail"] = value;
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
