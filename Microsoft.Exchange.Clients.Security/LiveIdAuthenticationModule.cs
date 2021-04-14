using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using Microsoft.Exchange.Clients.Common;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.Metabase;
using Microsoft.Exchange.Net.Wopi;
using Microsoft.Exchange.Security;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.Security.Authentication.FederatedAuthService;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Passport.RPS;

namespace Microsoft.Exchange.Clients.Security
{
	public sealed class LiveIdAuthenticationModule : IHttpModule
	{
		public static bool AppPasswordCheckEnabled
		{
			get
			{
				return LiveIdAuthenticationModule.appPasswordCheckEnabled;
			}
		}

		private static string AuthPolicyOverrideValue
		{
			get
			{
				if (LiveIdAuthenticationModule.authPolicyOverrideValue == null && !LiveIdAuthenticationModule.TryReadConfigString("LiveIdAuthPolicyOverrideValue", out LiveIdAuthenticationModule.authPolicyOverrideValue))
				{
					LiveIdAuthenticationModule.authPolicyOverrideValue = "MBI_SSL";
				}
				return LiveIdAuthenticationModule.authPolicyOverrideValue;
			}
		}

		private static bool IsSessionDataPreloadEnabled(HttpContext httpContext)
		{
			if (LiveIdAuthenticationModule.isSessionDataPreloadEnabled == null)
			{
				bool flag = false;
				LiveIdAuthenticationModule.isSessionDataPreloadEnabled = new bool?(LiveIdAuthenticationModule.TryReadConfigBool("IsSessionDataPreloadEnabled", out flag) && flag);
			}
			string text;
			bool flag2 = LiveIdAuthenticationModule.TryGetExplicitLogonUrlSegment(httpContext.Request.GetRequestUrlEvenIfProxied(), out text);
			return LiveIdAuthenticationModule.isSessionDataPreloadEnabled.Value && !flag2 && !OfflineClientRequestUtilities.IsRequestFromMOWAClient(httpContext.Request, httpContext.Request.UserAgent);
		}

		public void Init(HttpApplication context)
		{
			if (context == null)
			{
				throw new ArgumentNullException("context");
			}
			if (!LiveIdAuthenticationModule.staticVariablesInitialized)
			{
				lock (LiveIdAuthenticationModule.staticLock)
				{
					if (!LiveIdAuthenticationModule.staticVariablesInitialized)
					{
						LiveIdAuthenticationModule.virtualDirectoryName = HttpRuntime.AppDomainAppVirtualPath.Replace("'", "''");
						if (LiveIdAuthenticationModule.virtualDirectoryName[0] == '/')
						{
							LiveIdAuthenticationModule.virtualDirectoryName = LiveIdAuthenticationModule.virtualDirectoryName.Substring(1);
						}
						LiveIdAuthenticationModule.isInCalendarVDir = (!string.IsNullOrEmpty(HttpRuntime.AppDomainAppId) && HttpRuntime.AppDomainAppId.EndsWith("/calendar", StringComparison.CurrentCultureIgnoreCase));
						LiveIdAuthenticationModule.isInOmaVDir = (!string.IsNullOrEmpty(HttpRuntime.AppDomainAppId) && HttpRuntime.AppDomainAppId.EndsWith("/oma", StringComparison.CurrentCultureIgnoreCase));
						if (!LiveIdAuthenticationModule.isInCalendarVDir && !LiveIdAuthenticationModule.isInOmaVDir)
						{
							LiveIdAuthenticationModule.liveIdAuthenticationEnabled = LiveIdAuthenticationModule.IsLiveIdAuthenticationEnabled();
						}
						if (LiveIdAuthenticationModule.liveIdAuthenticationEnabled || LiveIdAuthenticationModule.isInCalendarVDir || LiveIdAuthenticationModule.isInOmaVDir)
						{
							LiveIdAuthenticationModule.TryReadConfigString("PodRedirectTemplate", out LiveIdAuthenticationModule.podRedirectTemplate);
							if (!LiveIdAuthenticationModule.TryReadConfigInt("PodSiteStartRange", out LiveIdAuthenticationModule.podSiteStartRange))
							{
								ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceError(0L, "No pod site start range specified in web.config");
								LiveIdAuthenticationModule.liveIdAuthenticationEnabled = false;
								return;
							}
							if (!LiveIdAuthenticationModule.TryReadConfigInt("PodSiteEndRange", out LiveIdAuthenticationModule.podSiteEndRange))
							{
								ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceError(0L, "No pod site end range specified in web.config");
								LiveIdAuthenticationModule.liveIdAuthenticationEnabled = false;
								return;
							}
						}
						if (LiveIdAuthenticationModule.liveIdAuthenticationEnabled)
						{
							LiveIdAuthenticationModule.InitLiveIdAuthenticationStaticVariables();
						}
						else
						{
							ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceDebug(0L, "Live Id Authentication is disabled");
						}
						LiveIdAuthenticationCounters.TotalSessionDataPreloadRequestsSent.RawValue = 0L;
						LiveIdAuthenticationCounters.TotalSessionDataPreloadRequestsFailed.RawValue = 0L;
						LiveIdAuthenticationModule.hostNameController = new HostNameController(ConfigurationManager.AppSettings);
						LiveIdAuthenticationModule.staticVariablesInitialized = true;
					}
				}
			}
			if (LiveIdAuthenticationModule.liveIdAuthenticationEnabled)
			{
				context.AuthenticateRequest += this.OnAuthenticateRequest;
				context.PostAuthenticateRequest += this.OnPostAuthenticateRequest;
			}
			if (LiveIdAuthenticationModule.isInCalendarVDir)
			{
				context.BeginRequest += this.OnBeginRequest;
			}
		}

		public void Dispose()
		{
		}

		internal static string GetLocalizedLiveIdSignoutLinkMessage(HttpContext httpContext)
		{
			string liveLogoutRedirectUrl = LiveIdAuthentication.GetLiveLogoutRedirectUrl(LiveIdAuthenticationModule.CreateRedirectUrl(httpContext.Request.GetRequestUrlEvenIfProxied(), LiveIdAuthenticationModule.GetOrgOwnSiteReturnUrlHost(httpContext), LiveIdAuthenticationModule.iOwnSiteDNSSubdomain), LiveIdAuthenticationModule.GetOrgOwnSiteName(httpContext), LiveIdAuthenticationModule.IsConsumerRequestForO365(httpContext.Request));
			return "<BR><BR>" + string.Format(CultureInfo.InvariantCulture, Strings.LogonErrorLogoutUrlText, new object[]
			{
				liveLogoutRedirectUrl
			});
		}

		internal static string GetVirtualDirectoryUrl(Uri url)
		{
			return LiveIdAuthenticationModule.GetVirtualDirectoryUrl(url, false);
		}

		internal static void GenerateCommonAccessToken(HttpContext httpContext, LiveIDIdentity identity, string puid)
		{
			if (httpContext == null)
			{
				throw new ArgumentNullException("httpContext");
			}
			if (identity == null)
			{
				throw new ArgumentNullException("identity");
			}
			CommonAccessToken token = LiveIdFbaTokenAccessor.Create(identity).GetToken();
			token.ExtensionData["Puid"] = puid;
			token.ExtensionData["AuthenticationAuthority"] = AuthenticationAuthority.ORGID.ToString();
			httpContext.Items["AuthenticatedUserOrganization"] = identity.UserOrganizationId.ConfigurationUnit.Parent.Name;
			httpContext.Items["Item-CommonAccessToken"] = token;
		}

		private static bool DoesCommonAccessTokenExist(HttpContext httpContext)
		{
			return !string.IsNullOrWhiteSpace(httpContext.Request.Headers["X-CommonAccessToken"]);
		}

		internal static void ContinueOnAuthenticate(HttpContext httpContext)
		{
			LiveIdAuthenticationModule.InternalOnAuthenticate(httpContext);
		}

		private static void HandleAuthException(HttpContext httpContext, Exception exception)
		{
			if (exception.GetType() != typeof(ThreadAbortException))
			{
				ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceDebug<string>(0L, "Exception in OnAuthenticate request: {0}", exception.ToString());
				RPSErrorCode? rpserrorCode = null;
				if (exception is LiveClientException)
				{
					rpserrorCode = new RPSErrorCode?((RPSErrorCode)((LiveClientException)exception).ErrorCode);
				}
				else if (exception is LiveConfigurationException)
				{
					rpserrorCode = new RPSErrorCode?((RPSErrorCode)((LiveConfigurationException)exception).ErrorCode);
				}
				else if (exception is LiveTransientException)
				{
					rpserrorCode = new RPSErrorCode?((RPSErrorCode)((LiveTransientException)exception).ErrorCode);
				}
				else if (exception is LiveOperationException)
				{
					rpserrorCode = new RPSErrorCode?((RPSErrorCode)((LiveOperationException)exception).ErrorCode);
				}
				if (rpserrorCode != null)
				{
					UrlUtilities.RewriteParameterInURL(httpContext, "lex", string.Format("{0}-{1}", exception.GetType().ToString(), rpserrorCode.Value.ToString()));
				}
				else
				{
					UrlUtilities.RewriteParameterInURL(httpContext, "lex", exception.GetType().ToString());
				}
				if (!(exception is HttpException))
				{
					ExWatson.SendReport(exception, ReportOptions.None, null);
				}
				try
				{
					Utilities.HandleException(httpContext, exception, false);
				}
				catch (ThreadAbortException)
				{
				}
			}
		}

		internal static bool IsConsumerRequestForO365(HttpRequest request)
		{
			return (request.Cookies["O365Consumer"] != null && "1".Equals(request.Cookies["O365Consumer"].Value, StringComparison.OrdinalIgnoreCase)) || (request.QueryString["isc"] != null && "1".Equals(request.QueryString["isc"], StringComparison.OrdinalIgnoreCase));
		}

		internal static void ProcessFrontEndLiveIdAuthCookie(HttpContext httpContext, string encodedHeaderValue)
		{
			if (httpContext == null)
			{
				throw new ArgumentNullException("httpContext");
			}
			if (!AuthCommon.IsFrontEnd)
			{
				throw new InvalidOperationException("This method can only be used on FrontEnd");
			}
			bool useConsumerRps = LiveIdAuthenticationModule.IsConsumerRequestForO365(httpContext.Request);
			LiveIdAuthentication.WriteHeadersToResponse(httpContext, HttpUtility.UrlDecode(encodedHeaderValue), useConsumerRps);
		}

