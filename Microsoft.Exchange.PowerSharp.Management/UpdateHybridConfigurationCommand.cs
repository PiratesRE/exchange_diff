using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class UpdateHybridConfigurationCommand : SyntheticCommandWithPipelineInput<HybridConfiguration, HybridConfiguration>
	{
		private UpdateHybridConfigurationCommand() : base("Update-HybridConfiguration")
		{
		}

		public UpdateHybridConfigurationCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual UpdateHybridConfigurationCommand SetParameters(UpdateHybridConfigurationCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual PSCredential OnPremisesCredentials
			{
				set
				{
					base.PowerSharpParameters["OnPremisesCredentials"] = value;
				}
			}

			public virtual PSCredential TenantCredentials
			{
				set
				{
					base.PowerSharpParameters["TenantCredentials"] = value;
				}
			}

			public virtual SwitchParameter ForceUpgrade
			{
				set
				{
					base.PowerSharpParameters["ForceUpgrade"] = value;
				}
			}

			public virtual SwitchParameter SuppressOAuthWarning
			{
				set
				{
					base.PowerSharpParameters["SuppressOAuthWarning"] = value;
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
