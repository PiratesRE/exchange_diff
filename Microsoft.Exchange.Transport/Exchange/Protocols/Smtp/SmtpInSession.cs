using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.IsMemberOfProvider;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Internal;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Internal.MExRuntime;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.MessageSecurity;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.SecureMail;
using Microsoft.Exchange.Security;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Security.Cryptography.X509Certificates;
using Microsoft.Exchange.Threading;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.Transport.Categorizer;
using Microsoft.Exchange.Transport.Logging;
using Microsoft.Exchange.Transport.Logging.ConnectionLog;
using Microsoft.Exchange.Transport.Logging.MessageTracking;
using Microsoft.Exchange.Transport.MessageThrottling;
using Microsoft.Exchange.Transport.ShadowRedundancy;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class SmtpInSession : ISmtpInSession, ISmtpSession
	{
		public SmtpInSession(INetworkConnection connection, ISmtpInServer server, SmtpReceiveConnectorStub connectorStub, IProtocolLog protocolLog, ExEventLog eventLogger, IAgentRuntime agentRuntime, IMailRouter mailRouter, IEnhancedDns enhancedDns, IIsMemberOfResolver<RoutingAddress> memberOfResolver, IMessageThrottlingManager messageThrottlingManager, IShadowRedundancyManager shadowRedundancyManager, ITransportAppConfig transportAppConfig, ITransportConfiguration transportConfiguration, IQueueQuotaComponent queueQuotaComponent, IAuthzAuthorization authzAuthorization, ISmtpMessageContextBlob smtpMessageContextBlob)
		{
			ArgumentValidator.ThrowIfNull("connection", connection);
			ArgumentValidator.ThrowIfNull("server", server);
			ArgumentValidator.ThrowIfNull("protocolLog", protocolLog);
			ArgumentValidator.ThrowIfNull("eventLogger", eventLogger);
			ArgumentValidator.ThrowIfNull("agentRuntime", agentRuntime);
			ArgumentValidator.ThrowIfNull("mailRouter", mailRouter);
			ArgumentValidator.ThrowIfNull("enhancedDns", enhancedDns);
			ArgumentValidator.ThrowIfNull("memberOfResolver", memberOfResolver);
			ArgumentValidator.ThrowIfNull("messageThrottlingManager", messageThrottlingManager);
			ArgumentValidator.ThrowIfNull("transportAppConfig", transportAppConfig);
			ArgumentValidator.ThrowIfNull("transportConfiguration", transportConfiguration);
			ArgumentValidator.ThrowIfNull("authzAuthorization", authzAuthorization);
			ArgumentValidator.ThrowIfNull("smtpMessageContextBlob", smtpMessageContextBlob);
			this.eventLogger = eventLogger;
			this.mailRouter = mailRouter;
			this.enhancedDns = enhancedDns;
			this.memberOfResolver = memberOfResolver;
			this.messageThrottlingManager = messageThrottlingManager;
			this.shadowRedundancyManager = shadowRedundancyManager;
			this.transportAppConfig = transportAppConfig;
			this.transportConfiguration = transportConfiguration;
			this.queueQuotaComponent = queueQuotaComponent;
			this.authzAuthorization = authzAuthorization;
			this.smtpMessageContextBlob = smtpMessageContextBlob;
			this.DropBreadcrumb(SmtpInSessionBreadcrumbs.Init);
			this.sessionStartTime = DateTime.UtcNow;
			this.connection = connection;
			this.connectionId = connection.ConnectionId;
			this.server = server;
			this.relatedMessageInfo = string.Empty;
			this.agentSession = agentRuntime.NewSmtpAgentSession(this, connection, !this.IsTrustedIP(connection.RemoteEndPoint.Address));
			if (connectorStub != null)
			{
				this.connectorStub = connectorStub;
				this.connector = connectorStub.Connector;
				this.smtpReceivePerfCountersInstance = connectorStub.SmtpReceivePerfCounterInstance;
				this.smtpAvailabilityPerfCounters = connectorStub.SmtpAvailabilityPerfCounters;
			}
			string connectorId = string.Empty;
			ProtocolLoggingLevel loggingLevel;
			if (this.connector != null)
			{
				loggingLevel = this.connector.ProtocolLoggingLevel;
				connectorId = this.connector.Id.ToString();
				if (this.connector.ExtendedProtectionPolicy != Microsoft.Exchange.Data.Directory.SystemConfiguration.ExtendedProtectionPolicySetting.None)
				{
					this.extendedProtectionConfig = new ExtendedProtectionConfig((int)this.connector.ExtendedProtectionPolicy, null, false);
				}
				else
				{
					this.extendedProtectionConfig = ExtendedProtectionConfig.NoExtendedProtection;
				}
			}
			else
			{
				loggingLevel = ProtocolLoggingLevel.Verbose;
			}
			this.logSession = protocolLog.OpenSession(connectorId, this.SessionId, this.connection.RemoteEndPoint, this.connection.LocalEndPoint, loggingLevel);
			this.logSession.LogConnect();
			if (this.connector == null)
			{
				return;
			}
			this.clientIpAddress = connection.RemoteEndPoint.Address;
			this.significantAddressBytes = SmtpInSessionUtils.GetSignificantBytesOfIPAddress(this.clientIpAddress);
			if (this.SmtpInServer.Ipv6ReceiveConnectionThrottlingEnabled)
			{
				this.clientIpData = this.connectorStub.AddConnection(this.clientIpAddress, this.significantAddressBytes, out this.maxConnectionsExceeded, out this.maxConnectionsPerSourceExceeded);
			}
			else
			{
				this.clientIpData = this.connectorStub.AddConnection(this.clientIpAddress, out this.maxConnectionsExceeded, out this.maxConnectionsPerSourceExceeded);
			}
			this.IncrementConnectionLevelPerfCounters();
			this.connection.Timeout = (int)this.Connector.ConnectionInactivityTimeout.TotalSeconds;
			this.sessionExpireTime = this.sessionStartTime.Add(this.Connector.ConnectionTimeout);
			this.ehloOptions = new EhloOptions();
			this.ehloOptions.AdvertisedIPAddress = this.RemoteEndPoint.Address;
			this.ehloOptions.AdvertisedFQDN = this.AdvertisedDomain;
			this.ehloOptions.Size = this.Connector.SizeEnabled;
			if (this.Connector.MaxMessageSize.ToBytes() > 9223372036854775807UL)
			{
				this.ehloOptions.MaxSize = long.MaxValue;
			}
			else
			{
				this.ehloOptions.MaxSize = (long)this.Connector.MaxMessageSize.ToBytes();
			}
			this.ehloOptions.Pipelining = this.Connector.PipeliningEnabled;
			this.ehloOptions.Dsn = this.Connector.DeliveryStatusNotificationEnabled;
			this.ehloOptions.EnhancedStatusCodes = this.Connector.EnhancedStatusCodesEnabled;
			this.ehloOptions.EightBitMime = this.Connector.EightBitMimeEnabled;
			this.ehloOptions.BinaryMime = this.Connector.BinaryMimeEnabled;
			this.ehloOptions.Chunking = this.Connector.ChunkingEnabled;
			this.ehloOptions.Xexch50 = (this.SmtpInServer.TransportSettings.Xexch50Enabled && (this.Connector.AuthMechanism & (AuthMechanisms.ExchangeServer | AuthMechanisms.ExternalAuthoritative)) != AuthMechanisms.None);
			this.ehloOptions.XLongAddr = this.Connector.LongAddressesEnabled;
			this.ehloOptions.XOrar = this.Connector.OrarEnabled;
			this.ehloOptions.SmtpUtf8 = (this.Connector.SmtpUtf8Enabled && (this.Connector.EightBitMimeEnabled || this.Connector.BinaryMimeEnabled));
			this.ehloOptions.XRDst = (this.server.IsBridgehead && (this.Connector.AuthMechanism & AuthMechanisms.ExchangeServer) != AuthMechanisms.None);
			if (this.shadowRedundancyManager != null)
			{
				this.shadowRedundancyManager.SetSmtpInEhloOptions(this.ehloOptions, this.Connector);
			}
			bool isFrontEndTransportProcess = ConfigurationComponent.IsFrontEndTransportProcess(Components.Configuration);
			SmtpInSessionUtils.ApplyRoleBasedEhloOptionsOverrides(this.ehloOptions, isFrontEndTransportProcess);
			this.ApplySupportIntegratedAuthOverride(isFrontEndTransportProcess);
			this.certificatesLoadedSuccessfully = this.LoadCertificates();
			this.sessionPermissions = this.AnonymousPermissions;
			this.LogInformation(ProtocolLoggingLevel.Verbose, "Set Session Permissions", Util.AsciiStringToBytes(Util.GetPermissionString(this.sessionPermissions)));
			if ((this.Connector.AuthMechanism & AuthMechanisms.ExternalAuthoritative) != AuthMechanisms.None)
			{
				this.RemoteIdentity = WellKnownSids.ExternallySecuredServers;
				this.RemoteIdentityName = "accepted_domain";
				this.SetSessionPermissions(this.RemoteIdentity);
			}
		}

		protected SmtpInSession()
		{
		}

		public X509Certificate2 AdvertisedTlsCertificate { get; private set; }

		public X509Certificate2 InternalTransportCertificate { get; private set; }

		public X509Certificate2 TlsRemoteCertificate { get; private set; }

		public SmtpProxyPerfCountersWrapper SmtpProxyPerfCounters
		{
			get
			{
				return this.smtpProxyPerfCounters;
			}
		}

		public INetworkConnection NetworkConnection
		{
			get
			{
				return this.connection;
			}
		}

		public long ConnectionId
		{
			get
			{
				return this.connectionId;
			}
		}

		public IPAddress ProxiedClientAddress
		{
			get
			{
				return this.proxiedClientAddress;
			}
		}

		public string ProxyHopHelloDomain
		{
			get
			{
				return this.proxyHopHelloDomain;
			}
		}

		public IPAddress ProxyHopAddress
		{
			get
			{
				return this.clientIpAddress;
			}
		}

		public InboundClientProxyStates InboundClientProxyState
		{
			get
			{
				return this.inboundClientProxyState;
			}
			set
			{
				this.inboundClientProxyState = value;
			}
		}

		public bool IsAnonymousClientProxiedSession
		{
			get
			{
				return this.proxiedClientAddress != null && this.inboundClientProxyState == InboundClientProxyStates.None;
			}
		}

		public bool StartClientProxySession
		{
			get
			{
				return this.startClientProxySession;
			}
			set
			{
				if (this.startOutboundProxySession && value)
				{
					throw new InvalidOperationException("StartOutboundProxySession and StartClientProxySession cannot both be true");
				}
				this.startClientProxySession = value;
			}
		}

		public string ProxyUserName
		{
			get
			{
				return this.proxyUserName;
			}
			set
			{
				this.proxyUserName = value;
			}
		}

		public SecureString ProxyPassword
		{
			get
			{
				return this.proxyPassword;
			}
			set
			{
				this.proxyPassword = value;
			}
		}

		public bool ClientProxyFailedDueToIncompatibleBackend
		{
			get
			{
				return this.clientProxyFailedDueToIncompatibleBackend;
			}
			set
			{
				this.clientProxyFailedDueToIncompatibleBackend = value;
			}
		}

		public IPEndPoint ClientEndPoint
		{
			get
			{
				return this.connection.RemoteEndPoint;
			}
		}

		public bool ShutdownConnectionCalled
		{
			get
			{
				return this.shutdownConnectionCalled;
			}
		}

		public bool IsTls
		{
			get
			{
				return this.connection.IsTls;
			}
		}

		public IProtocolLogSession LogSession
		{
			get
			{
				return this.logSession;
			}
		}

		public ulong SessionId
		{
			get
			{
				return (ulong)this.SessionSource.SessionId;
			}
		}

		public ClientData ClientIPData
		{
			get
			{
				return this.clientIpData;
			}
		}

		public IPEndPoint RemoteEndPoint
		{
			get
			{
				return this.SessionSource.RemoteEndPoint;
			}
		}

		public IPEndPoint LocalEndPoint
		{
			get
			{
				return this.SessionSource.LocalEndPoint;
			}
		}

		public string HelloDomain
		{
			get
			{
				return this.SessionSource.HelloDomain;
			}
		}

		public SmtpResponse Banner
		{
			get
			{
				return this.SessionSource.Banner;
			}
			set
			{
				this.SessionSource.Banner = value;
			}
		}

		public IEhloOptions AdvertisedEhloOptions
		{
			get
			{
				return this.ehloOptions;
			}
		}

		public string SenderShadowContext
		{
			get
			{
				return this.senderShadowContext;
			}
			set
			{
				this.senderShadowContext = value;
			}
		}

		public bool IsShadowedBySender
		{
			get
			{
				return !string.IsNullOrEmpty(this.senderShadowContext);
			}
		}

		public string PeerSessionPrimaryServer
		{
			get
			{
				return this.peerSessionPrimaryServer;
			}
			set
			{
				this.peerSessionPrimaryServer = value;
				this.SessionSource.Properties["Microsoft.Exchange.IsShadow"] = true;
			}
		}

		public bool IsPeerShadowSession
		{
			get
			{
				return !string.IsNullOrEmpty(this.PeerSessionPrimaryServer);
			}
		}

		public bool ShouldProxyClientSession
		{
			get
			{
				return ConfigurationComponent.IsFrontEndTransportProcess(this.transportConfiguration);
			}
		}

		public bool AcceptLongAddresses
		{
			get
			{
				return this.ehloOptions.XLongAddr;
			}
		}

		public SmtpReceiveCapabilities Capabilities
		{
			get
			{
				return Util.SessionCapabilitiesFromTlsAndNonTlsCapabilities(this.SecureState, this.connectorStub.NoTlsCapabilities, this.tlsDomainCapabilities);
			}
		}

		public SmtpReceiveCapabilities? TlsDomainCapabilities
		{
			get
			{
				return this.tlsDomainCapabilities;
			}
		}

		public XProxyToSmtpCommandParser XProxyToParser
		{
			get
			{
				return this.xProxyToParser;
			}
			set
			{
				this.xProxyToParser = value;
			}
		}

		public ITransportAppConfig TransportAppConfig
		{
			get
			{
				return this.transportAppConfig;
			}
		}

		public InboundRecipientCorrelator RecipientCorrelator
		{
			get
			{
				return this.recipientCorrelator;
			}
		}

		public ChannelBindingToken ChannelBindingToken
		{
			get
			{
				return this.connection.ChannelBindingToken;
			}
		}

		public ExtendedProtectionConfig ExtendedProtectionConfig
		{
			get
			{
				return this.extendedProtectionConfig;
			}
		}

		public bool DiscardingMessage
		{
			get
			{
				BaseDataSmtpCommand baseDataSmtpCommand = this.commandHandler as BaseDataSmtpCommand;
				return baseDataSmtpCommand != null && baseDataSmtpCommand.DiscardingMessage;
			}
		}

		public ChainValidityStatus TlsRemoteCertificateChainValidationStatus
		{
			get
			{
				if (this.secureState == SecureState.None)
				{
					return ChainValidityStatus.Valid;
				}
				if (this.tlsRemoteCertificateChainValidationStatus == null)
				{
					this.tlsRemoteCertificateChainValidationStatus = new ChainValidityStatus?(Util.CalculateTlsRemoteCertificateChainValidationStatus(this.SmtpInServer.Configuration.AppConfig.SecureMail.ClientCertificateChainValidationEnabled, this.server.CertificateValidator, this.TlsRemoteCertificate, this.logSession, this.eventLogger));
				}
				return this.tlsRemoteCertificateChainValidationStatus.Value;
			}
		}

		public AgentLatencyTracker AgentLatencyTracker
		{
			get
			{
				return this.AgentSession.LatencyTracker;
			}
		}

		public SmtpSession SessionSource
		{
			get
			{
				return this.AgentSession.SessionSource;
			}
		}

		public TransportMailItemWrapper TransportMailItemWrapper
		{
			get
			{
				if (this.transportMailItem != null)
				{
					return this.mailItemWrapper;
				}
				return null;
			}
			set
			{
				this.mailItemWrapper = value;
			}
		}

		public bool SendAsRequiredADLookup
		{
			get
			{
				return this.sendAsRequiredADLookup;
			}
			set
			{
				this.sendAsRequiredADLookup = value;
			}
		}

		public ISmtpInServer SmtpInServer
		{
			get
			{
				return this.server;
			}
		}

		public IMessageThrottlingManager MessageThrottlingManager
		{
			get
			{
				return this.messageThrottlingManager;
			}
		}

		public IQueueQuotaComponent QueueQuotaComponent
		{
			get
			{
				return this.queueQuotaComponent;
			}
		}

		public IIsMemberOfResolver<RoutingAddress> IsMemberOfResolver
		{
			get
			{
				return this.memberOfResolver;
			}
		}

		public ISmtpAgentSession AgentSession
		{
			get
			{
				return this.agentSession;
			}
		}

		public ReceiveConnector Connector
		{
			get
			{
				return this.connector;
			}
		}

		public SmtpReceiveConnectorStub ConnectorStub
		{
			get
			{
				return this.connectorStub;
			}
		}

		public string AdvertisedDomain
		{
			get
			{
				return Util.AdvertisedDomainFromReceiveConnector(this.Connector, () => ComputerInformation.DnsPhysicalFullyQualifiedDomainName);
			}
		}

		public bool IsExternalAuthoritative
		{
			get
			{
				return this.RemoteIdentity == WellKnownSids.ExternallySecuredServers;
			}
		}

		public Permission SessionPermissions
		{
			get
			{
				return this.sessionPermissions;
			}
			set
			{
				this.sessionPermissions = value;
				this.LogInformation(ProtocolLoggingLevel.Verbose, "Set Session Permissions", Util.AsciiStringToBytes(Util.GetPermissionString(this.sessionPermissions)));
			}
		}

		public Permission Permissions
		{
			get
			{
				Permission permission = this.sessionPermissions;
				if (this.TransportAppConfig != null && this.TransportAppConfig.SmtpReceiveConfiguration.GrantExchangeServerPermissions)
				{
					permission |= (Permission.SMTPSubmit | Permission.SMTPAcceptAnyRecipient | Permission.SMTPAcceptAuthenticationFlag | Permission.SMTPAcceptAnySender | Permission.SMTPAcceptAuthoritativeDomainSender | Permission.BypassAntiSpam | Permission.BypassMessageSizeLimit | Permission.SMTPAcceptEXCH50 | Permission.AcceptRoutingHeaders | Permission.AcceptForestHeaders | Permission.AcceptOrganizationHeaders | Permission.SMTPAcceptXShadow | Permission.SMTPAcceptXProxyFrom | Permission.SMTPAcceptXSessionParams | Permission.SMTPAcceptXMessageContextADRecipientCache | Permission.SMTPAcceptXMessageContextExtendedProperties | Permission.SMTPAcceptXMessageContextFastIndex | Permission.SMTPAcceptXAttr | Permission.SMTPAcceptXSysProbe);
				}
				return (permission | this.MailItemPermissionsGranted) & ~this.MailItemPermissionsDenied;
			}
		}

		public TransportMiniRecipient AuthUserRecipient
		{
			get
			{
				return this.authUserRecipient;
			}
			set
			{
				this.authUserRecipient = value;
			}
		}

		public SecurityIdentifier RemoteIdentity
		{
			get
			{
				return this.sessionRemoteIdentity;
			}
			set
			{
				this.sessionRemoteIdentity = value;
			}
		}

		public WindowsIdentity RemoteWindowsIdentity
		{
			get
			{
				return this.remoteWindowsIdentity;
			}
			set
			{
				this.remoteWindowsIdentity = value;
			}
		}

		public string RemoteIdentityName
		{
			get
			{
				return this.sessionRemoteIdentityName;
			}
			set
			{
				this.sessionRemoteIdentityName = value;
			}
		}

		public MultilevelAuthMechanism AuthMethod
		{
			get
			{
				return this.sessionAuthMethod;
			}
			set
			{
				this.sessionAuthMethod = value;
			}
		}

		public bool SeenHelo
		{
			get
			{
				return this.seenHelo;
			}
			set
			{
				this.seenHelo = value;
			}
		}

		public bool SeenEhlo
		{
			get
			{
				return this.seenEhlo;
			}
			set
			{
				this.seenEhlo = value;
				if (value)
				{
					this.isLastCommandEhloBeforeQuit = true;
				}
			}
		}

		public bool SeenRcpt2 { get; set; }

		public string HelloSmtpDomain
		{
			get
			{
				return this.SessionSource.HelloDomain;
			}
			set
			{
				this.SessionSource.HelloDomain = value;
			}
		}

		internal int MessagesSubmitted
		{
			get
			{
				return this.messagesSubmitted;
			}
		}

		public int LogonFailures
		{
			get
			{
				return this.logonFailures;
			}
			set
			{
				this.logonFailures = value;
			}
		}

		public int MaxLogonFailures
		{
			get
			{
				return this.Connector.MaxLogonFailures;
			}
		}

		public bool TarpitRset
		{
			get
			{
				return this.tarpitRset;
			}
			set
			{
				this.tarpitRset = value;
			}
		}

		public SecureState SecureState
		{
			get
			{
				return this.secureState;
			}
		}

		public SmtpInBdatState BdatState
		{
			get
			{
				return this.bdatState;
			}
			set
			{
				this.bdatState = value;
			}
		}

		public bool IsBdatOngoing
		{
			get
			{
				return this.bdatState != null;
			}
		}

		public TransportMailItem TransportMailItem
		{
			get
			{
				return this.transportMailItem;
			}
		}

		public MimeDocument MimeDocument
		{
			get
			{
				if (this.transportMailItem == null)
				{
					return null;
				}
				return this.transportMailItem.MimeDocument;
			}
		}

		public Stream MessageWriteStream
		{
			get
			{
				return this.messageWriteStream;
			}
		}

		public bool IsXexch50Received
		{
			get
			{
				return this.inboundExch50 != null;
			}
		}

		public bool StartTlsSupported
		{
			get
			{
				return this.AdvertisedTlsCertificate != null && !this.DisableStartTls;
			}
		}

		public bool AnonymousTlsSupported
		{
			get
			{
				return this.InternalTransportCertificate != null && !this.Connector.SuppressXAnonymousTls;
			}
		}

		public ISmtpReceivePerfCounters SmtpReceivePerformanceCounters
		{
			get
			{
				return this.smtpReceivePerfCountersInstance;
			}
		}

		public IInboundProxyDestinationPerfCounters InboundProxyDestinationPerfCounters
		{
			get
			{
				string instanceName;
				if (this.transportAppConfig.SmtpInboundProxyConfiguration.InboundProxyDestinationTrackingEnabled && Util.TryGetNextHopFqdnProperty(this.TransportMailItem.ExtendedPropertyDictionary, out instanceName))
				{
					return new InboundProxyDestinationPerfCountersWrapper(instanceName);
				}
				return new NullInboundProxyDestinationPerfCounters();
			}
		}

		public IInboundProxyDestinationPerfCounters InboundProxyAccountForestPerfCounters
		{
			get
			{
				if (this.transportAppConfig.SmtpInboundProxyConfiguration.InboundProxyAccountForestTrackingEnabled && this.TransportMailItem.ExoAccountForest != null)
				{
					return new InboundProxyAccountForestPerfCountersWrapper(this.TransportMailItem.ExoAccountForest);
				}
				return new NullInboundProxyDestinationPerfCounters();
			}
		}

		internal SmtpProxyPerfCountersWrapper SmtpProxyPerformanceCounters
		{
			get
			{
				return this.smtpProxyPerfCounters;
			}
		}

		public InboundExch50 InboundExch50
		{
			get
			{
				return this.inboundExch50;
			}
			set
			{
				this.inboundExch50 = value;
			}
		}

		public int TooManyRecipientsResponseCount
		{
			get
			{
				return this.tooManyRecipientsResponseCount;
			}
			set
			{
				this.tooManyRecipientsResponseCount = value;
			}
		}

		public byte[] TlsEapKey
		{
			get
			{
				return this.connection.TlsEapKey;
			}
		}

		public int TlsCipherKeySize
		{
			get
			{
				return this.connection.TlsCipherKeySize;
			}
		}

		public IMExSession MexSession
		{
			get
			{
				return this.mexSession;
			}
			set
			{
				this.mexSession = value;
			}
		}

		public string CurrentMessageTemporaryId
		{
			get
			{
				return string.Format(CultureInfo.InvariantCulture, "{0:X16};{1:yyyy-MM-ddTHH\\:mm\\:ss.fffZ};{2}", new object[]
				{
					this.SessionId,
					this.sessionStartTime,
					this.numberOfMessagesReceived
				});
			}
		}

		public DateTime SessionStartTime
		{
			get
			{
				return this.sessionStartTime;
			}
		}

		public int NumberOfMessagesReceived
		{
			get
			{
				return this.numberOfMessagesReceived;
			}
		}

		public string DestinationTrackerLastNextHopFqdn { get; set; }

		public string DestinationTrackerLastExoAccountForest { get; set; }

		internal MailDirectionality Directionality
		{
			get
			{
				return this.TransportMailItem.Directionality;
			}
		}

		public ExEventLog EventLogger
		{
			get
			{
				return this.eventLogger;
			}
		}

		public bool DisableStartTls
		{
			get
			{
				return this.startTlsDisabled;
			}
			set
			{
				if (value && this.SecureState == SecureState.StartTls)
				{
					throw new InvalidOperationException("Cannnot disable STARTTLS since session is already secure");
				}
				this.startTlsDisabled = value;
				if (this.startTlsDisabled && this.transportAppConfig.SmtpReceiveConfiguration.BlockedSessionLoggingEnabled)
				{
					this.LogSession.ProtocolLoggingLevel = ProtocolLoggingLevel.None;
					this.logSession.LogDisconnect(DisconnectReason.SuppressLogging);
				}
			}
		}

		public bool ForceRequestClientTlsCertificate
		{
			get
			{
				return this.forceRequestClientTlsCertificate;
			}
			set
			{
				this.forceRequestClientTlsCertificate = value;
			}
		}

		public uint XProxyFromSeqNum
		{
			get
			{
				return this.xProxyFromSeqNum;
			}
		}

		public IShadowRedundancyManager ShadowRedundancyManagerObject
		{
			get
			{
				return this.shadowRedundancyManager;
			}
		}

		public IShadowSession ShadowSession
		{
			get
			{
				return this.shadowSession;
			}
			set
			{
				this.shadowSession = value;
			}
		}

		public bool SupportIntegratedAuth
		{
			get
			{
				return this.supportIntegratedAuth && (this.Connector.AuthMechanism & AuthMechanisms.Integrated) != AuthMechanisms.None;
			}
		}

		public AuthenticationSource AuthenticationSourceForAgents
		{
			get
			{
				if (this.proxiedClientAuthSource != null)
				{
					return this.proxiedClientAuthSource.Value;
				}
				if (SmtpInSessionUtils.IsAnonymous(this.RemoteIdentity))
				{
					return AuthenticationSource.Anonymous;
				}
				if (!SmtpInSessionUtils.IsPartner(this.RemoteIdentity))
				{
					return AuthenticationSource.Organization;
				}
				return AuthenticationSource.Partner;
			}
		}

		public Permission ProxiedClientPermissions
		{
			get
			{
				if (this.proxiedClientPermissions == null)
				{
					return this.Permissions;
				}
				return (Permission)this.proxiedClientPermissions.Value;
			}
		}

		private TimeSpan MaxAcknowledgementDelay
		{
			get
			{
				return this.Connector.MaxAcknowledgementDelay;
			}
		}

		public Breadcrumbs<SmtpInSessionBreadcrumbs> Breadcrumbs
		{
			get
			{
				return this.breadcrumbs;
			}
		}

		public MailCommandMessageContextParameters MailCommandMessageContextInformation
		{
			get
			{
				return this.mailCommandMessageContextInformation;
			}
			set
			{
				this.mailCommandMessageContextInformation = value;
			}
		}

		public IAuthzAuthorization AuthzAuthorization
		{
			get
			{
				return this.authzAuthorization;
			}
		}

		public ISmtpMessageContextBlob MessageContextBlob
		{
			get
			{
				return this.smtpMessageContextBlob;
			}
		}

		public bool IsDataRedactionNecessary
		{
			get
			{
				return Util.IsDataRedactionNecessary();
			}
		}

		public Permission AnonymousPermissions
		{
			get
			{
				return this.DetermineAnonymousPermissions();
			}
		}

		public Permission PartnerPermissions
		{
			get
			{
				return this.DeterminePartnerPermissions();
			}
		}

		public ITracer Tracer
		{
			get
			{
				return ExTraceGlobals.SmtpReceiveTracer;
			}
		}

		public bool SmtpUtf8Supported
		{
			get
			{
				return this.smtpUtf8Supported;
			}
			set
			{
				this.smtpUtf8Supported = value;
			}
		}

		private Guid AuthUserMailboxGuid
		{
			get
			{
				if (this.authUserRecipient != null)
				{
					return this.authUserRecipient.ExchangeGuid;
				}
				return Guid.Empty;
			}
		}

		public static bool DelayedAckCompletedCallback(object state, DelayedAckCompletionStatus status, TimeSpan delay, string context)
		{
			if (state == null)
			{
				throw new ArgumentNullException("state");
			}
			SmtpInSession smtpInSession = (SmtpInSession)state;
			bool result;
			if (smtpInSession.delayedAckStatus == SmtpInSession.DelayedAckStatus.WaitingForShadowRedundancyManager)
			{
				string text;
				switch (status)
				{
				case DelayedAckCompletionStatus.Delivered:
					smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.DelayedAckCompletedByDelivery);
					ExTraceGlobals.SmtpReceiveTracer.TraceDebug<long>((long)smtpInSession.GetHashCode(), "SmtpInSession(id={0}).DelayedAckCompletedCallback: SRM relay notification received.", smtpInSession.connectionId);
					text = "Delivered";
					break;
				case DelayedAckCompletionStatus.Expired:
					smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.DelayedAckCompletedByExpiry);
					ExTraceGlobals.SmtpReceiveTracer.TraceDebug<long>((long)smtpInSession.GetHashCode(), "SmtpInSession(id={0}).DelayedAckCompletedCallback: SRM expiry notification received.", smtpInSession.connectionId);
					text = "Expired";
					break;
				case DelayedAckCompletionStatus.Skipped:
					smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.DelayedAckCompletedBySkipping);
					ExTraceGlobals.SmtpReceiveTracer.TraceDebug<long>((long)smtpInSession.GetHashCode(), "SmtpInSession(id={0}).DelayedAckCompletedCallback: SRM skipping notification received.", smtpInSession.connectionId);
					text = "Skipped";
					break;
				default:
					throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Unsupported completion status '{0}'.", new object[]
					{
						status
					}));
				}
				if (!string.IsNullOrEmpty(context))
				{
					text = text + ";" + context;
				}
				smtpInSession.LogTarpitEvent(delay, "DelayedAck", text);
				smtpInSession.DelayResponseCompleted(null);
				result = true;
			}
			else
			{
				if (smtpInSession.delayedAckStatus != SmtpInSession.DelayedAckStatus.ShadowRedundancyManagerNotified)
				{
					throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Session '{0}': Got a DelayedAckCompletedCallback() {1} notification for a session with status '{2}'.", new object[]
					{
						smtpInSession.SessionId,
						status,
						smtpInSession.delayedAckStatus
					}));
				}
				ExTraceGlobals.SmtpReceiveTracer.TraceDebug<long, DelayedAckCompletionStatus>((long)smtpInSession.GetHashCode(), "SmtpInSession(id={0}).DelayedAckCompletedCallback: SRM {1} notification received too early, retry later.", smtpInSession.connectionId, status);
				smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.DelayedAckCompletedTooEarly);
				result = false;
			}
			return result;
		}

		public void Start()
		{
			this.DropBreadcrumb(SmtpInSessionBreadcrumbs.Start);
			if (this.server.RejectCommands)
			{
				ExTraceGlobals.SmtpReceiveTracer.TraceDebug<long>((long)this.GetHashCode(), "SmtpInSession(id={0}) disconnected: reject commands", this.connectionId);
				if (this.server.RejectionSmtpResponse.Equals(SmtpResponse.InsufficientResource))
				{
					this.UpdateSmtpAvailabilityPerfCounter(LegitimateSmtpAvailabilityCategory.RejectDueToBackPressure);
				}
				this.WriteLineWithLogThenShutdown(this.server.RejectionSmtpResponse);
				return;
			}
			if (this.connector == null)
			{
				ExTraceGlobals.SmtpReceiveTracer.TraceDebug<long>((long)this.GetHashCode(), "SmtpInSession(id={0}) disconnected: no connector mapping found", this.connectionId);
				this.WriteLineWithLogThenShutdown(SmtpResponse.ServiceUnavailable);
				return;
			}
			MultiValuedProperty<IPRange> internalSMTPServers = this.SmtpInServer.TransportSettings.InternalSMTPServers;
			if (this.SmtpInServer.ServerConfiguration.AntispamAgentsEnabled && MultiValuedPropertyBase.IsNullOrEmpty(internalSMTPServers))
			{
				ExTraceGlobals.SmtpReceiveTracer.TraceDebug<long>((long)this.GetHashCode(), "SmtpInSession(id={0}) disconnected: list of internal SMTP servers is empty", this.connectionId);
				this.eventLogger.LogEvent(TransportEventLogConstants.Tuple_InternalSMTPServerListEmpty, this.Connector.Name, new object[]
				{
					this.Connector.Name,
					this.Connector.MaxInboundConnection
				});
			}
			if (this.maxConnectionsExceeded)
			{
				ExTraceGlobals.SmtpReceiveTracer.TraceDebug<long, Unlimited<int>>((long)this.GetHashCode(), "SmtpInSession(id={0}) disconnected, maximum number of connections ({1}) exceeded", this.connectionId, this.Connector.MaxInboundConnection);
				this.eventLogger.LogEvent(TransportEventLogConstants.Tuple_SmtpReceiveMaxConnectionReached, this.Connector.Name, new object[]
				{
					this.Connector.Name,
					this.Connector.MaxInboundConnection
				});
				this.WriteLineWithLogThenShutdown(SmtpResponse.TooManyConnections);
				return;
			}
			if (this.maxConnectionsPerSourceExceeded)
			{
				ExTraceGlobals.SmtpReceiveTracer.TraceDebug<long>((long)this.GetHashCode(), "SmtpInSession(id={0}) disconnected, maximum number of connections per source exceeded ", this.connectionId);
				this.eventLogger.LogEvent(TransportEventLogConstants.Tuple_SmtpReceiveMaxConnectionPerSourceReached, this.clientIpAddress.ToString(), new object[]
				{
					this.Connector.Name,
					this.Connector.MaxInboundConnectionPerSource,
					this.clientIpAddress
				});
				this.WriteLineWithLogThenShutdown(SmtpResponse.TooManyConnectionsPerSource);
				return;
			}
			SmtpResponse banner;
			if (SmtpResponse.TryParse(this.Connector.Banner, out banner))
			{
				this.Banner = banner;
			}
			else
			{
				this.Banner = Util.SmtpBanner(this.Connector, () => this.server.Name, this.server.Version, this.server.CurrentTime, false);
			}
			this.firedOnConnectEvent = true;
			this.AgentSession.BeginRaiseEvent("OnConnectEvent", ConnectEventSourceImpl.Create(this.SessionSource), new ConnectEventArgs(this.SessionSource), new AsyncCallback(this.OnConnectCompleted), null);
		}

		public void SetupSessionToProxyTarget(SmtpSendConnectorConfig outboundProxyConnector, IEnumerable<INextHopServer> outboundProxyDestinations, TlsSendConfiguration outboundProxyTlsSendConfiguration, RiskLevel outboundProxyRiskLevel, int outboundProxyOutboundIPPool, string outboundProxyNextHopDomain, string outboundProxySessionId)
		{
			if (outboundProxyConnector == null)
			{
				throw new ArgumentNullException("outboundProxySendConnector");
			}
			if (outboundProxyDestinations == null)
			{
				throw new ArgumentNullException("outboundProxyDestinations");
			}
			if (!outboundProxyDestinations.GetEnumerator().MoveNext())
			{
				throw new ArgumentException("outboundProxyDestinations cannot be empty");
			}
			if (outboundProxyTlsSendConfiguration == null)
			{
				throw new ArgumentNullException("outboundProxyTlsSendConfiguratio");
			}
			if (this.StartClientProxySession)
			{
				throw new InvalidOperationException("StartOutboundProxySession and StartClientProxySession cannot both be true");
			}
			this.XProxyToParser = null;
			this.startOutboundProxySession = true;
			this.outboundProxyDestinations = outboundProxyDestinations;
			this.outboundProxySendConnector = outboundProxyConnector;
			this.outboundProxyTlsSendConfiguration = outboundProxyTlsSendConfiguration;
			this.outboundProxyRiskLevel = outboundProxyRiskLevel;
			this.outboundProxyOutboundIPPool = outboundProxyOutboundIPPool;
			this.outboundProxyNextHopDomain = outboundProxyNextHopDomain;
			this.outboundProxySessionId = outboundProxySessionId;
		}

		public void SetupExpectedBlobs(MailCommandMessageContextParameters messageContextParameters)
		{
			if (messageContextParameters == null)
			{
				ExTraceGlobals.SmtpReceiveTracer.TraceDebug((long)this.GetHashCode(), "SmtpInSession(id={0}) Not Messagecontext is specified");
				return;
			}
			if (this.expectedBlobs == null)
			{
				this.expectedBlobs = new Queue<SmtpMessageContextBlob>(messageContextParameters.OrderedListOfBlobs.Count);
			}
			foreach (IInboundMessageContextBlob inboundMessageContextBlob in messageContextParameters.OrderedListOfBlobs)
			{
				SmtpMessageContextBlob item = (SmtpMessageContextBlob)inboundMessageContextBlob;
				this.expectedBlobs.Enqueue(item);
			}
		}

		public void ResetExpectedBlobs()
		{
			if (this.expectedBlobs != null)
			{
				this.expectedBlobs.Clear();
			}
			this.mailCommandMessageContextInformation = null;
		}

		public bool ShouldRejectMailItem(RoutingAddress fromAddress, bool checkRecipientCount, out SmtpResponse failureSmtpResponse)
		{
			bool result = CommandParsingHelper.ShouldRejectMailItem(fromAddress, SmtpInSessionState.FromSmtpInSession(this), checkRecipientCount, out failureSmtpResponse);
			if (failureSmtpResponse.Equals(SmtpResponse.InsufficientResource))
			{
				this.UpdateSmtpAvailabilityPerfCounter(LegitimateSmtpAvailabilityCategory.RejectDueToBackPressure);
			}
			return result;
		}

		public SmtpResponse TrackAndEnqueueMailItem()
		{
			this.UpdateSmtpReceivePerfCountersForMessageReceived(this.transportMailItem.Recipients.Count, this.transportMailItem.MimeSize);
			if (this.transportMailItem.AuthMethod == MultilevelAuthMechanism.MutualTLS)
			{
				Utils.SecureMailPerfCounters.DomainSecureMessagesReceivedTotal.Increment();
			}
			if (!string.IsNullOrEmpty(this.transportMailItem.MessageTrackingSecurityInfo))
			{
				this.msgTrackInfo = new MsgTrackReceiveInfo(this.msgTrackInfo.ClientIPAddress, this.msgTrackInfo.ClientHostname, this.msgTrackInfo.ServerIPAddress, this.msgTrackInfo.SourceContext, this.msgTrackInfo.ConnectorId, this.msgTrackInfo.RelatedMailItemId, this.transportMailItem.MessageTrackingSecurityInfo, this.relatedMessageInfo, string.Empty, this.msgTrackInfo.ProxiedClientIPAddress, this.msgTrackInfo.ProxiedClientHostname, this.transportMailItem.RootPart.Headers.FindAll(HeaderId.Received), this.AuthUserMailboxGuid);
			}
			if (this.transportConfiguration.ProcessTransportRole != ProcessTransportRole.MailboxSubmission && this.transportConfiguration.ProcessTransportRole != ProcessTransportRole.MailboxDelivery)
			{
				MessageTrackingLog.TrackReceive(MessageTrackingSource.SMTP, this.transportMailItem, this.msgTrackInfo);
			}
			if (this.transportConfiguration.ProcessTransportRole == ProcessTransportRole.MailboxDelivery)
			{
				this.transportMailItem.ExtendedProperties.SetValue<string>("Microsoft.Exchange.Transport.MailboxTransport.SmtpInClientHostname", this.msgTrackInfo.ClientHostname);
			}
			LatencyTracker.EndTrackLatency(LatencyComponent.SmtpReceive, this.transportMailItem.LatencyTracker);
			this.messagesSubmitted++;
			if (this.delayedAckStatus == SmtpInSession.DelayedAckStatus.Stamped)
			{
				double num = 0.0;
				if (!PoisonMessage.DidMessageCrashTransport(this.transportMailItem.InternetMessageId, out num))
				{
					ExTraceGlobals.SmtpReceiveTracer.TraceDebug<long, Guid>((long)this.GetHashCode(), "SmtpInSession(id={0}).TrackAndEnqueMailItem(): ShadowRedundancyManager notified about delayed ack message '{1}'.", this.connectionId, this.transportMailItem.ShadowMessageId);
					this.server.ShadowRedundancyManager.EnqueueDelayedAckMessage(this.transportMailItem.ShadowMessageId, this, this.server.CurrentTime, this.MaxAcknowledgementDelay);
					this.delayedAckStatus = SmtpInSession.DelayedAckStatus.ShadowRedundancyManagerNotified;
				}
				else
				{
					this.delayedAckStatus = SmtpInSession.DelayedAckStatus.None;
					this.LogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, string.Format("Disabling Delayed Ack for potential poison message. Internet Message Id:" + this.transportMailItem.InternetMessageId, new object[0]));
					ExTraceGlobals.SmtpReceiveTracer.TraceDebug<long>((long)this.GetHashCode(), "SmtpInSession(id={0}).TrackAndEnqueMailItem: Disabling Delayed Ack for potential poison message.", this.connectionId);
				}
			}
			this.transportMailItem.PerfCounterAttribution = "InQueue";
			return this.server.Categorizer.EnqueueSubmittedMessage(this.transportMailItem);
		}

		public void TrackAndEnqueuePeerShadowMailItem()
		{
			this.messagesSubmitted++;
			this.transportMailItem.PerfCounterAttribution = "InQueue";
			this.shadowRedundancyManager.EnqueuePeerShadowMailItem(this.transportMailItem, this.PeerSessionPrimaryServer);
		}

		public void ReleaseMailItem()
		{
			try
			{
				if (this.TransportMailItemWrapper != null)
				{
					this.TransportMailItemWrapper.CloseWrapper();
					this.TransportMailItemWrapper = null;
				}
				this.CloseMessageWriteStream();
			}
			catch (IOException)
			{
			}
			finally
			{
				this.messageWriteStream = null;
				if (this.transportConfiguration.ProcessTransportRole == ProcessTransportRole.MailboxDelivery && this.transportMailItem != null)
				{
					this.transportMailItem.ReleaseFromActive();
				}
				this.transportMailItem = null;
				this.recipientCorrelator = null;
				this.ResetMailItemPermissions();
				this.tooManyRecipientsResponseCount = 0;
				this.tarpitRset = false;
				this.shadowSession = null;
			}
		}

		public void UpdateSessionWithProxyInformation(IPAddress clientIp, int clientPort, string clientHelloDomain, bool isAuthenticatedProxy, SecurityIdentifier securityId, string clientIdentityName, WindowsIdentity identity, TransportMiniRecipient recipient, int? capabilitiesInt)
		{
			this.proxiedClientAddress = clientIp;
			if (this.proxyHopHelloDomain == string.Empty)
			{
				this.proxyHopHelloDomain = this.HelloSmtpDomain;
			}
			this.HelloSmtpDomain = clientHelloDomain;
			this.SessionSource.RemoteEndPoint = new IPEndPoint(clientIp, clientPort);
			this.SessionSource.IsExternalConnection = !this.IsTrustedIP(this.proxiedClientAddress);
			this.SessionSource.LastExternalIPAddress = (this.SessionSource.IsExternalConnection ? this.proxiedClientAddress : null);
			if (isAuthenticatedProxy)
			{
				this.secureState = SecureState.StartTls;
				this.tlsDomainCapabilities = new SmtpReceiveCapabilities?(SmtpReceiveCapabilities.None);
				if (capabilitiesInt != null && (capabilitiesInt.Value & 128) == 128)
				{
					this.tlsDomainCapabilities = new SmtpReceiveCapabilities?(SmtpReceiveCapabilities.AcceptXSysProbeProtocol);
				}
				this.inboundClientProxyState = InboundClientProxyStates.XProxyReceived;
				this.SessionSource.IsClientProxiedSession = true;
				this.ResetSessionAuthentication();
				if (securityId != null)
				{
					this.RemoteIdentity = securityId;
					this.inboundClientProxyState = InboundClientProxyStates.XProxyReceivedAndAuthenticated;
					if (!string.IsNullOrEmpty(clientIdentityName))
					{
						this.RemoteIdentityName = clientIdentityName;
					}
					else
					{
						this.RemoteIdentityName = "unknown";
					}
					if (identity != null)
					{
						this.remoteWindowsIdentity = identity;
						this.SetSessionPermissions(identity.Token);
					}
					this.authUserRecipient = recipient;
				}
			}
		}

		public void UpdateSessionWithProxyFromInformation(IPAddress clientIp, int clientPort, string clientHelloDomain, uint xProxyFromSeqNum, uint? permissionsInt, AuthenticationSource? authSource)
		{
			this.UpdateSessionWithProxyInformation(clientIp, clientPort, clientHelloDomain, false, null, null, null, null, null);
			this.xProxyFromSeqNum = xProxyFromSeqNum;
			this.proxiedClientAuthSource = authSource;
			this.proxiedClientPermissions = permissionsInt;
			if (permissionsInt != null)
			{
				if ((permissionsInt.Value & 64U) == 64U)
				{
					this.sessionPermissions |= Permission.BypassAntiSpam;
				}
				else
				{
					this.sessionPermissions &= ~Permission.BypassAntiSpam;
				}
				this.enforceMimeLimitsForProxiedSession = ((permissionsInt.Value & 128U) == 0U);
			}
			this.sessionPermissions |= Permission.BypassMessageSizeLimit;
			this.SessionSource.IsInboundProxiedSession = true;
		}

		public void Shutdown()
		{
			this.DropBreadcrumb(SmtpInSessionBreadcrumbs.Shutdown);
			if (this.transportAppConfig.SmtpInboundProxyConfiguration.TrackInboundProxyDestinationsInRcpt)
			{
				if (this.DestinationTrackerLastNextHopFqdn != null)
				{
					this.SmtpInServer.InboundProxyDestinationTracker.DecrementProxyCount(this.DestinationTrackerLastNextHopFqdn);
				}
				if (this.DestinationTrackerLastExoAccountForest != null)
				{
					this.SmtpInServer.InboundProxyAccountForestTracker.DecrementProxyCount(this.DestinationTrackerLastExoAccountForest);
				}
			}
			bool flag = this.SessionSource.ShouldDisconnect && !this.disconnectByServer;
			if (!flag && this.firedOnConnectEvent)
			{
				this.DropBreadcrumb(SmtpInSessionBreadcrumbs.RaiseOnDisconnectEvent);
				this.AgentSession.BeginRaiseEvent("OnDisconnectEvent", DisconnectEventSourceImpl.Create(this.SessionSource), new DisconnectEventArgs(this.SessionSource), new AsyncCallback(this.ShutdownCompletedFromMEx), null);
			}
			else if (flag)
			{
				this.DropBreadcrumb(SmtpInSessionBreadcrumbs.ShutdownAgentDisconnect);
				ExTraceGlobals.SmtpReceiveTracer.TraceDebug<long>((long)this.GetHashCode(), "SmtpInSession(id={0}).Shutdown: An agent decided to drop the connection", this.connectionId);
				ISmtpReceivePerfCounters smtpReceivePerformanceCounters = this.SmtpReceivePerformanceCounters;
				if (smtpReceivePerformanceCounters != null)
				{
					smtpReceivePerformanceCounters.ConnectionsDroppedByAgentsTotal.Increment();
				}
				SmtpResponse response = this.SessionSource.SmtpResponse;
				if (response.Equals(SmtpResponse.Empty))
				{
					response = SmtpResponse.ConnectionDroppedByAgentError;
				}
				this.WriteLineWithLog(response, new AsyncCallback(this.ShutdownCompleted), null, true);
			}
			else
			{
				this.ShutdownCompleted(null);
			}
			ExTraceGlobals.SmtpReceiveTracer.TraceDebug<long>((long)this.GetHashCode(), "SmtpInSession(id={0}).Shutdown completed", this.connectionId);
		}

		public void ShutdownConnection()
		{
			this.DropBreadcrumb(SmtpInSessionBreadcrumbs.ShutdownConnection);
			this.connection.Shutdown();
			SmtpInSession.BlindProxyContext blindProxyContext = this.blindProxyContext;
			if (blindProxyContext != null)
			{
				blindProxyContext.ProxyConnection.Shutdown();
			}
			this.shutdownConnectionCalled = true;
		}

		public void Disconnect(DisconnectReason disconnectReason)
		{
			this.disconnectByServer = true;
			this.SessionSource.DisconnectReason = disconnectReason;
			this.SessionSource.ShouldDisconnect = true;
		}

		public void HandleBlindProxySetupFailure(SmtpResponse response, bool clientProxy)
		{
			this.DropBreadcrumb(SmtpInSessionBreadcrumbs.HandleBlindProxySetupFailure);
			if (clientProxy)
			{
				this.ResetSessionAuthentication();
			}
			this.proxySetupHandler.ReleaseReferences();
			this.proxySetupHandler = null;
			this.commandHandler.SmtpResponse = response;
			this.ContinueProcessCommand(true);
		}

		public void ResetSessionAuthentication()
		{
			this.sessionPermissions = this.AnonymousPermissions;
			this.LogInformation(ProtocolLoggingLevel.Verbose, "Set Session Permissions", Util.AsciiStringToBytes(Util.GetPermissionString(this.sessionPermissions)));
			this.sessionRemoteIdentity = SmtpConstants.AnonymousSecurityIdentifier;
			this.sessionRemoteIdentityName = "anonymous";
			if (this.remoteWindowsIdentity != null)
			{
				this.remoteWindowsIdentity.Dispose();
				this.remoteWindowsIdentity = null;
			}
			this.authUserRecipient = null;
			this.AuthMethod = MultilevelAuthMechanism.None;
		}

		public void HandleBlindProxySetupSuccess(SmtpResponse successfulResponse, NetworkConnection targetConnection, ulong sendSessionId, IProtocolLogSession sendLogSession, bool isClientProxy)
		{
			this.DropBreadcrumb(SmtpInSessionBreadcrumbs.HandleBlindProxySetupSuccess);
			this.blindProxyContext = new SmtpInSession.BlindProxyContext(this, targetConnection, sendLogSession, sendSessionId);
			this.commandHandler.ParsingStatus = ParsingStatus.Complete;
			this.commandHandler.SmtpResponse = successfulResponse;
			this.proxySetupHandler.ReleaseReferences();
			this.proxySetupHandler = null;
			if (this.ProxyPassword != null)
			{
				this.ProxyPassword.Dispose();
				this.ProxyPassword = null;
			}
			this.blindProxyingAuthenticatedUser = isClientProxy;
			this.ContinueProcessCommand(true);
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "[SmtpInSession: SessionId={0} ConnectionId={1}]", new object[]
			{
				this.SessionId,
				this.connectionId
			});
		}

		public byte[] GetCertificatePublicKey()
		{
			IX509Certificate2 localCertificate = this.connection.LocalCertificate;
			if (localCertificate != null)
			{
				return localCertificate.GetPublicKey();
			}
			return null;
		}

		public bool IsTrustedIP(IPAddress address)
		{
			MultiValuedProperty<IPRange> internalSMTPServers = this.SmtpInServer.TransportSettings.InternalSMTPServers;
			return Util.IsTrustedIP(address, internalSMTPServers);
		}

		public bool DetermineTlsDomainCapabilities()
		{
			if (this.SecureState != SecureState.StartTls)
			{
				throw new InvalidOperationException("DetermineTlsDomainCapabilities() invoked without STARTTLS");
			}
			if (this.tlsDomainCapabilities != null)
			{
				return true;
			}
			SmtpReceiveCapabilities value;
			if (Util.TryDetermineTlsDomainCapabilities(this.SmtpInServer.CertificateValidator, this.TlsRemoteCertificate, this.TlsRemoteCertificateChainValidationStatus, this.connectorStub, this.LogSession, this.eventLogger, ExTraceGlobals.SmtpReceiveTracer, out value))
			{
				this.tlsDomainCapabilities = new SmtpReceiveCapabilities?(value);
				return true;
			}
			return false;
		}

		public Permission MailItemPermissionsGranted { get; private set; }

		public Permission MailItemPermissionsDenied { get; private set; }

		public void GrantMailItemPermissions(Permission permissions)
		{
			this.MailItemPermissionsGranted |= permissions;
			this.MailItemPermissionsDenied &= ~permissions;
			this.LogInformation(ProtocolLoggingLevel.Verbose, "Granted Mail Item Permissions", Util.AsciiStringToBytes(Util.GetPermissionString(permissions)));
		}

		public void DenyMailItemPermissions(Permission permissions)
		{
			this.MailItemPermissionsGranted &= ~permissions;
			this.MailItemPermissionsDenied |= permissions;
			this.LogInformation(ProtocolLoggingLevel.Verbose, "Denied Mail Item Permissions", Util.AsciiStringToBytes(Util.GetPermissionString(permissions)));
		}

		public void ResetMailItemPermissions()
		{
			this.MailItemPermissionsGranted = Permission.None;
			this.MailItemPermissionsDenied = Permission.None;
		}

		public void RemoveClientIpConnection()
		{
			if (this.connectorStub != null && !this.clientIpConnectionAlreadyRemoved)
			{
				if (this.SmtpInServer.Ipv6ReceiveConnectionThrottlingEnabled)
				{
					this.connectorStub.RemoveConnection(this.significantAddressBytes);
				}
				else
				{
					this.connectorStub.RemoveConnection(this.clientIpAddress);
				}
				this.clientIpConnectionAlreadyRemoved = true;
			}
		}

		public bool CreateTransportMailItem(OrganizationId mailCommandInternalOrganizationId, Guid mailCommandExternalOrganizationId, MailDirectionality mailCommandDirectionality, string mailCommandExoAccountForest, string mailCommandExoTenantContainer, out SmtpResponse smtpResponse)
		{
			smtpResponse = SmtpResponse.Empty;
			if (this.transportMailItem != null)
			{
				throw new InvalidOperationException("Previous use of transportMailItem was not cleaned up properly.");
			}
			bool result = false;
			try
			{
				ADRecipientCache<TransportMiniRecipient> recipientCache = null;
				Guid externalOrgId = Guid.Empty;
				MailDirectionality directionality = MailDirectionality.Undefined;
				if (this.authUserRecipient != null)
				{
					SmtpAddress primarySmtpAddress = this.authUserRecipient.PrimarySmtpAddress;
					if (primarySmtpAddress.IsValidAddress)
					{
						ProxyAddress proxyAddress = new SmtpProxyAddress(primarySmtpAddress.ToString(), true);
						Result<TransportMiniRecipient> result2 = new Result<TransportMiniRecipient>(this.authUserRecipient, null);
						ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
						{
							recipientCache = new ADRecipientCache<TransportMiniRecipient>(TransportMiniRecipientSchema.Properties, 1, this.authUserRecipient.OrganizationId);
						}, 0);
						if (adoperationResult.Succeeded)
						{
							recipientCache.AddCacheEntry(proxyAddress, result2);
							directionality = MailDirectionality.Originating;
							adoperationResult = MultiTenantTransport.TryGetExternalOrgId(this.authUserRecipient.OrganizationId, out externalOrgId);
						}
						switch (adoperationResult.ErrorCode)
						{
						case ADOperationErrorCode.RetryableError:
							MultiTenantTransport.TraceAttributionError("Retriable Error {0} attributing authUserRecipient {1}", new object[]
							{
								adoperationResult.Exception,
								this.authUserRecipient.PrimarySmtpAddress
							});
							smtpResponse = SmtpResponse.HubAttributionTransientFailureInCreateTmi;
							return false;
						case ADOperationErrorCode.PermanentError:
							if (this.transportAppConfig.SmtpReceiveConfiguration.RejectUnscopedMessages)
							{
								MultiTenantTransport.TraceAttributionError("Permanent Error {0} attributing authUserRecipient {1}", new object[]
								{
									adoperationResult.Exception,
									this.authUserRecipient.PrimarySmtpAddress
								});
								smtpResponse = SmtpResponse.HubAttributionFailureInCreateTmi;
								return false;
							}
							MultiTenantTransport.TraceAttributionError("Permanent Error {0} attributing authUserRecipient {1}. Falling back to safe tenant", new object[]
							{
								adoperationResult.Exception,
								this.authUserRecipient.PrimarySmtpAddress
							});
							externalOrgId = MultiTenantTransport.SafeTenantId;
							break;
						}
					}
				}
				this.transportMailItem = TransportMailItem.NewMailItem(recipientCache, LatencyComponent.SmtpReceive, directionality, externalOrgId);
				this.transportMailItem.ExposeMessage = false;
				this.transportMailItem.ExposeMessageHeaders = false;
				this.transportMailItem.PerfCounterAttribution = "SMTPIn";
				if (!this.IsShadowedBySender && this.MaxAcknowledgementDelay > TimeSpan.Zero && this.server.ShadowRedundancyManager != null && this.server.ShadowRedundancyManager.ShouldDelayAck())
				{
					ExTraceGlobals.SmtpReceiveTracer.TraceDebug<long>((long)this.GetHashCode(), "SmtpInSession(id={0}).CreateTransportMailItem: Message stamped as a delayed ack message.", this.connectionId);
					this.transportMailItem.ShadowServerContext = "$localhost$";
					this.transportMailItem.ShadowServerDiscardId = this.transportMailItem.ShadowMessageId.ToString();
					this.delayedAckStatus = SmtpInSession.DelayedAckStatus.Stamped;
				}
				else if (this.IsPeerShadowSession)
				{
					this.transportMailItem.ShadowServerContext = this.shadowRedundancyManager.GetShadowContextForInboundSession();
					this.transportMailItem.ShadowServerDiscardId = this.transportMailItem.ShadowMessageId.ToString();
				}
				else
				{
					this.delayedAckStatus = SmtpInSession.DelayedAckStatus.None;
				}
				string str = "SMTP:";
				this.transportMailItem.ReceiveConnectorName = str + (this.Connector.Name ?? "Unknown");
				this.transportMailItem.SourceIPAddress = ((this.proxiedClientAddress != null) ? this.proxiedClientAddress : this.clientIpAddress);
				this.transportMailItem.AuthMethod = this.sessionAuthMethod;
				this.inboundExch50 = null;
				if (this.ShouldInitializeMessageTrackingInfo())
				{
					IPAddress proxiedClientIPAddress = null;
					string proxiedClientHostname = string.Empty;
					string helloDomain;
					if (this.InboundClientProxyState != InboundClientProxyStates.None || this.IsAnonymousClientProxiedSession)
					{
						helloDomain = this.proxyHopHelloDomain;
						proxiedClientIPAddress = this.proxiedClientAddress;
						proxiedClientHostname = this.HelloDomain;
					}
					else
					{
						helloDomain = this.HelloDomain;
					}
					this.msgTrackInfo = new MsgTrackReceiveInfo(this.connection.RemoteEndPoint.Address, helloDomain, this.connection.LocalEndPoint.Address, this.CurrentMessageTemporaryId, this.connector.Id.ToString(), null, proxiedClientIPAddress, proxiedClientHostname, this.AuthUserMailboxGuid);
				}
				this.recipientCorrelator = new InboundRecipientCorrelator();
				if (!this.SessionSource.IsExternalConnection)
				{
					this.SessionSource.LastExternalIPAddress = null;
				}
				if (mailCommandExternalOrganizationId != Guid.Empty && mailCommandDirectionality != MailDirectionality.Undefined)
				{
					this.TransportMailItem.ExternalOrganizationId = mailCommandExternalOrganizationId;
					this.TransportMailItem.Directionality = mailCommandDirectionality;
					this.TransportMailItem.ExoAccountForest = mailCommandExoAccountForest;
					this.TransportMailItem.ExoTenantContainer = mailCommandExoTenantContainer;
					ADOperationResult adoperationResult2 = SmtpInSessionUtils.TryCreateOrUpdateADRecipientCache(this.transportMailItem, mailCommandInternalOrganizationId, this.LogSession);
					if (adoperationResult2.ErrorCode == ADOperationErrorCode.RetryableError)
					{
						smtpResponse = SmtpResponse.HubAttributionTransientFailureInMailFrom;
						this.transportMailItem = null;
						this.recipientCorrelator = null;
						return false;
					}
				}
				result = true;
			}
			catch (IOException)
			{
				this.UpdateSmtpAvailabilityPerfCounter(LegitimateSmtpAvailabilityCategory.RejectDueToIOException);
				this.transportMailItem = null;
				this.recipientCorrelator = null;
				smtpResponse = SmtpResponse.DataTransactionFailed;
			}
			this.numberOfMessagesReceived++;
			return result;
		}

		public void UpdateSmtpAvailabilityPerfCounter(LegitimateSmtpAvailabilityCategory category)
		{
			if (this.smtpAvailabilityPerfCounters != null)
			{
				this.smtpAvailabilityPerfCounters.UpdatePerformanceCounters(category);
			}
		}

		public void IncrementSmtpAvailabilityPerfCounterForMessageLoopsInLastHour(long incrementValue)
		{
			if (this.smtpAvailabilityPerfCounters != null)
			{
				this.smtpAvailabilityPerfCounters.IncrementMessageLoopsInLastHourCounter(incrementValue);
			}
		}

		public void UpdateSmtpReceivePerfCountersForMessageReceived(int recipients, long messageBytes)
		{
			ISmtpReceivePerfCounters smtpReceivePerformanceCounters = this.SmtpReceivePerformanceCounters;
			if (smtpReceivePerformanceCounters != null)
			{
				smtpReceivePerformanceCounters.MessagesReceivedTotal.Increment();
				smtpReceivePerformanceCounters.RecipientsAccepted.IncrementBy((long)recipients);
				smtpReceivePerformanceCounters.MessageBytesReceivedTotal.IncrementBy(messageBytes);
			}
		}

		public void UpdateInboundProxyDestinationPerfCountersForMessageReceived(int recipients, long messageBytes)
		{
			IInboundProxyDestinationPerfCounters inboundProxyDestinationPerfCounters = this.InboundProxyDestinationPerfCounters;
			inboundProxyDestinationPerfCounters.MessagesReceivedTotal.Increment();
			inboundProxyDestinationPerfCounters.RecipientsAccepted.IncrementBy((long)recipients);
			inboundProxyDestinationPerfCounters.MessageBytesReceivedTotal.IncrementBy(messageBytes);
			IInboundProxyDestinationPerfCounters inboundProxyAccountForestPerfCounters = this.InboundProxyAccountForestPerfCounters;
			inboundProxyAccountForestPerfCounters.MessagesReceivedTotal.Increment();
			inboundProxyAccountForestPerfCounters.RecipientsAccepted.IncrementBy((long)recipients);
			inboundProxyAccountForestPerfCounters.MessageBytesReceivedTotal.IncrementBy(messageBytes);
		}

		public void DeleteTransportMailItem()
		{
			this.DropBreadcrumb(SmtpInSessionBreadcrumbs.DeleteTransportMailItem);
			if (this.transportMailItem != null && !this.transportMailItem.IsNew && this.transportMailItem.IsActive)
			{
				this.TransportMailItem.ReleaseFromActiveMaterializedLazy();
			}
			this.ReleaseMailItem();
		}

		public void AbortMailTransaction()
		{
			if (this.shadowSession != null)
			{
				this.shadowSession.Close(AckStatus.Fail, SmtpResponse.Empty);
			}
			if (this.transportMailItem != null)
			{
				this.DeleteTransportMailItem();
			}
			SmtpInBdatState smtpInBdatState = this.BdatState;
			if (smtpInBdatState != null && smtpInBdatState.ProxyLayer != null)
			{
				smtpInBdatState.ProxyLayer.NotifySmtpInStopProxy();
			}
			this.BdatState = null;
			this.shadowSession = null;
		}

		public Stream OpenMessageWriteStream(bool expectBinaryContent)
		{
			if (this.transportMailItem == null)
			{
				throw new InvalidOperationException("No transport message");
			}
			MimeLimits mimeLimits;
			if (SmtpInSessionUtils.HasSMTPBypassMessageSizeLimitPermission(this.Permissions) && !this.enforceMimeLimitsForProxiedSession)
			{
				mimeLimits = MimeLimits.Unlimited;
			}
			else
			{
				mimeLimits = MimeLimits.Default;
			}
			this.messageWriteStream = this.transportMailItem.OpenMimeWriteStream(mimeLimits, expectBinaryContent);
			return this.messageWriteStream;
		}

		public void CloseMessageWriteStream()
		{
			Util.CloseMessageWriteStream(this.messageWriteStream, this.transportMailItem, ExTraceGlobals.SmtpReceiveTracer, this.GetHashCode());
			this.messageWriteStream = null;
		}

		public void PutBackReceivedBytes(int bytesUnconsumed)
		{
			ISmtpReceivePerfCounters smtpReceivePerformanceCounters = this.SmtpReceivePerformanceCounters;
			if (smtpReceivePerformanceCounters != null)
			{
				smtpReceivePerformanceCounters.TotalBytesReceived.IncrementBy((long)(-(long)bytesUnconsumed));
			}
			this.connection.PutBackReceivedBytes(bytesUnconsumed);
		}

		public void RawDataReceivedCompleted()
		{
			ExTraceGlobals.SmtpReceiveTracer.TraceDebug<long, string>((long)this.GetHashCode(), "SmtpInSession(id={0}).RawDataReceivedCompleted. ParsingStatus = {1}", this.connectionId, this.commandHandler.ParsingStatus.ToString());
			this.DropBreadcrumb(SmtpInSessionBreadcrumbs.RawDataReceivedCompleted);
			if (this.SessionSource.ShouldDisconnect)
			{
				this.DropBreadcrumb(SmtpInSessionBreadcrumbs.DisconnectFromRawDataReceivedCompleted);
				ExTraceGlobals.SmtpReceiveTracer.TraceDebug<long>((long)this.GetHashCode(), "SmtpInSession(id={0}).RawDataReceivedCompleted calling Shutdown", this.connectionId);
				SmtpResponse response = SmtpResponse.Empty;
				if (this.commandHandler != null)
				{
					response = this.commandHandler.SmtpResponse;
				}
				this.DisposeSmtpCommand();
				this.rawDataHandler = null;
				if (this.disconnectByServer && !response.Equals(SmtpResponse.Empty))
				{
					this.WriteLineWithLogThenShutdown(response);
					return;
				}
				this.Shutdown();
				return;
			}
			else
			{
				if (this.commandHandler.ParsingStatus == ParsingStatus.MoreDataRequired)
				{
					this.StartRead();
					return;
				}
				this.rawDataHandler = null;
				this.DelayResponseIfNecessary(true);
				return;
			}
		}

		public void SetRawModeAfterCommandCompleted(RawDataHandler rawDataHandler)
		{
			this.rawDataHandler = rawDataHandler;
		}

		public void LogInformation(ProtocolLoggingLevel loggingLevel, string information, byte[] data)
		{
			this.logSession.LogInformation(loggingLevel, data, information);
		}

		public void StartTls(SecureState secureState)
		{
			this.secureState = (secureState | SecureState.NegotiationRequested);
		}

		public IAsyncResult RaiseOnRejectEvent(byte[] command, EventArgs originalEventArgs, SmtpResponse smtpResponse, AsyncCallback callback)
		{
			this.DropBreadcrumb(SmtpInSessionBreadcrumbs.RaiseOnRejectEvent);
			RejectEventArgs rejectEventArgs = new RejectEventArgs(this.SessionSource);
			rejectEventArgs.RawCommand = command;
			rejectEventArgs.ParsingStatus = EnumConverter.InternalToPublic(this.commandHandler.ParsingStatus);
			rejectEventArgs.OriginalArguments = originalEventArgs;
			rejectEventArgs.SmtpResponse = smtpResponse;
			return this.AgentSession.BeginRaiseEvent("OnReject", RejectEventSourceImpl.Create(this.SessionSource), rejectEventArgs, callback, null);
		}

		public byte[] GetTlsEapKey()
		{
			return this.connection.TlsEapKey;
		}

		internal void SetSessionPermissions(SecurityIdentifier client)
		{
			ExTraceGlobals.FaultInjectionTracer.TraceTest(2347117885U);
			this.sessionPermissions = SmtpInSessionUtils.GetPermissions(this.authzAuthorization, client, this.connectorStub.SecurityDescriptor);
			ExTraceGlobals.SmtpReceiveTracer.TraceDebug<string, Permission>((long)this.GetHashCode(), "Client '{0}' is granted the following permissions: {1}", this.RemoteIdentityName, this.sessionPermissions);
			this.LogInformation(ProtocolLoggingLevel.Verbose, "Set Session Permissions", Util.AsciiStringToBytes(Util.GetPermissionString(this.sessionPermissions)));
		}

		public void SetSessionPermissions(IntPtr userToken)
		{
			this.sessionPermissions = SmtpInSessionUtils.GetPermissions(this.authzAuthorization, userToken, this.connectorStub.SecurityDescriptor);
			ExTraceGlobals.SmtpReceiveTracer.TraceDebug<string, Permission>((long)this.GetHashCode(), "Client '{0}' is granted the following permissions: {1}", this.RemoteIdentityName, this.sessionPermissions);
			this.LogInformation(ProtocolLoggingLevel.Verbose, "Set Session Permissions", Util.AsciiStringToBytes(Util.GetPermissionString(this.sessionPermissions)));
		}

		public void AddSessionPermissions(SmtpReceiveCapabilities capabilities)
		{
			this.SessionPermissions = Util.AddSessionPermissions(capabilities, this.SessionPermissions, this.authzAuthorization, this.connectorStub.SecurityDescriptor, this.logSession, ExTraceGlobals.SmtpReceiveTracer, this.GetHashCode());
			if (SmtpInSessionUtils.HasAcceptCrossForestMailCapability(capabilities))
			{
				this.RemoteIdentity = WellKnownSids.ExternallySecuredServers;
				this.RemoteIdentityName = "accepted_domain";
			}
		}

		public void DropBreadcrumb(SmtpInSessionBreadcrumbs breadcrumb)
		{
			this.breadcrumbs.Drop(breadcrumb);
		}

		public void SetupPoisonContext()
		{
			if (this.transportMailItem != null && !string.IsNullOrEmpty(this.transportMailItem.InternetMessageId))
			{
				PoisonMessage.Context = new MessageContext(this.transportMailItem.RecordId, this.transportMailItem.InternetMessageId, MessageProcessingSource.SmtpReceive);
			}
		}

		private static void ToShutdown(IAsyncResult asyncResult)
		{
			SmtpInSession.WriteCompleteLogCallbackParameters writeCompleteLogCallbackParameters = (SmtpInSession.WriteCompleteLogCallbackParameters)asyncResult.AsyncState;
			SmtpInSession session = writeCompleteLogCallbackParameters.Session;
			session.DropBreadcrumb(SmtpInSessionBreadcrumbs.ToShutdown);
			session.SetupPoisonContext();
			session.Shutdown();
		}

		private static void StartTlsNegotiation(IAsyncResult asyncResult)
		{
			SmtpInSession.WriteCompleteLogCallbackParameters writeCompleteLogCallbackParameters = (SmtpInSession.WriteCompleteLogCallbackParameters)asyncResult.AsyncState;
			SmtpInSession session = writeCompleteLogCallbackParameters.Session;
			session.DropBreadcrumb(SmtpInSessionBreadcrumbs.StartTlsNegotiation);
			session.SetupPoisonContext();
			SmtpInSession smtpInSession = session;
			smtpInSession.secureState &= ~SecureState.NegotiationRequested;
			X509Certificate2 cert = (session.secureState == SecureState.StartTls) ? session.AdvertisedTlsCertificate : session.InternalTransportCertificate;
			session.logSession.LogCertificate("Sending certificate", cert);
			bool requestClientCertificate = false;
			if (session.secureState == SecureState.AnonymousTls)
			{
				requestClientCertificate = true;
			}
			else if (session.secureState == SecureState.StartTls)
			{
				if (session.ForceRequestClientTlsCertificate)
				{
					requestClientCertificate = true;
				}
				else if (session.Connector.DomainSecureEnabled && session.SmtpInServer.TransportSettings.TLSReceiveDomainSecureList.Count > 0)
				{
					requestClientCertificate = true;
				}
				else if (session.ConnectorStub.ContainsTlsDomainCapabilities)
				{
					requestClientCertificate = true;
				}
			}
			session.connection.BeginNegotiateTlsAsServer(cert, requestClientCertificate, SmtpInSession.tlsNegotiationComplete, session);
		}

		private static void BeginReadLine(IAsyncResult asyncResult)
		{
			SmtpInSession.WriteCompleteLogCallbackParameters writeCompleteLogCallbackParameters = (SmtpInSession.WriteCompleteLogCallbackParameters)asyncResult.AsyncState;
			SmtpInSession session = writeCompleteLogCallbackParameters.Session;
			session.DropBreadcrumb(SmtpInSessionBreadcrumbs.BeginReadLine);
			session.SetupPoisonContext();
			session.StartReadLine();
		}

		private static void BeginRead(IAsyncResult asyncResult)
		{
			SmtpInSession.WriteCompleteLogCallbackParameters writeCompleteLogCallbackParameters = (SmtpInSession.WriteCompleteLogCallbackParameters)asyncResult.AsyncState;
			SmtpInSession session = writeCompleteLogCallbackParameters.Session;
			session.DropBreadcrumb(SmtpInSessionBreadcrumbs.BeginRead);
			session.SetupPoisonContext();
			session.StartRead();
		}

		private static void StartProxying(IAsyncResult asyncResult)
		{
			SmtpInSession.WriteCompleteLogCallbackParameters writeCompleteLogCallbackParameters = (SmtpInSession.WriteCompleteLogCallbackParameters)asyncResult.AsyncState;
			SmtpInSession session = writeCompleteLogCallbackParameters.Session;
			SmtpInSession.BlindProxyContext blindProxyContext = (SmtpInSession.BlindProxyContext)writeCompleteLogCallbackParameters.CallbackContextParam;
			session.DropBreadcrumb(SmtpInSessionBreadcrumbs.StartProxying);
			if (blindProxyContext == null)
			{
				throw new InvalidOperationException("blindProxyContext is null in StartProxying()");
			}
			session.StartReadFromProxyClient(blindProxyContext);
			session.StartReadFromProxyTarget(blindProxyContext);
		}

		private static void WriteCompleteLogCallback(IAsyncResult asyncResult)
		{
			SmtpInSession.WriteCompleteLogCallbackParameters writeCompleteLogCallbackParameters = (SmtpInSession.WriteCompleteLogCallbackParameters)asyncResult.AsyncState;
			writeCompleteLogCallbackParameters.Session.SetupPoisonContext();
			object obj;
			writeCompleteLogCallbackParameters.Session.connection.EndWrite(asyncResult, out obj);
			if (obj == null)
			{
				if (writeCompleteLogCallbackParameters.ResponseList != null)
				{
					foreach (SmtpResponse response in writeCompleteLogCallbackParameters.ResponseList)
					{
						writeCompleteLogCallbackParameters.Session.WriteLog(response);
					}
				}
				writeCompleteLogCallbackParameters.Callback(asyncResult);
				return;
			}
			if (writeCompleteLogCallbackParameters.AlwaysCall)
			{
				writeCompleteLogCallbackParameters.Callback(asyncResult);
				return;
			}
			if (writeCompleteLogCallbackParameters.CallbackContextParam is SmtpInSession.BlindProxyContext)
			{
				SmtpProxyPerfCountersWrapper smtpProxyPerfCountersWrapper = writeCompleteLogCallbackParameters.Session.SmtpProxyPerfCounters;
				if (smtpProxyPerfCountersWrapper != null)
				{
					smtpProxyPerfCountersWrapper.DecrementOutboundConnectionsCurrent();
				}
			}
			writeCompleteLogCallbackParameters.Session.HandleError(obj, false);
		}

		private static void ReadComplete(IAsyncResult asyncResult)
		{
			SmtpInSession smtpInSession = (SmtpInSession)asyncResult.AsyncState;
			smtpInSession.SetupPoisonContext();
			try
			{
				byte[] data;
				int offset;
				int num;
				object obj;
				smtpInSession.connection.EndRead(asyncResult, out data, out offset, out num, out obj);
				if (obj != null)
				{
					smtpInSession.HandleError(obj, true);
				}
				else if (smtpInSession.sessionExpireTime < smtpInSession.server.CurrentTime)
				{
					smtpInSession.WriteLineWithLogThenShutdown(SmtpResponse.ConnectionTimedOut);
				}
				else
				{
					ISmtpReceivePerfCounters smtpReceivePerformanceCounters = smtpInSession.SmtpReceivePerformanceCounters;
					if (smtpReceivePerformanceCounters != null)
					{
						smtpReceivePerformanceCounters.TotalBytesReceived.IncrementBy((long)num);
					}
					if (smtpInSession.rawDataHandler(data, offset, num) == AsyncReturnType.Sync)
					{
						smtpInSession.RawDataReceivedCompleted();
					}
				}
			}
			catch (Exception ex)
			{
				smtpInSession.eventLogger.LogEvent(TransportEventLogConstants.Tuple_SmtpReceiveCatchAll, null, new object[]
				{
					smtpInSession.clientIpAddress,
					ex
				});
				throw;
			}
		}

		private static void ReadCompleteFromProxyClient(IAsyncResult asyncResult)
		{
			SmtpInSession.BlindProxyContext blindProxyContext = (SmtpInSession.BlindProxyContext)asyncResult.AsyncState;
			SmtpInSession session = blindProxyContext.Session;
			session.DropBreadcrumb(SmtpInSessionBreadcrumbs.ReadCompleteFromProxyClient);
			session.SetupPoisonContext();
			try
			{
				byte[] buffer;
				int offset;
				int num;
				object obj;
				session.connection.EndRead(asyncResult, out buffer, out offset, out num, out obj);
				if (obj != null)
				{
					session.HandleErrorDuringBlindProxying(obj, blindProxyContext, true, true);
				}
				else
				{
					ISmtpReceivePerfCounters smtpReceivePerformanceCounters = session.SmtpReceivePerformanceCounters;
					if (smtpReceivePerformanceCounters != null)
					{
						smtpReceivePerformanceCounters.TotalBytesReceived.IncrementBy((long)num);
					}
					if (session.blindProxyingAuthenticatedUser || !session.ReceivedAndProcessedXRsetProxyToCommand(buffer, offset, num, blindProxyContext))
					{
						session.StartWriteToProxyTarget(buffer, offset, num, blindProxyContext);
						session.bytesToBeProxied = num;
					}
				}
			}
			catch (Exception ex)
			{
				session.eventLogger.LogEvent(TransportEventLogConstants.Tuple_SmtpReceiveProxyCatchAll, null, new object[]
				{
					session.SessionId.ToString("X16", NumberFormatInfo.InvariantInfo),
					session.clientIpAddress,
					ex,
					SmtpInSessionUtils.GetBreadcrumbsAsString(session.Breadcrumbs)
				});
				throw;
			}
		}

		private static void WriteToProxyTargetCompleted(IAsyncResult asyncResult)
		{
			SmtpInSession.BlindProxyContext blindProxyContext = (SmtpInSession.BlindProxyContext)asyncResult.AsyncState;
			SmtpInSession session = blindProxyContext.Session;
			session.DropBreadcrumb(SmtpInSessionBreadcrumbs.WriteToProxyTargetCompleted);
			session.SetupPoisonContext();
			try
			{
				object obj;
				blindProxyContext.ProxyConnection.EndWrite(asyncResult, out obj);
				if (Interlocked.CompareExchange(ref blindProxyContext.IsProxyTargetWritePending, 0, 1) != 1)
				{
					throw new InvalidOperationException("A write operation was not pending for proxy target");
				}
				if (obj != null)
				{
					session.HandleErrorDuringBlindProxying(obj, blindProxyContext, false, false);
				}
				else
				{
					SmtpProxyPerfCountersWrapper smtpProxyPerfCountersWrapper = session.smtpProxyPerfCounters;
					if (smtpProxyPerfCountersWrapper != null)
					{
						smtpProxyPerfCountersWrapper.UpdateBytesProxied(session.bytesToBeProxied);
					}
					session.bytesToBeProxied = 0;
					if (!session.StartWriteQuitToProxyTargetIfNecessary(blindProxyContext))
					{
						session.StartReadFromProxyClient(blindProxyContext);
					}
				}
			}
			catch (Exception ex)
			{
				NetworkConnection proxyConnection = blindProxyContext.ProxyConnection;
				session.eventLogger.LogEvent(TransportEventLogConstants.Tuple_SmtpReceiveProxyCatchAll, null, new object[]
				{
					session.SessionId.ToString("X16", NumberFormatInfo.InvariantInfo),
					(proxyConnection == null) ? "disposed" : proxyConnection.RemoteEndPoint.Address.ToString(),
					ex,
					SmtpInSessionUtils.GetBreadcrumbsAsString(session.Breadcrumbs)
				});
				throw;
			}
		}

		private static void WriteQuitToProxyTargetCompleted(IAsyncResult asyncResult)
		{
			SmtpInSession.BlindProxyContext blindProxyContext = (SmtpInSession.BlindProxyContext)asyncResult.AsyncState;
			SmtpInSession session = blindProxyContext.Session;
			session.DropBreadcrumb(SmtpInSessionBreadcrumbs.WriteQuitToProxyTargetCompleted);
			session.SetupPoisonContext();
			try
			{
				object obj;
				blindProxyContext.ProxyConnection.EndWrite(asyncResult, out obj);
				if (Interlocked.CompareExchange(ref blindProxyContext.IsProxyTargetWritePending, 0, 1) != 1)
				{
					throw new InvalidOperationException("WriteQuitToProxyTargetCompleted: A write operation was not pending for proxy target");
				}
				blindProxyContext.BlindProxySendLogSession.LogSend(SmtpInSession.QuitCommand);
				blindProxyContext.BlindProxySendLogSession.LogDisconnect(DisconnectReason.QuitVerb);
				ConnectionLog.SmtpConnectionStop(blindProxyContext.BlindProxySendSessionId, string.Empty, string.Empty, 0UL, 0UL, 0UL);
				blindProxyContext.ProxyConnection.Shutdown();
				blindProxyContext.ProxyConnection.Dispose();
				SmtpProxyPerfCountersWrapper smtpProxyPerfCountersWrapper = session.SmtpProxyPerfCounters;
				if (smtpProxyPerfCountersWrapper != null)
				{
					smtpProxyPerfCountersWrapper.DecrementOutboundConnectionsCurrent();
				}
			}
			catch (Exception ex)
			{
				session.eventLogger.LogEvent(TransportEventLogConstants.Tuple_SmtpReceiveProxyCatchAll, null, new object[]
				{
					session.SessionId.ToString("X16", NumberFormatInfo.InvariantInfo),
					blindProxyContext.ProxyConnection.RemoteEndPoint.Address.ToString(),
					ex,
					SmtpInSessionUtils.GetBreadcrumbsAsString(session.Breadcrumbs)
				});
				throw;
			}
		}

		private static void ReadCompleteFromProxyTarget(IAsyncResult asyncResult)
		{
			SmtpInSession.BlindProxyContext blindProxyContext = (SmtpInSession.BlindProxyContext)asyncResult.AsyncState;
			SmtpInSession session = blindProxyContext.Session;
			session.DropBreadcrumb(SmtpInSessionBreadcrumbs.ReadCompleteFromProxyTarget);
			if (Util.InterlockedEquals(ref blindProxyContext.BlindProxyWorkDone, 1))
			{
				return;
			}
			session.SetupPoisonContext();
			try
			{
				byte[] buffer;
				int offset;
				int size;
				object obj;
				blindProxyContext.ProxyConnection.EndRead(asyncResult, out buffer, out offset, out size, out obj);
				if (obj != null)
				{
					session.HandleErrorDuringBlindProxying(obj, blindProxyContext, false, true);
				}
				else
				{
					session.StartWriteToProxyClient(buffer, offset, size, blindProxyContext);
				}
			}
			catch (Exception ex)
			{
				session.eventLogger.LogEvent(TransportEventLogConstants.Tuple_SmtpReceiveProxyCatchAll, null, new object[]
				{
					session.SessionId.ToString("X16", NumberFormatInfo.InvariantInfo),
					blindProxyContext.ProxyConnection.RemoteEndPoint.Address.ToString(),
					ex,
					SmtpInSessionUtils.GetBreadcrumbsAsString(session.Breadcrumbs)
				});
				throw;
			}
		}

		private static void WriteToProxyClientCompleted(IAsyncResult asyncResult)
		{
			SmtpInSession.BlindProxyContext blindProxyContext = (SmtpInSession.BlindProxyContext)asyncResult.AsyncState;
			SmtpInSession session = blindProxyContext.Session;
			session.DropBreadcrumb(SmtpInSessionBreadcrumbs.WriteToProxyClientCompleted);
			session.SetupPoisonContext();
			try
			{
				object obj;
				session.connection.EndWrite(asyncResult, out obj);
				if (Interlocked.CompareExchange(ref blindProxyContext.IsProxyClientWritePending, 0, 1) != 1)
				{
					throw new InvalidOperationException("A write operation was not pending for proxy client");
				}
				if (obj != null)
				{
					session.HandleErrorDuringBlindProxying(obj, blindProxyContext, true, false);
				}
				else if (!session.StartWriteXRsetResponseToProxyClientIfNecessary(blindProxyContext) && Util.InterlockedEquals(ref blindProxyContext.BlindProxyWorkDone, 0))
				{
					session.StartReadFromProxyTarget(blindProxyContext);
				}
			}
			catch (Exception ex)
			{
				session.eventLogger.LogEvent(TransportEventLogConstants.Tuple_SmtpReceiveProxyCatchAll, null, new object[]
				{
					session.SessionId.ToString("X16", NumberFormatInfo.InvariantInfo),
					session.clientIpAddress,
					ex,
					SmtpInSessionUtils.GetBreadcrumbsAsString(session.Breadcrumbs)
				});
				throw;
			}
		}

		private static void WriteXRsetResponseToProxyClientCompleted(IAsyncResult asyncResult)
		{
			SmtpInSession.BlindProxyContext blindProxyContext = (SmtpInSession.BlindProxyContext)asyncResult.AsyncState;
			SmtpInSession session = blindProxyContext.Session;
			session.DropBreadcrumb(SmtpInSessionBreadcrumbs.WriteXRsetResponseToProxyClientCompleted);
			session.SetupPoisonContext();
			try
			{
				object obj;
				session.connection.EndWrite(asyncResult, out obj);
				if (Interlocked.CompareExchange(ref blindProxyContext.IsProxyClientWritePending, 0, 1) != 1)
				{
					throw new InvalidOperationException("WriteXRsetResponseToProxyClientCompleted: A write operation was not pending for proxy client");
				}
				if (obj != null)
				{
					session.HandleError(obj, false);
				}
				else
				{
					byte[] data = session.CreateXRsetProxyToAcceptedResponse().ToByteArray();
					session.LogSession.LogSend(data);
					session.StartReadLine();
				}
			}
			catch (Exception ex)
			{
				session.eventLogger.LogEvent(TransportEventLogConstants.Tuple_SmtpReceiveProxyCatchAll, null, new object[]
				{
					session.SessionId.ToString("X16", NumberFormatInfo.InvariantInfo),
					session.clientIpAddress,
					ex,
					SmtpInSessionUtils.GetBreadcrumbsAsString(session.Breadcrumbs)
				});
				throw;
			}
		}

		private static void ReadLineComplete(IAsyncResult asyncResult)
		{
			SmtpInSession smtpInSession = (SmtpInSession)asyncResult.AsyncState;
			bool flag = false;
			smtpInSession.SetupPoisonContext();
			try
			{
				smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.ReadLineComplete);
				ExTraceGlobals.SmtpReceiveTracer.TraceDebug<long>((long)smtpInSession.GetHashCode(), "SmtpInSession(id={0}).ReadLinecomplete", smtpInSession.connectionId);
				byte[] inputBuffer;
				int offset;
				int num;
				object obj;
				smtpInSession.connection.EndReadLine(asyncResult, out inputBuffer, out offset, out num, out obj);
				if (obj != null)
				{
					if (!(obj is SocketError) || (SocketError)obj != SocketError.MessageSize)
					{
						smtpInSession.HandleError(obj, true);
						return;
					}
					flag = true;
				}
				ISmtpReceivePerfCounters smtpReceivePerformanceCounters = smtpInSession.SmtpReceivePerformanceCounters;
				if (smtpReceivePerformanceCounters != null)
				{
					smtpReceivePerformanceCounters.TotalBytesReceived.IncrementBy((long)(num + (flag ? 0 : 2)));
				}
				if (smtpInSession.sessionExpireTime < smtpInSession.server.CurrentTime)
				{
					smtpInSession.WriteLineWithLogThenShutdown(SmtpResponse.ConnectionTimedOut);
				}
				else if (smtpInSession.server.RejectCommands)
				{
					ExTraceGlobals.SmtpReceiveTracer.TraceDebug<long>((long)smtpInSession.GetHashCode(), "SmtpInSession(id={0}) disconnected: reject commands", smtpInSession.connectionId);
					smtpInSession.WriteLineWithLogThenShutdown(smtpInSession.server.RejectionSmtpResponse);
				}
				else if (smtpInSession.StartProcessingCommand(inputBuffer, offset, num, flag) == AsyncReturnType.Sync)
				{
					if (smtpInSession.rawDataHandler == null)
					{
						smtpInSession.StartReadLine();
					}
					else
					{
						smtpInSession.StartRead();
					}
				}
			}
			catch (Exception ex)
			{
				smtpInSession.eventLogger.LogEvent(TransportEventLogConstants.Tuple_SmtpReceiveCatchAll, null, new object[]
				{
					smtpInSession.clientIpAddress,
					ex
				});
				throw;
			}
		}

		private static void TlsNegotiationComplete(IAsyncResult asyncResult)
		{
			SmtpInSession smtpInSession = (SmtpInSession)asyncResult.AsyncState;
			smtpInSession.SetupPoisonContext();
			try
			{
				smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.TlsNegotiationComplete);
				object obj;
				smtpInSession.connection.EndNegotiateTlsAsServer(asyncResult, out obj);
				ExTraceGlobals.SmtpReceiveTracer.TraceDebug<long, object>((long)smtpInSession.GetHashCode(), "SmtpInSession(id={0})TlsNegotiationComplete, Status: {1}", smtpInSession.connectionId, obj ?? "OK");
				if (obj != null)
				{
					smtpInSession.smtpReceivePerfCountersInstance.TlsNegotiationsFailed.Increment();
					smtpInSession.logSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "TLS negotiation failed with error {0}", new object[]
					{
						obj
					});
					smtpInSession.SessionSource.DisconnectReason = DisconnectReason.DroppedSession;
					smtpInSession.HandleError(obj, false);
				}
				else
				{
					ConnectionInfo tlsConnectionInfo = smtpInSession.connection.TlsConnectionInfo;
					Util.LogTlsSuccessResult(smtpInSession.logSession, tlsConnectionInfo, smtpInSession.connection.RemoteCertificate);
					smtpInSession.TlsNegotiationComplete();
				}
			}
			catch (Exception ex)
			{
				smtpInSession.eventLogger.LogEvent(TransportEventLogConstants.Tuple_SmtpReceiveCatchAll, null, new object[]
				{
					smtpInSession.clientIpAddress,
					ex
				});
				if (smtpInSession.logSession != null)
				{
					smtpInSession.logSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "TLS negotiation failed, fatal exception");
				}
				throw;
			}
		}

		private void HandleErrorDuringBlindProxying(object error, SmtpInSession.BlindProxyContext blindProxyContext, bool clientError, bool readError)
		{
			this.DropBreadcrumb(SmtpInSessionBreadcrumbs.HandleErrorDuringBlindProxying);
			if (Interlocked.CompareExchange(ref blindProxyContext.BlindProxyWorkDone, 1, 0) == 1)
			{
				return;
			}
			ExTraceGlobals.SmtpReceiveTracer.TraceDebug<long, object, string>((long)this.GetHashCode(), "SmtpInSession(id={0}).HandleErrorDuringBlindProxying (Error={1}, ClientError={2}).", this.connectionId, error, clientError.ToString());
			if (clientError)
			{
				if (SmtpInSessionUtils.IsRemoteConnectionError(error))
				{
					this.disconnectReason = DisconnectReason.Remote;
					this.remoteConnectionError = error.ToString();
				}
			}
			else if (SmtpInSessionUtils.IsRemoteConnectionError(error))
			{
				this.proxyTargetDisconnectReason = DisconnectReason.Remote;
				this.proxyTargetRemoteConnectionError = error.ToString();
			}
			if (!clientError && readError)
			{
				this.connection.Shutdown(5);
			}
			else
			{
				this.connection.Shutdown();
			}
			blindProxyContext.ProxyConnection.Shutdown();
			SmtpProxyPerfCountersWrapper smtpProxyPerfCountersWrapper = this.SmtpProxyPerfCounters;
			if (smtpProxyPerfCountersWrapper != null)
			{
				smtpProxyPerfCountersWrapper.DecrementOutboundConnectionsCurrent();
			}
			this.Shutdown();
		}

		private void HandleError(object error, bool receiveError)
		{
			this.DropBreadcrumb(SmtpInSessionBreadcrumbs.HandleError);
			if (error is SocketError)
			{
				ExTraceGlobals.SmtpReceiveTracer.TraceDebug<long, object>((long)this.GetHashCode(), "SmtpInSession(id={0}).HandleError (socketError={1}) ", this.connectionId, error);
				switch ((SocketError)error)
				{
				case SocketError.Shutdown:
					this.Shutdown(DisconnectReason.Local);
					return;
				case SocketError.TimedOut:
					this.SessionSource.DisconnectReason = DisconnectReason.Timeout;
					if (receiveError)
					{
						this.connection.SendTimeout = 15;
						this.WriteLineWithLogThenShutdown(SmtpResponse.TimeoutOccurred);
						return;
					}
					this.Shutdown(DisconnectReason.Local);
					return;
				}
				this.remoteConnectionError = error.ToString();
				this.Shutdown(DisconnectReason.Remote);
				return;
			}
			if (error is SecurityStatus)
			{
				ExTraceGlobals.SmtpReceiveTracer.TraceDebug<long, object>((long)this.GetHashCode(), "SmtpInSession(id={0}).HandleError (SecurityStatus={1})", this.connectionId, error);
				this.SessionSource.DisconnectReason = DisconnectReason.DroppedSession;
				this.Shutdown(DisconnectReason.Local);
				return;
			}
			ExTraceGlobals.SmtpReceiveTracer.TraceDebug<long, object>((long)this.GetHashCode(), "SmtpInSession(id={0}).HandleError (error={1})", this.connectionId, error);
			this.SessionSource.DisconnectReason = DisconnectReason.Local;
			this.Shutdown(DisconnectReason.Local);
		}

		private void OnConnectCompleted(IAsyncResult ar)
		{
			this.DropBreadcrumb(SmtpInSessionBreadcrumbs.OnConnectCompleted);
			this.SetupPoisonContext();
			SmtpResponse response = this.AgentSession.EndRaiseEvent(ar);
			if (!response.IsEmpty)
			{
				this.WriteLineWithLogThenShutdown(response);
				return;
			}
			if (this.SessionSource.ShouldDisconnect)
			{
				this.Shutdown();
				return;
			}
			if (!this.clientIpData.Discredited || this.Connector.TarpitInterval.CompareTo(EnhancedTimeSpan.Zero) <= 0)
			{
				this.WriteBanner(null);
				return;
			}
			this.LogTarpitEvent(this.Connector.TarpitInterval, "IP discredited", null);
			this.delayResponseTimer = new GuardedTimer(new TimerCallback(this.WriteBanner), null, (int)this.Connector.TarpitInterval.TotalMilliseconds, -1);
		}

		private void WriteBanner(object obj)
		{
			this.clientIpData.MarkGood();
			this.WriteLineWithLog(this.Banner);
		}

		private void StartRead()
		{
			ExTraceGlobals.SmtpReceiveTracer.TraceDebug<long>((long)this.GetHashCode(), "SmtpInSession(id={0}). StartRead.", this.connectionId);
			this.DropBreadcrumb(SmtpInSessionBreadcrumbs.StartRead);
			if (this.rawDataHandler == null)
			{
				throw new InvalidOperationException("StartRead called without handler");
			}
			this.connection.BeginRead(SmtpInSession.readComplete, this);
		}

		private void StartReadFromProxyClient(SmtpInSession.BlindProxyContext blindProxyContext)
		{
			this.DropBreadcrumb(SmtpInSessionBreadcrumbs.StartReadFromProxyClient);
			this.connection.BeginRead(SmtpInSession.readCompleteFromProxyClient, blindProxyContext);
		}

		private void StartReadFromProxyTarget(SmtpInSession.BlindProxyContext blindProxyContext)
		{
			this.DropBreadcrumb(SmtpInSessionBreadcrumbs.StartReadFromProxyTarget);
			blindProxyContext.ProxyConnection.BeginRead(SmtpInSession.readCompleteFromProxyTarget, blindProxyContext);
		}

		private void StartWriteToProxyTarget(byte[] buffer, int offset, int size, SmtpInSession.BlindProxyContext blindProxyContext)
		{
			this.DropBreadcrumb(SmtpInSessionBreadcrumbs.StartWriteToProxyTarget);
			if (Interlocked.CompareExchange(ref blindProxyContext.IsProxyTargetWritePending, 1, 0) != 0)
			{
				throw new InvalidOperationException("A wite operation to the proxy target is already pending");
			}
			blindProxyContext.ProxyConnection.BeginWrite(buffer, offset, size, SmtpInSession.writeToProxyTargetCompleted, blindProxyContext);
		}

		private void StartWriteToProxyClient(byte[] buffer, int offset, int size, SmtpInSession.BlindProxyContext blindProxyContext)
		{
			this.DropBreadcrumb(SmtpInSessionBreadcrumbs.StartWriteToProxyClient);
			if (Interlocked.CompareExchange(ref blindProxyContext.IsProxyClientWritePending, 1, 0) != 0)
			{
				this.DropBreadcrumb(SmtpInSessionBreadcrumbs.WriteToProxyClientSkipped);
				return;
			}
			this.connection.BeginWrite(buffer, offset, size, SmtpInSession.writeToProxyClientCompleted, blindProxyContext);
		}

		private bool StartWriteQuitToProxyTargetIfNecessary(SmtpInSession.BlindProxyContext blindProxyContext)
		{
			if (Interlocked.CompareExchange(ref blindProxyContext.QuitCommandToTargetWriteOwner, 1, 0) != 0)
			{
				return false;
			}
			try
			{
				if (Util.InterlockedEquals(ref blindProxyContext.QuitCommandToTargetNeeded, 0))
				{
					return false;
				}
				this.StartWriteQuitToProxyTarget(blindProxyContext);
				Interlocked.Exchange(ref blindProxyContext.QuitCommandToTargetNeeded, 0);
			}
			finally
			{
				if (Interlocked.CompareExchange(ref blindProxyContext.QuitCommandToTargetWriteOwner, 0, 1) != 1)
				{
					throw new InvalidOperationException("Unexpected quitCommandToTargetWriteOwner value");
				}
			}
			return true;
		}

		private void StartWriteQuitToProxyTarget(SmtpInSession.BlindProxyContext blindProxyContext)
		{
			this.DropBreadcrumb(SmtpInSessionBreadcrumbs.StartWriteQuitToProxyTarget);
			if (Interlocked.CompareExchange(ref blindProxyContext.IsProxyTargetWritePending, 1, 0) != 0)
			{
				throw new InvalidOperationException("StartWriteQuitToProxyTarget: A wite operation to the proxy target is already pending");
			}
			blindProxyContext.ProxyConnection.BeginWrite(SmtpInSession.QuitCommand, 0, SmtpInSession.QuitCommand.Length, SmtpInSession.writeQuitToProxyTargetCompleted, blindProxyContext);
		}

		private bool ShouldInitializeMessageTrackingInfo()
		{
			return this.msgTrackInfo == null || (this.IsAnonymousClientProxiedSession && (!string.Equals(this.msgTrackInfo.ProxiedClientHostname, this.HelloDomain, StringComparison.OrdinalIgnoreCase) || !object.Equals(this.msgTrackInfo.ProxiedClientIPAddress, this.ProxiedClientAddress)));
		}

		private bool StartWriteXRsetResponseToProxyClientIfNecessary(SmtpInSession.BlindProxyContext blindProxyContext)
		{
			if (Interlocked.CompareExchange(ref blindProxyContext.XRsetProxyToResponseWriteOwner, 1, 0) != 0)
			{
				return false;
			}
			try
			{
				if (Util.InterlockedEquals(ref blindProxyContext.XRsetProxyToResponseNeeded, 0))
				{
					return false;
				}
				this.StartWriteXRsetResponseToProxyClient(blindProxyContext);
				Interlocked.Exchange(ref blindProxyContext.XRsetProxyToResponseNeeded, 0);
			}
			finally
			{
				if (Interlocked.CompareExchange(ref blindProxyContext.XRsetProxyToResponseWriteOwner, 0, 1) != 1)
				{
					throw new InvalidOperationException("Unexpected XRsetProxyToResponseWriteOwner value");
				}
			}
			return true;
		}

		private void StartWriteXRsetResponseToProxyClient(SmtpInSession.BlindProxyContext blindProxyContext)
		{
			this.DropBreadcrumb(SmtpInSessionBreadcrumbs.StartWriteXRsetResponseToProxyClient);
			if (Interlocked.CompareExchange(ref blindProxyContext.IsProxyClientWritePending, 1, 0) != 0)
			{
				this.DropBreadcrumb(SmtpInSessionBreadcrumbs.WriteXRsetResponseToProxyClientSkipped);
				return;
			}
			byte[] array = this.CreateXRsetProxyToAcceptedResponse().ToByteArray();
			this.connection.BeginWrite(array, 0, array.Length, SmtpInSession.writeXRsetResponseToProxyClientCompleted, blindProxyContext);
		}

		private void StartReadLine()
		{
			this.DropBreadcrumb(SmtpInSessionBreadcrumbs.StartReadLine);
			if (this.rawDataHandler != null)
			{
				throw new InvalidOperationException("StartReadLine called with an outstanding handler");
			}
			this.connection.BeginReadLine(SmtpInSession.readLineComplete, this);
		}

		private void WriteLineWithLog(SmtpResponse response)
		{
			this.WriteLineWithLog(response, SmtpInSession.beginReadLine);
		}

		private void WriteLineWithLogThenShutdown(SmtpResponse response)
		{
			this.WriteLineWithLog(response, SmtpInSession.toShutdown, null, true);
		}

		private void WriteLineWithLog(SmtpResponse response, AsyncCallback callback, object callbackContextParam, bool alwaysCall)
		{
			SmtpInSession.WriteCompleteLogCallbackParameters state = new SmtpInSession.WriteCompleteLogCallbackParameters(this, new List<SmtpResponse>(1)
			{
				response
			}, callback, callbackContextParam, alwaysCall);
			byte[] array = response.ToByteArray();
			this.connection.BeginWrite(array, 0, array.Length, SmtpInSession.writeCompleteLogCallback, state);
		}

		private void WriteLineWithLog(SmtpResponse response, AsyncCallback callback)
		{
			this.WriteLineWithLog(response, callback, null, false);
		}

		private void WriteSendBuffer(AsyncCallback callback, object callbackContextParam, bool alwaysCall)
		{
			BufferBuilder bufferBuilder = this.sendBuffer;
			this.sendBuffer = new BufferBuilder();
			SmtpInSession.WriteCompleteLogCallbackParameters state = new SmtpInSession.WriteCompleteLogCallbackParameters(this, this.responseList, callback, callbackContextParam, alwaysCall);
			this.responseList = null;
			this.connection.BeginWrite(bufferBuilder.GetBuffer(), 0, bufferBuilder.Length, SmtpInSession.writeCompleteLogCallback, state);
		}

		private void BufferResponse(SmtpResponse response)
		{
			if (this.responseList == null)
			{
				this.responseList = new List<SmtpResponse>();
			}
			this.responseList.Add(response);
			this.sendBuffer.Append(response.ToByteArray());
		}

		private bool ReceivedAndProcessedXRsetProxyToCommand(byte[] buffer, int offset, int size, SmtpInSession.BlindProxyContext blindProxyContext)
		{
			string text = "XRSETPROXYTO " + this.outboundProxySessionId;
			this.DropBreadcrumb(SmtpInSessionBreadcrumbs.ReceivedAndProcessedXRsetProxyToCommand);
			if (size == text.Length + " YYYXXX".Length + "\r\n".Length)
			{
				string @string = Encoding.ASCII.GetString(buffer, offset, size);
				int count;
				if (@string.StartsWith(text, StringComparison.OrdinalIgnoreCase) && @string.EndsWith("\r\n", StringComparison.OrdinalIgnoreCase) && int.TryParse(@string.Substring(text.Length + 1, 3), out count))
				{
					SmtpProxyPerfCountersWrapper smtpProxyPerfCountersWrapper = this.SmtpProxyPerfCounters;
					if (smtpProxyPerfCountersWrapper != null)
					{
						smtpProxyPerfCountersWrapper.IncrementMessagesProxiedTotalBy(count);
					}
					if (Interlocked.CompareExchange(ref blindProxyContext.BlindProxyWorkDone, 1, 0) != 1)
					{
						this.blindProxyingAuthenticatedUser = false;
						this.logSession.LogReceive(ByteString.StringToBytes(@string, true));
						if (Interlocked.CompareExchange(ref blindProxyContext.QuitCommandToTargetNeeded, 1, 0) != 0)
						{
							throw new InvalidOperationException("QuitCommandToTargetNeeded is already set");
						}
						if (Interlocked.CompareExchange(ref blindProxyContext.XRsetProxyToResponseNeeded, 1, 0) != 0)
						{
							throw new InvalidOperationException("XRsetProxyToResponseNeeded is already set");
						}
						if (Util.InterlockedEquals(ref blindProxyContext.IsProxyTargetWritePending, 0))
						{
							this.StartWriteQuitToProxyTargetIfNecessary(blindProxyContext);
						}
						if (Util.InterlockedEquals(ref blindProxyContext.IsProxyClientWritePending, 0))
						{
							this.StartWriteXRsetResponseToProxyClientIfNecessary(blindProxyContext);
						}
						return true;
					}
				}
			}
			return false;
		}

		private SmtpResponse CreateXRsetProxyToAcceptedResponse()
		{
			return new SmtpResponse("250", null, new string[]
			{
				"XRSETPROXYTO accepted; " + this.outboundProxySessionId
			});
		}

		private void WriteLog(SmtpResponse response)
		{
			if (response.StatusCode == "334")
			{
				this.logSession.LogSend(SmtpInSession.AuthLogLine);
				return;
			}
			if (response.StatusCode == "235" && this.sessionAuthMethod == MultilevelAuthMechanism.MUTUALGSSAPI)
			{
				this.logSession.LogSend(SmtpInSession.ExchangeAuthSuccessLine);
				return;
			}
			this.logSession.LogSend(response.ToByteArray());
			if (response.SmtpResponseType == SmtpResponseType.TransientError || response.SmtpResponseType == SmtpResponseType.PermanentError)
			{
				string text = null;
				if (this.transportMailItem != null)
				{
					text = this.transportMailItem.InternetMessageId;
				}
				if (!string.IsNullOrEmpty(text))
				{
					this.logSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "InternetMessageId: {0}", new object[]
					{
						text
					});
				}
			}
		}

		private void CreateSmtpCommand()
		{
			int currentOffset;
			switch (SmtpInSessionUtils.IdentifySmtpCommand(this.originalCommand, out currentOffset))
			{
			case SmtpInCommand.AUTH:
				this.commandHandler = new AuthSmtpCommand(this, false, this.transportConfiguration);
				break;
			case SmtpInCommand.BDAT:
				if (ConfigurationComponent.IsFrontEndTransportProcess(Components.Configuration))
				{
					this.commandHandler = new BdatInboundProxySmtpCommand(this, this.transportAppConfig);
					this.isFrontEndProxyingInbound = true;
				}
				else if (this.expectedBlobs != null && this.expectedBlobs.Count != 0)
				{
					this.commandHandler = new BdatSmtpCommand(this, this.transportAppConfig, this.expectedBlobs.Dequeue());
				}
				else
				{
					this.commandHandler = new BdatSmtpCommand(this, this.transportAppConfig, null);
				}
				break;
			case SmtpInCommand.DATA:
				if (ConfigurationComponent.IsFrontEndTransportProcess(Components.Configuration))
				{
					this.commandHandler = new DataInboundProxySmtpCommand(this, this.transportAppConfig);
					this.isFrontEndProxyingInbound = true;
				}
				else
				{
					this.commandHandler = new DataSmtpCommand(this, this.transportAppConfig);
				}
				break;
			case SmtpInCommand.EHLO:
				this.commandHandler = new EHLOSmtpCommand(this, this.transportConfiguration);
				break;
			case SmtpInCommand.EXPN:
				this.commandHandler = new UnknownSmtpCommand(this, "expn", true);
				break;
			case SmtpInCommand.HELO:
				this.commandHandler = new HELOSmtpCommand(this);
				break;
			case SmtpInCommand.HELP:
				this.commandHandler = new HelpSmtpCommand(this);
				break;
			case SmtpInCommand.MAIL:
				this.commandHandler = new MailSmtpCommand(this, this.transportAppConfig);
				break;
			case SmtpInCommand.NOOP:
				this.commandHandler = new NoopSmtpCommand(this);
				break;
			case SmtpInCommand.QUIT:
				this.commandHandler = new QuitSmtpCommand(this);
				break;
			case SmtpInCommand.RCPT:
				this.commandHandler = new RcptSmtpCommand(this, this.recipientCorrelator, this.transportAppConfig);
				break;
			case SmtpInCommand.RSET:
				this.commandHandler = new RsetSmtpCommand(this);
				break;
			case SmtpInCommand.STARTTLS:
				this.commandHandler = new StarttlsSmtpCommand(this, false);
				break;
			case SmtpInCommand.VRFY:
				this.commandHandler = new UnknownSmtpCommand(this, "vrfy", true);
				break;
			case SmtpInCommand.XANONYMOUSTLS:
				this.commandHandler = new StarttlsSmtpCommand(this, true);
				break;
			case SmtpInCommand.XEXCH50:
				this.commandHandler = new Xexch50SmtpCommand(this, this.recipientCorrelator, this.mailRouter, this.transportAppConfig, this.transportConfiguration);
				break;
			case SmtpInCommand.XEXPS:
				this.commandHandler = new AuthSmtpCommand(this, true, this.transportConfiguration);
				break;
			case SmtpInCommand.XPROXY:
				this.commandHandler = new XProxySmtpCommand(this, this.transportConfiguration, this.transportAppConfig);
				break;
			case SmtpInCommand.XPROXYFROM:
				this.commandHandler = new XProxyFromSmtpCommand(this, this.transportConfiguration, this.transportAppConfig);
				break;
			case SmtpInCommand.XPROXYTO:
				this.commandHandler = new XProxyToSmtpCommand(this, this.transportConfiguration, this.transportAppConfig);
				break;
			case SmtpInCommand.XQDISCARD:
				this.commandHandler = new XQDiscardSmtpCommand(this, this.shadowRedundancyManager);
				break;
			case SmtpInCommand.XSESSIONPARAMS:
				this.commandHandler = new XSessionParamsSmtpCommand(this);
				break;
			case SmtpInCommand.XSHADOW:
				this.commandHandler = new XShadowSmtpCommand(this, this.shadowRedundancyManager);
				break;
			case SmtpInCommand.XSHADOWREQUEST:
				this.commandHandler = new XShadowRequestSmtpCommand(this, this.shadowRedundancyManager);
				break;
			case SmtpInCommand.RCPT2:
				this.commandHandler = new Rcpt2SmtpCommand(this);
				break;
			default:
				ExTraceGlobals.SmtpReceiveTracer.TraceError<byte[]>((long)this.GetHashCode(), "Received an unexpected command : {0}", this.originalCommand);
				this.commandHandler = new UnknownSmtpCommand(this, "unknown", false);
				break;
			}
			this.commandHandler.ProtocolCommand = this.originalCommand;
			this.commandHandler.CurrentOffset = currentOffset;
		}

		private void DisposeSmtpCommand()
		{
			if (this.commandHandler != null)
			{
				this.commandHandler.Dispose();
				this.commandHandler = null;
			}
		}

		private void LogTarpitEvent(TimeSpan tarpitInterval, string tarpitReason, string tarpitContext)
		{
			if (string.IsNullOrEmpty(tarpitReason))
			{
				byte[] data = Util.AsciiStringToBytes(string.Format(CultureInfo.InvariantCulture, "Tarpit for '{0}'", new object[]
				{
					SmtpInSessionUtils.FormatTimeSpan(tarpitInterval)
				}));
				this.LogInformation(ProtocolLoggingLevel.Verbose, tarpitContext, data);
				return;
			}
			byte[] data2 = Util.AsciiStringToBytes(string.Format(CultureInfo.InvariantCulture, "Tarpit for '{0}' due to '{1}'", new object[]
			{
				SmtpInSessionUtils.FormatTimeSpan(tarpitInterval),
				tarpitReason
			}));
			this.LogInformation(ProtocolLoggingLevel.Verbose, tarpitContext, data2);
		}

		private bool LoadCertificates()
		{
			DateTime utcNow = DateTime.UtcNow;
			ExEventLogWrapper eventLog = new ExEventLogWrapper(this.eventLogger);
			IX509Certificate2 ix509Certificate;
			bool flag = Util.LoadDirectTrustCertificate(this.connector, this.connectionId, this.SmtpInServer.ServerConfiguration.InternalTransportCertificateThumbprint, utcNow, this.server.CertificateCache, eventLog, ExTraceGlobals.SmtpReceiveTracer, out ix509Certificate);
			this.InternalTransportCertificate = ((ix509Certificate == null) ? null : ix509Certificate.Certificate);
			IX509Certificate2 ix509Certificate2;
			flag &= Util.LoadStartTlsCertificate(this.connector, this.ehloOptions.AdvertisedFQDN, this.connectionId, this.transportAppConfig.SmtpReceiveConfiguration.OneLevelWildcardMatchForCertSelection, utcNow, this.server.CertificateCache, eventLog, ExTraceGlobals.SmtpReceiveTracer, out ix509Certificate2);
			this.AdvertisedTlsCertificate = ((ix509Certificate2 == null) ? null : ix509Certificate2.Certificate);
			return flag;
		}

		private void Shutdown(DisconnectReason disconnectReason)
		{
			this.DropBreadcrumb(SmtpInSessionBreadcrumbs.ShutdownWithArg);
			ExTraceGlobals.SmtpReceiveTracer.TraceDebug<long>((long)this.GetHashCode(), "SmtpInSession(id={0}).Shutdown", this.connectionId);
			this.disconnectReason = disconnectReason;
			if (!this.certificatesLoadedSuccessfully && this.isLastCommandEhloBeforeQuit && (disconnectReason == DisconnectReason.Remote || disconnectReason == DisconnectReason.QuitVerb))
			{
				this.UpdateSmtpAvailabilityPerfCounter(LegitimateSmtpAvailabilityCategory.RejectDueToTLSError);
			}
			this.Shutdown();
		}

		private void ShutdownCompletedFromMEx(IAsyncResult ar)
		{
			this.DropBreadcrumb(SmtpInSessionBreadcrumbs.ShutdownCompletedFromMEx);
			this.SetupPoisonContext();
			this.AgentSession.EndRaiseEvent(ar);
			this.ShutdownCompleted(null);
		}

		private void ShutdownCompleted(IAsyncResult ar)
		{
			this.DropBreadcrumb(SmtpInSessionBreadcrumbs.ShutdownCompleted);
			ExTraceGlobals.SmtpReceiveTracer.TraceDebug<long>((long)this.GetHashCode(), "SmtpInSession(id={0}).ShutdownCompleted (disconnect)", this.connectionId);
			if (this.disconnectReason == DisconnectReason.Remote)
			{
				this.logSession.LogDisconnect(this.disconnectReason, this.remoteConnectionError);
			}
			else
			{
				this.logSession.LogDisconnect(this.disconnectReason);
			}
			this.AgentSession.Close();
			if (this.shadowSession != null)
			{
				this.shadowSession.Close(AckStatus.Fail, SmtpResponse.Empty);
			}
			this.DeleteTransportMailItem();
			this.server.RemoveConnection(this.connectionId);
			this.connection.Dispose();
			SmtpInSession.BlindProxyContext blindProxyContext = this.blindProxyContext;
			if (blindProxyContext != null)
			{
				blindProxyContext.ProxyConnection.Dispose();
				IProtocolLogSession blindProxySendLogSession = blindProxyContext.BlindProxySendLogSession;
				if (blindProxySendLogSession != null)
				{
					blindProxySendLogSession.LogDisconnect(this.proxyTargetDisconnectReason);
					string description = string.Empty;
					if (!string.IsNullOrEmpty(this.proxyTargetRemoteConnectionError))
					{
						description = string.Format("Remote error from proxy target - {0}", this.proxyTargetRemoteConnectionError);
					}
					else if (!string.IsNullOrEmpty(this.remoteConnectionError))
					{
						description = string.Format("Remote error from proxy client", new object[0]);
					}
					ConnectionLog.SmtpConnectionStop(blindProxyContext.BlindProxySendSessionId, string.Empty, description, 0UL, 0UL, 0UL);
				}
				this.blindProxyContext = null;
			}
			if (this.ProxyPassword != null)
			{
				this.ProxyPassword.Dispose();
			}
			this.RemoveClientIpConnection();
			ISmtpReceivePerfCounters smtpReceivePerformanceCounters = this.SmtpReceivePerformanceCounters;
			if (smtpReceivePerformanceCounters != null)
			{
				smtpReceivePerformanceCounters.ConnectionsCurrent.Decrement();
				if (this.IsTls)
				{
					smtpReceivePerformanceCounters.TlsConnectionsCurrent.Decrement();
				}
				if (this.isFrontEndProxyingInbound)
				{
					smtpReceivePerformanceCounters.InboundMessageConnectionsCurrent.Decrement();
				}
			}
			SmtpProxyPerfCountersWrapper smtpProxyPerfCountersWrapper = this.smtpProxyPerfCounters;
			if (smtpProxyPerfCountersWrapper != null)
			{
				smtpProxyPerfCountersWrapper.DecrementInboundConnectionsCurrent();
			}
			if (this.SmtpInServer.OutboundProxyBySourceTracker != null && !string.IsNullOrWhiteSpace(this.HelloSmtpDomain))
			{
				this.SmtpInServer.OutboundProxyBySourceTracker.DecrementProxyCount(this.HelloSmtpDomain);
			}
			this.server = null;
			this.connector = null;
			this.originalCommand = null;
			this.logSession = null;
			this.ehloOptions = null;
			this.msgTrackInfo = null;
			this.DisposeSmtpCommand();
			if (this.bdatState != null && this.bdatState.ProxyLayer != null)
			{
				this.bdatState.ProxyLayer.NotifySmtpInStopProxy();
				this.bdatState.ProxyLayer = null;
			}
			this.bdatState = null;
			if (this.delayResponseTimer != null)
			{
				this.delayResponseTimer.Dispose(false);
				this.delayResponseTimer = null;
			}
			this.smtpReceivePerfCountersInstance = null;
			this.smtpAvailabilityPerfCounters = null;
			this.smtpProxyPerfCounters = null;
			if (this.remoteWindowsIdentity != null)
			{
				this.remoteWindowsIdentity.Dispose();
				this.remoteWindowsIdentity = null;
			}
		}

		private AsyncReturnType StartProcessingCommand(byte[] inputBuffer, int offset, int size, bool overflow)
		{
			this.DropBreadcrumb(SmtpInSessionBreadcrumbs.StartProcessingCommand);
			BufferBuilder bufferBuilder = this.commandBuffer ?? new BufferBuilder(size);
			if (bufferBuilder.Length + size > 32768)
			{
				ExTraceGlobals.SmtpReceiveTracer.TraceDebug<long>((long)this.GetHashCode(), "SmtpInSession(id={0}) disconnected: Command too long", this.connectionId);
				this.WriteLineWithLogThenShutdown(SmtpResponse.CommandTooLong);
				return AsyncReturnType.Async;
			}
			bufferBuilder.Append(inputBuffer, offset, size);
			if (overflow)
			{
				this.commandBuffer = bufferBuilder;
				return AsyncReturnType.Sync;
			}
			this.commandBuffer = null;
			bufferBuilder.RemoveUnusedBufferSpace();
			if (this.commandHandler == null)
			{
				ExTraceGlobals.SmtpReceiveTracer.TraceDebug<long, BufferBuilder>((long)this.GetHashCode(), "SmtpInSession(id={0}) cmd: {1}", this.connectionId, bufferBuilder);
				this.originalCommand = bufferBuilder.GetBuffer();
				this.CreateSmtpCommand();
				if (!(this.commandHandler is AuthSmtpCommand) && !(this.commandHandler is MailSmtpCommand) && !(this.commandHandler is RcptSmtpCommand) && !(this.commandHandler is XProxySmtpCommand))
				{
					this.logSession.LogReceive(bufferBuilder.GetBuffer());
				}
			}
			else
			{
				this.commandHandler.ProtocolCommand = bufferBuilder.GetBuffer();
				this.commandHandler.CurrentOffset = 0;
			}
			this.commandHandler.InboundParseCommand();
			if (this.commandHandler.IsResponseReady && (this.commandHandler.ParsingStatus == ParsingStatus.ProtocolError || this.commandHandler.ParsingStatus == ParsingStatus.Error || this.commandHandler.ParsingStatus == ParsingStatus.IgnorableProtocolError))
			{
				IAsyncResult asyncResult = this.RaiseOnRejectEvent(this.commandHandler.ProtocolCommand, null, this.commandHandler.SmtpResponse, new AsyncCallback(this.OnRejectCallback));
				if (!asyncResult.CompletedSynchronously)
				{
					return AsyncReturnType.Async;
				}
				return this.ContinueOnReject(asyncResult, false);
			}
			else
			{
				this.DropBreadcrumb(SmtpInSessionBreadcrumbs.PostParseCommand);
				if (this.commandHandler.CommandEventComponent != LatencyComponent.None && this.transportMailItem != null)
				{
					this.AgentLatencyTracker.BeginTrackLatency(this.commandHandler.CommandEventComponent, this.transportMailItem.LatencyTracker);
				}
				IAsyncResult asyncResult2 = this.commandHandler.BeginRaiseEvent(new AsyncCallback(this.PostParseCommandCompleted), null);
				if (!asyncResult2.CompletedSynchronously)
				{
					return AsyncReturnType.Async;
				}
				return this.ContinuePostParseCommand(asyncResult2, false);
			}
		}

		private void PostParseCommandCompleted(IAsyncResult ar)
		{
			this.DropBreadcrumb(SmtpInSessionBreadcrumbs.PostParseCommandCompleted);
			this.SetupPoisonContext();
			if (!ar.CompletedSynchronously)
			{
				this.ContinuePostParseCommand(ar, true);
			}
		}

		private AsyncReturnType ContinuePostParseCommand(IAsyncResult ar, bool isAsync)
		{
			this.DropBreadcrumb(SmtpInSessionBreadcrumbs.ContinuePostParseCommand);
			ExTraceGlobals.SmtpReceiveTracer.TraceDebug<long>((long)this.GetHashCode(), "SmtpInSession(id={0}).ContinuePostParseCommand", this.connectionId);
			SmtpResponse smtpResponse = this.AgentSession.EndRaiseEvent(ar);
			if (this.commandHandler.CommandEventComponent != LatencyComponent.None && this.transportMailItem != null)
			{
				this.AgentLatencyTracker.EndTrackLatency();
			}
			this.commandHandler.InboundAgentEventCompleted();
			if (!smtpResponse.IsEmpty || this.SessionSource.ShouldDisconnect)
			{
				this.DropBreadcrumb(SmtpInSessionBreadcrumbs.DisconnectFromContinuePostParseCommand);
				SmtpResponse response = smtpResponse.IsEmpty ? this.commandHandler.SmtpResponse : smtpResponse;
				if (!response.IsEmpty)
				{
					this.WriteLineWithLogThenShutdown(response);
				}
				else
				{
					this.Shutdown();
				}
				return AsyncReturnType.Async;
			}
			if (this.SessionSource.SmtpResponse.Equals(SmtpResponse.Empty))
			{
				return this.ProcessCommand(isAsync);
			}
			this.commandHandler.SmtpResponse = this.SessionSource.SmtpResponse;
			this.SessionSource.SmtpResponse = SmtpResponse.Empty;
			byte[] command = null;
			if (this.commandHandler.OriginalEventArgsWrapper is ReceiveCommandEventArgs)
			{
				command = this.commandHandler.ProtocolCommand;
			}
			IAsyncResult asyncResult = this.RaiseOnRejectEvent(command, this.commandHandler.OriginalEventArgsWrapper, this.commandHandler.SmtpResponse, new AsyncCallback(this.OnRejectCallback));
			if (!asyncResult.CompletedSynchronously)
			{
				return AsyncReturnType.Async;
			}
			return this.ContinueOnReject(asyncResult, isAsync);
		}

		private void OnRejectCallback(IAsyncResult ar)
		{
			this.DropBreadcrumb(SmtpInSessionBreadcrumbs.OnRejectCallback);
			this.SetupPoisonContext();
			if (!ar.CompletedSynchronously)
			{
				this.ContinueOnReject(ar, true);
			}
		}

		private AsyncReturnType ContinueOnReject(IAsyncResult ar, bool isAsync)
		{
			this.DropBreadcrumb(SmtpInSessionBreadcrumbs.ContinueOnReject);
			ExTraceGlobals.SmtpReceiveTracer.TraceDebug<long>((long)this.GetHashCode(), "SmtpInSession(id={0}).OnRejectcompleted", this.connectionId);
			if (this.commandHandler.ParsingStatus == ParsingStatus.MoreDataRequired)
			{
				this.commandHandler.ParsingStatus = ParsingStatus.Complete;
			}
			SmtpResponse response = this.AgentSession.EndRaiseEvent(ar);
			if (!response.IsEmpty)
			{
				this.DropBreadcrumb(SmtpInSessionBreadcrumbs.DisconnectFromContinueOnReject);
				this.WriteLineWithLogThenShutdown(response);
				return AsyncReturnType.Async;
			}
			return this.DelayResponseIfNecessary(isAsync);
		}

		private AsyncReturnType ProcessCommand(bool isAsync)
		{
			this.DropBreadcrumb(SmtpInSessionBreadcrumbs.ProcessCommand);
			if (!(this.commandHandler is QuitSmtpCommand))
			{
				this.isLastCommandEhloBeforeQuit = false;
				this.isLastCommandAuthBeforeQuit = false;
			}
			else if (this.isLastCommandAuthBeforeQuit && this.AuthMethod == MultilevelAuthMechanism.MUTUALGSSAPI)
			{
				AuthCommandHelpers.TryFlushKerberosTicketCache(this.transportConfiguration.AppConfig.SmtpAvailabilityConfiguration.KerberosTicketCacheFlushMinInterval, this.LogSession);
			}
			if (this.commandHandler is AuthSmtpCommand)
			{
				this.isLastCommandAuthBeforeQuit = true;
			}
			this.commandHandler.InboundProcessCommand();
			if (this.StartClientProxySession)
			{
				this.StartClientProxySession = false;
				this.UpdateSmtpAvailabilityPerfCounter(LegitimateSmtpAvailabilityCategory.SuccessfulSubmission);
				this.smtpProxyPerfCounters = this.SmtpInServer.ClientProxyPerfCounters;
				if (!this.usedForBlindProxy)
				{
					this.usedForBlindProxy = true;
					this.smtpProxyPerfCounters.IncrementInboundConnectionsCurrent();
				}
				this.proxySetupHandler = new ProxySessionSetupHandler(this, this.enhancedDns, this.transportConfiguration);
				this.proxySetupHandler.BeginSettingUpProxySession();
				return AsyncReturnType.Async;
			}
			if (this.startOutboundProxySession)
			{
				this.startOutboundProxySession = false;
				this.UpdateSmtpAvailabilityPerfCounter(LegitimateSmtpAvailabilityCategory.SuccessfulSubmission);
				this.smtpProxyPerfCounters = this.SmtpInServer.OutboundProxyPerfCounters;
				if (!this.usedForBlindProxy)
				{
					this.usedForBlindProxy = true;
					this.smtpProxyPerfCounters.IncrementInboundConnectionsCurrent();
					if (!string.IsNullOrWhiteSpace(this.HelloSmtpDomain))
					{
						this.SmtpInServer.OutboundProxyBySourceTracker.IncrementProxyCount(this.HelloSmtpDomain);
					}
				}
				this.proxySetupHandler = new ProxySessionSetupHandler(this, this.enhancedDns, this.transportConfiguration, this.outboundProxyDestinations, this.outboundProxySendConnector, this.outboundProxyTlsSendConfiguration, this.outboundProxyRiskLevel, this.outboundProxyOutboundIPPool, this.outboundProxyNextHopDomain, this.outboundProxySessionId);
				this.proxySetupHandler.BeginSettingUpProxySession();
				return AsyncReturnType.Async;
			}
			return this.ContinueProcessCommand(isAsync);
		}

		private AsyncReturnType ContinueProcessCommand(bool isAsync)
		{
			this.DropBreadcrumb(SmtpInSessionBreadcrumbs.ContinueProcessCommand);
			if (this.commandHandler.IsResponseReady && !this.commandHandler.SmtpResponse.Equals(SmtpResponse.Empty) && (this.commandHandler.SmtpResponse.StatusCode[0] == '5' || this.commandHandler.SmtpResponse.StatusCode[0] == '4'))
			{
				IAsyncResult asyncResult = this.RaiseOnRejectEvent(this.commandHandler.ProtocolCommand, null, this.commandHandler.SmtpResponse, new AsyncCallback(this.OnRejectCallback));
				if (!asyncResult.CompletedSynchronously)
				{
					return AsyncReturnType.Async;
				}
				return this.ContinueOnReject(asyncResult, isAsync);
			}
			else
			{
				BdatSmtpCommand bdatSmtpCommand = this.commandHandler as BdatSmtpCommand;
				BdatInboundProxySmtpCommand bdatInboundProxySmtpCommand = this.commandHandler as BdatInboundProxySmtpCommand;
				if ((bdatSmtpCommand != null && bdatSmtpCommand.IsBdat0Last) || (bdatInboundProxySmtpCommand != null && bdatInboundProxySmtpCommand.IsBdat0Last))
				{
					if (this.rawDataHandler(SmtpInSession.EmptyBuffer, 0, 0) == AsyncReturnType.Sync)
					{
						this.RawDataReceivedCompleted();
					}
					return AsyncReturnType.Async;
				}
				return this.DelayResponseIfNecessary(isAsync);
			}
		}

		private AsyncReturnType DelayResponseIfNecessary(bool isAsync)
		{
			this.DropBreadcrumb(SmtpInSessionBreadcrumbs.DelayResponseIfNecessary);
			ParsingStatus parsingStatus = this.commandHandler.ParsingStatus;
			if (parsingStatus == ParsingStatus.ProtocolError)
			{
				this.protocolErrors++;
				if (SmtpInSessionUtils.IsMaxProtocolErrorsExceeded(this.protocolErrors, this.Connector))
				{
					this.commandHandler.SmtpResponse = SmtpResponse.TooManyProtocolErrors;
					this.Disconnect(DisconnectReason.TooManyErrors);
				}
			}
			if (parsingStatus != ParsingStatus.MoreDataRequired)
			{
				this.commandHandler.InboundCompleteCommand();
			}
			TarpitAction tarpitAction;
			TimeSpan timeSpan;
			string text;
			string tarpitContext;
			if (this.commandHandler.IsResponseReady)
			{
				tarpitAction = (SmtpInSessionUtils.IsTarpitAuthenticationLevelHigh(this.Permissions, this.RemoteIdentity, this.SmtpInServer.Configuration.AppConfig.SmtpReceiveConfiguration.TarpitMuaSubmission) ? this.commandHandler.HighAuthenticationLevelTarpitOverride : this.commandHandler.LowAuthenticationLevelTarpitOverride);
				timeSpan = this.commandHandler.TarpitInterval;
				text = this.commandHandler.TarpitReason;
				tarpitContext = this.commandHandler.TarpitContext;
				this.protocolResponse = this.commandHandler.SmtpResponse;
				this.isResponseBuffered = this.commandHandler.IsResponseBuffered;
				this.commandHandler.LowAuthenticationLevelTarpitOverride = TarpitAction.None;
				this.commandHandler.HighAuthenticationLevelTarpitOverride = TarpitAction.None;
				this.commandHandler.SmtpResponse = SmtpResponse.Empty;
			}
			else
			{
				tarpitAction = TarpitAction.None;
				timeSpan = TimeSpan.Zero;
				text = string.Empty;
				tarpitContext = null;
			}
			if (parsingStatus != ParsingStatus.MoreDataRequired)
			{
				this.DisposeSmtpCommand();
			}
			if (!this.protocolResponse.Equals(SmtpResponse.Empty) && parsingStatus != ParsingStatus.MoreDataRequired)
			{
				if (this.delayedAckStatus == SmtpInSession.DelayedAckStatus.ShadowRedundancyManagerNotified)
				{
					ExTraceGlobals.SmtpReceiveTracer.TraceDebug<long>((long)this.GetHashCode(), "SmtpInSession(id={0}).DelayResponseIfNecessary: Waiting for notification from ShadowRedundancyManager relating to delayed ack message", this.connectionId);
					this.DropBreadcrumb(SmtpInSessionBreadcrumbs.DelayedAckStarted);
					this.delayedAckStatus = SmtpInSession.DelayedAckStatus.WaitingForShadowRedundancyManager;
					return AsyncReturnType.Async;
				}
				bool flag = false;
				if (!SmtpInSessionUtils.IsTarpitAuthenticationLevelHigh(this.Permissions, this.RemoteIdentity, this.SmtpInServer.Configuration.AppConfig.SmtpReceiveConfiguration.TarpitMuaSubmission) && (this.protocolResponse.SmtpResponseType == SmtpResponseType.PermanentError || parsingStatus == ParsingStatus.Error || parsingStatus == ParsingStatus.ProtocolError))
				{
					if (!this.IsAnonymousClientProxiedSession && this.InboundClientProxyState == InboundClientProxyStates.None)
					{
						this.clientIpData.MarkBad();
					}
					flag = true;
					if (string.IsNullOrEmpty(text))
					{
						text = this.protocolResponse.ToString();
					}
				}
				if (tarpitAction == TarpitAction.DoTarpit)
				{
					flag = true;
				}
				else if (tarpitAction == TarpitAction.DoNotTarpit)
				{
					flag = false;
				}
				if (flag && timeSpan > TimeSpan.Zero)
				{
					ExTraceGlobals.SmtpReceiveTracer.TraceDebug<long, double, string>((long)this.GetHashCode(), "SmtpInSession(id={0}).DelayResponseIfNecessary: Delay ({1} msec.) the response due to '{0}'.", this.connectionId, timeSpan.TotalMilliseconds, text);
					ISmtpReceivePerfCounters smtpReceivePerformanceCounters = this.SmtpReceivePerformanceCounters;
					if (smtpReceivePerformanceCounters != null)
					{
						if (SmtpInSessionUtils.IsAnonymous(this.RemoteIdentity))
						{
							smtpReceivePerformanceCounters.TarpittingDelaysAnonymous.Increment();
						}
						else
						{
							smtpReceivePerformanceCounters.TarpittingDelaysAuthenticated.Increment();
						}
						if ("Back Pressure".Equals(text))
						{
							smtpReceivePerformanceCounters.TarpittingDelaysBackpressure.Increment();
						}
					}
					this.LogTarpitEvent(timeSpan, text, tarpitContext);
					this.delayResponseTimer = new GuardedTimer(new TimerCallback(this.DelayResponseCompleted), null, (int)timeSpan.TotalMilliseconds, -1);
					return AsyncReturnType.Async;
				}
			}
			return this.EndProcessingCommand(isAsync);
		}

		private void DelayResponseCompleted(object state)
		{
			this.DropBreadcrumb(SmtpInSessionBreadcrumbs.DelayResponseCompleted);
			ExTraceGlobals.SmtpReceiveTracer.TraceDebug<long>((long)this.GetHashCode(), "SmtpInSession(id={0}).DelayResponseCompleted", this.connectionId);
			if (this.delayedAckStatus == SmtpInSession.DelayedAckStatus.WaitingForShadowRedundancyManager)
			{
				this.delayedAckStatus = SmtpInSession.DelayedAckStatus.None;
			}
			else
			{
				if (this.delayResponseTimer != null)
				{
					this.delayResponseTimer.Dispose(false);
					this.delayResponseTimer = null;
				}
				this.clientIpData.MarkGood();
			}
			this.EndProcessingCommand(true);
		}

		private AsyncReturnType EndProcessingCommand(bool isAsync)
		{
			this.DropBreadcrumb(SmtpInSessionBreadcrumbs.EndProcessingCommand);
			if (this.SessionSource.ShouldDisconnect)
			{
				this.DropBreadcrumb(SmtpInSessionBreadcrumbs.DisconnectFromEndProcessingCommand);
				ExTraceGlobals.SmtpReceiveTracer.TraceDebug<long>((long)this.GetHashCode(), "SmtpInSession(id={0}).EndProcessingCommand calling Shutdown", this.connectionId);
				if (this.disconnectByServer && !this.protocolResponse.Equals(SmtpResponse.Empty))
				{
					if (this.sendBuffer.Length > 0)
					{
						this.BufferResponse(this.protocolResponse);
						this.WriteSendBuffer(SmtpInSession.toShutdown, null, true);
					}
					else
					{
						this.WriteLineWithLogThenShutdown(this.protocolResponse);
					}
				}
				else
				{
					this.Shutdown();
				}
				return AsyncReturnType.Async;
			}
			bool flag = (byte)(this.secureState & SecureState.NegotiationRequested) == 128;
			object callbackContextParam = null;
			AsyncCallback callback;
			if (flag)
			{
				callback = SmtpInSession.startTlsNegotiation;
			}
			else if (this.rawDataHandler != null)
			{
				callback = SmtpInSession.beginRead;
			}
			else
			{
				callback = SmtpInSession.beginReadLine;
			}
			SmtpInSession.BlindProxyContext blindProxyContext = this.blindProxyContext;
			if (blindProxyContext != null && Util.InterlockedEquals(ref blindProxyContext.BlindProxyWorkDone, 0))
			{
				this.LogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "Proxy session was successfully set up. {0} will now be proxied", new object[]
				{
					(this.ProxyUserName != null) ? ("Session for" + Util.Redact(this.ProxyUserName)) : "Outbound session"
				});
				callback = new AsyncCallback(SmtpInSession.StartProxying);
				callbackContextParam = blindProxyContext;
			}
			if (!this.protocolResponse.Equals(SmtpResponse.Empty))
			{
				SmtpResponse response = this.protocolResponse;
				this.protocolResponse = SmtpResponse.Empty;
				if (isAsync || !this.SeenEhlo || !this.isResponseBuffered || response.StatusCode[0] != '2' || !this.connection.IsLineAvailable || flag)
				{
					this.DropBreadcrumb(SmtpInSessionBreadcrumbs.EndProcessingCommandWriteResponse);
					isAsync = true;
					if (this.sendBuffer.Length > 0)
					{
						this.BufferResponse(response);
						this.WriteSendBuffer(callback, callbackContextParam, false);
					}
					else
					{
						this.WriteLineWithLog(response, callback, callbackContextParam, false);
					}
				}
				else
				{
					this.DropBreadcrumb(SmtpInSessionBreadcrumbs.EndProcessingCommandWriteResponseToBuffer);
					this.BufferResponse(response);
				}
			}
			else if (isAsync)
			{
				if (this.rawDataHandler == null)
				{
					this.StartReadLine();
				}
				else
				{
					this.StartRead();
				}
			}
			if (!isAsync)
			{
				return AsyncReturnType.Sync;
			}
			return AsyncReturnType.Async;
		}

		private void TlsNegotiationComplete()
		{
			if (this.connection.RemoteCertificate != null)
			{
				ExTraceGlobals.SmtpReceiveTracer.TraceDebug<string>((long)this.GetHashCode(), "Remote has supplied client certificate {0}", this.connection.RemoteCertificate.Subject);
				this.TlsRemoteCertificate = this.connection.RemoteCertificate.Certificate;
				if (this.secureState == SecureState.AnonymousTls)
				{
					this.RemoteIdentity = DirectTrust.MapCertToSecurityIdentifier(this.connection.RemoteCertificate.Certificate);
					if (this.RemoteIdentity != SmtpConstants.AnonymousSecurityIdentifier)
					{
						this.RemoteIdentityName = this.connection.RemoteCertificate.Subject;
						this.AuthMethod = MultilevelAuthMechanism.DirectTrustTLS;
						ExTraceGlobals.SmtpReceiveTracer.TraceDebug<string>((long)this.GetHashCode(), "DirectTrust certificate authenticated as {0}", this.RemoteIdentityName);
						CertificateExpiryCheck.CheckCertificateExpiry(this.connection.RemoteCertificate.Certificate, this.eventLogger, (this.secureState == SecureState.StartTls) ? SmtpSessionCertificateUse.RemoteSTARTTLS : SmtpSessionCertificateUse.RemoteDirectTrust, this.connection.RemoteCertificate.Subject);
					}
					else
					{
						this.RemoteIdentityName = "anonymous";
						ExTraceGlobals.SmtpReceiveTracer.TraceError<string>((long)this.GetHashCode(), "DirectTrust certificate failed to authenticate for {0}", this.TlsRemoteCertificate.Subject);
						this.LogInformation(ProtocolLoggingLevel.Verbose, "DirectTrust certificate failed to authenticate for " + this.connection.RemoteCertificate.Subject, null);
						this.eventLogger.LogEvent(TransportEventLogConstants.Tuple_SmtpReceiveDirectTrustFailed, this.RemoteEndPoint.Address.ToString(), new object[]
						{
							this.TlsRemoteCertificate.Subject,
							this.RemoteEndPoint.Address
						});
					}
				}
				this.SetSessionPermissions(this.RemoteIdentity);
			}
			if (this.IsTls)
			{
				this.SmtpReceivePerformanceCounters.TlsConnectionsCurrent.Increment();
			}
			this.ehloOptions.StartTLS = false;
			this.ehloOptions.AnonymousTLS = false;
			this.StartReadLine();
		}

		private void ApplySupportIntegratedAuthOverride(bool isFrontEndTransportProcess)
		{
			this.supportIntegratedAuth = SmtpInSessionUtils.ShouldSupportIntegratedAuthentication(this.supportIntegratedAuth, isFrontEndTransportProcess);
		}

		private Permission DetermineAnonymousPermissions()
		{
			if (this.anonymousPermissions == null)
			{
				this.anonymousPermissions = new Permission?(Util.GetPermissionsForSid(SmtpConstants.AnonymousSecurityIdentifier, this.connector.GetSecurityDescriptor(), this.authzAuthorization, "anonymous", this.connector.Name, ExTraceGlobals.SmtpReceiveTracer));
			}
			return this.anonymousPermissions.Value;
		}

		private Permission DeterminePartnerPermissions()
		{
			if (this.partnerPermissions == null)
			{
				this.partnerPermissions = new Permission?(Util.GetPermissionsForSid(WellKnownSids.PartnerServers, this.connector.GetSecurityDescriptor(), this.authzAuthorization, "partner", this.Connector.Name, ExTraceGlobals.SmtpReceiveTracer));
			}
			return this.partnerPermissions.Value;
		}

		private void IncrementConnectionLevelPerfCounters()
		{
			if (this.smtpAvailabilityPerfCounters != null)
			{
				int arg = (int)this.SmtpReceivePerformanceCounters.ConnectionsCurrent.Increment();
				this.SmtpReceivePerformanceCounters.ConnectionsTotal.Increment();
				ExTraceGlobals.SmtpReceiveTracer.TraceDebug<long, int>((long)this.GetHashCode(), "SmtpInSession(id={0}) created, connectionPerfCount={1}", this.connectionId, arg);
			}
		}

		private const int BlindProxyMaxClientConnectionShutdownWaitSeconds = 5;

		internal static readonly byte[] QuitCommand = new byte[]
		{
			81,
			85,
			73,
			84,
			13,
			10
		};

		private static readonly byte[] AuthLogLine = Encoding.ASCII.GetBytes("334 <authentication response>");

		private static readonly byte[] ExchangeAuthSuccessLine = Encoding.ASCII.GetBytes("235 <authentication response>");

		private static readonly AsyncCallback writeCompleteLogCallback = new AsyncCallback(SmtpInSession.WriteCompleteLogCallback);

		private static readonly AsyncCallback beginReadLine = new AsyncCallback(SmtpInSession.BeginReadLine);

		private static readonly AsyncCallback beginRead = new AsyncCallback(SmtpInSession.BeginRead);

		private static readonly AsyncCallback readCompleteFromProxyClient = new AsyncCallback(SmtpInSession.ReadCompleteFromProxyClient);

		private static readonly AsyncCallback writeToProxyTargetCompleted = new AsyncCallback(SmtpInSession.WriteToProxyTargetCompleted);

		private static readonly AsyncCallback readCompleteFromProxyTarget = new AsyncCallback(SmtpInSession.ReadCompleteFromProxyTarget);

		private static readonly AsyncCallback writeToProxyClientCompleted = new AsyncCallback(SmtpInSession.WriteToProxyClientCompleted);

		private static readonly AsyncCallback writeQuitToProxyTargetCompleted = new AsyncCallback(SmtpInSession.WriteQuitToProxyTargetCompleted);

		private static readonly AsyncCallback writeXRsetResponseToProxyClientCompleted = new AsyncCallback(SmtpInSession.WriteXRsetResponseToProxyClientCompleted);

		private static readonly AsyncCallback toShutdown = new AsyncCallback(SmtpInSession.ToShutdown);

		private static readonly AsyncCallback startTlsNegotiation = new AsyncCallback(SmtpInSession.StartTlsNegotiation);

		private static readonly AsyncCallback tlsNegotiationComplete = new AsyncCallback(SmtpInSession.TlsNegotiationComplete);

		private static readonly AsyncCallback readComplete = new AsyncCallback(SmtpInSession.ReadComplete);

		private static readonly AsyncCallback readLineComplete = new AsyncCallback(SmtpInSession.ReadLineComplete);

		private static readonly byte[] EmptyBuffer = new byte[0];

		protected ISmtpInServer server;

		protected ReceiveConnector connector;

		protected Permission sessionPermissions;

		protected IAuthzAuthorization authzAuthorization;

		protected IPAddress proxiedClientAddress;

		private readonly DateTime sessionStartTime;

		private readonly DateTime sessionExpireTime;

		private readonly ExtendedProtectionConfig extendedProtectionConfig;

		private readonly IMessageThrottlingManager messageThrottlingManager;

		private readonly IQueueQuotaComponent queueQuotaComponent;

		private readonly IIsMemberOfResolver<RoutingAddress> memberOfResolver;

		private readonly IMailRouter mailRouter;

		private readonly IEnhancedDns enhancedDns;

		private readonly IShadowRedundancyManager shadowRedundancyManager;

		private IShadowSession shadowSession;

		private readonly ITransportAppConfig transportAppConfig;

		private readonly ITransportConfiguration transportConfiguration;

		private readonly ExEventLog eventLogger;

		private IProtocolLogSession logSession;

		private readonly ISmtpAgentSession agentSession;

		private readonly ulong significantAddressBytes;

		private string remoteConnectionError;

		private bool sendAsRequiredADLookup;

		private int numberOfMessagesReceived;

		private readonly INetworkConnection connection;

		private readonly SmtpReceiveConnectorStub connectorStub;

		private readonly IPAddress clientIpAddress;

		private byte[] originalCommand;

		private SmtpCommand commandHandler;

		private SmtpResponse protocolResponse = SmtpResponse.Empty;

		private TransportMailItem transportMailItem;

		private TransportMailItemWrapper mailItemWrapper;

		private Stream messageWriteStream;

		private bool seenHelo;

		private bool seenEhlo;

		private int protocolErrors;

		private int logonFailures;

		private int messagesSubmitted;

		private bool tarpitRset = true;

		private readonly long connectionId;

		private bool isResponseBuffered;

		private GuardedTimer delayResponseTimer;

		private SmtpInBdatState bdatState;

		private EhloOptions ehloOptions;

		private bool firedOnConnectEvent;

		private MsgTrackReceiveInfo msgTrackInfo;

		private bool disconnectByServer;

		private AuthenticationSource? proxiedClientAuthSource;

		private uint? proxiedClientPermissions;

		private bool enforceMimeLimitsForProxiedSession;

		private SmtpSendConnectorConfig outboundProxySendConnector;

		private IEnumerable<INextHopServer> outboundProxyDestinations;

		private TlsSendConfiguration outboundProxyTlsSendConfiguration;

		private RiskLevel outboundProxyRiskLevel;

		private int outboundProxyOutboundIPPool;

		private string outboundProxySessionId;

		private string outboundProxyNextHopDomain;

		private string proxyUserName;

		private SecureString proxyPassword;

		private DisconnectReason proxyTargetDisconnectReason = DisconnectReason.Local;

		private string proxyTargetRemoteConnectionError;

		private bool blindProxyingAuthenticatedUser;

		private SmtpInSession.BlindProxyContext blindProxyContext;

		private int tooManyRecipientsResponseCount;

		private BufferBuilder commandBuffer;

		private SecureState secureState;

		private MultilevelAuthMechanism sessionAuthMethod;

		private readonly ClientData clientIpData;

		private SecurityIdentifier sessionRemoteIdentity = SmtpConstants.AnonymousSecurityIdentifier;

		private WindowsIdentity remoteWindowsIdentity;

		private string sessionRemoteIdentityName = "anonymous";

		private InboundRecipientCorrelator recipientCorrelator;

		private InboundExch50 inboundExch50;

		private XProxyToSmtpCommandParser xProxyToParser;

		private ChainValidityStatus? tlsRemoteCertificateChainValidationStatus;

		private RawDataHandler rawDataHandler;

		private BufferBuilder sendBuffer = new BufferBuilder();

		private List<SmtpResponse> responseList;

		private DisconnectReason disconnectReason = DisconnectReason.Local;

		private readonly Breadcrumbs<SmtpInSessionBreadcrumbs> breadcrumbs = new Breadcrumbs<SmtpInSessionBreadcrumbs>(64);

		private readonly bool certificatesLoadedSuccessfully;

		private bool isLastCommandEhloBeforeQuit;

		private ISmtpAvailabilityPerfCounters smtpAvailabilityPerfCounters;

		private ISmtpReceivePerfCounters smtpReceivePerfCountersInstance;

		private SmtpProxyPerfCountersWrapper smtpProxyPerfCounters;

		private readonly bool maxConnectionsExceeded;

		private readonly bool maxConnectionsPerSourceExceeded;

		private readonly string relatedMessageInfo;

		private SmtpInSession.DelayedAckStatus delayedAckStatus;

		private TransportMiniRecipient authUserRecipient;

		private string senderShadowContext;

		private SmtpReceiveCapabilities? tlsDomainCapabilities;

		private bool startTlsDisabled;

		private bool forceRequestClientTlsCertificate;

		private bool startClientProxySession;

		private bool startOutboundProxySession;

		private bool shutdownConnectionCalled;

		private string proxyHopHelloDomain = string.Empty;

		private InboundClientProxyStates inboundClientProxyState;

		private int bytesToBeProxied;

		private ProxySessionSetupHandler proxySetupHandler;

		private uint xProxyFromSeqNum;

		private Queue<SmtpMessageContextBlob> expectedBlobs;

		private bool supportIntegratedAuth = true;

		private bool isLastCommandAuthBeforeQuit;

		private bool isFrontEndProxyingInbound;

		private bool clientIpConnectionAlreadyRemoved;

		private MailCommandMessageContextParameters mailCommandMessageContextInformation;

		private bool clientProxyFailedDueToIncompatibleBackend;

		private IMExSession mexSession;

		private string peerSessionPrimaryServer;

		private Permission? anonymousPermissions;

		private Permission? partnerPermissions;

		private bool usedForBlindProxy;

		private readonly ISmtpMessageContextBlob smtpMessageContextBlob;

		private bool smtpUtf8Supported;

		private enum DelayedAckStatus
		{
			None,
			Stamped,
			ShadowRedundancyManagerNotified,
			WaitingForShadowRedundancyManager
		}

		private sealed class WriteCompleteLogCallbackParameters
		{
			public WriteCompleteLogCallbackParameters(SmtpInSession session, List<SmtpResponse> responseList, AsyncCallback callback, object callbackContextParam, bool alwaysCall)
			{
				this.session = session;
				this.responseList = responseList;
				this.callback = callback;
				this.callbackContextParam = callbackContextParam;
				this.alwaysCall = alwaysCall;
			}

			public SmtpInSession Session
			{
				get
				{
					return this.session;
				}
			}

			public List<SmtpResponse> ResponseList
			{
				get
				{
					return this.responseList;
				}
			}

			public AsyncCallback Callback
			{
				get
				{
					return this.callback;
				}
			}

			public object CallbackContextParam
			{
				get
				{
					return this.callbackContextParam;
				}
			}

			public bool AlwaysCall
			{
				get
				{
					return this.alwaysCall;
				}
			}

			private readonly SmtpInSession session;

			private readonly List<SmtpResponse> responseList;

			private readonly AsyncCallback callback;

			private readonly object callbackContextParam;

			private readonly bool alwaysCall;
		}

		private sealed class BlindProxyContext
		{
			public BlindProxyContext(SmtpInSession session, NetworkConnection proxyConnection, IProtocolLogSession blindProxySendLogSession, ulong blindProxySendSessionId)
			{
				if (proxyConnection == null)
				{
					throw new ArgumentNullException("proxyConnection");
				}
				this.Session = session;
				this.ProxyConnection = proxyConnection;
				this.BlindProxySendLogSession = blindProxySendLogSession;
				this.BlindProxySendSessionId = blindProxySendSessionId;
			}

			public readonly SmtpInSession Session;

			public readonly NetworkConnection ProxyConnection;

			public readonly IProtocolLogSession BlindProxySendLogSession;

			public readonly ulong BlindProxySendSessionId;

			public int IsProxyClientWritePending;

			public int XRsetProxyToResponseNeeded;

			public int XRsetProxyToResponseWriteOwner;

			public int IsProxyTargetWritePending;

			public int QuitCommandToTargetNeeded;

			public int QuitCommandToTargetWriteOwner;

			public int BlindProxyWorkDone;
		}
	}
}
