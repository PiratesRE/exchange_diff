using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetOnPremisesOrganizationCommand : SyntheticCommandWithPipelineInputNoOutput<OnPremisesOrganization>
	{
		private SetOnPremisesOrganizationCommand() : base("Set-OnPremisesOrganization")
		{
		}

		public SetOnPremisesOrganizationCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetOnPremisesOrganizationCommand SetParameters(SetOnPremisesOrganizationCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetOnPremisesOrganizationCommand SetParameters(SetOnPremisesOrganizationCommand.IdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual InboundConnectorIdParameter InboundConnector
			{
				set
				{
					base.PowerSharpParameters["InboundConnector"] = value;
				}
			}

			public virtual OutboundConnectorIdParameter OutboundConnector
			{
				set
				{
					base.PowerSharpParameters["OutboundConnector"] = value;
				}
			}

			public virtual string OrganizationName
			{
				set
				{
					base.PowerSharpParameters["OrganizationName"] = value;
				}
			}

			public virtual OrganizationRelationshipIdParameter OrganizationRelationship
			{
				set
				{
					base.PowerSharpParameters["OrganizationRelationship"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual MultiValuedProperty<SmtpDomain> HybridDomains
			{
				set
				{
					base.PowerSharpParameters["HybridDomains"] = value;
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
			public virtual OnPremisesOrganizationIdParameter Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = value;
				}
			}

			public virtual InboundConnectorIdParameter InboundConnector
			{
				set
				{
					base.PowerSharpParameters["InboundConnector"] = value;
				}
			}

			public virtual OutboundConnectorIdParameter OutboundConnector
			{
				set
				{
					base.PowerSharpParameters["OutboundConnector"] = value;
				}
			}

			public virtual string OrganizationName
			{
				set
				{
					base.PowerSharpParameters["OrganizationName"] = value;
				}
			}

			public virtual OrganizationRelationshipIdParameter OrganizationRelationship
			{
				set
				{
					base.PowerSharpParameters["OrganizationRelationship"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual MultiValuedProperty<SmtpDomain> HybridDomains
			{
				set
				{
					base.PowerSharpParameters["HybridDomains"] = value;
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
