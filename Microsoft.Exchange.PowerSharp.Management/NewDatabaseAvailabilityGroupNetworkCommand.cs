using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class NewDatabaseAvailabilityGroupNetworkCommand : SyntheticCommandWithPipelineInput<DatabaseAvailabilityGroupNetwork, DatabaseAvailabilityGroupNetwork>
	{
		private NewDatabaseAvailabilityGroupNetworkCommand() : base("New-DatabaseAvailabilityGroupNetwork")
		{
		}

		public NewDatabaseAvailabilityGroupNetworkCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual NewDatabaseAvailabilityGroupNetworkCommand SetParameters(NewDatabaseAvailabilityGroupNetworkCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
				}
			}

			public virtual DatabaseAvailabilityGroupIdParameter DatabaseAvailabilityGroup
			{
				set
				{
					base.PowerSharpParameters["DatabaseAvailabilityGroup"] = value;
				}
			}

			public virtual DatabaseAvailabilityGroupSubnetId Subnets
			{
				set
				{
					base.PowerSharpParameters["Subnets"] = value;
				}
			}

			public virtual string Description
			{
				set
				{
					base.PowerSharpParameters["Description"] = value;
				}
			}

			public virtual bool ReplicationEnabled
			{
				set
				{
					base.PowerSharpParameters["ReplicationEnabled"] = value;
				}
			}

			public virtual bool IgnoreNetwork
			{
				set
				{
					base.PowerSharpParameters["IgnoreNetwork"] = value;
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
