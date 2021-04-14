using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class NewDatabaseAvailabilityGroupConfigurationCommand : SyntheticCommandWithPipelineInput<DatabaseAvailabilityGroupConfiguration, DatabaseAvailabilityGroupConfiguration>
	{
		private NewDatabaseAvailabilityGroupConfigurationCommand() : base("New-DatabaseAvailabilityGroupConfiguration")
		{
		}

		public NewDatabaseAvailabilityGroupConfigurationCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual NewDatabaseAvailabilityGroupConfigurationCommand SetParameters(NewDatabaseAvailabilityGroupConfigurationCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual int ServersPerDag
			{
				set
				{
					base.PowerSharpParameters["ServersPerDag"] = value;
				}
			}

			public virtual int DatabasesPerServer
			{
				set
				{
					base.PowerSharpParameters["DatabasesPerServer"] = value;
				}
			}

			public virtual int DatabasesPerVolume
			{
				set
				{
					base.PowerSharpParameters["DatabasesPerVolume"] = value;
				}
			}

			public virtual int CopiesPerDatabase
			{
				set
				{
					base.PowerSharpParameters["CopiesPerDatabase"] = value;
				}
			}

			public virtual int MinCopiesPerDatabaseForMonitoring
			{
				set
				{
					base.PowerSharpParameters["MinCopiesPerDatabaseForMonitoring"] = value;
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
