﻿using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class GetIPBlockListProvidersConfigCommand : SyntheticCommandWithPipelineInput<IPBlockListProviderConfig, IPBlockListProviderConfig>
	{
		private GetIPBlockListProvidersConfigCommand() : base("Get-IPBlockListProvidersConfig")
		{
		}

		public GetIPBlockListProvidersConfigCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual GetIPBlockListProvidersConfigCommand SetParameters(GetIPBlockListProvidersConfigCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
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
