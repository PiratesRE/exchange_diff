using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class GetMailboxFolderStatisticsCommand : SyntheticCommandWithPipelineInput<MailboxFolderConfiguration, MailboxFolderConfiguration>
	{
		private GetMailboxFolderStatisticsCommand() : base("Get-MailboxFolderStatistics")
		{
		}

		public GetMailboxFolderStatisticsCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual GetMailboxFolderStatisticsCommand SetParameters(GetMailboxFolderStatisticsCommand.IdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual GetMailboxFolderStatisticsCommand SetParameters(GetMailboxFolderStatisticsCommand.AuditLogParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual GetMailboxFolderStatisticsCommand SetParameters(GetMailboxFolderStatisticsCommand.DefaultParameters parameters)
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
					base.PowerSharpParameters["Identity"] = ((value != null) ? new MailboxOrMailUserIdParameter(value) : null);
				}
			}

			public virtual SwitchParameter Archive
			{
				set
				{
					base.PowerSharpParameters["Archive"] = value;
				}
			}

			public virtual ElcFolderType? FolderScope
			{
				set
				{
					base.PowerSharpParameters["FolderScope"] = value;
				}
			}

			public virtual SwitchParameter IncludeOldestAndNewestItems
			{
				set
				{
					base.PowerSharpParameters["IncludeOldestAndNewestItems"] = value;
				}
			}

			public virtual SwitchParameter IncludeAnalysis
			{
				set
				{
					base.PowerSharpParameters["IncludeAnalysis"] = value;
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

		public class AuditLogParameters : ParametersBase
		{
			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new MailboxOrMailUserIdParameter(value) : null);
				}
			}

			public virtual SwitchParameter AuditLog
			{
				set
				{
					base.PowerSharpParameters["AuditLog"] = value;
				}
			}

			public virtual ElcFolderType? FolderScope
			{
				set
				{
					base.PowerSharpParameters["FolderScope"] = value;
				}
			}

			public virtual SwitchParameter IncludeOldestAndNewestItems
			{
				set
				{
					base.PowerSharpParameters["IncludeOldestAndNewestItems"] = value;
				}
			}

			public virtual SwitchParameter IncludeAnalysis
			{
				set
				{
					base.PowerSharpParameters["IncludeAnalysis"] = value;
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
			public virtual ElcFolderType? FolderScope
			{
				set
				{
					base.PowerSharpParameters["FolderScope"] = value;
				}
			}

			public virtual SwitchParameter IncludeOldestAndNewestItems
			{
				set
				{
					base.PowerSharpParameters["IncludeOldestAndNewestItems"] = value;
				}
			}

			public virtual SwitchParameter IncludeAnalysis
			{
				set
				{
					base.PowerSharpParameters["IncludeAnalysis"] = value;
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
