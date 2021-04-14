using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Internal.MExRuntime;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.SecureMail;
using Microsoft.Exchange.Security;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Security.Cryptography.X509Certificates;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.Transport.Logging;
using Microsoft.Exchange.Transport.Logging.MessageTracking;
using Microsoft.Exchange.Transport.MessageThrottling;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class SmtpInSessionState : SmtpSession, IDisposeTrackable, IDisposable
	{
		public SmtpInSessionState(SmtpInServerState serverState, INetworkConnection networkConnection, SmtpReceiveConnectorStub receiveConnectorStub)
		{
			ArgumentValidator.ThrowIfNull("serverState", serverState);
			ArgumentValidator.ThrowIfNull("networkConnection", networkConnection);
			ArgumentValidator.ThrowIfNull("receiveConnectorStub", receiveConnectorStub);
			this.ServerState = serverState;
			this.AuthzAuthorization = serverState.AuthzAuthorization;
			this.CertificateValidator = serverState.CertificateValidator;
			this.EventLog = serverState.EventLog;
			this.EventNotificationItem = serverState.EventNotificationItem;
			this.ExpectedMessageContextBlobs = SmtpInSessionState.EmptyInboundMessageContextBlobs;
			this.MessageContextBlob = serverState.MessageContextBlob;
			this.Tracer = serverState.Tracer;
			this.NetworkConnection = networkConnection;
			this.RemoteEndPoint = this.NetworkConnection.RemoteEndPoint;
			this.Configuration = this.ServerState.SmtpConfiguration;
			this.MessageThrottlingManager = serverState.MessageThrottlingManager;
			this.ReceiveConnector = receiveConnectorStub.Connector;
			this.ReceiveConnectorStub = receiveConnectorStub;
			this.SmtpResponse = SmtpResponse.Empty;
			this.DisconnectReason = DisconnectReason.None;
			this.LastExternalIPAddress = (this.IsExternalConnection ? this.RemoteEndPoint.Address : null);
			this.SessionStartTime = DateTime.UtcNow;
			this.sessionId = Microsoft.Exchange.Transport.SessionId.GetNextSessionId();
			this.AdvertisedEhloOptions = SmtpInSessionState.CreateEhloOptions(networkConnection, this.ReceiveConnector, this.AdvertisedDomain);
			this.ExtendedProtectionConfig = SmtpInSessionState.CreateExtendedProtectionConfig(this.ReceiveConnector.ExtendedProtectionPolicy);
			this.disposeTracker = this.GetDisposeTracker();
			IMExSession mexRuntimeSession;
			this.SmtpAgentSession = serverState.AgentRuntime.NewSmtpAgentSession(this, this.ServerState.IsMemberOfResolver, this.Configuration.TransportConfiguration.FirstOrgAcceptedDomainTable, this.Configuration.TransportConfiguration.RemoteDomainTable, this.Configuration.TransportConfiguration.Version, out mexRuntimeSession);
			this.MexRuntimeSession = mexRuntimeSession;
			this.ProtocolLogSession = this.ServerState.ProtocolLog.OpenSession(this.ReceiveConnector.Id.ToString(), this.sessionId, networkConnection.RemoteEndPoint, networkConnection.LocalEndPoint, this.ReceiveConnector.ProtocolLoggingLevel);
			this.ProtocolLogSession.LogConnect();
			bool isFrontEndTransportProcess = this.Configuration.TransportConfiguration.ProcessTransportRole == ProcessTransportRole.FrontEnd;
			SmtpInSessionUtils.ApplyRoleBasedEhloOptionsOverrides(this.AdvertisedEhloOptions, isFrontEndTransportProcess);
			this.supportIntegratedAuth = SmtpInSessionUtils.ShouldSupportIntegratedAuthentication(true, isFrontEndTransportProcess);
			this.LoadCertificates();
			this.SetInitialIdentity();
		}

		public static SmtpInSessionState FromSmtpInSession(ISmtpInSession session)
		{
			ArgumentValidator.ThrowIfNull("session", session);
			return new SmtpInSessionState(session);
		}

		public IEhloOptions AdvertisedEhloOptions { get; set; }

		public IX509Certificate2 AdvertisedTlsCertificate { get; private set; }

		public TransportMiniRecipient AuthenticatedUser { get; private set; }

		public override AuthenticationSource AuthenticationSource
		{
			get
			{
				return this.authenticationSourceForAgents;
			}
		}

		public MultilevelAuthMechanism AuthMethod { get; private set; }

		public IAuthzAuthorization AuthzAuthorization { get; private set; }

		public BdatState BdatState { get; private set; }

		public ICertificateValidator CertificateValidator { get; private set; }

		public ISmtpReceiveConfiguration Configuration { get; private set; }

		public DelayedAckStatus DelayedAckStatus { get; set; }

		internal override DisconnectReason DisconnectReason { get; set; }

		public IExEventLog EventLog { get; private set; }

		public IEventNotificationItem EventNotificationItem { get; private set; }

		public Queue<IInboundMessageContextBlob> ExpectedMessageContextBlobs { get; private set; }

		public ExtendedProtectionConfig ExtendedProtectionConfig { get; private set; }

		public bool FirstBdatCall { get; set; }

		internal override bool RequestClientTlsCertificate { get; set; }

		public IX509Certificate2 InternalTransportCertificate { get; private set; }

		internal override bool DiscardingMessage
		{
			get
			{
				return this.isDiscardingMessage;
			}
		}

		public override bool IsExternalConnection { get; internal set; }

		public object LastNetworkError
		{
			set
			{
				ArgumentValidator.ThrowIfInvalidValue<object>("value", value, (object v) => v is SocketError || v is SecurityStatus);
				this.lastNetworkError = value;
			}
		}

		public SocketError LastSocketError
		{
			get
			{
				if (this.lastNetworkError is SocketError)
				{
					return (SocketError)this.lastNetworkError;
				}
				return SocketError.Success;
			}
			set
			{
				this.lastNetworkError = value;
			}
		}

		public SecurityStatus LastTlsError
		{
			get
			{
				if (this.lastNetworkError is SecurityStatus)
				{
					return (SecurityStatus)this.lastNetworkError;
				}
				return SecurityStatus.OK;
			}
			set
			{
				this.lastNetworkError = value;
			}
		}

		public MailCommandMessageContextParameters MailCommandMessageContextInformation { get; set; }

		public ISmtpMessageContextBlob MessageContextBlob { get; private set; }

		public int NumberOfMessagesReceived { get; private set; }

		public int NumberOfMessagesSubmitted { get; private set; }

		public IMessageThrottlingManager MessageThrottlingManager { get; private set; }

		public MsgTrackReceiveInfo MessageTrackReceiveInfo { get; set; }

		public Stream MessageWriteStream { get; private set; }

		public IMExSession MexRuntimeSession { get; set; }

		public INetworkConnection NetworkConnection { get; private set; }

		public int NumLogonFailures { get; private set; }

		public string PeerSessionPrimaryServer { get; set; }

		public IProtocolLogSession ProtocolLogSession { get; private set; }

		public ReceiveConnector ReceiveConnector { get; private set; }

		public SmtpReceiveConnectorStub ReceiveConnectorStub { get; private set; }

		public InboundRecipientCorrelator RecipientCorrelator { get; private set; }

		public WindowsIdentity RemoteWindowsIdentity { get; private set; }

		public SecurityIdentifier RemoteIdentity { get; private set; }

		public string RemoteIdentityName { get; private set; }

		public SecureState SecureState { get; set; }

		public bool SendAsRequiredADLookup { get; set; }

		public string SenderShadowContext { get; set; }

		public SmtpInServerState ServerState { get; private set; }

		public Permission SessionPermissions
		{
			get
			{
				return this.sessionPermissions;
			}
			private set
			{
				this.sessionPermissions = value;
				this.TraceAndLogSessionPermissions();
			}
		}

		public DateTime SessionStartTime { get; private set; }

		public ISmtpAgentSession SmtpAgentSession { get; private set; }

		public bool SmtpUtf8Supported { get; set; }

		public SmtpReceiveCapabilities? TlsDomainCapabilities { get; set; }

		public IX509Certificate2 TlsRemoteCertificateInternal { get; set; }

		public ITracer Tracer { get; private set; }

		public TransportMailItem TransportMailItem { get; private set; }

		public TransportMailItemWrapper TransportMailItemWrapper { get; private set; }

		public virtual void OnDisconnect()
		{
			this.ProtocolLogSession.LogDisconnect(this.DisconnectReason);
			this.SmtpAgentSession.Close();
			this.AbortMailTransaction();
		}

		public void HandleNetworkError(object errorCode)
		{
			ArgumentValidator.ThrowIfInvalidValue<object>("errorCode", errorCode, (object v) => v is SocketError || v is SecurityStatus);
			this.LastNetworkError = errorCode;
			this.DisconnectReason = Util.DisconnectReasonFromError(errorCode);
		}

		public void AddSessionPermissions(SmtpReceiveCapabilities capabilities)
		{
			this.SessionPermissions = Util.AddSessionPermissions(capabilities, this.SessionPermissions, this.AuthzAuthorization, this.ReceiveConnectorStub.SecurityDescriptor, this.ProtocolLogSession, this.Tracer, this.GetHashCode());
			if (SmtpInSessionUtils.HasAcceptCrossForestMailCapability(capabilities))
			{
				this.RemoteIdentity = WellKnownSids.ExternallySecuredServers;
				this.RemoteIdentityName = "accepted_domain";
			}
		}

		public virtual bool TryCalculateTlsDomainCapabilitiesFromRemoteTlsCertificate(out SmtpReceiveCapabilities capabilities)
		{
			if (this.TlsDomainCapabilities != null)
			{
				capabilities = this.TlsDomainCapabilities.Value;
				return true;
			}
			return Util.TryDetermineTlsDomainCapabilities(this.CertificateValidator, this.TlsRemoteCertificateInternal, this.TlsRemoteCertificateChainValidationStatus, this.ReceiveConnectorStub, this.ProtocolLogSession, this.EventLog, this.Tracer, out capabilities);
		}

		public virtual IThrottlingPolicy GetThrottlingPolicy()
		{
			if (this.AuthenticatedUser == null)
			{
				return null;
			}
			return ThrottlingPolicyCache.Singleton.Get(this.AuthenticatedUser.OrganizationId, this.AuthenticatedUser.ThrottlingPolicy);
		}

		public virtual bool IsValidMessagePriority(out SmtpResponse failureResponse)
		{
			if (this.TransportMailItem == null)
			{
				failureResponse = SmtpResponse.Empty;
				return false;
			}
			if (this.TransportMailItem.ValidateDeliveryPriority(out failureResponse))
			{
				return true;
			}
			this.StartDiscardingMessage();
			return false;
		}

		internal override void GrantMailItemPermissions(Permission permissions)
		{
			this.MailItemPermissionsGranted |= permissions;
			this.MailItemPermissionsDenied &= ~permissions;
			this.ProtocolLogSession.LogInformation(ProtocolLoggingLevel.Verbose, Util.AsciiStringToBytes(Util.GetPermissionString(permissions)), "Granted Mail Item Permissions");
		}

		public void IncrementNumLogonFailures()
		{
			this.NumLogonFailures++;
		}

		public void DenyMailItemPermissions(Permission permissions)
		{
			this.MailItemPermissionsGranted &= ~permissions;
			this.MailItemPermissionsDenied |= permissions;
			this.ProtocolLogSession.LogInformation(ProtocolLoggingLevel.Verbose, Util.AsciiStringToBytes(Util.GetPermissionString(permissions)), "Denied Mail Item Permissions");
		}

		public virtual bool IsMessagePoison(string messageId)
		{
			return PoisonMessage.IsMessagePoison(messageId);
		}

		public void ResetMailItemPermissions()
		{
			this.MailItemPermissionsGranted = Permission.None;
			this.MailItemPermissionsDenied = Permission.None;
		}

		public void ResetExpectedBlobs()
		{
			this.ExpectedMessageContextBlobs.Clear();
			this.MailCommandMessageContextInformation = null;
		}

		public void CloseMessageWriteStream()
		{
			Util.CloseMessageWriteStream(this.MessageWriteStream, this.TransportMailItem, this.Tracer, this.GetHashCode());
			this.MessageWriteStream = null;
		}

		public virtual void AbortMailTransaction()
		{
			if (this.TransportMailItem != null)
			{
				if (this.TransportMailItem.IsActive && !this.TransportMailItem.IsNew)
				{
					this.TransportMailItem.ReleaseFromActiveMaterializedLazy();
				}
				this.ReleaseMailItem();
			}
			this.isDiscardingMessage = false;
			this.SmtpResponse = SmtpResponse.Empty;
		}

		public virtual void ReleaseMailItem()
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
				this.TransportMailItem.ReleaseFromActive();
				this.ResetMailItemPermissions();
				this.MessageWriteStream = null;
				this.TransportMailItem = null;
				this.RecipientCorrelator = null;
				this.BdatState = null;
				this.isDiscardingMessage = false;
				this.SmtpResponse = SmtpResponse.Empty;
			}
		}

		public void SetupExpectedBlobs(MailCommandMessageContextParameters messageContextParameters)
		{
			if (messageContextParameters == null)
			{
				this.Tracer.TraceDebug((long)this.GetHashCode(), "SmtpInSession(id={0}) Not Messagecontext is specified");
				return;
			}
			this.ExpectedMessageContextBlobs = new Queue<IInboundMessageContextBlob>(messageContextParameters.OrderedListOfBlobs.Count);
			foreach (IInboundMessageContextBlob item in messageContextParameters.OrderedListOfBlobs)
			{
				this.ExpectedMessageContextBlobs.Enqueue(item);
			}
			this.MailCommandMessageContextInformation = messageContextParameters;
		}

		public SmtpResponse CreateTransportMailItem(MailParseOutput parseOutput, MailCommandEventArgs agentEventArgs)
		{
			ArgumentValidator.ThrowIfNull("parseOutput", parseOutput);
			SmtpResponse result;
			try
			{
				ADRecipientCache<TransportMiniRecipient> recipientCache = null;
				Guid empty = Guid.Empty;
				MailDirectionality directionality = MailDirectionality.Undefined;
				if (this.AuthenticatedUser != null && this.AuthenticatedUser.PrimarySmtpAddress.IsValidAddress)
				{
					ADOperationResult adOperationResult = this.CreateRecipientCache(out recipientCache);
					result = this.HandleCacheCreationResponse(recipientCache, adOperationResult, this.Configuration.TransportConfiguration.RejectUnscopedMessages, out directionality, out empty);
					if (!result.IsEmpty)
					{
						return result;
					}
				}
				TransportMailItem transportMailItem = this.CreateAndInitializeTransportMailItem(parseOutput, recipientCache, directionality, empty);
				this.HandleShadowMessageChecks(transportMailItem);
				if (!this.IsExternalConnection)
				{
					this.LastExternalIPAddress = null;
				}
				result = this.TryUpdateRecipientCacheForAttributionData(transportMailItem, parseOutput);
				if (!result.IsEmpty)
				{
					return result;
				}
				this.InitializeMessageTrackingInfo();
				this.RecipientCorrelator = new InboundRecipientCorrelator();
				this.TransferMailCommandProperties(transportMailItem, parseOutput, agentEventArgs);
				this.TransportMailItem = transportMailItem;
				LatencyTracker.BeginTrackLatency(LatencyComponent.SmtpReceive, this.TransportMailItem.LatencyTracker);
				if (!Util.IsFrontEndRole(this.Configuration.TransportConfiguration.ProcessTransportRole))
				{
					LatencyComponent component = (this.IsInboundProxiedSession || this.IsClientProxiedSession) ? LatencyComponent.SmtpReceiveDataExternal : LatencyComponent.SmtpReceiveDataInternal;
					LatencyTracker.BeginTrackLatency(component, this.TransportMailItem.LatencyTracker);
				}
				this.TransportMailItemWrapper = new TransportMailItemWrapper(this.TransportMailItem, this.MexRuntimeSession, true);
			}
			catch (IOException)
			{
				this.UpdateAvailabilityPerfCounters(LegitimateSmtpAvailabilityCategory.RejectDueToIOException);
				this.TransportMailItem = null;
				this.MessageTrackReceiveInfo = null;
				this.RecipientCorrelator = null;
				result = SmtpResponse.DataTransactionFailed;
			}
			this.IncrementMessageCount();
			return result;
		}

		public virtual Stream OpenMessageWriteStream(bool expectBinaryContent)
		{
			if (this.TransportMailItem == null)
			{
				throw new InvalidOperationException("No transport message");
			}
			MimeLimits mimeLimits = SmtpInSessionUtils.HasSMTPBypassMessageSizeLimitPermission(this.CombinedPermissions) ? MimeLimits.Unlimited : MimeLimits.Default;
			this.MessageWriteStream = this.TransportMailItem.OpenMimeWriteStream(mimeLimits, expectBinaryContent);
			return this.MessageWriteStream;
		}

		public void UpdateSmtpAvailabilityPerfCounter(LegitimateSmtpAvailabilityCategory category)
		{
			if (this.ReceiveConnectorStub.SmtpAvailabilityPerfCounters != null)
			{
				this.ReceiveConnectorStub.SmtpAvailabilityPerfCounters.UpdatePerformanceCounters(category);
			}
		}

		public void StartDiscardingMessage()
		{
			if (this.isDiscardingMessage)
			{
				return;
			}
			this.isDiscardingMessage = true;
			if (this.TransportMailItem != null && this.TransportMailItem.MimeDocument != null)
			{
				this.TransportMailItem.MimeDocument.EndOfHeaders = null;
			}
		}

		public void PutBackReceivedBytes(int bytesUnconsumed)
		{
			ISmtpReceivePerfCounters smtpReceivePerfCounterInstance = this.ReceiveConnectorStub.SmtpReceivePerfCounterInstance;
			if (smtpReceivePerfCounterInstance != null)
			{
				smtpReceivePerfCounterInstance.TotalBytesReceived.IncrementBy((long)(-(long)bytesUnconsumed));
			}
			this.NetworkConnection.PutBackReceivedBytes(bytesUnconsumed);
		}

		public SmtpResponse TrackAndEnqueueMailItem()
		{
			this.UpdateSmtpReceivePerfCountersForMessageReceived(this.TransportMailItem.Recipients.Count, this.TransportMailItem.MimeSize);
			if (this.TransportMailItem.AuthMethod == MultilevelAuthMechanism.MutualTLS)
			{
				Utils.SecureMailPerfCounters.DomainSecureMessagesReceivedTotal.Increment();
			}
			if (!string.IsNullOrEmpty(this.TransportMailItem.MessageTrackingSecurityInfo))
			{
				this.MessageTrackReceiveInfo = new MsgTrackReceiveInfo(this.MessageTrackReceiveInfo.ClientIPAddress, this.MessageTrackReceiveInfo.ClientHostname, this.MessageTrackReceiveInfo.ServerIPAddress, this.MessageTrackReceiveInfo.SourceContext, this.MessageTrackReceiveInfo.ConnectorId, this.MessageTrackReceiveInfo.RelatedMailItemId, this.TransportMailItem.MessageTrackingSecurityInfo, string.Empty, string.Empty, this.MessageTrackReceiveInfo.ProxiedClientIPAddress, this.MessageTrackReceiveInfo.ProxiedClientHostname, this.TransportMailItem.RootPart.Headers.FindAll(HeaderId.Received), (this.AuthenticatedUser != null) ? this.AuthenticatedUser.ExchangeGuid : Guid.Empty);
			}
			if (!Util.IsMailboxTransportRole(this.Configuration.TransportConfiguration.ProcessTransportRole))
			{
				MessageTrackingLog.TrackReceive(MessageTrackingSource.SMTP, this.TransportMailItem, this.MessageTrackReceiveInfo);
			}
			if (this.Configuration.TransportConfiguration.ProcessTransportRole == ProcessTransportRole.MailboxDelivery)
			{
				this.TransportMailItem.ExtendedProperties.SetValue<string>("Microsoft.Exchange.Transport.MailboxTransport.SmtpInClientHostname", this.MessageTrackReceiveInfo.ClientHostname);
			}
			LatencyTracker.EndTrackLatency(LatencyComponent.SmtpReceive, this.TransportMailItem.LatencyTracker);
			this.NumberOfMessagesSubmitted++;
			this.TransportMailItem.PerfCounterAttribution = "InQueue";
			return this.ServerState.Categorizer.EnqueueSubmittedMessage(this.TransportMailItem);
		}

		public bool SetupMessageStream(bool allowBinaryContent, out Stream bodyStream)
		{
			bool result;
			try
			{
				bodyStream = this.OpenMessageWriteStream(allowBinaryContent);
				result = true;
			}
			catch (IOException arg)
			{
				this.Tracer.TraceError<IOException>((long)this.GetHashCode(), "OpenMessageWriteStream failed: {0}", arg);
				this.UpdateSmtpAvailabilityPerfCounter(LegitimateSmtpAvailabilityCategory.RejectDueToIOException);
				bodyStream = null;
				this.StartDiscardingMessage();
				this.AbortMailTransaction();
				result = false;
			}
			return result;
		}

		public bool InitializeBdatState(ISmtpInStreamBuilder streamBuilder, long chunkSize, long messageSizeLimit)
		{
			ArgumentValidator.ThrowIfNull("streamBuilder", streamBuilder);
			if (this.BdatState != null)
			{
				streamBuilder.BodyStream = this.BdatState.BdatStream;
				this.BdatState.IncrementAccumulatedChunkSize(chunkSize);
				return true;
			}
			Stream stream;
			if (!this.SetupMessageStream(true, out stream))
			{
				this.StartDiscardingMessage();
				streamBuilder.IsDiscardingData = true;
				return false;
			}
			streamBuilder.BodyStream = stream;
			this.BdatState = new BdatState(this.TransportMailItem.InternetMessageId, stream, chunkSize, 0L, messageSizeLimit, false);
			return true;
		}

		public string AdvertisedDomain
		{
			get
			{
				if (string.IsNullOrEmpty(this.advertisedDomain))
				{
					this.advertisedDomain = Util.AdvertisedDomainFromReceiveConnector(this.ReceiveConnector, () => this.Configuration.TransportConfiguration.PhysicalMachineName);
				}
				return this.advertisedDomain;
			}
		}

		public Permission AnonymousPermissions
		{
			get
			{
				if (this.anonymousPermissions == null)
				{
					this.anonymousPermissions = new Permission?(Util.GetPermissionsForSid(SmtpConstants.AnonymousSecurityIdentifier, this.ReceiveConnector.GetSecurityDescriptor(), this.AuthzAuthorization, "anonymous", this.ReceiveConnector.Name, this.Tracer));
				}
				return this.anonymousPermissions.Value;
			}
		}

		internal override SmtpResponse Banner { get; set; }

		public SmtpReceiveCapabilities Capabilities
		{
			get
			{
				return Util.SessionCapabilitiesFromTlsAndNonTlsCapabilities(this.SecureState, this.ReceiveConnectorStub.NoTlsCapabilities, this.TlsDomainCapabilities);
			}
		}

		public Permission CombinedPermissions
		{
			get
			{
				Permission permission = this.SessionPermissions;
				if (this.Configuration.TransportConfiguration.GrantExchangeServerPermissions)
				{
					permission |= (Permission.SMTPSubmit | Permission.SMTPAcceptAnyRecipient | Permission.SMTPAcceptAuthenticationFlag | Permission.SMTPAcceptAnySender | Permission.SMTPAcceptAuthoritativeDomainSender | Permission.BypassAntiSpam | Permission.BypassMessageSizeLimit | Permission.SMTPAcceptEXCH50 | Permission.AcceptRoutingHeaders | Permission.AcceptForestHeaders | Permission.AcceptOrganizationHeaders | Permission.SMTPAcceptXShadow | Permission.SMTPAcceptXProxyFrom | Permission.SMTPAcceptXSessionParams | Permission.SMTPAcceptXMessageContextADRecipientCache | Permission.SMTPAcceptXMessageContextExtendedProperties | Permission.SMTPAcceptXMessageContextFastIndex | Permission.SMTPAcceptXAttr | Permission.SMTPAcceptXSysProbe);
				}
				return (permission | this.MailItemPermissionsGranted) & ~this.MailItemPermissionsDenied;
			}
		}

		internal override string CurrentMessageTemporaryId
		{
			get
			{
				return SmtpInSessionUtils.GetFormattedTemporaryMessageId(this.sessionId, this.SessionStartTime, this.NumberOfMessagesReceived);
			}
		}

		public override string HelloDomain { get; internal set; }

		public bool IsAnonymousTlsSupported
		{
			get
			{
				return this.InternalTransportCertificate != null && !this.ReceiveConnector.SuppressXAnonymousTls;
			}
		}

		public bool IsIntegratedAuthSupported
		{
			get
			{
				return this.supportIntegratedAuth && (this.ReceiveConnector.AuthMechanism & AuthMechanisms.Integrated) != AuthMechanisms.None;
			}
		}

		public bool IsMaxLogonFailuresExceeded
		{
			get
			{
				return this.ReceiveConnector.MaxLogonFailures > 0 && this.NumLogonFailures > this.ReceiveConnector.MaxLogonFailures;
			}
		}

		public bool IsSecureSession
		{
			get
			{
				return this.SecureState == SecureState.AnonymousTls || this.SecureState == SecureState.StartTls;
			}
		}

		public bool IsStartTlsSupported
		{
			get
			{
				return this.AdvertisedTlsCertificate != null && !this.startTlsDisabled;
			}
		}

		public Permission PartnerPermissions
		{
			get
			{
				if (this.partnerPermissions == null)
				{
					this.partnerPermissions = new Permission?(Util.GetPermissionsForSid(WellKnownSids.PartnerServers, this.ReceiveConnector.GetSecurityDescriptor(), this.AuthzAuthorization, "partner", this.ReceiveConnector.Name, this.Tracer));
				}
				return this.partnerPermissions.Value;
			}
		}

		public ISmtpReceivePerfCounters ReceivePerfCounters
		{
			get
			{
				return this.ReceiveConnectorStub.SmtpReceivePerfCounterInstance;
			}
		}

		public override long SessionId
		{
			get
			{
				return (long)this.sessionId;
			}
		}

		internal override bool DisableStartTls
		{
			get
			{
				return this.startTlsDisabled;
			}
			set
			{
				if (value && this.SecureState == SecureState.StartTls)
				{
					throw new InvalidOperationException("Cannnot disable STARTTLS after the command has already been received");
				}
				this.startTlsDisabled = value;
			}
		}

		public ChainValidityStatus TlsRemoteCertificateChainValidationStatus
		{
			get
			{
				if (this.SecureState == SecureState.None)
				{
					return ChainValidityStatus.Valid;
				}
				if (this.tlsRemoteCertificateChainValidationStatus == null)
				{
					this.tlsRemoteCertificateChainValidationStatus = new ChainValidityStatus?(Util.CalculateTlsRemoteCertificateChainValidationStatus(this.Configuration.TransportConfiguration.ClientCertificateChainValidationEnabled, this.CertificateValidator, this.TlsRemoteCertificateInternal, this.ProtocolLogSession, this.EventLog));
				}
				return this.tlsRemoteCertificateChainValidationStatus.Value;
			}
		}

		public void UpdateAvailabilityPerfCounters(LegitimateSmtpAvailabilityCategory category)
		{
			if (this.ReceiveConnectorStub.SmtpAvailabilityPerfCounters != null)
			{
				this.ReceiveConnectorStub.SmtpAvailabilityPerfCounters.UpdatePerformanceCounters(category);
			}
		}

		public virtual DateTime UtcNow
		{
			get
			{
				return DateTime.UtcNow;
			}
		}

		private Permission MailItemPermissionsGranted { get; set; }

		private Permission MailItemPermissionsDenied { get; set; }

		protected DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<SmtpInSessionState>(this);
		}

		protected void InternalDispose(bool disposingNotFinalizing)
		{
			if (this.isInvokedFromLegacyStack)
			{
				return;
			}
			if (disposingNotFinalizing)
			{
				if (this.RemoteWindowsIdentity != null)
				{
					this.RemoteWindowsIdentity.Dispose();
					this.RemoteWindowsIdentity = null;
				}
				if (this.NetworkConnection != null)
				{
					this.NetworkConnection.Dispose();
					this.NetworkConnection = null;
				}
			}
		}

		protected void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				if (disposing && this.disposeTracker != null)
				{
					this.disposeTracker.Dispose();
					this.disposeTracker = null;
				}
				this.InternalDispose(disposing);
				this.disposed = true;
			}
		}

		protected void CheckDisposed()
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
		}

		protected virtual ADOperationResult CreateRecipientCache(out ADRecipientCache<TransportMiniRecipient> recipientCache)
		{
			ADRecipientCache<TransportMiniRecipient> adRecipientCache = null;
			ADOperationResult result = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				adRecipientCache = new ADRecipientCache<TransportMiniRecipient>(TransportMiniRecipientSchema.Properties, 1, this.AuthenticatedUser.OrganizationId);
			}, 0);
			recipientCache = adRecipientCache;
			return result;
		}

		protected virtual SmtpResponse HandleCacheCreationResponse(ADRecipientCache<TransportMiniRecipient> recipientCache, ADOperationResult adOperationResult, bool rejectUnscopedMessages, out MailDirectionality directionality, out Guid externalOrgId)
		{
			directionality = MailDirectionality.Undefined;
			externalOrgId = Guid.Empty;
			if (adOperationResult.Succeeded)
			{
				this.AddRecipientCacheEntry(recipientCache);
				directionality = MailDirectionality.Originating;
				adOperationResult = MultiTenantTransport.TryGetExternalOrgId(this.AuthenticatedUser.OrganizationId, out externalOrgId);
			}
			switch (adOperationResult.ErrorCode)
			{
			case ADOperationErrorCode.RetryableError:
				MultiTenantTransport.TraceAttributionError("Retriable Error {0} attributing authUserRecipient {1}", new object[]
				{
					adOperationResult.Exception,
					this.AuthenticatedUser.PrimarySmtpAddress
				});
				return SmtpResponse.HubAttributionTransientFailureInCreateTmi;
			case ADOperationErrorCode.PermanentError:
				if (rejectUnscopedMessages)
				{
					MultiTenantTransport.TraceAttributionError("Permanent Error {0} attributing authUserRecipient {1}", new object[]
					{
						adOperationResult.Exception,
						this.AuthenticatedUser.PrimarySmtpAddress
					});
					return SmtpResponse.HubAttributionFailureInCreateTmi;
				}
				MultiTenantTransport.TraceAttributionError("Permanent Error {0} attributing authUserRecipient {1}. Falling back to safe tenant", new object[]
				{
					adOperationResult.Exception,
					this.AuthenticatedUser.PrimarySmtpAddress
				});
				externalOrgId = MultiTenantTransport.SafeTenantId;
				break;
			}
			return SmtpResponse.Empty;
		}

		protected virtual TransportMailItem NewTransportMailItem(ADRecipientCache<TransportMiniRecipient> recipientCache, MailDirectionality directionality, Guid externalOrgId)
		{
			return TransportMailItem.NewMailItem(recipientCache, LatencyComponent.SmtpReceive, directionality, externalOrgId);
		}

		protected virtual void InitializeMessageTrackingInfo()
		{
			if (this.MessageTrackReceiveInfo == null)
			{
				this.MessageTrackReceiveInfo = new MsgTrackReceiveInfo(this.NetworkConnection.RemoteEndPoint.Address, this.HelloDomain, this.NetworkConnection.LocalEndPoint.Address, this.CurrentMessageTemporaryId, this.ReceiveConnector.Id.ToString(), null, null, string.Empty, (this.AuthenticatedUser != null) ? this.AuthenticatedUser.ExchangeGuid : Guid.Empty);
			}
		}

		protected virtual SmtpResponse TryUpdateRecipientCacheForAttributionData(TransportMailItem transportMailItem, MailParseOutput parseOutput)
		{
			SmtpResponse empty = SmtpResponse.Empty;
			if (!this.IsAttributionDataSpecified(parseOutput))
			{
				return empty;
			}
			OrganizationId mailCommandInternalOrganizationId = (parseOutput.XAttrOrgId != null) ? parseOutput.XAttrOrgId.InternalOrgId : null;
			ADOperationResult adoperationResult = SmtpInSessionUtils.TryCreateOrUpdateADRecipientCache(transportMailItem, mailCommandInternalOrganizationId, this.ProtocolLogSession);
			if (adoperationResult.ErrorCode != ADOperationErrorCode.RetryableError)
			{
				return empty;
			}
			this.RecipientCorrelator = null;
			return SmtpResponse.HubAttributionTransientFailureInMailFrom;
		}

		private static EhloOptions CreateEhloOptions(INetworkConnection networkConnection, ReceiveConnector receiveConnector, string advertisedDomain)
		{
			return new EhloOptions
			{
				AdvertisedFQDN = advertisedDomain,
				AdvertisedIPAddress = networkConnection.RemoteEndPoint.Address,
				BinaryMime = receiveConnector.BinaryMimeEnabled,
				Chunking = receiveConnector.ChunkingEnabled,
				Dsn = receiveConnector.DeliveryStatusNotificationEnabled,
				EightBitMime = receiveConnector.EightBitMimeEnabled,
				EnhancedStatusCodes = receiveConnector.EnhancedStatusCodesEnabled,
				MaxSize = SmtpInSessionState.MaxSizeEhloOptionFromReceiveConnector(receiveConnector.MaxMessageSize.ToBytes()),
				Pipelining = receiveConnector.PipeliningEnabled,
				Size = receiveConnector.SizeEnabled,
				SmtpUtf8 = receiveConnector.SmtpUtf8Enabled,
				Xexch50 = false,
				XLongAddr = receiveConnector.LongAddressesEnabled,
				XOrar = receiveConnector.OrarEnabled,
				XRDst = false
			};
		}

		private static long MaxSizeEhloOptionFromReceiveConnector(ulong maxMessageSize)
		{
			if (maxMessageSize <= 9223372036854775807UL)
			{
				return (long)maxMessageSize;
			}
			return long.MaxValue;
		}

		private static ExtendedProtectionConfig CreateExtendedProtectionConfig(Microsoft.Exchange.Data.Directory.SystemConfiguration.ExtendedProtectionPolicySetting policy)
		{
			if (policy == Microsoft.Exchange.Data.Directory.SystemConfiguration.ExtendedProtectionPolicySetting.None)
			{
				return ExtendedProtectionConfig.NoExtendedProtection;
			}
			return new ExtendedProtectionConfig((int)policy, null, false);
		}

		private void LoadCertificates()
		{
			IX509Certificate2 internalTransportCertificate;
			Util.LoadDirectTrustCertificate(this.ReceiveConnector, this.NetworkConnection.ConnectionId, this.Configuration.TransportConfiguration.InternalTransportCertificateThumbprint, this.UtcNow, this.ServerState.CertificateCache, this.EventLog, this.Tracer, out internalTransportCertificate);
			this.InternalTransportCertificate = internalTransportCertificate;
			IX509Certificate2 advertisedTlsCertificate;
			Util.LoadStartTlsCertificate(this.ReceiveConnector, this.AdvertisedEhloOptions.AdvertisedFQDN, this.NetworkConnection.ConnectionId, this.Configuration.TransportConfiguration.OneLevelWildcardMatchForCertSelection, this.UtcNow, this.ServerState.CertificateCache, this.EventLog, this.Tracer, out advertisedTlsCertificate);
			this.AdvertisedTlsCertificate = advertisedTlsCertificate;
		}

		private void SetInitialIdentity()
		{
			this.ResetToAnonymousIdentity();
			if (this.ReceiveConnector.HasExternalAuthoritativeAuthMechanism)
			{
				this.RemoteIdentity = WellKnownSids.ExternallySecuredServers;
				this.RemoteIdentityName = "accepted_domain";
				this.SessionPermissions = SmtpInSessionUtils.GetPermissions(this.AuthzAuthorization, this.RemoteIdentity, this.ReceiveConnectorStub.SecurityDescriptor);
			}
		}

		public void ResetToAnonymousIdentity()
		{
			this.AuthenticatedUser = null;
			this.AuthMethod = MultilevelAuthMechanism.None;
			this.RemoteIdentity = SmtpConstants.AnonymousSecurityIdentifier;
			this.RemoteIdentityName = "anonymous";
			this.SessionPermissions = this.AnonymousPermissions;
			if (this.RemoteWindowsIdentity != null)
			{
				this.RemoteWindowsIdentity.Dispose();
				this.RemoteWindowsIdentity = null;
			}
		}

		public void ResetToPartnerServersIdentity(string domain)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("domain", domain);
			this.AuthenticatedUser = null;
			this.AuthMethod = MultilevelAuthMechanism.None;
			this.RemoteIdentity = WellKnownSids.PartnerServers;
			this.RemoteIdentityName = domain;
			this.SessionPermissions = this.PartnerPermissions;
			if (this.RemoteWindowsIdentity != null)
			{
				this.RemoteWindowsIdentity.Dispose();
				this.RemoteWindowsIdentity = null;
			}
		}

		public void SetAuthenticatedIdentity(TransportMiniRecipient authenticatedUser, SecurityIdentifier remoteIdentity, string remoteIdentityName, WindowsIdentity remoteWindowsIdentity, MultilevelAuthMechanism multilevelAuthMechanism, Permission permissions)
		{
			ArgumentValidator.ThrowIfNull("remoteIdentity", remoteIdentity);
			ArgumentValidator.ThrowIfNullOrEmpty("remoteIdentityName", remoteIdentityName);
			ArgumentValidator.ThrowIfNull("remoteWindowsIdentity", remoteWindowsIdentity);
			this.AuthenticatedUser = authenticatedUser;
			this.RemoteIdentity = remoteIdentity;
			this.RemoteIdentityName = remoteIdentityName;
			this.RemoteWindowsIdentity = remoteWindowsIdentity;
			this.AuthMethod = multilevelAuthMechanism;
			this.SessionPermissions = permissions;
		}

		public void UpdateIdentityBasedOnClientTlsCertificate(IX509Certificate2 clientTlsCertificate)
		{
			ArgumentValidator.ThrowIfNull("clientTlsCertificate", clientTlsCertificate);
			this.RemoteIdentity = this.ServerState.DirectTrust.MapCertToSecurityIdentifier(clientTlsCertificate);
			if (this.RemoteIdentity == SmtpConstants.AnonymousSecurityIdentifier)
			{
				this.RemoteIdentityName = "anonymous";
				this.ProtocolLogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "DirectTrust certificate failed to authenticate for '{0}'", new object[]
				{
					clientTlsCertificate.Subject
				});
				this.EventLog.LogEvent(TransportEventLogConstants.Tuple_SmtpReceiveDirectTrustFailed, this.NetworkConnection.RemoteEndPoint.Address.ToString(), new object[]
				{
					clientTlsCertificate.Subject,
					this.NetworkConnection.RemoteEndPoint.Address
				});
			}
			else
			{
				this.RemoteIdentityName = clientTlsCertificate.Subject;
				this.AuthMethod = MultilevelAuthMechanism.DirectTrustTLS;
				this.ProtocolLogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "DirectTrust certificate authenticated as '{0}'", new object[]
				{
					this.RemoteIdentityName
				});
				CertificateExpiryCheck.CheckCertificateExpiry(clientTlsCertificate, this.EventLog, SmtpSessionCertificateUse.RemoteDirectTrust, clientTlsCertificate.Subject, this.UtcNow);
				this.ProtocolLogSession.LogCertificate("Received DirectTrust certificate", clientTlsCertificate);
			}
			this.SessionPermissions = SmtpInSessionUtils.GetPermissions(this.AuthzAuthorization, this.RemoteIdentity, this.ReceiveConnectorStub.SecurityDescriptor);
		}

		private void IncrementMessageCount()
		{
			this.NumberOfMessagesReceived++;
		}

		private bool IsAttributionDataSpecified(MailParseOutput parseOutput)
		{
			return parseOutput.XAttrOrgId != null && parseOutput.Directionality != MailDirectionality.Undefined;
		}

		private TransportMailItem CreateAndInitializeTransportMailItem(MailParseOutput parseOutput, ADRecipientCache<TransportMiniRecipient> recipientCache, MailDirectionality directionality, Guid externalOrgId)
		{
			TransportMailItem transportMailItem = this.NewTransportMailItem(recipientCache, directionality, externalOrgId);
			transportMailItem.DateReceived = DateTime.UtcNow;
			transportMailItem.From = (RoutingAddress.IsEmpty(parseOutput.OriginalFromAddress) ? parseOutput.FromAddress : parseOutput.OriginalFromAddress);
			transportMailItem.ExposeMessage = false;
			transportMailItem.ExposeMessageHeaders = false;
			transportMailItem.PerfCounterAttribution = "SMTPIn";
			transportMailItem.ReceiveConnectorName = "SMTP:" + (this.ReceiveConnector.Name ?? "Unknown");
			transportMailItem.SourceIPAddress = this.NetworkConnection.RemoteEndPoint.Address;
			transportMailItem.AuthMethod = this.AuthMethod;
			if (this.IsAttributionDataSpecified(parseOutput))
			{
				transportMailItem.ExternalOrganizationId = parseOutput.XAttrOrgId.ExternalOrgId;
				transportMailItem.Directionality = parseOutput.Directionality;
				transportMailItem.ExoAccountForest = parseOutput.XAttrOrgId.ExoAccountForest;
				transportMailItem.ExoTenantContainer = parseOutput.XAttrOrgId.ExoTenantContainer;
			}
			return transportMailItem;
		}

		private void TransferMailCommandProperties(TransportMailItem transportMailItem, MailParseOutput parseOutput, MailCommandEventArgs agentEventArgs)
		{
			transportMailItem.Auth = parseOutput.Auth;
			transportMailItem.EnvId = parseOutput.EnvelopeId;
			transportMailItem.DsnFormat = parseOutput.DsnFormat;
			transportMailItem.BodyType = parseOutput.MailBodyType;
			transportMailItem.Oorg = parseOutput.Oorg;
			transportMailItem.InternetMessageId = parseOutput.InternetMessageId;
			this.SetupExpectedBlobs(parseOutput.MessageContextParameters);
			transportMailItem.SystemProbeId = parseOutput.SystemProbeId;
			this.TransferShadowProperties(transportMailItem, parseOutput);
			this.TransferAuthenticationMechanism(transportMailItem);
			transportMailItem.HeloDomain = this.HelloDomain;
			if (agentEventArgs != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in agentEventArgs.MailItemProperties)
				{
					transportMailItem.ExtendedProperties.SetValue<object>(keyValuePair.Key, keyValuePair.Value);
				}
			}
			transportMailItem.ExtendedProperties.SetValue<ulong>("Microsoft.Exchange.Transport.SmtpInSessionId", this.sessionId);
		}

		private void TransferShadowProperties(TransportMailItem transportMailItem, MailParseOutput parseOutput)
		{
			if (string.IsNullOrEmpty(parseOutput.XShadow))
			{
				return;
			}
			transportMailItem.ShadowServerDiscardId = parseOutput.XShadow;
			if (!SmtpInSessionUtils.IsPeerShadowSession(this.PeerSessionPrimaryServer))
			{
				transportMailItem.ShadowServerContext = this.SenderShadowContext;
			}
			if (parseOutput.ShadowMessageId != Guid.Empty)
			{
				transportMailItem.ShadowMessageId = parseOutput.ShadowMessageId;
			}
		}

		private void TransferAuthenticationMechanism(TransportMailItem transportMailItem)
		{
			if (SmtpInSessionUtils.IsPartner(this.RemoteIdentity))
			{
				transportMailItem.AuthMethod = MultilevelAuthMechanism.MutualTLS;
				return;
			}
			if (SmtpInSessionUtils.IsExternalAuthoritative(this.RemoteIdentity))
			{
				transportMailItem.AuthMethod = MultilevelAuthMechanism.SecureExternalSubmit;
				return;
			}
			transportMailItem.AuthMethod = this.AuthMethod;
		}

		private void AddRecipientCacheEntry(ADRecipientCache<TransportMiniRecipient> recipientCache)
		{
			ProxyAddress proxyAddress = new SmtpProxyAddress(this.AuthenticatedUser.PrimarySmtpAddress.ToString(), true);
			Result<TransportMiniRecipient> result = new Result<TransportMiniRecipient>(this.AuthenticatedUser, null);
			recipientCache.AddCacheEntry(proxyAddress, result);
		}

		private void HandleShadowMessageChecks(TransportMailItem transportMailItem)
		{
			if (!SmtpInSessionUtils.IsShadowedBySender(this.SenderShadowContext) && this.ReceiveConnector.MaxAcknowledgementDelay > TimeSpan.Zero && this.ServerState.ShadowRedundancyManager != null && this.ServerState.ShadowRedundancyManager.ShouldDelayAck())
			{
				this.Tracer.TraceDebug<long>(0L, "SmtpInSession(id={0}).HandleShadowMessageChecks: Message stamped as a delayed ack message.", this.NetworkConnection.ConnectionId);
				transportMailItem.ShadowServerContext = "$localhost$";
				transportMailItem.ShadowServerDiscardId = transportMailItem.ShadowMessageId.ToString();
				this.DelayedAckStatus = DelayedAckStatus.Stamped;
				return;
			}
			if (SmtpInSessionUtils.IsPeerShadowSession(this.PeerSessionPrimaryServer) && this.ServerState.ShadowRedundancyManager != null)
			{
				transportMailItem.ShadowServerContext = this.ServerState.ShadowRedundancyManager.GetShadowContextForInboundSession();
				transportMailItem.ShadowServerDiscardId = transportMailItem.ShadowMessageId.ToString();
				return;
			}
			this.DelayedAckStatus = DelayedAckStatus.None;
		}

		private void TraceAndLogSessionPermissions()
		{
			if (this.Tracer != null)
			{
				this.Tracer.TraceDebug<string, Permission>((long)this.GetHashCode(), "Client '{0}' is granted the following permissions: {1}", this.RemoteIdentityName ?? "anonymous", this.SessionPermissions);
			}
			this.ProtocolLogSession.LogInformation(ProtocolLoggingLevel.Verbose, Util.AsciiStringToBytes(Util.GetPermissionString(this.SessionPermissions)), "Set Session Permissions");
		}

		private void UpdateSmtpReceivePerfCountersForMessageReceived(int recipients, long messageBytes)
		{
			ISmtpReceivePerfCounters receivePerfCounters = this.ReceivePerfCounters;
			if (receivePerfCounters != null)
			{
				receivePerfCounters.MessagesReceivedTotal.Increment();
				receivePerfCounters.RecipientsAccepted.IncrementBy((long)recipients);
				receivePerfCounters.MessageBytesReceivedTotal.IncrementBy(messageBytes);
			}
		}

		private void ThrowIfNotTls()
		{
			if (!this.NetworkConnection.IsTls)
			{
				throw new InvalidOperationException("Method can only be invoked for TLS session.");
			}
		}

		private SmtpInSessionState(ISmtpInSession session)
		{
			this.isInvokedFromLegacyStack = true;
			this.ServerState = new SmtpInServerState(session.SmtpInServer);
			this.AdvertisedEhloOptions = session.AdvertisedEhloOptions;
			this.AdvertisedTlsCertificate = ((session.AdvertisedTlsCertificate == null) ? null : new X509Certificate2Wrapper(session.AdvertisedTlsCertificate));
			this.AuthenticatedUser = session.AuthUserRecipient;
			this.authenticationSourceForAgents = session.AuthenticationSourceForAgents;
			this.AuthMethod = session.AuthMethod;
			this.AuthzAuthorization = session.AuthzAuthorization;
			this.CertificateValidator = session.SmtpInServer.CertificateValidator;
			this.Configuration = session.SmtpInServer.ReceiveConfiguration;
			this.EventLog = new ExEventLogWrapper(session.EventLogger);
			this.EventNotificationItem = session.SmtpInServer.EventNotificationItem;
			this.FirstBdatCall = !session.IsBdatOngoing;
			this.RequestClientTlsCertificate = session.ForceRequestClientTlsCertificate;
			this.InternalTransportCertificate = ((session.InternalTransportCertificate == null) ? null : new X509Certificate2Wrapper(session.InternalTransportCertificate));
			this.isDiscardingMessage = session.DiscardingMessage;
			this.IsExternalConnection = !session.IsTrustedIP(session.ClientEndPoint.Address);
			this.MailItemPermissionsGranted = session.MailItemPermissionsGranted;
			this.MailItemPermissionsDenied = session.MailItemPermissionsDenied;
			this.MessageContextBlob = session.MessageContextBlob;
			this.MessageThrottlingManager = session.MessageThrottlingManager;
			this.MessageWriteStream = session.MessageWriteStream;
			this.MexRuntimeSession = session.MexSession;
			this.NetworkConnection = session.NetworkConnection;
			this.NumberOfMessagesReceived = session.NumberOfMessagesReceived;
			this.PeerSessionPrimaryServer = session.PeerSessionPrimaryServer;
			this.ProtocolLogSession = session.LogSession;
			this.ReceiveConnector = session.Connector;
			this.ReceiveConnectorStub = session.ConnectorStub;
			this.RecipientCorrelator = session.RecipientCorrelator;
			this.RemoteIdentity = session.RemoteIdentity;
			this.RemoteIdentityName = session.RemoteIdentityName;
			this.SecureState = session.SecureState;
			this.SenderShadowContext = session.SenderShadowContext;
			this.sessionPermissions = session.SessionPermissions;
			this.SessionStartTime = session.SessionStartTime;
			this.SmtpAgentSession = session.AgentSession;
			this.startTlsDisabled = session.DisableStartTls;
			this.SmtpUtf8Supported = session.SmtpUtf8Supported;
			this.supportIntegratedAuth = session.SupportIntegratedAuth;
			this.TlsDomainCapabilities = session.TlsDomainCapabilities;
			this.TlsRemoteCertificateInternal = ((session.TlsRemoteCertificate == null) ? null : new X509Certificate2Wrapper(session.TlsRemoteCertificate));
			this.Tracer = session.Tracer;
			this.TransportMailItem = session.TransportMailItem;
		}

		public DisposeTracker GetDisposeTracker()
		{
			return this.InternalGetDisposeTracker();
		}

		public virtual void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		public virtual void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		internal override void DiscardMessage(SmtpResponse response, string sourceContext)
		{
			if (response.SmtpResponseType != SmtpResponseType.Success)
			{
				throw new InvalidOperationException("Response provided must be a success (2xx) one. If you want to reject, call RejectMessage instead");
			}
			this.SmtpResponse = response;
			SmtpSessionHelper.DiscardMessage(sourceContext, this.ExecutionControl, this.TransportMailItem, this.ProtocolLogSession, this.messageTrackingLogWrapper);
		}

		internal override void Disconnect()
		{
			this.ShouldDisconnect = true;
			ISmtpReceivePerfCounters receivePerfCounters = this.ReceivePerfCounters;
			if (receivePerfCounters != null)
			{
				receivePerfCounters.ConnectionsDroppedByAgentsTotal.Increment();
			}
			this.ExecutionControl.HaltExecution();
		}

		internal override void RejectMessage(SmtpResponse response)
		{
			this.RejectMessage(response, null);
		}

		internal override void RejectMessage(SmtpResponse response, string sourceContext)
		{
			this.SmtpResponse = response;
			SmtpSessionHelper.RejectMessage(response, sourceContext, this.ExecutionControl, this.TransportMailItem, this.LocalEndPoint, this.RemoteEndPoint, this.sessionId, this.ReceiveConnector, this.ProtocolLogSession, this.messageTrackingLogWrapper);
		}

		internal override CertificateValidationStatus ValidateCertificate()
		{
			this.ThrowIfNotTls();
			return SmtpSessionHelper.ConvertChainValidityStatusToCertValidationStatus(this.TlsRemoteCertificateChainValidationStatus);
		}

		internal override CertificateValidationStatus ValidateCertificate(string domain, out string matchedCertDomain)
		{
			this.ThrowIfNotTls();
			return SmtpSessionHelper.ValidateCertificate(domain, this.TlsRemoteCertificate, this.SecureState, this.TlsRemoteCertificateChainValidationStatus, this.ServerState.CertificateValidator, this.ProtocolLogSession, out matchedCertDomain);
		}

		public override bool AntispamBypass
		{
			get
			{
				return SmtpInSessionUtils.HasSMTPAntiSpamBypassPermission(this.CombinedPermissions);
			}
		}

		internal override IExecutionControl ExecutionControl { get; set; }

		internal override X509Certificate2 TlsRemoteCertificate
		{
			get
			{
				if (this.TlsRemoteCertificateInternal != null)
				{
					return this.TlsRemoteCertificateInternal.Certificate;
				}
				return null;
			}
		}

		internal override bool IsClientProxiedSession { get; set; }

		public override bool IsConnected
		{
			get
			{
				return !this.ShouldDisconnect;
			}
		}

		internal override bool IsInboundProxiedSession { get; set; }

		public override bool IsTls
		{
			get
			{
				return this.NetworkConnection.IsTls;
			}
		}

		public override IPAddress LastExternalIPAddress { get; internal set; }

		public override IPEndPoint LocalEndPoint
		{
			get
			{
				return this.NetworkConnection.LocalEndPoint;
			}
		}

		public override IDictionary<string, object> Properties
		{
			get
			{
				return this.properties;
			}
		}

		internal override string ReceiveConnectorName
		{
			get
			{
				return this.ReceiveConnector.Name;
			}
		}

		public override IPEndPoint RemoteEndPoint { get; internal set; }

		internal override bool ShouldDisconnect { get; set; }

		internal override SmtpResponse SmtpResponse { get; set; }

		internal override bool XAttrAdvertised
		{
			get
			{
				return this.AdvertisedEhloOptions.XAttr;
			}
		}

		private static readonly Queue<IInboundMessageContextBlob> EmptyInboundMessageContextBlobs = new Queue<IInboundMessageContextBlob>(0);

		private readonly bool isInvokedFromLegacyStack;

		private readonly AuthenticationSource authenticationSourceForAgents;

		private readonly MessageTrackingLogWrapper messageTrackingLogWrapper = new MessageTrackingLogWrapper();

		private readonly IDictionary<string, object> properties = new Dictionary<string, object>();

		private readonly ulong sessionId;

		private bool isDiscardingMessage;

		private bool disposed;

		private DisposeTracker disposeTracker;

		private string advertisedDomain;

		private Permission? anonymousPermissions;

		private Permission? partnerPermissions;

		public Permission sessionPermissions;

		private ChainValidityStatus? tlsRemoteCertificateChainValidationStatus;

		private object lastNetworkError;

		protected bool startTlsDisabled;

		protected bool supportIntegratedAuth;
	}
}
