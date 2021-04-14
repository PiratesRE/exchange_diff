using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Management;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetClientAccessServerCommand : SyntheticCommandWithPipelineInputNoOutput<ClientAccessServer>
	{
		private SetClientAccessServerCommand() : base("Set-ClientAccessServer")
		{
		}

		public SetClientAccessServerCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetClientAccessServerCommand SetParameters(SetClientAccessServerCommand.IdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetClientAccessServerCommand SetParameters(SetClientAccessServerCommand.AlternateServiceAccountParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetClientAccessServerCommand SetParameters(SetClientAccessServerCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class IdentityParameters : ParametersBase
		{
			public virtual Uri AutoDiscoverServiceInternalUri
			{
				set
				{
					base.PowerSharpParameters["AutoDiscoverServiceInternalUri"] = value;
				}
			}

			public virtual MultiValuedProperty<string> AutoDiscoverSiteScope
			{
				set
				{
					base.PowerSharpParameters["AutoDiscoverSiteScope"] = value;
				}
			}

			public virtual ClientAccessArrayIdParameter Array
			{
				set
				{
					base.PowerSharpParameters["Array"] = value;
				}
			}

			public virtual ClientAccessServerIdParameter Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual bool IsOutOfService
			{
				set
				{
					base.PowerSharpParameters["IsOutOfService"] = value;
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

		public class AlternateServiceAccountParameters : ParametersBase
		{
			public virtual PSCredential AlternateServiceAccountCredential
			{
				set
				{
					base.PowerSharpParameters["AlternateServiceAccountCredential"] = value;
				}
			}

			public virtual SwitchParameter CleanUpInvalidAlternateServiceAccountCredentials
			{
				set
				{
					base.PowerSharpParameters["CleanUpInvalidAlternateServiceAccountCredentials"] = value;
				}
			}

			public virtual SwitchParameter RemoveAlternateServiceAccountCredentials
			{
				set
				{
					base.PowerSharpParameters["RemoveAlternateServiceAccountCredentials"] = value;
				}
			}

			public virtual ClientAccessServerIdParameter Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual bool IsOutOfService
			{
				set
				{
					base.PowerSharpParameters["IsOutOfService"] = value;
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

		public class DefaultParameters : ParametersBase
		{
			public virtual ClientAccessServerIdParameter Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual bool IsOutOfService
			{
				set
				{
					base.PowerSharpParameters["IsOutOfService"] = value;
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
