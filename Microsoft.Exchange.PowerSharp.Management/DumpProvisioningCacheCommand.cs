using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class DumpProvisioningCacheCommand : SyntheticCommandWithPipelineInputNoOutput<Fqdn>
	{
		private DumpProvisioningCacheCommand() : base("Dump-ProvisioningCache")
		{
		}

		public DumpProvisioningCacheCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual DumpProvisioningCacheCommand SetParameters(DumpProvisioningCacheCommand.GlobalCacheParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual DumpProvisioningCacheCommand SetParameters(DumpProvisioningCacheCommand.OrganizationCacheParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual DumpProvisioningCacheCommand SetParameters(DumpProvisioningCacheCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class GlobalCacheParameters : ParametersBase
		{
			public virtual Fqdn Server
			{
				set
				{
					base.PowerSharpParameters["Server"] = value;
				}
			}

			public virtual string Application
			{
				set
				{
					base.PowerSharpParameters["Application"] = value;
				}
			}

			public virtual SwitchParameter GlobalCache
			{
				set
				{
					base.PowerSharpParameters["GlobalCache"] = value;
				}
			}

			public virtual MultiValuedProperty<Guid> CacheKeys
			{
				set
				{
					base.PowerSharpParameters["CacheKeys"] = value;
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

		public class OrganizationCacheParameters : ParametersBase
		{
			public virtual Fqdn Server
			{
				set
				{
					base.PowerSharpParameters["Server"] = value;
				}
			}

			public virtual string Application
			{
				set
				{
					base.PowerSharpParameters["Application"] = value;
				}
			}

			public virtual MultiValuedProperty<OrganizationIdParameter> Organizations
			{
				set
				{
					base.PowerSharpParameters["Organizations"] = value;
				}
			}

			public virtual SwitchParameter CurrentOrganization
			{
				set
				{
					base.PowerSharpParameters["CurrentOrganization"] = value;
				}
			}

			public virtual MultiValuedProperty<Guid> CacheKeys
			{
				set
				{
					base.PowerSharpParameters["CacheKeys"] = value;
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
			public virtual MultiValuedProperty<Guid> CacheKeys
			{
				set
				{
					base.PowerSharpParameters["CacheKeys"] = value;
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
