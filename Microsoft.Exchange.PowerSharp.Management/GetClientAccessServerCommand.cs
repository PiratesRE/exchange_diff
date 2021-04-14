using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class GetClientAccessServerCommand : SyntheticCommandWithPipelineInput<Server, Server>
	{
		private GetClientAccessServerCommand() : base("Get-ClientAccessServer")
		{
		}

		public GetClientAccessServerCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual GetClientAccessServerCommand SetParameters(GetClientAccessServerCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual GetClientAccessServerCommand SetParameters(GetClientAccessServerCommand.IdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual SwitchParameter IncludeAlternateServiceAccountCredentialStatus
			{
				set
				{
					base.PowerSharpParameters["IncludeAlternateServiceAccountCredentialStatus"] = value;
				}
			}

			public virtual SwitchParameter IncludeAlternateServiceAccountCredentialPassword
			{
				set
				{
					base.PowerSharpParameters["IncludeAlternateServiceAccountCredentialPassword"] = value;
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
			public virtual ClientAccessServerIdParameter Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = value;
				}
			}

			public virtual SwitchParameter IncludeAlternateServiceAccountCredentialStatus
			{
				set
				{
					base.PowerSharpParameters["IncludeAlternateServiceAccountCredentialStatus"] = value;
				}
			}

			public virtual SwitchParameter IncludeAlternateServiceAccountCredentialPassword
			{
				set
				{
					base.PowerSharpParameters["IncludeAlternateServiceAccountCredentialPassword"] = value;
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
