using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Management.RecipientTasks;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class NewUMCallAnsweringRuleCommand : SyntheticCommandWithPipelineInput<UMCallAnsweringRule, UMCallAnsweringRule>
	{
		private NewUMCallAnsweringRuleCommand() : base("New-UMCallAnsweringRule")
		{
		}

		public NewUMCallAnsweringRuleCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual NewUMCallAnsweringRuleCommand SetParameters(NewUMCallAnsweringRuleCommand.DefaultParameters parameters)
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

			public virtual string Organization
			{
				set
				{
					base.PowerSharpParameters["Organization"] = ((value != null) ? new OrganizationIdParameter(value) : null);
				}
			}

			public virtual MultiValuedProperty<CallerIdItem> CallerIds
			{
				set
				{
					base.PowerSharpParameters["CallerIds"] = value;
				}
			}

			public virtual bool CallersCanInterruptGreeting
			{
				set
				{
					base.PowerSharpParameters["CallersCanInterruptGreeting"] = value;
				}
			}

			public virtual bool CheckAutomaticReplies
			{
				set
				{
					base.PowerSharpParameters["CheckAutomaticReplies"] = value;
				}
			}

			public virtual MultiValuedProperty<string> ExtensionsDialed
			{
				set
				{
					base.PowerSharpParameters["ExtensionsDialed"] = value;
				}
			}

			public virtual MultiValuedProperty<KeyMapping> KeyMappings
			{
				set
				{
					base.PowerSharpParameters["KeyMappings"] = value;
				}
			}

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
				}
			}

			public virtual int Priority
			{
				set
				{
					base.PowerSharpParameters["Priority"] = value;
				}
			}

			public virtual int ScheduleStatus
			{
				set
				{
					base.PowerSharpParameters["ScheduleStatus"] = value;
				}
			}

			public virtual TimeOfDay TimeOfDay
			{
				set
				{
					base.PowerSharpParameters["TimeOfDay"] = value;
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
