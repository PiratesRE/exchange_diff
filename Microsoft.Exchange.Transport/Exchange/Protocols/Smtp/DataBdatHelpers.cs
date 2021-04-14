using System;
using System.Globalization;
using System.IO;
using System.Net;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Mime.Internal;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.SecureMail;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.Transport.Categorizer;
using Microsoft.Exchange.Transport.Logging;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal static class DataBdatHelpers
	{
		public static bool CheckMessageSubmitPermissions(ISmtpInSession session, SmtpCommand command)
		{
			if (!SmtpInSessionUtils.HasSMTPSubmitPermission(session.Permissions))
			{
				ExTraceGlobals.SmtpReceiveTracer.TraceDebug<string, string>((long)session.GetHashCode(), "Authenticated user '{0}' has no permission to submit mails with connector '{1}'", session.RemoteIdentityName, session.Connector.Name);
				SmtpCommand.EventLogger.LogEvent(TransportEventLogConstants.Tuple_SmtpReceiveSubmitDenied, null, new object[]
				{
					session.TransportMailItem.From,
					session.Connector.Name,
					session.RemoteIdentityName
				});
				command.SmtpResponse = SmtpResponse.SubmitDenied;
				command.IsResponseReady = false;
				return false;
			}
			return true;
		}

		public static SmtpResponse CheckMessageSubmitPermissions(SmtpInSessionState sessionState)
		{
			ArgumentValidator.ThrowIfNull("sessionState", sessionState);
			if (!SmtpInSessionUtils.HasSMTPSubmitPermission(sessionState.CombinedPermissions))
			{
				sessionState.Tracer.TraceDebug<string, string>((long)sessionState.GetHashCode(), "Authenticated user '{0}' has no permission to submit mails with connector '{1}'", sessionState.RemoteIdentityName, sessionState.ReceiveConnector.Name);
				sessionState.EventLog.LogEvent(TransportEventLogConstants.Tuple_SmtpReceiveSubmitDenied, null, new object[]
				{
					sessionState.TransportMailItem.From,
					sessionState.ReceiveConnector.Name,
					sessionState.RemoteIdentityName
				});
				return SmtpResponse.SubmitDenied;
			}
			return SmtpResponse.Empty;
		}

		public static bool MessageSizeExceeded(long currentMessageSize, long originalMessageSize, long messageSizeLimit, Permission permissions)
		{
			return !SmtpInSessionUtils.HasSMTPBypassMessageSizeLimitPermission(permissions) && Math.Min(currentMessageSize, originalMessageSize) > messageSizeLimit;
		}

		public static void EnableEOHEvent(ISmtpInSession session, HeaderList headers, out EndOfHeadersEventArgs eventArgs)
		{
			eventArgs = new EndOfHeadersEventArgs(session.SessionSource);
			eventArgs.Headers = headers;
			eventArgs.MailItem = session.TransportMailItemWrapper;
		}

		public static IAsyncResult RaiseEOHEvent(object state, ISmtpInSession session, AsyncCallback callback, EndOfHeadersEventArgs eventArgs)
		{
			if (session.TransportMailItem != null)
			{
				session.AgentLatencyTracker.BeginTrackLatency(LatencyComponent.SmtpReceiveOnEndOfHeaders, session.TransportMailItem.LatencyTracker);
			}
			return session.AgentSession.BeginRaiseEvent("OnEndOfHeaders", ReceiveMessageEventSourceImpl.Create(session.SessionSource, eventArgs.MailItem), eventArgs, callback, state);
		}

		public static bool CheckHeaders(HeaderList headerList, ISmtpInSession session, long eohPosition, SmtpCommand command)
		{
			if (eohPosition == -1L)
			{
				eohPosition = MimeInternalHelpers.GetDocumentPosition(session.MimeDocument);
			}
			long maxHeaderSize = (long)session.Connector.MaxHeaderSize.ToBytes();
			if (DataBdatHelpers.IsHeaderTooLarge(maxHeaderSize, eohPosition, session.Permissions, session.LogSession))
			{
				command.SmtpResponse = SmtpResponse.HeadersTooLarge;
				command.IsResponseReady = false;
				return false;
			}
			if (DataBdatHelpers.IsPartialMessage(headerList))
			{
				command.SmtpResponse = SmtpResponse.MessagePartialNotSupported;
				command.IsResponseReady = false;
				return false;
			}
			return true;
		}

		public static SmtpResponse PerformHeaderSizeAndPartialMessageChecks(HeaderList headerList, SmtpInSessionState sessionState, long eohPosition)
		{
			ArgumentValidator.ThrowIfNull("headerList", headerList);
			ArgumentValidator.ThrowIfNull("sessionState", sessionState);
			if (eohPosition == -1L)
			{
				eohPosition = MimeInternalHelpers.GetDocumentPosition(sessionState.TransportMailItem.MimeDocument);
			}
			long maxHeaderSize = (long)sessionState.ReceiveConnector.MaxHeaderSize.ToBytes();
			if (DataBdatHelpers.IsHeaderTooLarge(maxHeaderSize, eohPosition, sessionState.CombinedPermissions, sessionState.ProtocolLogSession))
			{
				return SmtpResponse.HeadersTooLarge;
			}
			if (DataBdatHelpers.IsPartialMessage(headerList))
			{
				return SmtpResponse.MessagePartialNotSupported;
			}
			return SmtpResponse.Empty;
		}

		public static void UpdateDagSelectorPerformanceCounters(HeaderList headerList, bool checkDagSelectorHeader, ISmtpReceivePerfCounters smtpReceivePerfCounters)
		{
			if (checkDagSelectorHeader && smtpReceivePerfCounters != null)
			{
				Header header = headerList.FindFirst("X-MS-Exchange-Organization-RoutedUsingDagSelector");
				if (header != null)
				{
					smtpReceivePerfCounters.MessagesReceivedForNonProvisionedUsers.Increment();
				}
			}
		}

		public static bool CheckMaxHopCounts(HeaderList headerList, ISmtpInSession session, SmtpCommand command, bool localLoopDetectionEnabled, int localLoopSubdomainDepth)
		{
			int num;
			return DataBdatHelpers.CheckMaxHopCounts(headerList, session, command, localLoopDetectionEnabled, localLoopSubdomainDepth, null, out num);
		}

		public static bool CheckMaxHopCounts(HeaderList headerList, ISmtpInSession session, SmtpCommand command, bool localLoopDetectionEnabled, int localLoopSubdomainDepth, int[] localLoopMessageDeferralIntervals, out int localLoopDeferralSeconds)
		{
			localLoopDeferralSeconds = 0;
			Header[] array = headerList.FindAll(HeaderId.Received);
			if (array == null || array.Length == 0)
			{
				return true;
			}
			if (array.Length > session.Connector.MaxHopCount)
			{
				command.SmtpResponse = SmtpResponse.HopCountExceeded;
				session.LogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "ReceivedHeadersLength: {0} > MaxHopCount: {1}", new object[]
				{
					array.Length,
					session.Connector.MaxHopCount
				});
				command.IsResponseReady = false;
				return false;
			}
			if (!localLoopDetectionEnabled)
			{
				return true;
			}
			int num;
			if (Util.IsLocalHopCountExceeded(array, session.AdvertisedDomain, localLoopSubdomainDepth, session.Connector.MaxLocalHopCount, out num))
			{
				command.SmtpResponse = SmtpResponse.HopCountExceeded;
				session.LogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "LocalHopCount: {0} > MaxLocalHopCount: {1}", new object[]
				{
					num,
					session.Connector.MaxLocalHopCount
				});
				session.UpdateSmtpAvailabilityPerfCounter(LegitimateSmtpAvailabilityCategory.RejectDueToMaxLocalLoopCount);
				command.IsResponseReady = false;
				return false;
			}
			if (num > 0 && localLoopMessageDeferralIntervals != null)
			{
				if (num <= localLoopMessageDeferralIntervals.Length)
				{
					localLoopDeferralSeconds = localLoopMessageDeferralIntervals[num - 1];
				}
				else
				{
					localLoopDeferralSeconds = localLoopMessageDeferralIntervals[localLoopMessageDeferralIntervals.Length - 1];
				}
			}
			return true;
		}

		public static SmtpResponse CheckMaxHopCounts(HeaderList headerList, SmtpInSessionState sessionState, out int localLoopDeferralSeconds)
		{
			ArgumentValidator.ThrowIfNull("headerList", headerList);
			ArgumentValidator.ThrowIfNull("sessionState", sessionState);
			localLoopDeferralSeconds = 0;
			Header[] array = headerList.FindAll(HeaderId.Received);
			if (array.Length == 0)
			{
				return SmtpResponse.Empty;
			}
			if (array.Length > sessionState.ReceiveConnector.MaxHopCount)
			{
				sessionState.ProtocolLogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "ReceivedHeadersLength: {0} > MaxHopCount: {1}", new object[]
				{
					array.Length,
					sessionState.ReceiveConnector.MaxHopCount
				});
				return SmtpResponse.HopCountExceeded;
			}
			if (!sessionState.Configuration.RoutingConfiguration.LocalLoopDetectionEnabled)
			{
				return SmtpResponse.Empty;
			}
			int num;
			if (Util.IsLocalHopCountExceeded(array, sessionState.AdvertisedDomain, sessionState.Configuration.RoutingConfiguration.LocalLoopSubdomainDepth, sessionState.ReceiveConnector.MaxLocalHopCount, out num))
			{
				sessionState.ProtocolLogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "LocalHopCount: {0} > MaxLocalHopCount: {1}", new object[]
				{
					num,
					sessionState.ReceiveConnector.MaxLocalHopCount
				});
				sessionState.UpdateSmtpAvailabilityPerfCounter(LegitimateSmtpAvailabilityCategory.RejectDueToMaxLocalLoopCount);
				return SmtpResponse.HopCountExceeded;
			}
			if (num > 0 && sessionState.Configuration.RoutingConfiguration.LocalLoopMessageDeferralIntervals != null)
			{
				localLoopDeferralSeconds = ((num <= sessionState.Configuration.RoutingConfiguration.LocalLoopMessageDeferralIntervals.Count) ? sessionState.Configuration.RoutingConfiguration.LocalLoopMessageDeferralIntervals[num - 1] : sessionState.Configuration.RoutingConfiguration.LocalLoopMessageDeferralIntervals[sessionState.Configuration.RoutingConfiguration.LocalLoopMessageDeferralIntervals.Count - 1]);
			}
			return SmtpResponse.Empty;
		}

		public static void UpdateLoopDetectionCounter(HeaderList headerList, ISmtpInSession session, int subDomainLeftToRightOffsetForPerfCounter, int maxLocalHopCount, string messageId)
		{
			Header[] array = headerList.FindAll(HeaderId.Received);
			if (array == null || array.Length == 0)
			{
				return;
			}
			int num = session.AdvertisedDomain.Split(new char[]
			{
				'.'
			}).Length - 1;
			int localLoopSubdomainDepth;
			if (subDomainLeftToRightOffsetForPerfCounter == 0)
			{
				localLoopSubdomainDepth = 0;
			}
			else if (subDomainLeftToRightOffsetForPerfCounter > num)
			{
				localLoopSubdomainDepth = 1;
			}
			else
			{
				localLoopSubdomainDepth = num - subDomainLeftToRightOffsetForPerfCounter;
			}
			int num2;
			if (Util.IsLocalHopCountExceeded(array, session.AdvertisedDomain, localLoopSubdomainDepth, maxLocalHopCount - 1, out num2) && num2 == maxLocalHopCount)
			{
				session.LogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "LocalHopCount: {0} == MaxLocalHopCount: {1}, updating the perf counter for MessageID: {2}", new object[]
				{
					num2,
					maxLocalHopCount,
					messageId
				});
				session.IncrementSmtpAvailabilityPerfCounterForMessageLoopsInLastHour(1L);
			}
		}

		public static void PatchHeaders(HeaderList headerList, ISmtpInSession session, bool acceptAndFixSmtpAddressWithInvalidLocalPart, out string messageId)
		{
			string from;
			string fromTcpInfo;
			if (session.IsAnonymousClientProxiedSession)
			{
				from = session.ProxyHopHelloDomain;
				fromTcpInfo = session.ProxyHopAddress.ToString();
			}
			else
			{
				from = session.HelloSmtpDomain;
				fromTcpInfo = session.RemoteEndPoint.Address.ToString();
			}
			DateHeader dateHeader = new DateHeader("Date", session.TransportMailItem.DateReceived.ToLocalTime());
			string value = dateHeader.Value;
			string text = "Microsoft SMTP Server";
			if (session.IsTls)
			{
				text += " (TLS)";
			}
			string via;
			if (Components.Configuration.ProcessTransportRole == ProcessTransportRole.FrontEnd)
			{
				via = DataBdatHelpers.ViaFrontEndTransport;
			}
			else if (Components.Configuration.ProcessTransportRole == ProcessTransportRole.MailboxDelivery)
			{
				via = "Mailbox Transport";
			}
			else
			{
				via = null;
			}
			ReceivedHeader receivedHeader = new ReceivedHeader(from, fromTcpInfo, session.AdvertisedDomain, session.LocalEndPoint.Address.ToString(), null, text, session.SmtpInServer.Version.ToString(), via, value);
			bool flag = headerList.FindFirst("X-MS-Exchange-Organization-OriginalArrivalTime") != null;
			Util.PatchHeaders(headerList, receivedHeader, session.TransportMailItem.From, session.TransportMailItem.DateReceived, session.SmtpInServer.ServerConfiguration.Fqdn, Components.Configuration.LocalServer.TransportServer.IsHubTransportServer, out messageId);
			Header[] array = headerList.FindAll(HeaderId.Received);
			IPAddress ipaddress = null;
			if (array != null)
			{
				ipaddress = Util.FindOriginatingIPFromHeadersAndStampOriginalClientServerIP(array, session.LocalEndPoint.Address, headerList, session.SmtpInServer.TransportSettings.InternalSMTPServers);
				if (!session.SessionSource.IsExternalConnection)
				{
					if (ipaddress != null)
					{
						ExTraceGlobals.SmtpReceiveTracer.TraceDebug<IPAddress>(0L, "{0} is the originating IP", ipaddress);
						session.SessionSource.LastExternalIPAddress = ipaddress;
					}
					else
					{
						session.SessionSource.LastExternalIPAddress = null;
					}
				}
			}
			if (session.InboundExch50 != null)
			{
				session.InboundExch50.PatchHeaders(headerList, !flag);
			}
			bool flag2;
			DataBdatHelpers.UpdateMessageAuthenticationStatus(session, headerList, out flag2);
			if (flag2 && ipaddress != null)
			{
				Util.SetAsciiHeader(headerList, "X-Originating-IP", string.Format(CultureInfo.InvariantCulture, "[{0}]", new object[]
				{
					ipaddress
				}));
			}
			if (acceptAndFixSmtpAddressWithInvalidLocalPart)
			{
				Util.FixP2HeadersWithInvalidLocalPart(headerList);
			}
			if (session.InboundClientProxyState != InboundClientProxyStates.None)
			{
				Header header = Header.Create("X-ClientProxiedBy");
				string value2 = string.Format("{0} ({1}) To {2} ({3})", new object[]
				{
					session.ProxyHopHelloDomain,
					session.ProxyHopAddress,
					session.AdvertisedDomain,
					session.LocalEndPoint.Address
				});
				header.Value = value2;
				headerList.AppendChild(header);
			}
		}

		public static void PatchHeaders(HeaderList headerList, SmtpInSessionState sessionState, out string messageId)
		{
			ArgumentValidator.ThrowIfNull("headerList", headerList);
			ArgumentValidator.ThrowIfNull("sessionState", sessionState);
			string helloDomain = sessionState.HelloDomain;
			string fromTcpInfo = sessionState.NetworkConnection.RemoteEndPoint.Address.ToString();
			DateHeader dateHeader = new DateHeader("Date", sessionState.TransportMailItem.DateReceived.ToLocalTime());
			string value = dateHeader.Value;
			string text = "Microsoft SMTP Server";
			if (sessionState.NetworkConnection.IsTls)
			{
				text += " (TLS)";
			}
			string via;
			if (sessionState.Configuration.TransportConfiguration.ProcessTransportRole == ProcessTransportRole.FrontEnd)
			{
				via = DataBdatHelpers.ViaFrontEndTransport;
			}
			else if (sessionState.Configuration.TransportConfiguration.ProcessTransportRole == ProcessTransportRole.MailboxDelivery)
			{
				via = "Mailbox Transport";
			}
			else
			{
				via = null;
			}
			ReceivedHeader receivedHeader = new ReceivedHeader(helloDomain, fromTcpInfo, sessionState.AdvertisedDomain, sessionState.NetworkConnection.LocalEndPoint.Address.ToString(), null, text, sessionState.Configuration.TransportConfiguration.Version.ToString(), via, value);
			Util.PatchHeaders(headerList, receivedHeader, sessionState.TransportMailItem.From, sessionState.TransportMailItem.DateReceived, sessionState.Configuration.TransportConfiguration.Fqdn, sessionState.Configuration.TransportConfiguration.IsHubTransportServer, out messageId);
			Header[] array = headerList.FindAll(HeaderId.Received);
			IPAddress ipaddress = null;
			if (array != null)
			{
				ipaddress = Util.FindOriginatingIPFromHeadersAndStampOriginalClientServerIP(array, sessionState.NetworkConnection.LocalEndPoint.Address, headerList, sessionState.Configuration.TransportConfiguration.InternalSMTPServers);
				if (!sessionState.IsExternalConnection)
				{
					if (ipaddress != null)
					{
						sessionState.Tracer.TraceDebug<IPAddress>(0L, "{0} is the originating IP", ipaddress);
						sessionState.LastExternalIPAddress = ipaddress;
					}
					else
					{
						sessionState.LastExternalIPAddress = null;
					}
				}
			}
			bool flag;
			DataBdatHelpers.UpdateMessageAuthenticationStatus(sessionState, headerList, out flag);
			if (flag && ipaddress != null)
			{
				Util.SetAsciiHeader(headerList, "X-Originating-IP", string.Format(CultureInfo.InvariantCulture, "[{0}]", new object[]
				{
					ipaddress
				}));
			}
			if (sessionState.Configuration.TransportConfiguration.AcceptAndFixSmtpAddressWithInvalidLocalPart)
			{
				Util.FixP2HeadersWithInvalidLocalPart(headerList);
			}
		}

		public static string GetOorg(TransportMailItem transportMailItem, SmtpReceiveCapabilities sessionSmtpReceiveCapabilities, IProtocolLogSession protocolLogSession, HeaderList headers)
		{
			ArgumentValidator.ThrowIfNull("transportMailItem", transportMailItem);
			ArgumentValidator.ThrowIfNull("protocolLogSession", protocolLogSession);
			string text = transportMailItem.Oorg;
			string text2 = "MAIL FROM:";
			if (string.IsNullOrEmpty(text))
			{
				Header header = headers.FindFirst("X-MS-Exchange-Organization-OriginatorOrganization");
				if (header != null)
				{
					text = Util.GetHeaderValue(header);
					text2 = "X-MS-Exchange-Organization-OriginatorOrganization";
				}
			}
			if (string.IsNullOrEmpty(text))
			{
				foreach (string text3 in MimeConstant.XOriginatorOrganization)
				{
					Header header2 = headers.FindFirst(text3);
					if (header2 != null)
					{
						text = Util.GetHeaderValue(header2);
						text2 = text3;
						break;
					}
				}
				if (!string.IsNullOrEmpty(text) && !SmtpInSessionUtils.HasAcceptOorgHeaderCapability(sessionSmtpReceiveCapabilities))
				{
					protocolLogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "Ignored {0} header value '{1}' because session capabilities do not allow it", new object[]
					{
						text2,
						text
					});
					text = null;
				}
			}
			if (!string.IsNullOrEmpty(text))
			{
				if (!RoutingAddress.IsValidDomain(text))
				{
					protocolLogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "Ignored invalid OORG '{0}' from source '{1}'", new object[]
					{
						text,
						text2
					});
					text = null;
				}
				else
				{
					protocolLogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "Set mail item OORG to '{0}' based on '{1}'", new object[]
					{
						text,
						text2
					});
				}
			}
			return text;
		}

		public static bool TryGetOriginalSize(HeaderList headerList, out long size)
		{
			ArgumentValidator.ThrowIfNull("headerList", headerList);
			size = 0L;
			Header header = headerList.FindFirst("X-MS-Exchange-Organization-OriginalSize");
			return header != null && long.TryParse(header.Value, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out size);
		}

		public static SmtpResponse CheckAttributionAndCreateAdRecipientCache(TransportMailItem transportMailItem, bool rejectUnscopedMessages, bool isAttributedClient, bool isProxy)
		{
			ArgumentValidator.ThrowIfNull("transportMailItem", transportMailItem);
			if (transportMailItem.ExternalOrganizationId == Guid.Empty || transportMailItem.Directionality == MailDirectionality.Undefined)
			{
				ADOperationResult adoperationResult = DataBdatHelpers.AttributeAuthenticatedClientMessageOrUsingHeaders(transportMailItem, isAttributedClient);
				switch (adoperationResult.ErrorCode)
				{
				case ADOperationErrorCode.RetryableError:
					if (!isProxy)
					{
						return SmtpResponse.HubAttributionTransientFailureInEOH;
					}
					return SmtpResponse.ProxyAttributionTransientFailureinEOH;
				case ADOperationErrorCode.PermanentError:
					if (rejectUnscopedMessages)
					{
						if (!isProxy)
						{
							return SmtpResponse.HubAttributionFailureInEOH;
						}
						return SmtpResponse.ProxyAttributionFailureInEOH;
					}
					else
					{
						SmtpResponse result = DataBdatHelpers.DoFallbackAttribution(transportMailItem, isProxy);
						if (!result.Equals(SmtpResponse.Empty))
						{
							return result;
						}
					}
					break;
				}
			}
			return DataBdatHelpers.CreateADRecipientCache(transportMailItem, rejectUnscopedMessages, isProxy);
		}

		public static SmtpResponse ValidateHeaderSize(SmtpInSessionState sessionState, long accumulatedMessageSize, bool hasSeenEoh)
		{
			ArgumentValidator.ThrowIfNull("sessionState", sessionState);
			SmtpResponse result = SmtpResponse.Empty;
			if (!sessionState.DiscardingMessage && !hasSeenEoh && !SmtpInSessionUtils.HasSMTPBypassMessageSizeLimitPermission(sessionState.CombinedPermissions) && sessionState.ReceiveConnector.MaxHeaderSize.ToBytes() < (ulong)accumulatedMessageSize)
			{
				result = SmtpResponse.HeadersTooLarge;
				sessionState.ProtocolLogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "AccumulatedMessageSize: {0} > MaxHeaderSize: {1}", new object[]
				{
					accumulatedMessageSize,
					sessionState.ReceiveConnector.MaxHeaderSize
				});
				sessionState.StartDiscardingMessage();
			}
			return result;
		}

		public static SmtpResponse ValidateMessageSize(SmtpInSessionState sessionState, long messageSizeLimit, long originalMessageSize, long accumulatedMessageSize, bool hasSeenEoh)
		{
			ArgumentValidator.ThrowIfNull("sessionState", sessionState);
			SmtpResponse result = SmtpResponse.Empty;
			if (sessionState.DiscardingMessage || (!hasSeenEoh && DataBdatHelpers.ShouldOnlyCheckMessageSizeAfterEoh(sessionState)))
			{
				return result;
			}
			if (DataBdatHelpers.MessageSizeExceeded(accumulatedMessageSize, originalMessageSize, messageSizeLimit, sessionState.CombinedPermissions))
			{
				result = SmtpResponse.MessageTooLarge;
				sessionState.ProtocolLogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "AccumulatedMessageSize: {0} > MessageSizeLimit: {1}", new object[]
				{
					accumulatedMessageSize,
					messageSizeLimit
				});
				sessionState.StartDiscardingMessage();
				if (sessionState.ReceiveConnectorStub.SmtpReceivePerfCounterInstance != null)
				{
					sessionState.ReceiveConnectorStub.SmtpReceivePerfCounterInstance.MessagesRefusedForSize.Increment();
				}
			}
			return result;
		}

		public static bool ShouldOnlyCheckMessageSizeAfterEoh(SmtpInSessionState sessionState)
		{
			ArgumentValidator.ThrowIfNull("sessionState", sessionState);
			return SmtpInSessionUtils.HasSMTPAcceptOrgHeadersPermission(sessionState.CombinedPermissions) || (Util.IsHubOrFrontEndRole(sessionState.Configuration.TransportConfiguration.ProcessTransportRole) && !SmtpInSessionUtils.IsAnonymous(sessionState.RemoteIdentity));
		}

		public static SmtpResponse ValidateMailFromAddress(SmtpInSessionState sessionState)
		{
			ArgumentValidator.ThrowIfNull("sessionState", sessionState);
			if (sessionState.TransportMailItem.From == RoutingAddress.NullReversePath)
			{
				return SmtpResponse.Empty;
			}
			SmtpResponse result;
			if (!SmtpInAccessChecker.VerifySenderOkForClient(sessionState, sessionState.Configuration.TransportConfiguration.GetAcceptedDomainTable(sessionState.TransportMailItem.ADRecipientCache.OrganizationId), sessionState.TransportMailItem.ADRecipientCache, Util.IsHubOrFrontEndRole(sessionState.Configuration.TransportConfiguration.ProcessTransportRole), sessionState.TransportMailItem.From, sessionState.RemoteWindowsIdentity, sessionState.Configuration.TransportConfiguration.FirstOrgAcceptedDomainTable.DefaultDomainName, true, out result))
			{
				sessionState.StartDiscardingMessage();
				return result;
			}
			if (SmtpInAccessChecker.HasZeroProhibitSendQuota(sessionState, sessionState.TransportMailItem.ADRecipientCache, sessionState.TransportMailItem.From, out result))
			{
				sessionState.StartDiscardingMessage();
				return result;
			}
			return SmtpResponse.Empty;
		}

		public static SmtpResponse ValidateFromAddressInHeader(SmtpInSessionState sessionState)
		{
			ArgumentValidator.ThrowIfNull("sessionState", sessionState);
			RoutingAddress routingAddress;
			Util.TryGetP2Sender(sessionState.TransportMailItem.RootPart.Headers, out routingAddress);
			if (!routingAddress.Equals(RoutingAddress.NullReversePath) && !routingAddress.Equals(RoutingAddress.Empty) && !routingAddress.Equals(sessionState.TransportMailItem.From))
			{
				SmtpResponse result;
				if (!SmtpInAccessChecker.VerifySenderOkForClient(sessionState, sessionState.Configuration.TransportConfiguration.GetAcceptedDomainTable(sessionState.TransportMailItem.ADRecipientCache.OrganizationId), sessionState.TransportMailItem.ADRecipientCache, Util.IsHubOrFrontEndRole(sessionState.Configuration.TransportConfiguration.ProcessTransportRole), routingAddress, sessionState.RemoteWindowsIdentity, sessionState.Configuration.TransportConfiguration.FirstOrgAcceptedDomainTable.DefaultDomainName, false, out result))
				{
					sessionState.StartDiscardingMessage();
					return result;
				}
				if (SmtpInAccessChecker.HasZeroProhibitSendQuota(sessionState, sessionState.TransportMailItem.ADRecipientCache, routingAddress, out result))
				{
					sessionState.StartDiscardingMessage();
					return result;
				}
			}
			return SmtpResponse.Empty;
		}

		public static bool SendOnBehalfOfChecksPass(SmtpInSessionState sessionState, out SmtpResponse failureResponse)
		{
			ArgumentValidator.ThrowIfNull("sessionState", sessionState);
			HeaderList headers = sessionState.TransportMailItem.RootPart.Headers;
			RoutingAddress senderAddress = Util.RetrieveRoutingAddress(headers, HeaderId.Sender);
			RoutingAddress routingAddress = Util.RetrieveRoutingAddress(headers, HeaderId.From);
			if (!SmtpInAccessChecker.VerifySendOnBehalfOfPermissionsInAD(sessionState, sessionState.TransportMailItem.ADRecipientCache, senderAddress, routingAddress, out failureResponse))
			{
				sessionState.StartDiscardingMessage();
				return false;
			}
			if (routingAddress.IsValid && routingAddress != RoutingAddress.NullReversePath && SmtpInAccessChecker.HasZeroProhibitSendQuota(sessionState, sessionState.TransportMailItem.ADRecipientCache, routingAddress, out failureResponse))
			{
				sessionState.StartDiscardingMessage();
				return false;
			}
			return true;
		}

		public static bool GetSenderSizeLimit(TransportMailItem transportMailItem, Unlimited<ByteQuantifiedSize> maxSendSize, out long limit)
		{
			ArgumentValidator.ThrowIfNull("transportMailItem", transportMailItem);
			limit = 0L;
			RoutingAddress outer;
			if (!Util.TryGetP2Sender(transportMailItem.RootPart.Headers, out outer))
			{
				return false;
			}
			ProxyAddress innermostAddress = Sender.GetInnermostAddress(outer);
			Result<TransportMiniRecipient> result = transportMailItem.ADRecipientCache.FindAndCacheRecipient(innermostAddress);
			if (result.Data == null)
			{
				return false;
			}
			Unlimited<ByteQuantifiedSize> unlimited = result.Data.MaxSendSize;
			if (unlimited.IsUnlimited)
			{
				unlimited = maxSendSize;
			}
			if (unlimited.IsUnlimited)
			{
				limit = long.MaxValue;
				return true;
			}
			limit = (long)unlimited.Value.ToBytes();
			return true;
		}

		public static SmtpResponse HandleFilterableException(Exception exception, SmtpInSessionState sessionStateToUse)
		{
			ArgumentValidator.ThrowIfNull("exception", exception);
			ArgumentValidator.ThrowIfInvalidValue<Exception>("exception", exception, (Exception e) => e is ExchangeDataException || e is IOException);
			ArgumentValidator.ThrowIfNull("sessionStateToUse", sessionStateToUse);
			SmtpResponse result;
			if (exception is ExchangeDataException)
			{
				result = SmtpResponse.InvalidContent;
				sessionStateToUse.ProtocolLogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "A parsing error has occurred: {0}", new object[]
				{
					exception.Message
				});
			}
			else
			{
				result = SmtpResponse.CTSParseError;
			}
			sessionStateToUse.Tracer.TraceDebug<Exception>((long)sessionStateToUse.GetHashCode(), "Handled parser exception: {0}", exception);
			sessionStateToUse.StartDiscardingMessage();
			return result;
		}

		public static bool IsFilterableException(Exception exception)
		{
			ArgumentValidator.ThrowIfNull("exception", exception);
			return exception is ExchangeDataException || exception is IOException;
		}

		public static ParseAndProcessResult<SmtpInStateMachineEvents> CreateResultFromResponse(SmtpResponse response, SmtpInStateMachineEvents stateMachineEvent)
		{
			return new ParseAndProcessResult<SmtpInStateMachineEvents>(ParsingStatus.Complete, response, stateMachineEvent, false);
		}

		private static ADOperationResult AttributeAuthenticatedClientMessageOrUsingHeaders(TransportMailItem transportMailItem, bool isAttributedClient)
		{
			ADOperationResult adoperationResult = isAttributedClient ? MultiTenantTransport.TryAttributeProxiedClientSubmission(transportMailItem) : MultiTenantTransport.TryAttributeMessageUsingHeaders(transportMailItem);
			if (!adoperationResult.Succeeded)
			{
				DataBdatHelpers.TraceAttributionError(transportMailItem, "attributing from " + (isAttributedClient ? "Authenticated Client Info" : "E14 Header"), adoperationResult);
			}
			return adoperationResult;
		}

		private static SmtpResponse DoFallbackAttribution(TransportMailItem transportMailItem, bool isProxy)
		{
			SmtpResponse empty = SmtpResponse.Empty;
			ADOperationResult adoperationResult = MultiTenantTransport.TryAttributeFromDomain(transportMailItem);
			if (!adoperationResult.Succeeded)
			{
				DataBdatHelpers.TraceAttributionError(transportMailItem, "attributing from domain", adoperationResult);
				switch (adoperationResult.ErrorCode)
				{
				case ADOperationErrorCode.RetryableError:
					if (!isProxy)
					{
						return SmtpResponse.HubAttributionTransientFailureInEOHFallback;
					}
					return SmtpResponse.ProxyAttributionTransientFailureInEOHFallback;
				case ADOperationErrorCode.PermanentError:
					transportMailItem.ExternalOrganizationId = MultiTenantTransport.SafeTenantId;
					break;
				}
			}
			return empty;
		}

		private static SmtpResponse CreateADRecipientCache(TransportMailItem transportMailItem, bool rejectUnscopedMessages, bool isProxy)
		{
			ADOperationResult adoperationResult = MultiTenantTransport.TryCreateADRecipientCache(transportMailItem);
			if (!adoperationResult.Succeeded)
			{
				DataBdatHelpers.TraceAttributionError(transportMailItem, "creating AD recipient cache", adoperationResult);
			}
			switch (adoperationResult.ErrorCode)
			{
			case ADOperationErrorCode.RetryableError:
				if (!isProxy)
				{
					return SmtpResponse.HubRecipientCacheCreationTransientFailureInEOH;
				}
				return SmtpResponse.ProxyRecipientCacheCreationTransientFailureInEOH;
			case ADOperationErrorCode.PermanentError:
				if (rejectUnscopedMessages)
				{
					if (!isProxy)
					{
						return SmtpResponse.HubRecipientCacheCreationFailureInEOH;
					}
					return SmtpResponse.ProxyRecipientCacheCreationFailureInEOH;
				}
				else
				{
					MultiTenantTransport.UpdateADRecipientCacheAndOrganizationScope(transportMailItem, OrganizationId.ForestWideOrgId);
				}
				break;
			}
			return SmtpResponse.Empty;
		}

		private static void TraceAttributionError(TransportMailItem transportMailItem, string errorMessage, ADOperationResult adOperationResult)
		{
			MultiTenantTransport.TraceAttributionError("Error {0} {1} for message {2}", new object[]
			{
				adOperationResult.Exception,
				errorMessage,
				MultiTenantTransport.ToString(transportMailItem)
			});
		}

		private static void UpdateMessageAuthenticationStatus(ISmtpInSession session, HeaderList headers, out bool isMuaSubmission)
		{
			SubmitAuthCategory submitAuthCategory = SubmitAuthCategory.Anonymous;
			string text = string.Empty;
			MultilevelAuthMechanism authMethod = session.TransportMailItem.AuthMethod;
			isMuaSubmission = SmtpInSessionUtils.IsMuaSubmission(session.Permissions, session.RemoteIdentity);
			session.TransportMailItem.ExtendedProperties.SetValue<bool>("Microsoft.Exchange.SmtpMuaSubmission", isMuaSubmission);
			if (string.IsNullOrEmpty(session.TransportMailItem.Auth) || !session.TransportMailItem.Auth.Equals((string)RoutingAddress.NullReversePath, StringComparison.OrdinalIgnoreCase))
			{
				session.TransportMailItem.Auth = string.Empty;
				text = session.RemoteIdentityName;
				if (SmtpInSessionUtils.IsPartner(session.RemoteIdentity))
				{
					submitAuthCategory = SubmitAuthCategory.Partner;
				}
				else if (SmtpInSessionUtils.IsAuthenticated(session.RemoteIdentity) && ((!SmtpInSessionUtils.HasSMTPAcceptAnySenderPermission(session.Permissions) && !SmtpInSessionUtils.HasSMTPAcceptAuthoritativeDomainSenderPermission(session.Permissions)) || SmtpInSessionUtils.HasSMTPAcceptAuthenticationFlag(session.Permissions)))
				{
					submitAuthCategory = SubmitAuthCategory.Internal;
				}
				if (submitAuthCategory == SubmitAuthCategory.Anonymous)
				{
					session.TransportMailItem.Auth = (string)RoutingAddress.NullReversePath;
					text = string.Empty;
				}
			}
			ExTraceGlobals.SmtpReceiveTracer.TraceDebug(0L, "Updating message authentication status category {0} mechanism {1} authDomain {2} authflag {3} remoteIdentityName {4} remoteCertificate {5}", new object[]
			{
				submitAuthCategory,
				authMethod,
				text,
				session.TransportMailItem.Auth,
				session.RemoteIdentityName,
				(session.TlsRemoteCertificate == null) ? "none" : session.TlsRemoteCertificate.Thumbprint
			});
			MultilevelAuth.EnsureSecurityAttributes(session.TransportMailItem, submitAuthCategory, authMethod, text, session.TlsRemoteCertificate, headers);
		}

		private static void UpdateMessageAuthenticationStatus(SmtpInSessionState sessionState, HeaderList headers, out bool isMuaSubmission)
		{
			SubmitAuthCategory submitAuthCategory = SubmitAuthCategory.Anonymous;
			string text = string.Empty;
			MultilevelAuthMechanism authMethod = sessionState.TransportMailItem.AuthMethod;
			isMuaSubmission = SmtpInSessionUtils.IsMuaSubmission(sessionState.CombinedPermissions, sessionState.RemoteIdentity);
			sessionState.TransportMailItem.ExtendedProperties.SetValue<bool>("Microsoft.Exchange.SmtpMuaSubmission", isMuaSubmission);
			if (string.IsNullOrEmpty(sessionState.TransportMailItem.Auth) || !sessionState.TransportMailItem.Auth.Equals((string)RoutingAddress.NullReversePath, StringComparison.OrdinalIgnoreCase))
			{
				sessionState.TransportMailItem.Auth = string.Empty;
				text = sessionState.RemoteIdentityName;
				if (SmtpInSessionUtils.IsPartner(sessionState.RemoteIdentity))
				{
					submitAuthCategory = SubmitAuthCategory.Partner;
				}
				else if (SmtpInSessionUtils.IsAuthenticated(sessionState.RemoteIdentity) && ((!SmtpInSessionUtils.HasSMTPAcceptAnySenderPermission(sessionState.CombinedPermissions) && !SmtpInSessionUtils.HasSMTPAcceptAuthoritativeDomainSenderPermission(sessionState.CombinedPermissions)) || SmtpInSessionUtils.HasSMTPAcceptAuthenticationFlag(sessionState.CombinedPermissions)))
				{
					submitAuthCategory = SubmitAuthCategory.Internal;
				}
				if (submitAuthCategory == SubmitAuthCategory.Anonymous)
				{
					sessionState.TransportMailItem.Auth = (string)RoutingAddress.NullReversePath;
					text = string.Empty;
				}
			}
			sessionState.Tracer.TraceDebug(0L, "Updating message authentication status category {0} mechanism {1} authDomain {2} authflag {3} remoteIdentityName {4} remoteCertificate {5}", new object[]
			{
				submitAuthCategory,
				authMethod,
				text,
				sessionState.TransportMailItem.Auth,
				sessionState.RemoteIdentityName,
				(sessionState.TlsRemoteCertificateInternal == null) ? "none" : sessionState.TlsRemoteCertificateInternal.Thumbprint
			});
			MultilevelAuth.EnsureSecurityAttributes(sessionState.TransportMailItem, submitAuthCategory, authMethod, text, (sessionState.TlsRemoteCertificateInternal == null) ? null : sessionState.TlsRemoteCertificateInternal.Certificate, headers);
		}

		private static bool IsHeaderTooLarge(long maxHeaderSize, long eohPosition, Permission permissions, IProtocolLogSession protocolLogSession)
		{
			if (SmtpInSessionUtils.HasSMTPBypassMessageSizeLimitPermission(permissions) || maxHeaderSize <= 0L || maxHeaderSize >= eohPosition)
			{
				return false;
			}
			protocolLogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "HeaderSize: {0} > MaxHeaderSize: {1}", new object[]
			{
				eohPosition,
				maxHeaderSize
			});
			return true;
		}

		private static bool IsPartialMessage(HeaderList headerList)
		{
			ContentTypeHeader contentTypeHeader = headerList.FindFirst(HeaderId.ContentType) as ContentTypeHeader;
			return contentTypeHeader != null && contentTypeHeader.Value != null && string.Equals(contentTypeHeader.Value, "message/partial", StringComparison.OrdinalIgnoreCase);
		}

		private const string ClientProxyHeaderName = "X-ClientProxiedBy";

		private const string ViaMailboxTransport = "Mailbox Transport";

		private const string MessagePartial = "message/partial";

		public static readonly string ViaFrontEndTransport = "Frontend Transport";
	}
}
