using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Security.AccessControl;
using System.Security.Principal;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Internal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.ProcessManager;
using Microsoft.Exchange.SecureMail;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.Transport.Logging;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal static class SmtpInSessionUtils
	{
		public static bool IsAnonymous(SecurityIdentifier identity)
		{
			ArgumentValidator.ThrowIfNull("identity", identity);
			return identity.IsWellKnown(WellKnownSidType.AnonymousSid);
		}

		public static bool IsAuthenticated(SecurityIdentifier identity)
		{
			ArgumentValidator.ThrowIfNull("identity", identity);
			return !SmtpInSessionUtils.IsAnonymous(identity);
		}

		public static bool IsExternalAuthoritative(SecurityIdentifier identity)
		{
			ArgumentValidator.ThrowIfNull("identity", identity);
			return identity == WellKnownSids.ExternallySecuredServers;
		}

		public static bool IsPartner(SecurityIdentifier identity)
		{
			ArgumentValidator.ThrowIfNull("identity", identity);
			return identity == WellKnownSids.PartnerServers;
		}

		public static bool IsShadowedBySender(string senderShadowContext)
		{
			return !string.IsNullOrEmpty(senderShadowContext);
		}

		public static bool IsPeerShadowSession(string peerSessionPrimaryServer)
		{
			return !string.IsNullOrEmpty(peerSessionPrimaryServer);
		}

		public static bool IsAnonymousClientProxiedSession(ISmtpInSession session)
		{
			ArgumentValidator.ThrowIfNull("session", session);
			return session.ProxiedClientAddress != null && session.InboundClientProxyState == InboundClientProxyStates.None;
		}

		public static bool HasSMTPAcceptXMessageContextADRecipientCachePermission(Permission permissions)
		{
			return (permissions & Permission.SMTPAcceptXMessageContextADRecipientCache) != Permission.None;
		}

		public static bool HasSMTPAcceptXMessageContextExtendedPropertiesPermission(Permission permissions)
		{
			return (permissions & Permission.SMTPAcceptXMessageContextExtendedProperties) != Permission.None;
		}

		public static bool HasSMTPAcceptXMessageContextFastIndexPermission(Permission permissions)
		{
			return (permissions & Permission.SMTPAcceptXMessageContextFastIndex) != Permission.None;
		}

		public static bool HasSMTPAcceptAnySenderPermission(Permission permissions)
		{
			return (permissions & Permission.SMTPAcceptAnySender) != Permission.None;
		}

		public static bool HasSMTPAntiSpamBypassPermission(Permission permissions)
		{
			return (permissions & Permission.BypassAntiSpam) != Permission.None;
		}

		public static bool HasSMTPBypassMessageSizeLimitPermission(Permission permissions)
		{
			return (permissions & Permission.BypassMessageSizeLimit) != Permission.None;
		}

		public static bool HasSMTPAcceptAuthoritativeDomainSenderPermission(Permission permissions)
		{
			return (permissions & Permission.SMTPAcceptAuthoritativeDomainSender) != Permission.None;
		}

		public static bool HasSMTPSubmitPermission(Permission permissions)
		{
			return (permissions & Permission.SMTPSubmit) != Permission.None;
		}

		public static bool HasSMTPAcceptEXCH50Permission(Permission permissions)
		{
			return (permissions & Permission.SMTPAcceptEXCH50) != Permission.None;
		}

		public static bool HasSMTPAcceptXShadowPermission(Permission permissions)
		{
			return (permissions & Permission.SMTPAcceptXShadow) != Permission.None;
		}

		public static bool HasSMTPAcceptXProxyFromPermission(Permission permissions)
		{
			return (permissions & Permission.SMTPAcceptXProxyFrom) != Permission.None;
		}

		public static bool HasSMTPAcceptXSessionParamsPermission(Permission permissions)
		{
			return (permissions & Permission.SMTPAcceptXSessionParams) != Permission.None;
		}

		public static bool HasSMTPAcceptAnyRecipientPermission(Permission permissions)
		{
			return (permissions & Permission.SMTPAcceptAnyRecipient) != Permission.None;
		}

		public static bool HasSMTPAcceptAuthenticationFlag(Permission permissions)
		{
			return (permissions & Permission.SMTPAcceptAuthenticationFlag) != Permission.None;
		}

		public static bool HasSMTPAcceptOrgHeadersPermission(Permission permissions)
		{
			return (permissions & Permission.AcceptOrganizationHeaders) != Permission.None;
		}

		public static bool HasSMTPAcceptForestHeadersPermission(Permission permissions)
		{
			return (permissions & Permission.AcceptForestHeaders) != Permission.None;
		}

		public static bool HasSendForestHeadersPermission(Permission permissions)
		{
			return (permissions & Permission.SendForestHeaders) != Permission.None;
		}

		public static bool HasSMTPAcceptRoutingHeadersPermission(Permission permissions)
		{
			return (permissions & Permission.AcceptRoutingHeaders) != Permission.None;
		}

		public static bool HasSMTPAcceptOrarPermission(Permission permissions)
		{
			return SmtpInSessionUtils.HasSMTPAcceptOrgHeadersPermission(permissions);
		}

		public static bool HasSMTPAcceptRDstPermission(Permission permissions)
		{
			return SmtpInSessionUtils.HasSMTPAcceptOrgHeadersPermission(permissions);
		}

		public static bool HasSMTPAcceptXAttrPermission(Permission permissions)
		{
			return (permissions & Permission.SMTPAcceptXAttr) != Permission.None;
		}

		public static bool HasSMTPAcceptXSysProbePermission(Permission permissions)
		{
			return (permissions & Permission.SMTPAcceptXSysProbe) != Permission.None;
		}

		public static bool HasSMTPAcceptXOrigFromPermission(Permission permissions)
		{
			return SmtpInSessionUtils.HasSMTPAcceptOrgHeadersPermission(permissions);
		}

		public static bool HasSMTPAcceptXProxyPermission(SecurityIdentifier remoteIdentity, MultilevelAuthMechanism authMechanism)
		{
			ArgumentValidator.ThrowIfNull("remoteIdentity", remoteIdentity);
			return SmtpInSessionUtils.IsAuthenticated(remoteIdentity) && authMechanism == MultilevelAuthMechanism.MUTUALGSSAPI;
		}

		public static bool HasSMTPAcceptXProxyToPermission(SecurityIdentifier remoteIdentity, MultilevelAuthMechanism authMechanism)
		{
			ArgumentValidator.ThrowIfNull("remoteIdentity", remoteIdentity);
			return SmtpInSessionUtils.IsAuthenticated(remoteIdentity) && authMechanism == MultilevelAuthMechanism.MUTUALGSSAPI;
		}

		public static bool HasAcceptOorgProtocolCapability(SmtpReceiveCapabilities capabilities)
		{
			return (capabilities & SmtpReceiveCapabilities.AcceptOorgProtocol) != SmtpReceiveCapabilities.None;
		}

		public static bool HasAcceptOorgHeaderCapability(SmtpReceiveCapabilities capabilities)
		{
			return (capabilities & SmtpReceiveCapabilities.AcceptOorgHeader) != SmtpReceiveCapabilities.None;
		}

		public static bool HasAcceptOrgHeadersCapability(SmtpReceiveCapabilities capabilities)
		{
			return (capabilities & SmtpReceiveCapabilities.AcceptOrgHeaders) != SmtpReceiveCapabilities.None;
		}

		public static bool HasAcceptCloudServicesMailCapability(SmtpReceiveCapabilities capabilities)
		{
			return (capabilities & SmtpReceiveCapabilities.AcceptCloudServicesMail) != SmtpReceiveCapabilities.None;
		}

		public static bool HasAcceptCrossForestMailCapability(SmtpReceiveCapabilities capabilities)
		{
			return (capabilities & SmtpReceiveCapabilities.AcceptCrossForestMail) != SmtpReceiveCapabilities.None;
		}

		public static bool HasAllowSubmitCapability(SmtpReceiveCapabilities capabilities)
		{
			return (capabilities & SmtpReceiveCapabilities.AllowSubmit) != SmtpReceiveCapabilities.None;
		}

		public static bool ShouldAcceptOorgProtocol(SmtpReceiveCapabilities capabilities)
		{
			return SmtpInSessionUtils.HasAcceptOorgProtocolCapability(capabilities) || SmtpInSessionUtils.HasAcceptCloudServicesMailCapability(capabilities);
		}

		public static bool ShouldAcceptProxyProtocol(ProcessTransportRole transportRole, SmtpReceiveCapabilities capabilities)
		{
			return transportRole == ProcessTransportRole.Hub && (capabilities & SmtpReceiveCapabilities.AcceptProxyProtocol) != SmtpReceiveCapabilities.None;
		}

		public static bool ShouldAcceptProxyFromProtocol(ProcessTransportRole transportRole, SmtpReceiveCapabilities capabilities)
		{
			return (transportRole == ProcessTransportRole.Hub || transportRole == ProcessTransportRole.FrontEnd) && (capabilities & SmtpReceiveCapabilities.AcceptProxyFromProtocol) != SmtpReceiveCapabilities.None;
		}

		public static bool ShouldAcceptProxyToProtocol(ProcessTransportRole transportRole, SmtpReceiveCapabilities capabilities)
		{
			return transportRole == ProcessTransportRole.FrontEnd && (capabilities & SmtpReceiveCapabilities.AcceptProxyToProtocol) != SmtpReceiveCapabilities.None;
		}

		public static bool ShouldAcceptXAttrProtocol(SmtpReceiveCapabilities capabilities)
		{
			return (capabilities & SmtpReceiveCapabilities.AcceptXAttrProtocol) != SmtpReceiveCapabilities.None;
		}

		public static bool ShouldAcceptXSysProbeProtocol(SmtpReceiveCapabilities capabilities)
		{
			return (capabilities & SmtpReceiveCapabilities.AcceptXSysProbeProtocol) != SmtpReceiveCapabilities.None;
		}

		public static bool ShouldAcceptXOrigFromProtocol(SmtpReceiveCapabilities capabilities)
		{
			return (capabilities & SmtpReceiveCapabilities.AcceptXOriginalFromProtocol) != SmtpReceiveCapabilities.None;
		}

		public static bool ShouldAllowConsumerMail(SmtpReceiveCapabilities capabilities)
		{
			return (capabilities & SmtpReceiveCapabilities.AllowConsumerMail) != SmtpReceiveCapabilities.None;
		}

		public static bool ShouldAuthLoginBeAdvertised(AuthMechanisms authMechanism, SecureState secureState)
		{
			return (authMechanism & AuthMechanisms.BasicAuth) != AuthMechanisms.None && ((authMechanism & AuthMechanisms.BasicAuthRequireTLS) == AuthMechanisms.None || secureState == SecureState.StartTls || secureState == SecureState.AnonymousTls);
		}

		public static bool ShouldAuthGssApiBeAdvertised(bool integratedAuthenticationSupported, bool authGssApiEnabled)
		{
			return integratedAuthenticationSupported && authGssApiEnabled;
		}

		public static bool ShouldAuthNtlmBeAdvertised(bool integratedAuthenticationSupported)
		{
			return integratedAuthenticationSupported;
		}

		public static bool ShouldExpsGssApiBeAdvertised(AuthMechanisms authMechanism, ProcessTransportRole role)
		{
			return (authMechanism & AuthMechanisms.ExchangeServer) != AuthMechanisms.None && SmtpInSessionUtils.DoesRoleSupportInboundXExpsGssapi(role);
		}

		public static bool DoesRoleSupportInboundXExpsGssapi(ProcessTransportRole role)
		{
			switch (role)
			{
			case ProcessTransportRole.Hub:
			case ProcessTransportRole.FrontEnd:
			case ProcessTransportRole.MailboxDelivery:
				return true;
			}
			return false;
		}

		public static bool ShouldExpsExchangeAuthBeAdvertised(AuthMechanisms authMechanism, SecureState secureState, ProcessTransportRole role)
		{
			return (authMechanism & AuthMechanisms.ExchangeServer) != AuthMechanisms.None && secureState == SecureState.AnonymousTls && SmtpInSessionUtils.DoesRoleSupportInboundXExpsExchangeAuth(role);
		}

		public static bool DoesRoleSupportInboundXExpsExchangeAuth(ProcessTransportRole role)
		{
			switch (role)
			{
			case ProcessTransportRole.Hub:
			case ProcessTransportRole.FrontEnd:
			case ProcessTransportRole.MailboxDelivery:
				return true;
			}
			return false;
		}

		public static bool ShouldExpsNtlmBeAdvertised(AuthMechanisms authMechanism)
		{
			return (authMechanism & AuthMechanisms.ExchangeServer) != AuthMechanisms.None;
		}

		public static bool ShouldStartTlsBeAdvertised(AuthMechanisms authMechanism, SecureState secureState, bool startTlsSupported)
		{
			return secureState == SecureState.None && (authMechanism & AuthMechanisms.Tls) != AuthMechanisms.None && startTlsSupported;
		}

		public static bool ShouldAnonymousTlsBeAdvertised(AuthMechanisms authMechanism, SecureState secureState, bool anonymousTlsSupported)
		{
			return secureState == SecureState.None && (authMechanism & AuthMechanisms.ExchangeServer) != AuthMechanisms.None && anonymousTlsSupported;
		}

		public static bool ShouldXoorgBeAdvertised(SmtpReceiveCapabilities capabilities)
		{
			return SmtpInSessionUtils.ShouldAcceptOorgProtocol(capabilities);
		}

		public static bool ShouldXproxyBeAdvertised(ProcessTransportRole role, SmtpReceiveCapabilities capabilities, SecureState secureState)
		{
			return SmtpInSessionUtils.ShouldAcceptProxyProtocol(role, capabilities) || secureState == SecureState.AnonymousTls;
		}

		public static bool ShouldXproxyFromBeAdvertised(ProcessTransportRole role, SmtpReceiveCapabilities capabilities, SecureState secureState)
		{
			return SmtpInSessionUtils.ShouldAcceptProxyFromProtocol(role, capabilities) || ((role == ProcessTransportRole.Hub || role == ProcessTransportRole.FrontEnd) && secureState == SecureState.AnonymousTls);
		}

		public static bool ShouldXproxyToBeAdvertised(ProcessTransportRole role, SmtpReceiveCapabilities capabilities, SecureState secureState)
		{
			return SmtpInSessionUtils.ShouldAcceptProxyToProtocol(role, capabilities) || (role == ProcessTransportRole.FrontEnd && secureState == SecureState.AnonymousTls);
		}

		public static bool ShouldXrsetProxyToBeAdvertised(ProcessTransportRole role, SmtpReceiveCapabilities capabilities, SecureState secureState)
		{
			return SmtpInSessionUtils.ShouldXproxyToBeAdvertised(role, capabilities, secureState);
		}

		public static bool ShouldXSessionMdbGuidBeAdvertised(ProcessTransportRole role, SecureState secureState)
		{
			return role == ProcessTransportRole.MailboxDelivery && secureState == SecureState.AnonymousTls;
		}

		public static bool ShouldXAttrBeAdvertised(SmtpReceiveCapabilities capabilities, SecureState secureState)
		{
			return SmtpInSessionUtils.ShouldAcceptXAttrProtocol(capabilities) || (secureState == SecureState.AnonymousTls && MultiTenantTransport.MultiTenancyEnabled);
		}

		public static bool ShouldXSysProbeBeAdvertised(SmtpReceiveCapabilities capabilities, SecureState secureState)
		{
			return SmtpInSessionUtils.ShouldAcceptXSysProbeProtocol(capabilities) || secureState == SecureState.AnonymousTls;
		}

		public static bool ShouldExtendedPropertiesBeAdvertised(ProcessTransportRole role, SecureState secureState, bool extendedPropertiesEnabled)
		{
			return secureState == SecureState.AnonymousTls && (role == ProcessTransportRole.MailboxDelivery || role == ProcessTransportRole.Hub) && extendedPropertiesEnabled;
		}

		public static bool ShouldADRecipientCacheBeAdvertised(ProcessTransportRole role, SecureState secureState, bool adRecipientCacheEnabled)
		{
			return secureState == SecureState.AnonymousTls && (role == ProcessTransportRole.MailboxDelivery || role == ProcessTransportRole.Hub) && adRecipientCacheEnabled;
		}

		public static bool ShouldFastIndexBeAdvertised(ProcessTransportRole role, SecureState secureState, bool fastIndexEnabled)
		{
			return secureState == SecureState.AnonymousTls && (role == ProcessTransportRole.MailboxDelivery || role == ProcessTransportRole.Hub) && fastIndexEnabled;
		}

		public static bool ShouldXSessionTypeBeAdvertised(ProcessTransportRole role, SecureState secureState)
		{
			return role == ProcessTransportRole.MailboxDelivery && secureState == SecureState.AnonymousTls;
		}

		public static Permission GetPermissions(IAuthzAuthorization authzAuthorization, IntPtr userToken, RawSecurityDescriptor securityDescriptor)
		{
			Permission result = Permission.None;
			try
			{
				if (securityDescriptor != null)
				{
					result = authzAuthorization.CheckPermissions(userToken, securityDescriptor, null);
				}
			}
			catch (Win32Exception ex)
			{
				ExTraceGlobals.SmtpReceiveTracer.TraceError<int>(0L, "AuthzAuthorization.CheckPermissions failed with {0}.", ex.NativeErrorCode);
			}
			return result;
		}

		public static Permission GetPermissions(IAuthzAuthorization authzAuthorization, SecurityIdentifier client, RawSecurityDescriptor securityDescriptor)
		{
			Permission result = Permission.None;
			try
			{
				if (securityDescriptor != null)
				{
					result = authzAuthorization.CheckPermissions(client, securityDescriptor, null);
				}
			}
			catch (Win32Exception ex)
			{
				ExTraceGlobals.SmtpReceiveTracer.TraceError<int>(0L, "AuthzAuthorization.CheckPermissions failed with {0}.", ex.NativeErrorCode);
			}
			return result;
		}

		public static RestrictedHeaderSet RestrictedHeaderSetFromPermissions(Permission permissions)
		{
			RestrictedHeaderSet restrictedHeaderSet = RestrictedHeaderSet.None;
			if (!SmtpInSessionUtils.HasSMTPAcceptRoutingHeadersPermission(permissions))
			{
				restrictedHeaderSet |= RestrictedHeaderSet.MTA;
			}
			if (!SmtpInSessionUtils.HasSMTPAcceptForestHeadersPermission(permissions))
			{
				restrictedHeaderSet |= RestrictedHeaderSet.Forest;
			}
			if (!SmtpInSessionUtils.HasSMTPAcceptOrgHeadersPermission(permissions))
			{
				restrictedHeaderSet |= RestrictedHeaderSet.Organization;
			}
			return restrictedHeaderSet;
		}

		public static string FormatTimeSpan(TimeSpan span)
		{
			string text;
			if (span.Milliseconds > 0)
			{
				text = string.Concat(new string[]
				{
					span.Hours.ToString("00"),
					":",
					span.Minutes.ToString("00"),
					":",
					span.Seconds.ToString("00"),
					".",
					span.Milliseconds.ToString("000")
				});
			}
			else
			{
				text = string.Concat(new string[]
				{
					span.Hours.ToString("00"),
					":",
					span.Minutes.ToString("00"),
					":",
					span.Seconds.ToString("00")
				});
			}
			if (span.TotalDays > 0.0)
			{
				text = ((int)span.TotalDays).ToString(CultureInfo.InvariantCulture) + "." + text;
			}
			return text;
		}

		public static bool IsRemoteConnectionError(object error)
		{
			if (error is SocketError)
			{
				SocketError socketError = (SocketError)error;
				return socketError != SocketError.Shutdown && socketError != SocketError.TimedOut;
			}
			return false;
		}

		public static bool IsMuaSubmission(Permission permissions, SecurityIdentifier remoteIdentity)
		{
			ArgumentValidator.ThrowIfNull("remoteIdentity", remoteIdentity);
			bool flag = SmtpInSessionUtils.HasSMTPAcceptAnySenderPermission(permissions) || SmtpInSessionUtils.HasSMTPAcceptAuthoritativeDomainSenderPermission(permissions);
			return SmtpInSessionUtils.IsAuthenticated(remoteIdentity) && !flag && !SmtpInSessionUtils.IsPartner(remoteIdentity);
		}

		public static bool IsTarpitAuthenticationLevelHigh(Permission permissions, SecurityIdentifier remoteIdentity, bool tarpitMuaSubmission)
		{
			ArgumentValidator.ThrowIfNull("remoteIdentity", remoteIdentity);
			bool flag = SmtpInSessionUtils.IsAuthenticated(remoteIdentity);
			if (tarpitMuaSubmission)
			{
				return flag && !SmtpInSessionUtils.IsMuaSubmission(permissions, remoteIdentity);
			}
			return flag;
		}

		public static ADOperationResult TryCreateOrUpdateADRecipientCache(TransportMailItem transportMailItem, OrganizationId mailCommandInternalOrganizationId, IProtocolLogSession logSession)
		{
			ArgumentValidator.ThrowIfNull("transportMailItem", transportMailItem);
			ArgumentValidator.ThrowIfNull("logSession", logSession);
			ADOperationResult adoperationResult;
			if (mailCommandInternalOrganizationId != null)
			{
				adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
				{
					MultiTenantTransport.UpdateADRecipientCacheAndOrganizationScope(transportMailItem, mailCommandInternalOrganizationId);
				}, 0);
			}
			else
			{
				adoperationResult = MultiTenantTransport.TryCreateADRecipientCache(transportMailItem);
			}
			if (!adoperationResult.Succeeded)
			{
				logSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "Encountered AD exception during attribution. Details:{0}", new object[]
				{
					adoperationResult.Exception
				});
				MultiTenantTransport.TraceAttributionError(string.Format("Error {0} creating recipient cache for message {1}. We will try again in EOH for Permanent error. For transient error mail is rejected with transient response code.", adoperationResult.Exception, MultiTenantTransport.ToString(transportMailItem)), new object[0]);
			}
			return adoperationResult;
		}

		public static bool IsMaxProtocolErrorsExceeded(int numProtocolErrors, ReceiveConnector receiveConnector)
		{
			ArgumentValidator.ThrowIfNull("receiveConnector", receiveConnector);
			return !receiveConnector.MaxProtocolErrors.IsUnlimited && numProtocolErrors > receiveConnector.MaxProtocolErrors.Value;
		}

		public static string GetBreadcrumbsAsString(Breadcrumbs<SmtpInSessionBreadcrumbs> breadcrumbs)
		{
			SmtpInSessionBreadcrumbs[] array = new SmtpInSessionBreadcrumbs[64];
			int num = 0;
			foreach (SmtpInSessionBreadcrumbs smtpInSessionBreadcrumbs in ((IEnumerable<SmtpInSessionBreadcrumbs>)breadcrumbs))
			{
				array[num] = smtpInSessionBreadcrumbs;
				if (++num == 64)
				{
					break;
				}
			}
			return string.Join<SmtpInSessionBreadcrumbs>(Environment.NewLine, array);
		}

		public static void ApplyRoleBasedEhloOptionsOverrides(IEhloOptions ehloOptions, bool isFrontEndTransportProcess)
		{
			ArgumentValidator.ThrowIfNull("ehloOptions", ehloOptions);
			if (isFrontEndTransportProcess)
			{
				ehloOptions.Xexch50 = false;
				ehloOptions.XShadow = false;
				ehloOptions.XShadowRequest = false;
				ehloOptions.XAdrc = false;
				ehloOptions.XExprops = false;
				ehloOptions.XFastIndex = false;
			}
		}

		public static bool ShouldSupportIntegratedAuthentication(bool supportIntegratedAuth, bool isFrontEndTransportProcess)
		{
			return (!isFrontEndTransportProcess || !VariantConfiguration.InvariantNoFlightingSnapshot.Global.WindowsLiveID.Enabled) && supportIntegratedAuth;
		}

		public static bool IsMailFromNotAuthorized(bool oorgPresent, bool xAttrPresent, bool xSysProbePresent, SmtpReceiveCapabilities capabilities, Permission permissions)
		{
			return (oorgPresent && !SmtpInSessionUtils.ShouldAcceptOorgProtocol(capabilities)) || (xAttrPresent && !SmtpInSessionUtils.ShouldAcceptXAttrProtocol(capabilities) && !SmtpInSessionUtils.HasSMTPAcceptXAttrPermission(permissions)) || (xSysProbePresent && !SmtpInSessionUtils.ShouldAcceptXSysProbeProtocol(capabilities) && !SmtpInSessionUtils.HasSMTPAcceptXSysProbePermission(permissions));
		}

		public static bool IsMessageTooLarge(long messageSize, Permission permissions, long maxMessageSize)
		{
			return !SmtpInSessionUtils.HasSMTPBypassMessageSizeLimitPermission(permissions) && !SmtpInSessionUtils.HasSMTPAcceptOrgHeadersPermission(permissions) && messageSize > maxMessageSize;
		}

		public static string GetFormattedTemporaryMessageId(ulong sessionId, DateTime sessionStartTime, int numberOfMessagesReceived)
		{
			return string.Format(CultureInfo.InvariantCulture, "{0:X16};{1:yyyy-MM-ddTHH\\:mm\\:ss.fffZ};{2}", new object[]
			{
				sessionId,
				sessionStartTime,
				numberOfMessagesReceived
			});
		}

		public static bool ShouldThrottleIncomingTLSConnections(IPConnectionTable tlsIPConnectionTable, bool receiveTlsThrottlingEnabled)
		{
			ArgumentValidator.ThrowIfNull("tlsIPConnectionTable", tlsIPConnectionTable);
			return receiveTlsThrottlingEnabled && !tlsIPConnectionTable.CanAcceptConnection(IPAddress.None);
		}

		public static ulong GetSignificantBytesOfIPAddress(IPAddress ipAddress)
		{
			ArgumentValidator.ThrowIfNull("ipAddress", ipAddress);
			IPvxAddress pvxAddress = new IPvxAddress(ipAddress);
			if (pvxAddress.AddressFamily == AddressFamily.InterNetwork)
			{
				return pvxAddress.LowBytes;
			}
			return pvxAddress.HighBytes;
		}

		public static SmtpInCommand IdentifySmtpCommand(byte[] command, out int cmdEndOffset)
		{
			return SmtpInSessionUtils.IdentifySmtpCommand(command, 0, (command == null) ? 0 : command.Length, out cmdEndOffset);
		}

		public static SmtpInCommand IdentifySmtpCommand(byte[] command, int validDataOffset, int validDataLength, out int cmdEndOffset)
		{
			cmdEndOffset = 0;
			if (command == null || command.Length == 0 || validDataLength == 0)
			{
				return SmtpInCommand.UNKNOWN;
			}
			int num;
			SmtpInSessionUtils.GetCommandOffsets(command, validDataOffset, validDataLength, out num, out cmdEndOffset);
			char c = (char)Util.LowerC[(int)command[validDataOffset]];
			int num2 = cmdEndOffset - num;
			char c2 = c;
			switch (c2)
			{
			case 'a':
				if (SmtpCommand.CompareArg(SmtpCommand.AUTH, command, num, num2))
				{
					return SmtpInCommand.AUTH;
				}
				break;
			case 'b':
				if (SmtpCommand.CompareArg(SmtpCommand.BDAT, command, num, num2))
				{
					return SmtpInCommand.BDAT;
				}
				break;
			case 'c':
			case 'f':
			case 'g':
			case 'i':
			case 'j':
			case 'k':
			case 'l':
			case 'o':
			case 'p':
				break;
			case 'd':
				if (SmtpCommand.CompareArg(SmtpCommand.DATA, command, num, num2))
				{
					return SmtpInCommand.DATA;
				}
				break;
			case 'e':
				if (SmtpCommand.CompareArg(SmtpCommand.EHLO, command, num, num2))
				{
					return SmtpInCommand.EHLO;
				}
				if (SmtpCommand.CompareArg(SmtpCommand.EXPN, command, num, num2))
				{
					return SmtpInCommand.EXPN;
				}
				break;
			case 'h':
				if (SmtpCommand.CompareArg(SmtpCommand.HELO, command, num, num2))
				{
					return SmtpInCommand.HELO;
				}
				if (SmtpCommand.CompareArg(SmtpCommand.HELP, command, num, num2))
				{
					return SmtpInCommand.HELP;
				}
				break;
			case 'm':
				if (SmtpCommand.CompareArg(SmtpCommand.MAIL, command, num, num2))
				{
					return SmtpInCommand.MAIL;
				}
				break;
			case 'n':
				if (SmtpCommand.CompareArg(SmtpCommand.NOOP, command, num, num2))
				{
					return SmtpInCommand.NOOP;
				}
				break;
			case 'q':
				if (SmtpCommand.CompareArg(SmtpCommand.QUIT, command, num, num2))
				{
					return SmtpInCommand.QUIT;
				}
				break;
			case 'r':
				if (SmtpCommand.CompareArg(SmtpCommand.RCPT, command, num, num2))
				{
					return SmtpInCommand.RCPT;
				}
				if (SmtpCommand.CompareArg(SmtpCommand.RSET, command, num, num2))
				{
					return SmtpInCommand.RSET;
				}
				if (SmtpCommand.CompareArg(SmtpCommand.RCPT2, command, num, num2))
				{
					return SmtpInCommand.RCPT2;
				}
				break;
			case 's':
				if (num2 != SmtpCommand.STARTTLS.Length)
				{
					return SmtpInCommand.UNKNOWN;
				}
				if (SmtpCommand.CompareArg(SmtpCommand.STARTTLS, command, num, num2))
				{
					return SmtpInCommand.STARTTLS;
				}
				break;
			default:
				switch (c2)
				{
				case 'v':
					if (SmtpCommand.CompareArg(SmtpCommand.VRFY, command, num, num2))
					{
						return SmtpInCommand.VRFY;
					}
					break;
				case 'x':
					if (SmtpCommand.CompareArg(SmtpCommand.XEXCH50, command, num, num2))
					{
						return SmtpInCommand.XEXCH50;
					}
					if (SmtpCommand.CompareArg(SmtpCommand.ANONYMOUSTLS, command, num, num2))
					{
						return SmtpInCommand.XANONYMOUSTLS;
					}
					if (SmtpCommand.CompareArg(SmtpCommand.EXPS, command, num, num2))
					{
						return SmtpInCommand.XEXPS;
					}
					if (SmtpCommand.CompareArg(SmtpCommand.XSHADOW, command, num, num2))
					{
						return SmtpInCommand.XSHADOW;
					}
					if (SmtpCommand.CompareArg(SmtpCommand.XQDISCARD, command, num, num2))
					{
						return SmtpInCommand.XQDISCARD;
					}
					if (SmtpCommand.CompareArg(SmtpCommand.XPROXY, command, num, num2))
					{
						return SmtpInCommand.XPROXY;
					}
					if (SmtpCommand.CompareArg(SmtpCommand.XPROXYFROM, command, num, num2))
					{
						return SmtpInCommand.XPROXYFROM;
					}
					if (SmtpCommand.CompareArg(SmtpCommand.XPROXYTO, command, num, num2))
					{
						return SmtpInCommand.XPROXYTO;
					}
					if (SmtpCommand.CompareArg(SmtpCommand.XSHADOWREQUEST, command, num, num2))
					{
						return SmtpInCommand.XSHADOWREQUEST;
					}
					if (SmtpCommand.CompareArg(SmtpCommand.XSESSIONPARAMS, command, num, num2))
					{
						return SmtpInCommand.XSESSIONPARAMS;
					}
					break;
				}
				break;
			}
			return SmtpInCommand.UNKNOWN;
		}

		private static void GetCommandOffsets(byte[] command, int validDataOffset, int validDataLength, out int beginOffset, out int endOffset)
		{
			beginOffset = 0;
			endOffset = 0;
			if (command == null)
			{
				return;
			}
			beginOffset = BufferParser.GetNextToken(command, validDataOffset, validDataLength, out endOffset);
		}
	}
}
