using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class NewAuthRedirectCommand : SyntheticCommandWithPipelineInput<AuthRedirect, AuthRedirect>
	{
		private NewAuthRedirectCommand() : base("New-AuthRedirect")
		{
		}

		public NewAuthRedirectCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual NewAuthRedirectCommand SetParameters(NewAuthRedirectCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual AuthScheme AuthScheme
			{
				set
				{
					base.PowerSharpParameters["AuthScheme"] = value;
				}
			}

			public virtual string TargetUrl
			{
				set
				{
					base.PowerSharpParameters["TargetUrl"] = value;
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
