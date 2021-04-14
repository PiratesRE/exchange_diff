using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security;
using System.Security.AccessControl;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Internal;
using Microsoft.Exchange.Data.Transport;
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
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.Transport.Categorizer;
using Microsoft.Exchange.Transport.Logging;
using Microsoft.Exchange.Transport.ShadowRedundancy;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class SmtpOutSession : ISmtpSession, ISmtpOutSession
	{
		public SmtpOutSession(ulong sessionId, SmtpOutConnection smtpOutConnection, NextHopConnection nextHopConnection, IPEndPoint target, ProtocolLog protocolLog, ProtocolLoggingLevel loggingLevel, IMailRouter mailRouter, CertificateCache certificateCache, CertificateValidator certificateValidator, ShadowRedundancyManager shadowRedundancyManager, TransportAppConfig transportAppConfig, ITransportConfiguration transportConfiguration, bool isProbeSession = false)
		{
			if (smtpOutConnection == null)
			{
				throw new ArgumentNullException("smtpOutConnection");
			}
			if (nextHopConnection == null)
			{
				throw new ArgumentNullException("nextHopConnection");
			}
			if (smtpOutConnection.Connector == null)
			{
				throw new ArgumentException("Outbound Smtp Connection has a null connector.", "nextHopConnection");
			}
			this.smtpOutConnection = smtpOutConnection;
			this.nextHopConnection = nextHopConnection;
			this.sendConnector = smtpOutConnection.Connector;
			this.sessionPermissions = this.sendConnector.GetAnonymousPermissions();
			this.smtpSendPerformanceCounters = smtpOutConnection.SmtpSendPerformanceCounters;
			this.mailRouter = mailRouter;
			this.certificateCache = certificateCache;
			this.certificateValidator = certificateValidator;
			this.TlsConfiguration = smtpOutConnection.TlsConfig;
			this.shadowRedundancyManager = shadowRedundancyManager;
			this.transportAppConfig = transportAppConfig;
			this.transportConfiguration = transportConfiguration;
			this.connectorPassword = this.sendConnector.GetSmartHostPassword();
			this.response = new List<string>(50);
			this.pipelinedCommandQueue = new Queue();
			this.pipelinedResponseQueue = new Queue();
			this.sessionProps = new SmtpSessionProps(sessionId);
			this.commandLists = new SmtpOutSession.CommandList[24];
			for (int i = 0; i < 24; i++)
			{
				this.commandLists[i] = new SmtpOutSession.CommandList((SmtpOutSession.SessionState)i);
			}
			this.CurrentState = SmtpOutSession.SessionState.ConnectResponse;
			this.EnqueueResponseHandler(null);
			this.sessionProps.AdvertisedEhloOptions = new EhloOptions();
			SmtpDomain fqdn = this.Connector.Fqdn;
			this.sessionProps.HelloDomain = ((fqdn != null && !string.IsNullOrEmpty(fqdn.Domain)) ? fqdn.Domain : ComputerInformation.DnsPhysicalFullyQualifiedDomainName);
			this.sessionProps.RemoteEndPoint = target;
			this.shadowRedundancyEnabled = (this.shadowRedundancyManager != null && this.shadowRedundancyManager.Configuration.Enabled);
			this.useDowngradedExchangeServerAuth = this.transportConfiguration.LocalServer.TransportServer.UseDowngradedExchangeServerAuth;
			this.LoadCertificate();
			string connectorId;
			if (smtpOutConnection.NextHopIsOutboundProxy)
			{
				IEnumerable<INextHopServer> enumerable;
				TlsSendConfiguration tlsSendConfiguration;
				RiskLevel riskLevel;
				int num;
				smtpOutConnection.GetOutboundProxyDestinationSettings(out enumerable, out this.outboundProxySendConnector, out tlsSendConfiguration, out riskLevel, out num);
				connectorId = this.outboundProxySendConnector.Id.ToString();
			}
			else
			{
				connectorId = this.sendConnector.Id.ToString();
			}
			this.logSession = protocolLog.OpenSession(connectorId, this.SessionId, target, null, loggingLevel);
			this.SetDefaultIdentity();
			if (this.NextHopConnection.Key.NextHopType == NextHopType.Heartbeat)
			{
				this.dontCacheThisConnection = true;
			}
			else if (this.nextHopConnection.Key.NextHopType == NextHopType.ShadowRedundancy && !this.transportAppConfig.ConnectionCacheConfig.EnableShadowConnectionCache)
			{
				this.dontCacheThisConnection = true;
			}
			this.logSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "attempting to connect");
			ExTraceGlobals.SmtpSendTracer.TraceDebug<IPEndPoint>((long)this.GetHashCode(), "Attempting to connect to {0}", target);
			SmtpOutConnection.Events.LogEvent(TransportEventLogConstants.Tuple_SmtpSendNewSession, null, new object[]
			{
				this.sendConnector.Name,
				target
			});
			this.TlsConfiguration.LogTlsOverride(this.logSession);
			this.isProbeSession = isProbeSession;
		}

		protected SmtpOutSession()
		{
		}

		public ulong SessionId
		{
			get
			{
				return this.sessionProps.SessionId;
			}
		}

		public IPEndPoint RemoteEndPoint
		{
			get
			{
				return this.sessionProps.RemoteEndPoint;
			}
		}

		public IPEndPoint LocalEndPoint
		{
			get
			{
				return this.sessionProps.LocalEndPoint;
			}
		}

		public virtual DateTime SessionStartTime
		{
			get
			{
				return this.sessionStartTime;
			}
		}

		public string HelloDomain
		{
			get
			{
				return this.sessionProps.HelloDomain;
			}
		}

		public SmtpResponse Banner
		{
			get
			{
				return this.sessionProps.Banner;
			}
			set
			{
				this.sessionProps.Banner = value;
			}
		}

		public IEhloOptions AdvertisedEhloOptions
		{
			get
			{
				return this.sessionProps.AdvertisedEhloOptions;
			}
		}

		public bool SupportLongAddresses
		{
			get
			{
				return this.AdvertisedEhloOptions.XLongAddr || this.SupportExch50;
			}
		}

		public bool SupportOrar
		{
			get
			{
				return this.AdvertisedEhloOptions.XOrar || this.SupportExch50;
			}
		}

		public bool SupportRDst
		{
			get
			{
				return this.AdvertisedEhloOptions.XRDst || this.SupportExch50;
			}
		}

		public bool SupportSmtpUtf8
		{
			get
			{
				return this.AdvertisedEhloOptions.SmtpUtf8 && (this.AdvertisedEhloOptions.EightBitMime || this.AdvertisedEhloOptions.BinaryMime);
			}
		}

		public virtual bool SupportExch50
		{
			get
			{
				return this.transportConfiguration.TransportSettings.TransportSettings.Xexch50Enabled && this.AdvertisedEhloOptions.Xexch50 && (this.Permissions & Permission.SMTPSendEXCH50) > Permission.None;
			}
		}

		public bool ShouldSendExch50blob
		{
			get
			{
				return this.SupportExch50 && (this.exch50DataPresent || this.NextHopDeliveryType == DeliveryType.SmtpRelayToTiRg || this.NextHopType.IsSmtpConnectorDeliveryType);
			}
		}

		public virtual bool SendShadow
		{
			get
			{
				return this.shadowRedundancyEnabled && this.shadowRedundancyManager.ShouldSmtpOutSendXShadow(this.Permissions, this.NextHopDeliveryType, this.AdvertisedEhloOptions, this.Connector);
			}
		}

		public virtual bool SendXShadowRequest
		{
			get
			{
				return false;
			}
		}

		public virtual bool SendXQDiscard
		{
			get
			{
				return this.Shadowed && this.shadowRedundancyManager != null && this.shadowRedundancyManager.ShouldSmtpOutSendXQDiscard(this.SmtpHost);
			}
		}

		public virtual bool ShouldReduceRecipientCacheForTransmission
		{
			get
			{
				return this.transportConfiguration.ProcessTransportRole == ProcessTransportRole.Hub;
			}
		}

		public bool Shadowed
		{
			get
			{
				return this.shadowed;
			}
			set
			{
				this.shadowed = value;
			}
		}

		public string SmtpHost
		{
			get
			{
				return this.smtpOutConnection.SmtpHost;
			}
		}

		public Permission Permissions
		{
			get
			{
				return this.sessionPermissions;
			}
		}

		public bool HasTlsClientCertificate
		{
			get
			{
				return this.advertisedTlsCertificate != null;
			}
		}

		public IProtocolLogSession LogSession
		{
			get
			{
				return this.logSession;
			}
		}

		public string NextHopDomain
		{
			get
			{
				return this.nextHopConnection.Key.NextHopDomain;
			}
		}

		public DeliveryType NextHopDeliveryType
		{
			get
			{
				return this.NextHopType.DeliveryType;
			}
		}

		public NextHopType NextHopType
		{
			get
			{
				return this.NextHopConnection.Key.NextHopType;
			}
		}

		public MultilevelAuthMechanism AuthMethod
		{
			get
			{
				return this.authMethod;
			}
			set
			{
				this.authMethod = value;
			}
		}

		public bool IsAuthenticated
		{
			get
			{
				return this.isAuthenticated;
			}
			set
			{
				this.isAuthenticated = value;
			}
		}

		public bool RemoteIsAuthenticated
		{
			get
			{
				return !this.RemoteIdentity.IsWellKnown(WellKnownSidType.AnonymousSid);
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

		public RestrictedHeaderSet RestrictedHeaderSet
		{
			get
			{
				RestrictedHeaderSet restrictedHeaderSet = RestrictedHeaderSet.None;
				if ((this.Permissions & Permission.SendRoutingHeaders) == Permission.None)
				{
					restrictedHeaderSet |= RestrictedHeaderSet.MTA;
				}
				if ((this.Permissions & Permission.SendForestHeaders) == Permission.None)
				{
					restrictedHeaderSet |= RestrictedHeaderSet.Forest;
				}
				if ((this.Permissions & Permission.SendOrganizationHeaders) == Permission.None)
				{
					restrictedHeaderSet |= RestrictedHeaderSet.Organization;
				}
				return restrictedHeaderSet;
			}
		}

		public SmtpSessionProps SessionProps
		{
			get
			{
				return this.sessionProps;
			}
		}

		public NextHopConnection NextHopConnection
		{
			get
			{
				return this.nextHopConnection;
			}
		}

		public AckDetails AckDetails
		{
			get
			{
				return this.ackDetails;
			}
		}

		public IReadOnlyMailItem RoutedMailItem
		{
			get
			{
				return this.routedMailItem;
			}
		}

		public virtual bool ShadowCurrentMailItem
		{
			get
			{
				return this.shadowCurrentMailItem;
			}
		}

		public MailRecipient CurrentRecipient
		{
			get
			{
				return this.currentRecipient;
			}
			set
			{
				this.currentRecipient = value;
			}
		}

		public MailRecipient NextRecipient
		{
			get
			{
				return this.nextRecipient;
			}
			set
			{
				this.nextRecipient = value;
			}
		}

		public int NumberOfRecipientsAttempted
		{
			get
			{
				return this.numberOfRecipientsAttempted;
			}
			set
			{
				this.numberOfRecipientsAttempted = value;
			}
		}

		public int NumberOfRecipientsSucceeded
		{
			get
			{
				return this.numberOfRecipientsSucceeded;
			}
		}

		public int NumberOfRecipientsAckedForRetry
		{
			get
			{
				return this.numberOfRecipientsAckedForRetry;
			}
		}

		public int NumberOfRecipientsAcked
		{
			get
			{
				return this.numberOfRecipientsAcked;
			}
			set
			{
				this.numberOfRecipientsAcked = value;
			}
		}

		public bool BetweenMessagesRset
		{
			get
			{
				return this.betweenMessagesRset;
			}
			set
			{
				this.betweenMessagesRset = value;
			}
		}

		public SecureState SecureState
		{
			get
			{
				return this.secureState;
			}
		}

		public SmtpOutSession.SessionState NextState
		{
			get
			{
				return this.nextState;
			}
			set
			{
				this.nextState = value;
			}
		}

		public bool RecipientsAckedPending
		{
			get
			{
				return this.recipsAckedPending;
			}
			set
			{
				this.recipsAckedPending = value;
			}
		}

		public SmtpSendConnectorConfig Connector
		{
			get
			{
				return this.sendConnector;
			}
			set
			{
				this.sendConnector = value;
			}
		}

		public SmtpSendConnectorConfig.AuthMechanisms AuthMechanism
		{
			get
			{
				return this.Connector.SmartHostAuthMechanism;
			}
		}

		public string AuthenticationUsername
		{
			get
			{
				string authenticationUserName = this.Connector.GetAuthenticationUserName();
				if (!string.IsNullOrEmpty(authenticationUserName))
				{
					return authenticationUserName;
				}
				return null;
			}
		}

		public SecureString AuthenticationPassword
		{
			get
			{
				return this.connectorPassword;
			}
		}

		public bool UsingHELO
		{
			get
			{
				return this.usingHELO;
			}
			set
			{
				this.usingHELO = value;
			}
		}

		public SmtpSendPerfCountersInstance SmtpSendPerformanceCounters
		{
			get
			{
				return this.smtpSendPerformanceCounters;
			}
		}

		public bool NeedToDownConvertMIME
		{
			get
			{
				return this.needToDownconvertMIME;
			}
			set
			{
				this.needToDownconvertMIME = value;
			}
		}

		public bool PipeLineNextMessagePending
		{
			get
			{
				return this.pipeLineNextMessagePending;
			}
		}

		public bool PipeLineFailOverPending
		{
			get
			{
				return this.pipeLineFailOverPending;
			}
		}

		public virtual bool Disconnected
		{
			get
			{
				return this.disconnected;
			}
		}

		public long ConnectionId
		{
			get
			{
				return this.connectionId;
			}
		}

		public string CurrentMessageTemporaryId
		{
			get
			{
				string[] value = new string[]
				{
					this.SessionId.ToString("X16", NumberFormatInfo.InvariantInfo),
					this.sessionStartTime.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffZ", DateTimeFormatInfo.InvariantInfo),
					this.messageCount.ToString()
				};
				return string.Join(";", value);
			}
		}

		public bool IsNextHopDomainSecured
		{
			get
			{
				return this.Connector.DomainSecureEnabled && this.transportConfiguration.TransportSettings.TransportSettings.IsTLSSendSecureDomain(this.NextHopConnection.Key.NextHopDomain);
			}
		}

		public bool RequiresDirectTrust
		{
			get
			{
				return this.AuthMechanism == SmtpSendConnectorConfig.AuthMechanisms.ExchangeServer && (this.sendConnector.IsInitialSendConnector() || this.NextHopDeliveryType == DeliveryType.SmtpRelayWithinAdSiteToEdge || (this.NextHopDeliveryType == DeliveryType.Heartbeat && (!Components.IsBridgehead || !this.IsHubServer(this.NextHopDomain))));
			}
		}

		public bool IsInternalTransportCertificateAvailable
		{
			get
			{
				return this.internalTransportCertificate != null;
			}
		}

		public bool IsOpportunisticTls
		{
			get
			{
				return (this.SecureState == SecureState.AnonymousTls || this.SecureState == SecureState.StartTls) && (!this.TlsConfiguration.RequireTls && this.AuthMechanism != SmtpSendConnectorConfig.AuthMechanisms.BasicAuthRequireTLS && this.AuthMechanism != SmtpSendConnectorConfig.AuthMechanisms.ExchangeServer) && !this.IsNextHopDomainSecured;
			}
		}

		public ulong DiscardIdsReceived
		{
			get
			{
				return this.smtpOutConnection.DiscardIdsReceived;
			}
			set
			{
				this.smtpOutConnection.DiscardIdsReceived = value;
			}
		}

		public bool CanDowngradeExchangeServerAuth
		{
			get
			{
				return this.useDowngradedExchangeServerAuth && (this.NextHopType.IsHubRelayDeliveryType || this.SendingIntraOrgHeartbeatFromHubToHub);
			}
		}

		public bool SendingIntraOrgHeartbeatFromHubToHub
		{
			get
			{
				bool flag = string.Equals(this.Connector.Name, Strings.IntraorgSendConnectorName);
				return this.NextHopDeliveryType == DeliveryType.Heartbeat && flag && Components.IsBridgehead && this.IsHubServer(this.NextHopDomain);
			}
		}

		public Queue<SmtpMessageContextBlob> BlobsToSend
		{
			get
			{
				return this.blobsToSend;
			}
		}

		public RecipientCorrelator RecipientCorrelator
		{
			get
			{
				return this.recipientCorrelator;
			}
		}

		public int MessagesSentOverSession
		{
			get
			{
				return this.messagesSentOverSession;
			}
		}

		public Queue<string> RemainingXProxyToCommands
		{
			get
			{
				return this.remainingXProxyToCommands;
			}
			set
			{
				this.remainingXProxyToCommands = value;
			}
		}

		public bool XRsetProxyToAccepted
		{
			set
			{
				this.xRsetProxyToAccepted = value;
			}
		}

		public bool IsProbeSession
		{
			get
			{
				return this.isProbeSession;
			}
		}

		internal TlsSendConfiguration TlsConfiguration { get; private set; }

		protected virtual bool MessageContextBlobTransferSupported
		{
			get
			{
				return true;
			}
		}

		protected virtual bool SendFewerMessagesToSlowerServerEnabled
		{
			get
			{
				return this.transportAppConfig.SmtpSendConfiguration.SendFewerMessagesToSlowerServerEnabled;
			}
		}

		protected virtual bool FailoverPermittedForRemoteShutdown
		{
			get
			{
				return true;
			}
		}

		private SmtpOutSession.SessionState CurrentState
		{
			get
			{
				return this.currentState;
			}
			set
			{
				this.currentState = value;
			}
		}

		public static bool MatchCertificateWithTlsDomain(IList<SmtpDomainWithSubdomains> tlsDomains, IX509Certificate2 remoteCertificate, IProtocolLogSession protocolLogSession, CertificateValidator certificateValidatorInstance)
		{
			foreach (SmtpDomainWithSubdomains domain in tlsDomains)
			{
				if (certificateValidatorInstance.MatchCertificateFqdns(domain, remoteCertificate, MatchOptions.None, protocolLogSession))
				{
					return true;
				}
			}
			return false;
		}

		public static X509Certificate2 LoadTlsCertificate(TlsSendConfiguration tlsConfiguration, CertificateCache cache, bool oneLevelWildcardMatch, string connectorName, int hashCode)
		{
			if (tlsConfiguration.ShouldSkipTls)
			{
				return null;
			}
			string text;
			X509Certificate2 x509Certificate;
			if (tlsConfiguration.TlsCertificateName != null)
			{
				text = tlsConfiguration.TlsCertificateName.ToString();
				x509Certificate = cache.Find(tlsConfiguration.TlsCertificateName);
			}
			else
			{
				text = (string.IsNullOrEmpty(tlsConfiguration.TlsCertificateFqdn) ? ComputerInformation.DnsPhysicalFullyQualifiedDomainName : tlsConfiguration.TlsCertificateFqdn);
				List<string> names = new List<string>
				{
					text
				};
				x509Certificate = cache.Find(names, false, WildcardMatchType.OneLevel);
				if (x509Certificate == null)
				{
					x509Certificate = cache.Find(names, true, oneLevelWildcardMatch ? WildcardMatchType.OneLevel : WildcardMatchType.MultiLevel);
				}
			}
			if (x509Certificate == null)
			{
				string text2 = string.Format("Can't load STARTTLS certificate for {0}", text);
				ExTraceGlobals.SmtpSendTracer.TraceError((long)hashCode, text2);
				SmtpOutConnection.Events.LogEvent(TransportEventLogConstants.Tuple_CannotLoadSTARTTLSCertificateFromStore, connectorName, new object[]
				{
					text,
					connectorName
				});
				EventNotificationItem.PublishPeriodic(ExchangeComponent.Transport.Name, "CannotLoadSTARTTLSCertificateFromStore", null, string.Format("Connector: '{0}' Error: '{1}'", connectorName, text2), "CannotLoadSTARTTLSCertificateFromStore", TimeSpan.FromMinutes(5.0), ResultSeverityLevel.Error, false);
				if (tlsConfiguration.TlsCertificateName != null)
				{
					throw new TlsCertificateNameNotFoundException(tlsConfiguration.TlsCertificateName.ToString(), connectorName);
				}
			}
			else if ((tlsConfiguration.TlsAuthLevel == null || tlsConfiguration.TlsAuthLevel.Value != RequiredTlsAuthLevel.EncryptionOnly) && CertificateExpiryCheck.CheckCertificateExpiry(x509Certificate, SmtpOutConnection.Events, SmtpSessionCertificateUse.STARTTLS, text))
			{
				ExTraceGlobals.SmtpSendTracer.TraceError<string>((long)hashCode, "STARTTLS certificate for {0} for outbound session is expired", text);
			}
			return x509Certificate;
		}

		public void ResetAdvertisedEhloOptions()
		{
			this.sessionProps.AdvertisedEhloOptions = new EhloOptions();
		}

		public virtual void ResetSession(SmtpOutConnection smtpOutConnection, NextHopConnection nextHopConnection)
		{
			if (smtpOutConnection == null)
			{
				throw new ArgumentNullException("smtpOutConnection");
			}
			if (nextHopConnection == null)
			{
				throw new ArgumentNullException("nextHopConnection");
			}
			this.DropBreadcrumb(SmtpOutSession.SmtpOutSessionBreadcrumbs.ResetSession);
			this.presentInSmtpOutSessionCache = false;
			this.smtpOutConnection.RemoveConnection();
			this.messagesSentOverSession = 0;
			this.smtpOutConnection = smtpOutConnection;
			this.nextHopConnection = nextHopConnection;
			if (this.smtpOutConnection.NextHopIsOutboundProxy)
			{
				IEnumerable<INextHopServer> enumerable;
				TlsSendConfiguration tlsSendConfiguration;
				RiskLevel riskLevel;
				int num;
				smtpOutConnection.GetOutboundProxyDestinationSettings(out enumerable, out this.outboundProxySendConnector, out tlsSendConfiguration, out riskLevel, out num);
				string connectorId = this.outboundProxySendConnector.Id.ToString();
				this.logSession = smtpOutConnection.ProtocolLog.OpenSession(connectorId, this.SessionId, this.RemoteEndPoint, null, this.outboundProxySendConnector.ProtocolLoggingLevel);
			}
		}

		public bool CheckDomainSecure<T>(bool condition, string trace, ExEventLog.EventTuple log, T additionalInformation)
		{
			string nextHopDomain = this.nextHopConnection.Key.NextHopDomain;
			if (this.transportConfiguration.TransportSettings.TransportSettings.IsTLSSendSecureDomain(nextHopDomain))
			{
				SmtpResponse smtpResponse = SmtpResponse.RequireTLSToSendMail;
				if (!this.Connector.DomainSecureEnabled)
				{
					trace = "DomainSecureEnabled was set to false";
					log = TransportEventLogConstants.Tuple_TlsDomainSecureDisabled;
					smtpResponse = SmtpResponse.DomainSecureDisabled;
				}
				else if (condition)
				{
					return true;
				}
				ExTraceGlobals.SmtpSendTracer.TraceError((long)this.GetHashCode(), "Message to secure domain '{0}' on send connector '{1}' failed because {2} {3}.", new object[]
				{
					nextHopDomain,
					this.Connector.Name,
					trace,
					additionalInformation
				});
				SmtpCommand.EventLogger.LogEvent(log, nextHopDomain + "-" + this.Connector.Name, new object[]
				{
					nextHopDomain,
					this.Connector.Name,
					additionalInformation
				});
				string context = string.Format(CultureInfo.InvariantCulture, "Message to secure domain '{0}' on send connector '{1}' failed because {2} {3}.", new object[]
				{
					nextHopDomain,
					this.Connector.Name,
					trace,
					additionalInformation
				});
				this.LogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, context);
				this.FailoverConnection(smtpResponse, SessionSetupFailureReason.ProtocolError);
				Utils.SecureMailPerfCounters.DomainSecureOutboundSessionFailuresTotal.Increment();
				this.NextState = SmtpOutSession.SessionState.Quit;
				return false;
			}
			return true;
		}

		public bool CheckRequireOorg()
		{
			if (!this.Connector.RequireOorg || this.AdvertisedEhloOptions.XOorg)
			{
				return true;
			}
			ExTraceGlobals.SmtpSendTracer.TraceError<IPEndPoint, string, string>((long)this.GetHashCode(), "Connection to remote endpoint '{0} ({1})' for send connector '{2}' will be dropped because the server did not advertise XOORG.", this.connection.RemoteEndPoint, this.smtpOutConnection.SmtpHostName, this.Connector.Name);
			SmtpCommand.EventLogger.LogEvent(TransportEventLogConstants.Tuple_SessionFailedBecauseXOorgNotOffered, this.Connector.Name + "-" + this.connection.RemoteEndPoint, new object[]
			{
				this.connection.RemoteEndPoint,
				this.smtpOutConnection.SmtpHostName,
				this.Connector.Name
			});
			string context = string.Format(CultureInfo.InvariantCulture, "Connection to remote endpoint '{0} ({1})' for send connector '{2}' will be dropped because the server did not advertise XOORG.", new object[]
			{
				this.connection.RemoteEndPoint,
				this.smtpOutConnection.SmtpHostName,
				this.Connector.Name
			});
			this.LogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, context);
			this.FailoverConnection(SmtpResponse.RequireXOorgToSendMail, SessionSetupFailureReason.ProtocolError);
			this.NextState = SmtpOutSession.SessionState.Quit;
			return false;
		}

		public void Disconnect()
		{
			this.Disconnect(DisconnectReason.Local, false, false, null, SessionSetupFailureReason.None);
		}

		public byte[] GetTlsEapKey()
		{
			return this.connection.TlsEapKey;
		}

		public byte[] GetCertificatePublicKey()
		{
			return this.connection.RemoteCertificate.GetPublicKey();
		}

		public virtual void ShutdownConnection()
		{
			this.DropBreadcrumb(SmtpOutSession.SmtpOutSessionBreadcrumbs.ShutdownConnection);
			if (this.connection != null)
			{
				this.connection.Shutdown();
			}
			this.shutdownConnectionCalled = true;
		}

		public RoutingAddress GetShortAddress(RoutingAddress p1Address)
		{
			if (!Util.IsLongAddressForE2k3(p1Address))
			{
				return p1Address;
			}
			if (this.AdvertisedEhloOptions.XLongAddr || !this.SupportExch50)
			{
				return p1Address;
			}
			RoutingAddress result;
			if (Util.TryGetShortAddress(p1Address, out result))
			{
				this.exch50DataPresent = true;
				ExTraceGlobals.SmtpSendTracer.TraceDebug<string, string>((long)this.GetHashCode(), "Replaced long address {0} with short address {1}", p1Address.ToString(), result.ToString());
				return result;
			}
			ExTraceGlobals.SmtpSendTracer.TraceDebug<string>((long)this.GetHashCode(), "Address {0} was not converted to a short form", p1Address.ToString());
			return p1Address;
		}

		public void ConnectionCompleted(NetworkConnection networkConnection)
		{
			this.connection = networkConnection;
			this.connectionId = networkConnection.ConnectionId;
			this.connection.MaxLineLength = 2000;
			this.logSession.LocalEndPoint = this.connection.LocalEndPoint;
			this.connection.Timeout = (int)this.Connector.ConnectionInactivityTimeOut.TotalSeconds;
			this.ackDetails = new AckDetails(this.connection.RemoteEndPoint, this.smtpOutConnection.SmtpHostName, this.SessionId.ToString("X16", NumberFormatInfo.InvariantInfo), this.Connector.Id.ToString(), this.connection.LocalEndPoint.Address);
			this.AckDetails.AddEventData("Microsoft.Exchange.Transport.MailRecipient.RequiredTlsAuthLevel", SmtpOutSession.AuthLevelToString(this.TlsConfiguration.TlsAuthLevel));
		}

		public void StartUsingConnection()
		{
			this.sessionStartTime = DateTime.UtcNow;
			this.IncrementConnectionCounters();
			this.logSession.LogConnect();
			this.StartReadLine();
		}

		public string GetConnectionInfo()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("LastIndex : ");
			stringBuilder.Append(this.breadcrumbs.LastFilledIndex);
			stringBuilder.AppendLine();
			stringBuilder.AppendLine("Session BreadCrumb : ");
			for (int i = 0; i < 64; i++)
			{
				stringBuilder.Append(Enum.Format(typeof(SmtpOutSession.SmtpOutSessionBreadcrumbs), this.breadcrumbs.BreadCrumb[i], "x"));
				stringBuilder.Append(" ");
			}
			stringBuilder.AppendLine();
			if (this.connection != null)
			{
				stringBuilder.Append(this.connection.GetBreadCrumbsInfo());
			}
			else
			{
				stringBuilder.AppendLine("connection = null");
			}
			return stringBuilder.ToString();
		}

		public virtual void PrepareForNextMessageOnCachedSession()
		{
			this.DropBreadcrumb(SmtpOutSession.SmtpOutSessionBreadcrumbs.PrepareForNextMessageOnCachedSession);
			if (this.smtpOutConnection.NextHopIsOutboundProxy)
			{
				this.nextState = SmtpOutSession.SessionState.XProxyTo;
				this.MoveToNextState();
				return;
			}
			this.PrepareForNextMessage(true);
			this.EnqueueNextPipeLinedCommands();
			this.SendNextCommands();
		}

		public void FailoverConnection(SmtpResponse smtpResponse)
		{
			this.FailoverConnection(smtpResponse, SessionSetupFailureReason.ProtocolError);
		}

		public void FailoverConnection(SmtpResponse smtpResponse, SessionSetupFailureReason failoverReason)
		{
			this.FailoverConnection(smtpResponse, true, false, failoverReason);
		}

		public void GetOutboundProxyDestinationSettings(out IEnumerable<INextHopServer> destinations, out SmtpSendConnectorConfig sendConnector, out TlsSendConfiguration tlsSendConfiguration, out RiskLevel riskLevel, out int outboundIPPool)
		{
			this.smtpOutConnection.GetOutboundProxyDestinationSettings(out destinations, out sendConnector, out tlsSendConfiguration, out riskLevel, out outboundIPPool);
			this.outboundProxySendConnector = sendConnector;
		}

		public void OutboundProxyConnectionEstablished(SmtpResponse xProxyToResponse, IPAddress proxyTargetIPAddress, string proxyTargetHostName, IEnumerable<INextHopServer> remainingDestinations, bool shouldSkipTls, int precedingXProxyToSpecificLines)
		{
			if (this.outboundProxySendConnector == null)
			{
				throw new InvalidOperationException("OutboundProxySendConnector should not be null");
			}
			this.nextHopIsProxyingBlindly = true;
			this.smtpOutConnection.UpdateOnSuccessfulOutboundProxySetup(remainingDestinations, shouldSkipTls);
			this.outboundProxyOriginalSessionState = new SmtpOutSession.OutboundProxyOriginalSessionState(this.sessionProps.AdvertisedEhloOptions, this.sendConnector, this.RemoteIdentity, this.RemoteIdentityName, this.AckDetails, this.sessionPermissions);
			this.sessionProps.AdvertisedEhloOptions = new EhloOptions();
			this.sessionProps.AdvertisedEhloOptions.ParseResponse(xProxyToResponse, this.RemoteEndPoint.Address, precedingXProxyToSpecificLines);
			if (!this.AdvertisedEhloOptions.Dsn)
			{
				ExTraceGlobals.SmtpSendTracer.TraceDebug((long)this.GetHashCode(), "Remote server does not support DSN. Relay DSNs will be generated for successful recipients as needed.");
				this.NextHopConnection.GenerateSuccessDSNs = DsnFlags.Relay;
			}
			this.sendConnector = this.outboundProxySendConnector;
			this.RemoteIdentity = SmtpOutSession.anonymousSecurityIdentifier;
			this.RemoteIdentityName = "anonymous";
			this.SetSessionPermissions(this.RemoteIdentity);
			this.SetDefaultIdentity();
			this.ackDetails = new AckDetails(this.connection.RemoteEndPoint, this.smtpOutConnection.SmtpHostName, this.SessionId.ToString("X16", NumberFormatInfo.InvariantInfo), this.Connector.Id.ToString(), this.connection.LocalEndPoint.Address);
			this.ackDetails.AddEventData("OutboundProxyTargetIPAddress", proxyTargetIPAddress.ToString());
			if (!string.IsNullOrEmpty(proxyTargetHostName))
			{
				this.ackDetails.AddEventData("OutboundProxyTargetHostName", proxyTargetHostName);
			}
			if (!string.IsNullOrEmpty(this.helloDomainOfOutboundProxyFrontEnd))
			{
				this.ackDetails.AddEventData("OutboundProxyFrontendName", this.helloDomainOfOutboundProxyFrontEnd);
			}
			string context = "This session is now being proxied through the next hop. The actual connector being used is: " + this.sendConnector.Name;
			this.logSession.LogInformation(ProtocolLoggingLevel.Verbose, null, context);
		}

		public void SetSessionPermissions(Permission permissions)
		{
			ExTraceGlobals.SmtpReceiveTracer.TraceDebug<string, Permission>((long)this.GetHashCode(), "Client {0} is granted the following permission {1}", this.RemoteIdentityName, permissions);
			this.sessionPermissions = permissions;
			this.logSession.LogInformation(ProtocolLoggingLevel.Verbose, Util.AsciiStringToBytes(Util.GetPermissionString(permissions)), "Set Session Permissions");
		}

		public void SetSessionPermissions(SecurityIdentifier client)
		{
			Permission permission = Permission.None;
			try
			{
				RawSecurityDescriptor securityDescriptor = this.sendConnector.GetSecurityDescriptor();
				if (securityDescriptor != null)
				{
					permission = AuthzAuthorization.CheckPermissions(client, securityDescriptor, null);
					ExTraceGlobals.SmtpReceiveTracer.TraceDebug<string, Permission>((long)this.GetHashCode(), "Client {0} is granted the following permission {1}", this.RemoteIdentityName, permission);
				}
				else
				{
					ExTraceGlobals.SmtpReceiveTracer.TraceError<string>((long)this.GetHashCode(), "Client {0}'s SD is null", this.RemoteIdentityName);
				}
			}
			catch (Win32Exception ex)
			{
				ExTraceGlobals.SmtpReceiveTracer.TraceError<int>((long)this.GetHashCode(), "AuthzAuthorization.CheckPermissions failed with {0}.", ex.NativeErrorCode);
			}
			this.sessionPermissions = permission;
			this.logSession.LogInformation(ProtocolLoggingLevel.Verbose, Util.AsciiStringToBytes(Util.GetPermissionString(permission)), "Set Session Permissions");
		}

		public void SetSessionPermissions(IntPtr userToken)
		{
			Permission permission = Permission.None;
			try
			{
				RawSecurityDescriptor securityDescriptor = this.sendConnector.GetSecurityDescriptor();
				if (securityDescriptor != null)
				{
					permission = AuthzAuthorization.CheckPermissions(userToken, securityDescriptor, null);
					ExTraceGlobals.SmtpSendTracer.TraceDebug<string, Permission>((long)this.GetHashCode(), "Client {0} is granted the following permission {1}", this.RemoteIdentityName, permission);
				}
				else
				{
					ExTraceGlobals.SmtpReceiveTracer.TraceError<string>((long)this.GetHashCode(), "Client {0}'s SD is null", this.RemoteIdentityName);
				}
			}
			catch (Win32Exception ex)
			{
				ExTraceGlobals.SmtpSendTracer.TraceError<int>((long)this.GetHashCode(), "AuthzAuthorization.CheckPermissions failed with {0}.", ex.NativeErrorCode);
			}
			this.sessionPermissions = permission;
			this.logSession.LogInformation(ProtocolLoggingLevel.Verbose, Util.AsciiStringToBytes(Util.GetPermissionString(permission)), "Set Session Permissions");
		}

		public void EnqueueCommandList(SmtpOutSession.SessionState state)
		{
			if (state >= SmtpOutSession.SessionState.NumStates)
			{
				throw new InvalidOperationException("Cannot enqueue an unknown state");
			}
			if (state == SmtpOutSession.SessionState.XProxy)
			{
				throw new InvalidOperationException("Cannot enqueue XProxy in SmtpOut");
			}
			SmtpOutSession.CommandList obj = this.commandLists[(int)state];
			this.pipelinedCommandQueue.Enqueue(obj);
			ExTraceGlobals.SmtpSendTracer.TraceDebug<SmtpOutSession.SessionState>((long)this.GetHashCode(), "Enqueued command List: {0}", state);
		}

		public void LoadCertificate()
		{
			if (this.RequiresDirectTrust)
			{
				string internalTransportCertificateThumbprint = this.transportConfiguration.LocalServer.TransportServer.InternalTransportCertificateThumbprint;
				if (internalTransportCertificateThumbprint != null)
				{
					ExTraceGlobals.SmtpSendTracer.TraceError<long>((long)this.GetHashCode(), "SmtpOutSession(id={0}). Loading Internal Transport Certificate for use with Direct Trust.", this.connectionId);
					this.internalTransportCertificate = this.certificateCache.GetInternalTransportCertificate(Components.Configuration.LocalServer.TransportServer.InternalTransportCertificateThumbprint, SmtpOutConnection.Events);
					if (this.internalTransportCertificate != null)
					{
						CertificateExpiryCheck.CheckCertificateExpiry(this.internalTransportCertificate, SmtpOutConnection.Events, SmtpSessionCertificateUse.DirectTrust, null);
					}
					else
					{
						ExTraceGlobals.SmtpSendTracer.TraceError<long>((long)this.GetHashCode(), "SmtpOutSession(id={0}). Internal Transport Certificate could not be loaded.", this.connectionId);
					}
				}
				else
				{
					ExTraceGlobals.SmtpSendTracer.TraceError((long)this.GetHashCode(), "this.transportConfiguration.LocalServer.TransportServer.InternalTransportCertificateThumbprint is null. Examine AD/ADAM server object");
				}
			}
			this.advertisedTlsCertificate = SmtpOutSession.LoadTlsCertificate(this.TlsConfiguration, this.certificateCache, this.transportAppConfig.SmtpSendConfiguration.OneLevelWildcardMatchForCertSelection, this.Connector.Name, this.GetHashCode());
		}

		public void StartTls(SecureState secureState)
		{
			ExTraceGlobals.SmtpSendTracer.TraceDebug((long)this.GetHashCode(), "Initiating TLS on the outboundConnection");
			this.secureState = (secureState | SecureState.NegotiationRequested);
		}

		public virtual void SetNextStateToQuit()
		{
			this.DropBreadcrumb(SmtpOutSession.SmtpOutSessionBreadcrumbs.SetNextStateToQuit);
			if (this.presentInSmtpOutSessionCache)
			{
				this.NextState = SmtpOutSession.SessionState.Quit;
				this.MoveToNextState();
				return;
			}
			if (this.nextHopIsProxyingBlindly && this.outboundProxyOriginalSessionState.EhloOptions.XRsetProxyTo)
			{
				this.NextState = SmtpOutSession.SessionState.XRsetProxyTo;
				return;
			}
			this.NextState = SmtpOutSession.SessionState.Quit;
		}

		public void AckConnection(AckStatus ackStatus, SmtpResponse smtpResponse)
		{
			this.AckConnection(ackStatus, smtpResponse, SessionSetupFailureReason.ProtocolError);
		}

		public void AckMessage(AckStatus ackStatus, SmtpResponse smtpResponse)
		{
			this.AckMessage(ackStatus, smtpResponse, 0L);
		}

		public void AckMessage(AckStatus ackStatus, SmtpResponse smtpResponse, long messageSize)
		{
			this.AckMessage(ackStatus, smtpResponse, messageSize, SessionSetupFailureReason.ProtocolError, true);
		}

		public virtual void AckMessage(AckStatus ackStatus, SmtpResponse smtpResponse, long messageSize, SessionSetupFailureReason failureReason, bool updateSmtpSendFailureCounters)
		{
			this.AckMessage(ackStatus, smtpResponse, messageSize, failureReason, null, updateSmtpSendFailureCounters, null);
		}

		public void AckMessage(AckStatus ackStatus, SmtpResponse smtpResponse, long messageSize, SessionSetupFailureReason failureReason, TimeSpan? retryInterval, bool updateSmtpSendFailureCounters, string messageTrackingSourceContext = null)
		{
			if (this.RoutedMailItem == null)
			{
				throw new InvalidOperationException("Message has already been acked!");
			}
			this.DropBreadcrumb(SmtpOutSession.SmtpOutSessionBreadcrumbs.AckMessage);
			ExTraceGlobals.SmtpSendTracer.TraceDebug<AckStatus>((long)this.GetHashCode(), "AckMessage called with status: {0}", ackStatus);
			if (ackStatus == AckStatus.Success)
			{
				if (this.numberOfRecipientsSucceeded > 0)
				{
					this.smtpOutConnection.MessagesSent += 1UL;
					this.messagesSentOverSession++;
				}
				this.currentRecipient = null;
				if (this.NextHopDeliveryType != DeliveryType.Heartbeat)
				{
					if (this.messageStream != null || messageSize == 0L)
					{
						throw new InvalidOperationException("Cleanup should be completed by this point.");
					}
					if (this.SmtpSendPerformanceCounters != null)
					{
						this.SmtpSendPerformanceCounters.MessageBytesSentTotal.IncrementBy(messageSize);
						this.SmtpSendPerformanceCounters.MessagesSentTotal.Increment();
						this.SmtpSendPerformanceCounters.TotalRecipientsSent.IncrementBy((long)this.NumberOfRecipientsSucceeded);
					}
					if (this.IsNextHopDomainSecured)
					{
						Utils.SecureMailPerfCounters.DomainSecureMessagesSentTotal.Increment();
					}
				}
			}
			else
			{
				if (this.messageStream != null)
				{
					this.messageStream.Close();
					this.messageStream = null;
				}
				this.currentRecipient = null;
				if (messageSize != 0L)
				{
					throw new InvalidOperationException("There is still part of a message left to send.");
				}
				SmtpOutConnection.Events.LogEvent(TransportEventLogConstants.Tuple_SmtpSendAckMessage, null, new object[]
				{
					this.Connector.Name,
					this.RoutedMailItem.InternetMessageId,
					smtpResponse
				});
				if (updateSmtpSendFailureCounters)
				{
					this.smtpOutConnection.UpdateSmtpSendFailurePerfCounter(failureReason);
				}
			}
			if (this.SmtpSendPerformanceCounters != null && DeliveryType.Heartbeat != this.NextHopDeliveryType)
			{
				this.SmtpSendPerformanceCounters.TotalBytesSent.IncrementBy(this.connection.BytesSent - this.bytesSentAtLastCount);
				this.bytesSentAtLastCount = this.connection.BytesSent;
			}
			this.nextHopConnection.AckMailItem(ackStatus, smtpResponse, this.ackDetails, retryInterval, MessageTrackingSource.SMTP, messageTrackingSourceContext, LatencyComponent.SmtpSend, this.AdvertisedEhloOptions.AdvertisedFQDN, this.ShadowCurrentMailItem, this.SmtpHost, (this.Permissions & Permission.SendOrganizationHeaders) == Permission.None);
			this.routedMailItem = null;
			this.shadowCurrentMailItem = false;
			this.recipientCorrelator = null;
			if (this.NextHopDeliveryType != DeliveryType.Heartbeat)
			{
				this.messageCount++;
			}
		}

		public void AckRecipient(AckStatus ackStatus, SmtpResponse smtpResponse)
		{
			ExTraceGlobals.SmtpSendTracer.TraceDebug<AckStatus>((long)this.GetHashCode(), "AckRecipient called with status: {0}", ackStatus);
			switch (ackStatus)
			{
			case AckStatus.Success:
				this.numberOfRecipientsSucceeded++;
				break;
			case AckStatus.Retry:
				this.numberOfRecipientsAckedForRetry++;
				break;
			}
			this.nextHopConnection.AckRecipient(ackStatus, smtpResponse);
		}

		public void FailoverConnection(SmtpResponse smtpResponse, bool ignorePipeLine)
		{
			this.FailoverConnection(smtpResponse, ignorePipeLine, false, SessionSetupFailureReason.ProtocolError);
		}

		public void RemoveConnection()
		{
			if (this.smtpOutConnection != null)
			{
				this.smtpOutConnection.RemoveConnection();
			}
		}

		public void PrepareNextStateForEstablishedSession()
		{
			this.DropBreadcrumb(SmtpOutSession.SmtpOutSessionBreadcrumbs.PrepareNextStateForEstablishedSession);
			if (this.nextHopConnection.ReadOnlyMailItem != null)
			{
				this.nextHopConnection.ReadOnlyMailItem.TrackSuccessfulConnectLatency(LatencyComponent.SmtpSendConnect);
			}
			if (!this.smtpOutConnection.NextHopIsOutboundProxy)
			{
				this.PrepareToSendXshadowOrMessage();
				return;
			}
			this.helloDomainOfOutboundProxyFrontEnd = this.AdvertisedEhloOptions.AdvertisedFQDN;
			if (this.AdvertisedEhloOptions.XProxyTo)
			{
				this.NextState = SmtpOutSession.SessionState.XProxyTo;
				return;
			}
			this.FailoverConnection(SmtpResponse.XProxyToRequired, SessionSetupFailureReason.ProtocolError);
			this.nextState = SmtpOutSession.SessionState.Quit;
		}

		public void PrepareToSendXshadowOrMessage()
		{
			this.DropBreadcrumb(SmtpOutSession.SmtpOutSessionBreadcrumbs.PrepareSendXshadowOrMessage);
			if (this.SendShadow)
			{
				ExTraceGlobals.SmtpSendTracer.TraceDebug((long)this.GetHashCode(), "Issue XSHADOW before sending messages.");
				this.NextState = SmtpOutSession.SessionState.XShadow;
				return;
			}
			if (this.SendXShadowRequest)
			{
				ExTraceGlobals.SmtpSendTracer.TraceDebug((long)this.GetHashCode(), "Issue XSHADOWREQUEST before sending shadow messages.");
				this.NextState = SmtpOutSession.SessionState.XShadowRequest;
				return;
			}
			ExTraceGlobals.SmtpSendTracer.TraceDebug((long)this.GetHashCode(), "Starting sending messages.");
			this.PrepareForNextMessage(false);
		}

		public void EnqueueNextPipeLinedCommands()
		{
			if (this.doPrepareForNextMessage)
			{
				this.doPrepareForNextMessage = false;
				this.betweenMessagesRset = this.issueBetweenMsgRset;
				if (this.betweenMessagesRset)
				{
					this.NextState = SmtpOutSession.SessionState.Rset;
				}
				else
				{
					this.NextState = SmtpOutSession.SessionState.MessageStart;
				}
				if (this.AdvertisedEhloOptions.Pipelining)
				{
					if (this.issueBetweenMsgRset)
					{
						this.EnqueueCommandList(SmtpOutSession.SessionState.Rset);
					}
					this.EnqueueCommandList(SmtpOutSession.SessionState.MessageStart);
					this.EnqueueCommandList(SmtpOutSession.SessionState.PerRecipient);
					this.numRcptCommandsInPipelineQueue = this.nextHopConnection.RecipientCount;
					ExTraceGlobals.SmtpSendTracer.TraceDebug<int>((long)this.GetHashCode(), "Total Number of RCPT TO commands enqueued : {0}", this.numRcptCommandsInPipelineQueue);
				}
			}
		}

		public void PrepareForNextMessage(bool issueBetweenMsgRset)
		{
			this.DropBreadcrumb(SmtpOutSession.SmtpOutSessionBreadcrumbs.PrepareForNextMessage);
			if (this.pipelinedResponseQueue.Count > 0 || this.doPrepareForNextMessage)
			{
				this.pipeLineNextMessagePending = true;
				ExTraceGlobals.SmtpSendTracer.TraceDebug((long)this.GetHashCode(), "PrepareForNextMessage will be honored when we finish the pipeline");
				return;
			}
			for (;;)
			{
				this.numberOfRecipientsAttempted = 0;
				this.numberOfRecipientsSucceeded = 0;
				this.numberOfRecipientsAckedForRetry = 0;
				this.numberOfRecipientsAcked = 0;
				this.currentRecipient = null;
				this.nextRecipient = null;
				this.shadowCurrentMailItem = false;
				this.ResetPipelineState();
				bool flag;
				if (this.ShouldAttemptSendingMessageOnSameConnection(out flag))
				{
					this.routedMailItem = this.nextHopConnection.GetNextMailItem();
					if (this.routedMailItem == null)
					{
						ExTraceGlobals.SmtpSendTracer.TraceDebug((long)this.GetHashCode(), "No more messages in queue");
					}
				}
				else
				{
					this.routedMailItem = null;
					ExTraceGlobals.SmtpSendTracer.TraceDebug((long)this.GetHashCode(), "The message count have reached SMTP Max Messages per connection.");
					this.messageSendAttemptCount = 0;
				}
				if (!flag)
				{
					this.dontCacheThisConnection = true;
				}
				if (this.routedMailItem == null)
				{
					break;
				}
				if (!this.Shadowed)
				{
					this.shadowCurrentMailItem = false;
				}
				else
				{
					this.shadowCurrentMailItem = (this.shadowRedundancyManager != null && this.shadowRedundancyManager.ShouldShadowMailItem(this.RoutedMailItem));
				}
				this.SetupPoisonContext();
				if (this.PreProcessMessage())
				{
					goto Block_9;
				}
			}
			this.pipelinedCommandQueue.Clear();
			if (this.SendXQDiscard)
			{
				ExTraceGlobals.SmtpSendTracer.TraceDebug((long)this.GetHashCode(), "XQDISCARD will be issued");
				this.NextState = SmtpOutSession.SessionState.XQDiscard;
				return;
			}
			this.SetNextStateToQuit();
			return;
			Block_9:
			this.messageSendAttemptCount++;
			this.recipientCorrelator = new RecipientCorrelator();
			ExTraceGlobals.SmtpSendTracer.TraceDebug((long)this.GetHashCode(), "Got a message");
			if (!this.doPrepareForNextMessage)
			{
				this.issueBetweenMsgRset = issueBetweenMsgRset;
				this.doPrepareForNextMessage = true;
			}
		}

		public void ResetPipelineState()
		{
			this.pipelinedCommandQueue.Clear();
			this.numRcptCommandsInPipelineQueue = 0;
			this.pipeLineNextMessagePending = false;
			this.pipeLineFailOverPending = false;
		}

		public virtual void SetNextStateForCachedSessionAndLogInfo(int cacheSize)
		{
			this.logSession.LogInformation(ProtocolLoggingLevel.Verbose, null, string.Format("successfully added connection to cache. Current cache size is {0}", cacheSize));
			this.SetNextStateForCachedSession();
		}

		public virtual void SetNextStateForCachedSession()
		{
			this.DropBreadcrumb(SmtpOutSession.SmtpOutSessionBreadcrumbs.SetNextStateForCachedSession);
			this.pipelinedCommandQueue.Clear();
			this.presentInSmtpOutSessionCache = true;
			this.responseBuffer = null;
			this.sendBuffer = new BufferBuilder();
			this.NextState = SmtpOutSession.SessionState.Inactive;
			this.AckConnection(AckStatus.Success, SmtpResponse.SuccessNoNewConnectionResponse, SessionSetupFailureReason.None);
		}

		public void DropBreadcrumb(SmtpOutSession.SmtpOutSessionBreadcrumbs breadcrumb)
		{
			this.breadcrumbs.Drop(breadcrumb);
		}

		public bool HasMoreBlobsPending()
		{
			return this.BlobsToSend != null && this.BlobsToSend.Count > 0;
		}

		public void SetupBlobsToSend()
		{
			if (this.AuthMechanism != SmtpSendConnectorConfig.AuthMechanisms.ExchangeServer || !this.MessageContextBlobTransferSupported)
			{
				ExTraceGlobals.SmtpSendTracer.TraceDebug((long)this.GetHashCode(), "Sending MessageContext blob is not supported");
				return;
			}
			List<SmtpMessageContextBlob> list = SmtpMessageContextBlob.GetBlobsToSend(this.AdvertisedEhloOptions, this);
			if (this.blobsToSend != null)
			{
				this.blobsToSend.Clear();
			}
			if (list != null)
			{
				if (this.blobsToSend == null)
				{
					this.blobsToSend = new Queue<SmtpMessageContextBlob>(list.Count);
				}
				foreach (SmtpMessageContextBlob item in list)
				{
					this.blobsToSend.Enqueue(item);
				}
			}
		}

		internal static string AuthLevelToString(RequiredTlsAuthLevel? authLevel)
		{
			if (authLevel != null)
			{
				return authLevel.Value.ToString();
			}
			return "Opportunistic";
		}

		public void UpdateServerLatency(TimeSpan latency)
		{
			if (this.SendFewerMessagesToSlowerServerEnabled)
			{
				this.smtpOutConnection.UpdateServerLatency(latency);
			}
		}

		protected virtual void IncrementConnectionCounters()
		{
			if (this.SmtpSendPerformanceCounters != null)
			{
				this.SmtpSendPerformanceCounters.ConnectionsTotal.Increment();
				this.SmtpSendPerformanceCounters.ConnectionsCurrent.Increment();
			}
		}

		protected virtual void DecrementConnectionCounters()
		{
			if (this.SmtpSendPerformanceCounters != null)
			{
				this.SmtpSendPerformanceCounters.ConnectionsCurrent.Decrement();
			}
		}

		protected virtual bool InvokeCommandHandler(SmtpCommand command)
		{
			this.DropBreadcrumb(SmtpOutSession.SmtpOutSessionBreadcrumbs.InvokeCommandHandler);
			command.OutboundFormatCommand();
			if (command.ProtocolCommandString != null)
			{
				command.ProtocolCommand = ByteString.StringToBytesAndAppendCRLF(command.ProtocolCommandString, true);
				if (string.IsNullOrEmpty(command.RedactedProtocolCommandString))
				{
					this.logSession.LogSend(command.ProtocolCommand);
				}
				else
				{
					this.logSession.LogSend(ByteString.StringToBytes(command.RedactedProtocolCommandString, true));
				}
				ExTraceGlobals.SmtpSendTracer.TraceDebug<string>((long)this.GetHashCode(), "Enqueuing Command: {0} on the connection", command.ProtocolCommandString);
				this.EnqueueResponseHandler(command);
				BdatSmtpCommand bdatSmtpCommand = command as BdatSmtpCommand;
				if (bdatSmtpCommand != null)
				{
					if (this.sendBuffer.Length != 0)
					{
						throw new InvalidOperationException("BDAT cannot be pipelined");
					}
					if (bdatSmtpCommand.SmtpMessageContextBlob != null)
					{
						this.logSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "Sending blob {0}", new object[]
						{
							bdatSmtpCommand.SmtpMessageContextBlob.Name
						});
					}
					this.SendBdatStream(command.ProtocolCommand, bdatSmtpCommand.BodyStream);
					return true;
				}
				else
				{
					this.sendBuffer.Append(command.ProtocolCommand);
				}
			}
			else if (command.ProtocolCommand != null)
			{
				this.EnqueueResponseHandler(command);
				this.logSession.LogSend(SmtpOutSession.BinaryData);
				this.sendBuffer.Append(command.ProtocolCommand);
			}
			else
			{
				DataSmtpCommand dataSmtpCommand = command as DataSmtpCommand;
				if (dataSmtpCommand != null && dataSmtpCommand.BodyStream != null)
				{
					if (this.sendBuffer.Length != 0)
					{
						throw new InvalidOperationException("DATA cannot send stream unless send buffer is empty");
					}
					this.EnqueueResponseHandler(command);
					this.SendDataStream(dataSmtpCommand.BodyStream);
					return true;
				}
				else
				{
					command.Dispose();
				}
			}
			return false;
		}

		protected virtual SmtpCommand CreateSmtpCommand(string cmd)
		{
			ExTraceGlobals.SmtpSendTracer.TraceDebug<string>((long)this.GetHashCode(), "Creating Smtp Command: {0}", cmd);
			SmtpCommand smtpCommand = null;
			if (cmd != null)
			{
				if (<PrivateImplementationDetails>{BF04BF36-E6FD-46F0-A1BE-B963EEF0FE07}.$$method0x6002793-1 == null)
				{
					<PrivateImplementationDetails>{BF04BF36-E6FD-46F0-A1BE-B963EEF0FE07}.$$method0x6002793-1 = new Dictionary<string, int>(20)
					{
						{
							"ConnectResponse",
							0
						},
						{
							"EHLO",
							1
						},
						{
							"HELO",
							2
						},
						{
							"AUTH",
							3
						},
						{
							"X-EXPS",
							4
						},
						{
							"STARTTLS",
							5
						},
						{
							"X-ANONYMOUSTLS",
							6
						},
						{
							"MAIL",
							7
						},
						{
							"RCPT",
							8
						},
						{
							"XEXCH50",
							9
						},
						{
							"DATA",
							10
						},
						{
							"BDAT",
							11
						},
						{
							"XBDATBLOB",
							12
						},
						{
							"RSET",
							13
						},
						{
							"XSHADOW",
							14
						},
						{
							"XQDISCARD",
							15
						},
						{
							"XPROXYTO",
							16
						},
						{
							"XRSETPROXYTO",
							17
						},
						{
							"XSESSIONPARAMS",
							18
						},
						{
							"QUIT",
							19
						}
					};
				}
				int num;
				if (<PrivateImplementationDetails>{BF04BF36-E6FD-46F0-A1BE-B963EEF0FE07}.$$method0x6002793-1.TryGetValue(cmd, out num))
				{
					switch (num)
					{
					case 0:
						this.DropBreadcrumb(SmtpOutSession.SmtpOutSessionBreadcrumbs.CreateCmdConnectResponse);
						break;
					case 1:
						this.DropBreadcrumb(SmtpOutSession.SmtpOutSessionBreadcrumbs.CreateCmdEhlo);
						smtpCommand = new EHLOSmtpCommand(this, this.transportConfiguration);
						break;
					case 2:
						this.DropBreadcrumb(SmtpOutSession.SmtpOutSessionBreadcrumbs.CreateCmdHelo);
						smtpCommand = new HELOSmtpCommand(this);
						break;
					case 3:
						this.DropBreadcrumb(SmtpOutSession.SmtpOutSessionBreadcrumbs.CreateCmdAuth);
						smtpCommand = new AuthSmtpCommand(this, false, this.transportConfiguration);
						break;
					case 4:
						this.DropBreadcrumb(SmtpOutSession.SmtpOutSessionBreadcrumbs.CreateCmdAuth);
						smtpCommand = new AuthSmtpCommand(this, true, this.transportConfiguration);
						break;
					case 5:
						this.DropBreadcrumb(SmtpOutSession.SmtpOutSessionBreadcrumbs.CreateCmdStarttls);
						smtpCommand = new StarttlsSmtpCommand(this, false);
						break;
					case 6:
						this.DropBreadcrumb(SmtpOutSession.SmtpOutSessionBreadcrumbs.CreateCmdStarttls);
						smtpCommand = new StarttlsSmtpCommand(this, true);
						break;
					case 7:
						this.DropBreadcrumb(SmtpOutSession.SmtpOutSessionBreadcrumbs.CreateCmdMail);
						smtpCommand = new MailSmtpCommand(this, this.transportAppConfig);
						break;
					case 8:
						this.DropBreadcrumb(SmtpOutSession.SmtpOutSessionBreadcrumbs.CreateCmdRcpt);
						smtpCommand = new RcptSmtpCommand(this, this.recipientCorrelator, this.transportAppConfig);
						break;
					case 9:
						this.DropBreadcrumb(SmtpOutSession.SmtpOutSessionBreadcrumbs.CreateCmdXexch50);
						smtpCommand = new Xexch50SmtpCommand(this, this.recipientCorrelator, this.mailRouter, this.transportAppConfig, this.transportConfiguration);
						break;
					case 10:
						this.DropBreadcrumb(SmtpOutSession.SmtpOutSessionBreadcrumbs.CreateCmdData);
						smtpCommand = new DataSmtpCommand(this, this.transportAppConfig);
						break;
					case 11:
						this.DropBreadcrumb(SmtpOutSession.SmtpOutSessionBreadcrumbs.CreateCmdBdat);
						smtpCommand = new BdatSmtpCommand(this, this.transportAppConfig, null);
						break;
					case 12:
						this.DropBreadcrumb(SmtpOutSession.SmtpOutSessionBreadcrumbs.CreateCmdBdat);
						smtpCommand = new BdatSmtpCommand(this, this.transportAppConfig, this.BlobsToSend.Dequeue());
						break;
					case 13:
						this.DropBreadcrumb(SmtpOutSession.SmtpOutSessionBreadcrumbs.CreateCmdRset);
						smtpCommand = new RsetSmtpCommand(this);
						break;
					case 14:
						this.DropBreadcrumb(SmtpOutSession.SmtpOutSessionBreadcrumbs.CreateCmdXShadow);
						smtpCommand = new XShadowSmtpCommand(this, this.shadowRedundancyManager);
						break;
					case 15:
						this.DropBreadcrumb(SmtpOutSession.SmtpOutSessionBreadcrumbs.CreateCmdXQDiscard);
						smtpCommand = new XQDiscardSmtpCommand(this, this.shadowRedundancyManager);
						break;
					case 16:
						this.DropBreadcrumb(SmtpOutSession.SmtpOutSessionBreadcrumbs.CreateCmdXProxyTo);
						smtpCommand = new XProxyToSmtpCommand(this, this.transportConfiguration, this.transportAppConfig);
						break;
					case 17:
						this.DropBreadcrumb(SmtpOutSession.SmtpOutSessionBreadcrumbs.CreateCmdXRsetProxyTo);
						smtpCommand = new XRsetProxyToSmtpCommand(this);
						break;
					case 18:
						this.DropBreadcrumb(SmtpOutSession.SmtpOutSessionBreadcrumbs.CreateCmdXSessionParams);
						smtpCommand = new XSessionParamsSmtpCommand(this);
						break;
					case 19:
						this.DropBreadcrumb(SmtpOutSession.SmtpOutSessionBreadcrumbs.CreateCmdQuit);
						smtpCommand = new QuitSmtpCommand(this);
						break;
					default:
						goto IL_391;
					}
					if (smtpCommand != null)
					{
						smtpCommand.ParsingStatus = ParsingStatus.Complete;
						smtpCommand.OutboundCreateCommand();
					}
					return smtpCommand;
				}
			}
			IL_391:
			throw new ArgumentException("Unknown command encountered in SmtpOut: " + cmd, "cmd");
		}

		protected virtual bool PreProcessMessage()
		{
			if (this.NextHopDeliveryType == DeliveryType.Heartbeat)
			{
				ExTraceGlobals.SmtpSendTracer.TraceDebug((long)this.GetHashCode(), "Message is a heartbeat message and is not sent to the remote server");
				this.AckMessage(AckStatus.Success, SmtpResponse.Empty);
				return false;
			}
			this.exch50DataPresent = (this.RoutedMailItem.LegacyXexch50Blob != null);
			if (!this.PreCheckMessageSize())
			{
				return false;
			}
			bool supportLongAddresses = this.SupportLongAddresses;
			bool supportOrar = this.SupportOrar;
			bool supportRDst = this.SupportRDst;
			bool supportSmtpUtf = this.SupportSmtpUtf8;
			if (!this.CheckLongSenderSupport(supportLongAddresses))
			{
				return false;
			}
			if (!this.CheckSmtpUtf8SenderSupport(supportSmtpUtf))
			{
				return false;
			}
			bool flag = false;
			foreach (MailRecipient recipient in this.nextHopConnection.ReadyRecipients)
			{
				if (this.PreProcessRecipient(recipient, supportLongAddresses, supportOrar, supportRDst, supportSmtpUtf))
				{
					flag = true;
				}
			}
			if (!flag)
			{
				ExTraceGlobals.SmtpSendTracer.TraceError<string>((long)this.GetHashCode(), "Message from '{0}' was NDR'ed because all recipients failed in PreProcess()", this.RoutedMailItem.From.ToString());
				this.AckMessage(AckStatus.Fail, SmtpResponse.NoRecipientSucceeded);
				return false;
			}
			return true;
		}

		protected virtual void ConnectResponseEvent(SmtpResponse smtpResponse)
		{
			ExTraceGlobals.SmtpSendTracer.TraceDebug<IPEndPoint>((long)this.GetHashCode(), "Connected to remote server: {0}", this.sessionProps.RemoteEndPoint);
			this.Banner = smtpResponse;
			if (!this.disconnected)
			{
				if (smtpResponse.StatusCode[0] != '2')
				{
					ExTraceGlobals.SmtpSendTracer.TraceError<SmtpResponse>((long)this.GetHashCode(), "Server is not accepting mail, connect response: {0}", smtpResponse);
					this.FailoverConnection(smtpResponse, SessionSetupFailureReason.ProtocolError);
					this.NextState = SmtpOutSession.SessionState.Quit;
					this.response.Clear();
					this.SendNextCommands();
					return;
				}
				if (this.Connector.ForceHELO)
				{
					this.NextState = SmtpOutSession.SessionState.Helo;
				}
				else
				{
					this.NextState = SmtpOutSession.SessionState.Ehlo;
				}
				this.response.Clear();
				this.SendNextCommands();
			}
		}

		protected void EnqueueResponseHandler(SmtpCommand command)
		{
			if (command != null)
			{
				ExTraceGlobals.SmtpSendTracer.TraceDebug<string>((long)this.GetHashCode(), "Enqueueing ResponseHandler for {0}", command.ProtocolCommandKeyword);
			}
			this.DropBreadcrumb(SmtpOutSession.SmtpOutSessionBreadcrumbs.EnqueueResponseHandler);
			this.pipelinedResponseQueue.Enqueue(command);
		}

		protected virtual void FinalizeNextStateAndSendCommands()
		{
			if (this.NextHopConnection != null && this.NextHopConnection.RetryQueueRequested)
			{
				this.AckConnection(AckStatus.Retry, this.NextHopConnection.RetryQueueSmtpResponse, SessionSetupFailureReason.None);
				return;
			}
			bool flag = true;
			if (this.NextState == SmtpOutSession.SessionState.Quit && !this.pipeLineFailOverPending)
			{
				NextHopSolutionKey nextHopKey = NextHopSolutionKey.Empty;
				IPEndPoint remoteEndPoint = this.RemoteEndPoint;
				if (this.smtpOutConnection.NextHopIsOutboundProxy)
				{
					nextHopKey = SmtpOutSessionCache.OutboundFrontendCacheKey;
					remoteEndPoint = SmtpOutSessionCache.OutboundFrontendIPEndpointCacheKey;
					this.outboundProxySendConnector = null;
					if (this.nextHopIsProxyingBlindly)
					{
						if (!this.xRsetProxyToAccepted)
						{
							this.dontCacheThisConnection = true;
						}
						else
						{
							this.xRsetProxyToAccepted = false;
							this.nextHopIsProxyingBlindly = false;
							if (this.outboundProxyOriginalSessionState == null)
							{
								throw new InvalidOperationException("original session state wasn't saved before blind proxy");
							}
							this.sessionProps.AdvertisedEhloOptions = this.outboundProxyOriginalSessionState.EhloOptions;
							this.sendConnector = this.outboundProxyOriginalSessionState.SendConnector;
							this.RemoteIdentity = this.outboundProxyOriginalSessionState.RemoteIdentity;
							this.RemoteIdentityName = this.outboundProxyOriginalSessionState.RemoteIdentityName;
							this.ackDetails = this.outboundProxyOriginalSessionState.AckDetails;
							this.sessionPermissions = this.outboundProxyOriginalSessionState.SessionPermissions;
						}
					}
					else if (!this.SessionProps.AdvertisedEhloOptions.XProxyTo)
					{
						this.dontCacheThisConnection = true;
					}
				}
				else if (this.NextHopConnection != null)
				{
					nextHopKey = this.NextHopConnection.Key;
				}
				else
				{
					this.dontCacheThisConnection = true;
				}
				NextHopConnection nextHopConnection = this.NextHopConnection;
				if (!this.dontCacheThisConnection && !this.failoverInProgress && SmtpOutConnectionHandler.SessionCache.TryAdd(nextHopKey, remoteEndPoint, this))
				{
					flag = false;
					if (nextHopConnection != null)
					{
						nextHopConnection.CreateConnectionIfNecessary();
					}
				}
				else if (this.NextHopConnection != null)
				{
					this.AckConnection(AckStatus.Success, SmtpResponse.SuccessfulConnection, SessionSetupFailureReason.None);
				}
			}
			if (flag)
			{
				this.EnqueueNextPipeLinedCommands();
				this.SendNextCommands();
			}
		}

		protected void StartReadLine()
		{
			this.connection.BeginReadLine(SmtpOutSession.readLineComplete, this);
		}

		protected virtual void HandleError(object error)
		{
			this.HandleError(error, false, true);
		}

		protected void HandleError(object error, bool retryWithoutStartTls, bool failoverConnection)
		{
			bool flag = false;
			this.SetupPoisonContext();
			string error2 = null;
			if (this.messageStream != null)
			{
				this.messageStream.Close();
				this.messageStream = null;
			}
			bool flag2;
			SessionSetupFailureReason failureReason;
			if (error is SocketError)
			{
				ExTraceGlobals.SmtpSendTracer.TraceDebug<long, object>((long)this.GetHashCode(), "SmtpOutSession(id={0}).HandleError (SocketError={1})", this.connectionId, error);
				error2 = ((SocketError)error).ToString();
				flag2 = false;
				failureReason = SessionSetupFailureReason.SocketError;
			}
			else if (error is SecurityStatus)
			{
				ExTraceGlobals.SmtpSendTracer.TraceDebug<long, object>((long)this.GetHashCode(), "SmtpOutSession(id={0}).HandleError (SecurityStatus={1})", this.connectionId, error);
				flag2 = true;
				failureReason = SessionSetupFailureReason.ProtocolError;
			}
			else if (error is BareLinefeedException)
			{
				ExTraceGlobals.SmtpSendTracer.TraceDebug<long, object>((long)this.GetHashCode(), "SmtpOutSession(id={0}).HandleError (Bare linefeed in content going out over SMTP DATA)", this.connectionId, error);
				this.routedMailItem.SuppressBodyInDsn = true;
				if (this.SmtpSendPerformanceCounters != null)
				{
					this.SmtpSendPerformanceCounters.MessagesSuppressedDueToBareLinefeeds.Increment();
				}
				this.AckMessage(AckStatus.Fail, AckReason.BareLinefeedsAreIllegal);
				flag2 = true;
				failureReason = SessionSetupFailureReason.ProtocolError;
			}
			else
			{
				ExTraceGlobals.SmtpSendTracer.TraceDebug<long, object>((long)this.GetHashCode(), "SmtpOutSession(id={0}).HandleError (error={1})", this.connectionId, error);
				flag2 = true;
				failureReason = SessionSetupFailureReason.ProtocolError;
			}
			if (error is SocketError && (SocketError)error == SocketError.ConnectionReset && this.routedMailItem != null && failoverConnection)
			{
				if ((this.IsHubDeliveringToNonMailbox(this.transportConfiguration.ProcessTransportRole) || this.transportConfiguration.ProcessTransportRole == ProcessTransportRole.Edge) && this.transportAppConfig.SmtpSendConfiguration.SuspiciousDisconnectRetryInterval > TimeSpan.Zero)
				{
					ExTraceGlobals.SmtpSendTracer.TraceDebug<long>((long)this.GetHashCode(), "SmtpOutSession(id={0}).HandleError has encountered a suspicious connection reset from a remote server.", this.connectionId);
					this.logSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "HandleError has encountered a suspicious connection reset from a remote, non-mailbox transport server (will retry in {0}).", new object[]
					{
						this.transportAppConfig.SmtpSendConfiguration.SuspiciousDisconnectRetryInterval
					});
					TimeSpan suspiciousDisconnectRetryInterval = this.transportAppConfig.SmtpSendConfiguration.SuspiciousDisconnectRetryInterval;
					string messageTrackingSourceContext = string.Format("Retrying due to suspicious connection reset with retry time of {0}", suspiciousDisconnectRetryInterval);
					this.AckMessage(AckStatus.Retry, AckReason.SuspiciousRemoteServerError, 0L, failureReason, new TimeSpan?(suspiciousDisconnectRetryInterval), false, messageTrackingSourceContext);
					this.AckConnection(AckStatus.Retry, AckReason.SuspiciousRemoteServerError, failureReason);
				}
				else if (this.transportConfiguration.ProcessTransportRole == ProcessTransportRole.MailboxSubmission || this.IsHubDeliveringToMailbox(this.transportConfiguration.ProcessTransportRole))
				{
					this.routedMailItem.IncrementPoisonForRemoteCount();
					if (this.routedMailItem.PoisonForRemoteCount > this.transportAppConfig.SmtpSendConfiguration.PoisonForRemoteThreshold)
					{
						ExTraceGlobals.SmtpSendTracer.TraceDebug<long, int>((long)this.GetHashCode(), "SmtpOutSession(id={0}).PoisonForRemote has exceeded the configurable threshold of {1}, acking the message as fail", this.connectionId, this.transportAppConfig.SmtpSendConfiguration.PoisonForRemoteThreshold);
						this.logSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "PoisonForRemote has exceeded the configurable threshold of {0}, acking the message as fail", new object[]
						{
							this.transportAppConfig.SmtpSendConfiguration.PoisonForRemoteThreshold
						});
						SmtpOutConnection.Events.LogEvent(TransportEventLogConstants.Tuple_SmtpSendPoisonForRemoteThresholdExceeded, null, new object[]
						{
							this.routedMailItem.InternetMessageId,
							this.sessionProps.RemoteEndPoint,
							this.transportAppConfig.SmtpSendConfiguration.PoisonForRemoteThreshold
						});
						if (this.transportConfiguration.ProcessTransportRole == ProcessTransportRole.Hub)
						{
							((RoutedMailItem)this.routedMailItem).Poison();
						}
						this.AckMessage(AckStatus.Fail, AckReason.MessageIsPoisonForRemoteServer, 0L, failureReason, false);
						this.AckConnection(AckStatus.Retry, AckReason.MessageIsPoisonForRemoteServer, failureReason);
						flag = true;
					}
				}
			}
			if (!failoverConnection)
			{
				if (this.RoutedMailItem != null)
				{
					this.AckMessage(AckStatus.Fail, AckReason.SendingError, 0L, failureReason, false);
				}
				this.AckConnection(AckStatus.Fail, AckReason.SendingError, failureReason);
			}
			if (this.nextHopConnection != null)
			{
				SmtpOutConnection.Events.LogEvent(TransportEventLogConstants.Tuple_SmtpSendRemoteDisconnected, null, new object[]
				{
					this.Connector.Name,
					this.sessionProps.RemoteEndPoint
				});
			}
			this.Disconnect(flag2 ? DisconnectReason.Local : DisconnectReason.Remote, failoverConnection && !flag && this.currentState != SmtpOutSession.SessionState.Quit, retryWithoutStartTls, error2, failureReason);
		}

		protected bool CheckSmtpUtf8SenderSupport(bool supportSmtpUtf8)
		{
			if (this.RoutedMailItem.From.IsUTF8 && !supportSmtpUtf8)
			{
				ExTraceGlobals.SmtpSendTracer.TraceError<string>((long)this.GetHashCode(), "Message from '{0}' was NDR'ed because the session does not support UTF-8 addresses", this.RoutedMailItem.From.ToString());
				this.AckMessage(AckStatus.Fail, AckReason.SmtpSendUtf8SenderAddress);
				return false;
			}
			return true;
		}

		private bool CheckSmtpUtf8RecipientSupport(MailRecipient recipient, bool supportSmtpUtf8)
		{
			if (recipient.Email.IsUTF8 && !supportSmtpUtf8)
			{
				ExTraceGlobals.SmtpSendTracer.TraceError<string>((long)this.GetHashCode(), "Recipient '{0}' was failed because the session does not support UTF-8 addresses", recipient.Email.ToString());
				recipient.Ack(AckStatus.Fail, AckReason.SmtpSendUtf8RecipientAddress);
				return false;
			}
			return true;
		}

		protected bool CheckLongSenderSupport(bool supportLongAddresses)
		{
			if (Util.IsLongAddress(this.RoutedMailItem.From))
			{
				if (!Util.IsValidInnerAddress(this.RoutedMailItem.From))
				{
					ExTraceGlobals.SmtpSendTracer.TraceError<string>((long)this.GetHashCode(), "Message from '{0}' was NDR'ed because the long address is invalid", this.RoutedMailItem.From.ToString());
					this.AckMessage(AckStatus.Fail, AckReason.SmtpSendInvalidLongSenderAddress);
					return false;
				}
				if (!supportLongAddresses)
				{
					ExTraceGlobals.SmtpSendTracer.TraceError<string>((long)this.GetHashCode(), "Message from '{0}' was NDR'ed because the session does not support long addresses", this.RoutedMailItem.From.ToString());
					this.AckMessage(AckStatus.Fail, AckReason.SmtpSendLongSenderAddress);
					return false;
				}
			}
			return true;
		}

		protected bool PreProcessRecipient(MailRecipient recipient, bool supportLongAddresses, bool supportOrar, bool supportRDst, bool supportSmtpUtf8)
		{
			if (recipient.ExtendedProperties.Contains("Microsoft.Exchange.Legacy.PassThru"))
			{
				this.exch50DataPresent = true;
			}
			return this.CheckLongRecipientSupport(recipient, supportLongAddresses) && this.CheckOrarSupport(recipient, supportOrar, supportLongAddresses) && this.CheckRDstSupport(recipient, supportRDst) && this.CheckSmtpUtf8RecipientSupport(recipient, supportSmtpUtf8);
		}

		protected void SendNextCommands()
		{
			this.DropBreadcrumb(SmtpOutSession.SmtpOutSessionBreadcrumbs.SendNextCommands);
			if (this.SendPipelinedCommands())
			{
				return;
			}
			if (this.pipelinedResponseQueue.Count != 0)
			{
				this.StartReadLine();
				return;
			}
			this.MoveToNextState();
		}

		private static void WriteCompleteReadLine(IAsyncResult asyncResult)
		{
			SmtpOutSession smtpOutSession = (SmtpOutSession)asyncResult.AsyncState;
			smtpOutSession.SetupPoisonContext();
			object obj;
			smtpOutSession.connection.EndWrite(asyncResult, out obj);
			if (obj != null)
			{
				smtpOutSession.HandleError(obj);
				return;
			}
			smtpOutSession.sendBuffer.Reset();
			smtpOutSession.StartReadLine();
		}

		private static void WriteBdatCompleteSendStream(IAsyncResult asyncResult)
		{
			SmtpOutSession smtpOutSession = (SmtpOutSession)asyncResult.AsyncState;
			smtpOutSession.SetupPoisonContext();
			object obj;
			smtpOutSession.connection.EndWrite(asyncResult, out obj);
			if (obj != null)
			{
				smtpOutSession.HandleError(obj);
				return;
			}
			smtpOutSession.connection.BeginWrite(smtpOutSession.messageStream, SmtpOutSession.writeStreamCompleteReadLine, smtpOutSession);
		}

		private static void WriteStreamCompleteReadLine(IAsyncResult asyncResult)
		{
			SmtpOutSession smtpOutSession = (SmtpOutSession)asyncResult.AsyncState;
			smtpOutSession.SetupPoisonContext();
			smtpOutSession.messageStream.Close();
			smtpOutSession.messageStream = null;
			object obj;
			smtpOutSession.connection.EndWrite(asyncResult, out obj);
			if (obj != null)
			{
				smtpOutSession.HandleError(obj);
				return;
			}
			smtpOutSession.sendBuffer.Reset();
			if (smtpOutSession.ShouldTrackMailboxDeliveryLatency())
			{
				LatencyTracker.BeginTrackLatency(LatencyComponent.SmtpSendMailboxDelivery, smtpOutSession.routedMailItem.LatencyTracker);
			}
			smtpOutSession.StartReadLine();
		}

		private static void CanCommandBePipelined(string command, out bool isLastCmdInPipeline, out bool isCmdPipelinable)
		{
			isLastCmdInPipeline = false;
			if ("MAIL".Equals(command, StringComparison.OrdinalIgnoreCase) || "RCPT".Equals(command, StringComparison.OrdinalIgnoreCase) || "RSET".Equals(command, StringComparison.OrdinalIgnoreCase))
			{
				isCmdPipelinable = true;
				return;
			}
			if ("DATA".Equals(command, StringComparison.OrdinalIgnoreCase))
			{
				isLastCmdInPipeline = true;
				isCmdPipelinable = true;
				return;
			}
			isCmdPipelinable = false;
		}

		private static void ReadLineComplete(IAsyncResult asyncResult)
		{
			SmtpOutSession smtpOutSession = (SmtpOutSession)asyncResult.AsyncState;
			smtpOutSession.SetupPoisonContext();
			bool overflow = false;
			byte[] buffer;
			int offset;
			int size;
			object obj;
			smtpOutSession.connection.EndReadLine(asyncResult, out buffer, out offset, out size, out obj);
			if (obj != null)
			{
				if (!(obj is SocketError) || (SocketError)obj != SocketError.MessageSize)
				{
					smtpOutSession.HandleError(obj);
					return;
				}
				overflow = true;
			}
			if (smtpOutSession.disconnected)
			{
				ExTraceGlobals.SmtpSendTracer.TraceError((long)smtpOutSession.GetHashCode(), "Command Received from NetworkConnection, but we are already disconnected");
				return;
			}
			if (smtpOutSession.shutdownConnectionCalled)
			{
				ExTraceGlobals.SmtpSendTracer.TraceError((long)smtpOutSession.GetHashCode(), "Command Received from NetworkConnection, but we have already begun to shut down");
				return;
			}
			if (smtpOutSession.ShouldTrackMailboxDeliveryLatency())
			{
				LatencyTracker.EndTrackLatency(LatencyComponent.SmtpSendMailboxDelivery, smtpOutSession.routedMailItem.LatencyTracker);
			}
			smtpOutSession.StartProcessingResponse(buffer, offset, size, overflow);
		}

		private static void TlsNegotiationComplete(IAsyncResult asyncResult)
		{
			SmtpOutSession smtpOutSession = (SmtpOutSession)asyncResult.AsyncState;
			smtpOutSession.SetupPoisonContext();
			object obj;
			smtpOutSession.connection.EndNegotiateTlsAsClient(asyncResult, out obj);
			if (obj != null)
			{
				smtpOutSession.smtpSendPerformanceCounters.TlsNegotiationsFailed.Increment();
				smtpOutSession.logSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "TLS negotiation failed with error {0}", new object[]
				{
					obj
				});
				bool retryWithoutStartTls;
				if (smtpOutSession.IsNextHopDomainSecured)
				{
					retryWithoutStartTls = false;
					string nextHopDomain = smtpOutSession.NextHopConnection.Key.NextHopDomain;
					Utils.SecureMailPerfCounters.DomainSecureOutboundSessionFailuresTotal.Increment();
					SmtpOutConnection.Events.LogEvent(TransportEventLogConstants.Tuple_MessageToSecureDomainFailedDueToTlsNegotiationFailure, nextHopDomain, new object[]
					{
						nextHopDomain,
						smtpOutSession.Connector.Name,
						obj
					});
				}
				else
				{
					retryWithoutStartTls = (smtpOutSession.currentState == SmtpOutSession.SessionState.StartTLS && !smtpOutSession.TlsConfiguration.RequireTls && smtpOutSession.AuthMechanism != SmtpSendConnectorConfig.AuthMechanisms.BasicAuthRequireTLS);
				}
				smtpOutSession.HandleError(obj, retryWithoutStartTls, true);
				return;
			}
			ConnectionInfo tlsConnectionInfo = smtpOutSession.connection.TlsConnectionInfo;
			Util.LogTlsSuccessResult(smtpOutSession.logSession, tlsConnectionInfo, smtpOutSession.connection.RemoteCertificate);
			smtpOutSession.TlsNegotiationComplete();
		}

		private void AckConnection(AckStatus ackStatus, SmtpResponse smtpResponse, SessionSetupFailureReason failureReason)
		{
			this.DropBreadcrumb(SmtpOutSession.SmtpOutSessionBreadcrumbs.AckConnection);
			if (this.nextHopConnection != null)
			{
				ExTraceGlobals.SmtpSendTracer.TraceDebug<AckStatus, SmtpResponse>((long)this.GetHashCode(), "AckConnection called with status: {0}, Response: {1}", ackStatus, smtpResponse);
				if (ackStatus == AckStatus.Success)
				{
					if (this.messageStream != null || this.currentRecipient != null || this.RoutedMailItem != null)
					{
						throw new InvalidOperationException("Cleanup should be completed by this point");
					}
				}
				else
				{
					if (this.messageStream != null)
					{
						this.messageStream.Close();
						this.messageStream = null;
					}
					this.routedMailItem = null;
					this.currentRecipient = null;
					this.recipientCorrelator = null;
					SmtpOutConnection.Events.LogEvent(TransportEventLogConstants.Tuple_SmtpSendAckConnection, null, new object[]
					{
						this.Connector.Name,
						this.RemoteEndPoint,
						smtpResponse
					});
				}
				this.smtpOutConnection.BytesSent = (ulong)this.connection.BytesSent;
				this.smtpOutConnection.AckConnection(ackStatus, smtpResponse, this.ackDetails, null, failureReason);
				this.nextHopConnection = null;
				return;
			}
			if (!this.smtpOutConnection.NextHopIsOutboundProxy)
			{
				throw new InvalidOperationException("Connection has already been acked!");
			}
		}

		private void Disconnect(DisconnectReason disconnectReason, bool failOverConnection, bool retryWithoutStartTls, string error, SessionSetupFailureReason failureReason)
		{
			ExTraceGlobals.SmtpSendTracer.TraceDebug<long, DisconnectReason, bool>((long)this.GetHashCode(), "Disconnect Initiated for connection {0}.  DisconnectReason : {1}, FailoverConnection : {2}", this.connectionId, disconnectReason, failOverConnection);
			this.DropBreadcrumb(SmtpOutSession.SmtpOutSessionBreadcrumbs.Disconnect);
			while (this.pipelinedResponseQueue.Count > 0)
			{
				SmtpCommand smtpCommand = (SmtpCommand)this.pipelinedResponseQueue.Dequeue();
				if (smtpCommand != null)
				{
					smtpCommand.Dispose();
				}
			}
			if (!this.disconnected)
			{
				if (this.connection != null)
				{
					this.connection.Dispose();
					this.logSession.LogDisconnect(disconnectReason);
					if (this.SmtpSendPerformanceCounters != null)
					{
						this.DecrementConnectionCounters();
					}
				}
				this.disconnected = true;
				if (failOverConnection && this.nextHopConnection != null)
				{
					SmtpResponse smtpResponse = SmtpResponse.Empty;
					if (disconnectReason == DisconnectReason.Local)
					{
						smtpResponse = SmtpResponse.ConnectionTimedOut;
					}
					else
					{
						smtpResponse = SmtpResponse.ConnectionDroppedDueTo(error);
					}
					ExTraceGlobals.SmtpSendTracer.TraceDebug<SmtpResponse>((long)this.GetHashCode(), "Initiating FailoverConnection {0}", smtpResponse);
					this.FailoverConnection(smtpResponse, true, retryWithoutStartTls, failureReason);
				}
				if (!this.failoverInProgress)
				{
					this.smtpOutConnection.RemoveConnection();
				}
			}
		}

		private void FailoverConnection(SmtpResponse smtpResponse, bool ignorePipeLine, bool retryWithoutStartTls, SessionSetupFailureReason failoverReason)
		{
			this.DropBreadcrumb(SmtpOutSession.SmtpOutSessionBreadcrumbs.FailoverConnection);
			if (this.RoutedMailItem != null)
			{
				this.AckMessage(AckStatus.Pending, smtpResponse);
			}
			else if (this.SmtpSendPerformanceCounters != null && this.connection != null)
			{
				this.SmtpSendPerformanceCounters.TotalBytesSent.IncrementBy(this.connection.BytesSent - this.bytesSentAtLastCount);
				this.bytesSentAtLastCount = this.connection.BytesSent;
			}
			if (!ignorePipeLine && this.pipelinedResponseQueue.Count > 0)
			{
				if (!this.pipeLineFailOverPending)
				{
					this.pipeLineFailOverResponse = smtpResponse;
				}
				this.pipeLineFailOverPending = true;
				ExTraceGlobals.SmtpSendTracer.TraceDebug<SmtpResponse>((long)this.GetHashCode(), "FailoverConnection pending with response {0}", smtpResponse);
				return;
			}
			SmtpResponse arg = this.pipeLineFailOverPending ? this.pipeLineFailOverResponse : smtpResponse;
			ExTraceGlobals.SmtpSendTracer.TraceDebug<SmtpResponse>((long)this.GetHashCode(), "FailoverConnection initiated with response {0}", arg);
			this.nextHopConnection = null;
			this.ResetPipelineState();
			this.failoverInProgress = true;
			this.smtpOutConnection.FailoverConnection(arg, retryWithoutStartTls, failoverReason, this.nextHopIsProxyingBlindly);
		}

		private bool ShouldTrackMailboxDeliveryLatency()
		{
			bool result = false;
			if (this.transportConfiguration.ProcessTransportRole == ProcessTransportRole.Hub && this.NextHopConnection != null && this.NextHopDeliveryType == DeliveryType.SmtpDeliveryToMailbox && this.pipelinedResponseQueue.Count > 0)
			{
				BdatSmtpCommand bdatSmtpCommand = this.pipelinedResponseQueue.Peek() as BdatSmtpCommand;
				if (bdatSmtpCommand != null && bdatSmtpCommand.IsLastChunkOutbound)
				{
					result = true;
				}
			}
			return result;
		}

		private bool IsHubServer(string fqdn)
		{
			if (!Components.IsBridgehead)
			{
				throw new InvalidOperationException("IsHubServer: should only be called on Hub server");
			}
			if (string.IsNullOrEmpty(fqdn))
			{
				throw new ArgumentNullException("fqdn");
			}
			return this.mailRouter.IsHubTransportServer(fqdn);
		}

		private void AppendToResponseBuffer(string responseline)
		{
			if ((this.response.Count >= 50 && this.CurrentState != SmtpOutSession.SessionState.XQDiscard) || (this.response.Count > this.transportAppConfig.ShadowRedundancy.MaxDiscardIdsPerSmtpCommand && this.CurrentState == SmtpOutSession.SessionState.XQDiscard))
			{
				throw new FormatException("Excessive data, unable to parse");
			}
			int length = responseline.Length;
			if (length > 0 && responseline[length - 1] == '\r')
			{
				responseline = responseline.Substring(0, length - 1);
			}
			this.response.Add(responseline);
		}

		private void SetDefaultIdentity()
		{
			if ((this.Connector.SmartHostAuthMechanism & SmtpSendConnectorConfig.AuthMechanisms.ExternalAuthoritative) != SmtpSendConnectorConfig.AuthMechanisms.None)
			{
				this.RemoteIdentity = WellKnownSids.ExternallySecuredServers;
				this.RemoteIdentityName = "accepted_domain";
				this.SetSessionPermissions(this.RemoteIdentity);
				return;
			}
			if (this.NextHopDeliveryType == DeliveryType.SmtpRelayToTiRg)
			{
				this.RemoteIdentity = WellKnownSids.LegacyExchangeServers;
				this.RemoteIdentityName = "ti_rg_servers";
				this.SetSessionPermissions(this.RemoteIdentity);
				return;
			}
			if (this.CanDowngradeExchangeServerAuth)
			{
				this.RemoteIdentity = WellKnownSids.HubTransportServers;
				this.RemoteIdentityName = "e14andhigher_hub_servers";
				this.SetSessionPermissions(this.RemoteIdentity);
			}
		}

		private void MoveToNextState()
		{
			ExTraceGlobals.SmtpSendTracer.TraceDebug<SmtpOutSession.SessionState, SmtpOutSession.SessionState>(0L, "MoveToNextState {0} -> {1}", this.currentState, this.nextState);
			this.DropBreadcrumb(SmtpOutSession.SmtpOutSessionBreadcrumbs.MoveToNextState);
			while (this.pipelinedResponseQueue.Count == 0 && !this.disconnected)
			{
				if ((byte)(this.secureState & SecureState.NegotiationRequested) == 128)
				{
					ExTraceGlobals.SmtpSendTracer.TraceDebug((long)this.GetHashCode(), "Waiting for TLS Handshake to complete");
					return;
				}
				if (this.doPrepareForNextMessage)
				{
					ExTraceGlobals.SmtpSendTracer.TraceDebug((long)this.GetHashCode(), "Waiting for doPrepareForNextMessage lookup to complete");
					return;
				}
				if (this.NextState == SmtpOutSession.SessionState.Inactive)
				{
					ExTraceGlobals.SmtpSendTracer.TraceDebug((long)this.GetHashCode(), "Waiting in the Smtp out Connection cache");
					return;
				}
				if (this.pipelinedCommandQueue.Count != 0 || (this.currentEnumeratorInPipeline != null && this.currentEnumeratorInPipeline.HasNext))
				{
					throw new InvalidOperationException("Should not move to the next state if there are any commands ready to be sent out");
				}
				this.CurrentState = this.NextState;
				this.EnqueueCommandList(this.CurrentState);
				if (this.SendPipelinedCommands())
				{
					return;
				}
			}
		}

		private bool SendPipelinedCommands()
		{
			this.DropBreadcrumb(SmtpOutSession.SmtpOutSessionBreadcrumbs.SendPipelinedCommands);
			bool flag = true;
			bool flag2 = false;
			int num = this.pipelinedResponseQueue.Count;
			if ((this.CurrentState == SmtpOutSession.SessionState.StartTLS || this.CurrentState == SmtpOutSession.SessionState.AnonymousTLS) && (byte)(this.secureState & SecureState.NegotiationRequested) == 128)
			{
				ExTraceGlobals.SmtpSendTracer.TraceDebug((long)this.GetHashCode(), "Waiting for TLS handshake completion to be called");
				if (this.pipelinedCommandQueue.Count != 0)
				{
					throw new InvalidOperationException("Pipelined command queue is not empty");
				}
				return false;
			}
			else
			{
				if ((this.lastCommandPipelined || this.pipelinedResponseQueue.Count <= 0) && !this.pipeLineNextMessagePending && !this.pipeLineFailOverPending && this.pipelinedResponseQueue.Count < 100)
				{
					try
					{
						while (flag && (this.pipelinedCommandQueue.Count > 0 || (this.currentEnumeratorInPipeline != null && this.currentEnumeratorInPipeline.HasNext)))
						{
							if (this.pipelinedCommandQueue.Count > 4)
							{
								throw new InvalidOperationException("The command queue can never grow bigger than 4, because the only states that can be enqueued at the same time are RSET, MAIL, RCPT, and DATA");
							}
							if (num >= 200)
							{
								ExTraceGlobals.SmtpSendTracer.TraceDebug((long)this.GetHashCode(), "Total commands pipelined greater than MaxPipelinedCommands");
								break;
							}
							if (this.currentCommandListInPipeline == null || this.currentEnumeratorInPipeline == null || !this.currentEnumeratorInPipeline.HasNext)
							{
								this.currentCommandListInPipeline = (SmtpOutSession.CommandList)this.pipelinedCommandQueue.Peek();
								if (this.AdvertisedEhloOptions.Pipelining && this.currentCommandListInPipeline.Equals(this.commandLists[8]))
								{
									this.numRcptCommandsInPipelineQueue--;
									if (this.numRcptCommandsInPipelineQueue < 0)
									{
										throw new InvalidOperationException("Number of recipients in pipeline should never be negative");
									}
									ExTraceGlobals.SmtpSendTracer.TraceDebug<int>((long)this.GetHashCode(), "Number of RCPT remaining to be sent out: {0}", this.numRcptCommandsInPipelineQueue);
								}
								if (!this.AdvertisedEhloOptions.Pipelining || !this.currentCommandListInPipeline.Equals(this.commandLists[8]) || this.numRcptCommandsInPipelineQueue == 0)
								{
									this.pipelinedCommandQueue.Dequeue();
								}
								this.currentEnumeratorInPipeline = (SmtpOutSession.CommandList.CommandListEnumerator)this.currentCommandListInPipeline.GetEnumerator();
							}
							if (!this.AdvertisedEhloOptions.Pipelining)
							{
								flag = false;
							}
							if (this.currentCommandListInPipeline.HasAddedCommands())
							{
								if (this.pipelinedResponseQueue.Count > 0)
								{
									ExTraceGlobals.SmtpSendTracer.TraceDebug((long)this.GetHashCode(), "There are new commands added for the current commandList and the response Q is not empty. Waiting for response before sending out more commands");
									break;
								}
								flag = false;
							}
							string text;
							if (this.lastCmdNotSent != null)
							{
								text = this.lastCmdNotSent;
								this.lastCmdNotSent = null;
								flag = false;
								ExTraceGlobals.SmtpSendTracer.TraceDebug<string>((long)this.GetHashCode(), "LastCmdNotSent {0}", text);
							}
							else
							{
								if (!this.currentEnumeratorInPipeline.MoveNext())
								{
									throw new InvalidOperationException("Cannot MoveNext in the command list even though HasNext is true");
								}
								text = (string)this.currentEnumeratorInPipeline.Current;
								ExTraceGlobals.SmtpSendTracer.TraceDebug<string>((long)this.GetHashCode(), "Command string : {0}", text);
								if (flag)
								{
									bool flag3 = true;
									bool flag4 = false;
									SmtpOutSession.CanCommandBePipelined(text, out flag3, out flag4);
									if (!flag4 && this.pipelinedResponseQueue.Count > 0)
									{
										this.lastCmdNotSent = text;
										ExTraceGlobals.SmtpSendTracer.TraceDebug<string>((long)this.GetHashCode(), "Command {0} cannot be pipelined as we are waiting on responses of other commands previously sent", text);
										break;
									}
									flag = (flag4 && !flag3);
								}
							}
							SmtpCommand smtpCommand = this.CreateSmtpCommand(text);
							if (smtpCommand != null)
							{
								ExTraceGlobals.SmtpSendTracer.TraceDebug<string>((long)this.GetHashCode(), "Invoking Command Handler for {0}", text);
								this.lastCommandPipelined = flag;
								flag2 = this.InvokeCommandHandler(smtpCommand);
								if (flag2 && flag)
								{
									throw new InvalidOperationException("ICH sent commands in the middle of a pipeline.");
								}
								num++;
							}
						}
					}
					finally
					{
						if (!flag2 && this.sendBuffer.Length > 0)
						{
							this.SendBufferThenReadLine();
							flag2 = true;
						}
					}
					return flag2;
				}
				ExTraceGlobals.SmtpSendTracer.TraceDebug((long)this.GetHashCode(), "Waiting for response from remote server before sending any more commands");
				if (this.sendBuffer.Length != 0)
				{
					throw new InvalidOperationException("Smtp send buffer is not empty");
				}
				return false;
			}
		}

		private void SendBufferThenReadLine()
		{
			this.DropBreadcrumb(SmtpOutSession.SmtpOutSessionBreadcrumbs.SendBufferThenReadLine);
			ExTraceGlobals.SmtpSendTracer.TraceDebug((long)this.GetHashCode(), "Flushing SendBuffer");
			this.connection.BeginWrite(this.sendBuffer.GetBuffer(), 0, this.sendBuffer.Length, SmtpOutSession.writeCompleteReadLine, this);
		}

		private void SendBdatStream(byte[] command, Stream stream)
		{
			this.DropBreadcrumb(SmtpOutSession.SmtpOutSessionBreadcrumbs.SendBdatStream);
			this.messageStream = stream;
			this.connection.BeginWrite(command, 0, command.Length, SmtpOutSession.writeBdatCompleteSendStream, this);
		}

		private void SendDataStream(Stream stream)
		{
			this.DropBreadcrumb(SmtpOutSession.SmtpOutSessionBreadcrumbs.SendDataStream);
			this.messageStream = stream;
			this.connection.BeginWrite(this.messageStream, SmtpOutSession.writeStreamCompleteReadLine, this);
		}

		private void ProcessResponse()
		{
			SmtpCommand command = (SmtpCommand)this.pipelinedResponseQueue.Dequeue();
			this.InvokeResponseHandler(command);
		}

		private void InvokeResponseHandler(SmtpCommand command)
		{
			this.DropBreadcrumb(SmtpOutSession.SmtpOutSessionBreadcrumbs.InvokeResponseHandler);
			SmtpResponse smtpResponse;
			if (!SmtpResponse.TryParse(this.response, out smtpResponse))
			{
				if (command != null)
				{
					command.Dispose();
				}
				throw new FormatException("Response text was incorrectly formed.");
			}
			this.response.Clear();
			if (command == null)
			{
				ExTraceGlobals.SmtpSendTracer.TraceDebug((long)this.GetHashCode(), "Invoked Response Handler for ConnectResponse");
				this.ConnectResponseEvent(smtpResponse);
				return;
			}
			ExTraceGlobals.SmtpSendTracer.TraceDebug<string>((long)this.GetHashCode(), "Invoked Response Handler for {0}", command.ProtocolCommandKeyword);
			command.SmtpResponse = smtpResponse;
			this.HandlePostParseResponse(command);
		}

		private void HandlePostParseResponse(SmtpCommand command)
		{
			if ((string.Equals(command.SmtpResponse.StatusCode, "421", StringComparison.Ordinal) && !Util.UpgradeCustomPermanentFailure(this.Connector.ErrorPolicies, command.SmtpResponse, this.transportAppConfig) && this.FailoverPermittedForRemoteShutdown && !(command is QuitSmtpCommand)) || this.pipeLineFailOverPending)
			{
				ExTraceGlobals.SmtpSendTracer.TraceError<string>((long)this.GetHashCode(), "Attempting failover. 421 Status code on {0}. NextState: Quit", command.ProtocolCommandKeyword);
				this.FailoverConnection(command.SmtpResponse, false);
				this.NextState = SmtpOutSession.SessionState.Quit;
			}
			else
			{
				command.OutboundProcessResponse();
				if ((byte)(this.secureState & SecureState.NegotiationRequested) == 128)
				{
					if (!(command is StarttlsSmtpCommand))
					{
						throw new InvalidOperationException("Command being processed is not StartTls");
					}
					command.Dispose();
					X509Certificate2 x509Certificate;
					if ((byte)(this.secureState & ~SecureState.NegotiationRequested) == 1)
					{
						x509Certificate = this.advertisedTlsCertificate;
					}
					else
					{
						x509Certificate = this.internalTransportCertificate;
					}
					this.logSession.LogCertificate("Sending certificate", x509Certificate);
					this.connection.BeginNegotiateTlsAsClient(x509Certificate, this.connection.RemoteEndPoint.Address.ToString(), SmtpOutSession.tlsNegotiationComplete, this);
					return;
				}
				else if (command.ParsingStatus == ParsingStatus.MoreDataRequired)
				{
					command.ProtocolCommand = null;
					command.ProtocolCommandString = null;
					command.ParsingStatus = ParsingStatus.Complete;
					if (!this.InvokeCommandHandler(command) && this.sendBuffer.Length != 0)
					{
						this.SendBufferThenReadLine();
					}
					return;
				}
			}
			command.Dispose();
			this.FinalizeNextStateAndSendCommands();
		}

		private void SetupPoisonContext()
		{
			if (this.RoutedMailItem != null)
			{
				PoisonMessage.Context = new MessageContext(this.RoutedMailItem.RecordId, this.RoutedMailItem.InternetMessageId, MessageProcessingSource.SmtpSend);
			}
		}

		private bool RequiresDownConversion()
		{
			bool result = false;
			if (this.RoutedMailItem != null)
			{
				switch (this.RoutedMailItem.BodyType)
				{
				case Microsoft.Exchange.Transport.BodyType.EightBitMIME:
					if (!this.AdvertisedEhloOptions.EightBitMime)
					{
						result = true;
					}
					break;
				case Microsoft.Exchange.Transport.BodyType.BinaryMIME:
					if (!this.AdvertisedEhloOptions.BinaryMime)
					{
						result = true;
					}
					else if (!this.AdvertisedEhloOptions.Chunking)
					{
						result = true;
					}
					break;
				}
			}
			return result;
		}

		private bool ShouldAttemptSendingMessageOnSameConnection(out bool canCacheConnection)
		{
			canCacheConnection = false;
			if (this.smtpOutConnection.NextHopIsOutboundProxy)
			{
				canCacheConnection = true;
			}
			if (this.sendConnector.SmtpMaxMessagesPerConnection == 0 || this.messageSendAttemptCount < this.sendConnector.SmtpMaxMessagesPerConnection)
			{
				canCacheConnection = true;
				if (!this.smtpOutConnection.NextHopIsOutboundProxy && this.smtpOutConnection.TotalTargets > 1 && this.SendFewerMessagesToSlowerServerEnabled)
				{
					int num = (int)this.smtpOutConnection.GetDelayForCurrentTarget(this.RemoteEndPoint).TotalSeconds;
					if (num < 1)
					{
						return true;
					}
					if (this.messageCount > 0 && this.messageCount > this.sendConnector.SmtpMaxMessagesPerConnection / 2 - num)
					{
						this.logSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "Detected a possibly slower server with delay {0}. Completing after {1} messages", new object[]
						{
							num,
							this.messageCount
						});
						return false;
					}
				}
				return true;
			}
			return false;
		}

		private void StartProcessingResponse(byte[] buffer, int offset, int size, bool overflow)
		{
			BufferBuilder bufferBuilder = this.responseBuffer ?? new BufferBuilder(size);
			try
			{
				this.SetupPoisonContext();
				if (bufferBuilder.Length + size > 32768)
				{
					this.logSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "line too long");
					string message = string.Format("Illegal response, length exceeds the maximum that can be handled by SmtpOut. Max = {0} chars", 32768);
					throw new FormatException(message);
				}
				bufferBuilder.Append(buffer, offset, size);
				if (overflow)
				{
					this.responseBuffer = bufferBuilder;
					this.StartReadLine();
				}
				else
				{
					this.responseBuffer = null;
					bufferBuilder.RemoveUnusedBufferSpace();
					if (!(this.pipelinedResponseQueue.Peek() is AuthSmtpCommand))
					{
						this.logSession.LogReceive(bufferBuilder.GetBuffer());
					}
					string text = bufferBuilder.ToString();
					this.AppendToResponseBuffer(text);
					if (text.Length < 3)
					{
						throw new FormatException("Illegal response: " + text);
					}
					if (text.Length > 3 && text[3] == '-')
					{
						this.StartReadLine();
					}
					else
					{
						this.ProcessResponse();
					}
				}
			}
			catch (FormatException ex)
			{
				ExTraceGlobals.SmtpSendTracer.TraceError<string>((long)this.GetHashCode(), "The connection was dropped because a response was illegally formatted. The error is: {0}", ex.Message);
				string text2;
				if (VariantConfiguration.InvariantNoFlightingSnapshot.Transport.VerboseLogging.Enabled)
				{
					text2 = ex.ToString();
				}
				else
				{
					text2 = ex.Message;
				}
				this.logSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "The connection was dropped because a response was illegally formatted. The error is: {0}", new object[]
				{
					text2
				});
				if (this.RoutedMailItem != null)
				{
					this.AckMessage(AckStatus.Retry, SmtpResponse.InvalidResponse);
				}
				if (this.nextHopConnection != null)
				{
					this.AckConnection(AckStatus.Retry, SmtpResponse.InvalidResponse, SessionSetupFailureReason.ProtocolError);
				}
				this.Disconnect();
			}
		}

		private void TlsNegotiationComplete()
		{
			ExTraceGlobals.SmtpSendTracer.TraceDebug<long>((long)this.GetHashCode(), "TLS negotiation completed for connection {0}. Reissue Ehlo", this.connectionId);
			if (this.connection.RemoteCertificate == null)
			{
				ExTraceGlobals.SmtpSendTracer.TraceError((long)this.GetHashCode(), "No remote certificate present");
				this.HandleError(SecurityStatus.IncompleteCredentials);
				return;
			}
			this.logSession.LogCertificateThumbprint("Received certificate", this.connection.RemoteCertificate.Certificate);
			this.secureState &= ~SecureState.NegotiationRequested;
			if (!this.IsOpportunisticTls && this.connection.TlsCipherKeySize < 128)
			{
				ExTraceGlobals.SmtpSendTracer.TraceError<int>((long)this.GetHashCode(), "Quit session because Tls cipher strength is too weak at {0}", this.connection.TlsCipherKeySize);
				this.logSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "Tls cipher strength is too weak");
				this.FailoverConnection(SmtpResponse.AuthTempFailureTLSCipherTooWeak, SessionSetupFailureReason.ProtocolError);
				this.NextState = SmtpOutSession.SessionState.Quit;
				this.MoveToNextState();
				return;
			}
			if (this.IsOpportunisticTls)
			{
				this.AckDetails.ExtraEventData.Add(new KeyValuePair<string, string>("Microsoft.Exchange.Transport.MailRecipient.EffectiveTlsAuthLevel", SmtpOutSession.AuthLevelToString(new RequiredTlsAuthLevel?(RequiredTlsAuthLevel.EncryptionOnly))));
			}
			if (this.RemoteIdentity == SmtpOutSession.anonymousSecurityIdentifier)
			{
				ExTraceGlobals.SmtpSendTracer.TraceDebug<string>((long)this.GetHashCode(), "Remote has supplied certificate {0}", this.connection.RemoteCertificate.Subject);
				if (this.secureState == SecureState.AnonymousTls && this.RequiresDirectTrust)
				{
					this.RemoteIdentity = DirectTrust.MapCertToSecurityIdentifier(this.connection.RemoteCertificate.Certificate);
					if (!(this.RemoteIdentity != SmtpOutSession.anonymousSecurityIdentifier))
					{
						string text = "DirectTrust certificate failed to authenticate";
						ExTraceGlobals.SmtpSendTracer.TraceError((long)this.GetHashCode(), text);
						SmtpOutConnection.Events.LogEvent(TransportEventLogConstants.Tuple_SmtpSendDirectTrustFailed, this.RemoteEndPoint.Address.ToString(), new object[]
						{
							this.connection.RemoteCertificate.Subject,
							this.RemoteEndPoint.Address
						});
						EventNotificationItem.Publish(ExchangeComponent.Transport.Name, "SmtpSendDirectTrustFailed", null, text, ResultSeverityLevel.Error, false);
						this.logSession.LogInformation(ProtocolLoggingLevel.Verbose, null, text);
						this.FailoverConnection(SmtpResponse.CertificateValidationFailure, SessionSetupFailureReason.ProtocolError);
						this.NextState = SmtpOutSession.SessionState.Quit;
						this.MoveToNextState();
						return;
					}
					this.RemoteIdentityName = this.connection.RemoteCertificate.Subject;
					this.SetSessionPermissions(this.RemoteIdentity);
					SmtpSessionCertificateUse use = (this.secureState == SecureState.StartTls) ? SmtpSessionCertificateUse.RemoteSTARTTLS : SmtpSessionCertificateUse.RemoteDirectTrust;
					CertificateExpiryCheck.CheckCertificateExpiry(this.connection.RemoteCertificate.Certificate, SmtpOutConnection.Events, use, this.connection.RemoteCertificate.Subject);
					this.logSession.LogCertificate("DirectTrust certificate", this.connection.RemoteCertificate.Certificate);
				}
				else if (this.Connector.SmartHostAuthMechanism == SmtpSendConnectorConfig.AuthMechanisms.BasicAuthRequireTLS)
				{
					ChainValidityStatus chainValidityStatus = ChainValidityStatus.SubjectMismatch;
					foreach (SmartHost smartHost in this.smtpOutConnection.Connector.SmartHosts)
					{
						string s = smartHost.ToString();
						SmtpDomainWithSubdomains domain;
						if (SmtpDomainWithSubdomains.TryParse(s, out domain) && this.certificateValidator.MatchCertificateFqdns(domain, this.connection.RemoteCertificate, MatchOptions.None, this.logSession))
						{
							chainValidityStatus = this.certificateValidator.ChainValidateAsAnonymous(this.connection.RemoteCertificate.Certificate, true);
							break;
						}
					}
					this.logSession.LogInformation(ProtocolLoggingLevel.Verbose, Encoding.UTF8.GetBytes(chainValidityStatus.ToString()), "Chain validation status");
					if (chainValidityStatus != ChainValidityStatus.Valid)
					{
						SmtpOutConnection.Events.LogEvent(TransportEventLogConstants.Tuple_MessageSecurityTLSCertificateValidationFailure, this.smtpOutConnection.Connector.Name, new object[]
						{
							this.smtpOutConnection.Connector.Name,
							chainValidityStatus
						});
						string notificationReason = string.Format("Unable to validate the TLS certificate of the smart host for the connector {0}. The certificate validation error for the certificate is {1}.", this.smtpOutConnection.Connector.Name, chainValidityStatus);
						EventNotificationItem.Publish(ExchangeComponent.Transport.Name, "MessageSecurityTLSCertificateValidationFailure", null, notificationReason, ResultSeverityLevel.Error, false);
						this.FailoverConnection(SmtpResponse.CertificateValidationFailure, SessionSetupFailureReason.ProtocolError);
						this.NextState = SmtpOutSession.SessionState.Quit;
						this.MoveToNextState();
						return;
					}
					this.logSession.LogCertificate("SmartHost certificate", this.connection.RemoteCertificate.Certificate);
				}
				else if (this.transportConfiguration.TransportSettings.TransportSettings.IsTLSSendSecureDomain(this.NextHopConnection.Key.NextHopDomain))
				{
					string nextHopDomain = this.NextHopConnection.Key.NextHopDomain;
					if (!this.Connector.DomainSecureEnabled)
					{
						ExTraceGlobals.SmtpSendTracer.TraceError<string, string>((long)this.GetHashCode(), "Message to secure domain '{0}' failed because DomainSecureEnabled on send connector '{1}' was set to false", nextHopDomain, this.Connector.Name);
						SmtpOutConnection.Events.LogEvent(TransportEventLogConstants.Tuple_TlsDomainSecureDisabled, this.Connector.Name + " - " + nextHopDomain, new object[]
						{
							nextHopDomain,
							this.Connector.Name
						});
						this.FailoverConnection(SmtpResponse.DomainSecureDisabled, SessionSetupFailureReason.ProtocolError);
						this.NextState = SmtpOutSession.SessionState.Quit;
						this.MoveToNextState();
						return;
					}
					SmtpDomainWithSubdomains domain2 = new SmtpDomainWithSubdomains(new SmtpDomain(nextHopDomain), true);
					ChainValidityStatus chainValidityStatus2;
					if (!this.certificateValidator.MatchCertificateFqdns(domain2, this.connection.RemoteCertificate, MatchOptions.MultiLevelCertWildcards, this.logSession))
					{
						chainValidityStatus2 = ChainValidityStatus.SubjectMismatch;
					}
					else
					{
						chainValidityStatus2 = this.certificateValidator.ChainValidateAsAnonymous(this.connection.RemoteCertificate.Certificate, true);
					}
					if (chainValidityStatus2 != ChainValidityStatus.Valid && chainValidityStatus2 != (ChainValidityStatus)2148081683U)
					{
						Utils.SecureMailPerfCounters.DomainSecureOutboundSessionFailuresTotal.Increment();
						if (chainValidityStatus2 == ChainValidityStatus.SubjectMismatch)
						{
							string text2 = string.Format("Message to secure domain '{0}' on connector '{1}' failed because the TLS server certificate subject did not match.", nextHopDomain, this.Connector.Name);
							ExTraceGlobals.SmtpSendTracer.TraceDebug((long)this.GetHashCode(), text2);
							SmtpOutConnection.Events.LogEvent(TransportEventLogConstants.Tuple_TlsDomainServerCertificateSubjectMismatch, nextHopDomain, new object[]
							{
								nextHopDomain,
								this.Connector.Name,
								this.connection.RemoteCertificate.Subject
							});
							EventNotificationItem.Publish(ExchangeComponent.Transport.Name, "TlsDomainServerCertificateSubjectMismatch", null, text2, ResultSeverityLevel.Warning, false);
						}
						else
						{
							string text3 = string.Format("Message to secure domain '{0}' on connector '{1}' failed because the TLS server certificate chain failed to validate and returned status '{2}'.", nextHopDomain, this.Connector.Name, chainValidityStatus2);
							ExTraceGlobals.SmtpSendTracer.TraceDebug((long)this.GetHashCode(), text3);
							SmtpOutConnection.Events.LogEvent(TransportEventLogConstants.Tuple_TlsDomainServerCertificateValidationFailure, nextHopDomain, new object[]
							{
								nextHopDomain,
								this.Connector.Name,
								chainValidityStatus2
							});
							EventNotificationItem.Publish(ExchangeComponent.Transport.Name, "TlsDomainServerCertificateValidationFailure", null, text3, ResultSeverityLevel.Error, false);
						}
						if (!this.certificateValidator.ShouldTreatValidationResultAsSuccess(chainValidityStatus2))
						{
							this.FailoverConnection(SmtpResponse.CertificateValidationFailure, SessionSetupFailureReason.ProtocolError);
							this.NextState = SmtpOutSession.SessionState.Quit;
							this.MoveToNextState();
							return;
						}
						SmtpOutConnection.Events.LogEvent(TransportEventLogConstants.Tuple_CertificateRevocationListCheckTrasientFailureTreatedAsSuccess, this.connection.RemoteCertificate.SerialNumber, new object[]
						{
							chainValidityStatus2.ToString(),
							this.connection.RemoteCertificate.SerialNumber,
							this.connection.RemoteCertificate.Subject,
							this.connection.RemoteCertificate.Issuer,
							this.connection.RemoteCertificate.Thumbprint,
							"SmtpOut"
						});
						this.logSession.LogCertificate(string.Format(CultureInfo.InvariantCulture, "CRL validation failed with status {0}. Treating the failure as success.", new object[]
						{
							chainValidityStatus2
						}), this.connection.RemoteCertificate.Certificate);
					}
					ExTraceGlobals.SmtpSendTracer.TraceDebug((long)this.GetHashCode(), "Upgrade remote identity to Partner.");
					this.RemoteIdentity = WellKnownSids.PartnerServers;
					this.RemoteIdentityName = nextHopDomain;
					this.SetSessionPermissions(this.Connector.GetPartnerPermissions());
					this.logSession.LogCertificate("Secure domain certificate", this.connection.RemoteCertificate.Certificate);
				}
				else if (this.TlsConfiguration.RequireTls && (this.TlsConfiguration.TlsAuthLevel == RequiredTlsAuthLevel.CertificateValidation || this.TlsConfiguration.TlsAuthLevel == RequiredTlsAuthLevel.DomainValidation))
				{
					ChainValidityStatus chainValidityStatus3 = ChainValidityStatus.Valid;
					RequiredTlsAuthLevel valueOrDefault = this.TlsConfiguration.TlsAuthLevel.GetValueOrDefault();
					RequiredTlsAuthLevel? requiredTlsAuthLevel;
					if (requiredTlsAuthLevel != null)
					{
						switch (valueOrDefault)
						{
						case RequiredTlsAuthLevel.CertificateValidation:
							chainValidityStatus3 = this.certificateValidator.ChainValidateAsAnonymous(this.connection.RemoteCertificate.Certificate, this.transportAppConfig.SmtpSendConfiguration.CacheOnlyUrlRetrievalForRemoteCertChain);
							break;
						case RequiredTlsAuthLevel.DomainValidation:
							if (!SmtpOutSession.MatchCertificateWithTlsDomain(this.TlsConfiguration.TlsDomains, this.connection.RemoteCertificate, this.logSession, this.certificateValidator))
							{
								chainValidityStatus3 = ChainValidityStatus.SubjectMismatch;
							}
							else
							{
								chainValidityStatus3 = this.certificateValidator.ChainValidateAsAnonymous(this.connection.RemoteCertificate.Certificate, this.transportAppConfig.SmtpSendConfiguration.CacheOnlyUrlRetrievalForRemoteCertChain);
							}
							break;
						}
					}
					if (chainValidityStatus3 != ChainValidityStatus.Valid)
					{
						ExTraceGlobals.SmtpSendTracer.TraceError((long)this.GetHashCode(), "Outbound TLS authentication failed with error {0} for Send connector {1}. The TLS authentication mechanism is {3}.Target is {4}.", new object[]
						{
							chainValidityStatus3.ToString(),
							this.Connector.Name,
							this.TlsConfiguration.TlsAuthLevel.ToString(),
							this.NextHopConnection.Key.NextHopDomain
						});
						SmtpOutConnection.Events.LogEvent(TransportEventLogConstants.Tuple_SmtpSendOutboundAtTLSAuthLevelFailed, this.NextHopConnection.Key.NextHopDomain, new object[]
						{
							chainValidityStatus3.ToString(),
							this.Connector.Name,
							this.TlsConfiguration.TlsAuthLevel.ToString(),
							this.NextHopConnection.Key.NextHopDomain
						});
						this.logSession.LogInformation(ProtocolLoggingLevel.Verbose, null, string.Format("Outbound TLS authentication failed for auth level {0} with error {1}", this.TlsConfiguration.TlsAuthLevel.ToString(), chainValidityStatus3.ToString()));
						if (!Components.CertificateComponent.Validator.ShouldTreatValidationResultAsSuccess(chainValidityStatus3))
						{
							this.FailoverConnection(SmtpResponse.CertificateValidationFailure, SessionSetupFailureReason.ProtocolError);
							this.NextState = SmtpOutSession.SessionState.Quit;
							this.MoveToNextState();
							return;
						}
						ExTraceGlobals.SmtpSendTracer.TraceDebug<ChainValidityStatus>((long)this.GetHashCode(), "Treating certification validation failure {0} as succcess", chainValidityStatus3);
						SmtpOutConnection.Events.LogEvent(TransportEventLogConstants.Tuple_CertificateRevocationListCheckTrasientFailureTreatedAsSuccess, this.connection.RemoteCertificate.SerialNumber, new object[]
						{
							chainValidityStatus3.ToString(),
							this.connection.RemoteCertificate.SerialNumber,
							this.connection.RemoteCertificate.Subject,
							this.connection.RemoteCertificate.Issuer,
							this.connection.RemoteCertificate.Thumbprint,
							"SmtpOut"
						});
						this.logSession.LogCertificate(string.Format(CultureInfo.InvariantCulture, "CRL validation failed with status {0}. Treating the failure as success.", new object[]
						{
							chainValidityStatus3
						}), this.connection.RemoteCertificate.Certificate);
					}
					this.AckDetails.ExtraEventData.Add(new KeyValuePair<string, string>("Microsoft.Exchange.Transport.MailRecipient.EffectiveTlsAuthLevel", SmtpOutSession.AuthLevelToString(this.TlsConfiguration.TlsAuthLevel)));
				}
			}
			this.NextState = SmtpOutSession.SessionState.Ehlo;
			this.MoveToNextState();
		}

		private bool PreCheckMessageSize()
		{
			this.needToDownconvertMIME = this.RequiresDownConversion();
			if (!this.needToDownconvertMIME)
			{
				long mimeSize = this.RoutedMailItem.MimeSize;
				if (!this.RemoteIsAuthenticated && !this.IsAuthenticated && this.AdvertisedEhloOptions.Size == SizeMode.Enabled && this.AdvertisedEhloOptions.MaxSize > 0L && mimeSize > this.AdvertisedEhloOptions.MaxSize)
				{
					ExTraceGlobals.SmtpSendTracer.TraceError((long)this.GetHashCode(), "Message from {0} was NDR'ed because the size was {1} whereas the maximum allowed size by the receiving server (at {2}) was {3}", new object[]
					{
						this.RoutedMailItem.From.ToString(),
						mimeSize,
						this.sessionProps.RemoteEndPoint,
						this.AdvertisedEhloOptions.MaxSize
					});
					this.RoutedMailItem.AddDsnParameters("MaxMessageSizeInKB", this.AdvertisedEhloOptions.MaxSize >> 10);
					this.RoutedMailItem.AddDsnParameters("CurrentMessageSizeInKB", mimeSize >> 10);
					this.AckMessage(AckStatus.Fail, AckReason.OverAdvertisedSizeLimit);
					return false;
				}
			}
			return true;
		}

		private bool CheckLongRecipientSupport(MailRecipient recipient, bool supportLongAddresses)
		{
			if (Util.IsLongAddress(recipient.Email))
			{
				if (!Util.IsValidInnerAddress(recipient.Email))
				{
					ExTraceGlobals.SmtpSendTracer.TraceError<string>((long)this.GetHashCode(), "Recipient '{0}' was failed because the long address is invalid", recipient.Email.ToString());
					recipient.Ack(AckStatus.Fail, AckReason.SmtpSendInvalidLongRecipientAddress);
					return false;
				}
				if (!supportLongAddresses)
				{
					ExTraceGlobals.SmtpSendTracer.TraceError<string>((long)this.GetHashCode(), "Recipient '{0}' was failed because the session does not support long addresses", recipient.Email.ToString());
					recipient.Ack(AckStatus.Fail, AckReason.SmtpSendLongRecipientAddress);
					return false;
				}
			}
			return true;
		}

		private bool CheckOrarSupport(MailRecipient recipient, bool supportOrar, bool supportLongAddresses)
		{
			RoutingAddress address;
			if (!OrarGenerator.TryGetOrarAddress(recipient, out address))
			{
				return true;
			}
			if (!supportOrar)
			{
				ExTraceGlobals.SmtpSendTracer.TraceError<string>((long)this.GetHashCode(), "Recipient '{0}' was failed because the ORAR address could not be transmitted", recipient.Email.ToString());
				SmtpOutConnection.Events.LogEvent(TransportEventLogConstants.Tuple_SmtpSendUnableToTransmitOrar, null, new object[]
				{
					this.AdvertisedEhloOptions.AdvertisedFQDN,
					this.Connector.Name,
					this.RoutedMailItem.InternetMessageId,
					recipient.Email.ToString()
				});
				recipient.Ack(AckStatus.Fail, AckReason.SmtpSendOrarNotTransmittable);
				return false;
			}
			if (!supportLongAddresses && Util.IsLongAddress(address))
			{
				ExTraceGlobals.SmtpSendTracer.TraceError<string>((long)this.GetHashCode(), "Recipient '{0}' was failed because the long ORAR address could not be transmitted", recipient.Email.ToString());
				SmtpOutConnection.Events.LogEvent(TransportEventLogConstants.Tuple_SmtpSendUnableToTransmitLongOrar, null, new object[]
				{
					address.ToString(),
					this.AdvertisedEhloOptions.AdvertisedFQDN,
					this.Connector.Name,
					this.RoutedMailItem.InternetMessageId,
					recipient.Email.ToString()
				});
				recipient.Ack(AckStatus.Fail, AckReason.SmtpSendLongOrarNotTransmittable);
				return false;
			}
			if (!this.AdvertisedEhloOptions.XOrar)
			{
				this.exch50DataPresent = true;
			}
			return true;
		}

		private bool CheckRDstSupport(MailRecipient recipient, bool supportRDst)
		{
			if (recipient.ExtendedProperties.Contains("Microsoft.Exchange.Transport.RoutingOverride"))
			{
				if (!supportRDst)
				{
					ExTraceGlobals.SmtpSendTracer.TraceError<string>((long)this.GetHashCode(), "Recipient '{0}' was failed because Routing Destination could not be transmitted", recipient.Email.ToString());
					SmtpOutConnection.Events.LogEvent(TransportEventLogConstants.Tuple_SmtpSendUnableToTransmitRDst, null, new object[]
					{
						this.AdvertisedEhloOptions.AdvertisedFQDN,
						this.Connector.Name,
						this.RoutedMailItem.InternetMessageId,
						recipient.Email.ToString()
					});
					recipient.Ack(AckStatus.Fail, AckReason.SmtpSendRDstNotTransmittable);
					return false;
				}
				if (!this.AdvertisedEhloOptions.XRDst)
				{
					this.exch50DataPresent = true;
				}
			}
			return true;
		}

		private bool IsHubDeliveringToMailbox(ProcessTransportRole processTransportRole)
		{
			return processTransportRole == ProcessTransportRole.Hub && this.NextHopDeliveryType == DeliveryType.SmtpDeliveryToMailbox;
		}

		private bool IsHubDeliveringToNonMailbox(ProcessTransportRole processTransportRole)
		{
			return processTransportRole == ProcessTransportRole.Hub && this.NextHopDeliveryType != DeliveryType.SmtpDeliveryToMailbox;
		}

		private const string MailCommand = "MAIL";

		private const string RcptCommand = "RCPT";

		private const string RsetCommand = "RSET";

		private const string DataCommand = "DATA";

		private const int MaxOutstandingResponses = 100;

		private const int MaxPipelinedCommands = 200;

		private const int MaxResponseLength = 32768;

		private const int NumberOfBreadcrumbs = 64;

		public static readonly byte[] BinaryData = Util.AsciiStringToBytesAndAppendCRLF("<Binary Data>");

		protected TransportAppConfig transportAppConfig;

		protected ITransportConfiguration transportConfiguration;

		protected NetworkConnection connection;

		protected bool dontCacheThisConnection;

		protected BufferBuilder sendBuffer = new BufferBuilder();

		protected ShadowRedundancyManager shadowRedundancyManager;

		protected RecipientCorrelator recipientCorrelator;

		protected IProtocolLogSession logSession;

		private static readonly AsyncCallback writeCompleteReadLine = new AsyncCallback(SmtpOutSession.WriteCompleteReadLine);

		private static readonly AsyncCallback writeBdatCompleteSendStream = new AsyncCallback(SmtpOutSession.WriteBdatCompleteSendStream);

		private static readonly AsyncCallback writeStreamCompleteReadLine = new AsyncCallback(SmtpOutSession.WriteStreamCompleteReadLine);

		private static readonly AsyncCallback tlsNegotiationComplete = new AsyncCallback(SmtpOutSession.TlsNegotiationComplete);

		private static readonly AsyncCallback readLineComplete = new AsyncCallback(SmtpOutSession.ReadLineComplete);

		private static readonly SecurityIdentifier anonymousSecurityIdentifier = new SecurityIdentifier(WellKnownSidType.AnonymousSid, null);

		private readonly SmtpOutSession.CommandList[] commandLists;

		private readonly bool isProbeSession;

		private SmtpOutConnection smtpOutConnection;

		private DateTime sessionStartTime;

		private int messageCount;

		private int messageSendAttemptCount;

		private long bytesSentAtLastCount;

		private NextHopConnection nextHopConnection;

		private IReadOnlyMailItem routedMailItem;

		private bool shadowCurrentMailItem;

		private bool shutdownConnectionCalled;

		private MailRecipient currentRecipient;

		private int numberOfRecipientsAttempted;

		private int numberOfRecipientsSucceeded;

		private int numberOfRecipientsAckedForRetry;

		private int numberOfRecipientsAcked;

		private Stream messageStream;

		private MailRecipient nextRecipient;

		private Queue pipelinedCommandQueue;

		private Queue pipelinedResponseQueue;

		private List<string> response;

		private bool betweenMessagesRset;

		private bool issueBetweenMsgRset;

		private bool doPrepareForNextMessage;

		private SecureState secureState;

		private SecurityIdentifier sessionRemoteIdentity = SmtpOutSession.anonymousSecurityIdentifier;

		private string sessionRemoteIdentityName = "anonymous";

		private X509Certificate2 advertisedTlsCertificate;

		private X509Certificate2 internalTransportCertificate;

		private SecureString connectorPassword;

		private MultilevelAuthMechanism authMethod;

		private bool isAuthenticated;

		private Permission sessionPermissions;

		private bool shadowRedundancyEnabled;

		private bool shadowed;

		private bool disconnected;

		private AckDetails ackDetails;

		private bool presentInSmtpOutSessionCache;

		private SmtpOutSession.SessionState currentState;

		private SmtpOutSession.SessionState nextState;

		private SmtpOutSession.CommandList.CommandListEnumerator currentEnumeratorInPipeline;

		private SmtpOutSession.CommandList currentCommandListInPipeline;

		private string lastCmdNotSent;

		private bool lastCommandPipelined;

		private bool recipsAckedPending;

		private int numRcptCommandsInPipelineQueue;

		private SmtpSessionProps sessionProps;

		private long connectionId;

		private SmtpSendConnectorConfig sendConnector;

		private bool usingHELO;

		private readonly SmtpSendPerfCountersInstance smtpSendPerformanceCounters;

		private bool needToDownconvertMIME;

		private bool pipeLineNextMessagePending;

		private bool pipeLineFailOverPending;

		private SmtpResponse pipeLineFailOverResponse;

		private BufferBuilder responseBuffer;

		private Breadcrumbs<SmtpOutSession.SmtpOutSessionBreadcrumbs> breadcrumbs = new Breadcrumbs<SmtpOutSession.SmtpOutSessionBreadcrumbs>(64);

		private bool failoverInProgress;

		private bool exch50DataPresent;

		private bool useDowngradedExchangeServerAuth;

		private IMailRouter mailRouter;

		private CertificateCache certificateCache;

		private CertificateValidator certificateValidator;

		private Queue<SmtpMessageContextBlob> blobsToSend;

		private Queue<string> remainingXProxyToCommands;

		private SmtpSendConnectorConfig outboundProxySendConnector;

		private bool nextHopIsProxyingBlindly;

		private string helloDomainOfOutboundProxyFrontEnd;

		private bool xRsetProxyToAccepted;

		private SmtpOutSession.OutboundProxyOriginalSessionState outboundProxyOriginalSessionState;

		private int messagesSentOverSession;

		public enum SessionState
		{
			ConnectResponse,
			Ehlo,
			Helo,
			Auth,
			Exps,
			StartTLS,
			AnonymousTLS,
			MessageStart,
			PerRecipient,
			Data,
			Xexch50,
			Bdat,
			XBdatBlob,
			XShadow,
			XQDiscard,
			XProxy,
			XProxyFrom,
			XProxyTo,
			XSessionParams,
			Quit,
			Rset,
			Inactive,
			XShadowRequest,
			XRsetProxyTo,
			NumStates
		}

		public enum SmtpOutSessionBreadcrumbs
		{
			EMPTY,
			FailoverConnection,
			SetNextStateToQuit,
			AckConnection,
			ResetSession,
			PrepareForNextMessageOnCachedSession,
			AckMessage,
			SetNextStateForCachedSession,
			CreateCmdConnectResponse,
			CreateCmdEhlo,
			CreateCmdHelo,
			CreateCmdAuth,
			CreateCmdStarttls,
			CreateCmdMail,
			CreateCmdRcpt,
			CreateCmdXexch50,
			CreateCmdData,
			Disconnect,
			CreateCmdBdat,
			CreateCmdRset,
			CreateCmdQuit,
			EnqueueResponseHandler,
			MoveToNextState,
			SendPipelinedCommands,
			SendBufferThenReadLine,
			SendBdatStream,
			SendDataStream,
			InvokeCommandHandler,
			InvokeResponseHandler,
			PrepareForNextMessage,
			SendNextCommands,
			ShutdownConnection,
			CreateCmdXShadow,
			CreateCmdXQDiscard,
			PrepareNextStateForEstablishedSession,
			InboundProxyCreateCmdConnectResponse,
			InboundProxyCreateCmdEhlo,
			InboundProxyCreateCmdAuth,
			InboundProxyCreateCmdStarttls,
			InboundProxyCreateCmdMail,
			InboundProxyCreateCmdRcpt,
			InboundProxyCreateCmdData,
			InboundProxyCreateCmdBdat,
			InboundProxyCreateCmdRset,
			InboundProxyCreateCmdQuit,
			InboundProxyShutdownConnection,
			InboundProxyInvokeCommandHandler,
			InboundProxySendDataBuffers,
			InboundProxyWriteBdatCompleteSendBuffers,
			InboundProxyReadFromProxyLayerComplete,
			InboundProxyWriteProxiedBytesToTargetComplete,
			InboundProxyCreateCmdXProxyFrom,
			InboundProxyPrepareForNextMessageOnCachedSession,
			CreateCmdXProxyTo,
			PrepareSendXshadowOrMessage,
			ShadowCreateCmdConnectResponse,
			ShadowCreateCmdEhlo,
			ShadowCreateCmdAuth,
			ShadowCreateCmdStarttls,
			ShadowCreateCmdXShadowRequest,
			ShadowCreateCmdRset,
			ShadowCreateCmdQuit,
			ShadowCreateCmdMail,
			ShadowCreateCmdRcpt,
			ShadowCreateCmdData,
			ShadowCreateCmdBdat,
			CreateCmdXSessionParams,
			SerializeExtendedPropertiesBlob,
			SerializeAdrcPropertiesBlob,
			SerializeFastIndexBlob,
			CreateCmdXRsetProxyTo
		}

		[Serializable]
		public class CommandList : IEnumerable
		{
			public CommandList(SmtpOutSession.SessionState state)
			{
				this.commands = new HybridDictionary();
				this.commands.Add(this.highestIndex, SmtpOutSession.CommandList.protocolCommands[(int)state]);
			}

			private int LowestIndex
			{
				get
				{
					return this.lowestIndex;
				}
			}

			private int HighestIndex
			{
				get
				{
					return this.highestIndex;
				}
			}

			private string this[int index]
			{
				get
				{
					if (this.commands.Contains(index))
					{
						return (string)this.commands[index];
					}
					return null;
				}
			}

			public void AddCommandToBeginningOfList(string command)
			{
				this.lowestIndex--;
				this.commands.Add(this.lowestIndex, command);
			}

			public void AddCommandToEndOfList(string command)
			{
				this.highestIndex++;
				this.commands.Add(this.highestIndex, command);
			}

			public bool HasCommandsBeforePredefined()
			{
				return this.lowestIndex < 0;
			}

			public bool HasCommandsAfterPredefined()
			{
				return this.highestIndex > 0;
			}

			public bool HasAddedCommands()
			{
				return this.HasCommandsBeforePredefined() || this.HasCommandsAfterPredefined();
			}

			public void Commit(SmtpOutSession.CommandList commandList)
			{
				this.commands = commandList.commands;
				this.lowestIndex = commandList.lowestIndex;
				this.highestIndex = commandList.highestIndex;
			}

			public IEnumerator GetEnumerator()
			{
				return new SmtpOutSession.CommandList.CommandListEnumerator(this);
			}

			private static string[] InitializeProtocolCommands()
			{
				string[] array = new string[24];
				array[0] = "ConnectResponse";
				array[1] = "EHLO";
				array[3] = "AUTH";
				array[4] = "X-EXPS";
				array[5] = "STARTTLS";
				array[6] = "X-ANONYMOUSTLS";
				array[2] = "HELO";
				array[7] = "MAIL";
				array[8] = "RCPT";
				array[10] = "XEXCH50";
				array[9] = "DATA";
				array[11] = "BDAT";
				array[12] = "XBDATBLOB";
				array[13] = "XSHADOW";
				array[22] = "XSHADOWREQUEST";
				array[14] = "XQDISCARD";
				array[15] = "XPROXY";
				array[16] = "XPROXYFROM";
				array[17] = "XPROXYTO";
				array[18] = "XSESSIONPARAMS";
				array[20] = "RSET";
				array[19] = "QUIT";
				array[23] = "XRSETPROXYTO";
				return array;
			}

			private static string[] protocolCommands = SmtpOutSession.CommandList.InitializeProtocolCommands();

			private HybridDictionary commands;

			private int lowestIndex;

			private int highestIndex;

			public class CommandListEnumerator : IEnumerator
			{
				public CommandListEnumerator(SmtpOutSession.CommandList commandList)
				{
					this.commandList = commandList;
					this.Reset();
				}

				public object Current
				{
					get
					{
						return this.commandList[this.index];
					}
				}

				public bool HasNext
				{
					get
					{
						return this.index < this.commandList.HighestIndex;
					}
				}

				public void Reset()
				{
					this.index = this.commandList.LowestIndex - 1;
				}

				public bool MoveNext()
				{
					if (this.HasNext)
					{
						this.index++;
						return true;
					}
					return false;
				}

				private SmtpOutSession.CommandList commandList;

				private int index = -1;
			}
		}

		private class OutboundProxyOriginalSessionState
		{
			public OutboundProxyOriginalSessionState(IEhloOptions ehloOptions, SmtpSendConnectorConfig sendConnector, SecurityIdentifier remoteIdentity, string remoteIdentityName, AckDetails ackDetails, Permission sessionPermissions)
			{
				this.EhloOptions = ehloOptions;
				this.SendConnector = sendConnector;
				this.RemoteIdentity = remoteIdentity;
				this.RemoteIdentityName = remoteIdentityName;
				this.AckDetails = ackDetails;
				this.SessionPermissions = sessionPermissions;
			}

			public readonly IEhloOptions EhloOptions;

			public readonly SmtpSendConnectorConfig SendConnector;

			public readonly SecurityIdentifier RemoteIdentity;

			public readonly string RemoteIdentityName;

			public readonly AckDetails AckDetails;

			public readonly Permission SessionPermissions;
		}
	}
}
