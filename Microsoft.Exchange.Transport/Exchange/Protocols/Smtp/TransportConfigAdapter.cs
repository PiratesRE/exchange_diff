using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.Transport.Configuration;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class TransportConfigAdapter : ITransportConfigProvider
	{
		public bool AcceptAndFixSmtpAddressWithInvalidLocalPart
		{
			get
			{
				return this.appConfig.SmtpDataConfiguration.AcceptAndFixSmtpAddressWithInvalidLocalPart;
			}
		}

		public bool AdvertiseADRecipientCache
		{
			get
			{
				return this.appConfig.MessageContextBlobConfiguration.AdvertiseADRecipientCache;
			}
		}

		public ByteQuantifiedSize AdrcCacheMaxBlobSize
		{
			get
			{
				return this.appConfig.MessageContextBlobConfiguration.AdrcCacheMaxBlobSize;
			}
		}

		public bool AdvertiseExtendedProperties
		{
			get
			{
				return this.appConfig.MessageContextBlobConfiguration.AdvertiseExtendedProperties;
			}
		}

		public bool AdvertiseFastIndex
		{
			get
			{
				return this.appConfig.MessageContextBlobConfiguration.AdvertiseFastIndex;
			}
		}

		public bool AntispamAgentsEnabled
		{
			get
			{
				return this.transportConfiguration.LocalServer.TransportServer.AntispamAgentsEnabled;
			}
		}

		public bool BlockedSessionLoggingEnabled
		{
			get
			{
				return this.appConfig.SmtpReceiveConfiguration.BlockedSessionLoggingEnabled;
			}
		}

		public bool ClientCertificateChainValidationEnabled
		{
			get
			{
				return this.appConfig.SecureMail.ClientCertificateChainValidationEnabled;
			}
		}

		public bool DisableHandleInheritance
		{
			get
			{
				return this.appConfig.SmtpReceiveConfiguration.DisableHandleInheritance;
			}
		}

		public bool EnableForwardingProhibitedFeature
		{
			get
			{
				return this.appConfig.Resolver.EnableForwardingProhibitedFeature;
			}
		}

		public bool ExclusiveAddressUse
		{
			get
			{
				return this.appConfig.SmtpReceiveConfiguration.ExclusiveAddressUse;
			}
		}

		public ByteQuantifiedSize ExtendedPropertiesMaxBlobSize
		{
			get
			{
				return this.appConfig.MessageContextBlobConfiguration.ExtendedPropertiesMaxBlobSize;
			}
		}

		public ByteQuantifiedSize FastIndexMaxBlobSize
		{
			get
			{
				return this.appConfig.MessageContextBlobConfiguration.FastIndexMaxBlobSize;
			}
		}

		public AcceptedDomainTable FirstOrgAcceptedDomainTable
		{
			get
			{
				return this.transportConfiguration.FirstOrgAcceptedDomainTable;
			}
		}

		public string Fqdn
		{
			get
			{
				return this.transportConfiguration.LocalServer.TransportServer.Fqdn;
			}
		}

		public bool GrantExchangeServerPermissions
		{
			get
			{
				return this.appConfig.SmtpReceiveConfiguration.GrantExchangeServerPermissions;
			}
		}

		public bool InboundApplyMissingEncoding
		{
			get
			{
				return this.appConfig.SmtpDataConfiguration.InboundApplyMissingEncoding;
			}
		}

		public MultiValuedProperty<IPRange> InternalSMTPServers
		{
			get
			{
				return this.transportConfiguration.TransportSettings.TransportSettings.InternalSMTPServers;
			}
		}

		public string InternalTransportCertificateThumbprint
		{
			get
			{
				return this.transportConfiguration.LocalServer.TransportServer.InternalTransportCertificateThumbprint;
			}
		}

		public bool IsFrontendTransportServer
		{
			get
			{
				return this.transportConfiguration.LocalServer.TransportServer.IsFrontendTransportServer;
			}
		}

		public bool IsHubTransportServer
		{
			get
			{
				return this.transportConfiguration.LocalServer.TransportServer.IsHubTransportServer;
			}
		}

		public bool IsIpv6ReceiveConnectionThrottlingEnabled
		{
			get
			{
				return this.isIpv6ReceiveConnectionThrottlingEnabled;
			}
		}

		public bool IsReceiveTlsThrottlingEnabled
		{
			get
			{
				return this.isReceiveTlsThrottlingEnabled;
			}
		}

		public TimeSpan KerberosTicketCacheFlushMinInterval
		{
			get
			{
				return this.appConfig.SmtpAvailabilityConfiguration.KerberosTicketCacheFlushMinInterval;
			}
		}

		public Server LocalServer
		{
			get
			{
				return this.transportConfiguration.LocalServer.TransportServer;
			}
		}

		public bool MailboxDeliveryAcceptAnonymousUsers
		{
			get
			{
				return this.appConfig.SmtpReceiveConfiguration.MailboxDeliveryAcceptAnonymousUsers;
			}
		}

		public Unlimited<ByteQuantifiedSize> MaxSendSize
		{
			get
			{
				return this.transportConfiguration.TransportSettings.TransportSettings.MaxSendSize;
			}
		}

		public int MaxTlsConnectionsPerMinute
		{
			get
			{
				return this.transportConfiguration.LocalServer.TransportServer.MaxReceiveTlsRatePerMinute;
			}
		}

		public int NetworkConnectionReceiveBufferSize
		{
			get
			{
				return this.appConfig.SmtpReceiveConfiguration.NetworkConnectionReceiveBufferSize;
			}
		}

		public TransportAppConfig.ISmtpMailCommandConfig MailSmtpCommandConfig
		{
			get
			{
				return this.appConfig.SmtpMailCommandConfiguration;
			}
		}

		public int MaxProxyHopCount
		{
			get
			{
				return this.appConfig.SmtpReceiveConfiguration.MaxProxyHopCount;
			}
		}

		public string PhysicalMachineName
		{
			get
			{
				return TransportConfigAdapter.physicalMachineName.Value;
			}
		}

		public bool OneLevelWildcardMatchForCertSelection
		{
			get
			{
				return this.appConfig.SmtpReceiveConfiguration.OneLevelWildcardMatchForCertSelection;
			}
		}

		public Guid OrganizationGuid
		{
			get
			{
				return this.transportConfiguration.TransportSettings.OrganizationGuid;
			}
		}

		public bool OutboundRejectBareLinefeeds
		{
			get
			{
				return this.appConfig.SmtpDataConfiguration.OutboundRejectBareLinefeeds;
			}
		}

		public ProcessTransportRole ProcessTransportRole
		{
			get
			{
				return this.transportConfiguration.ProcessTransportRole;
			}
		}

		public bool RejectUnscopedMessages
		{
			get
			{
				return this.appConfig.SmtpReceiveConfiguration.RejectUnscopedMessages;
			}
		}

		public RemoteDomainTable RemoteDomainTable
		{
			get
			{
				return this.transportConfiguration.RemoteDomainTable;
			}
		}

		public bool SmtpAcceptAnyRecipient
		{
			get
			{
				return this.appConfig.SmtpReceiveConfiguration.SMTPAcceptAnyRecipient;
			}
		}

		public int SmtpAvailabilityMinConnectionsToMonitor
		{
			get
			{
				return this.appConfig.SmtpAvailabilityConfiguration.SmtpAvailabilityMinConnectionsToMonitor;
			}
		}

		public string SmtpDataResponse
		{
			get
			{
				return this.appConfig.SmtpDataConfiguration.SmtpDataResponse;
			}
		}

		public int SubjectAlternativeNameLimit
		{
			get
			{
				return this.appConfig.SecureMail.SubjectAlternativeNameLimit;
			}
		}

		public bool TarpitMuaSubmission
		{
			get
			{
				return this.appConfig.SmtpReceiveConfiguration.TarpitMuaSubmission;
			}
		}

		public IEnumerable<SmtpDomain> TlsReceiveDomainSecureList
		{
			get
			{
				return this.transportConfiguration.TransportSettings.TransportSettings.TLSReceiveDomainSecureList;
			}
		}

		public bool TransferADRecipientCache
		{
			get
			{
				return this.appConfig.MessageContextBlobConfiguration.TransferADRecipientCache;
			}
		}

		public bool TransferExtendedProperties
		{
			get
			{
				return this.appConfig.MessageContextBlobConfiguration.TransferExtendedProperties;
			}
		}

		public bool TransferFastIndex
		{
			get
			{
				return this.appConfig.MessageContextBlobConfiguration.TransferFastIndex;
			}
		}

		public bool TreatCRLTransientFailuresAsSuccessEnabled
		{
			get
			{
				return this.appConfig.SecureMail.TreatCRLTransientFailuresAsSuccessEnabled;
			}
		}

		public Version Version
		{
			get
			{
				return this.transportConfiguration.LocalServer.TransportServer.AdminDisplayVersion;
			}
		}

		public bool WaitForSmtpSessionsAtShutdown
		{
			get
			{
				return this.appConfig.SmtpReceiveConfiguration.WaitForSmtpSessionsAtShutdown;
			}
		}

		public bool WatsonReportOnFailureEnabled
		{
			get
			{
				return this.appConfig.MessageContextBlobConfiguration.WatsonReportOnFailureEnabled;
			}
		}

		public bool Xexch50Enabled
		{
			get
			{
				return this.transportConfiguration.TransportSettings.TransportSettings.Xexch50Enabled;
			}
		}

		public static ITransportConfigProvider Create(ITransportAppConfig appConfig, ITransportConfiguration transportConfiguration)
		{
			ArgumentValidator.ThrowIfNull("appConfig", appConfig);
			ArgumentValidator.ThrowIfNull("transportConfiguration", transportConfiguration);
			return new TransportConfigAdapter(appConfig, transportConfiguration);
		}

		public bool IsTlsReceiveSecureDomain(string domainName)
		{
			return this.transportConfiguration.TransportSettings.TransportSettings.IsTLSReceiveSecureDomain(domainName);
		}

		public PerTenantAcceptedDomainTable GetAcceptedDomainTable(OrganizationId orgId)
		{
			return this.transportConfiguration.GetAcceptedDomainTable(orgId);
		}

		private TransportConfigAdapter(ITransportAppConfig appConfig, ITransportConfiguration transportConfiguration)
		{
			this.appConfig = appConfig;
			this.transportConfiguration = transportConfiguration;
			this.isIpv6ReceiveConnectionThrottlingEnabled = this.appConfig.SmtpReceiveConfiguration.Ipv6ReceiveConnectionThrottlingEnabled;
			this.isReceiveTlsThrottlingEnabled = this.appConfig.SmtpReceiveConfiguration.ReceiveTlsThrottlingEnabled;
		}

		private readonly bool isIpv6ReceiveConnectionThrottlingEnabled;

		private readonly bool isReceiveTlsThrottlingEnabled;

		private static readonly Lazy<string> physicalMachineName = new Lazy<string>(() => ComputerInformation.DnsPhysicalFullyQualifiedDomainName);

		private readonly ITransportAppConfig appConfig;

		private readonly ITransportConfiguration transportConfiguration;
	}
}
