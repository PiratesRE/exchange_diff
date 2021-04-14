using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetSyncConfigCommand : SyntheticCommandWithPipelineInputNoOutput<SyncOrganization>
	{
		private SetSyncConfigCommand() : base("Set-SyncConfig")
		{
		}

		public SetSyncConfigCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetSyncConfigCommand SetParameters(SetSyncConfigCommand.FederatedParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetSyncConfigCommand SetParameters(SetSyncConfigCommand.ManagedParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetSyncConfigCommand SetParameters(SetSyncConfigCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class FederatedParameters : ParametersBase
		{
			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new OrganizationIdParameter(value) : null);
				}
			}

			public virtual string FederatedIdentitySourceADAttribute
			{
				set
				{
					base.PowerSharpParameters["FederatedIdentitySourceADAttribute"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual bool WlidUseSMTPPrimary
			{
				set
				{
					base.PowerSharpParameters["WlidUseSMTPPrimary"] = value;
				}
			}

			public virtual SmtpDomainWithSubdomains ProvisioningDomain
			{
				set
				{
					base.PowerSharpParameters["ProvisioningDomain"] = value;
				}
			}

			public virtual EnterpriseExchangeVersionEnum EnterpriseExchangeVersion
			{
				set
				{
					base.PowerSharpParameters["EnterpriseExchangeVersion"] = value;
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

		public class ManagedParameters : ParametersBase
		{
			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new OrganizationIdParameter(value) : null);
				}
			}

			public virtual bool DisableWindowsLiveID
			{
				set
				{
					base.PowerSharpParameters["DisableWindowsLiveID"] = value;
				}
			}

			public virtual string PasswordFilePath
			{
				set
				{
					base.PowerSharpParameters["PasswordFilePath"] = value;
				}
			}

			public virtual bool ResetPasswordOnNextLogon
			{
				set
				{
					base.PowerSharpParameters["ResetPasswordOnNextLogon"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual bool WlidUseSMTPPrimary
			{
				set
				{
					base.PowerSharpParameters["WlidUseSMTPPrimary"] = value;
				}
			}

			public virtual SmtpDomainWithSubdomains ProvisioningDomain
			{
				set
				{
					base.PowerSharpParameters["ProvisioningDomain"] = value;
				}
			}

			public virtual EnterpriseExchangeVersionEnum EnterpriseExchangeVersion
			{
				set
				{
					base.PowerSharpParameters["EnterpriseExchangeVersion"] = value;
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
			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual bool WlidUseSMTPPrimary
			{
				set
				{
					base.PowerSharpParameters["WlidUseSMTPPrimary"] = value;
				}
			}

			public virtual SmtpDomainWithSubdomains ProvisioningDomain
			{
				set
				{
					base.PowerSharpParameters["ProvisioningDomain"] = value;
				}
			}

			public virtual EnterpriseExchangeVersionEnum EnterpriseExchangeVersion
			{
				set
				{
					base.PowerSharpParameters["EnterpriseExchangeVersion"] = value;
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