		private static void InitLiveIdAuthenticationStaticVariables()
		{
			if (!LiveIdAuthenticationModule.TryReadConfigString("LiveIdAuthModuleSiteName", out LiveIdAuthenticationModule.orgOwnSiteName))
			{
				ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceError(0L, "No sitename specified in web.config");
				LiveIdAuthenticationModule.liveIdAuthenticationEnabled = false;
				return;
			}
			if (!LiveIdAuthenticationModule.TryReadConfigString("OwaEcpCanonicalHostName", out LiveIdAuthenticationModule.CanonicalHostName))
			{
				ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceError(0L, "No canonical host name specified in web.config");
				LiveIdAuthenticationModule.liveIdAuthenticationEnabled = false;
				return;
			}
			if (!LiveIdAuthenticationModule.TryReadConfigString("LiveIdAuthModuleLegacySiteName", out LiveIdAuthenticationModule.legacySiteName))
			{
				ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceWarning(0L, "No legacy sitename specified in web.config");
			}
			if (!LiveIdAuthenticationModule.TryReadConfigString("LiveIdAuthModuleO365SiteName", out LiveIdAuthenticationModule.o365SiteName))
			{
				ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceWarning(0L, "No o365 sitename specified in web.config");
				if (string.IsNullOrEmpty(LiveIdAuthenticationModule.o365SiteName))
				{
					LiveIdAuthenticationModule.o365SiteName = "outlook.office365.com";
				}
			}
			if (!LiveIdAuthenticationModule.TryReadConfigString("LiveIdAuthModuleO365Namespace", out LiveIdAuthenticationModule.o365Namespace))
			{
				ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceWarning(0L, "No o365 sitename specified in web.config");
				if (string.IsNullOrEmpty(LiveIdAuthenticationModule.o365Namespace))
				{
					LiveIdAuthenticationModule.o365Namespace = "office365.com";
				}
			}
			string text;
			if (!LiveIdAuthenticationModule.TryReadConfigString("LiveIdAuthModuleSDFSiteName", out text))
			{
				ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceWarning(0L, "No SDF sitename specified in web.config");
				text = "sdfpilot.outlook.com";
			}
			ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceDebug<string>(0L, "Using string '{0}' to for SDF site names.", text);
			LiveIdAuthenticationModule.sdfSiteNames = text.Split(new char[]
			{
				','
			}, StringSplitOptions.RemoveEmptyEntries);
			if (!LiveIdAuthenticationModule.TryReadConfigString("EduNamespace", out LiveIdAuthenticationModule.eduNamespaceKey))
			{
				ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceError(0L, "No edu namespace key  specified in web.config");
			}
			if (!LiveIdAuthenticationModule.TryReadConfigString("EduWHR", out LiveIdAuthenticationModule.eduWHRKey))
			{
				ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceError(0L, "No whr specified for edu namespace key  in web.config");
			}
			LiveIdAuthenticationModule.regexEduRealmParameter = new Regex("([\\?&]realm=)" + LiveIdAuthenticationModule.eduNamespaceKey, RegexOptions.IgnoreCase | RegexOptions.Compiled);
			LiveIdAuthenticationModule.eduWHRRealmParameter = "$1" + LiveIdAuthenticationModule.eduWHRKey;
			if (!LiveIdAuthenticationModule.TryReadConfigBool("EducationPage", out LiveIdAuthenticationModule.educationPageEnabled))
			{
				ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceError(0L, "No education page enabled key  in web.config");
				LiveIdAuthenticationModule.educationPageEnabled = true;
			}
			if (!LiveIdAuthenticationModule.TryReadConfigBool("CheckAppPassword", out LiveIdAuthenticationModule.appPasswordCheckEnabled))
			{
				ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceDebug<bool>(0L, "Using default value for checking app password: {0}", true);
				LiveIdAuthenticationModule.appPasswordCheckEnabled = true;
			}
			if (!LiveIdAuthenticationModule.TryReadConfigString("LiveIdAuthNewMailUrl", out LiveIdAuthenticationModule.liveIdAuthNewMailUrl))
			{
				ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceError(0L, "No new mail sign in url key  in web.config");
				LiveIdAuthenticationModule.liveIdAuthNewMailUrl = string.Empty;
			}
			if (!LiveIdAuthenticationModule.TryReadConfigBool("LiveIdAuthEnableHotmailRedirect", out LiveIdAuthenticationModule.enableHotmailRedirect))
			{
				ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceError(0L, "No redirect to hotmail key  in web.config");
				LiveIdAuthenticationModule.enableHotmailRedirect = true;
			}
			if (!LiveIdAuthenticationModule.TryReadConfigBool("RedirectToConsumerInst", out LiveIdAuthenticationModule.redirectToConsumerInst))
			{
				ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceError(0L, "No key to redirect to consumer instance in web.config");
				LiveIdAuthenticationModule.redirectToConsumerInst = false;
			}
			if (!LiveIdAuthenticationModule.TryReadConfigString("LiveAuthModuleAccrualSiteName", out LiveIdAuthenticationModule.iOwnSiteName))
			{
				ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceError(0L, "No accrual sitename specified in web.config");
				LiveIdAuthenticationModule.liveIdAuthenticationEnabled = false;
				return;
			}
			if (!LiveIdAuthenticationModule.TryReadConfigString("LiveIdAuthAccrualSignUrl", out LiveIdAuthenticationModule.liveIdAuthAccrualSignUrl))
			{
				ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceError(0L, "No accrual url specified in web.config");
				LiveIdAuthenticationModule.liveIdAuthenticationEnabled = false;
				return;
			}
			LiveIdAuthenticationModule.continueOnMSAInitErrors = false;
			string value;
			if (LiveIdAuthenticationModule.TryReadConfigString("ContinueOnMSAInitErrors", out value))
			{
				LiveIdAuthenticationModule.continueOnMSAInitErrors = bool.Parse(value);
			}
			LiveIdAuthenticationModule.TryReadConfigString("AccrualSiteSubdomainDNS", out LiveIdAuthenticationModule.iOwnSiteDNSSubdomain);
			if (LiveIdAuthenticationModule.iOwnSiteDNSSubdomain == null || (string.IsNullOrEmpty(LiveIdAuthenticationModule.iOwnSiteDNSSubdomain.Trim()) && !LiveIdAuthenticationModule.iOwnSiteName.Equals(LiveIdAuthenticationModule.orgOwnSiteName, StringComparison.OrdinalIgnoreCase)))
			{
				ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceError(0L, "No subdomain for the accrual site was specified in web.config");
				LiveIdAuthenticationModule.liveIdAuthenticationEnabled = false;
				return;
			}
			if (!string.IsNullOrEmpty(LiveIdAuthenticationModule.iOwnSiteDNSSubdomain) && !LiveIdAuthenticationModule.iOwnSiteDNSSubdomain.EndsWith(".", StringComparison.OrdinalIgnoreCase))
			{
				LiveIdAuthenticationModule.iOwnSiteDNSSubdomain += ".";
			}
			if (!LiveIdAuthenticationModule.TryReadConfigBool("LiveIdAuthModuleSslOffloaded", out LiveIdAuthenticationModule.sslOffloaded))
			{
				ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceDebug<bool>(0L, "Using default value for ssl offloading: {0}", false);
				LiveIdAuthenticationModule.sslOffloaded = false;
			}
			string value2 = null;
			LiveIdAuthenticationModule.TryReadConfigString("LiveIdCookieAuthModule.EnableBEAuthVersion", out value2);
			LiveIdAuthenticationModule.isBeAuthEnabled = !string.IsNullOrWhiteSpace(value2);
			ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceDebug<bool>(0L, "Backend auth enabled: {0}", LiveIdAuthenticationModule.isBeAuthEnabled);
			LiveIdAuthenticationModule.TryReadConfigBool("LiveIdSkipAdLookupOnRandomBE", out LiveIdAuthenticationModule.skipAdLookupOnRandomBe);
			ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceDebug<bool>(0L, "ShouldSkipRandomBEADLookupKey: {0}", LiveIdAuthenticationModule.skipAdLookupOnRandomBe);
			LiveIdAuthenticationModule.TryReadConfigBool("ReturnToOringinalUrlByDefault", out LiveIdAuthenticationModule.returnToOringinalUrlByDefault);
			if (!LiveIdAuthenticationModule.TryReadConfigBool("MservLookupEnabledinTest", out LiveIdAuthenticationModule.isMservLookupEnabledinTest))
			{
				LiveIdAuthenticationModule.isMservLookupEnabledinTest = false;
			}
			if (!LiveIdAuthenticationModule.TryReadConfigBool("NewMailOptimizationsEnabled", out LiveIdAuthenticationModule.isNewMailOptimizationsEnabled))
			{
				LiveIdAuthenticationModule.isNewMailOptimizationsEnabled = true;
			}
			if (!LiveIdAuthenticationModule.TryReadConfigBool("AccountTerminationEnabled", out LiveIdAuthenticationModule.isAccountTerminationEnabled))
			{
				LiveIdAuthenticationModule.isAccountTerminationEnabled = false;
			}
			try
			{
				LiveIdAuthentication.Initialize(LiveIdAuthenticationModule.virtualDirectoryName, LiveIdAuthenticationModule.sslOffloaded);
			}
			catch (LiveTransientException ex)
			{
				ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceError<string, string>(0L, "LiveTransientException was thrown. Exception message: {0}. Stack trace: {1}", ex.Message, ex.StackTrace);
				LiveIdAuthenticationModule.liveIdAuthenticationEnabled = false;
				return;
			}
			catch (LiveConfigurationException ex2)
			{
				ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceError<string, string>(0L, "LiveConfigurationException was thrown. Exception message: {0}. Stack trace: {1}", ex2.Message, ex2.StackTrace);
				LiveIdAuthenticationModule.liveIdAuthenticationEnabled = false;
				return;
			}
			catch (LiveClientException ex3)
			{
				ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceError<string, string>(0L, "LiveClientException was thrown. Exception message: {0}. Stack trace: {1}", ex3.Message, ex3.StackTrace);
				LiveIdAuthenticationModule.liveIdAuthenticationEnabled = false;
				return;
			}
			if (!LiveIdAuthenticationModule.TryReadConfigString("LiveIdAuthModuleLogoffPage", out LiveIdAuthenticationModule.logoffPage))
			{
				ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceDebug<string>(0L, "Using default value for logoff page: {0}", "logoff.aspx");
				LiveIdAuthenticationModule.logoffPage = "logoff.aspx";
			}
			string defaultReturnUrl = LiveIdAuthentication.GetDefaultReturnUrl(LiveIdAuthenticationModule.orgOwnSiteName, false);
			if (!string.IsNullOrEmpty(defaultReturnUrl))
			{
				LiveIdAuthenticationModule.orgOwnSiteReturnURLHostEnterprise = new Uri(defaultReturnUrl).Authority;
				try
				{
					string defaultReturnUrl2 = LiveIdAuthentication.GetDefaultReturnUrl(LiveIdAuthenticationModule.orgOwnSiteName, true);
					if (string.IsNullOrEmpty(defaultReturnUrl2))
					{
						ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceError<string>(0L, "No default url for consumer site{0}", LiveIdAuthenticationModule.orgOwnSiteName);
						if (!LiveIdAuthenticationModule.continueOnMSAInitErrors)
						{
							LiveIdAuthenticationModule.liveIdAuthenticationEnabled = false;
							return;
						}
						LiveIdAuthenticationModule.orgOwnSiteReturnURLHostConsumer = null;
					}
					if (!string.IsNullOrEmpty(defaultReturnUrl2))
					{
						LiveIdAuthenticationModule.orgOwnSiteReturnURLHostConsumer = new Uri(defaultReturnUrl2).Authority;
					}
				}
				catch (Exception)
				{
					LiveIdAuthenticationModule.orgOwnSiteReturnURLHostConsumer = null;
				}
				if (!string.IsNullOrEmpty(LiveIdAuthenticationModule.legacySiteName))
				{
					string defaultReturnUrl3 = LiveIdAuthentication.GetDefaultReturnUrl(LiveIdAuthenticationModule.legacySiteName, false);
					if (string.IsNullOrEmpty(defaultReturnUrl3))
					{
						ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceWarning<string>(0L, "No default url for enterprise site {0}", LiveIdAuthenticationModule.legacySiteName);
					}
					else
					{
						LiveIdAuthenticationModule.legacySiteReturnURLHostEnterprise = new Uri(defaultReturnUrl3).Authority;
					}
					string text2 = null;
					try
					{
						text2 = LiveIdAuthentication.GetDefaultReturnUrl(LiveIdAuthenticationModule.legacySiteName, true);
					}
					catch (Exception)
					{
						text2 = null;
					}
					if (string.IsNullOrEmpty(text2))
					{
						ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceWarning<string>(0L, "No default url for consumer site {0}", LiveIdAuthenticationModule.legacySiteName);
					}
					else
					{
						LiveIdAuthenticationModule.legacySiteReturnURLHostConsumer = new Uri(text2).Authority;
					}
				}
				if (!string.IsNullOrEmpty(LiveIdAuthenticationModule.o365SiteName))
				{
					string defaultReturnUrl4 = LiveIdAuthentication.GetDefaultReturnUrl(LiveIdAuthenticationModule.o365SiteName, false);
					if (string.IsNullOrEmpty(defaultReturnUrl4))
					{
						ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceWarning<string>(0L, "No default url for enterprise site {0}", LiveIdAuthenticationModule.o365SiteName);
					}
					else
					{
						LiveIdAuthenticationModule.o365SiteReturnURLHostEnterprise = new Uri(defaultReturnUrl4).Authority;
					}
					string text3 = null;
					try
					{
						text3 = LiveIdAuthentication.GetDefaultReturnUrl(LiveIdAuthenticationModule.o365SiteName, true);
					}
					catch (Exception)
					{
						text3 = null;
					}
					if (string.IsNullOrEmpty(text3))
					{
						ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceWarning<string>(0L, "No default url for site {0}", LiveIdAuthenticationModule.o365SiteName);
					}
					else
					{
						LiveIdAuthenticationModule.o365SiteReturnURLHostConsumer = new Uri(text3).Authority;
					}
				}
				LiveIdAuthenticationModule.ticksBefore1970 = new DateTime(1970, 1, 1).Ticks;
				if (!LiveIdAuthenticationModule.TryReadConfigBool("IdentityTracingEnabled", out LiveIdAuthenticationModule.IdentityTracingEnabled))
				{
					LiveIdAuthenticationModule.IdentityTracingEnabled = true;
				}
				if (!LiveIdAuthenticationModule.TryReadConfigBool("LiveIdAuthModuleTimeoutEnabled", out LiveIdAuthenticationModule.timeoutEnabled))
				{
					LiveIdAuthenticationModule.timeoutEnabled = true;
				}
				if (LiveIdAuthenticationModule.timeoutEnabled)
				{
					if (!LiveIdAuthenticationModule.TryReadConfigInt("LiveIdAuthModuleTimeoutIntervalInSeconds", out LiveIdAuthenticationModule.timeoutIntervalInSeconds))
					{
						LiveIdAuthenticationModule.timeoutIntervalInSeconds = 82800;
					}
					else if (LiveIdAuthenticationModule.timeoutIntervalInSeconds <= 0 || LiveIdAuthenticationModule.timeoutIntervalInSeconds > 82800)
					{
						ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceDebug<int, int>(0L, "Invalid value: {0} specified for timeoutIntervalInSeconds in web.config. Using default value: {1}", LiveIdAuthenticationModule.timeoutIntervalInSeconds, 82800);
						LiveIdAuthenticationModule.timeoutIntervalInSeconds = 82800;
					}
				}
				if (!LiveIdAuthenticationModule.TryReadConfigString("TimeoutRedirectPolicy", out LiveIdAuthenticationModule.timeoutRedirectPolicy))
				{
					LiveIdAuthenticationModule.timeoutRedirectPolicy = "HBI";
				}
				if (!LiveIdAuthenticationModule.TryReadConfigTimeSpan("SlidingWindowOverride", out LiveIdAuthenticationModule.slidingWindowOverride))
				{
					LiveIdAuthenticationModule.slidingWindowOverride = TimeSpan.Zero;
				}
				if (!LiveIdAuthenticationModule.TryReadConfigBool("LiveIdAuthModuleCacheEnabled", out LiveIdAuthenticationModule.puidToSidCacheEnabled))
				{
					LiveIdAuthenticationModule.puidToSidCacheEnabled = true;
				}
				if (LiveIdAuthenticationModule.puidToSidCacheEnabled)
				{
					if (!LiveIdAuthenticationModule.TryReadConfigInt("LiveIdAuthModuleCacheSize", out LiveIdAuthenticationModule.puidToSidCacheSize))
					{
						LiveIdAuthenticationModule.puidToSidCacheSize = 8192;
					}
					else if (LiveIdAuthenticationModule.puidToSidCacheSize <= 0)
					{
						ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceDebug<int, int>(0L, "Invalid value: {0} specified for puidToSidCacheSize in web.config. Using default value: {1}", LiveIdAuthenticationModule.puidToSidCacheSize, 8192);
						LiveIdAuthenticationModule.puidToSidCacheSize = 8192;
					}
					if (!LiveIdAuthenticationModule.TryReadConfigInt("LiveIdAuthModuleCacheDurationInMinutes", out LiveIdAuthenticationModule.puidToSidCacheDurationInMinutes))
					{
						LiveIdAuthenticationModule.puidToSidCacheDurationInMinutes = 30;
					}
					else if (LiveIdAuthenticationModule.puidToSidCacheDurationInMinutes <= 0)
					{
						ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceDebug<int, int>(0L, "Invalid value: {0} specified for puidToSidCacheDurationInMinutes in web.config. Using default value: {1}", LiveIdAuthenticationModule.puidToSidCacheDurationInMinutes, 30);
						LiveIdAuthenticationModule.puidToSidCacheDurationInMinutes = 30;
					}
					LiveIdAuthenticationModule.puidToPrincipalCache = new MruDictionaryCache<string, LiveIdAuthenticationModule.CacheEntry>(LiveIdAuthenticationModule.puidToSidCacheSize, LiveIdAuthenticationModule.puidToSidCacheDurationInMinutes);
				}
				LiveIdAuthenticationModule.propertyDefinitionArraySID = new PropertyDefinition[]
				{
					ADMailboxRecipientSchema.Sid,
					ADUserSchema.UserPrincipalName,
					ADObjectSchema.OrganizationId,
					ADUserSchema.ExchangeUserAccountControl,
					ADRecipientSchema.RecipientTypeDetails
				};
				LiveIdAuthenticationModule.virtualDirectoryNameWithLeadingSlash = "/" + LiveIdAuthenticationModule.virtualDirectoryName;
				string text4;
				if (!LiveIdAuthenticationModule.TryReadConfigString("MemberNameIgnorePrefixes", out text4))
				{
					ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceDebug<string>(0L, "Using default value for memberNameIgnorePrefixes: {0}", "Live.com#");
					text4 = "Live.com#";
				}
				ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceDebug<string>(0L, "Using string '{0}' to for ignoring member name prefixes.", text4);
				LiveIdAuthenticationModule.memberNameIgnorePrefixes = text4.Split(new char[]
				{
					','
				}, StringSplitOptions.RemoveEmptyEntries);
				ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceDebug(0L, "Live Id Authentication is enabled");
				LiveIdAuthenticationModule.TryReadConfigString("PremiumVanityDomainRealm", out LiveIdAuthenticationModule.premiumVanityDomainRealm);
				return;
			}
			ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceError<string>(0L, "No default url for enterprise site{0}", LiveIdAuthenticationModule.orgOwnSiteName);
			LiveIdAuthenticationModule.liveIdAuthenticationEnabled = false;
		}

		private static bool IsLiveIdAuthenticationEnabled()
		{
			if (!AuthCommon.IsFrontEnd)
			{
				return true;
			}
			bool result;
			try
			{
				ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.FullyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 2096, "IsLiveIdAuthenticationEnabled", "f:\\15.00.1497\\sources\\dev\\clients\\src\\security\\LiveIdAuthenticationModule.cs");
				Server server = topologyConfigurationSession.FindLocalServer();
				string text = HttpRuntime.AppDomainAppId;
				if (text[0] == '/')
				{
					text = text.Substring(1);
				}
				int num = text.IndexOf('/');
				text = text.Substring(num);
				text = string.Format(CultureInfo.InvariantCulture, "IIS://{0}{1}", new object[]
				{
					server.Fqdn,
					text
				});
				num = text.LastIndexOf('/');
				text = IisUtility.GetWebSiteName(text.Substring(0, num));
				ADObjectId descendantId = new ADObjectId(server.DistinguishedName).GetDescendantId("Protocols", "HTTP", new string[]
				{
					string.Format(CultureInfo.InvariantCulture, "{0} ({1})", new object[]
					{
						LiveIdAuthenticationModule.virtualDirectoryName,
						text
					})
				});
				PropertyDefinition[] properties = new PropertyDefinition[]
				{
					ExchangeWebAppVirtualDirectorySchema.LiveIdAuthentication
				};
				ADRawEntry adrawEntry = topologyConfigurationSession.ReadADRawEntry(descendantId, properties);
				result = (bool)adrawEntry[ExchangeWebAppVirtualDirectorySchema.LiveIdAuthentication];
			}
			catch (Exception ex)
			{
				string formatString = "Error occured reading ExchangeWebAppVirtualDirectorySchema.LiveIdAuthentication property from AD. Exception message: {0}";
				ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceError<string>(0L, formatString, ex.Message);
				throw;
			}
			return result;
		}

		private static bool TryReadConfigTimeSpan(string key, out TimeSpan value)
		{
			return TimeSpan.TryParse(ConfigurationManager.AppSettings[key], out value);
		}

		private static bool TryReadConfigString(string key, out string value)
		{
			value = ConfigurationManager.AppSettings[key];
			return !string.IsNullOrEmpty(value);
		}

		private static bool TryReadConfigBool(string key, out bool value)
		{
			return bool.TryParse(ConfigurationManager.AppSettings[key], out value);
		}

		private static bool TryReadConfigInt(string key, out int value)
		{
			return int.TryParse(ConfigurationManager.AppSettings[key], out value);
		}

		private static void SetSkippedAuthFlagOnRequest(HttpContext context)
		{
			context.Items["LiveIdSkippedAuthForAnonResource"] = true;
		}

		private static GenericPrincipal GetPrincipal(LiveIdPropertySet propertySet, HttpContext httpContext, bool includeSofDeleted, out AccountLookupFailureReason accountLookupFailureReason)
		{
			accountLookupFailureReason = AccountLookupFailureReason.ADAccountNotFound;
			httpContext.Items["AdRequestStartTime"] = DateTime.UtcNow;
			GenericPrincipal genericPrincipal;
			if (propertySet.RequestType == RequestType.EcpDelegatedAdminTargetForest)
			{
				genericPrincipal = LiveIdAuthenticationModule.GetCrossForestDelegatedAdminPrincipal(propertySet);
			}
			else
			{
				genericPrincipal = LiveIdAuthenticationModule.GetExchangeAccountPrincipal(propertySet, httpContext, includeSofDeleted, out accountLookupFailureReason);
			}
			if (genericPrincipal != null)
			{
				propertySet.Identity = genericPrincipal.Identity;
				LiveIdAuthenticationModule.LoadOrganzationProperties(propertySet, httpContext);
			}
			httpContext.Items["AdRequestEndTime"] = DateTime.UtcNow;
			return genericPrincipal;
		}

		private static GenericPrincipal GetCrossForestDelegatedAdminPrincipal(LiveIdPropertySet propertySet)
		{
			propertySet.Identity = new GenericIdentity(propertySet.MemberName, "LiveId.EcpDelegatedAdminTargetForest");
			return new GenericPrincipal(propertySet.Identity, null);
		}

		private static GenericPrincipal GetExchangeAccountPrincipal(LiveIdPropertySet propertySet, HttpContext httpContext, bool includeSoftDeleted, out AccountLookupFailureReason accountLookupFailureReason)
		{
			accountLookupFailureReason = AccountLookupFailureReason.ADAccountNotFound;
			GenericPrincipal exchangeAccountPrincipalByPuid = LiveIdAuthenticationModule.GetExchangeAccountPrincipalByPuid(propertySet, httpContext, propertySet.PUID, includeSoftDeleted, out accountLookupFailureReason);
			ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceDebug<string, string>(0L, "Principal lookup with PUID {0} {1}", propertySet.PUID, (exchangeAccountPrincipalByPuid == null) ? "failed" : "succeeded");
			if (exchangeAccountPrincipalByPuid == null && !string.IsNullOrEmpty(propertySet.OrgIdPUID) && !propertySet.OrgIdPUID.Equals(propertySet.PUID))
			{
				exchangeAccountPrincipalByPuid = LiveIdAuthenticationModule.GetExchangeAccountPrincipalByPuid(propertySet, httpContext, propertySet.OrgIdPUID, includeSoftDeleted, out accountLookupFailureReason);
				if (LiveIdAuthenticationModule.puidToSidCacheEnabled && !string.IsNullOrEmpty(propertySet.PUID) && exchangeAccountPrincipalByPuid != null && !includeSoftDeleted)
				{
					LiveIdAuthenticationModule.AddToPuidToSidCache(propertySet.PUID, exchangeAccountPrincipalByPuid);
				}
				httpContext.Response.AppendToLog("&puidlkup2=1");
				ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceDebug<string, string>(0L, "Principal lookup with OrgIdPUID {0} {1}", propertySet.OrgIdPUID, (exchangeAccountPrincipalByPuid == null) ? "failed" : "succeeded");
			}
			return exchangeAccountPrincipalByPuid;
		}

