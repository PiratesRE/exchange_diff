using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class RestoreDatabaseAvailabilityGroupCommand : SyntheticCommandWithPipelineInput<DatabaseAvailabilityGroup, DatabaseAvailabilityGroup>
	{
		private RestoreDatabaseAvailabilityGroupCommand() : base("Restore-DatabaseAvailabilityGroup")
		{
		}

		public RestoreDatabaseAvailabilityGroupCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual RestoreDatabaseAvailabilityGroupCommand SetParameters(RestoreDatabaseAvailabilityGroupCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual RestoreDatabaseAvailabilityGroupCommand SetParameters(RestoreDatabaseAvailabilityGroupCommand.IdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual string ActiveDirectorySite
			{
				set
				{
					base.PowerSharpParameters["ActiveDirectorySite"] = ((value != null) ? new AdSiteIdParameter(value) : null);
				}
			}

			public virtual SwitchParameter UsePrimaryWitnessServer
			{
				set
				{
					base.PowerSharpParameters["UsePrimaryWitnessServer"] = value;
				}
			}

			public virtual FileShareWitnessServerName AlternateWitnessServer
			{
				set
				{
					base.PowerSharpParameters["AlternateWitnessServer"] = value;
				}
			}

			public virtual NonRootLocalLongFullPath AlternateWitnessDirectory
			{
				set
				{
					base.PowerSharpParameters["AlternateWitnessDirectory"] = value;
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
			public virtual DatabaseAvailabilityGroupIdParameter Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = value;
				}
			}

			public virtual string ActiveDirectorySite
			{
				set
				{
					base.PowerSharpParameters["ActiveDirectorySite"] = ((value != null) ? new AdSiteIdParameter(value) : null);
				}
			}

			public virtual SwitchParameter UsePrimaryWitnessServer
			{
				set
				{
					base.PowerSharpParameters["UsePrimaryWitnessServer"] = value;
				}
			}

			public virtual FileShareWitnessServerName AlternateWitnessServer
			{
				set
				{
					base.PowerSharpParameters["AlternateWitnessServer"] = value;
				}
			}

			public virtual NonRootLocalLongFullPath AlternateWitnessDirectory
			{
				set
				{
					base.PowerSharpParameters["AlternateWitnessDirectory"] = value;
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
	}
}
