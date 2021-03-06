using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetClientAccessArrayCommand : SyntheticCommandWithPipelineInputNoOutput<ClientAccessArray>
	{
		private SetClientAccessArrayCommand() : base("Set-ClientAccessArray")
		{
		}

		public SetClientAccessArrayCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetClientAccessArrayCommand SetParameters(SetClientAccessArrayCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetClientAccessArrayCommand SetParameters(SetClientAccessArrayCommand.IdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual string ArrayDefinition
			{
				set
				{
					base.PowerSharpParameters["ArrayDefinition"] = value;
				}
			}

			public virtual int ServerCount
			{
				set
				{
					base.PowerSharpParameters["ServerCount"] = value;
				}
			}

			public virtual string Site
			{
				set
				{
					base.PowerSharpParameters["Site"] = ((value != null) ? new AdSiteIdParameter(value) : null);
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
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

		public class IdentityParameters : ParametersBase
		{
			public virtual ClientAccessArrayIdParameter Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = value;
				}
			}

			public virtual string ArrayDefinition
			{
				set
				{
					base.PowerSharpParameters["ArrayDefinition"] = value;
				}
			}

			public virtual int ServerCount
			{
				set
				{
					base.PowerSharpParameters["ServerCount"] = value;
				}
			}

			public virtual string Site
			{
				set
				{
					base.PowerSharpParameters["Site"] = ((value != null) ? new AdSiteIdParameter(value) : null);
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
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
