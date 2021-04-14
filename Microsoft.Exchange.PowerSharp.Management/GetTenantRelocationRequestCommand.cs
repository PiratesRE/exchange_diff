using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class GetTenantRelocationRequestCommand : SyntheticCommandWithPipelineInput<TenantRelocationRequest, TenantRelocationRequest>
	{
		private GetTenantRelocationRequestCommand() : base("Get-TenantRelocationRequest")
		{
		}

		public GetTenantRelocationRequestCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual GetTenantRelocationRequestCommand SetParameters(GetTenantRelocationRequestCommand.PartitionWideParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual GetTenantRelocationRequestCommand SetParameters(GetTenantRelocationRequestCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual GetTenantRelocationRequestCommand SetParameters(GetTenantRelocationRequestCommand.IdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class PartitionWideParameters : ParametersBase
		{
			public virtual AccountPartitionIdParameter AccountPartition
			{
				set
				{
					base.PowerSharpParameters["AccountPartition"] = value;
				}
			}

			public virtual SwitchParameter SourceStateOnly
			{
				set
				{
					base.PowerSharpParameters["SourceStateOnly"] = value;
				}
			}

			public virtual RelocationStateRequested RelocationStateRequested
			{
				set
				{
					base.PowerSharpParameters["RelocationStateRequested"] = value;
				}
			}

			public virtual RelocationStatusDetailsSource RelocationStatusDetailsSource
			{
				set
				{
					base.PowerSharpParameters["RelocationStatusDetailsSource"] = value;
				}
			}

			public virtual RelocationError RelocationLastError
			{
				set
				{
					base.PowerSharpParameters["RelocationLastError"] = value;
				}
			}

			public virtual SwitchParameter Suspended
			{
				set
				{
					base.PowerSharpParameters["Suspended"] = value;
				}
			}

			public virtual SwitchParameter Lockdown
			{
				set
				{
					base.PowerSharpParameters["Lockdown"] = value;
				}
			}

			public virtual SwitchParameter StaleLockdown
			{
				set
				{
					base.PowerSharpParameters["StaleLockdown"] = value;
				}
			}

			public virtual SwitchParameter HasPermanentError
			{
				set
				{
					base.PowerSharpParameters["HasPermanentError"] = value;
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

		public class IdentityParameters : ParametersBase
		{
			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new TenantRelocationRequestIdParameter(value) : null);
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
		}
	}
}
