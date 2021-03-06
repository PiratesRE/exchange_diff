using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class NewAvailabilityConfigCommand : SyntheticCommandWithPipelineInput<AvailabilityConfig, AvailabilityConfig>
	{
		private NewAvailabilityConfigCommand() : base("New-AvailabilityConfig")
		{
		}

		public NewAvailabilityConfigCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual NewAvailabilityConfigCommand SetParameters(NewAvailabilityConfigCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual string OrgWideAccount
			{
				set
				{
					base.PowerSharpParameters["OrgWideAccount"] = ((value != null) ? new SecurityPrincipalIdParameter(value) : null);
				}
			}

			public virtual string Organization
			{
				set
				{
					base.PowerSharpParameters["Organization"] = ((value != null) ? new OrganizationIdParameter(value) : null);
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
