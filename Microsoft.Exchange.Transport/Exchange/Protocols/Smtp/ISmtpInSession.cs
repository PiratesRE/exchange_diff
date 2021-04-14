using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.IsMemberOfProvider;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Internal.MExRuntime;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.SecureMail;
using Microsoft.Exchange.Security;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Security.Cryptography.X509Certificates;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.Transport.Logging;
using Microsoft.Exchange.Transport.MessageThrottling;
using Microsoft.Exchange.Transport.ShadowRedundancy;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal interface ISmtpInSession : ISmtpSession
	{
		X509Certificate2 AdvertisedTlsCertificate { get; }

		INetworkConnection NetworkConnection { get; }

		long ConnectionId { get; }

		IPAddress ProxiedClientAddress { get; }

		string ProxyHopHelloDomain { get; }

		IPAddress ProxyHopAddress { get; }

		InboundClientProxyStates InboundClientProxyState { get; set; }

		X509Certificate2 InternalTransportCertificate { get; }

		bool IsAnonymousClientProxiedSession { get; }

		bool StartClientProxySession { get; set; }

		string ProxyUserName { get; set; }

		SecureString ProxyPassword { get; set; }

		bool ClientProxyFailedDueToIncompatibleBackend { get; set; }

		Permission Permissions { get; }

		TransportMiniRecipient AuthUserRecipient { get; set; }

		AgentLatencyTracker AgentLatencyTracker { get; }

		SmtpSession SessionSource { get; }

		TransportMailItemWrapper TransportMailItemWrapper { get; set; }

		bool SendAsRequiredADLookup { get; set; }

		bool SeenHelo { get; set; }

		bool SeenEhlo { get; set; }

		bool SeenRcpt2 { get; set; }

		string HelloSmtpDomain { get; set; }

		bool IsBdatOngoing { get; }

		bool IsXexch50Received { get; }

		bool IsTls { get; }

		MultilevelAuthMechanism AuthMethod { get; set; }

		AuthenticationSource AuthenticationSourceForAgents { get; }

		TransportMailItem TransportMailItem { get; }

		ISmtpAgentSession AgentSession { get; }

		IIsMemberOfResolver<RoutingAddress> IsMemberOfResolver { get; }

		ISmtpInServer SmtpInServer { get; }

		IPEndPoint ClientEndPoint { get; }

		bool ShutdownConnectionCalled { get; }

		IProtocolLogSession LogSession { get; }

		ulong SessionId { get; }

		ClientData ClientIPData { get; }

		IPEndPoint RemoteEndPoint { get; }

		IPEndPoint LocalEndPoint { get; }

		IEhloOptions AdvertisedEhloOptions { get; }

		string SenderShadowContext { get; set; }

		bool IsShadowedBySender { get; }

		string PeerSessionPrimaryServer { get; set; }

		bool IsPeerShadowSession { get; }

		bool ShouldProxyClientSession { get; }

		SmtpReceiveCapabilities Capabilities { get; }

		SmtpReceiveCapabilities? TlsDomainCapabilities { get; }

		bool AcceptLongAddresses { get; }

		XProxyToSmtpCommandParser XProxyToParser { get; set; }

		ITransportAppConfig TransportAppConfig { get; }

		InboundRecipientCorrelator RecipientCorrelator { get; }

		bool DiscardingMessage { get; }

		ChainValidityStatus TlsRemoteCertificateChainValidationStatus { get; }

		X509Certificate2 TlsRemoteCertificate { get; }

		SecureState SecureState { get; }

		Breadcrumbs<SmtpInSessionBreadcrumbs> Breadcrumbs { get; }

		MailCommandMessageContextParameters MailCommandMessageContextInformation { get; set; }

		string RemoteIdentityName { get; set; }

		SecurityIdentifier RemoteIdentity { get; set; }

		WindowsIdentity RemoteWindowsIdentity { get; set; }

		string CurrentMessageTemporaryId { get; }

		ExEventLog EventLogger { get; }

		bool DisableStartTls { get; set; }

		bool ForceRequestClientTlsCertificate { get; set; }

		SmtpProxyPerfCountersWrapper SmtpProxyPerfCounters { get; }

		ChannelBindingToken ChannelBindingToken { get; }

		ExtendedProtectionConfig ExtendedProtectionConfig { get; }

		IMessageThrottlingManager MessageThrottlingManager { get; }

		IQueueQuotaComponent QueueQuotaComponent { get; }

		ReceiveConnector Connector { get; }

		SmtpReceiveConnectorStub ConnectorStub { get; }

		string AdvertisedDomain { get; }

		Permission SessionPermissions { get; set; }

		int LogonFailures { get; set; }

		int MaxLogonFailures { get; }

		bool TarpitRset { get; set; }

		SmtpInBdatState BdatState { get; set; }

		MimeDocument MimeDocument { get; }

		Stream MessageWriteStream { get; }

		bool StartTlsSupported { get; }

		bool AnonymousTlsSupported { get; }

		ISmtpReceivePerfCounters SmtpReceivePerformanceCounters { get; }

		IInboundProxyDestinationPerfCounters InboundProxyDestinationPerfCounters { get; }

		IInboundProxyDestinationPerfCounters InboundProxyAccountForestPerfCounters { get; }

		InboundExch50 InboundExch50 { get; set; }

		int TooManyRecipientsResponseCount { get; set; }

		byte[] TlsEapKey { get; }

		int TlsCipherKeySize { get; }

		uint XProxyFromSeqNum { get; }

		IShadowRedundancyManager ShadowRedundancyManagerObject { get; }

		IShadowSession ShadowSession { get; set; }

		bool SupportIntegratedAuth { get; }

		Permission ProxiedClientPermissions { get; }

		IMExSession MexSession { get; set; }

		IAuthzAuthorization AuthzAuthorization { get; }

		ISmtpMessageContextBlob MessageContextBlob { get; }

		bool IsDataRedactionNecessary { get; }

		ITracer Tracer { get; }

		Permission AnonymousPermissions { get; }

		Permission PartnerPermissions { get; }

		bool SmtpUtf8Supported { get; set; }

		DateTime SessionStartTime { get; }

		int NumberOfMessagesReceived { get; }

		string DestinationTrackerLastNextHopFqdn { get; set; }

		string DestinationTrackerLastExoAccountForest { get; set; }

		void DropBreadcrumb(SmtpInSessionBreadcrumbs breadcrumb);

		void LogInformation(ProtocolLoggingLevel loggingLevel, string information, byte[] data);

		void Start();

		void SetupSessionToProxyTarget(SmtpSendConnectorConfig outboundProxyConnector, IEnumerable<INextHopServer> outboundProxyDestinationsParam, TlsSendConfiguration outboundProxyTlsSendConfigurationParam, RiskLevel outboundProxyRiskLevelParam, int outboundProxyOutboundIPPoolParam, string outboundProxyNextHopDomainParam, string outboundProxySessionIdParam);

		void SetupExpectedBlobs(MailCommandMessageContextParameters messageContextParameters);

		void ResetExpectedBlobs();

		bool ShouldRejectMailItem(RoutingAddress fromAddress, bool checkRecipientCount, out SmtpResponse failureSmtpResponse);

		SmtpResponse TrackAndEnqueueMailItem();

		void TrackAndEnqueuePeerShadowMailItem();

		void ReleaseMailItem();

		void UpdateSessionWithProxyInformation(IPAddress clientIp, int clientPort, string clientHelloDomain, bool isAuthenticatedProxy, SecurityIdentifier securityId, string clientIdentityName, WindowsIdentity identity, TransportMiniRecipient recipient, int? capabilitiesInt);

		void UpdateSessionWithProxyFromInformation(IPAddress clientIp, int clientPort, string clientHelloDomain, uint xProxyFromSequenceNum, uint? permissionsInt, AuthenticationSource? authSource);

		void Shutdown();

		void ShutdownConnection();

		void Disconnect(DisconnectReason disconnectReasonParam);

		void HandleBlindProxySetupFailure(SmtpResponse response, bool clientProxy);

		void ResetSessionAuthentication();

		void HandleBlindProxySetupSuccess(SmtpResponse successfulResponse, NetworkConnection networkConnection, ulong sendSessionId, IProtocolLogSession sendLogSession, bool isClientProxy);

		byte[] GetCertificatePublicKey();

		bool IsTrustedIP(IPAddress address);

		bool DetermineTlsDomainCapabilities();

		Permission MailItemPermissionsGranted { get; }

		Permission MailItemPermissionsDenied { get; }

		void GrantMailItemPermissions(Permission permissions);

		void DenyMailItemPermissions(Permission permissions);

		void ResetMailItemPermissions();

		void RemoveClientIpConnection();

		void UpdateSmtpAvailabilityPerfCounter(LegitimateSmtpAvailabilityCategory category);

		void IncrementSmtpAvailabilityPerfCounterForMessageLoopsInLastHour(long incrementValue);

		bool CreateTransportMailItem(OrganizationId mailCommandInternalOrganizationId, Guid mailCommandExternalOrganizationId, MailDirectionality mailCommandDirectionality, string exoAccountForest, string exoTenantContainer, out SmtpResponse smtpResponse);

		void DeleteTransportMailItem();

		void AbortMailTransaction();

		void UpdateSmtpReceivePerfCountersForMessageReceived(int recipients, long messageBytes);

		void UpdateInboundProxyDestinationPerfCountersForMessageReceived(int recipients, long messageBytes);

		Stream OpenMessageWriteStream(bool expectBinaryContent);

		void CloseMessageWriteStream();

		void PutBackReceivedBytes(int bytesUnconsumed);

		void RawDataReceivedCompleted();

		void SetRawModeAfterCommandCompleted(RawDataHandler rawDataHandler);

		void StartTls(SecureState secureState);

		IAsyncResult RaiseOnRejectEvent(byte[] command, EventArgs originalEventArgs, SmtpResponse smtpResponse, AsyncCallback callback);

		byte[] GetTlsEapKey();

		void SetSessionPermissions(IntPtr userToken);

		void AddSessionPermissions(SmtpReceiveCapabilities capabilities);

		void SetupPoisonContext();
	}
}
