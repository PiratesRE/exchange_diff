using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetGlobalLocatorServiceTenantCommand : SyntheticCommandWithPipelineInputNoOutput<SmtpDomain>
	{
		private SetGlobalLocatorServiceTenantCommand() : base("Set-GlobalLocatorServiceTenant")
		{
		}

		public SetGlobalLocatorServiceTenantCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetGlobalLocatorServiceTenantCommand SetParameters(SetGlobalLocatorServiceTenantCommand.DomainNameParameterSetParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetGlobalLocatorServiceTenantCommand SetParameters(SetGlobalLocatorServiceTenantCommand.ExternalDirectoryOrganizationIdParameterSetParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetGlobalLocatorServiceTenantCommand SetParameters(SetGlobalLocatorServiceTenantCommand.MsaUserNetIDParameterSetParameters parameters)
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

			public virtual string ResourceForest
			{
				set
				{
					base.PowerSharpParameters["ResourceForest"] = value;
				}
			}

			public virtual string AccountForest
			{
				set
				{
					base.PowerSharpParameters["AccountForest"] = value;
				}
			}

			public virtual string PrimarySite
			{
				set
				{
					base.PowerSharpParameters["PrimarySite"] = value;
				}
			}

			public virtual SmtpDomain SmtpNextHopDomain
			{
				set
				{
					base.PowerSharpParameters["SmtpNextHopDomain"] = value;
				}
			}

			public virtual GlsTenantFlags TenantFlags
			{
				set
				{
					base.PowerSharpParameters["TenantFlags"] = value;
				}
			}

			public virtual string TenantContainerCN
			{
				set
				{
					base.PowerSharpParameters["TenantContainerCN"] = value;
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

		public class ExternalDirectoryOrganizationIdParameterSetParameters : ParametersBase
		{
			public virtual string ResourceForest
			{
				set
				{
					base.PowerSharpParameters["ResourceForest"] = value;
				}
			}

			public virtual string AccountForest
			{
				set
				{
					base.PowerSharpParameters["AccountForest"] = value;
				}
			}

			public virtual string PrimarySite
			{
				set
				{
					base.PowerSharpParameters["PrimarySite"] = value;
				}
			}

			public virtual SmtpDomain SmtpNextHopDomain
			{
				set
				{
					base.PowerSharpParameters["SmtpNextHopDomain"] = value;
				}
			}

			public virtual GlsTenantFlags TenantFlags
			{
				set
				{
					base.PowerSharpParameters["TenantFlags"] = value;
				}
			}

			public virtual string TenantContainerCN
			{
				set
				{
					base.PowerSharpParameters["TenantContainerCN"] = value;
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
