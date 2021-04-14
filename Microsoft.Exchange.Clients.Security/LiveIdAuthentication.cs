using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Web;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.Net.Protocols;
using Microsoft.Exchange.Security;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Passport.RPS;

namespace Microsoft.Exchange.Clients.Security
{
	internal static class LiveIdAuthentication
	{
		public static bool IsInitialized
		{
			get
			{
				return LiveIdAuthentication.rpsOrgIdSession != null;
			}
		}

		public static void Initialize(string virtualDirectoryNameParam, bool sslOffloadedParam)
		{
			if (!string.IsNullOrEmpty(virtualDirectoryNameParam) && (virtualDirectoryNameParam.StartsWith("/", StringComparison.OrdinalIgnoreCase) || virtualDirectoryNameParam.EndsWith("/", StringComparison.OrdinalIgnoreCase)))
			{
				throw new ArgumentException("virtualDirectoryNameParam should not contain leading or trailing slashes", "virtualDirectoryNameParam");
			}
			if (!string.IsNullOrEmpty(virtualDirectoryNameParam))
			{
				LiveIdAuthentication.virtualDirectoryNameWithLeadingSlash = "/" + virtualDirectoryNameParam;
			}
			try
			{
				RPS rps = new RPS();
				rps.Initialize(null);
				LiveIdAuthentication.rpsOrgIdSession = rps;
			}
			catch (COMException e)
			{
				LiveIdAuthentication.rpsOrgIdSession = null;
				LiveIdErrorHandler.ThrowRPSException(e);
			}
			LiveIdAuthentication.sslOffloaded = sslOffloadedParam;
		}

		internal static string GetCurrentEnvironment(bool useConsumerRps)
		{
			string result;
			if (useConsumerRps)
			{
				if (LiveIdAuthentication.consumerCurrentEnvironment == null)
				{
					using (RPSHttpAuthClient rpshttpAuthClient = LiveIdAuthentication.CreateRPSClient(true))
					{
						LiveIdAuthentication.consumerCurrentEnvironment = rpshttpAuthClient.GetCurrentEnvironment();
					}
				}
				result = LiveIdAuthentication.consumerCurrentEnvironment;
			}
			else
			{
				if (LiveIdAuthentication.enterpriseCurrentEnvironment == null)
				{
					using (RPSHttpAuthClient rpshttpAuthClient2 = LiveIdAuthentication.CreateRPSClient(false))
					{
						LiveIdAuthentication.enterpriseCurrentEnvironment = rpshttpAuthClient2.GetCurrentEnvironment();
					}
				}
				result = LiveIdAuthentication.enterpriseCurrentEnvironment;
			}
			return result;
		}

		private static string GetSiteProperty(string siteName, string siteProperty, bool useConsumerRps)
		{
			string siteProperty2;
			using (RPSHttpAuthClient rpshttpAuthClient = LiveIdAuthentication.CreateRPSClient(useConsumerRps))
			{
				siteProperty2 = rpshttpAuthClient.GetSiteProperty(siteName, siteProperty);
			}
			return siteProperty2;
		}

		public static string GetDefaultReturnUrl(string siteName, bool useConsumerRps)
		{
			if (siteName == null)
			{
				throw new ArgumentNullException("siteName");
			}
			return LiveIdAuthentication.GetSiteProperty(siteName, "ReturnURL", useConsumerRps);
		}

		public static void Shutdown()
		{
			if (LiveIdAuthentication.rpsOrgIdSession != null)
			{
				LiveIdAuthentication.rpsOrgIdSession.Shutdown();
				LiveIdAuthentication.rpsOrgIdSession = null;
			}
		}

