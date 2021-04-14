using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Mapi;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class GetMailboxStatisticsCommand : SyntheticCommandWithPipelineInput<MailboxStatistics, MailboxStatistics>
	{
		private GetMailboxStatisticsCommand() : base("Get-MailboxStatistics")
		{
		}

		public GetMailboxStatisticsCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual GetMailboxStatisticsCommand SetParameters(GetMailboxStatisticsCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual GetMailboxStatisticsCommand SetParameters(GetMailboxStatisticsCommand.IdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual GetMailboxStatisticsCommand SetParameters(GetMailboxStatisticsCommand.AuditLogParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual GetMailboxStatisticsCommand SetParameters(GetMailboxStatisticsCommand.DatabaseParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual GetMailboxStatisticsCommand SetParameters(GetMailboxStatisticsCommand.ServerParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual SwitchParameter IncludeMoveHistory
			{
				set
				{
					base.PowerSharpParameters["IncludeMoveHistory"] = value;
				}
			}

			public virtual SwitchParameter IncludeMoveReport
			{
				set
				{
					base.PowerSharpParameters["IncludeMoveReport"] = value;
				}
			}

			public virtual SwitchParameter NoADLookup
			{
				set
				{
					base.PowerSharpParameters["NoADLookup"] = value;
				}
			}

			public virtual SwitchParameter IncludeQuarantineDetails
			{
				set
				{
					base.PowerSharpParameters["IncludeQuarantineDetails"] = value;
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
			public virtual SwitchParameter Archive
			{
				set
				{
					base.PowerSharpParameters["Archive"] = value;
				}
			}

			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new GeneralMailboxOrMailUserIdParameter(value) : null);
				}
			}

			public virtual ServerIdParameter CopyOnServer
			{
				set
				{
					base.PowerSharpParameters["CopyOnServer"] = value;
				}
			}

			public virtual SwitchParameter IncludeMoveHistory
			{
				set
				{
					base.PowerSharpParameters["IncludeMoveHistory"] = value;
				}
			}

			public virtual SwitchParameter IncludeMoveReport
			{
				set
				{
					base.PowerSharpParameters["IncludeMoveReport"] = value;
				}
			}

			public virtual SwitchParameter NoADLookup
			{
				set
				{
					base.PowerSharpParameters["NoADLookup"] = value;
				}
			}

			public virtual SwitchParameter IncludeQuarantineDetails
			{
				set
				{
					base.PowerSharpParameters["IncludeQuarantineDetails"] = value;
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
			public virtual SwitchParameter AuditLog
			{
				set
				{
					base.PowerSharpParameters["AuditLog"] = value;
				}
			}

			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new GeneralMailboxOrMailUserIdParameter(value) : null);
				}
			}

			public virtual SwitchParameter IncludeMoveHistory
			{
				set
				{
					base.PowerSharpParameters["IncludeMoveHistory"] = value;
				}
			}

			public virtual SwitchParameter IncludeMoveReport
			{
				set
				{
					base.PowerSharpParameters["IncludeMoveReport"] = value;
				}
			}

			public virtual SwitchParameter NoADLookup
			{
				set
				{
					base.PowerSharpParameters["NoADLookup"] = value;
				}
			}

			public virtual SwitchParameter IncludeQuarantineDetails
			{
				set
				{
					base.PowerSharpParameters["IncludeQuarantineDetails"] = value;
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

		public class DatabaseParameters : ParametersBase
		{
			public virtual StoreMailboxIdParameter StoreMailboxIdentity
			{
				set
				{
					base.PowerSharpParameters["StoreMailboxIdentity"] = value;
				}
			}

			public virtual string Filter
			{
				set
				{
					base.PowerSharpParameters["Filter"] = value;
				}
			}

			public virtual DatabaseIdParameter Database
			{
				set
				{
					base.PowerSharpParameters["Database"] = value;
				}
			}

			public virtual ServerIdParameter CopyOnServer
			{
				set
				{
					base.PowerSharpParameters["CopyOnServer"] = value;
				}
			}

			public virtual SwitchParameter IncludeMoveHistory
			{
				set
				{
					base.PowerSharpParameters["IncludeMoveHistory"] = value;
				}
			}

			public virtual SwitchParameter IncludeMoveReport
			{
				set
				{
					base.PowerSharpParameters["IncludeMoveReport"] = value;
				}
			}

			public virtual SwitchParameter NoADLookup
			{
				set
				{
					base.PowerSharpParameters["NoADLookup"] = value;
				}
			}

			public virtual SwitchParameter IncludeQuarantineDetails
			{
				set
				{
					base.PowerSharpParameters["IncludeQuarantineDetails"] = value;
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

		public class ServerParameters : ParametersBase
		{
			public virtual string Filter
			{
				set
				{
					base.PowerSharpParameters["Filter"] = value;
				}
			}

			public virtual ServerIdParameter Server
			{
				set
				{
					base.PowerSharpParameters["Server"] = value;
				}
			}

			public virtual SwitchParameter IncludePassive
			{
				set
				{
					base.PowerSharpParameters["IncludePassive"] = value;
				}
			}

			public virtual SwitchParameter IncludeMoveHistory
			{
				set
				{
					base.PowerSharpParameters["IncludeMoveHistory"] = value;
				}
			}

			public virtual SwitchParameter IncludeMoveReport
			{
				set
				{
					base.PowerSharpParameters["IncludeMoveReport"] = value;
				}
			}

			public virtual SwitchParameter NoADLookup
			{
				set
				{
					base.PowerSharpParameters["NoADLookup"] = value;
				}
			}

			public virtual SwitchParameter IncludeQuarantineDetails
			{
				set
				{
					base.PowerSharpParameters["IncludeQuarantineDetails"] = value;
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