		private static GenericPrincipal GetExchangeAccountPrincipalByPuid(LiveIdPropertySet propertySet, HttpContext httpContext, string puid, bool includeSoftDeleted, out AccountLookupFailureReason accountLookupFailureReason)
		{
			accountLookupFailureReason = AccountLookupFailureReason.ADAccountNotFound;
			GenericPrincipal genericPrincipal = null;
			string text = null;
			string text2 = puid;
			if (propertySet.RequestType == RequestType.EcpByoidAdmin)
			{
				text2 += propertySet.TargetTenant.ToUpperInvariant();
				text = propertySet.TargetTenant;
			}
			if (!LiveIdAuthenticationModule.puidToSidCacheEnabled || !LiveIdAuthenticationModule.TryGetFromPuidToSidCache(puid, out genericPrincipal))
			{
				if (LiveIdAuthenticationModule.puidToSidCacheEnabled)
				{
					LiveIdAuthenticationCounters.TotalFailedLookupsfromCache.IncrementBy(1L);
				}
				string text3 = text ?? new SmtpAddress(propertySet.MemberName).Domain;
				ITenantRecipientSession tenantRecipientSession = null;
				if (!string.IsNullOrEmpty(text3))
				{
					try
					{
						ADSessionSettings adsessionSettings = ADSessionSettings.FromTenantAcceptedDomain(text3);
						adsessionSettings.IncludeSoftDeletedObjects = includeSoftDeleted;
						tenantRecipientSession = DirectorySessionFactory.Default.CreateTenantRecipientSession(null, null, CultureInfo.InvariantCulture.LCID, true, ConsistencyMode.IgnoreInvalid, null, adsessionSettings, 2381, "GetExchangeAccountPrincipalByPuid", "f:\\15.00.1497\\sources\\dev\\clients\\src\\security\\LiveIdAuthenticationModule.cs");
					}
					catch (CannotResolveTenantNameException)
					{
						accountLookupFailureReason = AccountLookupFailureReason.CannotResolveTenantNameException;
						ExTraceGlobals.LiveIdAuthenticationModuleTracer.Information<string>(0L, "Could not resolve recipient session for domain '{0}'", text3);
					}
					catch (CannotResolvePartitionException)
					{
						accountLookupFailureReason = AccountLookupFailureReason.CannotResolvePartitionException;
						ExTraceGlobals.LiveIdAuthenticationModuleTracer.Information<string>(0L, "Could not resolve recipient session for domain '{0}'", text3);
					}
					catch (DataSourceOperationException)
					{
						accountLookupFailureReason = AccountLookupFailureReason.DataSourceOperationException;
						ExTraceGlobals.LiveIdAuthenticationModuleTracer.Information<string>(0L, "Could not create ADSession settings for tenant accepted domain  '{0}'", text3);
					}
				}
				if (tenantRecipientSession != null)
				{
					bool flag = false;
					ADRawEntry adrawEntry = null;
					DateTime utcNow = DateTime.UtcNow;
					try
					{
						adrawEntry = tenantRecipientSession.FindUniqueEntryByNetID(puid, text, LiveIdAuthenticationModule.propertyDefinitionArraySID);
					}
					catch (NonUniqueRecipientException arg)
					{
						accountLookupFailureReason = AccountLookupFailureReason.NonUniqueRecipientException;
						ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceError<string, NonUniqueRecipientException>(0L, "Found multiple AD accounts mapped to the PUID: {0} exception {1}", puid, arg);
						flag = true;
					}
					finally
					{
						LogonLatencyLogger.UpdateCookie(httpContext, "ADL", utcNow);
					}
					if (adrawEntry == null && !flag)
					{
						ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceError<string>(0L, "Could not find any AD account mapped to the PUID: {0}", puid);
					}
					else if (adrawEntry != null)
					{
						string text4 = adrawEntry[ADMailboxRecipientSchema.Sid].ToString();
						if (string.IsNullOrEmpty(text4))
						{
							accountLookupFailureReason = AccountLookupFailureReason.EmptySid;
							ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceError<string>(0L, "The SID of the AD account mapped to the PUID: {0}, is null or empty", puid);
						}
						else if (((UserAccountControlFlags)adrawEntry[ADUserSchema.ExchangeUserAccountControl] & UserAccountControlFlags.AccountDisabled) == UserAccountControlFlags.AccountDisabled)
						{
							if (((RecipientTypeDetails)adrawEntry[ADRecipientSchema.RecipientTypeDetails] & RecipientTypeDetails.SharedMailbox) == RecipientTypeDetails.SharedMailbox)
							{
								accountLookupFailureReason = AccountLookupFailureReason.SharedMailboxAccountDisabled;
								ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceError<string>(0L, "The AD account mapped to the PUID: {0}, is disabled and it is a shared mailbox.", puid);
							}
							else
							{
								accountLookupFailureReason = AccountLookupFailureReason.AccountDisabled;
								ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceError<string>(0L, "The account of the AD account mapped to the PUID: {0}, is disabled.", puid);
							}
						}
						else
						{
							string userPrincipal = adrawEntry[ADUserSchema.UserPrincipalName].ToString();
							OrganizationId organizationId = (OrganizationId)adrawEntry[ADObjectSchema.OrganizationId];
							string partitionId = null;
							if (organizationId != null && organizationId.PartitionId != null)
							{
								partitionId = organizationId.PartitionId.ToString();
							}
							LiveIDIdentity liveIDIdentity = new LiveIDIdentity(userPrincipal, text4, propertySet.MemberName, partitionId, new LiveIdLoginAttributes(propertySet.LoginAttributes), null);
							liveIDIdentity.UserOrganizationId = organizationId;
							OrganizationProperties userOrganizationProperties;
							OrganizationPropertyCache.TryGetOrganizationProperties(liveIDIdentity.UserOrganizationId, out userOrganizationProperties);
							liveIDIdentity.UserOrganizationProperties = userOrganizationProperties;
							genericPrincipal = new GenericPrincipal(liveIDIdentity, null);
							if (LiveIdAuthenticationModule.puidToSidCacheEnabled)
							{
								LiveIdAuthenticationModule.AddToPuidToSidCache(text2, genericPrincipal);
							}
						}
					}
				}
			}
			else
			{
				LiveIdAuthenticationCounters.TotalRetrievalsfromCache.IncrementBy(1L);
				if (string.IsNullOrEmpty(((LiveIDIdentity)genericPrincipal.Identity).Sid.ToString()))
				{
					ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceError<string>(0L, "The SID of the AD account mapped to the PUID: {0}, is null or empty", puid);
				}
			}
			if (AuthCommon.IsFrontEnd && genericPrincipal != null)
			{
				LiveIdAuthenticationModule.GenerateCommonAccessToken(httpContext, genericPrincipal.Identity as LiveIDIdentity, puid);
			}
			return genericPrincipal;
		}

		private static void LoadOrganzationProperties(LiveIdPropertySet propertySet, HttpContext httpContext)
		{
			switch (propertySet.RequestType)
			{
			case RequestType.EcpDelegatedAdmin:
			case RequestType.EcpDelegatedAdminTargetForest:
				LiveIdAuthenticationModule.LoadOrganzationPropertiesForDelegatedAdmin(propertySet);
				break;
			default:
				propertySet.OrganizationProperties = ((LiveIDIdentity)propertySet.Identity).UserOrganizationProperties;
				break;
			}
			if (propertySet.OrganizationProperties == null)
			{
				ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceError<string>(0L, "LoadOrganizationProperties() - Could not find tenantOrg for user '{0}'.", propertySet.MemberName);
				LiveIdAuthenticationModule.RaiseOrgIdMailboxNotFoundException(propertySet.MemberName, httpContext, AccountLookupFailureReason.OrganizationNotFound, null);
			}
		}

		private static void LoadOrganzationPropertiesForDelegatedAdmin(LiveIdPropertySet propertySet)
		{
			SmtpDomain smtpDomain = null;
			try
			{
				smtpDomain = new SmtpDomain(propertySet.TargetTenant);
			}
			catch (FormatException)
			{
			}
			if (smtpDomain != null)
			{
				OrganizationId organizationId = DomainToOrganizationIdCache.Singleton.Get(smtpDomain);
				if (organizationId != null)
				{
					OrganizationProperties organizationProperties;
					OrganizationPropertyCache.TryGetOrganizationProperties(organizationId, out organizationProperties);
					propertySet.OrganizationProperties = organizationProperties;
				}
			}
			if (propertySet.OrganizationProperties == null && propertySet.RequestType == RequestType.EcpDelegatedAdmin)
			{
				propertySet.OrganizationProperties = ((LiveIDIdentity)propertySet.Identity).UserOrganizationProperties;
			}
		}

		private static void SendOrgIdLogoutConfirmation(HttpContext httpContext, string siteName)
		{
			try
			{
				httpContext.Response.ContentType = "image/gif";
				httpContext.Response.BinaryWrite(LiveIdAuthenticationModule.logoutResponseImageByteArray);
				httpContext.Response.Flush();
			}
			finally
			{
				httpContext.ApplicationInstance.CompleteRequest();
			}
		}

		private static void ClearCookies(HttpResponse httpResponse, string[] cookies)
		{
			foreach (string name in cookies)
			{
				LiveIdAuthentication.DeleteCookie(httpResponse, name);
			}
		}

		private static void ClearAuthCookies(HttpContext httpContext, string siteName)
		{
			LiveIdAuthenticationModule.WhileHandlingAndRetryingLiveIdErrors("ClearAuthCookies", httpContext, delegate
			{
				LiveIdAuthentication.Logout(httpContext, siteName, LiveIdAuthenticationModule.IsConsumerRequestForO365(httpContext.Request));
				LiveIdAuthenticationModule.ClearCookies(httpContext.Response, LiveIdAuthenticationModule.UserCookies);
				return true;
			}, null);
			httpContext.Response.AddHeader("P3P", "CP=\"ALL IND DSP COR ADM CONo CUR CUSo IVAo IVDo PSA PSD TAI TELo OUR SAMo CNT COM INT NAV ONL PHY PRE PUR UNI\"");
		}

		private static string GetExternalUrlScheme()
		{
			return "https";
		}

		private static void SaveAuthCookieAndRedirectAsHttpGet(HttpContext httpContext, string rpsRespHeaders, Uri uri, string upn, string orgIdPuid)
		{
			bool isSessionDataPreloadEnabled = LiveIdAuthenticationModule.IsSessionDataPreloadEnabled(httpContext);
			bool isConsumerRequestForO365 = LiveIdAuthenticationModule.IsConsumerRequestForO365(httpContext.Request);
			LiveIdAuthenticationModule.WhileHandlingAndRetryingLiveIdErrors("SaveAuthCookieAndRedirectAsHttpGet", httpContext, delegate
			{
				httpContext.Response.AddHeader("P3P", "CP=\"ALL IND DSP COR ADM CONo CUR CUSo IVAo IVDo PSA PSD TAI TELo OUR SAMo CNT COM INT NAV ONL PHY PRE PUR UNI\"");
				LiveIdAuthentication.WriteHeadersToResponse(httpContext, rpsRespHeaders, isConsumerRequestForO365);
				if (!isSessionDataPreloadEnabled)
				{
					LiveIdAuthenticationModule.ClearCookies(httpContext.Response, LiveIdAuthenticationModule.UserCookies);
				}
				return true;
			}, null);
			LiveIdAuthenticationModule.SetDefaultAnchorMailboxCookie(httpContext, upn);
			if (isConsumerRequestForO365)
			{
				Utilities.SetCookie(httpContext, "O365Consumer", "1", null);
			}
			if (isSessionDataPreloadEnabled && uri.AbsolutePath.EndsWith("/owa/", StringComparison.OrdinalIgnoreCase))
			{
				PreloadSessionDataRequestCreator.CreateAsyncRequest(httpContext, orgIdPuid, upn, rpsRespHeaders);
			}
			using (AesCryptoServiceProvider aesCryptoServiceProvider = new AesCryptoServiceProvider())
			{
				HttpCookie cookie = new HttpCookie("SuiteServiceProxyKey", string.Format("{0}&{1}", Convert.ToBase64String(aesCryptoServiceProvider.Key), Convert.ToBase64String(aesCryptoServiceProvider.IV)));
				httpContext.Response.Cookies.Set(cookie);
			}
			if (!httpContext.Request.HttpMethod.Equals("GET", StringComparison.OrdinalIgnoreCase))
			{
				if (uri.Scheme.Equals("http", StringComparison.OrdinalIgnoreCase))
				{
					StringBuilder stringBuilder = new StringBuilder();
					stringBuilder.Append(LiveIdAuthenticationModule.GetExternalUrlScheme());
					stringBuilder.Append("://");
					stringBuilder.Append(uri.Host);
					stringBuilder.Append(uri.PathAndQuery);
					httpContext.Response.Redirect(stringBuilder.ToString());
					return;
				}
				if (httpContext.Request.Path.Equals("/owa/SuiteServiceProxy.aspx", StringComparison.OrdinalIgnoreCase))
				{
					ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceDebug(0L, "Marking the url as Authed so we can detect cookie drop scenarios");
					string url = uri.OriginalString + "&Authed=1";
					httpContext.Response.Redirect(url);
					return;
				}
				string text;
				string url2 = UrlUtilities.ShouldRedirectQueryParamsAsHashes(uri, out text) ? text : uri.OriginalString;
				httpContext.Response.Redirect(url2);
			}
		}

		private static void RedirectToLiveLogin(HttpContext httpContext, string siteName, string hostName, string exclusionHostPrefix, string authPolicy, bool isTimeout = false, bool useSilentAuthentication = false)
		{
			LiveIdAuthenticationModule.WhileHandlingAndRetryingLiveIdErrors("RedirectToLiveLogin", httpContext, delegate
			{
				string text = httpContext.Request.QueryString["realm"];
				string userName = httpContext.Request.QueryString[Utilities.UserNameParameter];
				string text2 = null;
				if (!string.IsNullOrEmpty(text))
				{
					text2 = HttpUtility.UrlDecode(text);
					if (!string.IsNullOrEmpty(text2) && text2.Equals(LiveIdAuthenticationModule.o365Namespace, StringComparison.OrdinalIgnoreCase))
					{
						text2 = null;
					}
				}
				if (httpContext.Request.Cookies["exchangecookie"] == null)
				{
					Utilities.SetCookie(httpContext, "exchangecookie", Guid.NewGuid().ToString("N"), null);
				}
				if (string.IsNullOrEmpty(authPolicy) && !string.IsNullOrEmpty(LiveIdAuthenticationModule.AuthPolicyOverrideValue))
				{
					authPolicy = LiveIdAuthenticationModule.AuthPolicyOverrideValue;
				}
				Uri requestUrlEvenIfProxied = httpContext.Request.GetRequestUrlEvenIfProxied();
				string text3 = LiveIdAuthentication.GetAuthenticateRedirectUrl(LiveIdAuthenticationModule.CreateRedirectUrl(requestUrlEvenIfProxied, hostName, exclusionHostPrefix), siteName, authPolicy, text2, userName, true, useSilentAuthentication, LiveIdAuthenticationModule.IsConsumerRequestForO365(httpContext.Request));
				if (isTimeout && HttpRuntime.AppDomainAppVirtualPath.Equals("/ecp", StringComparison.OrdinalIgnoreCase))
				{
					text3 = "/ecp/auth/TimeoutLogout.aspx?ru=" + HttpUtility.UrlEncode(text3);
				}
				httpContext.Response.Redirect(text3);
				return true;
			}, null);
		}

		private static string CreateRedirectUrl(Uri requestUri, string hostName, string exclusionHostPrefix)
		{
			UriBuilder uriBuilder = new UriBuilder(requestUri.OriginalString);
			if (exclusionHostPrefix != null && requestUri.Host.StartsWith(exclusionHostPrefix, StringComparison.OrdinalIgnoreCase))
			{
				uriBuilder.Host = hostName;
			}
			if (requestUri.AbsolutePath.EndsWith("errorfe.aspx", StringComparison.OrdinalIgnoreCase))
			{
				uriBuilder.Path = "/owa";
				uriBuilder.Query = null;
			}
			return uriBuilder.Uri.AbsoluteUri;
		}

		private static void RedirectToLiveLogout(HttpContext httpContext, string siteNameParam, string hostName, string exclusionHostPrefix, bool isTimeout = false, bool clearCookies = true)
		{
			LiveIdAuthenticationModule.WhileHandlingAndRetryingLiveIdErrors("RedirectToLiveLogout", httpContext, delegate
			{
				if (clearCookies)
				{
					LiveIdAuthenticationModule.ClearAuthCookies(httpContext, siteNameParam);
				}
				LiveIdAuthenticationModule.AddRealmParameterFromCookie(httpContext);
				string uriString = LiveIdAuthenticationModule.GetLogoutReturnUrlFromRequest(httpContext.Request);
				UriBuilder uriBuilder = new UriBuilder(new Uri(uriString));
				if (!string.IsNullOrEmpty(LiveIdAuthenticationModule.o365SiteName) && !LiveIdAuthenticationModule.IsPersonalInstanceUrl(httpContext))
				{
					uriBuilder.Host = LiveIdAuthenticationModule.orgOwnSiteName;
				}
				if (uriBuilder.Query != null && uriBuilder.Query.Length > 1)
				{
					StringBuilder stringBuilder = new StringBuilder(100);
					stringBuilder.Append(uriBuilder.Query.Substring(1)).Append("&").Append("exch=1");
					uriBuilder.Query = stringBuilder.ToString();
				}
				else
				{
					uriBuilder.Query = "exch=1";
				}
				uriString = uriBuilder.ToString();
				string text = LiveIdAuthentication.GetLiveLogoutRedirectUrl(LiveIdAuthenticationModule.CreateRedirectUrl(new Uri(uriString), hostName, exclusionHostPrefix), siteNameParam, LiveIdAuthenticationModule.IsConsumerRequestForO365(httpContext.Request));
				if (isTimeout && HttpRuntime.AppDomainAppVirtualPath.Equals("/ecp", StringComparison.OrdinalIgnoreCase))
				{
					text = "/ecp/auth/TimeoutLogout.aspx?signout=1&ru=" + HttpUtility.UrlEncode(text);
				}
				httpContext.Response.Redirect(text);
				return true;
			}, null);
		}

		private static void SaveOrganizationNameCookie(HttpContext httpContext, string organizationName)
		{
			Utilities.SetCookie(httpContext, "orgName", organizationName, null);
			LiveIdAuthenticationModule.SaveDomainNameCookie(httpContext, string.Empty);
		}

		private static void SaveDomainNameCookie(HttpContext httpContext, string domainName)
		{
			Utilities.SetCookie(httpContext, "domainName", domainName, null);
		}

		private static string GetLiveIdNewMailUrl(HttpContext httpContext)
		{
			int num = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
			httpContext.Response.AppendToLog("&hm=1");
			string text = httpContext.Request.GetRequestUrlEvenIfProxied().PathAndQuery.Substring(1);
			string str = text.EndsWith("/") ? text : (text + "/");
			return string.Format(LiveIdAuthenticationModule.liveIdAuthNewMailUrl, num, HttpUtility.UrlEncode(HttpUtility.UrlEncode(str)));
		}

