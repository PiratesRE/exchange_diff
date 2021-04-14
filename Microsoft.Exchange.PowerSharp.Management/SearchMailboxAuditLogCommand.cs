using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SearchMailboxAuditLogCommand : SyntheticCommandWithPipelineInput<ADUser, ADUser>
	{
		private SearchMailboxAuditLogCommand() : base("Search-MailboxAuditLog")
		{
		}

		public SearchMailboxAuditLogCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SearchMailboxAuditLogCommand SetParameters(SearchMailboxAuditLogCommand.MultipleMailboxesSearchParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SearchMailboxAuditLogCommand SetParameters(SearchMailboxAuditLogCommand.IdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SearchMailboxAuditLogCommand SetParameters(SearchMailboxAuditLogCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class MultipleMailboxesSearchParameters : ParametersBase
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

			public virtual bool? ExternalAccess
			{
				set
				{
					base.PowerSharpParameters["ExternalAccess"] = value;
				}
			}

			public virtual int ResultSize
			{
				set
				{
					base.PowerSharpParameters["ResultSize"] = value;
				}
			}

			public virtual ExDateTime? StartDate
			{
				set
				{
					base.PowerSharpParameters["StartDate"] = value;
				}
			}

			public virtual ExDateTime? EndDate
			{
				set
				{
					base.PowerSharpParameters["EndDate"] = value;
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
			public virtual string Organization
			{
				set
				{
					base.PowerSharpParameters["Organization"] = ((value != null) ? new OrganizationIdParameter(value) : null);
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

			public virtual bool? ExternalAccess
			{
				set
				{
					base.PowerSharpParameters["ExternalAccess"] = value;
				}
			}

			public virtual int ResultSize
			{
				set
				{
					base.PowerSharpParameters["ResultSize"] = value;
				}
			}

			public virtual ExDateTime? StartDate
			{
				set
				{
					base.PowerSharpParameters["StartDate"] = value;
				}
			}

			public virtual ExDateTime? EndDate
			{
				set
				{
					base.PowerSharpParameters["EndDate"] = value;
				}
			}

			public virtual SwitchParameter ShowDetails
			{
				set
				{
					base.PowerSharpParameters["ShowDetails"] = value;
				}
			}

			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new MailboxIdParameter(value) : null);
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
