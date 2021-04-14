using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class NewX400AuthoritativeDomainCommand : SyntheticCommandWithPipelineInput<X400AuthoritativeDomain, X400AuthoritativeDomain>
	{
		private NewX400AuthoritativeDomainCommand() : base("New-X400AuthoritativeDomain")
		{
		}

		public NewX400AuthoritativeDomainCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual NewX400AuthoritativeDomainCommand SetParameters(NewX400AuthoritativeDomainCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual X400Domain X400DomainName
			{
				set
				{
					base.PowerSharpParameters["X400DomainName"] = value;
				}
			}

			public virtual bool X400ExternalRelay
			{
				set
				{
					base.PowerSharpParameters["X400ExternalRelay"] = value;
				}
			}

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
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
