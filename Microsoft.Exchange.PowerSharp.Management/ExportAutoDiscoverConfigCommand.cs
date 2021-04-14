using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class ExportAutoDiscoverConfigCommand : SyntheticCommandWithPipelineInputNoOutput<string>
	{
		private ExportAutoDiscoverConfigCommand() : base("Export-AutoDiscoverConfig")
		{
		}

		public ExportAutoDiscoverConfigCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual ExportAutoDiscoverConfigCommand SetParameters(ExportAutoDiscoverConfigCommand.DefaultParameters parameters)
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

			public virtual PSCredential SourceForestCredential
			{
				set
				{
					base.PowerSharpParameters["SourceForestCredential"] = value;
				}
			}

			public virtual Fqdn PreferredSourceFqdn
			{
				set
				{
					base.PowerSharpParameters["PreferredSourceFqdn"] = value;
				}
			}

			public virtual bool? DeleteConfig
			{
				set
				{
					base.PowerSharpParameters["DeleteConfig"] = value;
				}
			}

			public virtual string TargetForestDomainController
			{
				set
				{
					base.PowerSharpParameters["TargetForestDomainController"] = value;
				}
			}

			public virtual PSCredential TargetForestCredential
			{
				set
				{
					base.PowerSharpParameters["TargetForestCredential"] = value;
				}
			}

			public virtual bool MultipleExchangeDeployments
			{
				set
				{
					base.PowerSharpParameters["MultipleExchangeDeployments"] = value;
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
