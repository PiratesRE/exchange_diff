using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.SecureMail;
using Microsoft.Exchange.Security.Cryptography.X509Certificates;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.Transport.MessageThrottling;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal static class MailCommandHelpers
	{
		public static bool CanSubmitMessage(RoutingAddress address, SmtpInSessionState sessionState, out SmtpResponse submissionResponse)
		{
			ArgumentValidator.ThrowIfNull("sessionState", sessionState);
			submissionResponse = SmtpResponse.Empty;
			if (SmtpInSessionUtils.IsPartner(sessionState.RemoteIdentity))
			{
				sessionState.ResetToAnonymousIdentity();
				sessionState.Tracer.TraceDebug(0L, "Identity changed from Partner to Anonymous.");
			}
			string domainPart = address.DomainPart;
			if (domainPart != null && SmtpInSessionUtils.IsAnonymous(sessionState.RemoteIdentity) && sessionState.Configuration.TransportConfiguration.IsTlsReceiveSecureDomain(domainPart))
			{
				bool flag = false;
				bool flag2 = false;
				if (!sessionState.ReceiveConnector.DomainSecureEnabled)
				{
					sessionState.Tracer.TraceError<string, string>(0L, "Message from secure domain '{0}' on connector '{1}' failed to authenticate because TLS was not started.", domainPart, sessionState.ReceiveConnector.Name);
					sessionState.EventLog.LogEvent(TransportEventLogConstants.Tuple_TlsDomainSecureDisabled, domainPart + " - " + sessionState.ReceiveConnector.Name, new object[]
					{
						domainPart,
						sessionState.ReceiveConnector.Name
					});
					submissionResponse = SmtpResponse.DomainSecureDisabled;
					return false;
				}
				if (sessionState.SecureState != SecureState.StartTls)
				{
					if (sessionState.IsStartTlsSupported && (sessionState.ReceiveConnector.AuthMechanism & AuthMechanisms.Tls) != AuthMechanisms.None)
					{
						sessionState.Tracer.TraceError<string, string>(0L, "Message from secure domain '{0}' on connector '{1}' failed to authenticate because STARTTLS was not used.", domainPart, sessionState.ReceiveConnector.Name);
						sessionState.EventLog.LogEvent(TransportEventLogConstants.Tuple_MessageNotAuthenticatedTlsNotStarted, domainPart, new object[]
						{
							domainPart,
							sessionState.ReceiveConnector.Name
						});
						flag = true;
					}
					else
					{
						sessionState.Tracer.TraceError<string, string>(0L, "Message from secure domain '{0}' on connector '{1}' failed to authenticate because TLS was not started.  The receive connector didn't advertise StartTLS.", domainPart, sessionState.ReceiveConnector.Name);
						sessionState.EventLog.LogEvent(TransportEventLogConstants.Tuple_MessageNotAuthenticatedTlsNotAdvertised, domainPart, new object[]
						{
							domainPart,
							sessionState.ReceiveConnector.Name
						});
						flag2 = true;
					}
				}
				else if (sessionState.TlsRemoteCertificateInternal == null)
				{
					sessionState.Tracer.TraceError<string, string>(0L, "Message from secure domain '{0}' on connector '{1}' failed to authenticate because no client certificate was supplied.", domainPart, sessionState.ReceiveConnector.Name);
					sessionState.EventLog.LogEvent(TransportEventLogConstants.Tuple_MessageNotAuthenticatedNoClientCertificate, domainPart, new object[]
					{
						domainPart,
						sessionState.ReceiveConnector.Name
					});
					flag = true;
				}
				else
				{
					SmtpDomainWithSubdomains domain = new SmtpDomainWithSubdomains(new SmtpDomain(domainPart), true);
					ChainValidityStatus chainValidityStatus;
					if (!sessionState.CertificateValidator.MatchCertificateFqdns(domain, sessionState.TlsRemoteCertificateInternal, MatchOptions.MultiLevelCertWildcards, sessionState.ProtocolLogSession))
					{
						chainValidityStatus = ChainValidityStatus.SubjectMismatch;
						sessionState.EventLog.LogEvent(TransportEventLogConstants.Tuple_TlsDomainClientCertificateSubjectMismatch, domainPart, new object[]
						{
							domainPart,
							sessionState.ReceiveConnector.Name,
							sessionState.TlsRemoteCertificateInternal.Subject
						});
						string notificationReason = string.Format("A message from domain-secured domain '{0}' on connector '{1}' failed to authenticate because the Transport Layer Security (TLS) certificate does not contain the domain name.", domainPart, sessionState.ReceiveConnector.Name);
						sessionState.EventNotificationItem.Publish(ExchangeComponent.Transport.Name, "TlsDomainClientCertificateSubjectMismatch", null, notificationReason, ResultSeverityLevel.Error, false);
					}
					else
					{
						chainValidityStatus = sessionState.TlsRemoteCertificateChainValidationStatus;
						sessionState.EventLog.LogEvent(TransportEventLogConstants.Tuple_TlsDomainCertificateValidationFailure, domainPart, new object[]
						{
							domainPart,
							sessionState.ReceiveConnector.Name,
							chainValidityStatus
						});
						string notificationReason2 = string.Format("A secure connection from domain-secured domain '{0}' on connector '{1}' failed to authenticate because validation of the Transport Layer Security (TLS) certificate failed with status '{2}'.", domainPart, sessionState.ReceiveConnector.Name, chainValidityStatus);
						sessionState.EventNotificationItem.Publish(ExchangeComponent.Transport.Name, "TlsDomainCertificateValidationFailure", null, notificationReason2, ResultSeverityLevel.Error, false);
					}
					if (chainValidityStatus != ChainValidityStatus.Valid)
					{
						sessionState.Tracer.TraceError<string, string, ChainValidityStatus>(0L, "Message from secure domain '{0}' on connector '{1}' failed to authenticate because the TLS client certificate chain failed to validate and returned status '{2}'.", domainPart, sessionState.ReceiveConnector.Name, chainValidityStatus);
						submissionResponse = SmtpResponse.CertificateValidationFailure;
						return false;
					}
				}
				if (flag)
				{
					sessionState.Tracer.TraceError(0L, "530 5.7.1 Not authenticated");
					submissionResponse = SmtpResponse.NotAuthenticated;
					return false;
				}
				if (flag2)
				{
					sessionState.Tracer.TraceError(0L, "454 4.7.0 Temporary authentication failure");
					submissionResponse = SmtpResponse.AuthTempFailure;
					return false;
				}
				sessionState.ResetToPartnerServersIdentity(domainPart);
				sessionState.Tracer.TraceError(0L, "Identity changed from Anonymous to Partner.");
			}
			if (SmtpInSessionUtils.HasSMTPSubmitPermission(sessionState.CombinedPermissions))
			{
				return true;
			}
			submissionResponse = (SmtpInSessionUtils.IsAnonymous(sessionState.RemoteIdentity) ? SmtpResponse.UnableToAcceptAnonymousSession : SmtpResponse.SubmitDenied);
			return false;
		}

		public static ParseResult CheckArgumentPermissions(SmtpInSessionState sessionState, MailParseOutput parseOutput)
		{
			ArgumentValidator.ThrowIfNull("sessionState", sessionState);
			ArgumentValidator.ThrowIfNull("parseOutput", parseOutput);
			ParseResult result = MailCommandHelpers.CheckOorgPermissions(sessionState, parseOutput);
			if (result.IsFailed)
			{
				return result;
			}
			ParseResult result2 = MailCommandHelpers.CheckXAttrPermissions(sessionState, parseOutput);
			if (result2.IsFailed)
			{
				return result2;
			}
			ParseResult result3 = MailCommandHelpers.CheckXSysProbePermissions(sessionState, parseOutput);
			if (result3.IsFailed)
			{
				return result3;
			}
			ParseResult result4 = MailCommandHelpers.CheckXOrigFromPermissions(sessionState, parseOutput);
			if (result4.IsFailed)
			{
				return result4;
			}
			return ParseResult.ParsingComplete;
		}

		public static Guid ExtractShadowMessageId(SmtpInSessionState sessionState, MailParseOutput parseOutput)
		{
			ArgumentValidator.ThrowIfNull("sessionState", sessionState);
			ArgumentValidator.ThrowIfNull("parseOutput", parseOutput);
			Guid result = Guid.Empty;
			if (string.IsNullOrEmpty(parseOutput.XShadow) || !SmtpInSessionUtils.HasSMTPAcceptOrgHeadersPermission(sessionState.CombinedPermissions))
			{
				return Guid.Empty;
			}
			Guid guid;
			if (GuidHelper.TryParseGuid(parseOutput.XShadow, out guid))
			{
				result = guid;
			}
			else
			{
				sessionState.Tracer.TraceError<string, string>(0L, "Message on connector '{0}' doesn't contain a valid XSHADOW parameter value: {1}", sessionState.ReceiveConnector.Name, parseOutput.XShadow);
			}
			return result;
		}

		public static bool AreAllMandatoryBlobsPresent(MailCommandMessageContextParameters mailFromMessageContextParameters, SmtpInSessionState sessionState)
		{
			ArgumentValidator.ThrowIfNull("sessionState", sessionState);
			foreach (SmtpMessageContextBlob smtpMessageContextBlob in sessionState.MessageContextBlob.GetAdvertisedMandatoryBlobs(sessionState.AdvertisedEhloOptions))
			{
				if (!MailCommandHelpers.IsMandatoryBlobSpecified(mailFromMessageContextParameters, smtpMessageContextBlob))
				{
					sessionState.Tracer.TraceError<string>(0L, "Did not receive mandatory XMESSAGECONTEXT blob {0} in MAIL command", smtpMessageContextBlob.Name);
					sessionState.ProtocolLogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "Did not find mandatory XMESSAGECONTEXT blob {0} in MAIL command", new object[]
					{
						smtpMessageContextBlob.Name
					});
					return false;
				}
			}
			return true;
		}

		public static bool AreSpecifiedBlobsAdvertised(MailCommandMessageContextParameters mailFromMessageContextParameters, SmtpInSessionState sessionState)
		{
			ArgumentValidator.ThrowIfNull("sessionState", sessionState);
			if (mailFromMessageContextParameters != null && mailFromMessageContextParameters.OrderedListOfBlobs != null)
			{
				using (IEnumerator<IInboundMessageContextBlob> enumerator = (from blob in mailFromMessageContextParameters.OrderedListOfBlobs
				where !blob.IsAdvertised(sessionState.AdvertisedEhloOptions)
				select blob).GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						SmtpMessageContextBlob smtpMessageContextBlob = (SmtpMessageContextBlob)enumerator.Current;
						sessionState.Tracer.TraceError<string>(0L, "Found unexpected XMESSAGECONTEXT blob in MAIL command {0}", smtpMessageContextBlob.Name);
						sessionState.ProtocolLogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "Found unexpected XMESSAGECONTEXT blob {0} in MAIL command", new object[]
						{
							smtpMessageContextBlob.Name
						});
						return false;
					}
				}
				return true;
			}
			return true;
		}

		public static bool IsValidXShadow(string xshadow, SmtpInSessionState sessionState)
		{
			ArgumentValidator.ThrowIfNull("sessionState", sessionState);
			if (!SmtpInSessionUtils.IsShadowedBySender(sessionState.SenderShadowContext) && !SmtpInSessionUtils.IsPeerShadowSession(sessionState.PeerSessionPrimaryServer))
			{
				return string.IsNullOrEmpty(xshadow);
			}
			if (string.IsNullOrEmpty(xshadow))
			{
				sessionState.Tracer.TraceDebug<string>(0L, "Session is shadowed; connector '{0}'. Message received on the session did not contain XSHADOW optional parameter.", sessionState.ReceiveConnector.Name);
			}
			return true;
		}

		public static bool HasMessageRateLimitExceeded(SmtpInSessionState sessionState)
		{
			ArgumentValidator.ThrowIfNull("sessionState", sessionState);
			if (SmtpInSessionUtils.HasSMTPBypassMessageSizeLimitPermission(sessionState.CombinedPermissions) || sessionState.ReceiveConnector.MessageRateSource == MessageRateSourceFlags.None || !sessionState.MessageThrottlingManager.Enabled)
			{
				return false;
			}
			int num = sessionState.ReceiveConnector.MessageRateLimit.IsUnlimited ? int.MaxValue : sessionState.ReceiveConnector.MessageRateLimit.Value;
			MessageRateSourceFlags messageRateSource = sessionState.ReceiveConnector.MessageRateSource;
			Guid? guid = null;
			int num2 = 0;
			MessageThrottlingReason messageThrottlingReason;
			if ((messageRateSource & MessageRateSourceFlags.User) == MessageRateSourceFlags.User)
			{
				if (sessionState.AuthenticatedUser != null)
				{
					TransportMiniRecipient authenticatedUser = sessionState.AuthenticatedUser;
					switch ((Microsoft.Exchange.Data.Directory.Recipient.RecipientType)authenticatedUser[ADRecipientSchema.RecipientType])
					{
					case Microsoft.Exchange.Data.Directory.Recipient.RecipientType.PublicFolder:
					case Microsoft.Exchange.Data.Directory.Recipient.RecipientType.PublicDatabase:
					case Microsoft.Exchange.Data.Directory.Recipient.RecipientType.SystemAttendantMailbox:
					case Microsoft.Exchange.Data.Directory.Recipient.RecipientType.SystemMailbox:
					case Microsoft.Exchange.Data.Directory.Recipient.RecipientType.MicrosoftExchange:
						break;
					default:
						guid = new Guid?(authenticatedUser.ExchangeGuid);
						if (guid == Guid.Empty)
						{
							guid = null;
						}
						break;
					}
					if (guid != null)
					{
						IThrottlingPolicy throttlingPolicy = sessionState.GetThrottlingPolicy();
						if (throttlingPolicy != null)
						{
							if (throttlingPolicy.MessageRateLimit.IsUnlimited)
							{
								guid = null;
							}
							else if (throttlingPolicy.MessageRateLimit.Value < 2147483647U)
							{
								num2 = (int)throttlingPolicy.MessageRateLimit.Value;
							}
							else
							{
								num2 = int.MaxValue;
							}
						}
					}
				}
				if (guid != null && guid.Value != Guid.Empty)
				{
					messageThrottlingReason = sessionState.MessageThrottlingManager.ShouldThrottleMessage(guid.Value, num2, sessionState.NetworkConnection.RemoteEndPoint.Address, num, messageRateSource);
				}
				else
				{
					messageThrottlingReason = sessionState.MessageThrottlingManager.ShouldThrottleMessage(sessionState.NetworkConnection.RemoteEndPoint.Address, num, MessageRateSourceFlags.IPAddress);
				}
			}
			else
			{
				messageThrottlingReason = sessionState.MessageThrottlingManager.ShouldThrottleMessage(sessionState.NetworkConnection.RemoteEndPoint.Address, num, messageRateSource);
			}
			if (messageThrottlingReason != MessageThrottlingReason.NotThrottled)
			{
				sessionState.Tracer.TraceDebug(0L, "The connection for user [{0}] with IP address [{1}] is being dropped. The rate of message submissions from this connection has exceeded the throttling policy of {2} per minute.", new object[]
				{
					sessionState.NetworkConnection.RemoteEndPoint.Address,
					sessionState.RemoteIdentity,
					sessionState.NetworkConnection.RemoteEndPoint.Address,
					(messageThrottlingReason == MessageThrottlingReason.IPAddressLimitExceeded) ? num : num2
				});
				sessionState.EventLog.LogEvent(TransportEventLogConstants.Tuple_SmtpReceiveMessageRateLimitExceeded, sessionState.NetworkConnection.RemoteEndPoint.Address.ToString(), new object[]
				{
					sessionState.RemoteIdentity,
					sessionState.NetworkConnection.RemoteEndPoint.Address,
					(messageThrottlingReason == MessageThrottlingReason.IPAddressLimitExceeded) ? num : num2
				});
				return true;
			}
			return false;
		}

		public static ParseResult ValidateMessageId(SmtpInSessionState sessionState, string internetMessageId)
		{
			ArgumentValidator.ThrowIfNull("sessionState", sessionState);
			if (string.IsNullOrEmpty(internetMessageId))
			{
				return ParseResult.ParsingComplete;
			}
			if (!sessionState.AdvertisedEhloOptions.XMsgId)
			{
				return ParseResult.CommandNotImplementedProtocolError;
			}
			if (sessionState.AuthMethod != MultilevelAuthMechanism.MUTUALGSSAPI)
			{
				return ParseResult.NotAuthorized;
			}
			if (sessionState.IsMessagePoison(internetMessageId))
			{
				sessionState.ProtocolLogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "Rejecting Message-ID: {0} because it was detected as poison", new object[]
				{
					internetMessageId
				});
				return ParseResult.TooManyRelatedErrors;
			}
			return ParseResult.ParsingComplete;
		}

		public static bool ValidateSizeRestrictions(SmtpInSessionState sessionState, long size)
		{
			ArgumentValidator.ThrowIfNull("sessionState", sessionState);
			if (SmtpInSessionUtils.HasSMTPBypassMessageSizeLimitPermission(sessionState.CombinedPermissions) || SmtpInSessionUtils.HasSMTPAcceptOrgHeadersPermission(sessionState.CombinedPermissions) || size <= (long)sessionState.ReceiveConnector.MaxMessageSize.ToBytes())
			{
				return true;
			}
			if (sessionState.ReceiveConnectorStub.SmtpReceivePerfCounterInstance != null)
			{
				sessionState.ReceiveConnectorStub.SmtpReceivePerfCounterInstance.MessagesRefusedForSize.Increment();
			}
			return false;
		}

		public static bool ValidateSecureState(SmtpInSessionState sessionState)
		{
			ArgumentValidator.ThrowIfNull("sessionState", sessionState);
			if (MailCommandHelpers.IsValidSecureState(sessionState))
			{
				return true;
			}
			string text = string.Format("The {0} connector is configured to receive mail only over TLS connections and we didn't achieve it", sessionState.ReceiveConnector.Name);
			sessionState.Tracer.TraceError(0L, text);
			sessionState.EventLog.LogEvent(TransportEventLogConstants.Tuple_SmtpReceiveTLSRequiredFailed, sessionState.ReceiveConnector.Name, new object[]
			{
				sessionState.ReceiveConnector.Name
			});
			sessionState.EventNotificationItem.Publish(ExchangeComponent.Transport.Name, "SmtpTLSReceiveRequiredFailed", null, text, ResultSeverityLevel.Warning, false);
			return false;
		}

		private static ParseResult CheckOorgPermissions(SmtpInSessionState sessionState, MailParseOutput parseOutput)
		{
			if (!string.IsNullOrEmpty(parseOutput.Oorg))
			{
				if (!sessionState.AdvertisedEhloOptions.XOorg)
				{
					return ParseResult.InvalidArguments;
				}
				if (!SmtpInSessionUtils.ShouldAcceptOorgProtocol(sessionState.Capabilities))
				{
					return ParseResult.NotAuthorized;
				}
			}
			return ParseResult.ParsingComplete;
		}

		private static ParseResult CheckXAttrPermissions(SmtpInSessionState sessionState, MailParseOutput parseOutput)
		{
			if (parseOutput.Directionality != MailDirectionality.Undefined || parseOutput.XAttrOrgId != null)
			{
				if (!sessionState.AdvertisedEhloOptions.XAttr)
				{
					return ParseResult.InvalidArguments;
				}
				if (!SmtpInSessionUtils.ShouldAcceptXAttrProtocol(sessionState.Capabilities) && !SmtpInSessionUtils.HasSMTPAcceptXAttrPermission(sessionState.CombinedPermissions))
				{
					return ParseResult.NotAuthorized;
				}
			}
			return ParseResult.ParsingComplete;
		}

		private static ParseResult CheckXOrigFromPermissions(SmtpInSessionState sessionState, MailParseOutput parseOutput)
		{
			if (!RoutingAddress.IsEmpty(parseOutput.OriginalFromAddress))
			{
				if (!sessionState.AdvertisedEhloOptions.XOrigFrom)
				{
					return ParseResult.InvalidArguments;
				}
				if (!SmtpInSessionUtils.ShouldAcceptXOrigFromProtocol(sessionState.Capabilities) && !SmtpInSessionUtils.HasSMTPAcceptXOrigFromPermission(sessionState.CombinedPermissions))
				{
					return ParseResult.NotAuthorized;
				}
			}
			return ParseResult.ParsingComplete;
		}

		private static ParseResult CheckXSysProbePermissions(SmtpInSessionState sessionState, MailParseOutput parseOutput)
		{
			if (parseOutput.SystemProbeId != Guid.Empty && !SmtpInSessionUtils.ShouldAcceptXSysProbeProtocol(sessionState.Capabilities) && !SmtpInSessionUtils.HasSMTPAcceptXSysProbePermission(sessionState.CombinedPermissions))
			{
				return ParseResult.NotAuthorized;
			}
			return ParseResult.ParsingComplete;
		}

		private static bool IsValidSecureState(SmtpInSessionState sessionState)
		{
			return !sessionState.ReceiveConnector.RequireTLS || sessionState.SecureState != SecureState.None;
		}

		private static bool IsMandatoryBlobSpecified(MailCommandMessageContextParameters messageContextParameters, SmtpMessageContextBlob blobToLookup)
		{
			return messageContextParameters != null && messageContextParameters.OrderedListOfBlobs != null && messageContextParameters.OrderedListOfBlobs.Any((IInboundMessageContextBlob receivedMailFromArguments) => blobToLookup.Name.Equals(receivedMailFromArguments.Name, StringComparison.OrdinalIgnoreCase));
		}
	}
}
