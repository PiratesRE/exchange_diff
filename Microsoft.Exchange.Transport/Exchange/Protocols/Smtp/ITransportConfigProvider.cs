using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.Transport.Configuration;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal interface ITransportConfigProvider
	{
		bool AcceptAndFixSmtpAddressWithInvalidLocalPart { get; }

		bool AdvertiseADRecipientCache { get; }

		ByteQuantifiedSize AdrcCacheMaxBlobSize { get; }

		bool AdvertiseExtendedProperties { get; }

		bool AdvertiseFastIndex { get; }

		bool AntispamAgentsEnabled { get; }

		bool BlockedSessionLoggingEnabled { get; }

		bool ClientCertificateChainValidationEnabled { get; }

		bool DisableHandleInheritance { get; }

		bool EnableForwardingProhibitedFeature { get; }

		bool ExclusiveAddressUse { get; }

		ByteQuantifiedSize ExtendedPropertiesMaxBlobSize { get; }

		ByteQuantifiedSize FastIndexMaxBlobSize { get; }

		AcceptedDomainTable FirstOrgAcceptedDomainTable { get; }

		string Fqdn { get; }

		bool GrantExchangeServerPermissions { get; }

		bool InboundApplyMissingEncoding { get; }

		MultiValuedProperty<IPRange> InternalSMTPServers { get; }

		string InternalTransportCertificateThumbprint { get; }

		bool IsFrontendTransportServer { get; }

		bool IsHubTransportServer { get; }

		bool IsIpv6ReceiveConnectionThrottlingEnabled { get; }

		bool IsReceiveTlsThrottlingEnabled { get; }

		TimeSpan KerberosTicketCacheFlushMinInterval { get; }

		Server LocalServer { get; }

		TransportAppConfig.ISmtpMailCommandConfig MailSmtpCommandConfig { get; }

		bool MailboxDeliveryAcceptAnonymousUsers { get; }

		int MaxProxyHopCount { get; }

		Unlimited<ByteQuantifiedSize> MaxSendSize { get; }

		int MaxTlsConnectionsPerMinute { get; }

		int NetworkConnectionReceiveBufferSize { get; }

		string PhysicalMachineName { get; }

		bool OneLevelWildcardMatchForCertSelection { get; }

		Guid OrganizationGuid { get; }

		bool OutboundRejectBareLinefeeds { get; }

		ProcessTransportRole ProcessTransportRole { get; }

		bool RejectUnscopedMessages { get; }

		RemoteDomainTable RemoteDomainTable { get; }

		bool SmtpAcceptAnyRecipient { get; }

		int SmtpAvailabilityMinConnectionsToMonitor { get; }

		string SmtpDataResponse { get; }

		int SubjectAlternativeNameLimit { get; }

		bool TarpitMuaSubmission { get; }

		IEnumerable<SmtpDomain> TlsReceiveDomainSecureList { get; }

		bool TransferADRecipientCache { get; }

		bool TransferExtendedProperties { get; }

		bool TransferFastIndex { get; }

		bool TreatCRLTransientFailuresAsSuccessEnabled { get; }

		Version Version { get; }

		bool WaitForSmtpSessionsAtShutdown { get; }

		bool WatsonReportOnFailureEnabled { get; }

		bool Xexch50Enabled { get; }

		bool IsTlsReceiveSecureDomain(string domainName);

		PerTenantAcceptedDomainTable GetAcceptedDomainTable(OrganizationId orgId);
	}
}
