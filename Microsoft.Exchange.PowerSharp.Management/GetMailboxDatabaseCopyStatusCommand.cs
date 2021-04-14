using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Management.SystemConfigurationTasks;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class GetMailboxDatabaseCopyStatusCommand : SyntheticCommandWithPipelineInput<DatabaseCopyStatusEntry, DatabaseCopyStatusEntry>
	{
		private GetMailboxDatabaseCopyStatusCommand() : base("Get-MailboxDatabaseCopyStatus")
		{
		}

		public GetMailboxDatabaseCopyStatusCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual GetMailboxDatabaseCopyStatusCommand SetParameters(GetMailboxDatabaseCopyStatusCommand.ExplicitServerParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual GetMailboxDatabaseCopyStatusCommand SetParameters(GetMailboxDatabaseCopyStatusCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual GetMailboxDatabaseCopyStatusCommand SetParameters(GetMailboxDatabaseCopyStatusCommand.IdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class ExplicitServerParameters : ParametersBase
		{
			public virtual MailboxServerIdParameter Server
			{
				set
				{
					base.PowerSharpParameters["Server"] = value;
				}
			}

			public virtual SwitchParameter Active
			{
				set
				{
					base.PowerSharpParameters["Active"] = value;
				}
			}

			public virtual SwitchParameter ConnectionStatus
			{
				set
				{
					base.PowerSharpParameters["ConnectionStatus"] = value;
				}
			}

			public virtual SwitchParameter ExtendedErrorInfo
			{
				set
				{
					base.PowerSharpParameters["ExtendedErrorInfo"] = value;
				}
			}

			public virtual SwitchParameter UseServerCache
			{
				set
				{
					base.PowerSharpParameters["UseServerCache"] = value;
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
			public virtual SwitchParameter Active
			{
				set
				{
					base.PowerSharpParameters["Active"] = value;
				}
			}

			public virtual SwitchParameter ConnectionStatus
			{
				set
				{
					base.PowerSharpParameters["ConnectionStatus"] = value;
				}
			}

			public virtual SwitchParameter ExtendedErrorInfo
			{
				set
				{
					base.PowerSharpParameters["ExtendedErrorInfo"] = value;
				}
			}

			public virtual SwitchParameter UseServerCache
			{
				set
				{
					base.PowerSharpParameters["UseServerCache"] = value;
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
			public virtual SwitchParameter Local
			{
				set
				{
					base.PowerSharpParameters["Local"] = value;
				}
			}

			public virtual DatabaseCopyIdParameter Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = value;
				}
			}

			public virtual SwitchParameter Active
			{
				set
				{
					base.PowerSharpParameters["Active"] = value;
				}
			}

			public virtual SwitchParameter ConnectionStatus
			{
				set
				{
					base.PowerSharpParameters["ConnectionStatus"] = value;
				}
			}

			public virtual SwitchParameter ExtendedErrorInfo
			{
				set
				{
					base.PowerSharpParameters["ExtendedErrorInfo"] = value;
				}
			}

			public virtual SwitchParameter UseServerCache
			{
				set
				{
					base.PowerSharpParameters["UseServerCache"] = value;
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