		public static bool Authenticate(HttpContext httpContext, string siteName, string authPolicyOverrideValue, string[] memberNameIgnorePrefixes, bool useConsumerRps, out string puid, out string orgIdPuid, out string cid, out string membername, out uint issueTime, out uint loginAttributes, out string responseHeaders, out uint rpsTicketType, out RPSTicket deprecatedRpsTicketObject, out bool hasAcceptedAccrual, out uint rpsAuthState, out bool isOrgIdFederatedMsaIdentity)
		{
			if (!LiveIdAuthentication.IsInitialized)
			{
				throw new InvalidOperationException(Strings.ComponentNotInitialized);
			}
			if (siteName == null)
			{
				throw new ArgumentNullException("siteName");
			}
			hasAcceptedAccrual = false;
			puid = null;
			orgIdPuid = null;
			cid = null;
			membername = null;
			issueTime = 0U;
			loginAttributes = 0U;
			responseHeaders = null;
			rpsTicketType = 0U;
			deprecatedRpsTicketObject = null;
			rpsAuthState = 0U;
			isOrgIdFederatedMsaIdentity = false;
			RPSPropBag rpspropBag = null;
			string text = httpContext.Request.QueryString["f"];
			if (!string.IsNullOrEmpty(text))
			{
				ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceError<string>(0L, "Querystring contains F-code: {0}.", text);
				return false;
			}
			try
			{
				if (!useConsumerRps)
				{
					rpspropBag = new RPSPropBag(LiveIdAuthentication.rpsOrgIdSession);
				}
				RPSProfile rpsprofile = null;
				using (RPSHttpAuthClient rpshttpAuthClient = LiveIdAuthentication.CreateRPSClient(useConsumerRps))
				{
					int? rpsErrorCode;
					string rpsErrorString;
					rpsprofile = rpshttpAuthClient.Authenticate(siteName, authPolicyOverrideValue, LiveIdAuthentication.sslOffloaded, httpContext.Request, rpspropBag, out rpsErrorCode, out rpsErrorString, out deprecatedRpsTicketObject);
					LiveIdAuthentication.ValidateRpsCallAndThrowOnFailure(rpsErrorCode, rpsErrorString);
				}
				if (rpsprofile == null)
				{
					return false;
				}
				if (!useConsumerRps && deprecatedRpsTicketObject != null)
				{
					try
					{
						using (RPSPropBag rpspropBag2 = new RPSPropBag(LiveIdAuthentication.rpsOrgIdSession))
						{
							rpspropBag2["SlidingWindow"] = 0;
							if (!string.IsNullOrEmpty(authPolicyOverrideValue))
							{
								rpspropBag2["AuthPolicy"] = authPolicyOverrideValue;
							}
							if (!deprecatedRpsTicketObject.Validate(rpspropBag2))
							{
								return false;
							}
						}
					}
					catch (COMException ex)
					{
						ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceError<COMException>(0L, "Failed to validate ticket: {0}.", ex);
						LiveIdErrorHandler.ThrowRPSException(ex);
					}
				}
				rpsAuthState = rpsprofile.RPSAuthState;
				rpsTicketType = rpsprofile.TicketType;
				if (LiveIdAuthenticationModule.AppPasswordCheckEnabled && !httpContext.Request.Url.AbsolutePath.StartsWith("/owa/", StringComparison.OrdinalIgnoreCase) && rpsprofile.AppPassword)
				{
					AppPasswordAccessException exception = new AppPasswordAccessException();
					httpContext.Response.AppendToLog("&AppPasswordBlocked");
					Utilities.HandleException(httpContext, exception, false);
				}
				hasAcceptedAccrual = LiveIdAuthentication.HasAcceptedAccruals(rpsprofile);
				orgIdPuid = rpsprofile.HexPuid;
				cid = (string.IsNullOrWhiteSpace(rpsprofile.ConsumerCID) ? rpsprofile.HexCID : rpsprofile.ConsumerCID);
				puid = (string.IsNullOrWhiteSpace(rpsprofile.ConsumerPuid) ? orgIdPuid : rpsprofile.ConsumerPuid);
				membername = rpsprofile.MemberName;
				string text2;
				if (LiveIdAuthentication.TryRemoveMemberNamePrefixes(membername, memberNameIgnorePrefixes, out text2))
				{
					membername = text2;
					isOrgIdFederatedMsaIdentity = true;
				}
				issueTime = rpsprofile.IssueInstant;
				loginAttributes = rpsprofile.LoginAttributes;
				string text3 = loginAttributes.ToString();
				httpContext.Response.AppendToLog("&loginAttributes=" + text3);
				if (!string.IsNullOrWhiteSpace(text3))
				{
					httpContext.Response.AppendToLog(string.Format("loginAttributes={0}", text3));
					httpContext.Request.Headers.Add("X-LoginAttributes", text3);
				}
				responseHeaders = rpsprofile.ResponseHeader;
			}
			finally
			{
				if (rpspropBag != null)
				{
					rpspropBag.Dispose();
				}
			}
			return true;
		}

