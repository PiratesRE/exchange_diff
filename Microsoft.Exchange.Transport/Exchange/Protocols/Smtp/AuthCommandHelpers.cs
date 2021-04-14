using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Security;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.Transport.Logging;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal static class AuthCommandHelpers
	{
		public static bool TryGetLocalCertificatePublicKey(INetworkConnection networkConnection, out byte[] publicKey, out Exception exception)
		{
			ArgumentValidator.ThrowIfNull("networkConnection", networkConnection);
			ArgumentValidator.ThrowIfInvalidValue<INetworkConnection>("networkConnection.IsTls", networkConnection, (INetworkConnection nc) => nc.IsTls);
			try
			{
				publicKey = networkConnection.LocalCertificate.GetPublicKey();
				exception = null;
				return true;
			}
			catch (CryptographicException ex)
			{
				exception = ex;
			}
			publicKey = null;
			return false;
		}

		public static bool TryFlushKerberosTicketCache(TimeSpan minIntervalSinceLastFlush, IProtocolLogSession logSession)
		{
			if (minIntervalSinceLastFlush == TimeSpan.MaxValue || AuthCommandHelpers.lastKerberosTicketCacheFlushTime + minIntervalSinceLastFlush > DateTime.UtcNow)
			{
				return true;
			}
			bool result = true;
			try
			{
				if (Interlocked.Increment(ref AuthCommandHelpers.kerberosTicketCacheFlushGuard) > 1)
				{
					return true;
				}
				try
				{
					Kerberos.FlushTicketCache();
					logSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "Flushed Kerberos ticket cache");
				}
				catch (Win32Exception ex)
				{
					result = false;
					logSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "Failed to flushed Kerberos ticket cache; ErrorCode=" + ex.ErrorCode);
				}
				AuthCommandHelpers.lastKerberosTicketCacheFlushTime = DateTime.UtcNow;
			}
			finally
			{
				Interlocked.Decrement(ref AuthCommandHelpers.kerberosTicketCacheFlushGuard);
			}
			return result;
		}

		public static TransportMiniRecipient GetMiniRecipientBySid(IRecipientSession recipientSession, SecurityIdentifier sid, ITracer tracer, int traceId)
		{
			ArgumentValidator.ThrowIfNull("sid", sid);
			ArgumentValidator.ThrowIfNull("tracer", tracer);
			TransportMiniRecipient miniRecipient = null;
			ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				try
				{
					miniRecipient = recipientSession.FindMiniRecipientBySid<TransportMiniRecipient>(sid, TransportMiniRecipientSchema.Properties);
				}
				catch (NonUniqueRecipientException arg)
				{
					tracer.TraceError<string, NonUniqueRecipientException>((long)traceId, "GetMiniRecipientBySid: Found more than one AD recipient for SecurityIdentifier '{0}', exception '{1}'", sid.ToString(), arg);
					miniRecipient = null;
				}
			});
			if (!adoperationResult.Succeeded)
			{
				tracer.TraceError<string, Exception>((long)traceId, "GetMiniRecipientBySid: Failed for SecurityIdentifier '{0}', exception '{1}'", sid.ToString(), adoperationResult.Exception);
			}
			return miniRecipient;
		}

		public static TransportMiniRecipient GetMiniRecipientByProxyAddress(IRecipientSession recipientSession, ProxyAddress address, ITracer tracer, int traceId)
		{
			TransportMiniRecipient miniRecipient = null;
			ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				try
				{
					miniRecipient = recipientSession.FindMiniRecipientByProxyAddress<TransportMiniRecipient>(address, TransportMiniRecipientSchema.Properties);
				}
				catch (NonUniqueRecipientException arg)
				{
					tracer.TraceError<string, NonUniqueRecipientException>((long)traceId, "GetMiniRecipientByProxyAddress: Found more than one AD recipient for proxy address '{0}', exception '{1}'", address.AddressString, arg);
					miniRecipient = null;
				}
			});
			if (!adoperationResult.Succeeded)
			{
				tracer.TraceError<string, Exception>((long)traceId, "GetMiniRecipientByProxyAddress: Failed for proxy address '{0}', exception '{1}'", address.AddressString, adoperationResult.Exception);
			}
			return miniRecipient;
		}

		public static bool IsExchangeAuthHashSupported(IEhloOptions advertisedEhloOptions)
		{
			ArgumentValidator.ThrowIfNull("advertisedEhloOptions", advertisedEhloOptions);
			return advertisedEhloOptions.ExchangeAuthArgs.Any((string hash) => hash.Equals("SHA256", StringComparison.OrdinalIgnoreCase));
		}

		public static void LogExchangeAuthHashNotSupported(string advertisedFqdn, ITracer tracer, int traceId, IProtocolLogSession logSession, IExEventLog eventLog)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("advertisedFqdn", advertisedFqdn);
			ArgumentValidator.ThrowIfNull("tracer", tracer);
			ArgumentValidator.ThrowIfNull("logSession", logSession);
			ArgumentValidator.ThrowIfNull("eventLog", eventLog);
			string text = string.Format("Remote {0} advertised hash for Exchange authentication which is not supported by this server.", advertisedFqdn);
			tracer.TraceError((long)traceId, text);
			logSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "Remote " + advertisedFqdn + " advertised hash for Exchange authentication which is not supported by this server.");
			eventLog.LogEvent(TransportEventLogConstants.Tuple_ExchangeAuthHashNotSupported, null, new object[]
			{
				advertisedFqdn
			});
			EventNotificationItem.Publish(ExchangeComponent.Transport.Name, "ExchangeAuthHashNotSupported", null, text, ResultSeverityLevel.Warning, false);
		}

		public static string RedactUserNameInErrorDetails(string errorDetails, bool isDataRedactionNecessary)
		{
			if (!isDataRedactionNecessary || string.IsNullOrEmpty(errorDetails))
			{
				return errorDetails;
			}
			List<string> list;
			if (!Util.ExtractEmailAddressesFromString(errorDetails, out list))
			{
				return errorDetails;
			}
			string newValue = Util.RedactIfNecessary(list[0], true);
			return errorDetails.Replace(list[0], newValue);
		}

		public static bool TryGetAuthenticatedSidFromIdentity(AuthenticationContext authContext, IProtocolLogSession protocolLogSession, ITracer tracer, int traceId, IExEventLog eventLog, out SecurityIdentifier authenticatedSid)
		{
			ArgumentValidator.ThrowIfNull("authContext", authContext);
			ArgumentValidator.ThrowIfNull("protocolLogSession", protocolLogSession);
			ArgumentValidator.ThrowIfNull("tracer", tracer);
			ArgumentValidator.ThrowIfNull("eventLog", eventLog);
			SystemException ex;
			if (!authContext.TryGetAuthenticatedSidFromIdentity(out authenticatedSid, out ex))
			{
				tracer.TraceError<SystemException>((long)traceId, "Could not obtain SID due to {0}. Failing authentication.", ex);
				eventLog.LogEvent(TransportEventLogConstants.Tuple_SmtpReceiveCouldNotDetermineUserNameOrSid, null, new object[]
				{
					ex
				});
				protocolLogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "Inbound authentication failed as we can not determine the identity of the client because of {0}", new object[]
				{
					ex.Message
				});
				return false;
			}
			return true;
		}

		public static string GetAuthenticatedUsernameFromIdentity(AuthenticationContext authContext, SecurityIdentifier authenticatedSid, ITracer tracer, int traceId, IExEventLog eventLog)
		{
			ArgumentValidator.ThrowIfNull("authContext", authContext);
			ArgumentValidator.ThrowIfNull("authenticatedSid", authenticatedSid);
			ArgumentValidator.ThrowIfNull("tracer", tracer);
			ArgumentValidator.ThrowIfNull("eventLog", eventLog);
			string result;
			SystemException ex;
			if (!authContext.TryGetAuthenticatedUsernameFromIdentity(out result, out ex))
			{
				result = authenticatedSid.ToString();
				tracer.TraceError<SystemException>((long)traceId, "Could not obtain name due to {0}. Using SID.", ex);
				eventLog.LogEvent(TransportEventLogConstants.Tuple_SmtpReceiveCouldNotDetermineUserNameOrSid, null, new object[]
				{
					ex
				});
			}
			return result;
		}

		public static bool IsProhibitedAccount(AuthenticationContext authContext)
		{
			ArgumentValidator.ThrowIfNull("authContext", authContext);
			return authContext.IsWellKnownAdministrator || authContext.IsGuest || authContext.IsAnonymous;
		}

		public static void LogAuthenticatedAsProhibitedAccount(string authenticatedUsername, string redactedAuthenticatedUsername, string receiveConnectorName, IProtocolLogSession protocolLogSession, ITracer tracer, int traceId, IExEventLog eventLog)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("authenticatedUsername", authenticatedUsername);
			ArgumentValidator.ThrowIfNullOrEmpty("redactedAuthenticatedUsername", redactedAuthenticatedUsername);
			ArgumentValidator.ThrowIfNullOrEmpty("receiveConnectorName", receiveConnectorName);
			ArgumentValidator.ThrowIfNull("protocolLogSession", protocolLogSession);
			ArgumentValidator.ThrowIfNull("tracer", tracer);
			ArgumentValidator.ThrowIfNull("eventLog", eventLog);
			tracer.TraceDebug<string>((long)traceId, "Reject well-known account authentication for {0}", authenticatedUsername);
			eventLog.LogEvent(TransportEventLogConstants.Tuple_SmtpReceiveAuthorizationRejected, null, new object[]
			{
				authenticatedUsername,
				receiveConnectorName
			});
			protocolLogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "Inbound authentication failed as we reject well-known account authentication for {0}", new object[]
			{
				redactedAuthenticatedUsername
			});
		}

		public static void LogDoesNotHaveSmtpSubmitPermission(string authenticatedUsername, string redactedAuthenticatedUsername, string receiveConnectorName, IProtocolLogSession protocolLogSession, ITracer tracer, int traceId, IExEventLog eventLog)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("authenticatedUsername", authenticatedUsername);
			ArgumentValidator.ThrowIfNullOrEmpty("redactedAuthenticatedUsername", redactedAuthenticatedUsername);
			ArgumentValidator.ThrowIfNullOrEmpty("receiveConnectorName", receiveConnectorName);
			ArgumentValidator.ThrowIfNull("protocolLogSession", protocolLogSession);
			ArgumentValidator.ThrowIfNull("tracer", tracer);
			ArgumentValidator.ThrowIfNull("eventLog", eventLog);
			tracer.TraceDebug((long)traceId, "Fail authentication because no submit permission.");
			eventLog.LogEvent(TransportEventLogConstants.Tuple_SmtpReceiveAuthorizationSubmitRejected, null, new object[]
			{
				authenticatedUsername,
				receiveConnectorName
			});
			protocolLogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "Inbound authentication failed because the client {0} doesn't have submit permission.", new object[]
			{
				redactedAuthenticatedUsername
			});
		}

		public static bool IsCancelAuthBlob(CommandContext commandContext)
		{
			ArgumentValidator.ThrowIfNull("commandContext", commandContext);
			return commandContext.Length == 1 && commandContext.Command[commandContext.Offset] == 42;
		}

		public static SecureString SecureStringFromBytes(byte[] bytes)
		{
			ArgumentValidator.ThrowIfNull("bytes", bytes);
			SecureString secureString = new SecureString();
			int arrayLengthWithNoMoreThanOneTrailingNull = AuthCommandHelpers.GetArrayLengthWithNoMoreThanOneTrailingNull(bytes);
			for (int i = 0; i < arrayLengthWithNoMoreThanOneTrailingNull; i++)
			{
				secureString.AppendChar((char)bytes[i]);
			}
			return secureString;
		}

		public static byte[] UsernameFromDomainAndUsername(byte[] domain, byte[] username)
		{
			ArgumentValidator.ThrowIfNull("domain", domain);
			ArgumentValidator.ThrowIfNull("username", username);
			int num = (domain.Length == 1) ? 0 : AuthCommandHelpers.GetArrayLengthWithNoMoreThanOneTrailingNull(domain);
			int arrayLengthWithNoMoreThanOneTrailingNull = AuthCommandHelpers.GetArrayLengthWithNoMoreThanOneTrailingNull(username);
			byte[] array = new byte[num + arrayLengthWithNoMoreThanOneTrailingNull];
			int num2 = 0;
			for (int i = 0; i < num; i++)
			{
				if (i == num - 1)
				{
					array[num2] = 47;
				}
				else
				{
					array[num2] = domain[i];
				}
				num2++;
			}
			for (int j = 0; j < arrayLengthWithNoMoreThanOneTrailingNull; j++)
			{
				array[num2] = username[j];
				num2++;
			}
			return array;
		}

		private static int GetArrayLengthWithNoMoreThanOneTrailingNull(byte[] array)
		{
			ArgumentValidator.ThrowIfNull("array", array);
			int num = array.Length;
			if (array.Length > 1)
			{
				int num2 = array.Length - 1;
				while (num2 >= 1 && array[num2] == 0 && array[num2 - 1] == 0)
				{
					num--;
					num2--;
				}
			}
			return num;
		}

		public static bool IsUserLookupRequiredOnAuth(SmtpAuthenticationMechanism authenticationMechanism, SecurityIdentifier remoteIdentity, ProcessTransportRole role)
		{
			ArgumentValidator.ThrowIfNull("remoteIdentity", remoteIdentity);
			return (role == ProcessTransportRole.FrontEnd || role == ProcessTransportRole.Hub) && AuthCommandHelpers.IsEndUserAuthentication(authenticationMechanism, remoteIdentity);
		}

		public static bool TryLookupAuthenticatedUserRecipientObjectFromSid(byte[] loginUserName, SecurityIdentifier authenticatedSid, ReceiveConnector receiveConnector, IProtocolLogSession protocolLogSession, ITracer tracer, int traceCorrelator, string redactedUserName, out TransportMiniRecipient authUserRecipient, out SmtpResponse failureResponse)
		{
			ArgumentValidator.ThrowIfNull("receiveConnector", receiveConnector);
			ArgumentValidator.ThrowIfNull("authenticatedSid", authenticatedSid);
			ArgumentValidator.ThrowIfNull("protocolLogSession", protocolLogSession);
			ArgumentValidator.ThrowIfNull("tracer", tracer);
			ArgumentValidator.ThrowIfNullOrEmpty("redactedUserName", redactedUserName);
			IRecipientSession recipientSession = null;
			if (receiveConnector.LiveCredentialEnabled && loginUserName != null)
			{
				SmtpAddress authenticatedAddress = new SmtpAddress(new ASCIIEncoding().GetString(loginUserName, 0, loginUserName.Length - 1));
				ADSessionSettings sessionSettings = null;
				ADNotificationAdapter.TryRunADOperation(delegate()
				{
					sessionSettings = ADSessionSettings.FromTenantAcceptedDomain(authenticatedAddress.Domain);
				});
				if (sessionSettings != null)
				{
					if (AuthCommandHelpers.IsTenantInLockedOutState(sessionSettings, protocolLogSession, tracer, traceCorrelator, redactedUserName, out failureResponse))
					{
						authUserRecipient = null;
						return false;
					}
					ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
					{
						recipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.IgnoreInvalid, sessionSettings, 712, "TryLookupAuthenticatedUserRecipientObjectFromSid", "f:\\15.00.1497\\sources\\dev\\Transport\\src\\Smtp\\SmtpIn\\SmtpCommands\\AuthCommandHelpers.cs");
					});
					if (!adoperationResult.Succeeded)
					{
						tracer.TraceDebug<string, Exception>((long)traceCorrelator, "TenantAccepted Domain not found {0}: {1}", authenticatedAddress.Domain, adoperationResult.Exception);
						protocolLogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "Inbound Authentication failed to find accepted domain " + authenticatedAddress.Domain);
					}
				}
			}
			if (recipientSession == null)
			{
				ADOperationResult adoperationResult2 = ADNotificationAdapter.TryRunADOperation(delegate()
				{
					recipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 739, "TryLookupAuthenticatedUserRecipientObjectFromSid", "f:\\15.00.1497\\sources\\dev\\Transport\\src\\Smtp\\SmtpIn\\SmtpCommands\\AuthCommandHelpers.cs");
				}, 0);
				if (!adoperationResult2.Succeeded)
				{
					tracer.TraceDebug<Exception>((long)traceCorrelator, "Error when getting recipient session: {0}", adoperationResult2.Exception);
					protocolLogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "Error when getting recipient session: {0}", new object[]
					{
						adoperationResult2.Exception
					});
					authUserRecipient = null;
					failureResponse = ((adoperationResult2.ErrorCode == ADOperationErrorCode.RetryableError) ? SmtpResponse.AuthenticationFailedTemporary : SmtpResponse.AuthenticationFailedPermanent);
					return false;
				}
			}
			authUserRecipient = AuthCommandHelpers.GetMiniRecipientBySid(recipientSession, authenticatedSid, tracer, traceCorrelator);
			if (authUserRecipient == null)
			{
				protocolLogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "Could not find user " + redactedUserName);
				failureResponse = SmtpResponse.AuthenticationFailureUserNotFound;
				return false;
			}
			failureResponse = SmtpResponse.Empty;
			return true;
		}

		public static ParseAndProcessResult<SmtpInStateMachineEvents> OnAuthenticationComplete(AuthParseOutput authParseOutput, AuthenticationContext authenticationContext, SmtpInSessionState sessionState, byte[] loginUsername, byte[] lastNegotiateSecurityContextResponse, AuthCommandHelpers.GetAuthenticationSuccessfulResult getAuthenticationSuccessfulResult, AuthCommandHelpers.GetAuthenticationFailureResult getAuthenticationFailureResult)
		{
			ArgumentValidator.ThrowIfNull("authParseOutput", authParseOutput);
			ArgumentValidator.ThrowIfNull("authenticationContext", authenticationContext);
			ArgumentValidator.ThrowIfNull("sessionState", sessionState);
			ArgumentValidator.ThrowIfNull("getAuthenticationSuccessfulResult", getAuthenticationSuccessfulResult);
			ArgumentValidator.ThrowIfNull("getAuthenticationFailureResult", getAuthenticationFailureResult);
			int hashCode = sessionState.GetHashCode();
			if (AuthCommandHelpers.IsInactiveFrontend(sessionState.ServerState.ServiceState, sessionState.Configuration.TransportConfiguration.ProcessTransportRole) && !AuthCommandHelpers.IsSysProbeSession(sessionState.TlsDomainCapabilities))
			{
				sessionState.ProtocolLogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "Rejecting authentication and disconnecting as frontend transport service is Inactive.");
				return getAuthenticationFailureResult(new SmtpResponse?(SmtpResponse.ServiceInactive), true);
			}
			SecurityIdentifier securityIdentifier;
			if (!AuthCommandHelpers.TryGetAuthenticatedSidFromIdentity(authenticationContext, sessionState.ProtocolLogSession, sessionState.Tracer, hashCode, sessionState.EventLog, out securityIdentifier))
			{
				AuthCommandHelpers.OnAuthenticationFailure(authenticationContext, sessionState);
				return getAuthenticationFailureResult(null, true);
			}
			string authenticatedUsernameFromIdentity = AuthCommandHelpers.GetAuthenticatedUsernameFromIdentity(authenticationContext, securityIdentifier, sessionState.Tracer, hashCode, sessionState.EventLog);
			string text = Util.RedactUserName(authenticatedUsernameFromIdentity);
			if (AuthCommandHelpers.IsProhibitedAccount(authenticationContext))
			{
				AuthCommandHelpers.LogAuthenticatedAsProhibitedAccount(authenticatedUsernameFromIdentity, text, sessionState.ReceiveConnector.Name, sessionState.ProtocolLogSession, sessionState.Tracer, hashCode, sessionState.EventLog);
				AuthCommandHelpers.OnAuthenticationFailure(authenticationContext, sessionState);
				return getAuthenticationFailureResult(null, false);
			}
			Permission permissions = SmtpInSessionUtils.GetPermissions(sessionState.AuthzAuthorization, authenticationContext.Identity.Token, sessionState.ReceiveConnectorStub.SecurityDescriptor);
			if (!SmtpInSessionUtils.HasSMTPSubmitPermission(permissions))
			{
				AuthCommandHelpers.LogDoesNotHaveSmtpSubmitPermission(authenticatedUsernameFromIdentity, text, sessionState.ReceiveConnector.Name, sessionState.ProtocolLogSession, sessionState.Tracer, hashCode, SmtpCommand.EventLog);
				AuthCommandHelpers.OnAuthenticationFailure(authenticationContext, sessionState);
				return getAuthenticationFailureResult(null, false);
			}
			TransportMiniRecipient transportMiniRecipient = null;
			if (AuthCommandHelpers.IsUserLookupRequiredOnAuth(authParseOutput.AuthenticationMechanism, securityIdentifier, sessionState.Configuration.TransportConfiguration.ProcessTransportRole))
			{
				SmtpResponse value;
				if (!AuthCommandHelpers.TryLookupAuthenticatedUserRecipientObjectFromSid(loginUsername, securityIdentifier, sessionState.ReceiveConnector, sessionState.ProtocolLogSession, sessionState.Tracer, hashCode, text, out transportMiniRecipient, out value))
				{
					AuthCommandHelpers.OnAuthenticationFailure(authenticationContext, sessionState);
					return getAuthenticationFailureResult(new SmtpResponse?(value), true);
				}
				if (VariantConfiguration.InvariantNoFlightingSnapshot.Transport.ClientAuthRequireMailboxDatabase.Enabled && transportMiniRecipient.Database == null)
				{
					sessionState.ProtocolLogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "Inbound authentication succeeded, but {0} mailbox database is null.", new object[]
					{
						text
					});
					AuthCommandHelpers.OnAuthenticationFailure(authenticationContext, sessionState);
					return getAuthenticationFailureResult(new SmtpResponse?(SmtpResponse.MailboxOffline), true);
				}
				if (SmtpInAccessChecker.HasZeroProhibitSendQuota(transportMiniRecipient, authenticatedUsernameFromIdentity, authenticatedUsernameFromIdentity, sessionState.ReceiveConnector.Name, sessionState.Tracer, hashCode))
				{
					sessionState.ProtocolLogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "Inbound authentication succeeded, but {0} has ProhibitSendQuota of zero.", new object[]
					{
						text
					});
					AuthCommandHelpers.OnAuthenticationFailure(authenticationContext, sessionState);
					return getAuthenticationFailureResult(new SmtpResponse?(SmtpResponse.SubmissionDisabledBySendQuota), true);
				}
			}
			WindowsIdentity windowsIdentity = authenticationContext.DetachIdentity();
			if (windowsIdentity != null)
			{
				permissions = SmtpInSessionUtils.GetPermissions(sessionState.AuthzAuthorization, windowsIdentity.Token, sessionState.ReceiveConnectorStub.SecurityDescriptor);
			}
			sessionState.SetAuthenticatedIdentity(transportMiniRecipient, securityIdentifier, authenticatedUsernameFromIdentity, windowsIdentity, authParseOutput.MultilevelAuthMechanism, permissions);
			return getAuthenticationSuccessfulResult(lastNegotiateSecurityContextResponse);
		}

		public static void OnAuthenticationFailure(AuthenticationContext authenticationContext, SmtpInSessionState sessionState)
		{
			ArgumentValidator.ThrowIfNull("authenticationContext", authenticationContext);
			ArgumentValidator.ThrowIfNull("sessionState", sessionState);
			sessionState.ProtocolLogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "Username: {0}", new object[]
			{
				Util.ExtractAuthUsernameToLog(authenticationContext)
			});
			sessionState.IncrementNumLogonFailures();
			if (sessionState.IsMaxLogonFailuresExceeded)
			{
				sessionState.EventLog.LogEvent(TransportEventLogConstants.Tuple_SmtpReceiveAuthenticationFailedTooManyErrors, null, new object[]
				{
					sessionState.ReceiveConnector.Name,
					sessionState.ReceiveConnector.MaxLogonFailures,
					sessionState.NetworkConnection.RemoteEndPoint
				});
			}
		}

		private static bool IsTenantInLockedOutState(ADSessionSettings sessionSettings, IProtocolLogSession protocolLogSession, ITracer tracer, int traceCorrelator, string redactedUserName, out SmtpResponse failureResponse)
		{
			failureResponse = SmtpResponse.Empty;
			if (sessionSettings.CurrentOrganizationId.Equals(OrganizationId.ForestWideOrgId))
			{
				return false;
			}
			bool isTenantLockedOut = false;
			ITenantConfigurationSession tenantConfigurationSession = null;
			ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				if (tenantConfigurationSession == null)
				{
					tenantConfigurationSession = DirectorySessionFactory.Default.CreateTenantConfigurationSession(ConsistencyMode.IgnoreInvalid, sessionSettings, 1051, "IsTenantInLockedOutState", "f:\\15.00.1497\\sources\\dev\\Transport\\src\\Smtp\\SmtpIn\\SmtpCommands\\AuthCommandHelpers.cs");
				}
				isTenantLockedOut = tenantConfigurationSession.IsTenantLockedOut();
			}, 0);
			if (!adoperationResult.Succeeded)
			{
				tracer.TraceDebug<Exception>((long)traceCorrelator, "Error when checking tenant locked out state: {0}", adoperationResult.Exception);
				protocolLogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "Error when checking tenant locked out state: {0}", new object[]
				{
					adoperationResult.Exception
				});
				failureResponse = ((adoperationResult.ErrorCode == ADOperationErrorCode.RetryableError) ? SmtpResponse.CheckTenantLockedOutFailedTemporary : SmtpResponse.CheckTenantLockedOutFailedPermanent);
				return true;
			}
			if (isTenantLockedOut)
			{
				protocolLogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "Tenant is locked out. Failing auth for " + redactedUserName);
				failureResponse = SmtpResponse.AuthenticationFailureTenantLockedOut;
			}
			return isTenantLockedOut;
		}

		private static bool IsEndUserAuthentication(SmtpAuthenticationMechanism authenticationMechanism, SecurityIdentifier remoteIdentity)
		{
			return authenticationMechanism != SmtpAuthenticationMechanism.None && authenticationMechanism != SmtpAuthenticationMechanism.ExchangeAuth && !SmtpInSessionUtils.IsExternalAuthoritative(remoteIdentity) && !SmtpInSessionUtils.IsPartner(remoteIdentity);
		}

		public static bool IsInactiveFrontend(ServiceState serviceState, ProcessTransportRole role)
		{
			return serviceState == ServiceState.Inactive && role == ProcessTransportRole.FrontEnd;
		}

		public static bool IsSysProbeSession(SmtpReceiveCapabilities? tlsDomainCapabilities)
		{
			return tlsDomainCapabilities != null && (tlsDomainCapabilities.Value & SmtpReceiveCapabilities.AcceptXSysProbeProtocol) != SmtpReceiveCapabilities.None;
		}

		private static DateTime lastKerberosTicketCacheFlushTime = DateTime.MinValue;

		private static int kerberosTicketCacheFlushGuard;

		public delegate ParseAndProcessResult<SmtpInStateMachineEvents> GetAuthenticationSuccessfulResult(byte[] lastNegotiateSecurityContextResponse);

		public delegate ParseAndProcessResult<SmtpInStateMachineEvents> GetAuthenticationFailureResult(SmtpResponse? customSmtpResponse = null, bool disconnectClient = false);
	}
}
