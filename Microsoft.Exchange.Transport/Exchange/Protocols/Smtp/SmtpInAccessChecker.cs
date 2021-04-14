using System;
using System.ComponentModel;
using System.IO;
using System.Security.Principal;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.Transport.Categorizer;
using Microsoft.Exchange.Transport.Configuration;
using Microsoft.Exchange.Transport.Logging;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal static class SmtpInAccessChecker
	{
		public static bool VerifySenderOkForClient(ISmtpInSession smtpInSession, PerTenantAcceptedDomainTable perTenantAcceptedDomainTable, IADRecipientCache<TransportMiniRecipient> recipientCache, bool isBridgehead, RoutingAddress senderAddress, WindowsIdentity remoteWindowsIdentity, string firstOrgDefaultDomainName, bool isMailFromSender, out SmtpResponse failureResponse)
		{
			ArgumentValidator.ThrowIfNull("smtpInSession", smtpInSession);
			ArgumentValidator.ThrowIfNull("perTenantAcceptedDomainTable", perTenantAcceptedDomainTable);
			ArgumentValidator.ThrowIfNull("recipientCache", recipientCache);
			bool sendAsRequiredADLookup;
			bool result = SmtpInAccessChecker.VerifySenderOkForClient(perTenantAcceptedDomainTable, recipientCache, isBridgehead, senderAddress, remoteWindowsIdentity, firstOrgDefaultDomainName, isMailFromSender, smtpInSession.Connector.Name, smtpInSession.RemoteIdentity, smtpInSession.RemoteIdentityName, smtpInSession.Permissions, smtpInSession.AuthzAuthorization, smtpInSession.ConnectorStub.SmtpAvailabilityPerfCounters, smtpInSession.Tracer, smtpInSession.GetHashCode(), out sendAsRequiredADLookup, out failureResponse);
			smtpInSession.SendAsRequiredADLookup = sendAsRequiredADLookup;
			return result;
		}

		public static bool VerifySendOnBehalfOfPermissionsInAD(ISmtpInSession smtpInSession, IADRecipientCache<TransportMiniRecipient> recipientCache, RoutingAddress senderAddress, RoutingAddress fromAddress, out SmtpResponse failureResponse)
		{
			ArgumentValidator.ThrowIfNull("smtpInSession", smtpInSession);
			ArgumentValidator.ThrowIfNull("recipientCache", recipientCache);
			return SmtpInAccessChecker.VerifySendOnBehalfOfPermissionsInAD(recipientCache, smtpInSession.TransportMailItem, senderAddress, fromAddress, smtpInSession.SendAsRequiredADLookup, smtpInSession.Connector.Name, smtpInSession.RemoteIdentityName, smtpInSession.ConnectorStub.SmtpAvailabilityPerfCounters, smtpInSession.LogSession, smtpInSession.Tracer, smtpInSession.GetHashCode(), out failureResponse);
		}

		public static bool HasZeroProhibitSendQuota(ISmtpInSession smtpInSession, IADRecipientCache<TransportMiniRecipient> recipientCache, RoutingAddress routingAddress, out SmtpResponse failureResponse)
		{
			ArgumentValidator.ThrowIfNull("smtpInSession", smtpInSession);
			ArgumentValidator.ThrowIfNull("recipientCache", recipientCache);
			return SmtpInAccessChecker.HasZeroProhibitSendQuota(recipientCache, routingAddress, smtpInSession.AuthUserRecipient, smtpInSession.Connector.Name, smtpInSession.RemoteIdentity, smtpInSession.RemoteIdentityName, smtpInSession.ConnectorStub.SmtpAvailabilityPerfCounters, smtpInSession.LogSession, smtpInSession.Tracer, smtpInSession.GetHashCode(), out failureResponse);
		}

		public static bool VerifySenderOkForClient(SmtpInSessionState sessionState, PerTenantAcceptedDomainTable perTenantAcceptedDomainTable, IADRecipientCache<TransportMiniRecipient> recipientCache, bool isBridgehead, RoutingAddress senderAddress, WindowsIdentity remoteWindowsIdentity, string firstOrgDefaultDomainName, bool isMailFromSender, out SmtpResponse failureResponse)
		{
			ArgumentValidator.ThrowIfNull("sessionState", sessionState);
			ArgumentValidator.ThrowIfNull("perTenantAcceptedDomainTable", perTenantAcceptedDomainTable);
			ArgumentValidator.ThrowIfNull("recipientCache", recipientCache);
			bool sendAsRequiredADLookup;
			bool result = SmtpInAccessChecker.VerifySenderOkForClient(perTenantAcceptedDomainTable, recipientCache, isBridgehead, senderAddress, remoteWindowsIdentity, firstOrgDefaultDomainName, isMailFromSender, sessionState.ReceiveConnector.Name, sessionState.RemoteIdentity, sessionState.RemoteIdentityName, sessionState.CombinedPermissions, sessionState.AuthzAuthorization, sessionState.ReceiveConnectorStub.SmtpAvailabilityPerfCounters, sessionState.Tracer, sessionState.GetHashCode(), out sendAsRequiredADLookup, out failureResponse);
			sessionState.SendAsRequiredADLookup = sendAsRequiredADLookup;
			return result;
		}

		public static bool VerifySendOnBehalfOfPermissionsInAD(SmtpInSessionState sessionState, IADRecipientCache<TransportMiniRecipient> recipientCache, RoutingAddress senderAddress, RoutingAddress fromAddress, out SmtpResponse failureResponse)
		{
			ArgumentValidator.ThrowIfNull("sessionState", sessionState);
			ArgumentValidator.ThrowIfNull("recipientCache", recipientCache);
			return SmtpInAccessChecker.VerifySendOnBehalfOfPermissionsInAD(recipientCache, sessionState.TransportMailItem, senderAddress, fromAddress, sessionState.SendAsRequiredADLookup, sessionState.ReceiveConnector.Name, sessionState.RemoteIdentityName, sessionState.ReceiveConnectorStub.SmtpAvailabilityPerfCounters, sessionState.ProtocolLogSession, sessionState.Tracer, sessionState.GetHashCode(), out failureResponse);
		}

		public static bool HasZeroProhibitSendQuota(SmtpInSessionState sessionState, IADRecipientCache<TransportMiniRecipient> recipientCache, RoutingAddress routingAddress, out SmtpResponse failureResponse)
		{
			ArgumentValidator.ThrowIfNull("sessionState", sessionState);
			ArgumentValidator.ThrowIfNull("recipientCache", recipientCache);
			return SmtpInAccessChecker.HasZeroProhibitSendQuota(recipientCache, routingAddress, sessionState.AuthenticatedUser, sessionState.ReceiveConnector.Name, sessionState.RemoteIdentity, sessionState.RemoteIdentityName, sessionState.ReceiveConnectorStub.SmtpAvailabilityPerfCounters, sessionState.ProtocolLogSession, sessionState.Tracer, sessionState.GetHashCode(), out failureResponse);
		}

		public static bool HasZeroProhibitSendQuota(ADRawEntry rawEntry, string identity, string remoteIdentityName, string receiveConnectorName, ITracer tracer, int traceId)
		{
			ArgumentValidator.ThrowIfNull("tracer", tracer);
			if (rawEntry == null)
			{
				tracer.TraceDebug<string, string, string>((long)traceId, "Address '{0}' for authenticated user '{1}' on connector '{2}' did not resolve.", identity, remoteIdentityName, receiveConnectorName);
				return false;
			}
			object obj = rawEntry[ADMailboxRecipientSchema.ProhibitSendQuota];
			if (obj == null)
			{
				tracer.TraceDebug<string>((long)traceId, "{0} does not have a ProhibitSendQuota", identity);
				return false;
			}
			Unlimited<ByteQuantifiedSize> unlimited = (Unlimited<ByteQuantifiedSize>)obj;
			if (unlimited.IsUnlimited || unlimited.Value.ToBytes() != 0UL)
			{
				tracer.TraceDebug<string>((long)traceId, "ProhibitSendQuota for {0} is nonzero", identity);
				return false;
			}
			tracer.TraceError<string>((long)traceId, "ProhibitSendQuota for {0} is zero", identity);
			return true;
		}

		private static bool VerifySenderOkForClient(PerTenantAcceptedDomainTable perTenantAcceptedDomainTable, IADRecipientCache<TransportMiniRecipient> recipientCache, bool isBridgehead, RoutingAddress senderAddress, WindowsIdentity remoteWindowsIdentity, string firstOrgDefaultDomainName, bool isMailFromSender, string connectorName, SecurityIdentifier remoteIdentity, string remoteIdentityName, Permission permissions, IAuthzAuthorization authzAuthorization, ISmtpAvailabilityPerfCounters availabilityPerfCounters, ITracer tracer, int traceId, out bool sendAsRequiredADLookup, out SmtpResponse failureResponse)
		{
			failureResponse = SmtpResponse.NoopOk;
			sendAsRequiredADLookup = false;
			if (!senderAddress.IsValid)
			{
				tracer.TraceDebug<string, string, string>((long)traceId, "SMTP rejected a mail From '{0}' on '{1}' connector with authenticated user '{2}'. The Sender address is invalid.", senderAddress.ToString(), connectorName, remoteIdentityName);
				failureResponse = SmtpResponse.SendAsDenied;
				return false;
			}
			bool isSenderFromInternalDomain = false;
			SmtpDomain senderDomain;
			if (SmtpDomain.TryParse(senderAddress.DomainPart, out senderDomain))
			{
				ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
				{
					isSenderFromInternalDomain = perTenantAcceptedDomainTable.AcceptedDomainTable.CheckInternal(senderDomain);
				}, 0);
				if (adoperationResult.Exception != null)
				{
					SmtpInAccessChecker.LogADExceptionForSendAs(adoperationResult.Exception, senderAddress, connectorName, remoteIdentityName, availabilityPerfCounters, tracer, traceId, isMailFromSender);
					failureResponse = SmtpResponse.AuthTempFailure;
					return false;
				}
			}
			if (SmtpInSessionUtils.HasSMTPAcceptAnySenderPermission(permissions) && !isSenderFromInternalDomain)
			{
				return true;
			}
			if (SmtpInSessionUtils.HasSMTPAcceptAuthoritativeDomainSenderPermission(permissions) && isSenderFromInternalDomain)
			{
				return true;
			}
			SmtpDomain rhs;
			if (SmtpInSessionUtils.IsPartner(remoteIdentity) && !string.IsNullOrEmpty(remoteIdentityName) && SmtpDomain.TryParse(remoteIdentityName, out rhs) && senderDomain.Equals(rhs))
			{
				return true;
			}
			if (isBridgehead && isSenderFromInternalDomain)
			{
				sendAsRequiredADLookup = true;
				return SmtpInAccessChecker.VerifySendAsPermissionsInAD(recipientCache, senderAddress, remoteWindowsIdentity, isMailFromSender, firstOrgDefaultDomainName, connectorName, remoteIdentity, remoteIdentityName, authzAuthorization, availabilityPerfCounters, tracer, traceId, out failureResponse);
			}
			tracer.TraceDebug<string, string, string>((long)traceId, "SMTP rejected a mail From '{0}' on '{1}' connector with authenticated user '{2}'. No sufficient permissions.", senderAddress.ToString(), connectorName, remoteIdentityName);
			if (SmtpInSessionUtils.IsAuthenticated(remoteIdentity))
			{
				SmtpInAccessChecker.eventLogger.LogEvent(TransportEventLogConstants.Tuple_SmtpReceiveSendAsDenied, null, new object[]
				{
					senderAddress.ToString(),
					connectorName,
					remoteIdentityName,
					isMailFromSender ? "P1" : "P2"
				});
				failureResponse = SmtpResponse.SendAsDenied;
			}
			else
			{
				failureResponse = SmtpResponse.AnonymousSendAsDenied;
			}
			return false;
		}

		private static bool VerifySendOnBehalfOfPermissionsInAD(IADRecipientCache<TransportMiniRecipient> recipientCache, TransportMailItem transportMailItem, RoutingAddress senderAddress, RoutingAddress fromAddress, bool sendAsRequiredADLookup, string connectorName, string remoteIdentityName, ISmtpAvailabilityPerfCounters availabilityPerfCounters, IProtocolLogSession protocolLogSession, ITracer tracer, int traceId, out SmtpResponse failureResponse)
		{
			failureResponse = SmtpResponse.NoopOk;
			if (!sendAsRequiredADLookup)
			{
				return true;
			}
			if (senderAddress == RoutingAddress.Empty || fromAddress == RoutingAddress.Empty || !fromAddress.IsValid || !senderAddress.IsValid || senderAddress == fromAddress)
			{
				return true;
			}
			try
			{
				if (transportMailItem.Message.MapiMessageClass == "IPM.Schedule.Meeting.Request")
				{
					return true;
				}
			}
			catch (ExchangeDataException ex)
			{
				tracer.TraceDebug<string>((long)traceId, "Send On Behalf Of checks are rejecting message due to invalid content. {0}", ex.Message);
				failureResponse = SmtpResponse.InvalidContent;
				protocolLogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "Send On Behalf Of checks are rejecting message due to invalid content. {0}", new object[]
				{
					ex.Message
				});
				return false;
			}
			catch (IOException ex2)
			{
				tracer.TraceDebug<string>((long)traceId, "Send On Behalf Of checks are rejecting message due to IO error. {0}", ex2.Message);
				if (availabilityPerfCounters != null)
				{
					availabilityPerfCounters.UpdatePerformanceCounters(LegitimateSmtpAvailabilityCategory.RejectDueToIOException);
				}
				if (ExceptionHelper.IsHandleableTransientCtsException(ex2))
				{
					failureResponse = SmtpResponse.CTSParseError;
					return false;
				}
				throw;
			}
			ProxyAddress senderProxyAddress = Sender.GetInnermostAddress(senderAddress);
			ProxyAddress fromProxyAddress = Sender.GetInnermostAddress(fromAddress);
			Result<TransportMiniRecipient> senderADRawEntry = default(Result<TransportMiniRecipient>);
			Result<TransportMiniRecipient> fromADRawEntry = default(Result<TransportMiniRecipient>);
			ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				senderADRawEntry = recipientCache.FindAndCacheRecipient(senderProxyAddress);
				fromADRawEntry = recipientCache.ReloadRecipient(fromProxyAddress, SmtpInAccessChecker.extraPropertiesWithGrantSendOnBehalfTo);
			}, 3);
			if (adoperationResult.Exception != null)
			{
				tracer.TraceDebug((long)traceId, "SMTP rejected a mail From '{0}', Sender '{1}' on '{2}' connector with authenticated user '{3}'. AD lookup threw exception: {4}", new object[]
				{
					fromAddress.ToString(),
					senderAddress.ToString(),
					connectorName,
					remoteIdentityName,
					adoperationResult.Exception.Message
				});
				if (availabilityPerfCounters != null)
				{
					availabilityPerfCounters.UpdatePerformanceCounters(LegitimateSmtpAvailabilityCategory.RejectDueToADDown);
				}
				SmtpInAccessChecker.eventLogger.LogEvent(TransportEventLogConstants.Tuple_SmtpReceiveSendOnBehalfOfDeniedTempAuthFailure, null, new object[]
				{
					fromAddress.ToString(),
					connectorName,
					remoteIdentityName,
					senderAddress.ToString(),
					adoperationResult.Exception.Message
				});
				failureResponse = SmtpResponse.AuthTempFailure;
				return false;
			}
			if (senderADRawEntry.Data == null || senderADRawEntry.Error != null)
			{
				throw new InvalidOperationException("Sender should have been always valid here");
			}
			if (senderADRawEntry.Data == null || senderADRawEntry.Error != null)
			{
				failureResponse = SmtpResponse.AuthTempFailure;
				return false;
			}
			if (fromADRawEntry.Data == null)
			{
				return true;
			}
			if (fromADRawEntry.Error != null)
			{
				tracer.TraceDebug((long)traceId, "SMTP rejected a mail From '{0}', Sender '{1}' on '{2}' connector with authenticated user '{3}'. The from address lookup returned error: {4}", new object[]
				{
					fromAddress.ToString(),
					senderAddress.ToString(),
					connectorName,
					remoteIdentityName,
					fromADRawEntry.Error.ToString()
				});
				SmtpInAccessChecker.eventLogger.LogEvent(TransportEventLogConstants.Tuple_SmtpReceiveSendOnBehalfOfDeniedFromAddressDataInvalid, null, new object[]
				{
					fromAddress.ToString(),
					connectorName,
					remoteIdentityName,
					senderAddress.ToString(),
					fromADRawEntry.Error.ToString()
				});
				failureResponse = SmtpResponse.SendOnBehalfOfDenied;
				return false;
			}
			MultiValuedProperty<ADObjectId> grantSendOnBehalfTo = fromADRawEntry.Data.GrantSendOnBehalfTo;
			if (grantSendOnBehalfTo != null && grantSendOnBehalfTo.Count > 0)
			{
				foreach (ADObjectId adobjectId in grantSendOnBehalfTo)
				{
					if (adobjectId.Equals(senderADRawEntry.Data.Id))
					{
						return true;
					}
				}
				foreach (ADObjectId groupId in grantSendOnBehalfTo)
				{
					if (ADRecipient.IsMemberOf(senderADRawEntry.Data.Id, groupId, false, recipientCache.ADSession))
					{
						return true;
					}
				}
			}
			tracer.TraceError((long)traceId, "SMTP rejected a mail From '{0}', Sender '{1}' on '{2}' connector with authenticated user '{3}'. The sender does not have send on behalf of permissions", new object[]
			{
				fromAddress.ToString(),
				senderAddress.ToString(),
				connectorName,
				remoteIdentityName
			});
			SmtpInAccessChecker.eventLogger.LogEvent(TransportEventLogConstants.Tuple_SmtpReceiveSendOnBehalfOfDenied, null, new object[]
			{
				fromAddress.ToString(),
				connectorName,
				remoteIdentityName,
				senderAddress.ToString()
			});
			failureResponse = SmtpResponse.SendOnBehalfOfDenied;
			return false;
		}

		private static bool HasZeroProhibitSendQuota(IADRecipientCache<TransportMiniRecipient> recipientCache, RoutingAddress routingAddress, TransportMiniRecipient authenticatedUser, string connectorName, SecurityIdentifier remoteIdentity, string remoteIdentityName, ISmtpAvailabilityPerfCounters availabilityPerfCounters, IProtocolLogSession protocolLogSession, ITracer tracer, int traceId, out SmtpResponse failureResponse)
		{
			ArgumentValidator.ThrowIfNull("recipientCache", recipientCache);
			ArgumentValidator.ThrowIfNull("protocolLogSession", protocolLogSession);
			ArgumentValidator.ThrowIfNull("tracer", tracer);
			if (authenticatedUser == null)
			{
				tracer.TraceDebug<SecurityIdentifier>(0L, "Bypassed VerifyNonzeroProhibitSendQuota check for remote identity {0} because this is not an authenticated client submission", remoteIdentity);
				failureResponse = SmtpResponse.Empty;
				return false;
			}
			if (!routingAddress.IsValid)
			{
				tracer.TraceDebug<string, string, string>((long)traceId, "SMTP rejected a mail From '{0}' on '{1}' connector with authenticated user '{2}'. VerifyNonzeroProhibitSendQuota cannot proceed because the Sender address is invalid.", routingAddress.ToString(), connectorName, remoteIdentityName);
				failureResponse = SmtpResponse.InvalidSenderAddress;
				return true;
			}
			string text = routingAddress.ToString();
			if (string.IsNullOrEmpty(text))
			{
				tracer.TraceDebug<string, string, string>((long)traceId, "Address '{0}' for authenticated user '{1}' on connector '{2}' is empty, skipping ProhibitSendQuota check.", text, remoteIdentityName, connectorName);
				failureResponse = SmtpResponse.Empty;
				return false;
			}
			if (routingAddress == RoutingAddress.NullReversePath)
			{
				tracer.TraceDebug<string, string, string>((long)traceId, "Address '{0}' for authenticated user '{1}' on connector '{2}' is null reverse path, skipping ProhibitSendQuota check.", text, remoteIdentityName, connectorName);
				failureResponse = SmtpResponse.Empty;
				return false;
			}
			TransportMiniRecipient rawEntry;
			if (!SmtpInAccessChecker.TryResolve(recipientCache, routingAddress, connectorName, remoteIdentityName, availabilityPerfCounters, tracer, traceId, out rawEntry, out failureResponse))
			{
				return true;
			}
			if (SmtpInAccessChecker.HasZeroProhibitSendQuota(rawEntry, text, remoteIdentityName, connectorName, tracer, traceId))
			{
				string text2 = string.Format("Refusing submission from '{0}' because ProhibitSendQuota is zero.", text);
				tracer.TraceError((long)traceId, text2);
				protocolLogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, text2);
				failureResponse = SmtpResponse.SubmissionDisabledBySendQuota;
				return true;
			}
			failureResponse = SmtpResponse.Empty;
			return false;
		}

		private static bool TryResolve(IADRecipientCache<TransportMiniRecipient> recipientCache, RoutingAddress address, string connectorName, string remoteIdentityName, ISmtpAvailabilityPerfCounters availabilityPerfCounters, ITracer tracer, int traceId, out TransportMiniRecipient miniRecipient, out SmtpResponse failureResponse)
		{
			failureResponse = SmtpResponse.NoopOk;
			if (!address.IsValid)
			{
				tracer.TraceError<string, string, string>((long)traceId, "SMTP rejected a message from '{0}', on '{1}' connector with authenticated user '{2}'. The address is not valid.", address.ToString(), connectorName, remoteIdentityName);
				miniRecipient = null;
				failureResponse = SmtpResponse.InvalidSenderAddress;
				return false;
			}
			ProxyAddress proxyAddress = Sender.GetInnermostAddress(address);
			if (proxyAddress == null)
			{
				tracer.TraceError<string, string, string>((long)traceId, "SMTP rejected a message from '{0}', on '{1}' connector with authenticated user '{2}'. The address is not valid.", address.ToString(), connectorName, remoteIdentityName);
				miniRecipient = null;
				failureResponse = SmtpResponse.InvalidSenderAddress;
				return false;
			}
			TransportMiniRecipient refParametersCannotBeUsedInAnonymousMethods = null;
			Result<TransportMiniRecipient> result;
			ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				result = recipientCache.FindAndCacheRecipient(proxyAddress);
				refParametersCannotBeUsedInAnonymousMethods = result.Data;
			}, 3);
			if (adoperationResult.Exception != null)
			{
				tracer.TraceError((long)traceId, "SMTP rejected a message from '{0}', on '{1}' connector with authenticated user '{2}'. AD lookup threw exception: {3}", new object[]
				{
					address.ToString(),
					connectorName,
					remoteIdentityName,
					adoperationResult.Exception.Message
				});
				if (availabilityPerfCounters != null)
				{
					availabilityPerfCounters.UpdatePerformanceCounters(LegitimateSmtpAvailabilityCategory.RejectDueToADDown);
				}
				SmtpInAccessChecker.eventLogger.LogEvent(TransportEventLogConstants.Tuple_SmtpReceiveProhibitSendQuotaDeniedTempAuthFailure, null, new object[]
				{
					address.ToString(),
					connectorName,
					remoteIdentityName,
					adoperationResult.Exception.Message
				});
				failureResponse = SmtpResponse.AuthorizationTempFailure;
				miniRecipient = null;
				return false;
			}
			miniRecipient = refParametersCannotBeUsedInAnonymousMethods;
			return true;
		}

		private static bool VerifySendAsPermissionsInAD(IADRecipientCache<TransportMiniRecipient> recipientCache, RoutingAddress senderAddress, WindowsIdentity remoteWindowsIdentity, bool isMailFromSender, string firstOrgDefaultDomainName, string connectorName, SecurityIdentifier remoteIdentity, string remoteIdentityName, IAuthzAuthorization authzAuthorization, ISmtpAvailabilityPerfCounters availabilityPerfCounters, ITracer tracer, int traceId, out SmtpResponse failureResponse)
		{
			ProxyAddress senderProxyAddress = Sender.GetInnermostAddress(senderAddress, firstOrgDefaultDomainName);
			failureResponse = SmtpResponse.NoopOk;
			Result<TransportMiniRecipient> senderEntry = default(Result<TransportMiniRecipient>);
			ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				senderEntry = recipientCache.ReadSecurityDescriptor(senderProxyAddress);
			}, 3);
			if (adoperationResult.Exception != null)
			{
				SmtpInAccessChecker.LogADExceptionForSendAs(adoperationResult.Exception, senderAddress, connectorName, remoteIdentityName, availabilityPerfCounters, tracer, traceId, isMailFromSender);
				failureResponse = SmtpResponse.AuthTempFailure;
				return false;
			}
			if (senderEntry.Data == null || senderEntry.Error is ObjectValidationError || senderEntry.Data[ADObjectSchema.NTSecurityDescriptor] == null)
			{
				string text = (senderEntry.Error != null) ? senderEntry.Error.ToString() : "No Object Found";
				tracer.TraceDebug((long)traceId, "SMTP rejected a mail From '{0}' on '{1}' connector with authenticated user '{2}'. The sender address lookup failed. {3}", new object[]
				{
					senderAddress.ToString(),
					connectorName,
					remoteIdentityName,
					text
				});
				SmtpInAccessChecker.eventLogger.LogEvent(TransportEventLogConstants.Tuple_SmtpReceiveSendAsDeniedSenderAddressDataInvalid, senderAddress.ToString(), new object[]
				{
					senderAddress.ToString(),
					connectorName,
					remoteIdentityName,
					isMailFromSender ? "P1" : "P2",
					text
				});
				failureResponse = SmtpResponse.SendAsDenied;
				return false;
			}
			SecurityIdentifier sid = senderEntry.Data.Sid;
			SecurityDescriptor securityDescriptor = (SecurityDescriptor)senderEntry.Data[ADObjectSchema.NTSecurityDescriptor];
			if (!isMailFromSender)
			{
				recipientCache.DropSecurityDescriptor(senderProxyAddress);
			}
			bool flag = false;
			IntPtr intPtr = IntPtr.Zero;
			bool flag2 = false;
			try
			{
				if (remoteWindowsIdentity != null)
				{
					intPtr = remoteWindowsIdentity.Token;
				}
			}
			catch (SystemException ex)
			{
				tracer.TraceDebug<string>((long)traceId, "Failed to get the token handle from remoteWindowsIdentity: {0}", ex.Message);
				intPtr = IntPtr.Zero;
			}
			if (intPtr != IntPtr.Zero)
			{
				try
				{
					flag = authzAuthorization.CheckSinglePermission(intPtr, securityDescriptor, Permission.SendAs, sid);
					goto IL_4B7;
				}
				catch (Win32Exception ex2)
				{
					tracer.TraceDebug((long)traceId, "SMTP rejected a mail From '{0}' on '{1}' connector with authenticated user '{2}'. CheckSinglePermission threw exception. {3}", new object[]
					{
						senderAddress.ToString(),
						connectorName,
						remoteIdentityName,
						ex2.Message
					});
					SmtpInAccessChecker.eventLogger.LogEvent(TransportEventLogConstants.Tuple_SmtpReceiveSendAsDeniedTempAuthFailure, null, new object[]
					{
						senderAddress.ToString(),
						connectorName,
						remoteIdentityName,
						isMailFromSender ? "P1" : "P2",
						ex2.Message
					});
					failureResponse = SmtpResponse.AuthTempFailure;
					return false;
				}
			}
			try
			{
				flag = authzAuthorization.CheckSinglePermission(remoteIdentity, false, securityDescriptor, Permission.SendAs, sid);
			}
			catch (Win32Exception ex3)
			{
				if (ex3.NativeErrorCode != 5)
				{
					tracer.TraceDebug((long)traceId, "SMTP rejected a mail From '{0}' on '{1}' connector with authenticated user '{2}'. CheckSinglePermission threw exception. {3}", new object[]
					{
						senderAddress.ToString(),
						connectorName,
						remoteIdentityName,
						ex3.Message
					});
					SmtpInAccessChecker.eventLogger.LogEvent(TransportEventLogConstants.Tuple_SmtpReceiveSendAsDeniedTempAuthFailure, null, new object[]
					{
						senderAddress.ToString(),
						connectorName,
						remoteIdentityName,
						isMailFromSender ? "P1" : "P2",
						ex3.Message
					});
					failureResponse = SmtpResponse.AuthTempFailure;
					return false;
				}
				tracer.TraceDebug((long)traceId, "Client '{0}' connected to connector '{1}' with authenticated user '{2}'. CheckSinglePermission with option 'token group expansion' failed. {3}", new object[]
				{
					senderAddress.ToString(),
					connectorName,
					remoteIdentityName,
					ex3.Message
				});
				flag2 = true;
			}
			if (flag2)
			{
				try
				{
					flag = authzAuthorization.CheckSinglePermission(remoteIdentity, true, securityDescriptor, Permission.SendAs, sid);
				}
				catch (Win32Exception ex4)
				{
					tracer.TraceDebug((long)traceId, "SMTP rejected a mail From '{0}' on '{1}' connector with authenticated user '{2}'. CheckSinglePermission threw exception. {3}", new object[]
					{
						senderAddress.ToString(),
						connectorName,
						remoteIdentityName,
						ex4.Message
					});
					SmtpInAccessChecker.eventLogger.LogEvent(TransportEventLogConstants.Tuple_SmtpReceiveSendAsDeniedTempAuthFailure, null, new object[]
					{
						senderAddress.ToString(),
						connectorName,
						remoteIdentityName,
						isMailFromSender ? "P1" : "P2",
						ex4.Message
					});
					failureResponse = SmtpResponse.AuthTempFailure;
					return false;
				}
			}
			IL_4B7:
			if (!flag)
			{
				tracer.TraceDebug<string, string, string>((long)traceId, "SMTP rejected a mail From '{0}' on '{1}' connector with authenticated user '{2}'. No SendAs permissions", senderAddress.ToString(), connectorName, remoteIdentityName);
				SmtpInAccessChecker.eventLogger.LogEvent(TransportEventLogConstants.Tuple_SmtpReceiveSendAsDenied, null, new object[]
				{
					senderAddress.ToString(),
					connectorName,
					remoteIdentityName,
					isMailFromSender ? "P1" : "P2"
				});
				failureResponse = SmtpResponse.SendAsDenied;
			}
			return flag;
		}

		private static void LogADExceptionForSendAs(Exception exception, RoutingAddress senderAddress, string connectorName, string remoteIdentityName, ISmtpAvailabilityPerfCounters availabilityPerfCounters, ITracer tracer, int traceId, bool isMailFromSender)
		{
			tracer.TraceDebug((long)traceId, "SMTP rejected a mail From '{0}' on '{1}' connector with authenticated user '{2}'. The sender address lookup threw AD exception. {3}", new object[]
			{
				senderAddress.ToString(),
				connectorName,
				remoteIdentityName,
				exception.Message
			});
			if (availabilityPerfCounters != null)
			{
				availabilityPerfCounters.UpdatePerformanceCounters(LegitimateSmtpAvailabilityCategory.RejectDueToADDown);
			}
			SmtpInAccessChecker.eventLogger.LogEvent(TransportEventLogConstants.Tuple_SmtpReceiveSendAsDeniedTempAuthFailure, null, new object[]
			{
				senderAddress.ToString(),
				connectorName,
				remoteIdentityName,
				isMailFromSender ? "P1" : "P2",
				exception.Message
			});
		}

		private const string P1Sender = "P1";

		private const string P2Sender = "P2";

		private const uint ErrorAccessDenied = 5U;

		private static readonly ADPropertyDefinition[] extraPropertiesWithGrantSendOnBehalfTo = new ADPropertyDefinition[]
		{
			ADRecipientSchema.GrantSendOnBehalfTo
		};

		private static readonly ExEventLog eventLogger = new ExEventLog(ExTraceGlobals.SmtpReceiveTracer.Category, TransportEventLog.GetEventSource());
	}
}