		public static bool ValidateWithSlidingWindow(RPSTicket rpsTicket, TimeSpan slidingWindow)
		{
			RPSPropBag rpspropBag = null;
			try
			{
				rpspropBag = new RPSPropBag(LiveIdAuthentication.rpsOrgIdSession);
				rpspropBag["SlidingWindow"] = slidingWindow.TotalSeconds;
				if (!rpsTicket.Validate(rpspropBag))
				{
					int num = (int)rpspropBag["ReasonHR"];
					if (num == -2147184087)
					{
						return false;
					}
				}
			}
			catch (COMException e)
			{
				LiveIdErrorHandler.ThrowRPSException(e);
			}
			finally
			{
				if (rpspropBag != null)
				{
					rpspropBag.Dispose();
				}
			}
			return true;
		}

		public static void Logout(HttpContext httpContext, string siteName, bool useConsumerRps)
		{
			using (RPSHttpAuthClient rpshttpAuthClient = LiveIdAuthentication.CreateRPSClient(useConsumerRps))
			{
				int? rpsErrorCode = null;
				string rpsErrorString = null;
				string logoutHeaders = rpshttpAuthClient.GetLogoutHeaders(siteName, out rpsErrorCode, out rpsErrorString);
				LiveIdAuthentication.ValidateRpsCallAndThrowOnFailure(rpsErrorCode, rpsErrorString);
				LiveIdAuthentication.WriteHeadersToResponse(httpContext, logoutHeaders, useConsumerRps);
			}
		}

		public static void WriteHeadersToResponse(HttpContext httpContext, string headers, bool useConsumerRps)
		{
			HttpResponse response = httpContext.Response;
			if (!"no-cache".Equals(response.CacheControl, StringComparison.OrdinalIgnoreCase) && !"no-store".Equals(response.CacheControl, StringComparison.OrdinalIgnoreCase) && !"private".Equals(response.CacheControl, StringComparison.OrdinalIgnoreCase))
			{
				response.Cache.SetCacheability(HttpCacheability.NoCache, "set-cookie");
			}
			try
			{
				using (RPSHttpAuth rpshttpAuth = new RPSHttpAuth(LiveIdAuthentication.rpsOrgIdSession))
				{
					if (AuthCommon.IsFrontEnd || CafeHelper.IsFromNativeProxy(httpContext.Request))
					{
						rpshttpAuth.WriteHeaders(response, headers);
					}
					else
					{
						response.SetCookie(new HttpCookie("CopyLiveIdAuthCookieFromBE", HttpUtility.UrlEncode(headers)));
					}
				}
			}
			catch (COMException e)
			{
				LiveIdErrorHandler.ThrowRPSException(e);
			}
		}

		private static string GetRedirectUrl(LiveIdAuthentication.RedirectType rt, string siteName, string returnUrl, string authPolicy, bool useConsumerRps)
		{
			string constructUrlParam;
			if (rt == LiveIdAuthentication.RedirectType.Logout)
			{
				constructUrlParam = "Logout";
			}
			else if (rt == LiveIdAuthentication.RedirectType.SilentAuthenticate)
			{
				constructUrlParam = "SilentAuth";
			}
			else
			{
				constructUrlParam = "Auth";
			}
			string formattedReturnUrl;
			if (!LiveIdAuthentication.TryFormatUrl(returnUrl, out formattedReturnUrl))
			{
				formattedReturnUrl = returnUrl;
			}
			string result;
			using (RPSHttpAuthClient rpshttpAuthClient = LiveIdAuthentication.CreateRPSClient(useConsumerRps))
			{
				int? rpsErrorCode = null;
				string rpsErrorString = null;
				string redirectUrl = rpshttpAuthClient.GetRedirectUrl(constructUrlParam, siteName, formattedReturnUrl, authPolicy, out rpsErrorCode, out rpsErrorString);
				LiveIdAuthentication.ValidateRpsCallAndThrowOnFailure(rpsErrorCode, rpsErrorString);
				result = redirectUrl;
			}
			return result;
		}

		private static bool HasAcceptedAccruals(RPSProfile ticket)
		{
			string consumerPuid = ticket.ConsumerPuid;
			bool flag;
			bool flag2;
			bool flag3;
			if (string.IsNullOrEmpty(consumerPuid))
			{
				int tokenFlags = ticket.TokenFlags;
				flag = ((tokenFlags & 536870912) == 0 || (tokenFlags & 16384) == 0);
				flag2 = ((tokenFlags & 128) != 0);
				flag3 = ((tokenFlags & 32) != 0 || (tokenFlags & 64) != 0);
			}
			else
			{
				flag = Convert.ToBoolean(ticket.HasSignedTOU);
				flag2 = Convert.ToBoolean(ticket.ConsumerChild);
				string consumerConsentLevel = ticket.ConsumerConsentLevel;
				flag3 = ("FULL".Equals(consumerConsentLevel) || "PARTIAL".Equals(consumerConsentLevel));
			}
			return flag && (!flag2 || flag3);
		}

