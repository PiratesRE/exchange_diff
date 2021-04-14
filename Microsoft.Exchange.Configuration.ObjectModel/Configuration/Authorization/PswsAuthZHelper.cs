using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Security.Principal;
using System.Web;
using Microsoft.Exchange.Configuration.Core;
using Microsoft.Exchange.Configuration.ObjectModel.EventLog;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.CmdletInfra;
using Microsoft.Exchange.Diagnostics.Components.Authorization;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.Security.OAuth;

namespace Microsoft.Exchange.Configuration.Authorization
{
	internal static class PswsAuthZHelper
	{
		internal static IIdentity GetExecutingAuthZUser(UserToken userToken)
		{
			Microsoft.Exchange.Configuration.Core.AuthenticationType authenticationType = userToken.AuthenticationType;
			ExTraceGlobals.PublicPluginAPITracer.TraceDebug<Microsoft.Exchange.Configuration.Core.AuthenticationType>(0L, "[PswsAuthZHelper.GetExecutingAuthZUser] authenticationType = \"{0}\".", authenticationType);
			IIdentity identity = HttpContext.Current.Items["X-Psws-CurrentLogonUser"] as IIdentity;
			if (identity is SidOAuthIdentity)
			{
				AuthZLogger.SafeAppendGenericInfo("PswsLogonUser", "SidOAuthIdentity");
				return identity;
			}
			if (identity is WindowsTokenIdentity)
			{
				AuthZLogger.SafeAppendGenericInfo("PswsLogonUser", "WindowsTokenIdentity");
				return ((WindowsTokenIdentity)identity).ToSerializedIdentity();
			}
			return AuthZPluginHelper.ConstructAuthZUser(userToken, authenticationType);
		}

		internal static PswsAuthZUserToken GetAuthZPluginUserToken(UserToken userToken)
		{
			ExAssert.RetailAssert(userToken != null, "[PswsAuthorization.GetAuthZPluginUserToken] userToken can't be null.");
			Microsoft.Exchange.Configuration.Core.AuthenticationType authenticationType = userToken.AuthenticationType;
			SecurityIdentifier userSid = userToken.UserSid;
			DelegatedPrincipal delegatedPrincipal = userToken.DelegatedPrincipal;
			ExAssert.RetailAssert(userSid != null, "The user sid is invalid (null).");
			PartitionId partitionId = userToken.PartitionId;
			string text = AuthenticatedUserCache.CreateKeyForPsws(userSid, userToken.AuthenticationType, partitionId);
			ExTraceGlobals.PublicPluginAPITracer.TraceDebug<string>(0L, "[PswsAuthZHelper.GetAuthZPluginUserToken] User cache key = \"{0}\".", text);
			AuthZPluginUserToken authZPluginUserToken;
			if (!AuthenticatedUserCache.Instance.TryGetValue(text, out authZPluginUserToken))
			{
				ExTraceGlobals.PublicPluginAPITracer.TraceDebug(0L, "[PswsAuthZHelper.GetAuthZPluginUserToken] User not found in cache.");
				IIdentity identity = HttpContext.Current.Items["X-Psws-CurrentLogonUser"] as IIdentity;
				SerializedIdentity serializedIdentity = null;
				if (identity is WindowsTokenIdentity)
				{
					serializedIdentity = ((WindowsTokenIdentity)identity).ToSerializedIdentity();
				}
				ADRawEntry adrawEntry = ExchangeAuthorizationPlugin.FindUserEntry(userSid, null, serializedIdentity, partitionId);
				ExAssert.RetailAssert(adrawEntry != null, "UnAuthorized. Unable to find the user.");
				bool condition = (adrawEntry is MiniRecipient || adrawEntry is ADUser) && (bool)adrawEntry[ADRecipientSchema.RemotePowerShellEnabled];
				ExAssert.RetailAssert(condition, "UnAuthorized. PSWS not enabled user.");
				authZPluginUserToken = new AuthZPluginUserToken(delegatedPrincipal, adrawEntry, authenticationType, userSid.Value);
				AuthenticatedUserCache.Instance.AddUserToCache(text, authZPluginUserToken);
			}
			return new PswsAuthZUserToken(authZPluginUserToken.DelegatedPrincipal, authZPluginUserToken.UserEntry, authenticationType, authZPluginUserToken.DefaultUserName, userToken.UserName);
		}

