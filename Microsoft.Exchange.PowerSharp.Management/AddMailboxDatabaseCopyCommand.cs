using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class AddMailboxDatabaseCopyCommand : SyntheticCommandWithPipelineInput<Database, Database>
	{
		private AddMailboxDatabaseCopyCommand() : base("Add-MailboxDatabaseCopy")
		{
		}

		public AddMailboxDatabaseCopyCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual AddMailboxDatabaseCopyCommand SetParameters(AddMailboxDatabaseCopyCommand.IdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual AddMailboxDatabaseCopyCommand SetParameters(AddMailboxDatabaseCopyCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class IdentityParameters : ParametersBase
		{
			public virtual MailboxServerIdParameter MailboxServer
			{
				set
				{
					base.PowerSharpParameters["MailboxServer"] = value;
				}
			}

			public virtual DatabaseIdParameter Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = value;
				}
			}

			public virtual uint ActivationPreference
			{
				set
				{
					base.PowerSharpParameters["ActivationPreference"] = value;
				}
			}

			public virtual SwitchParameter SeedingPostponed
			{
				set
				{
					base.PowerSharpParameters["SeedingPostponed"] = value;
				}
			}

			public virtual SwitchParameter ConfigurationOnly
			{
				set
				{
					base.PowerSharpParameters["ConfigurationOnly"] = value;
				}
			}

			public virtual EnhancedTimeSpan ReplayLagTime
			{
				set
				{
					base.PowerSharpParameters["ReplayLagTime"] = value;
				}
			}

			public virtual EnhancedTimeSpan TruncationLagTime
			{
				set
				{
					base.PowerSharpParameters["TruncationLagTime"] = value;
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

		public class DefaultParameters : ParametersBase
		{
			public virtual uint ActivationPreference
			{
				set
				{
					base.PowerSharpParameters["ActivationPreference"] = value;
				}
			}

			public virtual SwitchParameter SeedingPostponed
			{
				set
				{
					base.PowerSharpParameters["SeedingPostponed"] = value;
				}
			}

			public virtual SwitchParameter ConfigurationOnly
			{
				set
				{
					base.PowerSharpParameters["ConfigurationOnly"] = value;
				}
			}

			public virtual EnhancedTimeSpan ReplayLagTime
			{
				set
				{
					base.PowerSharpParameters["ReplayLagTime"] = value;
				}
			}

			public virtual EnhancedTimeSpan TruncationLagTime
			{
				set
				{
					base.PowerSharpParameters["TruncationLagTime"] = value;
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
