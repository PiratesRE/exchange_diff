using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Management.SystemConfigurationTasks;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class UpdateMailboxDatabaseCopyCommand : SyntheticCommandWithPipelineInputNoOutput<DatabaseCopyIdParameter>
	{
		private UpdateMailboxDatabaseCopyCommand() : base("Update-MailboxDatabaseCopy")
		{
		}

		public UpdateMailboxDatabaseCopyCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual UpdateMailboxDatabaseCopyCommand SetParameters(UpdateMailboxDatabaseCopyCommand.CancelSeedParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual UpdateMailboxDatabaseCopyCommand SetParameters(UpdateMailboxDatabaseCopyCommand.IdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual UpdateMailboxDatabaseCopyCommand SetParameters(UpdateMailboxDatabaseCopyCommand.ExplicitServerParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual UpdateMailboxDatabaseCopyCommand SetParameters(UpdateMailboxDatabaseCopyCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class CancelSeedParameters : ParametersBase
		{
			public virtual DatabaseCopyIdParameter Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = value;
				}
			}

			public virtual SwitchParameter CancelSeed
			{
				set
				{
					base.PowerSharpParameters["CancelSeed"] = value;
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

			public virtual SwitchParameter Confirm
			{
				set
				{
					base.PowerSharpParameters["Confirm"] = value;
				}
			}
		}

		public class IdentityParameters : ParametersBase
		{
			public virtual DatabaseCopyIdParameter Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = value;
				}
			}

			public virtual SwitchParameter BeginSeed
			{
				set
				{
					base.PowerSharpParameters["BeginSeed"] = value;
				}
			}

			public virtual SwitchParameter DatabaseOnly
			{
				set
				{
					base.PowerSharpParameters["DatabaseOnly"] = value;
				}
			}

			public virtual SwitchParameter CatalogOnly
			{
				set
				{
					base.PowerSharpParameters["CatalogOnly"] = value;
				}
			}

			public virtual SwitchParameter ManualResume
			{
				set
				{
					base.PowerSharpParameters["ManualResume"] = value;
				}
			}

			public virtual SwitchParameter DeleteExistingFiles
			{
				set
				{
					base.PowerSharpParameters["DeleteExistingFiles"] = value;
				}
			}

			public virtual SwitchParameter SafeDeleteExistingFiles
			{
				set
				{
					base.PowerSharpParameters["SafeDeleteExistingFiles"] = value;
				}
			}

			public virtual SwitchParameter Force
			{
				set
				{
					base.PowerSharpParameters["Force"] = value;
				}
			}

			public virtual DatabaseAvailabilityGroupNetworkIdParameter Network
			{
				set
				{
					base.PowerSharpParameters["Network"] = value;
				}
			}

			public virtual UseDagDefaultOnOff NetworkCompressionOverride
			{
				set
				{
					base.PowerSharpParameters["NetworkCompressionOverride"] = value;
				}
			}

			public virtual UseDagDefaultOnOff NetworkEncryptionOverride
			{
				set
				{
					base.PowerSharpParameters["NetworkEncryptionOverride"] = value;
				}
			}

			public virtual ServerIdParameter SourceServer
			{
				set
				{
					base.PowerSharpParameters["SourceServer"] = value;
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

			public virtual SwitchParameter Confirm
			{
				set
				{
					base.PowerSharpParameters["Confirm"] = value;
				}
			}
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

			public virtual int MaximumSeedsInParallel
			{
				set
				{
					base.PowerSharpParameters["MaximumSeedsInParallel"] = value;
				}
			}

			public virtual SwitchParameter DatabaseOnly
			{
				set
				{
					base.PowerSharpParameters["DatabaseOnly"] = value;
				}
			}

			public virtual SwitchParameter CatalogOnly
			{
				set
				{
					base.PowerSharpParameters["CatalogOnly"] = value;
				}
			}

			public virtual SwitchParameter ManualResume
			{
				set
				{
					base.PowerSharpParameters["ManualResume"] = value;
				}
			}

			public virtual SwitchParameter DeleteExistingFiles
			{
				set
				{
					base.PowerSharpParameters["DeleteExistingFiles"] = value;
				}
			}

			public virtual SwitchParameter SafeDeleteExistingFiles
			{
				set
				{
					base.PowerSharpParameters["SafeDeleteExistingFiles"] = value;
				}
			}

			public virtual UseDagDefaultOnOff NetworkCompressionOverride
			{
				set
				{
					base.PowerSharpParameters["NetworkCompressionOverride"] = value;
				}
			}

			public virtual UseDagDefaultOnOff NetworkEncryptionOverride
			{
				set
				{
					base.PowerSharpParameters["NetworkEncryptionOverride"] = value;
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

			public virtual SwitchParameter Confirm
			{
				set
				{
					base.PowerSharpParameters["Confirm"] = value;
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

			public virtual SwitchParameter WhatIf
			{
				set
				{
					base.PowerSharpParameters["WhatIf"] = value;
				}
			}

			public virtual SwitchParameter Confirm
			{
				set
				{
					base.PowerSharpParameters["Confirm"] = value;
				}
			}
		}
	}
}
