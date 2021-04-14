using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Management.E4E;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetOMEConfigurationCommand : SyntheticCommandWithPipelineInputNoOutput<EncryptionConfiguration>
	{
		private SetOMEConfigurationCommand() : base("Set-OMEConfiguration")
		{
		}

		public SetOMEConfigurationCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetOMEConfigurationCommand SetParameters(SetOMEConfigurationCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetOMEConfigurationCommand SetParameters(SetOMEConfigurationCommand.IdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual string Organization
			{
				set
				{
					base.PowerSharpParameters["Organization"] = ((value != null) ? new OrganizationIdParameter(value) : null);
				}
			}

			public virtual byte Image
			{
				set
				{
					base.PowerSharpParameters["Image"] = value;
				}
			}

			public virtual string EmailText
			{
				set
				{
					base.PowerSharpParameters["EmailText"] = value;
				}
			}

			public virtual string PortalText
			{
				set
				{
					base.PowerSharpParameters["PortalText"] = value;
				}
			}

			public virtual string DisclaimerText
			{
				set
				{
					base.PowerSharpParameters["DisclaimerText"] = value;
				}
			}

			public virtual bool OTPEnabled
			{
				set
				{
					base.PowerSharpParameters["OTPEnabled"] = value;
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
					base.PowerSharpParameters["Identity"] = ((value != null) ? new OMEConfigurationIdParameter(value) : null);
				}
			}

			public virtual string Organization
			{
				set
				{
					base.PowerSharpParameters["Organization"] = ((value != null) ? new OrganizationIdParameter(value) : null);
				}
			}

			public virtual byte Image
			{
				set
				{
					base.PowerSharpParameters["Image"] = value;
				}
			}

			public virtual string EmailText
			{
				set
				{
					base.PowerSharpParameters["EmailText"] = value;
				}
			}

			public virtual string PortalText
			{
				set
				{
					base.PowerSharpParameters["PortalText"] = value;
				}
			}

			public virtual string DisclaimerText
			{
				set
				{
					base.PowerSharpParameters["DisclaimerText"] = value;
				}
			}

			public virtual bool OTPEnabled
			{
				set
				{
					base.PowerSharpParameters["OTPEnabled"] = value;
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
