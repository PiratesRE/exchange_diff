using System;
using System.Management.Automation;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class GetExchangeServerAccessLicenseUserCommand : SyntheticCommandWithPipelineInputNoOutput<string>
	{
		private GetExchangeServerAccessLicenseUserCommand() : base("Get-ExchangeServerAccessLicenseUser")
		{
		}

		public GetExchangeServerAccessLicenseUserCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual GetExchangeServerAccessLicenseUserCommand SetParameters(GetExchangeServerAccessLicenseUserCommand.LicenseNameParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class LicenseNameParameters : ParametersBase
		{
			public virtual string LicenseName
			{
				set
				{
					base.PowerSharpParameters["LicenseName"] = value;
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