		private static void RedirectToLiveIdNewMailUrl(HttpContext httpContext)
		{
			int num = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
			httpContext.Response.AppendToLog("&hm=1");
			string text = httpContext.Request.GetRequestUrlEvenIfProxied().PathAndQuery.Substring(1);
			string url = string.Format(LiveIdAuthenticationModule.liveIdAuthNewMailUrl, num, HttpUtility.UrlEncode(HttpUtility.UrlEncode(text.EndsWith("/") ? text : (text + "/"))));
			httpContext.Response.Redirect(url);
		}

		private static bool ShouldRedirectToLiveIdNewMailUrl(HttpContext httpContext)
		{
			if (!LiveIdAuthenticationModule.enableHotmailRedirect || string.IsNullOrEmpty(LiveIdAuthenticationModule.liveIdAuthNewMailUrl))
			{
				return false;
			}
			Uri requestUrlEvenIfProxied = httpContext.Request.GetRequestUrlEvenIfProxied();
			if (LiveIdAuthenticationModule.IsRequestForExchange(requestUrlEvenIfProxied))
			{
				return false;
			}
			string value = httpContext.Request.Params["hm"];
			if (!string.IsNullOrEmpty(value))
			{
				return false;
			}
			if (requestUrlEvenIfProxied.Segments.Length > 1)
			{
				bool flag = requestUrlEvenIfProxied.Segments[1].Equals("ecp/", StringComparison.OrdinalIgnoreCase);
				if (flag)
				{
					return false;
				}
			}
			string text;
			bool flag2 = LiveIdAuthenticationModule.TryGetExplicitLogonUrlSegment(requestUrlEvenIfProxied, out text);
			return !flag2;
		}

		private static void RedirectToEducationPage(HttpContext httpContext)
		{
			string text = httpContext.Request.Params[Utilities.LiveIdUrlParameter];
			if (!string.IsNullOrEmpty(text))
			{
				string userDomain = Utilities.GetUserDomain(text);
				if (!string.IsNullOrEmpty(userDomain) && SmtpAddress.IsValidDomain(userDomain))
				{
					StringBuilder stringBuilder = new StringBuilder(180);
					stringBuilder.Append("https");
					stringBuilder.Append("://");
					stringBuilder.Append(LiveIdAuthenticationModule.GetOrgOwnSiteName(httpContext));
					stringBuilder.Append("/");
					stringBuilder.Append(userDomain);
					UrlUtilities.RewriteParameterInURL(httpContext, Utilities.EducationUrlParameter, stringBuilder.ToString());
				}
			}
			else
			{
				string value = httpContext.Request.Params[Utilities.DestinationUrlParameter];
				if (string.IsNullOrEmpty(value))
				{
					UrlUtilities.RewriteParameterInURL(httpContext, Utilities.DestinationUrlParameter, httpContext.Request.GetRequestUrlEvenIfProxied().ToString());
				}
			}
			Utilities.ExecutePageAndCompleteRequest(httpContext, "/LIDAuth/Education.aspx");
		}

		private static bool IsEducationPagePost(HttpContext httpContext)
		{
			return httpContext.Request.Params[Utilities.LiveIdUrlParameter] != null;
		}

		private static void AddRealmParameter(HttpContext httpContext)
		{
			string text = httpContext.Request.QueryString["realm"];
			Uri requestUrlEvenIfProxied = httpContext.Request.GetRequestUrlEvenIfProxied();
			if (!string.IsNullOrEmpty(text))
			{
				LiveIdAuthenticationModule.SaveDomainNameCookie(httpContext, text);
				if (text.Equals(LiveIdAuthenticationModule.eduNamespaceKey, StringComparison.OrdinalIgnoreCase) && requestUrlEvenIfProxied.Segments.Length >= 2 && requestUrlEvenIfProxied.Segments[1].Equals("ecp/", StringComparison.OrdinalIgnoreCase))
				{
					string path = LiveIdAuthenticationModule.regexEduRealmParameter.Replace(requestUrlEvenIfProxied.PathAndQuery, LiveIdAuthenticationModule.eduWHRRealmParameter);
					httpContext.RewritePath(path);
				}
				return;
			}
			if (!string.IsNullOrEmpty(LiveIdAuthenticationModule.premiumVanityDomainRealm))
			{
				text = LiveIdAuthenticationModule.premiumVanityDomainRealm;
				LiveIdAuthenticationModule.SaveDomainNameCookie(httpContext, text);
			}
			string tenantSpecificUrl = LiveIdAuthenticationModule.GetTenantSpecificUrl(httpContext);
			if (!string.IsNullOrEmpty(tenantSpecificUrl))
			{
				UrlUtilities.RewriteDomainFromTenantSpecificURL(httpContext, tenantSpecificUrl);
				return;
			}
			if (UrlUtilities.IsSpecialDomainUrl(requestUrlEvenIfProxied, LiveIdAuthenticationModule.eduNamespaceKey))
			{
				UrlUtilities.RewriteRealmParameterInURL(httpContext, LiveIdAuthenticationModule.eduWHRKey);
				return;
			}
			string text2;
			UrlUtilities.RewriteFederatedDomainInURL(httpContext, out text2);
			if (!string.IsNullOrEmpty(text2))
			{
				LiveIdAuthenticationModule.SaveDomainNameCookie(httpContext, text2);
			}
		}

		private static void AddRealmParameterForSDF(HttpContext httpContext, bool isConsumerO365)
		{
			if (isConsumerO365)
			{
				return;
			}
			string host = httpContext.Request.GetRequestUrlEvenIfProxied().Host;
			if (LiveIdAuthenticationModule.sdfSiteNames != null && !string.IsNullOrEmpty(host))
			{
				foreach (string value in LiveIdAuthenticationModule.sdfSiteNames)
				{
					if (host.Contains(value))
					{
						UrlUtilities.RewriteRealmParameterInURL(httpContext, "microsoft.com");
						return;
					}
				}
			}
		}

		private static void AddRealmParameterFromCookie(HttpContext httpContext)
		{
			string value = httpContext.Request.QueryString["realm"];
			if (string.IsNullOrEmpty(value))
			{
				HttpCookie httpCookie = httpContext.Request.Cookies["orgName"];
				if (httpCookie != null && !string.IsNullOrEmpty(httpCookie.Value))
				{
					UrlUtilities.RewriteRealmParameterInURL(httpContext, httpCookie.Value);
					return;
				}
				HttpCookie httpCookie2 = httpContext.Request.Cookies["domainName"];
				if (httpCookie2 != null && !string.IsNullOrEmpty(httpCookie2.Value))
				{
					UrlUtilities.RewriteRealmParameterInURL(httpContext, httpCookie2.Value);
				}
			}
		}

		private static void AddConsumerRealmParameter(HttpContext httpContext)
		{
			string value = httpContext.Request.QueryString["realm"];
			if (!string.IsNullOrEmpty(value))
			{
				return;
			}
			UrlUtilities.RewriteRealmParameterInURL(httpContext, LiveIdAuthenticationModule.eduWHRKey);
		}

		private static void AddRealmParameterFromRpsMemberName(HttpContext httpContext, string membername)
		{
			string value = httpContext.Request.QueryString["realm"];
			if (!string.IsNullOrEmpty(value))
			{
				return;
			}
			if (!string.IsNullOrEmpty(membername))
			{
				string userDomain = Utilities.GetUserDomain(membername);
				UrlUtilities.RewriteRealmParameterInURL(httpContext, userDomain);
			}
		}

		private static bool TryGetExplicitLogonUrlSegment(Uri url, out string explicitLogonSegment)
		{
			explicitLogonSegment = string.Empty;
			string originalString = url.OriginalString;
			string text = LiveIdAuthenticationModule.virtualDirectoryName + "/";
			int num = originalString.IndexOf(text) + text.Length;
			if (num < 0 || num >= originalString.Length)
			{
				return false;
			}
			int num2 = originalString.IndexOf("/", num);
			if (num2 == -1)
			{
				return false;
			}
			int length = num2 - num;
			explicitLogonSegment = originalString.Substring(num, length);
			int num3 = explicitLogonSegment.IndexOf('@');
			return num3 > 0 && num3 < explicitLogonSegment.Length - 2;
		}