		public static string GetAuthenticateRedirectUrl(string returnUrl, string siteName, string authPolicy, string federatedDomain, string userName, bool addCBCXT, bool useSilentAuthentication, bool useConsumerRps)
		{
			if (useSilentAuthentication)
			{
				returnUrl += ((returnUrl.IndexOf('?') == -1) ? "?" : "&");
				returnUrl += "silent=1";
			}
			string text = LiveIdAuthentication.GetRedirectUrl(useSilentAuthentication ? LiveIdAuthentication.RedirectType.SilentAuthenticate : LiveIdAuthentication.RedirectType.Authenticate, siteName, returnUrl, authPolicy, useConsumerRps);
			if (!string.IsNullOrEmpty(federatedDomain))
			{
				ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceDebug<string>(0L, "Append whr parameter {0} for live authentication to bypass the 'go there' experience", federatedDomain);
				text = text + "&whr=" + HttpUtility.UrlEncode(federatedDomain);
			}
			if (!string.IsNullOrEmpty(userName))
			{
				string text2 = text;
				text = string.Concat(new string[]
				{
					text2,
					"&",
					Utilities.UserNameParameter,
					"=",
					userName
				});
			}
			if (addCBCXT)
			{
				text += "&CBCXT=out";
			}
			return text;
		}

		public static string GetLiveLogoutRedirectUrl(string returnUrl, string siteName, bool useConsumerRps)
		{
			return LiveIdAuthentication.GetRedirectUrl(LiveIdAuthentication.RedirectType.Logout, siteName, returnUrl, null, useConsumerRps);
		}

		private static bool TryFormatUrl(string url, out string formattedUrl)
		{
			formattedUrl = string.Empty;
			Uri uri = new Uri(url);
			if (!string.IsNullOrEmpty(LiveIdAuthentication.virtualDirectoryNameWithLeadingSlash) && uri.LocalPath.EndsWith(LiveIdAuthentication.virtualDirectoryNameWithLeadingSlash, StringComparison.OrdinalIgnoreCase))
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("https://");
				stringBuilder.Append(uri.Host);
				stringBuilder.Append(uri.LocalPath);
				stringBuilder.Append("/");
				stringBuilder.Append(uri.Query);
				formattedUrl = stringBuilder.ToString();
				return true;
			}
			if (url.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
			{
				StringBuilder stringBuilder2 = new StringBuilder();
				stringBuilder2.Append("https://");
				stringBuilder2.Append(uri.Host);
				stringBuilder2.Append(uri.PathAndQuery);
				formattedUrl = stringBuilder2.ToString();
				return true;
			}
			return false;
		}

		public static void DeleteCookie(HttpResponse response, string name)
		{
			if (response == null)
			{
				throw new ArgumentNullException("response");
			}
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentException("name can not be null or empty string");
			}
			bool flag = false;
			for (int i = 0; i < response.Cookies.Count; i++)
			{
				HttpCookie httpCookie = response.Cookies[i];
				if (httpCookie.Name != null && string.Equals(httpCookie.Name, name, StringComparison.OrdinalIgnoreCase))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				response.Cookies.Add(new HttpCookie(name, string.Empty));
			}
			response.Cookies[name].Expires = DateTime.UtcNow.AddYears(-30);
		}

		internal static bool TryRemoveMemberNamePrefixes(string memberName, IEnumerable<string> memberNameIgnorePrefixes, out string normalizedMemberName)
		{
			if (string.IsNullOrWhiteSpace(memberName))
			{
				throw new ArgumentNullException("memberName", "memberName cannot be null or empty.");
			}
			bool result = false;
			normalizedMemberName = memberName;
			ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceDebug<string>(0L, "OrgID returned RPSMembername '{0}'.", memberName);
			if (memberNameIgnorePrefixes != null)
			{
				foreach (string text in memberNameIgnorePrefixes.OrderBy((string s) => s.Length, Comparer<int>.Create((int len1, int len2) => len2.CompareTo(len1))))
				{
					ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceDebug<string>(0L, "Attempting to remove prefix '{0}'.", text);
					if (normalizedMemberName.IndexOf(text, StringComparison.OrdinalIgnoreCase) == 0)
					{
						normalizedMemberName = normalizedMemberName.Remove(0, text.Length).Trim();
						ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceDebug<string>(0L, "Normalized RPSMembername to '{0}'.", normalizedMemberName);
						result = true;
						break;
					}
				}
			}
			return result;
		}

