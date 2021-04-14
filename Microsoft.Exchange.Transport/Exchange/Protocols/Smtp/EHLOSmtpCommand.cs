using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.Transport;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class EHLOSmtpCommand : SmtpCommand
	{
		public EHLOSmtpCommand(ISmtpSession session, ITransportConfiguration transportConfiguration) : base(session, "EHLO", "OnEhloCommand", LatencyComponent.None)
		{
			this.ehloCommandEventArgs = new EhloCommandEventArgs();
			this.CommandEventArgs = this.ehloCommandEventArgs;
			this.transportConfiguration = transportConfiguration;
		}

		internal string HelloDomain
		{
			get
			{
				return this.ehloCommandEventArgs.Domain;
			}
			set
			{
				this.ehloCommandEventArgs.Domain = value;
			}
		}

		internal override void InboundParseCommand()
		{
			ISmtpInSession smtpInSession = (ISmtpInSession)base.SmtpSession;
			smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.EHLOInboundParseCommand);
			if (!base.VerifyNoOngoingBdat())
			{
				smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.WrongSequence);
				return;
			}
			HeloEhloParseOutput heloEhloParseOutput;
			ParseResult parseResult = HeloSmtpCommandParser.Parse(CommandContext.FromSmtpCommand(this), SmtpInSessionState.FromSmtpInSession(smtpInSession), HeloOrEhlo.Ehlo, out heloEhloParseOutput);
			if (!parseResult.IsFailed && !string.IsNullOrEmpty(heloEhloParseOutput.HeloDomain))
			{
				this.HelloDomain = heloEhloParseOutput.HeloDomain;
				base.CurrentOffset = base.ProtocolCommandLength;
			}
			base.SmtpResponse = parseResult.SmtpResponse;
			base.ParsingStatus = parseResult.ParsingStatus;
		}

		internal override void InboundProcessCommand()
		{
			if (base.ParsingStatus != ParsingStatus.Complete)
			{
				return;
			}
			ISmtpInSession smtpInSession = (ISmtpInSession)base.SmtpSession;
			smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.EHLOInboundProcessCommand);
			if ((smtpInSession.SecureState == SecureState.AnonymousTls || smtpInSession.SecureState == SecureState.StartTls) && smtpInSession.TlsCipherKeySize < 128)
			{
				ExTraceGlobals.SmtpReceiveTracer.TraceError<int>((long)this.GetHashCode(), "Negotiated TLS cipher strength is too weak at {0} bit.", smtpInSession.TlsCipherKeySize);
				base.SmtpResponse = SmtpResponse.AuthTempFailureTLSCipherTooWeak;
				smtpInSession.Disconnect(DisconnectReason.DroppedSession);
				return;
			}
			if (smtpInSession.SecureState == SecureState.AnonymousTls && smtpInSession.TlsRemoteCertificate != null && smtpInSession.RemoteIdentity == SmtpConstants.AnonymousSecurityIdentifier)
			{
				base.SmtpResponse = SmtpResponse.AuthTempFailure;
				smtpInSession.Disconnect(DisconnectReason.DroppedSession);
				return;
			}
			if (smtpInSession.SeenHelo || smtpInSession.SeenEhlo)
			{
				base.LowAuthenticationLevelTarpitOverride = TarpitAction.DoTarpit;
			}
			if (smtpInSession.DisableStartTls)
			{
				smtpInSession.AdvertisedEhloOptions.StartTLS = false;
			}
			smtpInSession.HelloSmtpDomain = this.HelloDomain;
			smtpInSession.SeenHelo = true;
			smtpInSession.SeenEhlo = true;
			smtpInSession.AbortMailTransaction();
			EhloOptions ehloOptions = ((EhloOptions)smtpInSession.AdvertisedEhloOptions).Clone();
			if ((smtpInSession.Connector.AuthMechanism & AuthMechanisms.Tls) != AuthMechanisms.None && smtpInSession.SecureState == SecureState.None)
			{
				if (smtpInSession.StartTlsSupported && !smtpInSession.DisableStartTls)
				{
					ehloOptions.StartTLS = true;
				}
				else
				{
					ExTraceGlobals.SmtpReceiveTracer.TraceError((long)this.GetHashCode(), "Receive connector is configured to advertise STARTTLS but we can't locate the proper certificate for it OR STARTTLS has been disabled by agent");
				}
			}
			if ((smtpInSession.Connector.AuthMechanism & AuthMechanisms.BasicAuth) != AuthMechanisms.None)
			{
				if ((smtpInSession.Connector.AuthMechanism & AuthMechanisms.BasicAuthRequireTLS) != AuthMechanisms.None)
				{
					if ((smtpInSession.SecureState == SecureState.StartTls || smtpInSession.SecureState == SecureState.AnonymousTls) && !ehloOptions.AuthenticationMechanisms.Contains("AUTH LOGIN"))
					{
						ehloOptions.AuthenticationMechanisms.Add("AUTH LOGIN");
					}
				}
				else if (!ehloOptions.AuthenticationMechanisms.Contains("AUTH LOGIN"))
				{
					ehloOptions.AuthenticationMechanisms.Add("AUTH LOGIN");
				}
			}
			if (smtpInSession.SupportIntegratedAuth)
			{
				if (smtpInSession.Connector.EnableAuthGSSAPI && !ehloOptions.AuthenticationMechanisms.Contains("AUTH GSSAPI"))
				{
					ehloOptions.AuthenticationMechanisms.Add("AUTH GSSAPI");
				}
				if (!ehloOptions.AuthenticationMechanisms.Contains("AUTH NTLM"))
				{
					ehloOptions.AuthenticationMechanisms.Add("AUTH NTLM");
				}
			}
			if ((smtpInSession.Connector.AuthMechanism & AuthMechanisms.ExchangeServer) != AuthMechanisms.None)
			{
				if (smtpInSession.SecureState == SecureState.None && smtpInSession.AnonymousTlsSupported)
				{
					ehloOptions.AnonymousTLS = true;
				}
				if (smtpInSession.SmtpInServer.IsBridgehead)
				{
					if (!ehloOptions.AuthenticationMechanisms.Contains("X-EXPS GSSAPI"))
					{
						ehloOptions.AuthenticationMechanisms.Add("X-EXPS GSSAPI");
					}
					if (smtpInSession.SecureState == SecureState.AnonymousTls && !ehloOptions.AuthenticationMechanisms.Contains("X-EXPS EXCHANGEAUTH"))
					{
						ehloOptions.AuthenticationMechanisms.Add("X-EXPS EXCHANGEAUTH");
					}
				}
				if (!ehloOptions.AuthenticationMechanisms.Contains("X-EXPS NTLM"))
				{
					ehloOptions.AuthenticationMechanisms.Add("X-EXPS NTLM");
				}
			}
			if (smtpInSession.SecureState == SecureState.StartTls && !smtpInSession.DetermineTlsDomainCapabilities())
			{
				base.SmtpResponse = SmtpResponse.CertificateValidationFailure;
				smtpInSession.Disconnect(DisconnectReason.DroppedSession);
				return;
			}
			smtpInSession.AddSessionPermissions(smtpInSession.Capabilities);
			if (SmtpInSessionUtils.ShouldAcceptOorgProtocol(smtpInSession.Capabilities))
			{
				ehloOptions.XOorg = true;
				smtpInSession.AdvertisedEhloOptions.XOorg = true;
			}
			if (smtpInSession.SecureState == SecureState.AnonymousTls || SmtpInSessionUtils.ShouldAcceptProxyProtocol(this.transportConfiguration.ProcessTransportRole, smtpInSession.Capabilities))
			{
				ehloOptions.XProxy = true;
				smtpInSession.AdvertisedEhloOptions.XProxy = true;
			}
			if (SmtpInSessionUtils.ShouldAcceptProxyFromProtocol(this.transportConfiguration.ProcessTransportRole, smtpInSession.Capabilities) || ((this.transportConfiguration.ProcessTransportRole == ProcessTransportRole.Hub || this.transportConfiguration.ProcessTransportRole == ProcessTransportRole.FrontEnd) && smtpInSession.SecureState == SecureState.AnonymousTls))
			{
				ehloOptions.XProxyFrom = true;
				smtpInSession.AdvertisedEhloOptions.XProxyFrom = true;
			}
			if (SmtpInSessionUtils.ShouldAcceptProxyToProtocol(this.transportConfiguration.ProcessTransportRole, smtpInSession.Capabilities) || (this.transportConfiguration.ProcessTransportRole == ProcessTransportRole.FrontEnd && smtpInSession.SecureState == SecureState.AnonymousTls))
			{
				ehloOptions.XProxyTo = true;
				ehloOptions.XRsetProxyTo = true;
				smtpInSession.AdvertisedEhloOptions.XProxyTo = true;
				smtpInSession.AdvertisedEhloOptions.XRsetProxyTo = true;
			}
			if (smtpInSession.SecureState == SecureState.AnonymousTls && this.transportConfiguration.ProcessTransportRole == ProcessTransportRole.MailboxDelivery)
			{
				ehloOptions.XSessionMdbGuid = true;
				smtpInSession.AdvertisedEhloOptions.XSessionMdbGuid = true;
				ehloOptions.XSessionType = true;
				smtpInSession.AdvertisedEhloOptions.XSessionType = true;
			}
			if (SmtpInSessionUtils.ShouldAcceptXAttrProtocol(smtpInSession.Capabilities) || (smtpInSession.SecureState == SecureState.AnonymousTls && MultiTenantTransport.MultiTenancyEnabled))
			{
				ehloOptions.XAttr = true;
				smtpInSession.AdvertisedEhloOptions.XAttr = true;
			}
			if (smtpInSession.SecureState == SecureState.AnonymousTls || SmtpInSessionUtils.ShouldAcceptXSysProbeProtocol(smtpInSession.Capabilities))
			{
				ehloOptions.XSysProbe = true;
				smtpInSession.AdvertisedEhloOptions.XSysProbe = true;
			}
			if (smtpInSession.SecureState == SecureState.AnonymousTls && this.transportConfiguration.ProcessTransportRole == ProcessTransportRole.MailboxDelivery)
			{
				ehloOptions.XMsgId = true;
				smtpInSession.AdvertisedEhloOptions.XMsgId = true;
			}
			if (smtpInSession.SecureState == SecureState.AnonymousTls || SmtpInSessionUtils.ShouldAcceptXOrigFromProtocol(smtpInSession.Capabilities))
			{
				ehloOptions.XOrigFrom = true;
				smtpInSession.AdvertisedEhloOptions.XOrigFrom = true;
			}
			smtpInSession.AdvertisedEhloOptions.XAdrc = false;
			smtpInSession.AdvertisedEhloOptions.XExprops = false;
			smtpInSession.AdvertisedEhloOptions.XFastIndex = false;
			if (smtpInSession.SecureState == SecureState.AnonymousTls)
			{
				if ((this.transportConfiguration.ProcessTransportRole == ProcessTransportRole.MailboxDelivery || this.transportConfiguration.ProcessTransportRole == ProcessTransportRole.Hub) && smtpInSession.TransportAppConfig.MessageContextBlobConfiguration.AdvertiseExtendedProperties)
				{
					ehloOptions.XExprops = true;
					smtpInSession.AdvertisedEhloOptions.XExprops = true;
				}
				if (this.transportConfiguration.ProcessTransportRole != ProcessTransportRole.FrontEnd && this.transportConfiguration.ProcessTransportRole != ProcessTransportRole.Edge)
				{
					if (smtpInSession.TransportAppConfig.MessageContextBlobConfiguration.AdvertiseADRecipientCache)
					{
						ehloOptions.XAdrc = true;
						smtpInSession.AdvertisedEhloOptions.XAdrc = true;
					}
					if (smtpInSession.TransportAppConfig.MessageContextBlobConfiguration.AdvertiseFastIndex)
					{
						ehloOptions.XFastIndex = true;
						smtpInSession.AdvertisedEhloOptions.XFastIndex = true;
					}
				}
			}
			base.SmtpResponse = ehloOptions.CreateSmtpResponse(SmtpMessageContextBlob.AdrcSmtpMessageContextBlobInstance, SmtpMessageContextBlob.ExtendedPropertiesSmtpMessageContextBlobInstance, SmtpMessageContextBlob.FastIndexSmtpMessageContextBlobInstance);
		}

		internal override void OutboundCreateCommand()
		{
			this.HelloDomain = base.SmtpSession.HelloDomain;
		}

		internal override void OutboundFormatCommand()
		{
			base.ProtocolCommandString = "EHLO " + this.HelloDomain;
			ExTraceGlobals.SmtpSendTracer.TraceDebug<string>((long)this.GetHashCode(), "Formatted command : {0}", base.ProtocolCommandString);
		}

		internal override void OutboundProcessResponse()
		{
			SmtpOutSession smtpOutSession = (SmtpOutSession)base.SmtpSession;
			string statusCode = base.SmtpResponse.StatusCode;
			ExTraceGlobals.SmtpSendTracer.TraceDebug<string, SmtpResponse>((long)this.GetHashCode(), "Processing EHLO response. Status Code: {0} Response {1}", statusCode, base.SmtpResponse);
			if (statusCode[0] == '5')
			{
				ExTraceGlobals.SmtpSendTracer.TraceError<SmtpResponse>((long)this.GetHashCode(), "EHLO command failed with response {0}", base.SmtpResponse);
				smtpOutSession.NextState = SmtpOutSession.SessionState.Helo;
				return;
			}
			if (statusCode[0] == '4')
			{
				ExTraceGlobals.SmtpSendTracer.TraceError<SmtpResponse>((long)this.GetHashCode(), "EHLO failed with response {0}", base.SmtpResponse);
				ExTraceGlobals.SmtpSendTracer.TraceError((long)this.GetHashCode(), "Initiating failover");
				smtpOutSession.FailoverConnection(base.SmtpResponse);
				smtpOutSession.SetNextStateToQuit();
				return;
			}
			if (statusCode[0] != '2')
			{
				ExTraceGlobals.SmtpSendTracer.TraceError<SmtpResponse>((long)this.GetHashCode(), "EHLO command failed, the response was {0}", base.SmtpResponse);
				ExTraceGlobals.SmtpSendTracer.TraceError((long)this.GetHashCode(), "The session will be terminated");
				smtpOutSession.AckConnection(AckStatus.Retry, base.SmtpResponse);
				smtpOutSession.SetNextStateToQuit();
				return;
			}
			smtpOutSession.AdvertisedEhloOptions.ParseResponse(base.SmtpResponse, smtpOutSession.RemoteEndPoint.Address);
			if (smtpOutSession.TlsConfiguration.RequireTls && !smtpOutSession.AdvertisedEhloOptions.StartTLS)
			{
				ExTraceGlobals.SmtpSendTracer.TraceError((long)this.GetHashCode(), "Connector is configured to send mail only over TLS connections and remote doesn't support TLS");
				string nextHopDomain = smtpOutSession.NextHopDomain;
				SmtpCommand.EventLogger.LogEvent(TransportEventLogConstants.Tuple_SmtpSendTLSRequiredFailed, smtpOutSession.Connector.Name, new object[]
				{
					smtpOutSession.Connector.Name,
					nextHopDomain,
					smtpOutSession.AdvertisedEhloOptions.AdvertisedFQDN
				});
				smtpOutSession.LogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "Connector is configured to send mail only over TLS connections and remote doesn't support TLS");
				string notificationReason = string.Format("Send connector {0} couldn't connect to remote domain {1}. The send connector requires Transport Layer Security (TLS) authentication, but is unable to establish TLS with the receiving server for the remote domain. Check this connector's authentication setting and the EHLO response from the remote server {2}.", smtpOutSession.Connector.Name, nextHopDomain, smtpOutSession.AdvertisedEhloOptions.AdvertisedFQDN);
				EventNotificationItem.Publish(ExchangeComponent.Transport.Name, "SmtpSendTLSRequiredFailed", null, notificationReason, ResultSeverityLevel.Error, false);
				smtpOutSession.FailoverConnection(SmtpResponse.RequireTLSToSendMail);
				smtpOutSession.SetNextStateToQuit();
				return;
			}
			if (!smtpOutSession.CheckDomainSecure<string>(smtpOutSession.AdvertisedEhloOptions.StartTLS, "TLS was not offered", TransportEventLogConstants.Tuple_MessageToSecureDomainFailedBecauseTlsNotOffered, string.Empty))
			{
				EventNotificationItem.Publish(ExchangeComponent.Transport.Name, "MessageToSecureDomainFailedBecauseTlsNotOffered", null, "A message from a domain-secured domain failed to authenticate because the TLS certificate does not contain the domain name.", ResultSeverityLevel.Error, false);
				return;
			}
			if (!smtpOutSession.AdvertisedEhloOptions.Dsn)
			{
				ExTraceGlobals.SmtpSendTracer.TraceDebug((long)this.GetHashCode(), "Remote server does not support DSN. Relay DSNs will be generated for successful recipients as needed.");
				smtpOutSession.NextHopConnection.GenerateSuccessDSNs = DsnFlags.Relay;
			}
			EHLOSmtpCommand.OutboundAuthNextState outboundAuthNextState = this.EvaluateOutboundAuthRequirements();
			switch (outboundAuthNextState)
			{
			case EHLOSmtpCommand.OutboundAuthNextState.Auth:
			case EHLOSmtpCommand.OutboundAuthNextState.SessionEstablished:
				break;
			case EHLOSmtpCommand.OutboundAuthNextState.Failure:
				smtpOutSession.SetNextStateToQuit();
				return;
			case EHLOSmtpCommand.OutboundAuthNextState.OpportunisticTls:
				if (smtpOutSession.AdvertisedEhloOptions.StartTLS && smtpOutSession.SecureState == SecureState.None && !smtpOutSession.TlsConfiguration.ShouldSkipTls)
				{
					ExTraceGlobals.SmtpSendTracer.TraceDebug((long)this.GetHashCode(), "Will try to establish a TLS session opportunistically");
					smtpOutSession.NextState = SmtpOutSession.SessionState.StartTLS;
					return;
				}
				ExTraceGlobals.SmtpSendTracer.TraceDebug((long)this.GetHashCode(), "Skipping STARTTLS because TLS is not available");
				outboundAuthNextState = EHLOSmtpCommand.OutboundAuthNextState.SessionEstablished;
				break;
			case EHLOSmtpCommand.OutboundAuthNextState.Tls:
				return;
			default:
				throw new InvalidOperationException("Unexpected OutboundAuthNextState value: " + outboundAuthNextState);
			}
			if (!smtpOutSession.CheckRequireOorg())
			{
				return;
			}
			if (outboundAuthNextState == EHLOSmtpCommand.OutboundAuthNextState.SessionEstablished)
			{
				smtpOutSession.PrepareNextStateForEstablishedSession();
			}
		}

		private EHLOSmtpCommand.OutboundAuthNextState EvaluateOutboundAuthRequirements()
		{
			SmtpOutSession smtpOutSession = (SmtpOutSession)base.SmtpSession;
			if (smtpOutSession.Connector.DNSRoutingEnabled && !string.Equals(smtpOutSession.Connector.Name, Strings.IntraorgSendConnectorName) && !string.Equals(smtpOutSession.Connector.Name, Strings.InternalDestinationInboundProxySendConnector) && !string.Equals(smtpOutSession.Connector.Name, Strings.MailboxProxySendConnector) && !string.Equals(smtpOutSession.Connector.Name, Strings.InternalDestinationOutboundProxySendConnector))
			{
				ExTraceGlobals.SmtpSendTracer.TraceDebug((long)this.GetHashCode(), "EHLO command succeeded for DNS connector, proceeding with opportunistic TLS if advertised");
				return EHLOSmtpCommand.OutboundAuthNextState.OpportunisticTls;
			}
			SmtpSendConnectorConfig.AuthMechanisms authMechanism = smtpOutSession.AuthMechanism;
			switch (authMechanism)
			{
			case SmtpSendConnectorConfig.AuthMechanisms.None:
				ExTraceGlobals.SmtpSendTracer.TraceDebug((long)this.GetHashCode(), "EHLO command succeeded, no authentication mechanism is specified. Proceeding with opportunistic TLS if advertised.");
				return EHLOSmtpCommand.OutboundAuthNextState.OpportunisticTls;
			case (SmtpSendConnectorConfig.AuthMechanisms)1:
			case (SmtpSendConnectorConfig.AuthMechanisms)3:
				break;
			case SmtpSendConnectorConfig.AuthMechanisms.BasicAuth:
			case SmtpSendConnectorConfig.AuthMechanisms.BasicAuthRequireTLS:
				if (smtpOutSession.AuthMechanism == SmtpSendConnectorConfig.AuthMechanisms.BasicAuthRequireTLS && smtpOutSession.SecureState != SecureState.StartTls)
				{
					if (smtpOutSession.AdvertisedEhloOptions.StartTLS)
					{
						ExTraceGlobals.SmtpSendTracer.TraceDebug((long)this.GetHashCode(), "EHLO command succeeded, we require AUTH after TLS, will try to starttls");
						smtpOutSession.NextState = SmtpOutSession.SessionState.StartTLS;
						return EHLOSmtpCommand.OutboundAuthNextState.Tls;
					}
					ExTraceGlobals.SmtpSendTracer.TraceError((long)this.GetHashCode(), "We require AUTH after TLS, but remote doesn't advertise STARTTLS, will drop session");
					smtpOutSession.FailoverConnection(SmtpResponse.RequireSTARTTLSToBasicAuth);
					return EHLOSmtpCommand.OutboundAuthNextState.Failure;
				}
				else
				{
					if (smtpOutSession.AuthMechanism == SmtpSendConnectorConfig.AuthMechanisms.BasicAuth && !smtpOutSession.TlsConfiguration.ShouldSkipTls && smtpOutSession.AdvertisedEhloOptions.StartTLS && smtpOutSession.SecureState == SecureState.None)
					{
						ExTraceGlobals.SmtpSendTracer.TraceDebug((long)this.GetHashCode(), "EHLO command succeeded, we still take opportunistic TLS before AUTH LOGIN");
						smtpOutSession.NextState = SmtpOutSession.SessionState.StartTLS;
						return EHLOSmtpCommand.OutboundAuthNextState.Tls;
					}
					if (!smtpOutSession.AdvertisedEhloOptions.AuthenticationMechanisms.Contains("AUTH LOGIN"))
					{
						ExTraceGlobals.SmtpSendTracer.TraceError((long)this.GetHashCode(), "Unable to AUTH LOGIN because remote doesn't advertise AUTH LOGIN. Drop session");
						smtpOutSession.FailoverConnection(SmtpResponse.RequireBasicAuthentication);
						return EHLOSmtpCommand.OutboundAuthNextState.Failure;
					}
					if (!string.IsNullOrEmpty(smtpOutSession.AuthenticationUsername) && smtpOutSession.AuthenticationPassword != null)
					{
						ExTraceGlobals.SmtpSendTracer.TraceDebug((long)this.GetHashCode(), "EHLO command succeeded, will try to authenticate under AUTH LOGIN");
						smtpOutSession.NextState = SmtpOutSession.SessionState.Auth;
						return EHLOSmtpCommand.OutboundAuthNextState.Auth;
					}
					ExTraceGlobals.SmtpSendTracer.TraceError((long)this.GetHashCode(), "Unable to AUTH LOGIN because username or password is null. Drop session");
					smtpOutSession.FailoverConnection(SmtpResponse.AuthTempFailure);
					return EHLOSmtpCommand.OutboundAuthNextState.Failure;
				}
				break;
			default:
				if (authMechanism != SmtpSendConnectorConfig.AuthMechanisms.ExchangeServer)
				{
					if (authMechanism == SmtpSendConnectorConfig.AuthMechanisms.ExternalAuthoritative)
					{
						ExTraceGlobals.SmtpSendTracer.TraceDebug((long)this.GetHashCode(), "EHLO command succeeded, auth is external secured. Proceeding with opportunistic TLS if advertised.");
						return EHLOSmtpCommand.OutboundAuthNextState.OpportunisticTls;
					}
				}
				else if (smtpOutSession.AdvertisedEhloOptions.AnonymousTLS && smtpOutSession.SecureState == SecureState.None)
				{
					if (smtpOutSession.RequiresDirectTrust && !smtpOutSession.IsInternalTransportCertificateAvailable)
					{
						ExTraceGlobals.SmtpSendTracer.TraceError((long)this.GetHashCode(), "EHLO command succeeded, but we cannot load our Internal Transport Certificate");
						smtpOutSession.FailoverConnection(SmtpResponse.InternalTransportCertificateNotAvailable);
						return EHLOSmtpCommand.OutboundAuthNextState.Failure;
					}
					ExTraceGlobals.SmtpSendTracer.TraceDebug((long)this.GetHashCode(), "EHLO command succeeded, try to perform Exchange server authentication using x-anonymoustls");
					smtpOutSession.NextState = SmtpOutSession.SessionState.AnonymousTLS;
					return EHLOSmtpCommand.OutboundAuthNextState.Tls;
				}
				else
				{
					if (Components.IsBridgehead && smtpOutSession.AdvertisedEhloOptions.AuthenticationMechanisms.Contains("X-EXPS EXCHANGEAUTH"))
					{
						ExTraceGlobals.SmtpSendTracer.TraceDebug((long)this.GetHashCode(), "Will try to authenticate using x-exps exchangeauth.");
						smtpOutSession.NextState = SmtpOutSession.SessionState.Exps;
						return EHLOSmtpCommand.OutboundAuthNextState.Auth;
					}
					if (Components.IsBridgehead && smtpOutSession.AdvertisedEhloOptions.AuthenticationMechanisms.Contains("X-EXPS GSSAPI"))
					{
						if (smtpOutSession.NextHopDeliveryType == DeliveryType.SmtpRelayToTiRg)
						{
							ExTraceGlobals.SmtpSendTracer.TraceDebug((long)this.GetHashCode(), "EHLO command succeeded, will try to authenticate using x-exps gssapi to Ti server.");
							smtpOutSession.NextState = SmtpOutSession.SessionState.Exps;
							return EHLOSmtpCommand.OutboundAuthNextState.Auth;
						}
						if (smtpOutSession.CanDowngradeExchangeServerAuth)
						{
							ExTraceGlobals.SmtpSendTracer.TraceDebug<string>((long)this.GetHashCode(), "EHLO command succeeded, will try to authenticate using x-exps gssapi to E14+ server for delivery type {0}.", smtpOutSession.NextHopDeliveryType.ToString());
							smtpOutSession.NextState = SmtpOutSession.SessionState.Exps;
							return EHLOSmtpCommand.OutboundAuthNextState.Auth;
						}
					}
					if (smtpOutSession.SecureState == SecureState.None)
					{
						ExTraceGlobals.SmtpSendTracer.TraceError((long)this.GetHashCode(), "Connector requires ExchangeServer authentication but we can't achieve it.");
						smtpOutSession.FailoverConnection(SmtpResponse.CannotExchangeAuthenticate);
						return EHLOSmtpCommand.OutboundAuthNextState.Failure;
					}
					return EHLOSmtpCommand.OutboundAuthNextState.SessionEstablished;
				}
				break;
			}
			ExTraceGlobals.SmtpSendTracer.TraceDebug<SmtpSendConnectorConfig.AuthMechanisms>((long)this.GetHashCode(), "EHLO command succeeded, but the session's auth mechanism <{0}> is not recognised. Proceeding with opportunistic TLS if advertised.", smtpOutSession.AuthMechanism);
			return EHLOSmtpCommand.OutboundAuthNextState.OpportunisticTls;
		}

		protected ITransportConfiguration transportConfiguration;

		private readonly EhloCommandEventArgs ehloCommandEventArgs;

		public enum OutboundAuthNextState
		{
			Auth,
			Failure,
			OpportunisticTls,
			SessionEstablished,
			Tls
		}
	}
}