		private static string GetVirtualDirectoryUrl(Uri url, bool addCobrandingRedirectPage)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(LiveIdAuthenticationModule.GetExternalUrlScheme());
			stringBuilder.Append("://");
			stringBuilder.Append(url.Host);
			stringBuilder.Append("/");
			if (addCobrandingRedirectPage)
			{
				stringBuilder.Append("owa/auth/cobrandingredir.aspx");
			}
			else
			{
				stringBuilder.Append(LiveIdAuthenticationModule.virtualDirectoryName);
				stringBuilder.Append("/");
			}
			return stringBuilder.ToString();
		}

		private static bool IsLogoffRequest(Uri url)
		{
			return url.LocalPath.EndsWith(LiveIdAuthenticationModule.logoffPage, StringComparison.OrdinalIgnoreCase);
		}

		private static bool IsCobrandingRedirectPageRequest(Uri url)
		{
			return url.LocalPath.EndsWith("cobrandingredir.aspx", StringComparison.OrdinalIgnoreCase);
		}

		private static bool HasLiveLogoffParameter(Uri url)
		{
			return url.Query.Contains("exlive=1");
		}

		private static bool IsReferalFromTdsLiveLogout(HttpContext httpContext)
		{
			bool result = false;
			if (httpContext != null && httpContext.Request != null)
			{
				string text = LiveIdAuthenticationModule.orgOwnSiteName ?? string.Empty;
				if (-1 != text.IndexOf("live-int.com", StringComparison.OrdinalIgnoreCase))
				{
					Uri urlReferrer = httpContext.Request.UrlReferrer;
					if (urlReferrer != null)
					{
						string text2 = urlReferrer.AbsolutePath ?? string.Empty;
						if (-1 != text2.IndexOf("logout.srf", StringComparison.OrdinalIgnoreCase))
						{
							result = true;
						}
					}
				}
			}
			return result;
		}

		private static bool IsLogoffFromExchange(Uri url)
		{
			return url.Query.Contains("src=exch");
		}

		private static bool IsRequestForExchange(Uri url)
		{
			return url.Query.Contains("exch=1");
		}

		private static string GetTenantSpecificUrl(HttpContext httpContext)
		{
			return httpContext.Request.QueryString["targetName"];
		}

		private static bool IsPersonalInstanceUrl(HttpContext httpContext)
		{
			return !LiveIdAuthenticationModule.MatchSiteName(httpContext, LiveIdAuthenticationModule.o365SiteName) || string.IsNullOrEmpty(LiveIdAuthenticationModule.o365SiteName);
		}

		private static string GetLogoutReturnUrlFromRequest(HttpRequest httpRequest)
		{
			if (httpRequest == null)
			{
				return null;
			}
			Uri uri = null;
			Uri requestUrlEvenIfProxied = httpRequest.GetRequestUrlEvenIfProxied();
			string text = httpRequest.QueryString["ru"];
			if (!string.IsNullOrEmpty(text))
			{
				if (!LiveIdAuthenticationModule.ValidateReturnUrl(requestUrlEvenIfProxied, text, out uri))
				{
					uri = null;
				}
			}
			else if (LiveIdAuthenticationModule.ShouldSaveUrlOnLogoff(requestUrlEvenIfProxied))
			{
				uri = requestUrlEvenIfProxied;
			}
			string text2;
			if (uri == null)
			{
				bool addCobrandingRedirectPage = false;
				if (httpRequest.Cookies["CoTS"] != null)
				{
					addCobrandingRedirectPage = true;
				}
				text2 = LiveIdAuthenticationModule.GetVirtualDirectoryUrl(httpRequest.GetRequestUrlEvenIfProxied(), addCobrandingRedirectPage);
				string text3 = httpRequest.QueryString["realm"];
				if (!string.IsNullOrEmpty(text3))
				{
					UriBuilder uriBuilder = new UriBuilder(text2);
					if (uriBuilder.Query != null && uriBuilder.Query.Length > 1)
					{
						StringBuilder stringBuilder = new StringBuilder(100);
						stringBuilder.Append(uriBuilder.Query.Substring(1)).Append("&").AppendFormat("{0}={1}", "realm", text3);
						uriBuilder.Query = stringBuilder.ToString();
					}
					else
					{
						uriBuilder.Query = string.Format("{0}={1}", "realm", text3);
					}
					text2 = uriBuilder.Uri.AbsoluteUri;
				}
			}
			else
			{
				StringBuilder stringBuilder2 = new StringBuilder(64);
				stringBuilder2.Append(LiveIdAuthenticationModule.GetExternalUrlScheme());
				stringBuilder2.Append("://");
				stringBuilder2.Append(uri.Host);
				stringBuilder2.Append(uri.PathAndQuery);
				text2 = stringBuilder2.ToString();
			}
			return text2;
		}

		private static bool ShouldSaveUrlOnLogoff(Uri url)
		{
			return LiveIdAuthenticationModule.returnToOringinalUrlByDefault || url.Query.Contains("exsvurl=1") || url.Query.Contains("rru=contacts");
		}

		private static bool ValidateReturnUrl(Uri requestUrl, string returnUrl, out Uri returnUrlUri)
		{
			returnUrlUri = null;
			if (requestUrl == null)
			{
				ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceDebug(0L, "currentUri is null.");
				return false;
			}
			if (string.IsNullOrEmpty(returnUrl))
			{
				ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceDebug(0L, "returnUrl is null.");
				return false;
			}
			if (!Uri.TryCreate(returnUrl, UriKind.Absolute, out returnUrlUri))
			{
				ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceDebug<string>(0L, "Fail to create Uri from return url {0}.", returnUrl);
				return false;
			}
			if (StringComparer.InvariantCultureIgnoreCase.Compare(requestUrl.Host, returnUrlUri.Host) != 0)
			{
				ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceDebug<string, string>(0L, "The host of current url {0} and return url {1} are not the same.", requestUrl.OriginalString, returnUrl);
				return false;
			}
			if (returnUrlUri.AbsolutePath == null || !returnUrlUri.AbsolutePath.StartsWith(LiveIdAuthenticationModule.virtualDirectoryNameWithLeadingSlash))
			{
				ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceDebug<string, string>(0L, "The vdir of current url {0} and return url {1} are not the same.", requestUrl.OriginalString, returnUrl);
				return false;
			}
			return true;
		}

		private static bool HasSessionTimedOut(uint issueInstant)
		{
			return LiveIdAuthenticationModule.SecondsSinceInstant(issueInstant) > (long)LiveIdAuthenticationModule.timeoutIntervalInSeconds;
		}

		private static long SecondsSinceInstant(uint instant)
		{
			long num = (DateTime.UtcNow.Ticks - LiveIdAuthenticationModule.ticksBefore1970) / 10000000L;
			return num - (long)((ulong)instant);
		}

		private static bool HandleOmeLogoffRequest(HttpContext httpContext)
		{
			string text = "https://" + LiveIdAuthenticationModule.o365SiteName + "/encryption/";
			string text2 = text + "signedoutpage.aspx";
			Uri requestUrlEvenIfProxied = httpContext.Request.GetRequestUrlEvenIfProxied();
			if (requestUrlEvenIfProxied.LocalPath.EndsWith("/ome.aspx", StringComparison.OrdinalIgnoreCase))
			{
				string text3 = httpContext.Request.QueryString["ru"];
				if (string.IsNullOrWhiteSpace(text3) || !text3.StartsWith(text, StringComparison.OrdinalIgnoreCase))
				{
					text3 = text2;
				}
				ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceDebug<string>(0L, "Handle OME logoff request. Redirect to : {0}", text3);
				httpContext.Response.AppendToLog("&LogoffReason=OMELogoff");
				LiveIdAuthenticationModule.SetSkippedAuthFlagOnRequest(httpContext);
				httpContext.Response.Redirect(text3);
				return true;
			}
			return false;
		}

		private static bool HandleLogoffRequest(HttpContext httpContext)
		{
			Uri requestUrlEvenIfProxied = httpContext.Request.GetRequestUrlEvenIfProxied();
			if (LiveIdAuthenticationModule.IsLogoffRequest(requestUrlEvenIfProxied))
			{
				ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceDebug<Uri>(0L, "Logoff request {0}", requestUrlEvenIfProxied);
				if (LiveIdAuthenticationModule.HasLiveLogoffParameter(requestUrlEvenIfProxied))
				{
					ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceDebug<Uri>(0L, "This is a logoff request and has live logoff param: {0}", requestUrlEvenIfProxied);
					return true;
				}
				if (LiveIdAuthenticationModule.IsLogoffFromExchange(requestUrlEvenIfProxied))
				{
					ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceDebug<Uri>(0L, "HandleLogoffRequest(): Logoff request, redirect to live logout: {0}", requestUrlEvenIfProxied);
					httpContext.Response.AppendToLog("&LogoffReason=LogoffRequestExchange");
					LiveIdAuthenticationModule.WriteAuthenticatedUserToIISLog(httpContext);
					LiveIdAuthenticationModule.RedirectToLiveLogout(httpContext, LiveIdAuthenticationModule.GetOrgOwnSiteName(httpContext), LiveIdAuthenticationModule.GetOrgOwnSiteReturnUrlHost(httpContext), LiveIdAuthenticationModule.iOwnSiteDNSSubdomain, false, true);
				}
				else
				{
					ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceDebug<Uri>(0L, "HandleLogoffRequest(): Live logoff. Clear auth cookies, redirect to live login: {0}", requestUrlEvenIfProxied);
					httpContext.Response.AppendToLog("&LogoffReason=LogoffRequestOrgId");
					LiveIdAuthenticationModule.WriteAuthenticatedUserToIISLog(httpContext);
					LiveIdAuthenticationModule.ClearAuthCookies(httpContext, LiveIdAuthenticationModule.GetOrgOwnSiteName(httpContext));
					if (LogOnSettings.IsLegacyLogOff)
					{
						LiveIdAuthenticationModule.RedirectToLiveLogin(httpContext, LiveIdAuthenticationModule.GetOrgOwnSiteName(httpContext), LiveIdAuthenticationModule.GetOrgOwnSiteReturnUrlHost(httpContext), LiveIdAuthenticationModule.iOwnSiteDNSSubdomain, null, false, false);
					}
					else
					{
						httpContext.Response.Redirect("/owa/auth/signout.aspx");
					}
				}
			}
			return false;
		}

		private static void WriteAuthenticatedUserToIISLog(HttpContext httpContext)
		{
			if (httpContext.User != null)
			{
				LiveIDIdentity liveIDIdentity = httpContext.User.Identity as LiveIDIdentity;
				if (liveIDIdentity != null)
				{
					httpContext.Response.AppendToLog(string.Format("&AuthenticatedUser={0}", liveIDIdentity.MemberName));
				}
			}
		}

		private static void HandleActivityTimeout(HttpContext httpContext, string membername)
		{
			ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceDebug(0L, "HandleActivityTimeout");
			httpContext.Response.AppendToLog("&LogoffReason=ActivityTimeout");
			if (Utilities.Need440Response(httpContext.Request))
			{
				ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceDebug(0L, "Session timed out, need 440, render 440 response timeout");
				Utilities.Render440TimeoutResponse(httpContext.Response, httpContext.Request.HttpMethod, httpContext);
			}
			LiveIdAuthenticationModule.AddRealmParameterFromRpsMemberName(httpContext, membername);
			ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceDebug<Uri>(0L, "HandleActivityTimeout(): Redirect to live login. User tried to access url {0}", httpContext.Request.GetRequestUrlEvenIfProxied());
			LiveIdAuthenticationModule.WriteAuthenticatedUserToIISLog(httpContext);
			LiveIdAuthenticationModule.RedirectToLiveLogin(httpContext, LiveIdAuthenticationModule.GetOrgOwnSiteName(httpContext), LiveIdAuthenticationModule.GetOrgOwnSiteReturnUrlHost(httpContext), LiveIdAuthenticationModule.iOwnSiteDNSSubdomain, LiveIdAuthenticationModule.timeoutRedirectPolicy, true, false);
		}

		private static void ValidateWithSlidingWindow(HttpContext httpContext, string rpsRespHeaders, TimeSpan timeoutInterval, bool timeoutWithSSOEnabled, uint rpsTicketType, uint issueInstant, string membername, RPSTicket rpsTicket)
		{
			TimeSpan timeSpan;
			if (LiveIdAuthenticationModule.slidingWindowOverride != TimeSpan.Zero)
			{
				timeSpan = LiveIdAuthenticationModule.slidingWindowOverride;
			}
			else
			{
				timeSpan = timeoutInterval;
			}
			ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceDebug<TimeSpan, uint>(0L, "slidingWindow is: {0} and rpsTicket.TicketType is {1}", timeSpan, rpsTicketType);
			if (timeSpan > LiveIdAuthenticationModule.MaxSlidingWindow)
			{
				timeSpan = LiveIdAuthenticationModule.MaxSlidingWindow;
			}
			if (rpsTicketType == 3U || rpsTicketType == 4U)
			{
				if (!LiveIdAuthentication.ValidateWithSlidingWindow(rpsTicket, timeSpan))
				{
					ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceDebug(0L, "LiveIdAuthentication.ValidateWithSlidingWindow failed");
					LiveIdAuthenticationModule.HandleActivityTimeout(httpContext, membername);
				}
				if (OwaAuthenticationHelper.IsOwaUserActivityRequest(httpContext.Request) && !string.IsNullOrEmpty(rpsRespHeaders))
				{
					ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceDebug(0L, "Include appropriate P3P compact policy header for IE clients to accept 3rd party set-cookie headers");
					httpContext.Response.AddHeader("P3P", "CP=\"ALL IND DSP COR ADM CONo CUR CUSo IVAo IVDo PSA PSD TAI TELo OUR SAMo CNT COM INT NAV ONL PHY PRE PUR UNI\"");
					LiveIdAuthentication.WriteHeadersToResponse(httpContext, rpsRespHeaders, LiveIdAuthenticationModule.IsConsumerRequestForO365(httpContext.Request));
					return;
				}
			}
			else if (rpsTicketType == 2U)
			{
				if (!timeoutWithSSOEnabled && (double)LiveIdAuthenticationModule.SecondsSinceInstant(issueInstant) > timeSpan.TotalSeconds)
				{
					ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceDebug(0L, "ValidateWithSlidingWindow(): Timeout. AddRealmParameterFromTicket and RedirectToLiveLogin for compact ticket.");
					httpContext.Response.AppendToLog("&LogoffReason=SlidingWindow");
					LiveIdAuthenticationModule.WriteAuthenticatedUserToIISLog(httpContext);
					LiveIdAuthenticationModule.AddRealmParameterFromRpsMemberName(httpContext, membername);
					LiveIdAuthenticationModule.RedirectToLiveLogin(httpContext, LiveIdAuthenticationModule.GetOrgOwnSiteName(httpContext), LiveIdAuthenticationModule.GetOrgOwnSiteReturnUrlHost(httpContext), LiveIdAuthenticationModule.iOwnSiteDNSSubdomain, LiveIdAuthenticationModule.timeoutRedirectPolicy, false, false);
					return;
				}
			}
			else
			{
				LiveIdAuthenticationModule.HandleActivityTimeout(httpContext, membername);
			}
		}

		private static bool IsEcpAnonymousRequest(HttpContext httpContext)
		{
			Uri requestUrlEvenIfProxied = httpContext.Request.GetRequestUrlEvenIfProxied();
			return HttpRuntime.AppDomainAppVirtualPath.Equals("/ecp", StringComparison.OrdinalIgnoreCase) && requestUrlEvenIfProxied.Segments.Length > 3 && requestUrlEvenIfProxied.Segments[2].Equals("auth/", StringComparison.OrdinalIgnoreCase);
		}

		private static bool IsOwaAnonymousRequest(HttpContext httpContext)
		{
			Uri requestUrlEvenIfProxied = httpContext.Request.GetRequestUrlEvenIfProxied();
			return HttpRuntime.AppDomainAppVirtualPath.Equals("/owa", StringComparison.OrdinalIgnoreCase) && requestUrlEvenIfProxied.Segments.Length > 3 && requestUrlEvenIfProxied.Segments[2].Equals("auth/", StringComparison.OrdinalIgnoreCase);
		}

		private static void VerifyActivityTimeout(HttpContext httpContext, LiveIdPropertySet propertySet, RPSTicket rpsTicket)
		{
			OrganizationProperties organizationProperties = propertySet.OrganizationProperties;
			if (organizationProperties == null)
			{
				LiveIdAuthenticationModule.RaiseOrgIdMailboxNotFoundException(propertySet.MemberName, httpContext, AccountLookupFailureReason.OrganizationNotFound, null);
			}
			uint rpsTicketType = propertySet.RpsTicketType;
			string rpsRespHeaders = propertySet.RpsRespHeaders;
			string memberName = propertySet.MemberName;
			uint issueInstant = propertySet.IssueInstant;
			ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceDebug(0L, "Verify Activity Timeout");
			if (organizationProperties.ActivityBasedAuthenticationTimeoutEnabled && !OfflineClientRequestUtilities.IsRequestFromMOWAClient(httpContext.Request, httpContext.Request.UserAgent))
			{
				ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceDebug(0L, "orgProperties.ActivityBasedAuthenticationTimeoutEnabled is true");
				if (!httpContext.Request.GetRequestUrlEvenIfProxied().LocalPath.EndsWith("logoff.owa", StringComparison.OrdinalIgnoreCase))
				{
					ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceDebug(0L, "The URL is not a logoff request, validate the ticket against sliding window rules");
					LiveIdAuthenticationModule.ValidateWithSlidingWindow(httpContext, rpsRespHeaders, organizationProperties.ActivityBasedAuthenticationTimeoutInterval, organizationProperties.ActivityBasedAuthenticationTimeoutWithSingleSignOnEnabled, rpsTicketType, issueInstant, memberName, rpsTicket);
				}
			}
		}

		private static void CheckTOU(HttpContext httpContext, LiveIdPropertySet propertySet)
		{
			ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceDebug(0L, "Checking TOU");
			if (propertySet.RequestType != RequestType.EcpDelegatedAdminTargetForest)
			{
				ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceDebug<RequestType>(0L, "propertySet.RequestType is {0}", propertySet.RequestType);
				LiveIDIdentity liveIDIdentity = (LiveIDIdentity)propertySet.Identity;
				bool hasAcceptedAccruals = propertySet.HasAcceptedAccruals;
				if (!liveIDIdentity.UserOrganizationProperties.SkipToUAndParentalControlCheck && !hasAcceptedAccruals)
				{
					ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceDebug(0L, "TOU not signed");
					string value = httpContext.Request.QueryString["accrual"];
					if (!string.IsNullOrEmpty(value))
					{
						string message = "TOU should have been signed, render error page";
						ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceDebug(0L, message);
						InvalidOperationException exception = new InvalidOperationException(message);
						Utilities.HandleException(httpContext, exception, false);
					}
					LiveIdAuthenticationModule.ClearAuthCookies(httpContext, LiveIdAuthenticationModule.GetOrgOwnSiteName(httpContext));
					ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceDebug(0L, "CheckTOU(): Cleared auth cookies, redirect to live login for accruals");
					httpContext.Response.AppendToLog("&LogoffReason=TOU");
					LiveIdAuthenticationModule.WriteAuthenticatedUserToIISLog(httpContext);
					httpContext.Response.Redirect(LiveIdAuthenticationModule.liveIdAuthAccrualSignUrl);
				}
				liveIDIdentity.HasAcceptedAccruals = hasAcceptedAccruals;
				if (AuthCommon.IsFrontEnd)
				{
					return;
				}
				ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceDebug(0L, "Checking TOU in special post scenario");
				if (!hasAcceptedAccruals && httpContext.Request.HttpMethod.Equals("POST", StringComparison.OrdinalIgnoreCase) && httpContext.Request.ContentLength <= 128 && httpContext.Request.Form != null && httpContext.Request.Form["InstantMessage_TOU"] != null && httpContext.Request.Form["InstantMessage_TOU"].Equals("1", StringComparison.OrdinalIgnoreCase))
				{
					LiveIdAuthenticationModule.ClearAuthCookies(httpContext, LiveIdAuthenticationModule.GetOrgOwnSiteName(httpContext));
					ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceDebug(0L, "CheckTOU(): Cleared auth cookies, redirect to live login - special post scenario");
					httpContext.Response.AppendToLog("&LogoffReason=TOUSpecialPost");
					LiveIdAuthenticationModule.WriteAuthenticatedUserToIISLog(httpContext);
					httpContext.Response.Redirect(LiveIdAuthenticationModule.liveIdAuthAccrualSignUrl);
				}
			}
		}

		private static bool MatchSiteName(HttpContext httpContext, string siteName)
		{
			string host = httpContext.Request.GetRequestUrlEvenIfProxied().Host;
			return !string.IsNullOrEmpty(host) && host.EndsWith(siteName, StringComparison.InvariantCultureIgnoreCase);
		}

		private static string GetOrgOwnSiteReturnUrlHost(HttpContext httpContext)
		{
			bool flag = LiveIdAuthenticationModule.IsConsumerRequestForO365(httpContext.Request);
			if (LiveIdAuthenticationModule.MatchSiteName(httpContext, LiveIdAuthenticationModule.legacySiteName) && flag && !string.IsNullOrEmpty(LiveIdAuthenticationModule.legacySiteReturnURLHostConsumer))
			{
				return LiveIdAuthenticationModule.legacySiteReturnURLHostConsumer;
			}
			if (LiveIdAuthenticationModule.MatchSiteName(httpContext, LiveIdAuthenticationModule.legacySiteName) && !flag && !string.IsNullOrEmpty(LiveIdAuthenticationModule.legacySiteReturnURLHostEnterprise))
			{
				return LiveIdAuthenticationModule.legacySiteReturnURLHostEnterprise;
			}
			if (LiveIdAuthenticationModule.MatchSiteName(httpContext, LiveIdAuthenticationModule.o365SiteName) && flag && !string.IsNullOrEmpty(LiveIdAuthenticationModule.o365SiteReturnURLHostConsumer))
			{
				return LiveIdAuthenticationModule.o365SiteReturnURLHostConsumer;
			}
			if (LiveIdAuthenticationModule.MatchSiteName(httpContext, LiveIdAuthenticationModule.o365SiteName) && !flag && !string.IsNullOrEmpty(LiveIdAuthenticationModule.o365SiteReturnURLHostEnterprise))
			{
				return LiveIdAuthenticationModule.o365SiteReturnURLHostEnterprise;
			}
			if (flag)
			{
				return LiveIdAuthenticationModule.orgOwnSiteReturnURLHostConsumer;
			}
			return LiveIdAuthenticationModule.orgOwnSiteReturnURLHostEnterprise;
		}

		private static string GetOrgOwnSiteName(HttpContext httpContext)
		{
			if (LiveIdAuthenticationModule.MatchSiteName(httpContext, LiveIdAuthenticationModule.legacySiteName) && !string.IsNullOrEmpty(LiveIdAuthenticationModule.legacySiteName))
			{
				return LiveIdAuthenticationModule.legacySiteName;
			}
			if (LiveIdAuthenticationModule.MatchSiteName(httpContext, LiveIdAuthenticationModule.o365SiteName) && !string.IsNullOrEmpty(LiveIdAuthenticationModule.o365SiteName))
			{
				return LiveIdAuthenticationModule.o365SiteName;
			}
			return LiveIdAuthenticationModule.orgOwnSiteName;
		}

		private static void AddToPuidToSidCache(string key, GenericPrincipal principal)
		{
			LiveIdAuthenticationModule.puidToPrincipalCache.Add(key, LiveIdAuthenticationModule.GetPuidToSidCacheEntry(principal));
		}

		private static bool TryGetFromPuidToSidCache(string key, out GenericPrincipal principal)
		{
			LiveIdAuthenticationModule.CacheEntry cacheEntry;
			bool flag = LiveIdAuthenticationModule.puidToPrincipalCache.TryGetValue(key, out cacheEntry);
			principal = null;
			if (flag)
			{
				if (cacheEntry.ExpiryTimeInTicks > 0L && cacheEntry.ExpiryTimeInTicks < DateTime.UtcNow.Ticks)
				{
					flag = false;
					LiveIdAuthenticationModule.puidToPrincipalCache.Remove(key);
				}
				else
				{
					principal = cacheEntry.Principal;
				}
			}
			return flag;
		}

		private static LiveIdAuthenticationModule.CacheEntry GetPuidToSidCacheEntry(GenericPrincipal principal)
		{
			long expiryTimeInTicks = 0L;
			TenantRelocationState tenantRelocationState;
			bool flag;
			if (TenantRelocationStateCache.TryGetTenantRelocationStateByObjectId(((LiveIDIdentity)principal.Identity).UserOrganizationId.OrganizationalUnit, out tenantRelocationState, out flag) && (tenantRelocationState.SourceForestState == TenantRelocationStatus.Synchronization || tenantRelocationState.SourceForestState == TenantRelocationStatus.Lockdown || tenantRelocationState.SourceForestState == TenantRelocationStatus.Retired))
			{
				expiryTimeInTicks = DateTime.UtcNow.Add(ExpiringTenantRelocationStateValue.TenantRelocationStateExpirationWindowProvider.GetExpirationWindow(tenantRelocationState)).Ticks;
			}
			return new LiveIdAuthenticationModule.CacheEntry(principal, expiryTimeInTicks);
		}

		private static bool WhileHandlingAndRetryingLiveIdErrors(string callingMethodName, HttpContext httpContext, Func<bool> tryBlockToExecute, Action finallyBlockToExectute = null)
		{
			bool result = false;
			ushort num = 0;
			Exception ex = null;
			try
			{
				do
				{
					try
					{
						ex = null;
						num += 1;
						ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceDebug<ushort, string>(0L, "WhileHandlingAndRetryingLiveIdErrors() starting attempt {0} for method '{1}'.", num, callingMethodName);
						result = tryBlockToExecute();
						break;
					}
					catch (LiveTransientException ex2)
					{
						string debuggingDetails = LiveIdAuthenticationModule.GetDebuggingDetails(httpContext);
						ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceError<string, LiveTransientException, string>(0L, "WhileHandlingAndRetryingLiveIdErrors() for method '{0}' failed with retryable LiveTransientException: {1}. Extra Information: {2}", callingMethodName, ex2, debuggingDetails);
						ex = ex2;
						Thread.Sleep(TimeSpan.FromSeconds(1.0));
					}
					catch (LiveConfigurationException ex3)
					{
						string debuggingDetails2 = LiveIdAuthenticationModule.GetDebuggingDetails(httpContext);
						ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceError<string, LiveConfigurationException, string>(0L, "WhileHandlingAndRetryingLiveIdErrors() for method '{0}' failed with non-retryable LiveConfigurationException: {1}. Extra Information: {2}", callingMethodName, ex3, debuggingDetails2);
						ex = ex3;
						break;
					}
					catch (LiveClientException ex4)
					{
						string debuggingDetails3 = LiveIdAuthenticationModule.GetDebuggingDetails(httpContext);
						ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceError<string, LiveClientException, string>(0L, "WhileHandlingAndRetryingLiveIdErrors() for method '{0}' failed with non-retryable LiveClientException: {1}. Extra Information: {2}", callingMethodName, ex4, debuggingDetails3);
						ex = ex4;
						break;
					}
					catch (LiveExternalHRESULTException ex5)
					{
						ex5.AdditionalWatsonData = LiveIdAuthenticationModule.GetDebuggingDetails(httpContext);
						ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceError<string, LiveExternalHRESULTException>(0L, "WhileHandlingAndRetryingLiveIdErrors() for method '{0}' failed with unexpected LiveExternalHRESULTException: {1}.", callingMethodName, ex5);
						throw;
					}
					catch (LiveOperationException ex6)
					{
						ex6.AdditionalWatsonData = LiveIdAuthenticationModule.GetDebuggingDetails(httpContext);
						ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceError<string, LiveOperationException>(0L, "WhileHandlingAndRetryingLiveIdErrors() for method '{0}' failed with unexpected LiveOperationException: {1}.", callingMethodName, ex6);
						throw;
					}
				}
				while (num <= 3);
				if (ex == null)
				{
					ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceDebug<string>(0L, "WhileHandlingAndRetryingLiveIdErrors() for method '{0}' succeeded.", callingMethodName);
				}
				else
				{
					result = false;
					ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceError<string, Exception>(0L, "WhileHandlingAndRetryingLiveIdErrors() for method '{0}' failed with retries. Exception: {1}.", callingMethodName, ex);
					Utilities.HandleException(httpContext, ex, false);
				}
			}
			finally
			{
				if (finallyBlockToExectute != null)
				{
					ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceDebug<string>(0L, "WhileHandlingAndRetryingLiveIdErrors() is executing finally block for method '{0}'.", callingMethodName);
					finallyBlockToExectute();
				}
			}
			return result;
		}

		private static string GetDebuggingDetails(HttpContext httpContext)
		{
			if (httpContext == null)
			{
				return null;
			}
			StringBuilder stringBuilder = new StringBuilder();
			if (httpContext.User != null && httpContext.User.Identity != null && !string.IsNullOrWhiteSpace(httpContext.User.Identity.Name))
			{
				stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "UserName: {0}", new object[]
				{
					httpContext.User.Identity.Name
				}).AppendLine();
			}
			if (httpContext.Request != null)
			{
				if (!string.IsNullOrWhiteSpace(httpContext.Request.HttpMethod))
				{
					stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "Request Verb: {0}", new object[]
					{
						httpContext.Request.HttpMethod
					}).AppendLine();
				}
				Uri requestUrlEvenIfProxied = httpContext.Request.GetRequestUrlEvenIfProxied();
				if (requestUrlEvenIfProxied != null)
				{
					stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "Request URL: {0}", new object[]
					{
						requestUrlEvenIfProxied
					}).AppendLine();
				}
				if (httpContext.Request.UrlReferrer != null)
				{
					stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "Referrer: {0}", new object[]
					{
						httpContext.Request.UrlReferrer
					}).AppendLine();
				}
				if (httpContext.Request.Headers != null)
				{
					stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "Request Headers: {0}", new object[]
					{
						string.Join(", ", new object[]
						{
							httpContext.Request.Headers
						})
					}).AppendLine();
				}
				stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "Request is local: {0}", new object[]
				{
					httpContext.Request.IsLocal
				}).AppendLine();
				if (!string.IsNullOrWhiteSpace(httpContext.Request.UserHostAddress))
				{
					stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "Calling IP: {0}", new object[]
					{
						httpContext.Request.UserHostAddress
					}).AppendLine();
				}
				if (!string.IsNullOrWhiteSpace(httpContext.Request.UserHostName))
				{
					stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "Calling DNS name: {0}", new object[]
					{
						httpContext.Request.UserHostName
					}).AppendLine();
				}
			}
			return stringBuilder.ToString();
		}

		private static void RaiseOrgIdMailboxNotFoundException(string memberName, HttpContext httpContext, AccountLookupFailureReason reason, LiveIdPropertySet propertySet)
		{
			string localizedLiveIdSignoutLinkMessage = LiveIdAuthenticationModule.GetLocalizedLiveIdSignoutLinkMessage(httpContext);
			OrgIdMailboxNotFoundException exception = null;
			if (reason == AccountLookupFailureReason.AccountDisabled)
			{
				exception = new OrgIdAccountDisabledException(memberName, localizedLiveIdSignoutLinkMessage);
			}
			else if (reason == AccountLookupFailureReason.SharedMailboxAccountDisabled)
			{
				exception = new OrgIdSharedMailboxAccountDisabledException(memberName, localizedLiveIdSignoutLinkMessage);
			}
			else if (LiveIdAuthenticationModule.ShouldThrowAccountRecentlyCreatedException(memberName, httpContext, localizedLiveIdSignoutLinkMessage, out exception))
			{
				reason = AccountLookupFailureReason.MailboxRecentlyCreated;
			}
			else if (LiveIdAuthenticationModule.ShouldThrowAccountSoftDeletedException(memberName, httpContext, localizedLiveIdSignoutLinkMessage, propertySet, out exception))
			{
				reason = AccountLookupFailureReason.MailboxSoftDeleted;
			}
			else
			{
				exception = new OrgIdMailboxNotFoundException(memberName, localizedLiveIdSignoutLinkMessage);
			}
			httpContext.Response.AppendToLog("&NoMailbox=" + reason);
			bool shouldSend440Response = false;
			if (propertySet.RpsTicketType != 2U && (reason == AccountLookupFailureReason.MailboxSoftDeleted || reason == AccountLookupFailureReason.AccountDisabled))
			{
				shouldSend440Response = true;
			}
			Utilities.HandleException(httpContext, exception, shouldSend440Response);
		}

		private static bool ShouldThrowAccountRecentlyCreatedException(string memberName, HttpContext httpContext, string logoutLink, out OrgIdMailboxNotFoundException exception)
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			DateTime accountCreationTime;
			try
			{
				accountCreationTime = ProfileService.Instance.GetAccountCreationTime(memberName);
			}
			catch (Exception ex)
			{
				ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceError<Exception>(0L, "Exception occurred while attempting to get the account creation time: {0}", ex);
				httpContext.Response.AppendToLog("&excptRdngAcctCrtnTm=" + ex.GetType().FullName);
				exception = null;
				return false;
			}
			finally
			{
				stopwatch.Stop();
				LogonLatencyLogger.LogProfileReadLatency(httpContext, stopwatch.Elapsed);
			}
			TimeSpan creationTimeSpanToNow = DateTime.UtcNow - accountCreationTime;
			httpContext.Response.AppendToLog("&accntcrtntmdiff=" + creationTimeSpanToNow.TotalHours);
			if (creationTimeSpanToNow.TotalDays < 1.0)
			{
				exception = new OrgIdMailboxRecentlyCreatedException(memberName, logoutLink, creationTimeSpanToNow);
				return true;
			}
			exception = null;
			return false;
		}

		private static bool ShouldThrowAccountSoftDeletedException(string memberName, HttpContext httpContext, string logoutLink, LiveIdPropertySet propertySet, out OrgIdMailboxNotFoundException exception)
		{
			exception = null;
			if (propertySet == null)
			{
				return false;
			}
			AccountLookupFailureReason accountLookupFailureReason;
			if (LiveIdAuthenticationModule.GetPrincipal(propertySet, httpContext, true, out accountLookupFailureReason) != null)
			{
				exception = new OrgIdMailboxSoftDeletedException(memberName, logoutLink);
				return true;
			}
			return false;
		}

		private static bool ShouldPeformFullAuthentication(HttpContext httpContext)
		{
			return !AuthCommon.IsFrontEnd || !LiveIdAuthenticationModule.isBeAuthEnabled || CompositeIdentityAuthenticationHelper.IsUnifiedMailboxRequest(httpContext);
		}

		private static void SetDefaultAnchorMailboxCookie(HttpContext httpContext, string upn)
		{
			HttpCookie httpCookie = new HttpCookie("DefaultAnchorMailbox", upn)
			{
				HttpOnly = false
			};
			HttpCookie httpCookie2 = httpContext.Request.Cookies["RPSSecAuth"];
			HttpCookie httpCookie3 = httpContext.Request.Cookies["RPSAuth"];
			if (httpCookie2 != null)
			{
				httpCookie.Expires = httpCookie2.Expires;
			}
			else if (httpCookie3 != null)
			{
				httpCookie.Expires = httpCookie3.Expires;
			}
			httpContext.Response.SetCookie(httpCookie);
		}

		private static void InternalOnAuthenticate(HttpContext httpContext)
		{
			ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceDebug(0L, "Start InternalOnAuthenticate");
			httpContext.Items["RequestStartTime"] = DateTime.UtcNow;
			httpContext.Items["AuthType"] = "OrgId";
			StringBuilder identityDiagnostics = new StringBuilder();
			try
			{
				LiveIdAuthenticationModule.AddRealmParameter(httpContext);
				string realm = httpContext.Request.QueryString["realm"];
				Uri requestUrl = httpContext.Request.GetRequestUrlEvenIfProxied();
				ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceDebug<Uri>(0L, "Add Realm Parameter, url is now: {0}", requestUrl);
				bool flag = Utility.IsResourceRequest(requestUrl.LocalPath);
				if (flag && (!AuthCommon.IsFrontEnd || Utility.IsOwaRequestWithRoutingHint(httpContext.Request) || Utility.HasResourceRoutingHint(httpContext.Request) || Utility.IsAnonymousResourceRequest(httpContext.Request)))
				{
					httpContext.User = new WindowsPrincipal(WindowsIdentity.GetAnonymous());
					ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceDebug<Uri>(0L, "This is a resource request: {0}", requestUrl);
					LiveIdAuthenticationModule.SetSkippedAuthFlagOnRequest(httpContext);
				}
				else if (EDiscoveryExportToolRequestPathHandler.IsEDiscoveryExportToolRequest(httpContext.Request))
				{
					httpContext.User = new WindowsPrincipal(WindowsIdentity.GetAnonymous());
					ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceDebug<Uri>(0L, "This is an eDiscovery export tool request: {0}", requestUrl);
					LiveIdAuthenticationModule.SetSkippedAuthFlagOnRequest(httpContext);
				}
				else if (WopiRequestPathHandler.IsWopiRequest(httpContext.Request, AuthCommon.IsFrontEnd))
				{
					LiveIdAuthenticationModule.SetSkippedAuthFlagOnRequest(httpContext);
				}
				else if (AuthCommon.IsFrontEnd || !UrlUtilities.IsRemoteNotificationRequest(httpContext.Request))
				{
					if (LiveIdAuthenticationModule.HandleLogoffRequest(httpContext))
					{
						LiveIdAuthenticationModule.SetSkippedAuthFlagOnRequest(httpContext);
					}
					else if (LiveIdAuthenticationModule.HandleOmeLogoffRequest(httpContext))
					{
						LiveIdAuthenticationModule.SetSkippedAuthFlagOnRequest(httpContext);
					}
					else if (LiveIdAuthenticationModule.IsCobrandingRedirectPageRequest(requestUrl))
					{
						ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceDebug<Uri>(0L, "Cobranding redirect page, return: {0}", requestUrl);
						LiveIdAuthenticationModule.SetSkippedAuthFlagOnRequest(httpContext);
					}
					else
					{
						HttpCookie httpCookie = httpContext.Request.Cookies["lastResponse"];
						bool flag2 = false;
						if (httpCookie != null && !string.IsNullOrEmpty(httpCookie.Value))
						{
							flag2 = true;
						}
						bool isConsumerRequestForO365 = LiveIdAuthenticationModule.IsConsumerRequestForO365(httpContext.Request);
						if (!isConsumerRequestForO365 && httpContext.Request.HttpMethod.Equals("GET", StringComparison.OrdinalIgnoreCase) && (string.Equals(requestUrl.Host, LiveIdAuthenticationModule.orgOwnSiteName, StringComparison.OrdinalIgnoreCase) || string.Equals(requestUrl.Host, "www." + LiveIdAuthenticationModule.orgOwnSiteName, StringComparison.OrdinalIgnoreCase)) && LiveIdAuthenticationModule.ShouldRedirectToLiveIdNewMailUrl(httpContext) && string.IsNullOrEmpty(realm) && LiveIdAuthenticationModule.isNewMailOptimizationsEnabled && !flag2 && (requestUrl.Host.IndexOf("-int", StringComparison.OrdinalIgnoreCase) < 0 || LiveIdAuthenticationModule.isMservLookupEnabledinTest))
						{
							ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceDebug(0L, "InternalOnAuthenticate(): MSERV lookup is enabled in test/onebox. Redirecting to new Live ID Mail URL");
							httpContext.Response.AppendToLog("&LogoffReason=ConsumerRedirect");
							LiveIdAuthenticationModule.RedirectToLiveIdNewMailUrl(httpContext);
						}
						string puid = null;
						string orgIdPuid = null;
						string cid = null;
						uint issueInstant = 0U;
						string membername = null;
						string rpsRespHeaders = null;
						string upn = null;
						bool hasAcceptedAccruals = false;
						uint loginAttributes = 0U;
						uint rpsTicketType = 0U;
						RPSTicket deprecatedRpsTicketObject = null;
						GenericPrincipal principal = null;
						bool isOrgIdFederatedMsaIdentity = false;
						LiveIdPropertySet propertySet = LiveIdPropertySet.GetLiveIdPropertySet(httpContext);
						if (LiveIdAuthenticationModule.WhileHandlingAndRetryingLiveIdErrors("InternalOnAuthenticate", httpContext, delegate
						{
							string siteName = LiveIdAuthenticationModule.GetOrgOwnSiteName(httpContext);
							uint num = 0U;
							ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceDebug<string>(0L, "Test if we come from accrual site: {0}", requestUrl.Host);
							if (requestUrl.Host.Equals(LiveIdAuthenticationModule.iOwnSiteName, StringComparison.OrdinalIgnoreCase))
							{
								ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceDebug(0L, "InternalOnAuthenticate(): Redirect to live login after we came from accrual");
								httpContext.Response.AppendToLog("&LogoffReason=Accrual");
								UrlUtilities.RewriteParameterInURL(httpContext, "exsvurl=1");
								UrlUtilities.RewriteParameterInURL(httpContext, "accrual", "1");
								LiveIdAuthenticationModule.RedirectToLiveLogin(httpContext, LiveIdAuthenticationModule.GetOrgOwnSiteName(httpContext), LiveIdAuthenticationModule.GetOrgOwnSiteReturnUrlHost(httpContext), null, "MBI_KEY_FORCE_REFRESH", false, false);
							}
							ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceDebug(0L, "Call LiveIdAuthentication.Authenticate");
							if (!LiveIdAuthentication.Authenticate(httpContext, siteName, LiveIdAuthenticationModule.AuthPolicyOverrideValue, LiveIdAuthenticationModule.memberNameIgnorePrefixes, isConsumerRequestForO365, out puid, out orgIdPuid, out cid, out membername, out issueInstant, out loginAttributes, out rpsRespHeaders, out rpsTicketType, out deprecatedRpsTicketObject, out hasAcceptedAccruals, out num, out isOrgIdFederatedMsaIdentity))
							{
								bool flag3 = httpContext.Request.Path.Equals("/owa/SuiteServiceProxy.aspx", StringComparison.OrdinalIgnoreCase);
								ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceDebug(0L, "LiveIdAuthentication.Authenticate failed");
								httpContext.Response.AppendToLog("&LogoffReason=Unauthenticated");
								if (LiveIdAuthenticationModule.HasSessionTimedOut(issueInstant) && Utilities.Need440Response(httpContext.Request) && !LiveIdAuthenticationModule.IsEducationPagePost(httpContext))
								{
									ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceDebug(0L, "Session timed out, need 440, render 440 response timeout");
									Utilities.Render440TimeoutResponse(httpContext.Response, httpContext.Request.HttpMethod, httpContext);
								}
								else if (flag3 && "1".Equals(httpContext.Request.QueryString["Authed"]))
								{
									if (httpContext.Request.Cookies["SuiteServiceProxyInit"] == null)
									{
										ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceDebug(0L, "Auth cookies should have been present, redirecting to RedirSuiteServiceProxy");
										string url = string.Format("/owa/auth/RedirSuiteServiceProxy.aspx?returnUrl={0}", HttpUtility.UrlEncode(httpContext.Request.QueryString["returnUrl"]));
										httpContext.Response.Redirect(url);
									}
									else
									{
										Utilities.RenderErrorPage(httpContext.Response, 403, "403 Forbidden", string.Empty, httpContext);
									}
								}
								else if (httpContext.Request.QueryString["silent"] != null)
								{
									ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceDebug(0L, "SuiteServiceProxy page cannot authenticate, credentials needed.");
									Utilities.RenderErrorPage(httpContext.Response, 403, "403 Forbidden", string.Empty, httpContext);
								}
								else
								{
									Utilities.SetCookie(httpContext, "lastResponse", string.Empty, null);
									if (LiveIdAuthenticationModule.HasSessionTimedOut(issueInstant))
									{
										if (!LiveIdAuthenticationModule.isNewMailOptimizationsEnabled)
										{
											if (!string.Equals(requestUrl.Host, LiveIdAuthenticationModule.orgOwnSiteName, StringComparison.OrdinalIgnoreCase) && !string.Equals(requestUrl.Host, "www." + LiveIdAuthenticationModule.orgOwnSiteName, StringComparison.OrdinalIgnoreCase))
											{
												LiveIdAuthenticationModule.AddRealmParameterFromCookie(httpContext);
											}
										}
										else
										{
											LiveIdAuthenticationModule.AddRealmParameterFromCookie(httpContext);
										}
									}
									realm = httpContext.Request.QueryString["realm"];
									if (string.IsNullOrEmpty(realm))
									{
										LiveIdAuthenticationModule.AddRealmParameterForSDF(httpContext, isConsumerRequestForO365);
										realm = httpContext.Request.QueryString["realm"];
									}
									ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceDebug<Uri>(0L, "Session timed out, tried to add realm: {0}", requestUrl);
									LogonLatencyLogger.CreateCookie(httpContext, "LGN01");
									ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceDebug<Uri>(0L, "InternalOnAuthenticate(): User is not authenticated. Redirect to live login. User tried to access url {0}", requestUrl);
									bool useSilentAuthentication = flag3;
									LiveIdAuthenticationModule.RedirectToLiveLogin(httpContext, LiveIdAuthenticationModule.GetOrgOwnSiteName(httpContext), LiveIdAuthenticationModule.GetOrgOwnSiteReturnUrlHost(httpContext), LiveIdAuthenticationModule.iOwnSiteDNSSubdomain, null, false, useSilentAuthentication);
								}
							}
							propertySet.SetLiveIdProperties(puid, orgIdPuid, cid, membername, hasAcceptedAccruals, loginAttributes, rpsTicketType, rpsRespHeaders, siteName, issueInstant);
							try
							{
								if (!string.IsNullOrWhiteSpace(membername))
								{
									identityDiagnostics.AppendFormat("liveIdMemberName=<PII>{0}</PII>", membername);
								}
								if (!string.IsNullOrWhiteSpace(orgIdPuid))
								{
									identityDiagnostics.AppendFormat("liveIdPuid=<PII>{0}</PII>", orgIdPuid);
								}
								if (!string.IsNullOrWhiteSpace(puid))
								{
									identityDiagnostics.AppendFormat("liveIdConsumerPuid=<PII>{0}</PII>", puid);
								}
								identityDiagnostics.AppendFormat("liveIdTicketType={0}", rpsTicketType);
								HttpCookie httpCookie4 = httpContext.Request.Cookies["RPSAuth"];
								if (httpCookie4 != null && !string.IsNullOrWhiteSpace(httpCookie4.Value))
								{
									identityDiagnostics.AppendFormat("liveIdIncomingRpsAuthCookie={0}", httpCookie4.Value.GetHashCode());
								}
								HttpCookie httpCookie5 = httpContext.Request.Cookies["RPSSecAuth"];
								if (httpCookie5 != null && !string.IsNullOrWhiteSpace(httpCookie5.Value))
								{
									identityDiagnostics.AppendFormat("liveIdIncomingRpsSecAuthCookie={0}", httpCookie5.Value.GetHashCode());
								}
								if (!string.IsNullOrWhiteSpace(rpsRespHeaders))
								{
									IEnumerable<string> enumerable = from s in rpsRespHeaders.Split(new char[]
									{
										' '
									})
									where s.StartsWith("RPSAuth=") || s.StartsWith("RPSSecAuth=")
									select s;
									foreach (string text2 in enumerable)
									{
										if (text2.StartsWith("RPSAuth="))
										{
											identityDiagnostics.AppendFormat("&liveIdOutgoingRpsAuthCookie={0}", text2.Substring(8, text2.Length - 9).GetHashCode());
										}
										else if (text2.StartsWith("RPSSecAuth="))
										{
											identityDiagnostics.AppendFormat("&liveIdOutgoingRpsSecAuthCookie={0}", text2.Substring(11, text2.Length - 12).GetHashCode());
										}
									}
								}
							}
							catch (Exception)
							{
							}
							if (LiveIdAuthenticationModule.skipAdLookupOnRandomBe && rpsTicketType == 2U)
							{
								ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceDebug(0L, "Preemptively redirect authenticated using LiveIdAuthentication.Authenticate, save cookies");
								upn = membername;
								LiveIdAuthenticationModule.SaveOrganizationNameCookie(httpContext, Utilities.GetUserDomain(membername));
								LiveIdAuthenticationModule.SaveAuthCookieAndRedirectAsHttpGet(httpContext, rpsRespHeaders, requestUrl, upn, orgIdPuid);
							}
							if (isOrgIdFederatedMsaIdentity)
							{
								ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceError<string>(0L, "User '{0}' has a migrated account and should not be logging in with LiveID.", membername);
								string liveLogoutRedirectUrl = LiveIdAuthentication.GetLiveLogoutRedirectUrl(LiveIdAuthenticationModule.CreateRedirectUrl(requestUrl, LiveIdAuthenticationModule.GetOrgOwnSiteReturnUrlHost(httpContext), LiveIdAuthenticationModule.iOwnSiteDNSSubdomain), LiveIdAuthenticationModule.GetOrgOwnSiteName(httpContext), isConsumerRequestForO365);
								MigratedUserLiveIdLogonException exception2 = new MigratedUserLiveIdLogonException(membername, liveLogoutRedirectUrl);
								Utilities.HandleException(httpContext, exception2, false);
							}
							if (LiveIdAuthenticationModule.timeoutEnabled && !OfflineClientRequestUtilities.IsRequestFromMOWAClient(httpContext.Request, httpContext.Request.UserAgent) && LiveIdAuthenticationModule.HasSessionTimedOut(issueInstant))
							{
								LiveIdAuthenticationModule.HandleSessionTimedOutFromExchangePerspective(httpContext, "&LogoffReason=SessionTimeout", true);
							}
							if (string.IsNullOrEmpty(puid))
							{
								ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceError(0L, "InternalOnAuthenticate(): The PUID is null or empty, redirect to live logout");
								httpContext.Response.AppendToLog("&LogoffReason=InvalidIdentity");
								LiveIdAuthenticationModule.RedirectToLiveLogout(httpContext, LiveIdAuthenticationModule.GetOrgOwnSiteName(httpContext), LiveIdAuthenticationModule.GetOrgOwnSiteReturnUrlHost(httpContext), LiveIdAuthenticationModule.iOwnSiteDNSSubdomain, false, true);
							}
							else
							{
								string value2 = httpContext.Request.QueryString["SecurityToken"];
								if (string.IsNullOrEmpty(value2))
								{
									HttpCookie httpCookie6 = httpContext.Request.Cookies["SecurityToken"];
									if (httpCookie6 != null)
									{
										value2 = httpCookie6.Value;
									}
								}
								if (!string.IsNullOrEmpty(value2))
								{
									httpContext.Items["RPSEnv"] = LiveIdAuthentication.GetCurrentEnvironment(isConsumerRequestForO365);
									httpContext.Items["RPSMemberName"] = membername;
									httpContext.Items["RPSCID"] = cid;
									httpContext.Items["RPSPUID"] = puid;
									httpContext.Items["RPSOrgIdPUID"] = orgIdPuid;
									if (!AuthCommon.IsFrontEnd)
									{
										httpContext.Request.Headers["RPSEnv"] = (string)httpContext.Items["RPSEnv"];
										httpContext.Request.Headers["RPSMemberName"] = membername;
										httpContext.Request.Headers["RPSPUID"] = puid;
										httpContext.Request.Headers["RPSOrgIdPUID"] = orgIdPuid;
									}
									return false;
								}
								AccountLookupFailureReason reason;
								if ((principal = LiveIdAuthenticationModule.GetPrincipal(propertySet, httpContext, false, out reason)) == null)
								{
									if (string.IsNullOrEmpty(membername))
									{
										ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceDebug(0L, "We couldn't get the membername info from the liveid profile db");
										UrlUtilities.RewriteParameterInURL(httpContext, "lex", "membername");
										httpContext.Response.AppendToLog(string.Format("&puid={0}", puid));
									}
									else
									{
										UrlUtilities.RewriteParameterInURL(httpContext, "lex", "redirecturl");
										httpContext.Response.AppendToLog(string.Format("&puid={0}&membername={1}", puid, membername));
									}
									ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceError<string>(0L, "InternalOnAuthenticate() - Could not find user '{0}' in AD.", membername);
									LiveIdAuthenticationModule.RaiseOrgIdMailboxNotFoundException(membername, httpContext, reason, propertySet);
									return false;
								}
							}
							LiveIDIdentity liveIDIdentity = principal.Identity as LiveIDIdentity;
							if (liveIDIdentity != null)
							{
								if (!string.IsNullOrWhiteSpace(liveIDIdentity.PrincipalName))
								{
									identityDiagnostics.AppendFormat("liveIdAdUpn=<PII>{0}</PII>", liveIDIdentity.PrincipalName);
								}
								if (liveIDIdentity.Sid != null)
								{
									identityDiagnostics.AppendFormat("liveIdAdSid=<PII>{0}</PII>", liveIDIdentity.Sid.ToString());
								}
								if (liveIDIdentity.UserOrganizationId != null)
								{
									identityDiagnostics.AppendFormat("liveIdAdOrganization=<PII>{0}</PII>", liveIDIdentity.UserOrganizationId.ToString());
								}
							}
							httpContext.Items["RPSEnv"] = LiveIdAuthentication.GetCurrentEnvironment(isConsumerRequestForO365);
							httpContext.Items["RPSMemberName"] = membername;
							httpContext.Items["RPSCID"] = cid;
							httpContext.Items["RPSPUID"] = puid;
							httpContext.Items["RPSOrgIdPUID"] = orgIdPuid;
							if (!AuthCommon.IsFrontEnd)
							{
								httpContext.Request.Headers["RPSEnv"] = (string)httpContext.Items["RPSEnv"];
								httpContext.Request.Headers["RPSMemberName"] = membername;
								httpContext.Request.Headers["RPSPUID"] = puid;
								httpContext.Request.Headers["RPSOrgIdPUID"] = orgIdPuid;
							}
							if (rpsTicketType == 2U)
							{
								ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceDebug(0L, "Redirect authenticated using LiveIdAuthentication.Authenticate, save cookies");
								LogonLatencyLogger.UpdateCookie(httpContext, "LGN02");
							}
							if (!LiveIdAuthenticationModule.EnsureAccountTerminationStatus(httpContext, propertySet, issueInstant))
							{
								return false;
							}
							if (!isConsumerRequestForO365)
							{
								LiveIdAuthenticationModule.VerifyActivityTimeout(httpContext, propertySet, deprecatedRpsTicketObject);
							}
							if (principal.Identity is LiveIDIdentity)
							{
								upn = (principal.Identity as LiveIDIdentity).PrincipalName;
							}
							if (string.IsNullOrWhiteSpace(upn))
							{
								upn = membername;
							}
							if (rpsTicketType == 2U)
							{
								ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceDebug(0L, "Redirect authenticated using LiveIdAuthentication.Authenticate, save cookies");
								LiveIdAuthenticationModule.SaveOrganizationNameCookie(httpContext, Utilities.GetUserDomain(membername));
								LiveIdAuthenticationModule.SaveAuthCookieAndRedirectAsHttpGet(httpContext, rpsRespHeaders, requestUrl, upn, orgIdPuid);
							}
							return true;
						}, delegate
						{
							if (deprecatedRpsTicketObject != null)
							{
								deprecatedRpsTicketObject.Dispose();
								deprecatedRpsTicketObject = null;
							}
						}))
						{
							if (httpContext.Request.Cookies["DefaultAnchorMailbox"] == null)
							{
								ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceDebug(0L, "DefaultAnchorMailbox cookie was not present. Adding and redirecting.");
								httpContext.Response.AppendToLog("pltRoutingCookieMissing=1");
								LiveIdAuthenticationModule.SetDefaultAnchorMailboxCookie(httpContext, upn);
								if (httpContext.Request.HttpMethod.Equals("GET", StringComparison.OrdinalIgnoreCase))
								{
									httpContext.Response.Redirect(httpContext.Request.GetRequestUrlEvenIfProxied().AbsoluteUri);
								}
								else
								{
									Utilities.Render440TimeoutResponse(httpContext.Response, httpContext.Request.HttpMethod, httpContext);
								}
							}
							else
							{
								HttpCookie httpCookie2 = httpContext.Request.Cookies["DefaultAnchorMailbox"];
								HttpCookie httpCookie3 = (httpContext.Request.Cookies["RPSSecAuth"] == null) ? httpContext.Request.Cookies["RPSAuth"] : httpContext.Request.Cookies["RPSSecAuth"];
								string value = httpCookie2.Value;
								if (!string.IsNullOrWhiteSpace(value))
								{
									identityDiagnostics.AppendFormat("liveIdDefaultAnchorMailboxCookie=<PII>{0}</PII>", value);
								}
								if (httpCookie2.Expires != httpCookie3.Expires)
								{
									ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceDebug<DateTime>(0L, "DefaultAnchorMailbox cookie needs its expiration updated to match RPS: {0}.", httpCookie3.Expires);
									LiveIdAuthenticationModule.SetDefaultAnchorMailboxCookie(httpContext, upn);
								}
								if (httpContext.Request.Cookies["logonLatency"] != null)
								{
									LogonLatencyLogger.WriteLatencyToIISLogAndDeleteCookie(httpContext);
								}
								LiveIdAuthenticationModule.CheckTOU(httpContext, propertySet);
								CompositeIdentityBuilder.AddIdentity(httpContext, principal.Identity as GenericIdentity, AuthenticationAuthority.ORGID);
								httpContext.User = principal;
							}
						}
					}
				}
			}
			catch (Exception exception)
			{
				LiveIdAuthenticationModule.HandleAuthException(httpContext, exception);
			}
			finally
			{
				try
				{
					string text = identityDiagnostics.ToString();
					if (LiveIdAuthenticationModule.IdentityTracingEnabled && !string.IsNullOrWhiteSpace(text))
					{
						httpContext.Response.AppendToLog(text);
					}
				}
				catch (Exception)
				{
				}
				LogonLatencyLogger.LogTimeInLiveIdModule(httpContext);
				if (!AuthCommon.IsFrontEnd)
				{
					ServiceCommonMetadataPublisher.PublishMetadata();
				}
			}
		}

		private static void HandleSessionTimedOutFromExchangePerspective(HttpContext httpContext, string reason, bool clearCookies = true)
		{
			httpContext.Response.AppendToLog(reason);
			if (Utilities.Need440Response(httpContext.Request))
			{
				ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceDebug(0L, "Session timed out, need 440, render 440 response timeout");
				Utilities.Render440TimeoutResponse(httpContext.Response, httpContext.Request.HttpMethod, httpContext);
				return;
			}
			ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceDebug<string>(0L, "InternalOnAuthenticate(): Session timed out ({0}), redirect to live logout", reason);
			httpContext.Response.AppendToLog(reason);
			LiveIdAuthenticationModule.WriteAuthenticatedUserToIISLog(httpContext);
			LiveIdAuthenticationModule.RedirectToLiveLogout(httpContext, LiveIdAuthenticationModule.GetOrgOwnSiteName(httpContext), LiveIdAuthenticationModule.GetOrgOwnSiteReturnUrlHost(httpContext), LiveIdAuthenticationModule.iOwnSiteDNSSubdomain, true, clearCookies);
		}

		private static bool EnsureAccountTerminationStatus(HttpContext httpContext, LiveIdPropertySet liveIdProperties, uint issueInstant)
		{
			string puid = liveIdProperties.PUID;
			SidBasedIdentity sidBasedIdentity = liveIdProperties.Identity as SidBasedIdentity;
			bool result;
			if (AuthCommon.IsFrontEnd)
			{
				ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceDebug(0L, "AccountTermination check not running because it can only run on BE auth machines");
				result = true;
			}
			else if (!LiveIdAuthenticationModule.isAccountTerminationEnabled)
			{
				ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceWarning(0L, "AccountTermination check not running because it is not enabled in the web.config");
				result = true;
			}
			else if (sidBasedIdentity == null)
			{
				ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceWarning(0L, "AccountTermination check not running because the liveid identity is not sid-based");
				result = true;
			}
			else if (!UrlUtilities.IsAuthRedirectRequest(httpContext.Request) && LiveIdAuthenticationModule.PltPaths.Contains(httpContext.Request.Path))
			{
				ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceDebug(0L, "AccountTermination check not running because the request path is a plt path: " + httpContext.Request.Path);
				result = true;
			}
			else if (liveIdProperties.RpsTicketType == 2U)
			{
				ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceDebug(0L, "AccountTermination check not running because the request is a postback from live id");
				result = true;
			}
			else
			{
				IAccountValidationContext accountValidationContext = new AccountValidationContextBySID(sidBasedIdentity.Sid, sidBasedIdentity.UserOrganizationId, ExDateTime.UtcNow.AddSeconds((double)(LiveIdAuthenticationModule.SecondsSinceInstant(issueInstant) * -1L)), "Microsoft.Exchange.OWA");
				httpContext.Items["AccountValidationContext"] = accountValidationContext;
				AccountState accountState = accountValidationContext.CheckAccount();
				result = LiveIdAuthenticationModule.HandleAccountValidationState(httpContext, liveIdProperties, accountState);
			}
			return result;
		}

		private static void RaiseAccountTerminationException(HttpContext httpContext, AccountState state, string memberName)
		{
			AccountTerminationException exception = new AccountTerminationException(state);
			Utilities.HandleException(httpContext, exception, false);
		}

		private static bool HandleAccountValidationState(HttpContext httpContext, LiveIdPropertySet liveIdProperties, AccountState accountState)
		{
			bool result;
			if (accountState == AccountState.AccountEnabled)
			{
				result = true;
			}
			else
			{
				result = false;
				ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceWarning<AccountState>(0L, "AccountTermination check found account failure state {0}", accountState);
				httpContext.Response.AppendToLog("&at=" + accountState.ToString());
				if (accountState == AccountState.PasswordExpired)
				{
					LiveIdAuthenticationModule.HandleSessionTimedOutFromExchangePerspective(httpContext, "&LogoffReason=AccountState.PasswordExpired", false);
				}
				else if (OfflineClientRequestUtilities.IsRequestFromMOWAClient(httpContext.Request, httpContext.Request.UserAgent))
				{
					LiveIdAuthenticationModule.SendMowaDisabledResponse(httpContext, liveIdProperties, accountState);
				}
				else if (OfflineClientRequestUtilities.IsRequestFromOfflineClient(httpContext.Request) && Utilities.Need440Response(httpContext.Request))
				{
					LiveIdAuthenticationModule.SendOfflineOwaWipeResponse(httpContext, liveIdProperties, accountState);
				}
				else if (Utilities.Need440Response(httpContext.Request))
				{
					LiveIdAuthenticationModule.HandleSessionTimedOutFromExchangePerspective(httpContext, "&LogoffReason=AccountState." + accountState.ToString(), false);
				}
				else
				{
					LiveIdAuthenticationModule.RaiseAccountTerminationException(httpContext, accountState, liveIdProperties.MemberName);
				}
			}
			return result;
		}

		private static void SendMowaDisabledResponse(HttpContext httpContext, LiveIdPropertySet liveIdProperties, AccountState state)
		{
			OwaExtendedError.SendError(httpContext.Response.Headers, delegate(HttpStatusCode sc)
			{
				httpContext.Response.StatusCode = (int)sc;
			}, OwaExtendedErrorCode.MowaDisabled, new AccountTerminationException(state).Message, liveIdProperties.MemberName, state.ToString());
		}

		private static void SendOfflineOwaWipeResponse(HttpContext httpContext, LiveIdPropertySet liveIdProperties, AccountState state)
		{
			OwaExtendedError.SendError(httpContext.Response.Headers, delegate(HttpStatusCode sc)
			{
				httpContext.Response.StatusCode = (int)sc;
			}, OwaExtendedErrorCode.RemoteWipe, new AccountTerminationException(state).Message, liveIdProperties.MemberName, state.ToString());
		}

		private void OnAuthenticateRequest(object source, EventArgs e)
		{
			HttpApplication httpApplication = (HttpApplication)source;
			HttpContext context = ((HttpApplication)source).Context;
			try
			{
				if ((!context.Request.IsAuthenticated || CompositeIdentityBuilder.IsLiveIdAuthStillNeededForOwa(context)) && !LiveIdAuthenticationModule.DoesCommonAccessTokenExist(context))
				{
					if (AuthCommon.IsFrontEnd)
					{
						if (LiveIdAuthenticationModule.IsEcpAnonymousRequest(context) || LiveIdAuthenticationModule.IsOwaAnonymousRequest(context))
						{
							return;
						}
						if (LiveIdAuthenticationModule.hostNameController.TrySwitchOwaHostNameAndReturnPermanentRedirect(context))
						{
							return;
						}
					}
					if (LiveIdAuthenticationModule.ShouldPeformFullAuthentication(context))
					{
						ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceDebug(0L, "OnAuthenticateRequest(): Completing full auth on front end.");
						LiveIdAuthenticationModule.ContinueOnAuthenticate(context);
					}
					else
					{
						ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceDebug(0L, "OnAuthenticateRequest(): Allowing mailbox server to authenticate request.");
						GenericIdentity identity = new GenericIdentity(AuthCommon.MemberNameNullSid.ToString(), "OrgId");
						context.User = new GenericPrincipal(identity, null);
					}
				}
			}
			catch (Exception exception)
			{
				LiveIdAuthenticationModule.HandleAuthException(context, exception);
			}
		}

		private void OnPostAuthenticateRequest(object source, EventArgs e)
		{
			HttpContext context = ((HttpApplication)source).Context;
			Uri requestUrlEvenIfProxied = context.Request.GetRequestUrlEvenIfProxied();
			if (LiveIdAuthenticationModule.IsLogoffRequest(requestUrlEvenIfProxied))
			{
				if (LiveIdAuthenticationModule.HasLiveLogoffParameter(requestUrlEvenIfProxied) || LiveIdAuthenticationModule.IsReferalFromTdsLiveLogout(context))
				{
					string siteName = LiveIdAuthenticationModule.GetOrgOwnSiteName(context);
					LiveIdAuthenticationModule.ClearAuthCookies(context, siteName);
					if (!string.IsNullOrEmpty(LiveIdAuthenticationModule.o365SiteName) && LiveIdAuthenticationModule.IsPersonalInstanceUrl(context) && !LiveIdAuthenticationModule.o365SiteName.Equals(LiveIdAuthenticationModule.orgOwnSiteName))
					{
						LiveIdAuthenticationModule.AddRealmParameterFromCookie(context);
						UriBuilder uriBuilder = new UriBuilder(requestUrlEvenIfProxied);
						uriBuilder.Host = LiveIdAuthenticationModule.o365SiteName;
						context.Response.Redirect(uriBuilder.ToString());
						return;
					}
					LiveIdAuthenticationModule.SendOrgIdLogoutConfirmation(context, siteName);
					return;
				}
				else
				{
					ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceDebug(0L, "OnPostAuthenticateRequest(): Application initiated logout. RedirectToLiveLogout");
					context.Response.AppendToLog("&LogoffReason=LogoffRequestApplication");
					LiveIdAuthenticationModule.WriteAuthenticatedUserToIISLog(context);
					LiveIdAuthenticationModule.RedirectToLiveLogout(context, LiveIdAuthenticationModule.GetOrgOwnSiteName(context), LiveIdAuthenticationModule.GetOrgOwnSiteReturnUrlHost(context), LiveIdAuthenticationModule.iOwnSiteDNSSubdomain, false, true);
				}
			}
		}

		private void OnBeginRequest(object source, EventArgs e)
		{
			HttpContext context = ((HttpApplication)source).Context;
			PublishingUrl publishingUrl = null;
			string empty = string.Empty;
			SmtpDomain smtpDomain = null;
			Uri requestUrlEvenIfProxied = context.Request.GetRequestUrlEvenIfProxied();
			UrlUtilities.SaveOriginalRequestUrlToContext(context, requestUrlEvenIfProxied);
			try
			{
				publishingUrl = PublishingUrl.Create(requestUrlEvenIfProxied.ToString());
			}
			catch (ArgumentException)
			{
				return;
			}
			try
			{
				smtpDomain = new SmtpDomain(publishingUrl.Domain);
			}
			catch (FormatException)
			{
				return;
			}
			OrganizationId a = DomainToOrganizationIdCache.Singleton.Get(smtpDomain);
			if (!(a != null))
			{
				Type typeFromHandle = typeof(CannotResolveTenantNameException);
				context.Response.AppendToLog(string.Format("&MservEx={0}", typeFromHandle.ToString()));
				context.Response.Headers.Add("X-OWA-Error", typeFromHandle.FullName);
				return;
			}
		}

		public const string TimeoutLogoutPage = "auth/TimeoutLogout.aspx";

		public const string SuiteServiceProxyPage = "/owa/SuiteServiceProxy.aspx";

		public const string RedirSuiteServiceProxyPageTemplate = "/owa/auth/RedirSuiteServiceProxy.aspx?returnUrl={0}";

		public const string RpsEnvKey = "RPSEnv";

		public const string MemberNameKey = "RPSMemberName";

		public const string LiveIdPuidKey = "RPSPUID";

		public const string OrgIdPuidKey = "RPSOrgIdPUID";

		internal const string SilentAuthQueryStringKey = "silent";

		internal const string SuiteServiceProxyInitCookie = "SuiteServiceProxyInit";

		internal const string SkippedAuthForAnonResourceKey = "LiveIdSkippedAuthForAnonResource";

		internal const string O365ConsumerRequestCookieName = "O365Consumer";

		internal const string O365ConsumerQueryStringName = "isc";

		internal const string IsConsumerQueryCookieValue = "1";

		private const char CommaSeparator = ',';

		private const string RPSAuthCookieName = "RPSAuth";

		private const string RPSSecAuthCookieName = "RPSSecAuth";

		private const string LiveIdAuthModuleLogoffPageKey = "LiveIdAuthModuleLogoffPage";

		private const string LogoffPageDefault = "logoff.aspx";

		private const string LogoffRequest = "logoff.owa";

		private const string CobrandingRedirectPage = "cobrandingredir.aspx";

		private const string CobrandingRedirectPagePath = "owa/auth/cobrandingredir.aspx";

		private const string EducationPagePath = "/LIDAuth/Education.aspx";

		private const string SignoutPagePath = "/owa/auth/signout.aspx";

		private const string F5RuleOriginalHostParameter = "targetName";

		private const string TenantSignOutUrl = "CoTS";

		private const string LiveLogoffParameter = "exlive=1";

		private const string ExchangeLogoffParameter = "src=exch";

		private const string ExchangeLogonParameter = "exch=1";

		private const string RruUrlParameter = "rru=contacts";

		private const string AccrualParameter = "accrual";

		private const string UseBackendAuthKey = "LiveIdCookieAuthModule.EnableBEAuthVersion";

		private const string SkipRandomBEADLookupKey = "LiveIdSkipAdLookupOnRandomBE";

		private const string ReturnToOringinalUrlByDefaultKey = "ReturnToOringinalUrlByDefault";

		private const string ReturnUrlParameter = "ru";

		private const string P3P = "P3P";

		private const string P3PHeaders = "CP=\"ALL IND DSP COR ADM CONo CUR CUSo IVAo IVDo PSA PSD TAI TELo OUR SAMo CNT COM INT NAV ONL PHY PRE PUR UNI\"";

		private const string LogoutResponseImageContentType = "image/gif";

		private const string AffinityCookieName = "exchangecookie";

		private const string OrganizationNameCookieName = "orgName";

		private const string DomainNameCookieName = "domainName";

		private const string LiveIdAuthModuleTimeoutEnabledKey = "LiveIdAuthModuleTimeoutEnabled";

		private const bool TimeoutEnabledDefault = true;

		private const string IdentityTracingEnabledKey = "IdentityTracingEnabled";

		private const string LiveIdAuthModuleTimeoutIntervalInSecondsKey = "LiveIdAuthModuleTimeoutIntervalInSeconds";

		private const int TimeoutIntervalInSecondsDefault = 82800;

		private const string LiveIdAuthPolicyOverrideValueKey = "LiveIdAuthPolicyOverrideValue";

		private const string AuthPolicyOverrideValueDefault = "MBI_SSL";

		private const string HttpMethodGet = "GET";

		private const string HttpMethodPost = "POST";

		private const string LiveIdAuthModuleCacheEnabledKey = "LiveIdAuthModuleCacheEnabled";

		private const bool CacheEnabledDefault = true;

		private const string LiveIdAuthModuleCacheSizeKey = "LiveIdAuthModuleCacheSize";

		private const int CacheSizeDefault = 8192;

		private const string LiveIdAuthModuleCacheDurationInMinutesKey = "LiveIdAuthModuleCacheDurationInMinutes";

		private const int CacheDurationInMinutesDefault = 30;

		private const string LiveIdAuthModuleSslOffloadedKey = "LiveIdAuthModuleSslOffloaded";

		private const bool SslOffloadedDefault = false;

		private const string CheckAppPasswordKey = "CheckAppPassword";

		private const bool AppPasswordCheckDefault = true;

		private const string PodSiteStartRange = "PodSiteStartRange";

		private const string PodSiteEndRange = "PodSiteEndRange";

		private const string DefaultO365SiteName = "outlook.office365.com";

		private const string DefaultO365Namespace = "office365.com";

		private const string HttpPrefix = "http";

		private const string HttpsPrefix = "https";

		private const string OutlookDotComHostNamePrefix = "www";

		private const string PremiumVanityDomainRealmKey = "PremiumVanityDomainRealm";

		private const string LiveIdAuthModuleOrgOwnSiteNameKey = "LiveIdAuthModuleSiteName";

		private const string LiveIdAuthModuleOwaEcpCanonicalHostNameKey = "OwaEcpCanonicalHostName";

		private const string LiveIdAuthModuleIOwnSiteNameKey = "LiveAuthModuleAccrualSiteName";

		private const string LiveIdAuthModuleLegacySiteNameKey = "LiveIdAuthModuleLegacySiteName";

		private const string LiveIdAuthModuleO365SiteNameKey = "LiveIdAuthModuleO365SiteName";

		private const string LiveIdAuthModuleO365NamespaceKey = "LiveIdAuthModuleO365Namespace";

		private const string LiveIdAuthModuleSDFSiteNameKey = "LiveIdAuthModuleSDFSiteName";

		private const string LiveIdAuthAccrualSignUrlKey = "LiveIdAuthAccrualSignUrl";

		private const string ContinueOnMSAInitErrorsKey = "ContinueOnMSAInitErrors";

		private const string IOwnSiteSubDomainDNS = "AccrualSiteSubdomainDNS";

		private const string AccrualForceSignInPolicy = "MBI_KEY_FORCE_REFRESH";

		private const string EduNamespaceKey = "EduNamespace";

		private const string EduWHRKey = "EduWHR";

		private const string EducationPageKey = "EducationPage";

		private const string LiveIdAuthNewMailUrlKey = "LiveIdAuthNewMailUrl";

		private const string LiveIdAuthEnableHotmailRedirectKey = "LiveIdAuthEnableHotmailRedirect";

		private const string RedirectToConsumerInstKey = "RedirectToConsumerInst";

		private const string MservLookupEnabledinTestKey = "MservLookupEnabledinTest";

		private const bool MservLookupEnabledinTestDefault = false;

		private const string NewMailOptimizationsEnabledKey = "NewMailOptimizationsEnabled";

		private const bool NewMailOptimizationsEnabledDefault = true;

		private const string AccountTerminationEnabledKey = "AccountTerminationEnabled";

		private const bool AccountTerminationEnabledDefault = false;

		private const string LiveIdAuthModuleSDFSiteNameDefault = "sdfpilot.outlook.com";

		private const string SchemePostFix = "://";

		private const string ForwardSlash = "/";

		private const int ResponseStatusCode401 = 401;

		private const string ResponseStatus401 = "401 Unauthorized Access";

		private const int ResponseStatusCode403 = 403;

		private const string ResponseStatus403 = "403 Forbidden";

		private const int ResponseStatusCode500 = 500;

		private const string ResponseStatus500 = "500 Internal Server Error";

		private const string ErrorMessageHtml = "<html><body>{0}</body></html>";

		private const string TermsOfUseIMField = "InstantMessage_TOU";

		private const string TimeoutRedirectPolicyDefault = "HBI";

		private const string CalendarVDirPostfix = "/calendar";

		private const string OmaVDirPostfix = "/oma";

		private const string TimeoutRedirectPolicyKey = "TimeoutRedirectPolicy";

		private const string SlidingWindowOverrideKey = "SlidingWindowOverride";

		private const string MemberNameIgnorePrefixesKey = "MemberNameIgnorePrefixes";

		private const string DefaultMemberNameIgnorePrefixes = "Live.com#";

		private const string ExceptionParameter = "lex";

		private const string SuiteProxyCookieKey = "SuiteServiceProxyKey";

		private const string AccountTerminationLogParam = "&at=";

		public static bool IdentityTracingEnabled = true;

		internal static readonly SecurityIdentifier MemberNameNullSid = new SecurityIdentifier(WellKnownSidType.NullSid, null);

		internal static string CanonicalHostName;

		private static string[] memberNameIgnorePrefixes = null;

		private static bool isBeAuthEnabled = false;

		private static bool skipAdLookupOnRandomBe = false;

		private static readonly string[] UserCookies = new string[]
		{
			"UserContext",
			"SuiteServiceProxyKey",
			"DefaultAnchorMailbox"
		};

		private static readonly HashSet<string> PltPaths = new HashSet<string>(new string[]
		{
			"/owa/sessiondata.ashx",
			"/owa/userspecificresourceinjector.ashx",
			"/owa/"
		});

		private static readonly TimeSpan MaxSlidingWindow = new TimeSpan(288000000000L);

		private static byte[] logoutResponseImageByteArray = new byte[]
		{
			71,
			73,
			70,
			56,
			57,
			97,
			16,
			0,
			16,
			0,
			196,
			0,
			0,
			41,
			74,
			156,
			49,
			90,
			173,
			57,
			57,
			57,
			74,
			115,
			206,
			99,
			82,
			49,
			99,
			99,
			99,
			99,
			140,
			214,
			115,
			90,
			49,
			115,
			99,
			74,
			115,
			156,
			231,
			123,
			165,
			247,
			132,
			115,
			74,
			132,
			132,
			132,
			132,
			173,
			247,
			156,
			189,
			247,
			173,
			132,
			66,
			173,
			156,
			115,
			173,
			198,
			byte.MaxValue,
			189,
			173,
			132,
			198,
			156,
			66,
			206,
			173,
			74,
			206,
			189,
			148,
			214,
			214,
			214,
			231,
			189,
			99,
			231,
			222,
			181,
			239,
			198,
			99,
			247,
			206,
			107,
			247,
			222,
			123,
			byte.MaxValue,
			231,
			140,
			byte.MaxValue,
			247,
			156,
			byte.MaxValue,
			byte.MaxValue,
			198,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			33,
			249,
			4,
			1,
			0,
			0,
			31,
			0,
			44,
			0,
			0,
			0,
			0,
			16,
			0,
			16,
			0,
			0,
			5,
			132,
			224,
			103,
			49,
			197,
			98,
			125,
			104,
			170,
			126,
			204,
			105,
			21,
			107,
			12,
			163,
			66,
			172,
			54,
			73,
			98,
			236,
			3,
			86,
			85,
			146,
			96,
			240,
			147,
			136,
			24,
			35,
			142,
			128,
			103,
			185,
			236,
			112,
			16,
			196,
			72,
			163,
			161,
			104,
			40,
			61,
			157,
			44,
			103,
			2,
			45,
			42,
			0,
			134,
			4,
			160,
			233,
			164,
			92,
			160,
			134,
			136,
			34,
			160,
			27,
			59,
			57,
			148,
			141,
			6,
			237,
			80,
			240,
			0,
			239,
			201,
			69,
			126,
			248,
			24,
			2,
			0,
			129,
			129,
			25,
			20,
			15,
			26,
			27,
			122,
			125,
			40,
			24,
			77,
			28,
			28,
			27,
			142,
			143,
			15,
			4,
			41,
			21,
			141,
			145,
			145,
			23,
			148,
			40,
			18,
			88,
			143,
			114,
			26,
			26,
			25,
			163,
			155,
			31,
			16,
			11,
			168,
			7,
			170,
			170,
			4,
			173,
			165,
			54,
			176,
			177,
			49,
			33,
			0,
			59
		};

		private static string logoffPage;

		private static long ticksBefore1970;

		private static bool timeoutEnabled;

		private static int timeoutIntervalInSeconds;

		private static MruDictionaryCache<string, LiveIdAuthenticationModule.CacheEntry> puidToPrincipalCache;

		private static bool puidToSidCacheEnabled;

		private static int puidToSidCacheSize;

		private static int puidToSidCacheDurationInMinutes;

		private static bool sslOffloaded;

		private static bool appPasswordCheckEnabled;

		private static PropertyDefinition[] propertyDefinitionArraySID;

		private static string virtualDirectoryName;

		private static string virtualDirectoryNameWithLeadingSlash;

		private static bool liveIdAuthenticationEnabled;

		private static bool staticVariablesInitialized;

		private static object staticLock = new object();

		private static string podRedirectTemplate;

		private static int podSiteStartRange;

		private static int podSiteEndRange;

		private static string timeoutRedirectPolicy;

		private static TimeSpan slidingWindowOverride;

		private static string authPolicyOverrideValue;

		private static bool? isSessionDataPreloadEnabled;

		private static bool isAccountTerminationEnabled;

		private static string orgOwnSiteName;

		private static string iOwnSiteName;

		private static string legacySiteName;

		private static string o365SiteName;

		private static string o365Namespace;

		private static string[] sdfSiteNames;

		private static string liveIdAuthAccrualSignUrl;

		private static bool continueOnMSAInitErrors;

		private static string iOwnSiteDNSSubdomain;

		private static string orgOwnSiteReturnURLHostEnterprise;

		private static string orgOwnSiteReturnURLHostConsumer;

		private static string legacySiteReturnURLHostEnterprise;

		private static string legacySiteReturnURLHostConsumer;

		private static string o365SiteReturnURLHostEnterprise;

		private static string o365SiteReturnURLHostConsumer;

		private static string eduNamespaceKey;

		private static string eduWHRKey;

		private static Regex regexEduRealmParameter;

		private static string eduWHRRealmParameter;

		private static bool educationPageEnabled;

		private static string liveIdAuthNewMailUrl;

		private static bool enableHotmailRedirect;

		private static bool redirectToConsumerInst;

		private static bool isInCalendarVDir;

		private static bool isInOmaVDir;

		private static bool returnToOringinalUrlByDefault;

		private static bool isNewMailOptimizationsEnabled;

		private static bool isMservLookupEnabledinTest;

		private static string premiumVanityDomainRealm;

		private static HostNameController hostNameController;

		internal class CacheEntry
		{
			internal CacheEntry(GenericPrincipal principal, long expiryTimeInTicks)
			{
				this.principal = principal;
				this.expiryTimeInTicks = expiryTimeInTicks;
			}

			internal GenericPrincipal Principal
			{
				get
				{
					return this.principal;
				}
			}

			internal long ExpiryTimeInTicks
			{
				get
				{
					return this.expiryTimeInTicks;
				}
			}

			private readonly GenericPrincipal principal;

			private readonly long expiryTimeInTicks;
		}
	}
}
