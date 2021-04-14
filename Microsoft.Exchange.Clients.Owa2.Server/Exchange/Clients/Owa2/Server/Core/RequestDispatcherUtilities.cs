using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Net;
using System.Text;
using System.Web;
using Microsoft.Exchange.Clients.Common;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	public class RequestDispatcherUtilities
	{
		public static bool IsOwaUrl(Uri requestUrl, OwaUrl owaUrl, bool exactMatch)
		{
			return RequestDispatcherUtilities.IsOwaUrl(requestUrl, owaUrl, exactMatch, true);
		}

		public static bool IsOwaUrl(Uri requestUrl, OwaUrl owaUrl, bool exactMatch, bool useLocal)
		{
			if (requestUrl == null)
			{
				throw new ArgumentNullException("requestUrl");
			}
			if (owaUrl == null)
			{
				throw new ArgumentNullException("owaUrl");
			}
			int length = owaUrl.ImplicitUrl.Length;
			string text = useLocal ? requestUrl.LocalPath : requestUrl.PathAndQuery;
			bool flag = string.Compare(text, 0, owaUrl.ImplicitUrl, 0, length, StringComparison.OrdinalIgnoreCase) == 0;
			if (exactMatch)
			{
				flag = (flag && length == text.Length);
			}
			return flag;
		}

		public static OwaRequestType GetRequestType(HttpRequest request)
		{
			OwaRequestType result;
			if (Globals.IsAnonymousCalendarApp && RequestDispatcherUtilities.IsOwaUrl(request.Url, OwaUrl.PublishedCalendar, true))
			{
				result = OwaRequestType.PublishedCalendarView;
			}
			else if (Globals.IsAnonymousCalendarApp && RequestDispatcherUtilities.IsOwaUrl(request.Url, OwaUrl.PublishedICal, true))
			{
				result = OwaRequestType.ICalHttpHandler;
			}
			else if (Utility.IsResourceRequest(request.Url.LocalPath))
			{
				result = OwaRequestType.Resource;
			}
			else if (RequestDispatcherUtilities.IsOwaUrl(request.Url, OwaUrl.OehUrl, true))
			{
				result = OwaRequestType.Oeh;
			}
			else if (RequestDispatcherUtilities.IsOwaUrl(request.Url, OwaUrl.LanguagePage, true))
			{
				result = OwaRequestType.LanguagePage;
			}
			else if (RequestDispatcherUtilities.IsOwaUrl(request.Url, OwaUrl.Default14Page, true))
			{
				result = OwaRequestType.Form15;
			}
			else if (RequestDispatcherUtilities.IsOwaUrl(request.Url, OwaUrl.AttachmentHandler, true))
			{
				result = OwaRequestType.Attachment;
			}
			else if (UrlUtilities.IsWacRequest(request))
			{
				result = OwaRequestType.WopiRequest;
			}
			else if (RequestDispatcherUtilities.IsOwaUrl(request.Url, OwaUrl.LanguagePost, true))
			{
				result = OwaRequestType.LanguagePost;
			}
			else if (RequestDispatcherUtilities.IsOwaUrl(request.Url, OwaUrl.LogoffOwa, true))
			{
				result = OwaRequestType.Logoff;
			}
			else if (RequestDispatcherUtilities.IsOwaUrl(request.Url, OwaUrl.ProxyLogon, true))
			{
				result = OwaRequestType.ProxyLogon;
			}
			else if (RequestDispatcherUtilities.IsOwaUrl(request.Url, OwaUrl.ProxyPing, true))
			{
				result = OwaRequestType.ProxyPing;
			}
			else if (RequestDispatcherUtilities.IsOwaUrl(request.Url, OwaUrl.KeepAlive, true))
			{
				result = OwaRequestType.KeepAlive;
			}
			else if (RequestDispatcherUtilities.IsOwaUrl(request.Url, OwaUrl.GroupSubscription, true))
			{
				result = OwaRequestType.GroupSubscriptionRequest;
			}
			else if (UrlUtilities.IsRemoteNotificationRequest(request))
			{
				result = OwaRequestType.RemoteNotificationRequest;
			}
			else if (RequestDispatcherUtilities.IsOwaUrl(request.Url, OwaUrl.SuiteServiceProxyPage, true))
			{
				result = OwaRequestType.SuiteServiceProxyPage;
			}
			else if (request.Url.LocalPath.EndsWith(".owa") && string.Equals(request.Params["ns"], "WebReady", StringComparison.InvariantCultureIgnoreCase))
			{
				result = OwaRequestType.WebReadyRequest;
			}
			else if (request.Url.LocalPath.EndsWith(".owa", StringComparison.OrdinalIgnoreCase))
			{
				result = OwaRequestType.Invalid;
			}
			else if (RequestDispatcherUtilities.IsOwaUrl(request.Url, OwaUrl.AuthFolderUrl, false))
			{
				result = OwaRequestType.Authorize;
			}
			else if (request.Url.LocalPath.EndsWith(".aspx", StringComparison.OrdinalIgnoreCase) || request.Url.LocalPath.EndsWith(".ashx", StringComparison.OrdinalIgnoreCase))
			{
				result = OwaRequestType.Aspx;
			}
			else if (request.Url.LocalPath.EndsWith(RequestDispatcherUtilities.VirtualDirectoryNameWithLeadingSlash, StringComparison.OrdinalIgnoreCase) || request.Url.LocalPath.EndsWith(RequestDispatcherUtilities.VirtualDirectoryNameWithLeadingAndTrailingSlash, StringComparison.OrdinalIgnoreCase))
			{
				result = OwaRequestType.Form15;
			}
			else if (EsoRequest.IsEsoRequest(request))
			{
				result = OwaRequestType.EsoRequest;
			}
			else if (request.Url.LocalPath.Contains("service.svc"))
			{
				result = OwaRequestType.ServiceRequest;
			}
			else if (request.Url.LocalPath.IndexOf("Speech.reco", StringComparison.OrdinalIgnoreCase) != -1)
			{
				result = OwaRequestType.SpeechReco;
			}
			else if (Globals.IsPreCheckinApp)
			{
				result = OwaRequestType.Form15;
			}
			else
			{
				result = OwaRequestType.Invalid;
			}
			return result;
		}

		public static bool IsPremiumRequest(HttpRequest httpRequest)
		{
			return httpRequest.Url.LocalPath.EndsWith(".owa2", StringComparison.InvariantCultureIgnoreCase) || httpRequest.Url.AbsoluteUri.Contains("/wopi/") || httpRequest.Url.LocalPath.EndsWith("sessiondata.ashx", StringComparison.InvariantCultureIgnoreCase) || httpRequest.Url.LocalPath.EndsWith("remotenotification.ashx", StringComparison.InvariantCultureIgnoreCase) || httpRequest.Url.LocalPath.EndsWith("Plt1.ashx", StringComparison.InvariantCultureIgnoreCase) || httpRequest.Url.LocalPath.EndsWith("speech.reco", StringComparison.InvariantCultureIgnoreCase) || httpRequest.Url.LocalPath.EndsWith("appCacheManifestHandler.ashx", StringComparison.InvariantCultureIgnoreCase) || httpRequest.Url.LocalPath.EndsWith("userspecificresourceinjector.ashx", StringComparison.InvariantCultureIgnoreCase) || httpRequest.Url.LocalPath.EndsWith("mowapendingget.ashx", StringComparison.InvariantCultureIgnoreCase) || httpRequest.Url.LocalPath.EndsWith("service.svc", StringComparison.InvariantCultureIgnoreCase) || httpRequest.Url.LocalPath.EndsWith("service.svc/s/GetUserPhoto", StringComparison.InvariantCultureIgnoreCase) || httpRequest.Url.LocalPath.EndsWith("proxylogon.owa", StringComparison.InvariantCultureIgnoreCase) || httpRequest.Url.LocalPath.EndsWith("/service.svc/s/GetFileAttachment", StringComparison.InvariantCultureIgnoreCase);
		}

		public static bool IsDownLevelClient(HttpContext httpContext, bool avoidUserContextAccess = false)
		{
			if (httpContext == null)
			{
				throw new ArgumentNullException("httpContext cannot be null");
			}
			HttpCookie httpCookie = httpContext.Request.Cookies["PALEnabled"];
			if (httpCookie != null && !string.IsNullOrEmpty(httpCookie.Value))
			{
				return false;
			}
			if (httpContext.Items.Contains("IsDownLevelClient"))
			{
				return (bool)httpContext.Items["IsDownLevelClient"];
			}
			if (Utility.IsResourceRequest(httpContext.Request.Url.LocalPath))
			{
				avoidUserContextAccess = true;
			}
			bool flag = false;
			bool flag2 = false;
			bool flag3 = true;
			bool isAndroidPremiumEnabled = false;
			if (httpContext.User != null && httpContext.User.Identity.IsAuthenticated && !avoidUserContextAccess)
			{
				UserContext userContext = UserContextManager.GetMailboxContext(httpContext, null, true) as UserContext;
				if (userContext != null)
				{
					bool flag4 = Culture.GetPreferredCultureInfo(userContext.ExchangePrincipal) != null;
					if (flag4)
					{
						ConfigurationContext configurationContext = new ConfigurationContext(userContext);
						flag3 = configurationContext.IsFeatureEnabled(Feature.RichClient);
						flag2 = userContext.IsOptimizedForAccessibility;
						flag = true;
						ExTraceGlobals.CoreCallTracer.TraceDebug(0L, string.Format("isOptimizedForAccessibility: {0}, isRichClientFeatureEnabled: {1}", flag2, flag3));
					}
					if (userContext.FeaturesManager.ServerSettings.AndroidPremium.Enabled)
					{
						isAndroidPremiumEnabled = true;
					}
				}
				else
				{
					ExTraceGlobals.CoreCallTracer.TraceDebug(0L, "userContext is null when process IsDownLevelClient");
				}
			}
			bool flag5 = RequestDispatcherUtilities.IsDownLevelClient(httpContext.Request, flag2, flag3, isAndroidPremiumEnabled);
			if (flag)
			{
				httpContext.Items["IsDownLevelClient"] = flag5;
				ExTraceGlobals.CoreCallTracer.TraceDebug(0L, string.Format("Cache result '{0}' for IsDownLevelClient", flag5));
			}
			else
			{
				ExTraceGlobals.CoreCallTracer.TraceDebug(0L, string.Format("Result '{0}' for IsDownLevelClient is not cached.", flag5));
			}
			return flag5;
		}

		public static bool IsChangePasswordLogoff(HttpRequest request)
		{
			return HttpUtilities.GetQueryStringParameter(request, "ChgPwd", false) == "1";
		}

		public static bool IsLayoutParameterForLight(HttpRequest request)
		{
			string text = request.QueryString["layout"];
			if (!string.IsNullOrEmpty(text))
			{
				if (string.Compare("light", text, true) == 0)
				{
					return true;
				}
			}
			else
			{
				HttpCookie httpCookie = HttpContext.Current.Request.Cookies["OwaLight"];
				if (httpCookie != null && !string.IsNullOrEmpty(httpCookie.Value))
				{
					return true;
				}
			}
			return false;
		}

		internal static void RespondProxyPing(RequestContext requestContext)
		{
			requestContext.HttpContext.Response.AppendHeader("X-OWA-Version", Globals.ApplicationVersion);
			requestContext.HttpStatusCode = (HttpStatusCode)242;
		}

		internal static void GetLanguagePostFormParameters(RequestContext requestContext, HttpRequest request, out CultureInfo culture, out string timeZoneKeyName, out bool isOptimized, out string destination)
		{
			culture = null;
			timeZoneKeyName = string.Empty;
			isOptimized = false;
			destination = string.Empty;
			string text = request.Form["lcid"];
			int num = -1;
			if (string.IsNullOrEmpty(text))
			{
				throw new OwaInvalidRequestException("locale ID parameter is missing or empty");
			}
			if (!int.TryParse(text, out num) || !ClientCultures.IsSupportedCulture(num))
			{
				throw new OwaInvalidRequestException("locale ID parameter is invalid");
			}
			culture = new CultureInfo(num);
			timeZoneKeyName = request.Form["tzid"];
			if (string.IsNullOrEmpty(timeZoneKeyName))
			{
				throw new OwaInvalidRequestException("timezone ID parameter is missing or empty");
			}
			ExTimeZone exTimeZone = null;
			if (!ExTimeZoneEnumerator.Instance.TryGetTimeZoneByName(timeZoneKeyName, out exTimeZone))
			{
				throw new OwaInvalidRequestException("timezone ID parameter is invalid");
			}
			if (request.Form["opt"] != null)
			{
				isOptimized = true;
			}
			destination = HttpUtilities.GetFormParameter(requestContext.HttpContext.Request, "destination", false);
		}

		internal static bool TryReadMowaGlobalizationSettings(RequestContext requestContext, out CultureInfo culture, out string timeZoneKeyName)
		{
			string text = requestContext.HttpContext.Request.QueryString["MOWALanguageID"];
			string text2 = requestContext.HttpContext.Request.QueryString["MOWATimeZoneID"];
			culture = null;
			timeZoneKeyName = null;
			if (string.IsNullOrEmpty(text))
			{
				HttpCookie httpCookie = requestContext.HttpContext.Request.Cookies["MOWALanguageID"];
				if (httpCookie != null)
				{
					text = httpCookie.Value;
				}
			}
			if (string.IsNullOrEmpty(text2))
			{
				HttpCookie httpCookie2 = requestContext.HttpContext.Request.Cookies["MOWATimeZoneID"];
				if (httpCookie2 != null)
				{
					text2 = httpCookie2.Value;
				}
			}
			if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(text2))
			{
				return false;
			}
			try
			{
				culture = new CultureInfo(text);
				if (!ClientCultures.IsSupportedCulture(culture))
				{
					return false;
				}
			}
			catch (CultureNotFoundException arg)
			{
				ExTraceGlobals.CoreCallTracer.TraceDebug<string, CultureNotFoundException>(0L, "The language passed in by MOWA is invalid, language={0}, exception: {1}", text, arg);
				return false;
			}
			timeZoneKeyName = text2;
			return !string.IsNullOrEmpty(timeZoneKeyName) && RequestDispatcherUtilities.IsValidTimeZoneKeyName(timeZoneKeyName);
		}

		internal static bool TryReadUpdatedUserSettingsCookie(RequestContext requestContext, out EcpUserSettings settings)
		{
			settings = (EcpUserSettings)0U;
			HttpCookie httpCookie = requestContext.HttpContext.Request.Cookies["UpdatedUserSettings"];
			if (httpCookie == null)
			{
				return false;
			}
			string value = httpCookie.Value;
			bool result;
			try
			{
				settings = (EcpUserSettings)Convert.ToUInt32(value);
				result = true;
			}
			catch (FormatException)
			{
				throw new OwaInvalidRequestException(string.Format(CultureInfo.InvariantCulture, "The value of the cookie {0} is invalid.", new object[]
				{
					"UpdatedUserSettings"
				}));
			}
			catch (OverflowException)
			{
				throw new OwaInvalidRequestException(string.Format(CultureInfo.InvariantCulture, "The value of the cookie {0} is invalid.", new object[]
				{
					"UpdatedUserSettings"
				}));
			}
			return result;
		}

		internal static CultureInfo GetUserCultureFromEcpCookie(RequestContext requestContext, EcpUserSettings settingToReload)
		{
			CultureInfo cultureInfo = null;
			if (UserContextUtilities.IsFlagSet((int)settingToReload, 32))
			{
				HttpCookie httpCookie = HttpContext.Current.Request.Cookies["mkt"];
				if (httpCookie != null && !string.IsNullOrEmpty(httpCookie.Value))
				{
					CultureInfo cultureInfo2 = CultureInfo.GetCultureInfo(httpCookie.Value);
					if (ClientCultures.IsSupportedCulture(cultureInfo2))
					{
						cultureInfo = cultureInfo2;
					}
				}
				if (cultureInfo == null)
				{
					ExTraceGlobals.UserContextCallTracer.TraceDebug(0L, "UserContext.ReloadUserSettings: The culture cookie doesn't have a valid value.");
				}
			}
			return cultureInfo;
		}

		internal static void DeleteUserSettingsCookie(RequestContext requestContext)
		{
			HttpUtilities.DeleteCookie(requestContext.HttpContext.Response, "UpdatedUserSettings");
		}

		internal static bool IsValidTimeZoneKeyName(string timeZoneKeyName)
		{
			ExTimeZone exTimeZone = null;
			return ExTimeZoneEnumerator.Instance.TryGetTimeZoneByName(timeZoneKeyName, out exTimeZone);
		}

		internal static string GetDestinationForRedirectToLanguagePage(RequestContext requestContext)
		{
			string text = OwaUrl.LanguagePage.GetExplicitUrl(requestContext.HttpContext.Request);
			if (!string.IsNullOrEmpty(requestContext.DestinationUrlQueryString))
			{
				text += string.Concat(new string[]
				{
					"?",
					RequestDispatcherUtilities.GetRequestQueryStringLcidParams(requestContext.HttpContext.Request),
					"url",
					"=",
					OwaUrl.ApplicationRoot.GetExplicitUrl(requestContext.HttpContext.Request),
					"?",
					HttpUtility.UrlEncode(requestContext.DestinationUrlQueryString)
				});
			}
			return text;
		}

		internal static string GetRequestQueryStringLcidParams(HttpRequest request)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (request != null && request.QueryString.Count > 0)
			{
				foreach (string text in request.QueryString.AllKeys)
				{
					if (text.ToLower().Equals("ll-cc"))
					{
						stringBuilder.Append(text);
						stringBuilder.Append("=");
						stringBuilder.Append(request.QueryString[text]);
						stringBuilder.Append("&");
					}
				}
			}
			return stringBuilder.ToString();
		}

		internal static bool IsCmdWebPart(HttpRequest request)
		{
			string text = request.QueryString["cmd"];
			return !string.IsNullOrEmpty(text) && string.Equals(text, "contents", StringComparison.OrdinalIgnoreCase);
		}

		internal static void SetXFrameOptionsHeader(RequestContext requestContext, OwaRequestType requestType)
		{
			if (requestContext == null)
			{
				return;
			}
			HttpContext httpContext = requestContext.HttpContext;
			if (httpContext == null || !httpContext.Request.HttpMethod.Equals("GET") || (httpContext.Request.UserAgent != null && httpContext.Request.UserAgent.Contains("MSAppHost")))
			{
				return;
			}
			switch (requestType)
			{
			case OwaRequestType.EsoRequest:
			case OwaRequestType.Oeh:
			case OwaRequestType.ProxyPing:
			case OwaRequestType.KeepAlive:
			case OwaRequestType.Resource:
			case OwaRequestType.PublishedCalendarView:
			case OwaRequestType.ICalHttpHandler:
			case OwaRequestType.HealthPing:
			case OwaRequestType.SpeechReco:
				break;
			case OwaRequestType.Form15:
			case OwaRequestType.ProxyLogon:
			case OwaRequestType.LanguagePage:
			case OwaRequestType.LanguagePost:
			case OwaRequestType.Attachment:
			case OwaRequestType.WebPart:
			case OwaRequestType.ServiceRequest:
				goto IL_9D;
			default:
				if (requestType != OwaRequestType.SuiteServiceProxyPage)
				{
					goto IL_9D;
				}
				break;
			}
			return;
			IL_9D:
			string value = "SAMEORIGIN";
			if (RequestDispatcherUtilities.IsCmdWebPart(httpContext.Request))
			{
				UserContext userContext = (requestContext.UserContext ?? UserContextManager.GetMailboxContext(httpContext, null, true)) as UserContext;
				if (userContext != null)
				{
					ConfigurationContext configurationContext = new ConfigurationContext(userContext);
					if (configurationContext != null)
					{
						switch ((int)configurationContext.GetFeaturesEnabled(Feature.WebPartsDefaultOrigin | Feature.WebPartsEnableOrigins))
						{
						case 0:
						case 1:
							value = "DENY";
							break;
						case 2:
							value = null;
							break;
						}
					}
				}
			}
			if (!string.IsNullOrEmpty(value))
			{
				httpContext.Response.Headers.Set("X-Frame-Options", value);
			}
		}

		internal static string GetStringUrlParameter(HttpContext context, string paramName)
		{
			string text = context.Request.QueryString[paramName];
			if (string.IsNullOrEmpty(text))
			{
				try
				{
					Uri uri;
					string text2 = context.Request.TryParseUrlReferrer(out uri) ? uri.Query : null;
					if (!string.IsNullOrEmpty(text2))
					{
						NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(text2);
						text = nameValueCollection[paramName];
						if (!string.IsNullOrEmpty(text) && paramName == "layout" && uri.AbsoluteUri != null && uri.AbsoluteUri.ToLowerInvariant().Contains("appcachemanifesthandler.ashx"))
						{
							return null;
						}
					}
				}
				catch (UriFormatException)
				{
				}
				catch (InvalidOperationException)
				{
				}
				return text;
			}
			return text;
		}

		internal static int GetIntUrlParameter(HttpContext context, string paramName, int defaultValue)
		{
			string stringUrlParameter = RequestDispatcherUtilities.GetStringUrlParameter(context, paramName);
			int result;
			if (int.TryParse(stringUrlParameter, out result))
			{
				return result;
			}
			return defaultValue;
		}

		private static bool IsDownLevelClient(HttpRequest request, bool isOptimizedForAccessibility, bool isRichClientFeatureEnabled, bool isAndroidPremiumEnabled)
		{
			if (request == null)
			{
				throw new ArgumentNullException("request cannot be null");
			}
			ExTraceGlobals.CoreCallTracer.TraceDebug<string>(0L, "Utilities.IsDownLevelClient. user-agent = {0}", (request.UserAgent == null) ? string.Empty : request.UserAgent);
			if (!isRichClientFeatureEnabled)
			{
				return true;
			}
			if (isOptimizedForAccessibility)
			{
				return true;
			}
			string text = request.QueryString["layout"];
			if (!string.IsNullOrEmpty(text))
			{
				if (string.Compare("light", text, true) == 0)
				{
					HttpCookie httpCookie = new HttpCookie("OwaLight");
					httpCookie.Value = "1";
					HttpContext.Current.Response.Cookies.Add(httpCookie);
					return true;
				}
				HttpContext.Current.Response.Cookies.Remove("OwaLight");
				return false;
			}
			else
			{
				HttpCookie httpCookie2 = HttpContext.Current.Request.Cookies["OwaLight"];
				if (httpCookie2 != null && !string.IsNullOrEmpty(httpCookie2.Value))
				{
					return true;
				}
				if (string.IsNullOrEmpty(request.UserAgent))
				{
					return true;
				}
				UserAgent userAgent = new UserAgent(request.UserAgent, false, HttpContext.Current.Request.Cookies);
				return (!userAgent.IsBrowserIE() || userAgent.IsBrowserMobileIE() || !(RequestDispatcherUtilities.IsDatacenterNonDedicated ? (userAgent.BrowserVersion.Build >= 9 || userAgent.GetTridentVersion() >= 5.0) : (userAgent.BrowserVersion.Build >= 8 || (userAgent.BrowserVersion.Build == 7 && request.UserAgent.IndexOf("Trident") > 0)))) && ((!userAgent.IsBrowserMobileIE() || userAgent.BrowserVersion.Build < 10 || userAgent.IsMobileIEDesktopMode()) && (!userAgent.IsBrowserSafari() || userAgent.BrowserVersion.Build < 4 || !string.Equals(userAgent.Platform, "Macintosh", StringComparison.OrdinalIgnoreCase)) && ((!string.Equals(userAgent.Platform, "iPhone", StringComparison.OrdinalIgnoreCase) && !string.Equals(userAgent.Platform, "iPad", StringComparison.OrdinalIgnoreCase)) || userAgent.PlatformVersion.Build < 6)) && (!isAndroidPremiumEnabled || !string.Equals(userAgent.Platform, "Android", StringComparison.OrdinalIgnoreCase) || !userAgent.IsBrowserChrome() || userAgent.BrowserVersion.Build < 30 || ((userAgent.PlatformVersion.Build != 4 || userAgent.PlatformVersion.Major < 1) && userAgent.PlatformVersion.Build <= 4)) && (!userAgent.IsBrowserFirefox() || userAgent.BrowserVersion.Build < 4 || (!string.Equals(userAgent.Platform, "Windows NT", StringComparison.OrdinalIgnoreCase) && !string.Equals(userAgent.Platform, "Macintosh", StringComparison.OrdinalIgnoreCase) && !string.Equals(userAgent.Platform, "Linux", StringComparison.OrdinalIgnoreCase))) && (!userAgent.IsBrowserChrome() || (!string.Equals(userAgent.Platform, "Windows NT", StringComparison.OrdinalIgnoreCase) && !string.Equals(userAgent.Platform, "Macintosh", StringComparison.OrdinalIgnoreCase) && !string.Equals(userAgent.Platform, "Linux", StringComparison.OrdinalIgnoreCase) && !string.Equals(userAgent.Platform, "CrOS", StringComparison.OrdinalIgnoreCase)));
			}
		}

		private static bool IsDatacenterNonDedicated
		{
			get
			{
				return VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Global.MultiTenancy.Enabled;
			}
		}

		internal const string UserCultureCookie = "mkt";

		internal const string LanguagePostLocaleIdParameter = "lcid";

		internal const string LanguagePostTimeZoneKeyNameParameter = "tzid";

		internal const string LanguagePostLightExperienceParameter = "opt";

		internal const string LanguagePostDestinationParameter = "destination";

		internal const string UpdatedUserSettingCookie = "UpdatedUserSettings";

		internal const string LegacyOptionsPathFragment = "/ecp/";

		internal const string MowaLanguageCookie = "MOWALanguageID";

		internal const string MowaTimeZoneCookie = "MOWATimeZoneID";

		internal const string DestinationParameter = "url";

		internal const string LcidParameter = "ll-cc";

		internal const int HttpStatusRetryRequest = 241;

		internal const int HttpStatusProxyPingSucceeded = 242;

		internal const string LayoutParameterName = "layout";

		private const string Command = "cmd";

		private const string CommandValue = "contents";

		private const string LayoutOwaLight = "light";

		private const string OwaLightCookieName = "OwaLight";

		private const string IsDownLevelClientCacheKey = "IsDownLevelClient";

		private static readonly string VirtualDirectoryNameWithLeadingSlash = HttpRuntime.AppDomainAppVirtualPath;

		private static readonly string VirtualDirectoryNameWithLeadingAndTrailingSlash = HttpRuntime.AppDomainAppVirtualPath + "/";

		private enum WebPartsFrameOptions
		{
			Deny,
			AllowFrom,
			None,
			SameOrigin
		}
	}
}
