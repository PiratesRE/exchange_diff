using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetReceiveConnectorCommand : SyntheticCommandWithPipelineInputNoOutput<ReceiveConnector>
	{
		private SetReceiveConnectorCommand() : base("Set-ReceiveConnector")
		{
		}

		public SetReceiveConnectorCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetReceiveConnectorCommand SetParameters(SetReceiveConnectorCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetReceiveConnectorCommand SetParameters(SetReceiveConnectorCommand.IdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual AcceptedDomainIdParameter DefaultDomain
			{
				set
				{
					base.PowerSharpParameters["DefaultDomain"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual AuthMechanisms AuthMechanism
			{
				set
				{
					base.PowerSharpParameters["AuthMechanism"] = value;
				}
			}

			public virtual string Banner
			{
				set
				{
					base.PowerSharpParameters["Banner"] = value;
				}
			}

			public virtual bool BinaryMimeEnabled
			{
				set
				{
					base.PowerSharpParameters["BinaryMimeEnabled"] = value;
				}
			}

			public virtual MultiValuedProperty<IPBinding> Bindings
			{
				set
				{
					base.PowerSharpParameters["Bindings"] = value;
				}
			}

			public virtual bool ChunkingEnabled
			{
				set
				{
					base.PowerSharpParameters["ChunkingEnabled"] = value;
				}
			}

			public virtual bool DeliveryStatusNotificationEnabled
			{
				set
				{
					base.PowerSharpParameters["DeliveryStatusNotificationEnabled"] = value;
				}
			}

			public virtual bool EightBitMimeEnabled
			{
				set
				{
					base.PowerSharpParameters["EightBitMimeEnabled"] = value;
				}
			}

			public virtual bool SmtpUtf8Enabled
			{
				set
				{
					base.PowerSharpParameters["SmtpUtf8Enabled"] = value;
				}
			}

			public virtual bool BareLinefeedRejectionEnabled
			{
				set
				{
					base.PowerSharpParameters["BareLinefeedRejectionEnabled"] = value;
				}
			}

			public virtual bool DomainSecureEnabled
			{
				set
				{
					base.PowerSharpParameters["DomainSecureEnabled"] = value;
				}
			}

			public virtual bool EnhancedStatusCodesEnabled
			{
				set
				{
					base.PowerSharpParameters["EnhancedStatusCodesEnabled"] = value;
				}
			}

			public virtual bool LongAddressesEnabled
			{
				set
				{
					base.PowerSharpParameters["LongAddressesEnabled"] = value;
				}
			}

			public virtual bool OrarEnabled
			{
				set
				{
					base.PowerSharpParameters["OrarEnabled"] = value;
				}
			}

			public virtual bool SuppressXAnonymousTls
			{
				set
				{
					base.PowerSharpParameters["SuppressXAnonymousTls"] = value;
				}
			}

			public virtual bool ProxyEnabled
			{
				set
				{
					base.PowerSharpParameters["ProxyEnabled"] = value;
				}
			}

			public virtual bool AdvertiseClientSettings
			{
				set
				{
					base.PowerSharpParameters["AdvertiseClientSettings"] = value;
				}
			}

			public virtual Fqdn Fqdn
			{
				set
				{
					base.PowerSharpParameters["Fqdn"] = value;
				}
			}

			public virtual Fqdn ServiceDiscoveryFqdn
			{
				set
				{
					base.PowerSharpParameters["ServiceDiscoveryFqdn"] = value;
				}
			}

			public virtual SmtpX509Identifier TlsCertificateName
			{
				set
				{
					base.PowerSharpParameters["TlsCertificateName"] = value;
				}
			}

			public virtual string Comment
			{
				set
				{
					base.PowerSharpParameters["Comment"] = value;
				}
			}

			public virtual bool Enabled
			{
				set
				{
					base.PowerSharpParameters["Enabled"] = value;
				}
			}

			public virtual EnhancedTimeSpan ConnectionTimeout
			{
				set
				{
					base.PowerSharpParameters["ConnectionTimeout"] = value;
				}
			}

			public virtual EnhancedTimeSpan ConnectionInactivityTimeout
			{
				set
				{
					base.PowerSharpParameters["ConnectionInactivityTimeout"] = value;
				}
			}

			public virtual Unlimited<int> MessageRateLimit
			{
				set
				{
					base.PowerSharpParameters["MessageRateLimit"] = value;
				}
			}

			public virtual MessageRateSourceFlags MessageRateSource
			{
				set
				{
					base.PowerSharpParameters["MessageRateSource"] = value;
				}
			}

			public virtual Unlimited<int> MaxInboundConnection
			{
				set
				{
					base.PowerSharpParameters["MaxInboundConnection"] = value;
				}
			}

			public virtual Unlimited<int> MaxInboundConnectionPerSource
			{
				set
				{
					base.PowerSharpParameters["MaxInboundConnectionPerSource"] = value;
				}
			}

			public virtual int MaxInboundConnectionPercentagePerSource
			{
				set
				{
					base.PowerSharpParameters["MaxInboundConnectionPercentagePerSource"] = value;
				}
			}

			public virtual ByteQuantifiedSize MaxHeaderSize
			{
				set
				{
					base.PowerSharpParameters["MaxHeaderSize"] = value;
				}
			}

			public virtual int MaxHopCount
			{
				set
				{
					base.PowerSharpParameters["MaxHopCount"] = value;
				}
			}

			public virtual int MaxLocalHopCount
			{
				set
				{
					base.PowerSharpParameters["MaxLocalHopCount"] = value;
				}
			}

			public virtual int MaxLogonFailures
			{
				set
				{
					base.PowerSharpParameters["MaxLogonFailures"] = value;
				}
			}

			public virtual ByteQuantifiedSize MaxMessageSize
			{
				set
				{
					base.PowerSharpParameters["MaxMessageSize"] = value;
				}
			}

			public virtual Unlimited<int> MaxProtocolErrors
			{
				set
				{
					base.PowerSharpParameters["MaxProtocolErrors"] = value;
				}
			}

			public virtual int MaxRecipientsPerMessage
			{
				set
				{
					base.PowerSharpParameters["MaxRecipientsPerMessage"] = value;
				}
			}

			public virtual PermissionGroups PermissionGroups
			{
				set
				{
					base.PowerSharpParameters["PermissionGroups"] = value;
				}
			}

			public virtual bool PipeliningEnabled
			{
				set
				{
					base.PowerSharpParameters["PipeliningEnabled"] = value;
				}
			}

			public virtual ProtocolLoggingLevel ProtocolLoggingLevel
			{
				set
				{
					base.PowerSharpParameters["ProtocolLoggingLevel"] = value;
				}
			}

			public virtual MultiValuedProperty<IPRange> RemoteIPRanges
			{
				set
				{
					base.PowerSharpParameters["RemoteIPRanges"] = value;
				}
			}

			public virtual bool RequireEHLODomain
			{
				set
				{
					base.PowerSharpParameters["RequireEHLODomain"] = value;
				}
			}

			public virtual bool RequireTLS
			{
				set
				{
					base.PowerSharpParameters["RequireTLS"] = value;
				}
			}

			public virtual bool EnableAuthGSSAPI
			{
				set
				{
					base.PowerSharpParameters["EnableAuthGSSAPI"] = value;
				}
			}

			public virtual ExtendedProtectionPolicySetting ExtendedProtectionPolicy
			{
				set
				{
					base.PowerSharpParameters["ExtendedProtectionPolicy"] = value;
				}
			}

			public virtual bool LiveCredentialEnabled
			{
				set
				{
					base.PowerSharpParameters["LiveCredentialEnabled"] = value;
				}
			}

			public virtual MultiValuedProperty<SmtpReceiveDomainCapabilities> TlsDomainCapabilities
			{
				set
				{
					base.PowerSharpParameters["TlsDomainCapabilities"] = value;
				}
			}

			public virtual ServerRole TransportRole
			{
				set
				{
					base.PowerSharpParameters["TransportRole"] = value;
				}
			}

			public virtual SizeMode SizeEnabled
			{
				set
				{
					base.PowerSharpParameters["SizeEnabled"] = value;
				}
			}

			public virtual EnhancedTimeSpan TarpitInterval
			{
				set
				{
					base.PowerSharpParameters["TarpitInterval"] = value;
				}
			}

			public virtual EnhancedTimeSpan MaxAcknowledgementDelay
			{
				set
				{
					base.PowerSharpParameters["MaxAcknowledgementDelay"] = value;
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
			public virtual ReceiveConnectorIdParameter Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = value;
				}
			}

			public virtual AcceptedDomainIdParameter DefaultDomain
			{
				set
				{
					base.PowerSharpParameters["DefaultDomain"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual AuthMechanisms AuthMechanism
			{
				set
				{
					base.PowerSharpParameters["AuthMechanism"] = value;
				}
			}

			public virtual string Banner
			{
				set
				{
					base.PowerSharpParameters["Banner"] = value;
				}
			}

			public virtual bool BinaryMimeEnabled
			{
				set
				{
					base.PowerSharpParameters["BinaryMimeEnabled"] = value;
				}
			}

			public virtual MultiValuedProperty<IPBinding> Bindings
			{
				set
				{
					base.PowerSharpParameters["Bindings"] = value;
				}
			}

			public virtual bool ChunkingEnabled
			{
				set
				{
					base.PowerSharpParameters["ChunkingEnabled"] = value;
				}
			}

			public virtual bool DeliveryStatusNotificationEnabled
			{
				set
				{
					base.PowerSharpParameters["DeliveryStatusNotificationEnabled"] = value;
				}
			}

			public virtual bool EightBitMimeEnabled
			{
				set
				{
					base.PowerSharpParameters["EightBitMimeEnabled"] = value;
				}
			}

			public virtual bool SmtpUtf8Enabled
			{
				set
				{
					base.PowerSharpParameters["SmtpUtf8Enabled"] = value;
				}
			}

			public virtual bool BareLinefeedRejectionEnabled
			{
				set
				{
					base.PowerSharpParameters["BareLinefeedRejectionEnabled"] = value;
				}
			}

			public virtual bool DomainSecureEnabled
			{
				set
				{
					base.PowerSharpParameters["DomainSecureEnabled"] = value;
				}
			}

			public virtual bool EnhancedStatusCodesEnabled
			{
				set
				{
					base.PowerSharpParameters["EnhancedStatusCodesEnabled"] = value;
				}
			}

			public virtual bool LongAddressesEnabled
			{
				set
				{
					base.PowerSharpParameters["LongAddressesEnabled"] = value;
				}
			}

			public virtual bool OrarEnabled
			{
				set
				{
					base.PowerSharpParameters["OrarEnabled"] = value;
				}
			}

			public virtual bool SuppressXAnonymousTls
			{
				set
				{
					base.PowerSharpParameters["SuppressXAnonymousTls"] = value;
				}
			}

			public virtual bool ProxyEnabled
			{
				set
				{
					base.PowerSharpParameters["ProxyEnabled"] = value;
				}
			}

			public virtual bool AdvertiseClientSettings
			{
				set
				{
					base.PowerSharpParameters["AdvertiseClientSettings"] = value;
				}
			}

			public virtual Fqdn Fqdn
			{
				set
				{
					base.PowerSharpParameters["Fqdn"] = value;
				}
			}

			public virtual Fqdn ServiceDiscoveryFqdn
			{
				set
				{
					base.PowerSharpParameters["ServiceDiscoveryFqdn"] = value;
				}
			}

			public virtual SmtpX509Identifier TlsCertificateName
			{
				set
				{
					base.PowerSharpParameters["TlsCertificateName"] = value;
				}
			}

			public virtual string Comment
			{
				set
				{
					base.PowerSharpParameters["Comment"] = value;
				}
			}

			public virtual bool Enabled
			{
				set
				{
					base.PowerSharpParameters["Enabled"] = value;
				}
			}

			public virtual EnhancedTimeSpan ConnectionTimeout
			{
				set
				{
					base.PowerSharpParameters["ConnectionTimeout"] = value;
				}
			}

			public virtual EnhancedTimeSpan ConnectionInactivityTimeout
			{
				set
				{
					base.PowerSharpParameters["ConnectionInactivityTimeout"] = value;
				}
			}

			public virtual Unlimited<int> MessageRateLimit
			{
				set
				{
					base.PowerSharpParameters["MessageRateLimit"] = value;
				}
			}

			public virtual MessageRateSourceFlags MessageRateSource
			{
				set
				{
					base.PowerSharpParameters["MessageRateSource"] = value;
				}
			}

			public virtual Unlimited<int> MaxInboundConnection
			{
				set
				{
					base.PowerSharpParameters["MaxInboundConnection"] = value;
				}
			}

			public virtual Unlimited<int> MaxInboundConnectionPerSource
			{
				set
				{
					base.PowerSharpParameters["MaxInboundConnectionPerSource"] = value;
				}
			}

			public virtual int MaxInboundConnectionPercentagePerSource
			{
				set
				{
					base.PowerSharpParameters["MaxInboundConnectionPercentagePerSource"] = value;
				}
			}

			public virtual ByteQuantifiedSize MaxHeaderSize
			{
				set
				{
					base.PowerSharpParameters["MaxHeaderSize"] = value;
				}
			}

			public virtual int MaxHopCount
			{
				set
				{
					base.PowerSharpParameters["MaxHopCount"] = value;
				}
			}

			public virtual int MaxLocalHopCount
			{
				set
				{
					base.PowerSharpParameters["MaxLocalHopCount"] = value;
				}
			}

			public virtual int MaxLogonFailures
			{
				set
				{
					base.PowerSharpParameters["MaxLogonFailures"] = value;
				}
			}

			public virtual ByteQuantifiedSize MaxMessageSize
			{
				set
				{
					base.PowerSharpParameters["MaxMessageSize"] = value;
				}
			}

			public virtual Unlimited<int> MaxProtocolErrors
			{
				set
				{
					base.PowerSharpParameters["MaxProtocolErrors"] = value;
				}
			}

			public virtual int MaxRecipientsPerMessage
			{
				set
				{
					base.PowerSharpParameters["MaxRecipientsPerMessage"] = value;
				}
			}

			public virtual PermissionGroups PermissionGroups
			{
				set
				{
					base.PowerSharpParameters["PermissionGroups"] = value;
				}
			}

			public virtual bool PipeliningEnabled
			{
				set
				{
					base.PowerSharpParameters["PipeliningEnabled"] = value;
				}
			}

			public virtual ProtocolLoggingLevel ProtocolLoggingLevel
			{
				set
				{
					base.PowerSharpParameters["ProtocolLoggingLevel"] = value;
				}
			}

			public virtual MultiValuedProperty<IPRange> RemoteIPRanges
			{
				set
				{
					base.PowerSharpParameters["RemoteIPRanges"] = value;
				}
			}

			public virtual bool RequireEHLODomain
			{
				set
				{
					base.PowerSharpParameters["RequireEHLODomain"] = value;
				}
			}

			public virtual bool RequireTLS
			{
				set
				{
					base.PowerSharpParameters["RequireTLS"] = value;
				}
			}

			public virtual bool EnableAuthGSSAPI
			{
				set
				{
					base.PowerSharpParameters["EnableAuthGSSAPI"] = value;
				}
			}

			public virtual ExtendedProtectionPolicySetting ExtendedProtectionPolicy
			{
				set
				{
					base.PowerSharpParameters["ExtendedProtectionPolicy"] = value;
				}
			}

			public virtual bool LiveCredentialEnabled
			{
				set
				{
					base.PowerSharpParameters["LiveCredentialEnabled"] = value;
				}
			}

			public virtual MultiValuedProperty<SmtpReceiveDomainCapabilities> TlsDomainCapabilities
			{
				set
				{
					base.PowerSharpParameters["TlsDomainCapabilities"] = value;
				}
			}

			public virtual ServerRole TransportRole
			{
				set
				{
					base.PowerSharpParameters["TransportRole"] = value;
				}
			}

			public virtual SizeMode SizeEnabled
			{
				set
				{
					base.PowerSharpParameters["SizeEnabled"] = value;
				}
			}

			public virtual EnhancedTimeSpan TarpitInterval
			{
				set
				{
					base.PowerSharpParameters["TarpitInterval"] = value;
				}
			}

			public virtual EnhancedTimeSpan MaxAcknowledgementDelay
			{
				set
				{
					base.PowerSharpParameters["MaxAcknowledgementDelay"] = value;
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
