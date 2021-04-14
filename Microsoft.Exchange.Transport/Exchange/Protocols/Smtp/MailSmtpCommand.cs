using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Net.ExSmtpClient;
using Microsoft.Exchange.SecureMail;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Transport;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class MailSmtpCommand : SmtpCommand
	{
		public MailSmtpCommand(ISmtpSession session, ITransportAppConfig transportAppConfig) : base(session, "MAIL", "OnMailCommand", LatencyComponent.None)
		{
			base.IsResponseBuffered = true;
			this.mailCommandEventArgs = new MailCommandEventArgs();
			this.CommandEventArgs = this.mailCommandEventArgs;
			this.transportAppConfig = transportAppConfig;
		}

		internal RoutingAddress FromAddress
		{
			get
			{
				return this.mailCommandEventArgs.FromAddress;
			}
			set
			{
				this.mailCommandEventArgs.FromAddress = value;
			}
		}

		internal long Size
		{
			get
			{
				return this.mailCommandEventArgs.Size;
			}
			set
			{
				this.mailCommandEventArgs.Size = value;
			}
		}

		internal Microsoft.Exchange.Transport.BodyType BodyType
		{
			get
			{
				return EnumConverter.PublicToInternal(this.mailCommandEventArgs.BodyType);
			}
			set
			{
				this.mailCommandEventArgs.BodyType = EnumConverter.InternalToPublic(value);
			}
		}

		internal DsnFormat Ret
		{
			get
			{
				return EnumConverter.PublicToInternal(this.mailCommandEventArgs.DsnFormatRequested);
			}
			set
			{
				this.mailCommandEventArgs.DsnFormatRequested = EnumConverter.InternalToPublic(value);
			}
		}

		internal string EnvId
		{
			get
			{
				return this.mailCommandEventArgs.EnvelopeId;
			}
			set
			{
				this.mailCommandEventArgs.EnvelopeId = value;
			}
		}

		internal string Auth
		{
			get
			{
				return this.mailCommandEventArgs.Auth;
			}
			set
			{
				this.mailCommandEventArgs.Auth = value;
			}
		}

		internal string XShadow
		{
			get
			{
				return this.xshadow;
			}
			set
			{
				this.xshadow = value;
			}
		}

		internal string Oorg
		{
			get
			{
				return this.mailCommandEventArgs.Oorg;
			}
			set
			{
				this.mailCommandEventArgs.Oorg = value;
			}
		}

		internal Guid SystemProbeId
		{
			get
			{
				return this.mailCommandEventArgs.SystemProbeId;
			}
			set
			{
				this.mailCommandEventArgs.SystemProbeId = value;
			}
		}

		internal RoutingAddress OriginalFromAddress
		{
			get
			{
				return this.mailCommandEventArgs.OriginalFromAddress;
			}
			set
			{
				this.mailCommandEventArgs.OriginalFromAddress = value;
			}
		}

		internal bool SmtpUtf8
		{
			get
			{
				return this.mailCommandEventArgs.SmtpUtf8;
			}
			set
			{
				this.mailCommandEventArgs.SmtpUtf8 = value;
			}
		}

		internal Dictionary<string, string> ConsumerMailOptionalArguments
		{
			get
			{
				return this.mailCommandEventArgs.ConsumerMailOptionalArguments;
			}
			set
			{
				this.mailCommandEventArgs.ConsumerMailOptionalArguments = value;
			}
		}

		internal static void FormatCommand(StringBuilder mailFromLine, RoutingAddress fromAddress, IEhloOptions ehloOptions, TransportAppConfig.ISmtpMailCommandConfig mailCommandConfig, bool usingHelo, long size, string auth, string envId, DsnFormat ret, Microsoft.Exchange.Transport.BodyType bodyType, MailDirectionality directionality, Guid externalOrganizationId, OrganizationId internalOrganizationId, string exoAccountForest, string exoTenantContainer)
		{
			MailSmtpCommand.FormatCommand(mailFromLine, null, fromAddress, ehloOptions, mailCommandConfig, usingHelo, size, auth, null, envId, ret, bodyType, null, null, directionality, externalOrganizationId, internalOrganizationId, exoAccountForest, exoTenantContainer, Guid.Empty, string.Empty, RoutingAddress.Empty, false);
		}

		internal override void InboundParseCommand()
		{
			ISmtpInSession smtpInSession = (ISmtpInSession)base.SmtpSession;
			smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.MailInboundParseCommand);
			CommandContext commandContext = CommandContext.FromSmtpCommand(this);
			SmtpInSessionState smtpInSessionState = SmtpInSessionState.FromSmtpInSession(smtpInSession);
			TimeSpan tarpitInterval = base.TarpitInterval;
			bool flag = false;
			RoutingAddress routingAddress;
			MailParseOutput mailParseOutput;
			ParseResult parseResult = MailSmtpCommandParser.Parse(commandContext, smtpInSessionState, smtpInSession.SmtpInServer.CertificateValidator, smtpInSession.IsDataRedactionNecessary, new MailSmtpCommandParser.ValidateSequence(this.IsValidSequence), new MailSmtpCommandParser.UpdateState(this.ResetState), new MailSmtpCommandParser.IsPoisonMessage(PoisonMessage.IsMessagePoison), smtpInSessionState.EventLog, new MailSmtpCommandParser.PublishNotification(EventNotificationItem.Publish), ref flag, ref tarpitInterval, out routingAddress, out mailParseOutput);
			base.SmtpResponse = parseResult.SmtpResponse;
			base.ParsingStatus = parseResult.ParsingStatus;
			if (flag)
			{
				smtpInSession.Disconnect(DisconnectReason.Local);
			}
			if (routingAddress != RoutingAddress.Empty && routingAddress.IsValid)
			{
				this.FromAddress = routingAddress;
			}
			if (parseResult.IsFailed)
			{
				base.TarpitInterval = tarpitInterval;
				return;
			}
			base.CurrentOffset = commandContext.Offset;
			this.Size = mailParseOutput.Size;
			this.BodyType = mailParseOutput.MailBodyType;
			this.Ret = mailParseOutput.DsnFormat;
			this.EnvId = mailParseOutput.EnvelopeId;
			this.Auth = mailParseOutput.Auth;
			this.XShadow = mailParseOutput.XShadow;
			this.Oorg = mailParseOutput.Oorg;
			this.messageContext = mailParseOutput.MessageContextParameters;
			this.directionality = mailParseOutput.Directionality;
			this.SystemProbeId = mailParseOutput.SystemProbeId;
			this.internetMessageId = mailParseOutput.InternetMessageId;
			this.shadowMessageId = mailParseOutput.ShadowMessageId;
			this.OriginalFromAddress = mailParseOutput.OriginalFromAddress;
			this.SmtpUtf8 = mailParseOutput.SmtpUtf8;
			this.ConsumerMailOptionalArguments = mailParseOutput.ConsumerMailOptionalArguments;
			if (mailParseOutput.XAttrOrgId != null)
			{
				this.externalOrganizationId = mailParseOutput.XAttrOrgId.ExternalOrgId;
				this.internalOrganizationId = mailParseOutput.XAttrOrgId.InternalOrgId;
				this.exoAccountForest = mailParseOutput.XAttrOrgId.ExoAccountForest;
				this.exoTenantContainer = mailParseOutput.XAttrOrgId.ExoTenantContainer;
			}
			smtpInSession.RemoteIdentity = smtpInSessionState.RemoteIdentity;
			smtpInSession.RemoteIdentityName = smtpInSessionState.RemoteIdentityName;
			smtpInSession.SessionPermissions = smtpInSessionState.SessionPermissions;
		}

		internal override void InboundProcessCommand()
		{
			if (base.ParsingStatus != ParsingStatus.Complete)
			{
				return;
			}
			ISmtpInSession smtpInSession = (ISmtpInSession)base.SmtpSession;
			smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.MailInboundProcessCommand);
			if (Components.SmtpInComponent.TargetRunningState == ServiceState.Inactive && this.SystemProbeId == Guid.Empty)
			{
				base.SmtpResponse = SmtpResponse.ServiceInactive;
				smtpInSession.LogInformation(ProtocolLoggingLevel.Verbose, "Rejecting the non-probe message and disconnecting as transport service is Inactive.", null);
				smtpInSession.Disconnect(DisconnectReason.Local);
				return;
			}
			string arg;
			if (this.externalOrganizationId != Guid.Empty && smtpInSession.QueueQuotaComponent != null && !smtpInSession.IsPeerShadowSession && smtpInSession.QueueQuotaComponent.IsOrganizationOverQuota(this.exoAccountForest, this.externalOrganizationId, this.GetThrottledAddress(), out arg))
			{
				base.SmtpResponse = SmtpResponse.OrgQueueQuotaExceeded;
				smtpInSession.LogInformation(ProtocolLoggingLevel.Verbose, string.Format("Rejecting the message QQ limit was exceeded. ({0})", arg), null);
				return;
			}
			smtpInSession.SmtpUtf8Supported = this.SmtpUtf8;
			SmtpResponse smtpResponse;
			if (smtpInSession.CreateTransportMailItem(this.internalOrganizationId, this.externalOrganizationId, this.directionality, this.exoAccountForest, this.exoTenantContainer, out smtpResponse))
			{
				smtpInSession.LogInformation(ProtocolLoggingLevel.Verbose, "receiving message", Encoding.ASCII.GetBytes(smtpInSession.CurrentMessageTemporaryId));
				if (!RoutingAddress.IsEmpty(this.OriginalFromAddress))
				{
					smtpInSession.TransportMailItem.From = this.OriginalFromAddress;
				}
				smtpInSession.TransportMailItem.DateReceived = DateTime.UtcNow;
				smtpInSession.TransportMailItem.From = this.FromAddress;
				smtpInSession.TransportMailItem.Auth = this.Auth;
				smtpInSession.TransportMailItem.EnvId = this.EnvId;
				smtpInSession.TransportMailItem.DsnFormat = this.Ret;
				smtpInSession.TransportMailItem.BodyType = this.BodyType;
				smtpInSession.TransportMailItem.Oorg = this.Oorg;
				smtpInSession.TransportMailItem.InternetMessageId = this.internetMessageId;
				LatencyTracker.BeginTrackLatency(LatencyComponent.SmtpReceive, smtpInSession.TransportMailItem.LatencyTracker);
				if (!ConfigurationComponent.IsFrontEndTransportProcess(Components.Configuration))
				{
					LatencyComponent component = (smtpInSession.SessionSource.IsInboundProxiedSession || smtpInSession.SessionSource.IsClientProxiedSession) ? LatencyComponent.SmtpReceiveDataExternal : LatencyComponent.SmtpReceiveDataInternal;
					LatencyTracker.BeginTrackLatency(component, smtpInSession.TransportMailItem.LatencyTracker);
				}
				smtpInSession.SetupPoisonContext();
				smtpInSession.SetupExpectedBlobs(this.messageContext);
				smtpInSession.MailCommandMessageContextInformation = this.messageContext;
				smtpInSession.TransportMailItem.SystemProbeId = this.SystemProbeId;
				if (!string.IsNullOrEmpty(this.XShadow))
				{
					smtpInSession.TransportMailItem.ShadowServerDiscardId = this.XShadow;
					if (!smtpInSession.IsPeerShadowSession)
					{
						smtpInSession.TransportMailItem.ShadowServerContext = smtpInSession.SenderShadowContext;
					}
					if (this.shadowMessageId != Guid.Empty)
					{
						smtpInSession.TransportMailItem.ShadowMessageId = this.shadowMessageId;
					}
				}
				if (SmtpInSessionUtils.IsPartner(smtpInSession.RemoteIdentity))
				{
					smtpInSession.TransportMailItem.AuthMethod = MultilevelAuthMechanism.MutualTLS;
				}
				else if (SmtpInSessionUtils.IsExternalAuthoritative(smtpInSession.RemoteIdentity))
				{
					smtpInSession.TransportMailItem.AuthMethod = MultilevelAuthMechanism.SecureExternalSubmit;
				}
				else
				{
					smtpInSession.TransportMailItem.AuthMethod = smtpInSession.AuthMethod;
				}
				smtpInSession.TransportMailItem.HeloDomain = smtpInSession.HelloSmtpDomain;
				foreach (KeyValuePair<string, object> keyValuePair in this.mailCommandEventArgs.MailItemProperties)
				{
					smtpInSession.TransportMailItem.ExtendedProperties.SetValue<object>(keyValuePair.Key, keyValuePair.Value);
				}
				smtpInSession.TransportMailItem.ExtendedProperties.SetValue<ulong>("Microsoft.Exchange.Transport.SmtpInSessionId", smtpInSession.SessionId);
				if (smtpInSession.XProxyFromSeqNum > 0U)
				{
					smtpInSession.TransportMailItem.ExtendedProperties.SetValue<uint>("Microsoft.Exchange.Transport.TransportMailItem.InboundProxySequenceNumber", smtpInSession.XProxyFromSeqNum);
				}
				smtpInSession.TransportMailItemWrapper = new TransportMailItemWrapper(smtpInSession.TransportMailItem, smtpInSession.MexSession, true);
				base.SmtpResponse = SmtpResponse.MailFromOk;
			}
			else
			{
				base.SmtpResponse = smtpResponse;
			}
			TimeSpan throttleDelay = smtpInSession.SmtpInServer.ThrottleDelay;
			if (throttleDelay > TimeSpan.Zero)
			{
				base.LowAuthenticationLevelTarpitOverride = TarpitAction.DoTarpit;
				base.HighAuthenticationLevelTarpitOverride = TarpitAction.DoTarpit;
				base.TarpitInterval = throttleDelay;
				base.TarpitReason = "Back Pressure";
				base.TarpitContext = smtpInSession.SmtpInServer.ThrottleDelayContext;
			}
		}

		internal override void OutboundCreateCommand()
		{
			SmtpOutSession smtpOutSession = (SmtpOutSession)base.SmtpSession;
			this.FromAddress = smtpOutSession.RoutedMailItem.From;
			this.Size = smtpOutSession.RoutedMailItem.MimeSize;
			if (smtpOutSession.RoutedMailItem.BodyType != Microsoft.Exchange.Transport.BodyType.Default)
			{
				this.BodyType = smtpOutSession.RoutedMailItem.BodyType;
				ExTraceGlobals.SmtpSendTracer.TraceDebug<Microsoft.Exchange.Transport.BodyType>((long)this.GetHashCode(), "BodyType: {0}", this.BodyType);
			}
			if (smtpOutSession.AdvertisedEhloOptions.AreAnyAuthMechanismsSupported() && !string.IsNullOrEmpty(smtpOutSession.RoutedMailItem.Auth))
			{
				this.Auth = smtpOutSession.RoutedMailItem.Auth;
			}
			if (smtpOutSession.ShadowCurrentMailItem)
			{
				if (smtpOutSession.RoutedMailItem.ShadowMessageId == Guid.Empty)
				{
					throw new InvalidOperationException(string.Format("MailItem {0} doesn't have a valid ShadowMessageId", smtpOutSession.RoutedMailItem.RecordId));
				}
				this.XShadow = smtpOutSession.RoutedMailItem.ShadowMessageId.ToString();
			}
			if (smtpOutSession.AdvertisedEhloOptions.Dsn)
			{
				if (!string.IsNullOrEmpty(smtpOutSession.RoutedMailItem.EnvId))
				{
					this.EnvId = smtpOutSession.RoutedMailItem.EnvId;
				}
				if (smtpOutSession.RoutedMailItem.DsnFormat == DsnFormat.Full)
				{
					this.Ret = DsnFormat.Full;
				}
				else if (smtpOutSession.RoutedMailItem.DsnFormat == DsnFormat.Headers)
				{
					this.Ret = DsnFormat.Headers;
				}
				else
				{
					this.Ret = DsnFormat.Default;
				}
			}
			if (smtpOutSession.AdvertisedEhloOptions.XOorg)
			{
				this.Oorg = smtpOutSession.RoutedMailItem.Oorg;
			}
			if (smtpOutSession.AdvertisedEhloOptions.XAttr)
			{
				this.directionality = smtpOutSession.RoutedMailItem.Directionality;
				this.externalOrganizationId = smtpOutSession.RoutedMailItem.ExternalOrganizationId;
				this.exoAccountForest = smtpOutSession.RoutedMailItem.ExoAccountForest;
				this.exoTenantContainer = smtpOutSession.RoutedMailItem.ExoTenantContainer;
				if ((smtpOutSession.Permissions & Permission.SendForestHeaders) != Permission.None)
				{
					this.internalOrganizationId = smtpOutSession.RoutedMailItem.OrganizationId;
				}
			}
			if (smtpOutSession.AdvertisedEhloOptions.XSysProbe)
			{
				this.SystemProbeId = smtpOutSession.RoutedMailItem.SystemProbeId;
			}
			if (smtpOutSession.AdvertisedEhloOptions.XOrigFrom && !this.FromAddress.Equals(smtpOutSession.RoutedMailItem.OriginalFrom))
			{
				this.OriginalFromAddress = smtpOutSession.RoutedMailItem.OriginalFrom;
			}
			if (smtpOutSession.AdvertisedEhloOptions.SmtpUtf8)
			{
				this.SmtpUtf8 = this.ShouldSendOutboundSmtpUtf8(this.FromAddress, smtpOutSession.RoutedMailItem);
			}
			smtpOutSession.SetupBlobsToSend();
		}

		internal override void OutboundFormatCommand()
		{
			SmtpOutSession smtpOutSession = (SmtpOutSession)base.SmtpSession;
			if (smtpOutSession.RoutedMailItem != null)
			{
				ExTraceGlobals.FaultInjectionTracer.TraceTest<string>(3754306877U, smtpOutSession.RoutedMailItem.Subject);
				ExTraceGlobals.FaultInjectionTracer.TraceTest<string>(2949000509U, smtpOutSession.RoutedMailItem.Subject);
			}
			if (smtpOutSession.TlsConfiguration.RequireTls && smtpOutSession.SecureState == SecureState.None)
			{
				ExTraceGlobals.SmtpSendTracer.TraceError((long)this.GetHashCode(), "Connector is configured to send mail only over TLS connections and we didn't achieve it");
				string nextHopDomain = smtpOutSession.NextHopDomain;
				SmtpCommand.EventLogger.LogEvent(TransportEventLogConstants.Tuple_SmtpSendTLSRequiredFailed, smtpOutSession.Connector.Name, new object[]
				{
					smtpOutSession.Connector.Name,
					nextHopDomain,
					smtpOutSession.AdvertisedEhloOptions.AdvertisedFQDN
				});
				string notificationReason = string.Format("Send connector {0} couldn't connect to remote domain {1}. The send connector requires Transport Layer Security (TLS) authentication, but is unable to establish TLS with the receiving server for the remote domain. Check this connector's authentication setting and the EHLO response from the remote server {2}.", smtpOutSession.Connector.Name, nextHopDomain, smtpOutSession.AdvertisedEhloOptions.AdvertisedFQDN);
				EventNotificationItem.Publish(ExchangeComponent.Transport.Name, "SmtpSendTLSRequiredFailed", null, notificationReason, ResultSeverityLevel.Error, false);
				smtpOutSession.LogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "Drop connection because TLS configuration requires TLS before MailFrom command and could not achieve it, check connection authentication setting or remote capabilities");
				smtpOutSession.FailoverConnection(SmtpResponse.RequireTLSToSendMail);
				smtpOutSession.SetNextStateToQuit();
				return;
			}
			if (smtpOutSession.RoutedMailItem == null)
			{
				throw new InvalidOperationException("Must have a message object obtained in PrepareForNextMessage");
			}
			RoutingAddress shortAddress = smtpOutSession.GetShortAddress(this.FromAddress);
			smtpOutSession.LogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "sending message with RecordId {0} and InternetMessageId {1}", new object[]
			{
				smtpOutSession.RoutedMailItem.RecordId.ToString(NumberFormatInfo.InvariantInfo),
				smtpOutSession.RoutedMailItem.InternetMessageId
			});
			StringBuilder stringBuilder = new StringBuilder("MAIL FROM:");
			StringBuilder stringBuilder2 = null;
			if (Util.IsDataRedactionNecessary())
			{
				stringBuilder2 = new StringBuilder("MAIL FROM:");
			}
			MailSmtpCommand.FormatCommand(stringBuilder, stringBuilder2, shortAddress, smtpOutSession.AdvertisedEhloOptions, this.transportAppConfig.SmtpMailCommandConfiguration, smtpOutSession.UsingHELO, this.Size, this.Auth, this.XShadow, this.EnvId, this.Ret, this.BodyType, this.Oorg, smtpOutSession.BlobsToSend, this.directionality, this.externalOrganizationId, this.internalOrganizationId, this.exoAccountForest, this.exoTenantContainer, this.SystemProbeId, smtpOutSession.RoutedMailItem.InternetMessageId, this.OriginalFromAddress, this.SmtpUtf8);
			base.ProtocolCommandString = stringBuilder.ToString();
			base.RedactedProtocolCommandString = ((stringBuilder2 != null) ? stringBuilder2.ToString() : null);
			ExTraceGlobals.SmtpSendTracer.TraceDebug<string>((long)this.GetHashCode(), "Formatted mail command: {0}", base.ProtocolCommandString);
			this.outboundStopwatch = new Stopwatch();
			this.outboundStopwatch.Start();
		}

		internal override void OutboundProcessResponse()
		{
			SmtpOutSession smtpOutSession = (SmtpOutSession)base.SmtpSession;
			string statusCode = base.SmtpResponse.StatusCode;
			this.outboundStopwatch.Stop();
			if (smtpOutSession.NextHopConnection == null)
			{
				ExTraceGlobals.SmtpSendTracer.TraceError<SmtpResponse>((long)this.GetHashCode(), "Connection already marked for Failover.  Not processing response for MAIL command: {0}", base.SmtpResponse);
				return;
			}
			if (smtpOutSession.PipeLineFailOverPending)
			{
				ExTraceGlobals.SmtpSendTracer.TraceError<SmtpResponse>((long)this.GetHashCode(), "Fail over initiated by prior responses to pipelined commands. Not processing response for MAIL command : {0}", base.SmtpResponse);
				smtpOutSession.FailoverConnection(base.SmtpResponse, false);
				return;
			}
			if (smtpOutSession.RecipientsAckedPending)
			{
				smtpOutSession.RecipientsAckedPending = false;
			}
			if (smtpOutSession.NextHopDeliveryType == DeliveryType.SmtpDeliveryToMailbox && base.SmtpResponse.Equals(SmtpResponse.TooManyRelatedErrors))
			{
				ExTraceGlobals.SmtpSendTracer.TraceError<string, SmtpResponse>((long)this.GetHashCode(), "MAIL FROM {0} failed with: {1}", smtpOutSession.RoutedMailItem.From.ToString(), base.SmtpResponse);
				((RoutedMailItem)smtpOutSession.RoutedMailItem).Poison();
				smtpOutSession.AckMessage(AckStatus.Fail, base.SmtpResponse);
				smtpOutSession.PrepareForNextMessage(true);
				return;
			}
			if ((base.SmtpResponse.SmtpResponseType == SmtpResponseType.PermanentError && !Util.DowngradeCustomPermanentFailure(smtpOutSession.Connector.ErrorPolicies, base.SmtpResponse, this.transportAppConfig)) || (base.SmtpResponse.SmtpResponseType == SmtpResponseType.TransientError && Util.UpgradeCustomPermanentFailure(smtpOutSession.Connector.ErrorPolicies, base.SmtpResponse, this.transportAppConfig)))
			{
				ExTraceGlobals.SmtpSendTracer.TraceError<string, SmtpResponse>((long)this.GetHashCode(), "MAIL FROM {0} failed with: {1}", smtpOutSession.RoutedMailItem.From.ToString(), base.SmtpResponse);
				smtpOutSession.AckMessage(AckStatus.Fail, base.SmtpResponse);
				smtpOutSession.PrepareForNextMessage(true);
				return;
			}
			if (statusCode[0] != '2')
			{
				ExTraceGlobals.SmtpSendTracer.TraceError<string, SmtpResponse>((long)this.GetHashCode(), "MAIL FROM {0} failed with: {1}", smtpOutSession.RoutedMailItem.From.ToString(), base.SmtpResponse);
				smtpOutSession.AckMessage(AckStatus.Retry, base.SmtpResponse);
				smtpOutSession.PrepareForNextMessage(true);
				return;
			}
			ExTraceGlobals.SmtpSendTracer.TraceDebug<string>((long)this.GetHashCode(), "MAIL command for {0} succeeded, will issue RCPT", smtpOutSession.RoutedMailItem.From.ToString());
			smtpOutSession.NextState = SmtpOutSession.SessionState.PerRecipient;
			smtpOutSession.UpdateServerLatency(this.outboundStopwatch.Elapsed);
			this.outboundStopwatch = null;
		}

		private static void FormatCommand(StringBuilder mailFromLine, StringBuilder redactedMailFromLine, RoutingAddress fromAddress, IEhloOptions ehloOptions, TransportAppConfig.ISmtpMailCommandConfig mailCommandConfig, bool usingHelo, long size, string auth, string xshadow, string envId, DsnFormat ret, Microsoft.Exchange.Transport.BodyType bodyType, string oorg, IEnumerable<SmtpMessageContextBlob> messageContextBlobs, MailDirectionality directionality, Guid externalOrganizationId, OrganizationId internalOrganizationId, string exoAccountForest, string exoTenantContainer, Guid systemProbeId, string internetMessageId, RoutingAddress originalFromAddress, bool smtpUtf8)
		{
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = ehloOptions != null && ehloOptions.SmtpUtf8 && smtpUtf8;
			if (ehloOptions != null && ehloOptions.Size == SizeMode.Enabled)
			{
				stringBuilder.Append(" SIZE=" + size);
			}
			if ((ehloOptions == null || ehloOptions.AreAnyAuthMechanismsSupported()) && !string.IsNullOrEmpty(auth))
			{
				stringBuilder.Append(" AUTH=" + SmtpUtils.ToXtextString(auth, false));
			}
			if (!string.IsNullOrEmpty(xshadow))
			{
				stringBuilder.Append(" XSHADOW=" + SmtpUtils.ToXtextString(xshadow, false));
			}
			if (ehloOptions == null || ehloOptions.Dsn)
			{
				if (!string.IsNullOrEmpty(envId))
				{
					stringBuilder.Append(" ENVID=" + SmtpUtils.ToXtextString(envId, false));
				}
				if (ret == DsnFormat.Full)
				{
					stringBuilder.Append(" RET=FULL");
				}
				else if (ret == DsnFormat.Headers)
				{
					stringBuilder.Append(" RET=HDRS");
				}
			}
			switch (bodyType)
			{
			case Microsoft.Exchange.Transport.BodyType.SevenBit:
				if (ehloOptions == null || (!usingHelo && (ehloOptions.EightBitMime || ehloOptions.BinaryMime)))
				{
					stringBuilder.Append(" BODY=7BIT");
					ExTraceGlobals.GeneralTracer.TraceDebug(0L, "Body Type: 7 BIT MIME");
				}
				else
				{
					ExTraceGlobals.GeneralTracer.TraceDebug(0L, "Using HELO. Body Type Not set");
				}
				break;
			case Microsoft.Exchange.Transport.BodyType.EightBitMIME:
				if (ehloOptions != null && !ehloOptions.EightBitMime)
				{
					ExTraceGlobals.GeneralTracer.TraceDebug(0L, "Message body is 8 bit MIME, but 8 Bit MIME not advertised by remote host. Use 7 bit");
					if (!usingHelo && ehloOptions.BinaryMime)
					{
						stringBuilder.Append(" BODY=7BIT");
					}
				}
				else
				{
					stringBuilder.Append(" BODY=8BITMIME");
					ExTraceGlobals.GeneralTracer.TraceDebug(0L, "Body Type: 8 BIT MIME");
				}
				break;
			case Microsoft.Exchange.Transport.BodyType.BinaryMIME:
				if (ehloOptions != null && !ehloOptions.BinaryMime)
				{
					ExTraceGlobals.GeneralTracer.TraceDebug(0L, "Message body is Binary MIME, but Binary MIME not advertised by remote host. Use 7 bit");
					if (!usingHelo && ehloOptions.EightBitMime)
					{
						stringBuilder.Append(" BODY=7BIT");
					}
				}
				else if (ehloOptions != null && !ehloOptions.Chunking)
				{
					ExTraceGlobals.GeneralTracer.TraceError(0L, "Binary MIME is advertised by remote host, but CHUNKING is not. Cannot send Binary MIME with DATA. Downconverting message to 7 bit");
					if (!usingHelo)
					{
						stringBuilder.Append(" BODY=7BIT");
					}
				}
				else
				{
					stringBuilder.Append(" BODY=BINARYMIME");
					ExTraceGlobals.GeneralTracer.TraceDebug(0L, "Body Type: BINARY MIME");
				}
				break;
			default:
				ExTraceGlobals.GeneralTracer.TraceDebug(0L, "Body Type: Not set");
				break;
			}
			if (ehloOptions != null && ehloOptions.XOorg && !string.IsNullOrEmpty(oorg))
			{
				stringBuilder.Append(" XOORG=" + SmtpUtils.ToXtextString(oorg, false));
			}
			if (ehloOptions != null && (ehloOptions.XAdrc || ehloOptions.XExprops || ehloOptions.XFastIndex) && messageContextBlobs != null && ((ICollection)messageContextBlobs).Count != 0)
			{
				stringBuilder.Append(" XMESSAGECONTEXT=" + MailSmtpCommand.GetMessageContext(messageContextBlobs, ehloOptions));
			}
			if (ehloOptions != null && ehloOptions.XAttr && directionality != MailDirectionality.Undefined && externalOrganizationId != Guid.Empty)
			{
				stringBuilder.Append(" XATTRDIRECT=" + SmtpUtils.ToXtextString(MailSmtpCommandParser.XAttrHelper.GetDirectionalityString(directionality), false));
				stringBuilder.Append(" XATTRORGID=" + SmtpUtils.ToXtextString(MailSmtpCommandParser.XAttrHelper.GetOrgIdString(mailCommandConfig, externalOrganizationId, internalOrganizationId, exoAccountForest, exoTenantContainer), false));
			}
			if (ehloOptions != null && ehloOptions.XSysProbe && systemProbeId != Guid.Empty)
			{
				stringBuilder.Append(" XSYSPROBEID=" + SmtpUtils.ToXtextString(systemProbeId.ToString(), false));
			}
			if (ehloOptions != null && ehloOptions.XMsgId && !string.IsNullOrEmpty(internetMessageId) && internetMessageId.Length <= 500)
			{
				stringBuilder.Append(" XMSGID=" + SmtpUtils.ToXtextString(internetMessageId, false));
			}
			if ((ehloOptions != null & ehloOptions.XOrigFrom) && !RoutingAddress.IsEmpty(originalFromAddress))
			{
				stringBuilder.Append(" XORIGFROM=" + SmtpUtils.ToXtextString(originalFromAddress.ToString(), flag));
			}
			if (flag)
			{
				stringBuilder.Append(" SMTPUTF8");
			}
			string value = stringBuilder.ToString();
			string emailAddress = fromAddress.ToString();
			mailFromLine.Append(SmtpCommand.GetBracketedString(emailAddress));
			mailFromLine.Append(value);
			if (redactedMailFromLine != null)
			{
				redactedMailFromLine.Append(SmtpCommand.GetBracketedString(Util.Redact(fromAddress)));
				redactedMailFromLine.Append(value);
			}
		}

		private static string GetMessageContext(IEnumerable<SmtpMessageContextBlob> messageContext, IEhloOptions ehloOptions)
		{
			StringBuilder stringBuilder = null;
			foreach (SmtpMessageContextBlob smtpMessageContextBlob in messageContext)
			{
				if (stringBuilder == null)
				{
					stringBuilder = new StringBuilder();
				}
				else
				{
					stringBuilder.Append(",");
				}
				stringBuilder.Append(smtpMessageContextBlob.GetVersionToSend(ehloOptions));
			}
			if (stringBuilder == null)
			{
				throw new InvalidOperationException("Expecting atleast one blob to be present, but did not find any.");
			}
			return stringBuilder.ToString();
		}

		private void ResetState(SmtpInSessionState sessionState)
		{
			ISmtpInSession smtpInSession = (ISmtpInSession)base.SmtpSession;
			smtpInSession.ResetMailItemPermissions();
			smtpInSession.ResetExpectedBlobs();
			smtpInSession.SeenRcpt2 = false;
		}

		private ParseResult IsValidSequence()
		{
			ISmtpInSession smtpInSession = (ISmtpInSession)base.SmtpSession;
			if (!base.VerifyHelloReceived() || !base.VerifyNoOngoingBdat())
			{
				smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.WrongSequence);
				return new ParseResult(base.ParsingStatus, base.SmtpResponse, false);
			}
			return ParseResult.ParsingComplete;
		}

		private string GetThrottledAddress()
		{
			if (this.OriginalFromAddress != RoutingAddress.Empty)
			{
				return this.OriginalFromAddress.ToString();
			}
			return this.FromAddress.ToString();
		}

		private bool ShouldSendOutboundSmtpUtf8(RoutingAddress sender, IReadOnlyMailItem mailItem)
		{
			bool flag = sender.IsUTF8;
			if (!flag)
			{
				flag = mailItem.Recipients.Any((MailRecipient recipient) => recipient.Email.IsUTF8);
			}
			return flag;
		}

		private const string XShadowKeyword = "XSHADOW";

		private const string DirectionalityParam = "XATTRDIRECT";

		private const string OrganizationIdParam = "XATTRORGID";

		private const string SystemProbeIdParam = "XSYSPROBEID";

		private const string MessageIdParam = "XMSGID";

		private const string OriginalFromParam = "XORIGFROM";

		private readonly MailCommandEventArgs mailCommandEventArgs;

		private string xshadow;

		private Guid shadowMessageId;

		private readonly ITransportAppConfig transportAppConfig;

		private MailCommandMessageContextParameters messageContext;

		private MailDirectionality directionality;

		private Guid externalOrganizationId;

		private OrganizationId internalOrganizationId;

		private string exoAccountForest;

		private string exoTenantContainer;

		private string internetMessageId;

		private Stopwatch outboundStopwatch;
	}
}
