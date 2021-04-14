using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class NewInboundConnectorCommand : SyntheticCommandWithPipelineInput<TenantInboundConnector, TenantInboundConnector>
	{
		private NewInboundConnectorCommand() : base("New-InboundConnector")
		{
		}

		public NewInboundConnectorCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual NewInboundConnectorCommand SetParameters(NewInboundConnectorCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual bool Enabled
			{
				set
				{
					base.PowerSharpParameters["Enabled"] = value;
				}
			}

			public virtual TenantConnectorType ConnectorType
			{
				set
				{
					base.PowerSharpParameters["ConnectorType"] = value;
				}
			}

			public virtual TenantConnectorSource ConnectorSource
			{
				set
				{
					base.PowerSharpParameters["ConnectorSource"] = value;
				}
			}

			public virtual string Comment
			{
				set
				{
					base.PowerSharpParameters["Comment"] = value;
				}
			}

			public virtual MultiValuedProperty<IPRange> SenderIPAddresses
			{
				set
				{
					base.PowerSharpParameters["SenderIPAddresses"] = value;
				}
			}

			public virtual MultiValuedProperty<AddressSpace> SenderDomains
			{
				set
				{
					base.PowerSharpParameters["SenderDomains"] = value;
				}
			}

			public virtual bool RequireTls
			{
				set
				{
					base.PowerSharpParameters["RequireTls"] = value;
				}
			}

			public virtual bool RestrictDomainsToCertificate
			{
				set
				{
					base.PowerSharpParameters["RestrictDomainsToCertificate"] = value;
				}
			}

			public virtual bool RestrictDomainsToIPAddresses
			{
				set
				{
					base.PowerSharpParameters["RestrictDomainsToIPAddresses"] = value;
				}
			}

			public virtual bool CloudServicesMailEnabled
			{
				set
				{
					base.PowerSharpParameters["CloudServicesMailEnabled"] = value;
				}
			}

			public virtual TlsCertificate TlsSenderCertificateName
			{
				set
				{
					base.PowerSharpParameters["TlsSenderCertificateName"] = value;
				}
			}

			public virtual MultiValuedProperty<AcceptedDomainIdParameter> AssociatedAcceptedDomains
			{
				set
				{
					base.PowerSharpParameters["AssociatedAcceptedDomains"] = value;
				}
			}

			public virtual bool BypassValidation
			{
				set
				{
					base.PowerSharpParameters["BypassValidation"] = value;
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
