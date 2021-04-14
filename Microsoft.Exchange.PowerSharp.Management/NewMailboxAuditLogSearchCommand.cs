using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.SystemConfigurationTasks;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class NewMailboxAuditLogSearchCommand : SyntheticCommandWithPipelineInput<MailboxAuditLogSearch, MailboxAuditLogSearch>
	{
		private NewMailboxAuditLogSearchCommand() : base("New-MailboxAuditLogSearch")
		{
		}

		public NewMailboxAuditLogSearchCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual NewMailboxAuditLogSearchCommand SetParameters(NewMailboxAuditLogSearchCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual NewMailboxAuditLogSearchCommand SetParameters(NewMailboxAuditLogSearchCommand.IdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual MultiValuedProperty<MailboxIdParameter> Mailboxes
			{
				set
				{
					base.PowerSharpParameters["Mailboxes"] = value;
				}
			}

			public virtual MultiValuedProperty<AuditScopes> LogonTypes
			{
				set
				{
					base.PowerSharpParameters["LogonTypes"] = value;
				}
			}

			public virtual MultiValuedProperty<MailboxAuditOperations> Operations
			{
				set
				{
					base.PowerSharpParameters["Operations"] = value;
				}
			}

			public virtual SwitchParameter ShowDetails
			{
				set
				{
					base.PowerSharpParameters["ShowDetails"] = value;
				}
			}

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
				}
			}

			public virtual ExDateTime StartDate
			{
				set
				{
					base.PowerSharpParameters["StartDate"] = value;
				}
			}

			public virtual ExDateTime EndDate
			{
				set
				{
					base.PowerSharpParameters["EndDate"] = value;
				}
			}

			public virtual bool? ExternalAccess
			{
				set
				{
					base.PowerSharpParameters["ExternalAccess"] = value;
				}
			}

			public virtual MultiValuedProperty<SmtpAddress> StatusMailRecipients
			{
				set
				{
					base.PowerSharpParameters["StatusMailRecipients"] = value;
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
			public virtual string Organization
			{
				set
				{
					base.PowerSharpParameters["Organization"] = ((value != null) ? new OrganizationIdParameter(value) : null);
				}
			}

			public virtual MultiValuedProperty<MailboxIdParameter> Mailboxes
			{
				set
				{
					base.PowerSharpParameters["Mailboxes"] = value;
				}
			}

			public virtual MultiValuedProperty<AuditScopes> LogonTypes
			{
				set
				{
					base.PowerSharpParameters["LogonTypes"] = value;
				}
			}

			public virtual MultiValuedProperty<MailboxAuditOperations> Operations
			{
				set
				{
					base.PowerSharpParameters["Operations"] = value;
				}
			}

			public virtual SwitchParameter ShowDetails
			{
				set
				{
					base.PowerSharpParameters["ShowDetails"] = value;
				}
			}

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
				}
			}

			public virtual ExDateTime StartDate
			{
				set
				{
					base.PowerSharpParameters["StartDate"] = value;
				}
			}

			public virtual ExDateTime EndDate
			{
				set
				{
					base.PowerSharpParameters["EndDate"] = value;
				}
			}

			public virtual bool? ExternalAccess
			{
				set
				{
					base.PowerSharpParameters["ExternalAccess"] = value;
				}
			}

			public virtual MultiValuedProperty<SmtpAddress> StatusMailRecipients
			{
				set
				{
					base.PowerSharpParameters["StatusMailRecipients"] = value;
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
