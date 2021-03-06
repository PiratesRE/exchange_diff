using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class RemoveGlobalLocatorServiceDomainCommand : SyntheticCommandWithPipelineInputNoOutput<SmtpDomain>
	{
		private RemoveGlobalLocatorServiceDomainCommand() : base("Remove-GlobalLocatorServiceDomain")
		{
		}

		public RemoveGlobalLocatorServiceDomainCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual RemoveGlobalLocatorServiceDomainCommand SetParameters(RemoveGlobalLocatorServiceDomainCommand.DomainNameParameterSetParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DomainNameParameterSetParameters : ParametersBase
		{
			public virtual SmtpDomain DomainName
			{
				set
				{
					base.PowerSharpParameters["DomainName"] = value;
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

			public virtual SwitchParameter Confirm
			{
				set
				{
					base.PowerSharpParameters["Confirm"] = value;
				}
			}
		}
	}
}
