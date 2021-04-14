using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.MessagingPolicies.Journaling;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class NewJournalruleCommand : SyntheticCommandWithPipelineInput<TransportRule, TransportRule>
	{
		private NewJournalruleCommand() : base("New-Journalrule")
		{
		}

		public NewJournalruleCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual NewJournalruleCommand SetParameters(NewJournalruleCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual JournalRuleScope Scope
			{
				set
				{
					base.PowerSharpParameters["Scope"] = value;
				}
			}

			public virtual SmtpAddress? Recipient
			{
				set
				{
					base.PowerSharpParameters["Recipient"] = value;
				}
			}

			public virtual string JournalEmailAddress
			{
				set
				{
					base.PowerSharpParameters["JournalEmailAddress"] = ((value != null) ? new RecipientIdParameter(value) : null);
				}
			}

			public virtual bool Enabled
			{
				set
				{
					base.PowerSharpParameters["Enabled"] = value;
				}
			}

			public virtual SwitchParameter LawfulInterception
			{
				set
				{
					base.PowerSharpParameters["LawfulInterception"] = value;
				}
			}

			public virtual bool FullReport
			{
				set
				{
					base.PowerSharpParameters["FullReport"] = value;
				}
			}

			public virtual DateTime? ExpiryDate
			{
				set
				{
					base.PowerSharpParameters["ExpiryDate"] = value;
				}
			}

			public virtual string Organization
			{
				set
				{
					base.PowerSharpParameters["Organization"] = ((value != null) ? new OrganizationIdParameter(value) : null);
				}
			}

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
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