		private static void ValidateRpsCallAndThrowOnFailure(int? rpsErrorCode, string rpsErrorString)
		{
			try
			{
				if (rpsErrorCode != null)
				{
					rpsErrorString = (string.IsNullOrWhiteSpace(rpsErrorString) ? "An error occurred calling RPS" : rpsErrorString);
					ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceDebug<int, string>(0L, "RPSHttpAuthClient failed with error code {0} and message {1}.", rpsErrorCode.Value, rpsErrorString);
					throw new COMException(rpsErrorString, rpsErrorCode.Value);
				}
			}
			catch (COMException e)
			{
				LiveIdErrorHandler.ThrowRPSException(e);
			}
		}

		private static RPSHttpAuthClient CreateRPSClient(bool useConsumerRps)
		{
			int maxValue = int.MaxValue;
			return new RPSHttpAuthClient(useConsumerRps, LiveIdAuthentication.rpsOrgIdSession, maxValue);
		}

		internal const string CopyLiveIdAuthCookieName = "CopyLiveIdAuthCookieFromBE";

		internal const string RPSConstructURLLogout = "Logout";

		internal const string RPSHexCID = "HexCID";

		internal const string RPSMembername = "Membername";

		internal const string RPSConstructURLAuth = "Auth";

		internal const string RPSConstructURLSilentAuth = "SilentAuth";

		internal const string RPSReturnURL = "ReturnURL";

		internal const string RPSAuthPolicy = "AuthPolicy";

		internal const string RPSAuthInstant = "AuthInstant";

		internal const string RPSIssueInstant = "IssueInstant";

		internal const string RPSConsumerTOUAccepted = "ConsumerTOUAccepted";

		internal const string RPSConsumerChild = "ConsumerChild";

		internal const string RPSConsumerChildConsent = "ConsumerConsentLevel";

		internal const string RPSConsumerPUID = "ConsumerPUID";

		internal const string RPSConsumerCID = "ConsumerCID";

		internal const string RPSLoginAttributes = "LoginAttributes";

		private const string RPSRespHeaders = "RPSRespHeaders";

		private const string WHRParameter = "whr";

		internal const string AuthPolicy_MBI_KEY = "MBI_KEY";

		internal const string AuthPolicy_MBI_SSL = "MBI_SSL";

		internal const string AuthPolicy_MBI_SSL_60SECTEST = "MBI_SSL_60SECTEST";

		private const string CBCXTParameter = "CBCXT=out";

		private const string ConsentLevelNone = "NONE";

		private const string ConsentLevelPartial = "PARTIAL";

		private const string ConsentLevelFull = "FULL";

		private const string HttpPrefix = "http://";

		private const string HttpsPrefix = "https://";

		private const int AccrualTouBitMask = 16384;

		private const int AccrualMsnTouBitMask = 536870912;

		private const int AccrualParentalLimitedConsentBitMask = 32;

		private const int AccrualParentalFullConsentBitMask = 64;

		private const int AccrualIsChildBitMask = 128;

		private const string ForwardSlash = "/";

		private const string RPSPropBagHttps = "HTTPS";

		private const int SlidingWindowExpired = -2147184087;

		private const string SlidingWindow = "SlidingWindow";

		private const string ReasonHR = "ReasonHR";

		private const string NoCacheHeader = "no-cache";

		private const string NoStoreCacheHeader = "no-store";

		private const string PrivateCacheHeader = "private";

		private static RPS rpsOrgIdSession;

		private static bool sslOffloaded;

		private static string enterpriseCurrentEnvironment;

		private static string consumerCurrentEnvironment;

		private static string virtualDirectoryNameWithLeadingSlash;

		internal enum RPSTicketType
		{
			Compact = 2,
			RPSAuth,
			RPSSecAuth
		}

		private enum RedirectType
		{
			Authenticate,
			Logout,
			SilentAuthenticate
		}

		internal enum RPSAuthState
		{
			No,
			Yes,
			Maybe
		}
	}
}
