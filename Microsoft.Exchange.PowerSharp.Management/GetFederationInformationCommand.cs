using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class GetFederationInformationCommand : SyntheticCommandWithPipelineInputNoOutput<SmtpDomain>
	{
		private GetFederationInformationCommand() : base("Get-FederationInformation")
		{
		}

		public GetFederationInformationCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual GetFederationInformationCommand SetParameters(GetFederationInformationCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual SmtpDomain DomainName
			{
				set
				{
					base.PowerSharpParameters["DomainName"] = value;
				}
			}

			public virtual MultiValuedProperty<string> TrustedHostnames
			{
				set
				{
					base.PowerSharpParameters["TrustedHostnames"] = value;
				}
			}

			public virtual SwitchParameter Force
			{
				set
				{
					base.PowerSharpParameters["Force"] = value;
				}
			}

			public virtual SwitchParameter BypassAdditionalDomainValidation
			{
				set
				{
					base.PowerSharpParameters["BypassAdditionalDomainValidation"] = value;
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
