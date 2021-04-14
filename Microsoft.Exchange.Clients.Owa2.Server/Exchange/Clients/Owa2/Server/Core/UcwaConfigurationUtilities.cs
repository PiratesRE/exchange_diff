using System;
using System.Text;
using Microsoft.Exchange.Clients.Owa2.Server.Diagnostics;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.Security.OAuth;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.OnlineMeetings;
using Microsoft.Exchange.Services.OnlineMeetings.Autodiscover;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal static class UcwaConfigurationUtilities
	{
		public static UcwaUserConfiguration GetUcwaUserConfiguration(string sipUri, CallContext callContext)
		{
			callContext.ProtocolLog.Set(GetUcwaUserConfigurationMetaData.IsTaskCompleted, bool.FalseString);
			callContext.ProtocolLog.Set(GetUcwaUserConfigurationMetaData.IsUcwaSupported, bool.FalseString);
			UcwaUserConfiguration ucwaUserConfiguration = new UcwaUserConfiguration();
			ucwaUserConfiguration.SipUri = sipUri;
			if (string.IsNullOrEmpty(sipUri))
			{
				callContext.ProtocolLog.Set(GetUcwaUserConfigurationMetaData.IsTaskCompleted, bool.TrueString);
				throw new OwaInvalidRequestException("No sipUri specified");
			}
			UserContext userContext = UserContextManager.GetUserContext(callContext.HttpContext, callContext.EffectiveCaller, true);
			if (string.Compare(userContext.SipUri, sipUri, StringComparison.OrdinalIgnoreCase) != 0)
			{
				callContext.ProtocolLog.Set(GetUcwaUserConfigurationMetaData.ManagerSipUri, ExtensibleLogger.FormatPIIValue(sipUri));
			}
			if (userContext.ExchangePrincipal.MailboxInfo.OrganizationId != null && userContext.ExchangePrincipal.MailboxInfo.OrganizationId.OrganizationalUnit != null)
			{
				callContext.ProtocolLog.Set(GetUcwaUserConfigurationMetaData.Organization, userContext.ExchangePrincipal.MailboxInfo.OrganizationId.OrganizationalUnit.Name);
			}
			OAuthCredentials oauthCredential;
			try
			{
				oauthCredential = UcwaConfigurationUtilities.GetOAuthCredential(sipUri);
				oauthCredential.ClientRequestId = new Guid?(Guid.NewGuid());
				callContext.ProtocolLog.Set(GetUcwaUserConfigurationMetaData.OAuthCorrelationId, oauthCredential.ClientRequestId.Value.ToString());
			}
			catch (OwaException ex)
			{
				string text = UcwaConfigurationUtilities.BuildFailureLogString(ex);
				callContext.ProtocolLog.Set(GetUcwaUserConfigurationMetaData.Exceptions, text);
				ExTraceGlobals.OnlineMeetingTracer.TraceError<string>(0L, "[UcwaConfigurationUtilities.GetUcwaUserConfiguration] An error occurred while obtaining OAuth credential; Exception: {0}", text);
				ucwaUserConfiguration.DiagnosticInfo = text;
				return ucwaUserConfiguration;
			}
			AutodiscoverResult ucwaDiscoveryUrl = AutodiscoverWorker.GetUcwaDiscoveryUrl(sipUri, oauthCredential);
			callContext.ProtocolLog.Set(GetUcwaUserConfigurationMetaData.AuthenticatedLyncAutodiscoverServer, ucwaDiscoveryUrl.AuthenticatedLyncAutodiscoverServer);
			callContext.ProtocolLog.Set(GetUcwaUserConfigurationMetaData.IsAuthdServerFromCache, ucwaDiscoveryUrl.IsAuthdServerFromCache ? bool.TrueString : bool.FalseString);
			callContext.ProtocolLog.Set(GetUcwaUserConfigurationMetaData.IsUcwaUrlFromCache, ucwaDiscoveryUrl.IsUcwaUrlFromCache ? bool.TrueString : bool.FalseString);
			callContext.ProtocolLog.Set(GetUcwaUserConfigurationMetaData.UnauthenticatedRedirectHops, string.Join(";", ucwaDiscoveryUrl.UnauthenticatedRedirects));
			callContext.ProtocolLog.Set(GetUcwaUserConfigurationMetaData.AuthenticatedRedirectHops, string.Join(";", ucwaDiscoveryUrl.AuthenticatedRedirects));
			callContext.ProtocolLog.Set(GetUcwaUserConfigurationMetaData.WorkerExceptions, ucwaDiscoveryUrl.BuildFailureString());
			if (!ucwaDiscoveryUrl.IsOnlineMeetingEnabled)
			{
				ExTraceGlobals.OnlineMeetingTracer.TraceInformation(0, 0L, "[UcwaConfigurationUtilities.GetUcwaUserConfiguration] Online meetings are not enabled in this topology");
				ucwaUserConfiguration.DiagnosticInfo = "Online meetings not enabled on the server";
				return ucwaUserConfiguration;
			}
			if (ucwaDiscoveryUrl.HasError)
			{
				Exception exception = ucwaDiscoveryUrl.Error.Exception;
				StringBuilder stringBuilder = new StringBuilder(ucwaDiscoveryUrl.Error.FailureStep.ToString() + "_");
				stringBuilder.Append(UcwaConfigurationUtilities.BuildFailureLogString(exception));
				stringBuilder.Append(ucwaDiscoveryUrl.Error.ResponseFailureReason ?? string.Empty);
				string text2 = stringBuilder.ToString();
				callContext.ProtocolLog.Set(GetUcwaUserConfigurationMetaData.Exceptions, text2);
				callContext.ProtocolLog.Set(GetUcwaUserConfigurationMetaData.RequestHeaders, ucwaDiscoveryUrl.Error.RequestHeaders);
				callContext.ProtocolLog.Set(GetUcwaUserConfigurationMetaData.ResponseHeaders, ucwaDiscoveryUrl.Error.ResponseHeaders);
				callContext.ProtocolLog.Set(GetUcwaUserConfigurationMetaData.ResponseBody, ucwaDiscoveryUrl.Error.ResponseBody);
				if (ucwaDiscoveryUrl.IsAuthdServerFromCache)
				{
					AutodiscoverCache.InvalidateDomain(OnlineMeetingHelper.GetSipDomain(sipUri));
					callContext.ProtocolLog.Set(GetUcwaUserConfigurationMetaData.CacheOperation, AutodiscoverCacheOperation.InvalidateDomain.ToString());
				}
				ExTraceGlobals.OnlineMeetingTracer.TraceError<string>(0L, "[UcwaConfigurationUtilities.GetUcwaUserConfiguration] An error occured while fetching UCWA configuration; Exception: {0}", text2);
				ucwaUserConfiguration.DiagnosticInfo = text2;
				return ucwaUserConfiguration;
			}
			callContext.ProtocolLog.Set(GetUcwaUserConfigurationMetaData.ResponseBody, ucwaDiscoveryUrl.ResponseBody);
			ucwaUserConfiguration.IsUcwaSupported = ucwaDiscoveryUrl.IsUcwaSupported;
			ucwaUserConfiguration.UcwaUrl = ucwaDiscoveryUrl.UcwaDiscoveryUrl;
			callContext.ProtocolLog.Set(GetUcwaUserConfigurationMetaData.IsTaskCompleted, bool.TrueString);
			if (ucwaUserConfiguration.IsUcwaSupported)
			{
				callContext.ProtocolLog.Set(GetUcwaUserConfigurationMetaData.IsUcwaSupported, bool.TrueString);
				callContext.ProtocolLog.Set(GetUcwaUserConfigurationMetaData.UcwaUrl, ucwaUserConfiguration.UcwaUrl);
			}
			return ucwaUserConfiguration;
		}

		internal static OAuthCredentials GetOAuthCredential(string sipUri)
		{
			ProxyAddress proxyAddress = ProxyAddress.Parse(sipUri);
			string sipDomain = OnlineMeetingHelper.GetSipDomain(sipUri);
			if (string.IsNullOrEmpty(sipDomain))
			{
				ExTraceGlobals.OnlineMeetingTracer.TraceError<string>(0L, "[UcwaConfigurationUtilities.GetOAuthCredential] Unable to determine domain from sip uri: {0}", sipUri);
				throw new OwaException(string.Format("Unable to determine domain from sip uri: {0}", sipUri));
			}
			MiniRecipient miniRecipient = null;
			IRecipientSession recipientSession = null;
			try
			{
				recipientSession = UserContextUtilities.CreateScopedRecipientSession(true, ConsistencyMode.FullyConsistent, sipDomain, null);
			}
			catch (OwaADObjectNotFoundException innerException)
			{
				throw new OwaException(string.Format("Couldn't create a scoped recipient session for {0}", sipDomain), innerException);
			}
			try
			{
				miniRecipient = recipientSession.FindByProxyAddress<MiniRecipient>(proxyAddress);
				if (miniRecipient == null)
				{
					ExTraceGlobals.OnlineMeetingTracer.TraceError<string>(0L, "[UcwaConfigurationUtilities.GetOAuthCredential] IRecipientSession.FindByProxyAddress() unable to find to recipient with address: {0}", sipUri);
					throw new OwaException(string.Format("Couldn't find a match for {0}", proxyAddress.ToString()));
				}
			}
			catch (NonUniqueRecipientException innerException2)
			{
				ExTraceGlobals.OnlineMeetingTracer.TraceError<string>(0L, "[UcwaConfigurationUtilities.GetOAuthCredential] Couldn't find a unique match for: {0}", sipUri);
				throw new OwaException(string.Format("Couldn't find a unique match for {0}", proxyAddress.ToString()), innerException2);
			}
			return OAuthCredentials.GetOAuthCredentialsForAppActAsToken(miniRecipient.OrganizationId, miniRecipient, sipDomain);
		}

		internal static string BuildFailureLogString(Exception ex)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (ex != null)
			{
				stringBuilder.Append(ex.GetType().Name);
				if (ex.InnerException != null)
				{
					stringBuilder.AppendFormat("_{0}_{1}", ex.InnerException.GetType().Name, ex.InnerException.Message);
				}
				else
				{
					stringBuilder.AppendFormat("_NoInnerException_{0}", ex.Message);
				}
			}
			return stringBuilder.ToString();
		}
	}
}
