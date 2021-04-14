using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetFederatedOrganizationIdentifierCommand : SyntheticCommandWithPipelineInputNoOutput<FederatedOrganizationId>
	{
		private SetFederatedOrganizationIdentifierCommand() : base("Set-FederatedOrganizationIdentifier")
		{
		}

		public SetFederatedOrganizationIdentifierCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetFederatedOrganizationIdentifierCommand SetParameters(SetFederatedOrganizationIdentifierCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetFederatedOrganizationIdentifierCommand SetParameters(SetFederatedOrganizationIdentifierCommand.IdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual FederationTrustIdParameter DelegationFederationTrust
			{
				set
				{
					base.PowerSharpParameters["DelegationFederationTrust"] = value;
				}
			}

			public virtual SmtpDomain AccountNamespace
			{
				set
				{
					base.PowerSharpParameters["AccountNamespace"] = value;
				}
			}

			public virtual SmtpDomain DefaultDomain
			{
				set
				{
					base.PowerSharpParameters["DefaultDomain"] = value;
				}
			}

			public virtual bool Enabled
			{
				set
				{
					base.PowerSharpParameters["Enabled"] = value;
				}
			}

			public virtual SmtpAddress OrganizationContact
			{
				set
				{
					base.PowerSharpParameters["OrganizationContact"] = value;
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

		public class IdentityParameters : ParametersBase
		{
			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new OrganizationIdParameter(value) : null);
				}
			}

			public virtual FederationTrustIdParameter DelegationFederationTrust
			{
				set
				{
					base.PowerSharpParameters["DelegationFederationTrust"] = value;
				}
			}

			public virtual SmtpDomain AccountNamespace
			{
				set
				{
					base.PowerSharpParameters["AccountNamespace"] = value;
				}
			}

			public virtual SmtpDomain DefaultDomain
			{
				set
				{
					base.PowerSharpParameters["DefaultDomain"] = value;
				}
			}

			public virtual bool Enabled
			{
				set
				{
					base.PowerSharpParameters["Enabled"] = value;
				}
			}

			public virtual SmtpAddress OrganizationContact
			{
				set
				{
					base.PowerSharpParameters["OrganizationContact"] = value;
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
