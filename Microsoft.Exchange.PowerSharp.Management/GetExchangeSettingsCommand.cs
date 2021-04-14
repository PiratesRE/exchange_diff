using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class GetExchangeSettingsCommand : SyntheticCommandWithPipelineInput<ExchangeSettings, ExchangeSettings>
	{
		private GetExchangeSettingsCommand() : base("Get-ExchangeSettings")
		{
		}

		public GetExchangeSettingsCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual GetExchangeSettingsCommand SetParameters(GetExchangeSettingsCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual GetExchangeSettingsCommand SetParameters(GetExchangeSettingsCommand.IdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual SwitchParameter Diagnostic
			{
				set
				{
					base.PowerSharpParameters["Diagnostic"] = value;
				}
			}

			public virtual string DiagnosticArgument
			{
				set
				{
					base.PowerSharpParameters["DiagnosticArgument"] = value;
				}
			}

			public virtual string ConfigName
			{
				set
				{
					base.PowerSharpParameters["ConfigName"] = value;
				}
			}

			public virtual DatabaseIdParameter Database
			{
				set
				{
					base.PowerSharpParameters["Database"] = value;
				}
			}

			public virtual ServerIdParameter Server
			{
				set
				{
					base.PowerSharpParameters["Server"] = value;
				}
			}

			public virtual string Process
			{
				set
				{
					base.PowerSharpParameters["Process"] = value;
				}
			}

			public virtual Guid User
			{
				set
				{
					base.PowerSharpParameters["User"] = value;
				}
			}

			public virtual string Organization
			{
				set
				{
					base.PowerSharpParameters["Organization"] = ((value != null) ? new OrganizationIdParameter(value) : null);
				}
			}

			public virtual string GenericScopeName
			{
				set
				{
					base.PowerSharpParameters["GenericScopeName"] = value;
				}
			}

			public virtual string GenericScopeValue
			{
				set
				{
					base.PowerSharpParameters["GenericScopeValue"] = value;
				}
			}

			public virtual string GenericScopes
			{
				set
				{
					base.PowerSharpParameters["GenericScopes"] = value;
				}
			}

			public virtual SwitchParameter Force
			{
				set
				{
					base.PowerSharpParameters["Force"] = value;
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
			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new ExchangeSettingsIdParameter(value) : null);
				}
			}

			public virtual SwitchParameter Diagnostic
			{
				set
				{
					base.PowerSharpParameters["Diagnostic"] = value;
				}
			}

			public virtual string DiagnosticArgument
			{
				set
				{
					base.PowerSharpParameters["DiagnosticArgument"] = value;
				}
			}

			public virtual string ConfigName
			{
				set
				{
					base.PowerSharpParameters["ConfigName"] = value;
				}
			}

			public virtual DatabaseIdParameter Database
			{
				set
				{
					base.PowerSharpParameters["Database"] = value;
				}
			}

			public virtual ServerIdParameter Server
			{
				set
				{
					base.PowerSharpParameters["Server"] = value;
				}
			}

			public virtual string Process
			{
				set
				{
					base.PowerSharpParameters["Process"] = value;
				}
			}

			public virtual Guid User
			{
				set
				{
					base.PowerSharpParameters["User"] = value;
				}
			}

			public virtual string Organization
			{
				set
				{
					base.PowerSharpParameters["Organization"] = ((value != null) ? new OrganizationIdParameter(value) : null);
				}
			}

			public virtual string GenericScopeName
			{
				set
				{
					base.PowerSharpParameters["GenericScopeName"] = value;
				}
			}

			public virtual string GenericScopeValue
			{
				set
				{
					base.PowerSharpParameters["GenericScopeValue"] = value;
				}
			}

			public virtual string GenericScopes
			{
				set
				{
					base.PowerSharpParameters["GenericScopes"] = value;
				}
			}

			public virtual SwitchParameter Force
			{
				set
				{
					base.PowerSharpParameters["Force"] = value;
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
