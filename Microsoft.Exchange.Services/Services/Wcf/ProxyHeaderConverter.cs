using System;
using System.Security.Principal;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Services.Core;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal static class ProxyHeaderConverter
	{
		internal static AuthZClientInfo ToAuthZClientInfo(AuthZClientInfo callerClientInfo, ProxyHeaderType headerType, byte[] proxyHeaderBytes)
		{
			AuthZClientInfo result;
			try
			{
				ProxyHeaderConverter.VerifyTokenSerializationRight(callerClientInfo);
				ProxyHeaderValue proxyHeaderValue = new ProxyHeaderValue(headerType, proxyHeaderBytes);
				SerializedSecurityAccessToken serializedSecurityAccessToken = ProxySecurityContextDecoder.Decode(proxyHeaderValue);
				ExTraceGlobals.ProxyEvaluatorTracer.TraceDebug<string>(0L, "[ProxyHeaderConverter::ToAuthZClientInfo] Creating AuthZClientInfo from token (smtpAddress [for DC env] is \"{0}\")", (!string.IsNullOrEmpty(serializedSecurityAccessToken.SmtpAddress)) ? serializedSecurityAccessToken.SmtpAddress : "<null>");
				result = AuthZClientInfo.FromSecurityAccessToken(serializedSecurityAccessToken);
			}
			catch (AuthzException innerException)
			{
				ExTraceGlobals.ProxyEvaluatorTracer.TraceDebug(0L, "[ProxyHeaderConverter::ToAuthZClientInfo] Trying to create a ClientSecurityContext from proxy context failed with AuthZException.");
				AuthZFailureException exception = new AuthZFailureException(innerException);
				throw FaultExceptionUtilities.CreateFault(exception, FaultParty.Sender);
			}
			catch (InvalidSerializedAccessTokenException exception2)
			{
				throw FaultExceptionUtilities.CreateFault(exception2, FaultParty.Sender);
			}
			return result;
		}

		internal static AuthZClientInfo ToPartnerAuthZClientInfo(AuthZClientInfo callerClientInfo, ProxyHeaderType headerType, byte[] proxyHeaderBytes)
		{
			ProxyHeaderConverter.VerifyTokenSerializationRight(callerClientInfo);
			PartnerAccessToken token = PartnerAccessToken.FromBytes(proxyHeaderBytes);
			return PartnerAuthZClientInfo.FromPartnerAccessToken(token);
		}

		private static void VerifyTokenSerializationRight(AuthZClientInfo callerClientInfo)
		{
			if (!LocalServer.AllowsTokenSerializationBy(callerClientInfo.ClientSecurityContext))
			{
				ProxyEventLogHelper.LogCallerDeniedProxyRight(callerClientInfo.ClientSecurityContext.UserSid);
				ExTraceGlobals.ProxyEvaluatorTracer.TraceDebug<SecurityIdentifier>(0L, "[ProxyHeaderConverter::VerifyTokenSerializationRight] Caller '{0}' sent a proxy request but does not have token serialization rights on this CAS server.", callerClientInfo.ClientSecurityContext.UserSid);
				throw FaultExceptionUtilities.CreateFault(new TokenSerializationDeniedException(), FaultParty.Sender);
			}
		}
	}
}
