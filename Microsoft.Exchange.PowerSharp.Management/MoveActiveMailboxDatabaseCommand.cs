using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class MoveActiveMailboxDatabaseCommand : SyntheticCommandWithPipelineInput<Database, Database>
	{
		private MoveActiveMailboxDatabaseCommand() : base("Move-ActiveMailboxDatabase")
		{
		}

		public MoveActiveMailboxDatabaseCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual MoveActiveMailboxDatabaseCommand SetParameters(MoveActiveMailboxDatabaseCommand.ServerParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual MoveActiveMailboxDatabaseCommand SetParameters(MoveActiveMailboxDatabaseCommand.IdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual MoveActiveMailboxDatabaseCommand SetParameters(MoveActiveMailboxDatabaseCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual MoveActiveMailboxDatabaseCommand SetParameters(MoveActiveMailboxDatabaseCommand.ActivatePreferredParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class ServerParameters : ParametersBase
		{
			public virtual MailboxServerIdParameter ActivateOnServer
			{
				set
				{
					base.PowerSharpParameters["ActivateOnServer"] = value;
				}
			}

			public virtual MailboxServerIdParameter Server
			{
				set
				{
					base.PowerSharpParameters["Server"] = value;
				}
			}

			public virtual DatabaseMountDialOverride MountDialOverride
			{
				set
				{
					base.PowerSharpParameters["MountDialOverride"] = value;
				}
			}

			public virtual SwitchParameter SkipActiveCopyChecks
			{
				set
				{
					base.PowerSharpParameters["SkipActiveCopyChecks"] = value;
				}
			}

			public virtual SwitchParameter SkipClientExperienceChecks
			{
				set
				{
					base.PowerSharpParameters["SkipClientExperienceChecks"] = value;
				}
			}

			public virtual SwitchParameter SkipHealthChecks
			{
				set
				{
					base.PowerSharpParameters["SkipHealthChecks"] = value;
				}
			}

			public virtual SwitchParameter SkipLagChecks
			{
				set
				{
					base.PowerSharpParameters["SkipLagChecks"] = value;
				}
			}

			public virtual SwitchParameter SkipMaximumActiveDatabasesChecks
			{
				set
				{
					base.PowerSharpParameters["SkipMaximumActiveDatabasesChecks"] = value;
				}
			}

			public virtual SwitchParameter TerminateOnWarning
			{
				set
				{
					base.PowerSharpParameters["TerminateOnWarning"] = value;
				}
			}

			public virtual string MoveComment
			{
				set
				{
					base.PowerSharpParameters["MoveComment"] = value;
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
			public virtual MailboxServerIdParameter ActivateOnServer
			{
				set
				{
					base.PowerSharpParameters["ActivateOnServer"] = value;
				}
			}

			public virtual DatabaseIdParameter Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = value;
				}
			}

			public virtual DatabaseMountDialOverride MountDialOverride
			{
				set
				{
					base.PowerSharpParameters["MountDialOverride"] = value;
				}
			}

			public virtual SwitchParameter SkipActiveCopyChecks
			{
				set
				{
					base.PowerSharpParameters["SkipActiveCopyChecks"] = value;
				}
			}

			public virtual SwitchParameter SkipClientExperienceChecks
			{
				set
				{
					base.PowerSharpParameters["SkipClientExperienceChecks"] = value;
				}
			}

			public virtual SwitchParameter SkipHealthChecks
			{
				set
				{
					base.PowerSharpParameters["SkipHealthChecks"] = value;
				}
			}

			public virtual SwitchParameter SkipLagChecks
			{
				set
				{
					base.PowerSharpParameters["SkipLagChecks"] = value;
				}
			}

			public virtual SwitchParameter SkipMaximumActiveDatabasesChecks
			{
				set
				{
					base.PowerSharpParameters["SkipMaximumActiveDatabasesChecks"] = value;
				}
			}

			public virtual SwitchParameter TerminateOnWarning
			{
				set
				{
					base.PowerSharpParameters["TerminateOnWarning"] = value;
				}
			}

			public virtual string MoveComment
			{
				set
				{
					base.PowerSharpParameters["MoveComment"] = value;
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
			public virtual DatabaseMountDialOverride MountDialOverride
			{
				set
				{
					base.PowerSharpParameters["MountDialOverride"] = value;
				}
			}

			public virtual SwitchParameter SkipActiveCopyChecks
			{
				set
				{
					base.PowerSharpParameters["SkipActiveCopyChecks"] = value;
				}
			}

			public virtual SwitchParameter SkipClientExperienceChecks
			{
				set
				{
					base.PowerSharpParameters["SkipClientExperienceChecks"] = value;
				}
			}

			public virtual SwitchParameter SkipHealthChecks
			{
				set
				{
					base.PowerSharpParameters["SkipHealthChecks"] = value;
				}
			}

			public virtual SwitchParameter SkipLagChecks
			{
				set
				{
					base.PowerSharpParameters["SkipLagChecks"] = value;
				}
			}

			public virtual SwitchParameter SkipMaximumActiveDatabasesChecks
			{
				set
				{
					base.PowerSharpParameters["SkipMaximumActiveDatabasesChecks"] = value;
				}
			}

			public virtual SwitchParameter TerminateOnWarning
			{
				set
				{
					base.PowerSharpParameters["TerminateOnWarning"] = value;
				}
			}

			public virtual string MoveComment
			{
				set
				{
					base.PowerSharpParameters["MoveComment"] = value;
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

		public class ActivatePreferredParameters : ParametersBase
		{
			public virtual MailboxServerIdParameter ActivatePreferredOnServer
			{
				set
				{
					base.PowerSharpParameters["ActivatePreferredOnServer"] = value;
				}
			}

			public virtual DatabaseMountDialOverride MountDialOverride
			{
				set
				{
					base.PowerSharpParameters["MountDialOverride"] = value;
				}
			}

			public virtual SwitchParameter SkipActiveCopyChecks
			{
				set
				{
					base.PowerSharpParameters["SkipActiveCopyChecks"] = value;
				}
			}

			public virtual SwitchParameter SkipClientExperienceChecks
			{
				set
				{
					base.PowerSharpParameters["SkipClientExperienceChecks"] = value;
				}
			}

			public virtual SwitchParameter SkipHealthChecks
			{
				set
				{
					base.PowerSharpParameters["SkipHealthChecks"] = value;
				}
			}

			public virtual SwitchParameter SkipLagChecks
			{
				set
				{
					base.PowerSharpParameters["SkipLagChecks"] = value;
				}
			}

			public virtual SwitchParameter SkipMaximumActiveDatabasesChecks
			{
				set
				{
					base.PowerSharpParameters["SkipMaximumActiveDatabasesChecks"] = value;
				}
			}

			public virtual SwitchParameter TerminateOnWarning
			{
				set
				{
					base.PowerSharpParameters["TerminateOnWarning"] = value;
				}
			}

			public virtual string MoveComment
			{
				set
				{
					base.PowerSharpParameters["MoveComment"] = value;
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
