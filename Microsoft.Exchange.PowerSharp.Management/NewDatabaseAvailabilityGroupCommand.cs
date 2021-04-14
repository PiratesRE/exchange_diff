using System;
using System.Management.Automation;
using System.Net;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class NewDatabaseAvailabilityGroupCommand : SyntheticCommandWithPipelineInput<DatabaseAvailabilityGroup, DatabaseAvailabilityGroup>
	{
		private NewDatabaseAvailabilityGroupCommand() : base("New-DatabaseAvailabilityGroup")
		{
		}

		public NewDatabaseAvailabilityGroupCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual NewDatabaseAvailabilityGroupCommand SetParameters(NewDatabaseAvailabilityGroupCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual FileShareWitnessServerName WitnessServer
			{
				set
				{
					base.PowerSharpParameters["WitnessServer"] = value;
				}
			}

			public virtual NonRootLocalLongFullPath WitnessDirectory
			{
				set
				{
					base.PowerSharpParameters["WitnessDirectory"] = value;
				}
			}

			public virtual ThirdPartyReplicationMode ThirdPartyReplication
			{
				set
				{
					base.PowerSharpParameters["ThirdPartyReplication"] = value;
				}
			}

			public virtual IPAddress DatabaseAvailabilityGroupIpAddresses
			{
				set
				{
					base.PowerSharpParameters["DatabaseAvailabilityGroupIpAddresses"] = value;
				}
			}

			public virtual DatabaseAvailabilityGroupConfigurationIdParameter DagConfiguration
			{
				set
				{
					base.PowerSharpParameters["DagConfiguration"] = value;
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
