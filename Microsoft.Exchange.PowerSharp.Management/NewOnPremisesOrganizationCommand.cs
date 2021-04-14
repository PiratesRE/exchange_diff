using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class NewOnPremisesOrganizationCommand : SyntheticCommandWithPipelineInput<OnPremisesOrganization, OnPremisesOrganization>
	{
		private NewOnPremisesOrganizationCommand() : base("New-OnPremisesOrganization")
		{
		}

		public NewOnPremisesOrganizationCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual NewOnPremisesOrganizationCommand SetParameters(NewOnPremisesOrganizationCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual Guid OrganizationGuid
			{
				set
				{
					base.PowerSharpParameters["OrganizationGuid"] = value;
				}
			}

			public virtual MultiValuedProperty<SmtpDomain> HybridDomains
			{
				set
				{
					base.PowerSharpParameters["HybridDomains"] = value;
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

			public virtual string Organization
			{
				set
				{
					base.PowerSharpParameters["Organization"] = ((value != null) ? new OrganizationIdParameter(value) : null);
				}
			}

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
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
