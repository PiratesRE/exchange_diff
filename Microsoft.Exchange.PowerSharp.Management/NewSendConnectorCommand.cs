using System;
using System.Management.Automation;
using System.Net;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.SystemConfigurationTasks;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class NewSendConnectorCommand : SyntheticCommandWithPipelineInput<SmtpSendConnectorConfig, SmtpSendConnectorConfig>
	{
		private NewSendConnectorCommand() : base("New-SendConnector")
		{
		}

		public NewSendConnectorCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual NewSendConnectorCommand SetParameters(NewSendConnectorCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual NewSendConnectorCommand SetParameters(NewSendConnectorCommand.AddressSpacesParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual NewSendConnector.UsageType Usage
			{
				set
				{
					base.PowerSharpParameters["Usage"] = value;
				}
			}

			public virtual SwitchParameter Internet
			{
				set
				{
					base.PowerSharpParameters["Internet"] = value;
				}
			}

			public virtual SwitchParameter Internal
			{
				set
				{
					base.PowerSharpParameters["Internal"] = value;
				}
			}

			public virtual SwitchParameter Partner
			{
				set
				{
					base.PowerSharpParameters["Partner"] = value;
				}
			}

			public virtual SwitchParameter Custom
			{
				set
				{
					base.PowerSharpParameters["Custom"] = value;
				}
			}

			public virtual SwitchParameter Force
			{
				set
				{
					base.PowerSharpParameters["Force"] = value;
				}
			}

			public virtual bool DomainSecureEnabled
			{
				set
				{
					base.PowerSharpParameters["DomainSecureEnabled"] = value;
				}
			}

			public virtual bool DNSRoutingEnabled
			{
				set
				{
					base.PowerSharpParameters["DNSRoutingEnabled"] = value;
				}
			}

			public virtual MultiValuedProperty<SmartHost> SmartHosts
			{
				set
				{
					base.PowerSharpParameters["SmartHosts"] = value;
				}
			}

			public virtual int Port
			{
				set
				{
					base.PowerSharpParameters["Port"] = value;
				}
			}

			public virtual EnhancedTimeSpan ConnectionInactivityTimeOut
			{
				set
				{
					base.PowerSharpParameters["ConnectionInactivityTimeOut"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> MaxMessageSize
			{
				set
				{
					base.PowerSharpParameters["MaxMessageSize"] = value;
				}
			}

			public virtual Fqdn Fqdn
			{
				set
				{
					base.PowerSharpParameters["Fqdn"] = value;
				}
			}

			public virtual SmtpX509Identifier TlsCertificateName
			{
				set
				{
					base.PowerSharpParameters["TlsCertificateName"] = value;
				}
			}

			public virtual bool ForceHELO
			{
				set
				{
					base.PowerSharpParameters["ForceHELO"] = value;
				}
			}

			public virtual bool FrontendProxyEnabled
			{
				set
				{
					base.PowerSharpParameters["FrontendProxyEnabled"] = value;
				}
			}

			public virtual bool IgnoreSTARTTLS
			{
				set
				{
					base.PowerSharpParameters["IgnoreSTARTTLS"] = value;
				}
			}

			public virtual bool CloudServicesMailEnabled
			{
				set
				{
					base.PowerSharpParameters["CloudServicesMailEnabled"] = value;
				}
			}

			public virtual bool RequireOorg
			{
				set
				{
					base.PowerSharpParameters["RequireOorg"] = value;
				}
			}

			public virtual bool RequireTLS
			{
				set
				{
					base.PowerSharpParameters["RequireTLS"] = value;
				}
			}

			public virtual bool Enabled
			{
				set
				{
					base.PowerSharpParameters["Enabled"] = value;
				}
			}

			public virtual ProtocolLoggingLevel ProtocolLoggingLevel
			{
				set
				{
					base.PowerSharpParameters["ProtocolLoggingLevel"] = value;
				}
			}

			public virtual SmtpSendConnectorConfig.AuthMechanisms SmartHostAuthMechanism
			{
				set
				{
					base.PowerSharpParameters["SmartHostAuthMechanism"] = value;
				}
			}

			public virtual bool UseExternalDNSServersEnabled
			{
				set
				{
					base.PowerSharpParameters["UseExternalDNSServersEnabled"] = value;
				}
			}

			public virtual string Comment
			{
				set
				{
					base.PowerSharpParameters["Comment"] = value;
				}
			}

			public virtual IPAddress SourceIPAddress
			{
				set
				{
					base.PowerSharpParameters["SourceIPAddress"] = value;
				}
			}

			public virtual int SmtpMaxMessagesPerConnection
			{
				set
				{
					base.PowerSharpParameters["SmtpMaxMessagesPerConnection"] = value;
				}
			}

			public virtual PSCredential AuthenticationCredential
			{
				set
				{
					base.PowerSharpParameters["AuthenticationCredential"] = value;
				}
			}

			public virtual MultiValuedProperty<ServerIdParameter> SourceTransportServers
			{
				set
				{
					base.PowerSharpParameters["SourceTransportServers"] = value;
				}
			}

			public virtual SmtpDomainWithSubdomains TlsDomain
			{
				set
				{
					base.PowerSharpParameters["TlsDomain"] = value;
				}
			}

			public virtual TlsAuthLevel? TlsAuthLevel
			{
				set
				{
					base.PowerSharpParameters["TlsAuthLevel"] = value;
				}
			}

			public virtual ErrorPolicies ErrorPolicies
			{
				set
				{
					base.PowerSharpParameters["ErrorPolicies"] = value;
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

		public class AddressSpacesParameters : ParametersBase
		{
			public virtual MultiValuedProperty<AddressSpace> AddressSpaces
			{
				set
				{
					base.PowerSharpParameters["AddressSpaces"] = value;
				}
			}

			public virtual bool IsScopedConnector
			{
				set
				{
					base.PowerSharpParameters["IsScopedConnector"] = value;
				}
			}

			public virtual NewSendConnector.UsageType Usage
			{
				set
				{
					base.PowerSharpParameters["Usage"] = value;
				}
			}

			public virtual SwitchParameter Internet
			{
				set
				{
					base.PowerSharpParameters["Internet"] = value;
				}
			}

			public virtual SwitchParameter Internal
			{
				set
				{
					base.PowerSharpParameters["Internal"] = value;
				}
			}

			public virtual SwitchParameter Partner
			{
				set
				{
					base.PowerSharpParameters["Partner"] = value;
				}
			}

			public virtual SwitchParameter Custom
			{
				set
				{
					base.PowerSharpParameters["Custom"] = value;
				}
			}

			public virtual SwitchParameter Force
			{
				set
				{
					base.PowerSharpParameters["Force"] = value;
				}
			}

			public virtual bool DomainSecureEnabled
			{
				set
				{
					base.PowerSharpParameters["DomainSecureEnabled"] = value;
				}
			}

			public virtual bool DNSRoutingEnabled
			{
				set
				{
					base.PowerSharpParameters["DNSRoutingEnabled"] = value;
				}
			}

			public virtual MultiValuedProperty<SmartHost> SmartHosts
			{
				set
				{
					base.PowerSharpParameters["SmartHosts"] = value;
				}
			}

			public virtual int Port
			{
				set
				{
					base.PowerSharpParameters["Port"] = value;
				}
			}

			public virtual EnhancedTimeSpan ConnectionInactivityTimeOut
			{
				set
				{
					base.PowerSharpParameters["ConnectionInactivityTimeOut"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> MaxMessageSize
			{
				set
				{
					base.PowerSharpParameters["MaxMessageSize"] = value;
				}
			}

			public virtual Fqdn Fqdn
			{
				set
				{
					base.PowerSharpParameters["Fqdn"] = value;
				}
			}

			public virtual SmtpX509Identifier TlsCertificateName
			{
				set
				{
					base.PowerSharpParameters["TlsCertificateName"] = value;
				}
			}

			public virtual bool ForceHELO
			{
				set
				{
					base.PowerSharpParameters["ForceHELO"] = value;
				}
			}

			public virtual bool FrontendProxyEnabled
			{
				set
				{
					base.PowerSharpParameters["FrontendProxyEnabled"] = value;
				}
			}

			public virtual bool IgnoreSTARTTLS
			{
				set
				{
					base.PowerSharpParameters["IgnoreSTARTTLS"] = value;
				}
			}

			public virtual bool CloudServicesMailEnabled
			{
				set
				{
					base.PowerSharpParameters["CloudServicesMailEnabled"] = value;
				}
			}

			public virtual bool RequireOorg
			{
				set
				{
					base.PowerSharpParameters["RequireOorg"] = value;
				}
			}

			public virtual bool RequireTLS
			{
				set
				{
					base.PowerSharpParameters["RequireTLS"] = value;
				}
			}

			public virtual bool Enabled
			{
				set
				{
					base.PowerSharpParameters["Enabled"] = value;
				}
			}

			public virtual ProtocolLoggingLevel ProtocolLoggingLevel
			{
				set
				{
					base.PowerSharpParameters["ProtocolLoggingLevel"] = value;
				}
			}

			public virtual SmtpSendConnectorConfig.AuthMechanisms SmartHostAuthMechanism
			{
				set
				{
					base.PowerSharpParameters["SmartHostAuthMechanism"] = value;
				}
			}

			public virtual bool UseExternalDNSServersEnabled
			{
				set
				{
					base.PowerSharpParameters["UseExternalDNSServersEnabled"] = value;
				}
			}

			public virtual string Comment
			{
				set
				{
					base.PowerSharpParameters["Comment"] = value;
				}
			}

			public virtual IPAddress SourceIPAddress
			{
				set
				{
					base.PowerSharpParameters["SourceIPAddress"] = value;
				}
			}

			public virtual int SmtpMaxMessagesPerConnection
			{
				set
				{
					base.PowerSharpParameters["SmtpMaxMessagesPerConnection"] = value;
				}
			}

			public virtual PSCredential AuthenticationCredential
			{
				set
				{
					base.PowerSharpParameters["AuthenticationCredential"] = value;
				}
			}

			public virtual MultiValuedProperty<ServerIdParameter> SourceTransportServers
			{
				set
				{
					base.PowerSharpParameters["SourceTransportServers"] = value;
				}
			}

			public virtual SmtpDomainWithSubdomains TlsDomain
			{
				set
				{
					base.PowerSharpParameters["TlsDomain"] = value;
				}
			}

			public virtual TlsAuthLevel? TlsAuthLevel
			{
				set
				{
					base.PowerSharpParameters["TlsAuthLevel"] = value;
				}
			}

			public virtual ErrorPolicies ErrorPolicies
			{
				set
				{
					base.PowerSharpParameters["ErrorPolicies"] = value;
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