		internal static ExchangeRunspaceConfigurationSettings BuildRunspaceConfigurationSettings(string connectionString, UserToken userToken, NameValueCollection collection)
		{
			Uri uri = new Uri(connectionString, UriKind.Absolute);
			ExchangeRunspaceConfigurationSettings exchangeRunspaceConfigurationSettings = ExchangeRunspaceConfigurationSettings.CreateConfigurationSettingsFromNameValueCollection(uri, collection, ExchangeRunspaceConfigurationSettings.ExchangeApplication.PswsClient);
			if (string.IsNullOrEmpty(exchangeRunspaceConfigurationSettings.TenantOrganization))
			{
				exchangeRunspaceConfigurationSettings.TenantOrganization = userToken.ManagedOrganization;
			}
			return exchangeRunspaceConfigurationSettings;
		}

		internal static string GetPswsMembershipId(UserToken userToken, NameValueCollection collection)
		{
			ExAssert.RetailAssert(userToken != null, "[PswsAuthorization.GetPswsMembershipId] userToken can't be null.");
			string friendlyName = userToken.Organization.GetFriendlyName();
			ExchangeRunspaceConfigurationSettings exchangeRunspaceConfigurationSettings = PswsAuthZHelper.BuildRunspaceConfigurationSettings("https://www.outlook.com/Psws/Service.svc", userToken, collection);
			CultureInfo cultureInfo;
			PswsAuthZHelper.TryParseCultureInfo(collection, out cultureInfo);
			string text = userToken.ManagedOrganization;
			if (string.IsNullOrWhiteSpace(text))
			{
				text = exchangeRunspaceConfigurationSettings.TenantOrganization;
			}
			string result = string.Format("Name:{0};AT:{1};UserOrg:{2};ManOrg:{3};SL:{4};FSL:{5};CA:{6};EDK:{7};Cul:{8};Proxy:{9}", new object[]
			{
				PswsAuthZHelper.GetUserNameForCache(userToken),
				userToken.AuthenticationType,
				friendlyName,
				text,
				exchangeRunspaceConfigurationSettings.CurrentSerializationLevel,
				exchangeRunspaceConfigurationSettings.ProxyFullSerialization,
				exchangeRunspaceConfigurationSettings.ClientApplication,
				exchangeRunspaceConfigurationSettings.EncodeDecodeKey,
				(cultureInfo == null) ? "null" : cultureInfo.Name,
				exchangeRunspaceConfigurationSettings.IsProxy
			});
			AuthZLogger.SafeSetLogger(PswsMetadata.IsProxy, exchangeRunspaceConfigurationSettings.IsProxy);
			AuthZLogger.SafeSetLogger(PswsMetadata.ClientApplication, exchangeRunspaceConfigurationSettings.ClientApplication);
			AuthZLogger.SafeSetLogger(PswsMetadata.ProxyFullSerialzation, exchangeRunspaceConfigurationSettings.ProxyFullSerialization);
			AuthZLogger.SafeSetLogger(PswsMetadata.SerializationLevel, exchangeRunspaceConfigurationSettings.CurrentSerializationLevel);
			AuthZLogger.SafeSetLogger(PswsMetadata.CultureInfo, (cultureInfo == null) ? "null" : cultureInfo.Name);
			AuthZLogger.SafeSetLogger(PswsMetadata.TenantOrganization, text);
			return result;
		}

		internal static bool TryParseCultureInfo(NameValueCollection headers, out CultureInfo cultureInfo)
		{
			cultureInfo = null;
			string text = headers.Get("X-CultureInfo");
			if (!string.IsNullOrWhiteSpace(text))
			{
				try
				{
					cultureInfo = new CultureInfo(text);
					return true;
				}
				catch (CultureNotFoundException ex)
				{
					ExTraceGlobals.RunspaceConfigTracer.TraceError<string, CultureNotFoundException>(0L, "[PswsAuthZHelper.TryParseCultureInfo] Invalid culture info \"{0}\". Exception: {1}", text, ex);
					TaskLogger.LogRbacEvent(TaskEventLogConstants.Tuple_InvalidCultureInfo, text, new object[]
					{
						text,
						ex.ToString()
					});
					AuthZLogger.SafeAppendGenericError("InvalidCultureInfo", text, false);
				}
				return false;
			}
			return false;
		}

		private static string GetUserNameForCache(UserToken userToken)
		{
			if (userToken.UserSid != null)
			{
				return userToken.UserSid.ToString();
			}
			return userToken.UserName;
		}

		private const string MembershipIdFormat = "Name:{0};AT:{1};UserOrg:{2};ManOrg:{3};SL:{4};FSL:{5};CA:{6};EDK:{7};Cul:{8};Proxy:{9}";

		private const string DummyUri = "https://www.outlook.com/Psws/Service.svc";

		internal const string EncodeDecodeKeyHeaderName = "X-EncodeDecode-Key";

		internal const string CultureInfoHeaderKey = "X-CultureInfo";

		internal const string CurrentLogonUserKey = "X-Psws-CurrentLogonUser";
	}
}
