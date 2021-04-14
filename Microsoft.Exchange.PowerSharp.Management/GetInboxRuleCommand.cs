using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Management.Common;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class GetInboxRuleCommand : SyntheticCommandWithPipelineInput<InboxRule, InboxRule>
	{
		private GetInboxRuleCommand() : base("Get-InboxRule")
		{
		}

		public GetInboxRuleCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual GetInboxRuleCommand SetParameters(GetInboxRuleCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual GetInboxRuleCommand SetParameters(GetInboxRuleCommand.IdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual string Mailbox
			{
				set
				{
					base.PowerSharpParameters["Mailbox"] = ((value != null) ? new MailboxIdParameter(value) : null);
				}
			}

			public virtual ExTimeZoneValue DescriptionTimeZone
			{
				set
				{
					base.PowerSharpParameters["DescriptionTimeZone"] = value;
				}
			}

			public virtual string DescriptionTimeFormat
			{
				set
				{
					base.PowerSharpParameters["DescriptionTimeFormat"] = value;
				}
			}

			public virtual SwitchParameter IncludeHidden
			{
				set
				{
					base.PowerSharpParameters["IncludeHidden"] = value;
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
					base.PowerSharpParameters["Identity"] = ((value != null) ? new InboxRuleIdParameter(value) : null);
				}
			}

			public virtual string Mailbox
			{
				set
				{
					base.PowerSharpParameters["Mailbox"] = ((value != null) ? new MailboxIdParameter(value) : null);
				}
			}

			public virtual ExTimeZoneValue DescriptionTimeZone
			{
				set
				{
					base.PowerSharpParameters["DescriptionTimeZone"] = value;
				}
			}

			public virtual string DescriptionTimeFormat
			{
				set
				{
					base.PowerSharpParameters["DescriptionTimeFormat"] = value;
				}
			}

			public virtual SwitchParameter IncludeHidden
			{
				set
				{
					base.PowerSharpParameters["IncludeHidden"] = value;
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
