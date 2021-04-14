using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class NewOutboundConnectorCommand : SyntheticCommandWithPipelineInput<TenantOutboundConnector, TenantOutboundConnector>
	{
		private NewOutboundConnectorCommand() : base("New-OutboundConnector")
		{
		}

		public NewOutboundConnectorCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual NewOutboundConnectorCommand SetParameters(NewOutboundConnectorCommand.DefaultParameters parameters)
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

			public virtual bool UseMXRecord
			{
				set
				{
					base.PowerSharpParameters["UseMXRecord"] = value;
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

			public virtual MultiValuedProperty<SmtpDomainWithSubdomains> RecipientDomains
			{
				set
				{
					base.PowerSharpParameters["RecipientDomains"] = value;
				}
			}

			public virtual MultiValuedProperty<SmartHost> SmartHosts
			{
				set
				{
					base.PowerSharpParameters["SmartHosts"] = value;
				}
			}

			public virtual SmtpDomainWithSubdomains TlsDomain
			{
				set
				{
					base.PowerSharpParameters["TlsDomain"] = value;
				}
			}

			public virtual TlsAuthLevel? TlsSettings
			{
				set
				{
					base.PowerSharpParameters["TlsSettings"] = value;
				}
			}

			public virtual bool IsTransportRuleScoped
			{
				set
				{
					base.PowerSharpParameters["IsTransportRuleScoped"] = value;
				}
			}

			public virtual bool RouteAllMessagesViaOnPremises
			{
				set
				{
					base.PowerSharpParameters["RouteAllMessagesViaOnPremises"] = value;
				}
			}

			public virtual bool BypassValidation
			{
				set
				{
					base.PowerSharpParameters["BypassValidation"] = value;
				}
			}

			public virtual bool CloudServicesMailEnabled
			{
				set
				{
					base.PowerSharpParameters["CloudServicesMailEnabled"] = value;
				}
			}

			public virtual bool AllAcceptedDomains
			{
				set
				{
					base.PowerSharpParameters["AllAcceptedDomains"] = value;
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
