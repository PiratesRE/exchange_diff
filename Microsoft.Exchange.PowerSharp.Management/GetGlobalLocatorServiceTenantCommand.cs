using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class GetGlobalLocatorServiceTenantCommand : SyntheticCommandWithPipelineInputNoOutput<SmtpDomain>
	{
		private GetGlobalLocatorServiceTenantCommand() : base("Get-GlobalLocatorServiceTenant")
		{
		}

		public GetGlobalLocatorServiceTenantCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual GetGlobalLocatorServiceTenantCommand SetParameters(GetGlobalLocatorServiceTenantCommand.DomainNameParameterSetParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual GetGlobalLocatorServiceTenantCommand SetParameters(GetGlobalLocatorServiceTenantCommand.ExternalDirectoryOrganizationIdParameterSetParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual GetGlobalLocatorServiceTenantCommand SetParameters(GetGlobalLocatorServiceTenantCommand.MsaUserNetIDParameterSetParameters parameters)
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

			public virtual SwitchParameter ShowDomainNames
			{
				set
				{
					base.PowerSharpParameters["ShowDomainNames"] = value;
				}
			}

			public virtual SwitchParameter UseOfflineGLS
			{
				set
				{
					base.PowerSharpParameters["UseOfflineGLS"] = value;
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

		public class ExternalDirectoryOrganizationIdParameterSetParameters : ParametersBase
		{
			public virtual SwitchParameter ShowDomainNames
			{
				set
				{
					base.PowerSharpParameters["ShowDomainNames"] = value;
				}
			}

			public virtual SwitchParameter UseOfflineGLS
			{
				set
				{
					base.PowerSharpParameters["UseOfflineGLS"] = value;
				}
			}

			public virtual Guid ExternalDirectoryOrganizationId
			{
				set
				{
					base.PowerSharpParameters["ExternalDirectoryOrganizationId"] = value;
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

		public class MsaUserNetIDParameterSetParameters : ParametersBase
		{
			public virtual Guid ExternalDirectoryOrganizationId
			{
				set
				{
					base.PowerSharpParameters["ExternalDirectoryOrganizationId"] = value;
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
