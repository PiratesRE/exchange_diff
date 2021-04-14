using System;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.Net;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.Cafe;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Security;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Security.Authentication.FederatedAuthService;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Security.Authentication
{
	public class LiveIdBasicAuthModule : IHttpModule
	{
		public static bool SyncADBackendOnly
		{
			get
			{
				return LiveIdBasicAuthModule.syncADBackEndOnly;
			}
		}

		internal static ExEventLog EventLogger
		{
			get
			{
				return LiveIdBasicAuthModule.eventLogger;
			}
		}

		internal static string LocalFqdn
		{
			get
			{
				if (string.IsNullOrEmpty(LiveIdBasicAuthModule.localFqdn))
				{
					LiveIdBasicAuthModule.localFqdn = Dns.GetHostEntry("LocalHost").HostName;
				}
				return LiveIdBasicAuthModule.localFqdn;
			}
		}

		internal static string FrontEndOWAUrl
		{
			get
			{
				if (string.IsNullOrEmpty(LiveIdBasicAuthModule.frontEndOWAUrl))
				{
					LiveIdBasicAuthModule.frontEndOWAUrl = FrontEndLocator.GetDatacenterFrontEndOwaUrl().AbsoluteUri;
				}
				return LiveIdBasicAuthModule.frontEndOWAUrl;
			}
		}

		static LiveIdBasicAuthModule()
		{
			LiveIdBasicAuthModule.fileSearchAssemblyResolver.FileNameFilter = new Func<string, bool>(LiveIdBasicAuthModule.AssemblyFileNameFilter);
			LiveIdBasicAuthModule.fileSearchAssemblyResolver.SearchPaths = new string[]
			{
				ExchangeSetupContext.BinPath
			};
			LiveIdBasicAuthModule.fileSearchAssemblyResolver.Recursive = false;
			LiveIdBasicAuthModule.fileSearchAssemblyResolver.Install();
		}

		private static bool AssemblyFileNameFilter(string fileName)
		{
			if (AssemblyResolver.ExchangePrefixedAssembliesOnly(fileName))
			{
				return true;
			}
			foreach (string text in LiveIdBasicAuthModule.approvedAssemblies)
			{
				if (text.EndsWith("."))
				{
					if (fileName.StartsWith(text, StringComparison.OrdinalIgnoreCase))
					{
						return true;
					}
				}
				else if (string.Compare(fileName, text, StringComparison.OrdinalIgnoreCase) == 0)
				{
					return true;
				}
			}
			return false;
		}

		void IHttpModule.Init(HttpApplication application)
		{
			application.BeginRequest += this.OnBeginRequest;
			application.AddOnAuthenticateRequestAsync(new BeginEventHandler(this.BeginOnAuthenticate), new EndEventHandler(this.EndOnAuthenticate));
			if (AuthCommon.IsFrontEnd)
			{
				application.PreSendRequestHeaders += this.OnPreSendRequestHeaders;
			}
			application.EndRequest += this.OnEndRequest;
			if (!LiveIdBasicAuthModule.globalVariableInitialized)
			{
				string text = null;
				int rpsTicketLifetime = 0;
				this.TryReadAppSetting("LiveIdBasicAuthModule.SyncAD", ref LiveIdBasicAuthModule.syncAD);
				this.TryReadAppSetting("LiveIdBasicAuthModule.EnableCompositeIdentityHandling", ref LiveIdBasicAuthModule.isCompositeIdentityHandlingEnabled);
				this.TryReadAppSetting("LiveIdBasicAuthModule.SyncADBackEndOnly", ref LiveIdBasicAuthModule.syncADBackEndOnly);
				LiveIdBasicAuthModule.backendDoesAuth = this.TryReadAppSetting("LiveIdBasicAuthModule.EnableBEAuthVersion", ref text);
				this.TryReadAppSetting("LiveIdBasicAuthModule.SyncUPN", ref LiveIdBasicAuthModule.syncUPN);
				this.TryReadAppSetting("LiveIdBasicAuthModule.CafeProxy", ref LiveIdBasicAuthModule.cafeProxy);
				this.TryReadAppSetting("LiveIdBasicAuthModule.MailboxProxy", ref LiveIdBasicAuthModule.mailboxProxy);
				this.TryReadAppSetting("LiveIdBasicAuthModule.AllowLiveIDOnlyAuth", ref LiveIdBasicAuthModule.allowLiveIDOnlyAuth);
				this.TryReadAppSetting("LiveIdBasicAuthModule.AllowCookieAuth", ref LiveIdBasicAuthModule.allowCookieAuth);
				this.TryReadAppSetting("LiveIdBasicAuthModule.AllowOfflineOrgIdAsPrimeAuth", ref LiveIdBasicAuthModule.allowOfflineOrgIdAsPrimeAuth);
				this.TryReadAppSetting("LiveIdBasicAuthModule.AuthCookieLifetime", ref LiveIdBasicAuthModule.BasicAuthCookieLifetime);
				this.TryReadAppSetting("LiveIdBasicAuthModule.AuthCookieName", ref LiveIdBasicAuthModule.BasicAuthCookieName);
				this.TryReadAppSetting("LiveIdBasicAuthModule.FailRedirects", ref LiveIdBasicAuthModule.failRedirects);
				this.TryReadAppSetting("LiveIdBasicAuthModule.AllowHotmailRedirect", ref LiveIdBasicAuthModule.allowHotmailRedirect);
				this.TryReadAppSetting("LiveIdBasicAuthModule.DisposeWindowsIdentities", ref LiveIdBasicAuthModule.disposeIdentities);
				if (!this.TryReadAppSetting("LiveIdBasicAuthModule.TranslateWCFException", ref LiveIdBasicAuthModule.translateWCFException) && "Microsoft.Exchange.WebServices".Equals(ConfigurationManager.AppSettings["LiveIdBasicAuthModule.ApplicationName"], StringComparison.OrdinalIgnoreCase))
				{
					LiveIdBasicAuthModule.translateWCFException = true;
				}
				LiveIdBasicAuthModule.applicationName = ConfigurationManager.AppSettings["LiveIdBasicAuthModule.ApplicationName"];
				LiveIdBasicAuthModule.hotmailRedirectUrl = ConfigurationManager.AppSettings["LiveIdBasicAuthModule.HotmailRedirectUrl"];
				string text2 = ConfigurationManager.AppSettings["LiveIdBasicAuthModule.HotmailTopLevelDomains"];
				if (!string.IsNullOrWhiteSpace(text2))
				{
					string[] array = text2.Split(new char[]
					{
						','
					}, StringSplitOptions.RemoveEmptyEntries);
					LiveIdBasicAuthModule.hotmailDomains = new string[array.Length];
					int num = 0;
					foreach (string str in array)
					{
						LiveIdBasicAuthModule.hotmailDomains[num++] = "@outlook." + str;
					}
				}
				else
				{
					LiveIdBasicAuthModule.hotmailDomains = new string[]
					{
						"com"
					};
				}
				this.TryReadAppSetting("LiveIdBasicAuthModule.AccessDeniedTarPitDelayms", ref LiveIdBasicAuthModule.tarpitAccessDenied);
				this.TryReadAppSetting("LiveIdBasicAuthModule.RecoverableErrorTarPitDelayms", ref LiveIdBasicAuthModule.tarpitRecoverableError);
				this.TryReadAppSetting("LiveIdBasicAuthModule.IdpFailureTarPitDelayms", ref LiveIdBasicAuthModule.tarpitIdpFailure);
				this.TryReadAppSetting("LiveIdBasicAuthModule.PasswordExpiredTarPitDelayms", ref LiveIdBasicAuthModule.tarpitPasswordExpired);
				this.TryReadAppSetting("LiveIdBasicAuthModule.RecoverableErrorStatus", ref LiveIdBasicAuthModule.recoverableErrorStatus);
				this.TryReadAppSetting("LiveIdBasicAuthModule.PasswordExpiredErrorStatus", ref LiveIdBasicAuthModule.passwordExpiredStatus);
				this.TryReadAppSetting("LiveIdBasicAuthModule.UserNotFoundStatus", ref LiveIdBasicAuthModule.userNotFoundStatus);
				this.TryReadAppSetting("LiveIdBasicAuthModule.IdpFailureStatus", ref LiveIdBasicAuthModule.idpFailureStatus);
				this.TryReadAppSetting("LiveIdBasicAuthModule.WcfTimeoutStatus", ref LiveIdBasicAuthModule.wcfTimeoutStatus);
				this.TryReadAppSetting("LiveIdBasicAuthModule.WcfTimeoutText", ref LiveIdBasicAuthModule.wcfTimeoutText);
				this.TryReadAppSetting("LiveIdBasicAuthModule.503RetryAfterSec", ref LiveIdBasicAuthModule.retryAfterSec);
				this.TryReadAppSetting("LiveIdBasicAuthModule.AdfsRulesDeniedStatus", ref LiveIdBasicAuthModule.adfsRulesDeniedStatus);
				this.TryReadAppSetting("LiveIdBasicAuthModule.UnfamiliarLocationStatus", ref LiveIdBasicAuthModule.unfamiliarLocationStatus);
				this.TryReadAppSetting("LiveIdBasicAuthModule.AccountNotProvisionedStatus", ref LiveIdBasicAuthModule.accountNotProvisionedStatus);
				this.TryReadAppSetting("LiveIdBasicAuthModule.InternalServerErrorStatus", ref LiveIdBasicAuthModule.internalServerErrorStatus);
				if (!string.IsNullOrEmpty(text = ConfigurationManager.AppSettings["LiveIdBasicAuthModule.UserAgentsForBackOff"]))
				{
					LiveIdBasicAuthModule.userAgentsToBackOff = text.Split(new char[]
					{
						','
					});
				}
				if (this.TryReadAppSetting("LiveIdBasicAuthModule.RpsTicketLifetimeSeconds", ref rpsTicketLifetime))
				{
					LiveIdSTSBase.RpsTicketLifetime = rpsTicketLifetime;
				}
				this.TryReadAppSetting("LiveIdBasicAuthModule.EnableClientSideLogonCache", ref LiveIdBasicAuthModule.enableClientSideLogonCache);
				this.TryReadAppSetting("LiveIdBasicAuthModule.PreferClientSideLogonCacheToCookie", ref LiveIdBasicAuthModule.preferClientSideLogonCacheToCookie);
				this.TryReadAppSetting("LiveIdBasicAuthModule.OfflineOrgIdCookieExpirationTimeInMin", ref LiveIdBasicAuthModule.OfflineOrgIdCookieExpirationTimeInMin);
				if (LiveIdBasicAuthModule.enableClientSideLogonCache)
				{
					this.TryReadAppSetting("LiveIdBasicAuthModule.LogonCacheSize", ref LiveIdBasicAuthModule.logonCacheSize);
					this.TryReadAppSetting("LiveIdBasicAuthModule.LogonCacheLifetime", ref LiveIdBasicAuthModule.logonCacheLifetime);
					this.TryReadAppSetting("LiveIdBasicAuthModule.Level1BadCredCacheSize", ref LiveIdBasicAuthModule.level1BadCredCacheSize);
					this.TryReadAppSetting("LiveIdBasicAuthModule.Level1BadCredLifetime", ref LiveIdBasicAuthModule.level1BadCredLifetime);
					this.TryReadAppSetting("LiveIdBasicAuthModule.Level2BadCredCacheSize", ref LiveIdBasicAuthModule.level2BadCredCacheSize);
					this.TryReadAppSetting("LiveIdBasicAuthModule.Level2BadCredLifetime", ref LiveIdBasicAuthModule.level2BadCredLifetime);
					this.TryReadAppSetting("LiveIdBasicAuthModule.Level2BadCredListSize", ref LiveIdBasicAuthModule.level2BadCredListSize);
					LogonCacheConfig.Initialize(LiveIdBasicAuthModule.badCredsLifetime, LiveIdBasicAuthModule.badCredsRecoverableLifetime);
					lock (LiveIdBasicAuthModule.cacheLockObject)
					{
						if (LiveIdBasicAuthModule.logonCache == null)
						{
							LiveIdBasicAuthModule.logonCache = new LogonCache(LiveIdBasicAuthModule.logonCacheSize, LiveIdBasicAuthModule.logonCacheLifetime, LiveIdBasicAuthModule.level1BadCredCacheSize, LiveIdBasicAuthModule.level1BadCredLifetime, LiveIdBasicAuthModule.level2BadCredCacheSize, LiveIdBasicAuthModule.level2BadCredLifetime, LiveIdBasicAuthModule.level2BadCredListSize);
						}
					}
				}
				if (string.IsNullOrEmpty(LiveIdBasicAuthModule.applicationName))
				{
					AuthModulePerformanceCounterHelper.Initialize("Unknown");
				}
				else
				{
					AuthModulePerformanceCounterHelper.Initialize(LiveIdBasicAuthModule.applicationName);
				}
				LiveIdBasicAuthModule.globalVariableInitialized = true;
			}
		}

		private bool TryReadAppSetting(string name, ref bool value)
		{
			bool flag;
			if (bool.TryParse(ConfigurationManager.AppSettings[name], out flag))
			{
				value = flag;
				return true;
			}
			return false;
		}

		private bool TryReadAppSetting(string name, ref int value)
		{
			int num;
			if (int.TryParse(ConfigurationManager.AppSettings[name], out num))
			{
				value = num;
				return true;
			}
			return false;
		}

		private bool TryReadAppSetting(string name, ref string value)
		{
			string text;
			if (!string.IsNullOrWhiteSpace(text = ConfigurationManager.AppSettings[name]))
			{
				value = text;
				return true;
			}
			return false;
		}

		void IHttpModule.Dispose()
		{
			if (LiveIdBasicAuthModule.disposeIdentities && this.lastUser != null)
			{
				((IDisposable)this.lastUser).Dispose();
				this.lastUser = null;
			}
			if (this.timer != null)
			{
				this.timer.Dispose();
				this.timer = null;
			}
		}

		private void OnBeginRequest(object source, EventArgs args)
		{
			this.TraceEnterFunction("OnBeginRequest");
			this.authInfo = new LiveIdBasicAuthInfo();
			this.receivedAuthCookie = false;
			this.organizationContext = null;
			this.memberName = null;
			this.userKey = null;
			this.logOfParsingAuthCookie = null;
			this.continueRequest = false;
			if (this.asyncOp != null)
			{
				ExTraceGlobals.AuthenticationTracer.TraceWarning((long)this.GetHashCode(), "OnBeginRequest called with existing asyncOp");
				this.asyncOp = null;
			}
		}

		private IAsyncResult BeginOnAuthenticate(object source, EventArgs args, AsyncCallback callback, object state)
		{
			this.TraceEnterFunction("BeginOnAuthenticate");
			this.lastException = null;
			this.logMessage = null;
			bool performFullAuth = false;
			ExWatson.SendReportOnUnhandledException(delegate()
			{
				this.application = (HttpApplication)source;
				HttpContext context = this.application.Context;
				if (this.asyncOp != null)
				{
					ExTraceGlobals.AuthenticationTracer.TraceError((long)this.GetHashCode(), "BeginOnAuthenticate called with existing asyncOp");
					throw new InvalidOperationException("this.asyncOp should be null");
				}
				if (this.continueRequest)
				{
					ExTraceGlobals.AuthenticationTracer.TraceError((long)this.GetHashCode(), "BeginOnAuthenticate called with existing continueRequest flag");
					throw new InvalidOperationException("this.continueRequest should be false");
				}
				if (LiveIdBasicAuthModule.disposeIdentities && this.lastUser != null)
				{
					((IDisposable)this.lastUser).Dispose();
				}
				this.lastUser = null;
				this.asyncOp = new LazyAsyncResult(this, state, callback);
				this.startTime = DateTime.UtcNow;
				if (context.Items.Contains(typeof(ActivityScope)))
				{
					IActivityScope activityScope = (IActivityScope)context.Items[typeof(ActivityScope)];
					this.requestId = ((activityScope != null) ? activityScope.ActivityId : Guid.Empty);
				}
				else
				{
					this.requestId = Guid.Empty;
				}
				if (LiveIdBasicAuthModule.isCompositeIdentityHandlingEnabled && CompositeIdentityAuthenticationHelper.IsCompositeIdentityEnabled())
				{
					this.compositeIdentityHelper = CompositeIdentityAuthenticationHelper.GetInstance(context);
				}
				string text = context.Request.Headers["Authorization"];
				if (this.compositeIdentityHelper == null)
				{
					if (context.Request.IsAuthenticated)
					{
						this.asyncOp.InvokeCallback();
						this.TraceExitFunction("BeginOnAuthenticate");
						return;
					}
				}
				else if (this.compositeIdentityHelper.IsMsaIdentityRequired && string.IsNullOrEmpty(this.compositeIdentityHelper.MsaMemberName) && string.IsNullOrEmpty(text))
				{
					this.application.Context.Response.StatusCode = 401;
					this.application.CompleteRequest();
					this.asyncOp.InvokeCallback();
					this.TraceExitFunction("BeginOnAuthenticate");
					return;
				}
				this.memberName = null;
				if (LiveIdBasicAuthModule.ParseCredentials(context, text, true, out this.authInfo.username, out this.authInfo.password))
				{
					this.memberName = Encoding.UTF8.GetString(this.authInfo.username).Trim();
				}
				else if (this.compositeIdentityHelper != null)
				{
					this.memberName = this.compositeIdentityHelper.MsaMemberName;
				}
				if (string.IsNullOrEmpty(this.memberName))
				{
					this.asyncOp.InvokeCallback();
					this.TraceExitFunction("BeginOnAuthenticate");
					return;
				}
				LiveIdBasicAuthModule.UpdateHttpContextItem(context, "WLID-MemberName", this.memberName);
				context.Response.AppendToLog("&LiveIdBasicMemberName=" + this.memberName);
				if (!SmtpAddress.IsValidSmtpAddress(this.memberName))
				{
					this.logMessage = "member name is not a valid SMTP address";
					LiveIdBasicAuthModule.UpdateHttpContextItem(this.application.Context, "LiveIdBasicLog", this.logMessage);
					context.Response.AppendToLog(string.Format("&{0}={1}", "LiveIdBasicLog", this.logMessage));
					this.asyncOp.InvokeCallback();
					this.TraceExitFunction("BeginOnAuthenticate");
					return;
				}
				if (LiveIdBasicAuthModule.allowHotmailRedirect && !string.IsNullOrEmpty(LiveIdBasicAuthModule.hotmailRedirectUrl) && LiveIdBasicAuthModule.IsHotmailAddress(this.memberName))
				{
					if (context.Request.Url.AbsolutePath.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
					{
						context.Response.AppendToLog("&HotmailRedirect=valid");
						context.Response.Redirect(LiveIdBasicAuthModule.hotmailRedirectUrl, false);
					}
					else
					{
						context.Response.AppendToLog("&HotmailRedirect=invalid");
						context.Response.StatusCode = 400;
					}
					this.application.CompleteRequest();
					this.asyncOp.InvokeCallback();
					this.TraceExitFunction("BeginOnAuthenticate");
					return;
				}
				if (!LiveIdBasicAuthModule.TryParseOrganizationContext(context.Request.Url, out this.organizationContext))
				{
					ExTraceGlobals.AuthenticationTracer.TraceError<string>((long)this.GetHashCode(), "Cannot parse organization context from Url: {0}", context.Request.Url.ToString());
					this.asyncOp.InvokeCallback();
					this.TraceExitFunction("BeginOnAuthenticate");
					return;
				}
				this.continueRequest = true;
				if (!string.IsNullOrEmpty(this.organizationContext))
				{
					LiveIdBasicAuthModule.UpdateHttpContextItem(context, "WLID-OrganizationContext", this.organizationContext);
				}
				if (LiveIdBasicAuthModule.backendDoesAuth)
				{
					GenericIdentity identity = new GenericIdentity(this.memberName, "LiveIdBasic");
					this.application.Context.User = new GenericPrincipal(identity, null);
					LiveIdBasicAuthModule.UpdateHttpContextItem(context, "WLID-BasicAuthModule", this);
					this.asyncOp.InvokeCallback();
					return;
				}
				performFullAuth = true;
			}, (object exception) => true, ReportOptions.DoNotCollectDumps | ReportOptions.DeepStackTraceHash);
			if (performFullAuth)
			{
				return this.InternalContinueOnAuthenticate(source, args, callback, state);
			}
			this.TraceExitFunction("BeginOnAuthenticate");
			return this.asyncOp;
		}

		internal static IAsyncResult ContinueOnAuthenticate(object source, EventArgs args, AsyncCallback callback, object state)
		{
			HttpApplication httpApplication = (HttpApplication)source;
			LiveIdBasicAuthModule liveIdBasicAuthModule = (LiveIdBasicAuthModule)httpApplication.Context.Items["WLID-BasicAuthModule"];
			if (liveIdBasicAuthModule == null)
			{
				ExTraceGlobals.AuthenticationTracer.TraceError((long)liveIdBasicAuthModule.GetHashCode(), "ContinueOnAuthenticate called with missing auth module");
				throw new ArgumentNullException("httpContext.Items[WLID-BasicAuthModule]");
			}
			httpApplication.Context.Items.Remove("WLID-BasicAuthModule");
			if (liveIdBasicAuthModule.asyncOp != null)
			{
				ExTraceGlobals.AuthenticationTracer.TraceError((long)liveIdBasicAuthModule.GetHashCode(), "ContinueOnAuthenticate called with existing asyncOp");
				throw new InvalidOperationException("ContinueOnAuthenticate called with existing asyncOp");
			}
			liveIdBasicAuthModule.asyncOp = new LazyAsyncResult(liveIdBasicAuthModule, state, callback);
			return liveIdBasicAuthModule.InternalContinueOnAuthenticate(source, args, callback, state);
		}

		private IAsyncResult InternalContinueOnAuthenticate(object source, EventArgs args, AsyncCallback callback, object state)
		{
			this.TraceEnterFunction("ContinueOnAuthenticate");
			ExWatson.SendReportOnUnhandledException(delegate()
			{
				HttpContext context = this.application.Context;
				if (this.asyncOp == null)
				{
					ExTraceGlobals.AuthenticationTracer.TraceError((long)this.GetHashCode(), "InternalContinueOnAuthenticate called without existing asyncOp");
					throw new InvalidOperationException("InternalContinueOnAuthenticate called without existing asyncOp");
				}
				if (!this.continueRequest)
				{
					ExTraceGlobals.AuthenticationTracer.TraceError((long)this.GetHashCode(), "ContinueOnAuthenticate called without existing continueRequest flag");
					throw new InvalidOperationException("ContinueOnAuthenticate called without existing continueRequest flag");
				}
				this.continueRequest = false;
				if (LiveIdBasicAuthModule.backendDoesAuth)
				{
					this.application.Context.User = null;
				}
				string text = context.Request.Headers["X-Forwarded-For"];
				string text2 = context.Request.Headers["User-Agent"];
				string text3 = context.Request.UserHostAddress;
				ExTraceGlobals.AuthenticationTracer.Information<string, string, string>((long)this.GetHashCode(), "X-Forwarded-For: '{0}' User-Agent: '{1}' UserHostAddress: '{2}'", text, text2, text3);
				if (LiveIdBasicAuthModule.mailboxProxy)
				{
					text3 = text;
				}
				else if (!string.IsNullOrEmpty(text))
				{
					text3 = string.Format("{0}, {1}", text, text3);
				}
				AuthModulePerformanceCounterHelper.percentageCookieHitLastMinute.AddDenominator(1L);
				AuthModulePerformanceCounterHelper.percentageLogonCacheHitLastMinute.AddDenominator(1L);
				if (LiveIdBasicAuthModule.enableClientSideLogonCache)
				{
					AuthModulePerformanceCounterHelper.counters.LogonCacheSize.RawValue = (long)LiveIdBasicAuthModule.logonCache.ValidCredsCount;
				}
				this.userKey = LiveIdBasicAuthModule.GetLogonKey(this.memberName, LiveIdBasicAuthModule.applicationName, text3, this.organizationContext);
				bool flag = false;
				string text4 = null;
				if (LiveIdBasicAuthModule.enableClientSideLogonCache && LiveIdBasicAuthModule.preferClientSideLogonCacheToCookie && LiveIdBasicAuthModule.syncADBackEndOnly)
				{
					flag = this.CheckLogonCache(context, out text4);
				}
				bool flag2 = false;
				if (!flag)
				{
					if (LiveIdBasicAuthModule.allowCookieAuth && LiveIdBasicAuthModule.cafeProxy && LiveIdBasicAuthModule.syncADBackEndOnly)
					{
						this.receivedAuthCookie = LiveIdBasicAuthModule.ParseAuthCookie(context, ref this.authInfo, out this.logOfParsingAuthCookie);
						if (this.receivedAuthCookie)
						{
							AuthModulePerformanceCounterHelper.counters.CookieBasedAuthRequests.Increment();
							if (this.authInfo.isExpired)
							{
								AuthModulePerformanceCounterHelper.counters.ExpiredCookieAuthRequests.Increment();
							}
						}
					}
					if (this.authInfo.isValidCookie && !string.Equals(this.authInfo.key, this.userKey, StringComparison.OrdinalIgnoreCase))
					{
						this.authInfo.isValidCookie = false;
						this.logOfParsingAuthCookie += string.Format("cookie cannot be replayed for different logon key {0} and {1}", this.authInfo.key, this.userKey);
						AuthModulePerformanceCounterHelper.counters.FailedCookieAuthRequests.Increment();
					}
					if (!this.authInfo.isValidCookie && this.authInfo.password == null)
					{
						if (this.receivedAuthCookie)
						{
							this.application.Response.Cookies.Set(this.GetEmptyCookie());
							string text5 = string.Format("&{0}=<CompactTokenSupplied><InvalidCookieSupplied>{1}", "LiveIdBasicLog", this.logOfParsingAuthCookie);
							LiveIdBasicAuthModule.UpdateHttpContextItem(this.application.Context, "LiveIdBasicLog", text5);
							context.Response.AppendToLog(text5);
						}
						this.asyncOp.InvokeCallback();
						this.TraceExitFunction("ContinueOnAuthenticate");
						return;
					}
				}
				if (!flag && !this.authInfo.isValidCookie && LiveIdBasicAuthModule.enableClientSideLogonCache && LiveIdBasicAuthModule.syncADBackEndOnly)
				{
					flag = this.CheckLogonCache(context, out text4);
				}
				if (flag || (LiveIdBasicAuthModule.allowCookieAuth && this.authInfo.isValidCookie && this.authInfo.puid != null && LiveIdBasicAuthModule.syncADBackEndOnly))
				{
					GenericIdentity identity = new GenericIdentity(this.memberName, "LiveIdBasic");
					this.application.Context.User = new GenericPrincipal(identity, null);
					LiveIdBasicTokenAccessor liveIdBasicTokenAccessor = LiveIdBasicTokenAccessor.Create(this.authInfo.puid, this.memberName);
					CommonAccessToken token = liveIdBasicTokenAccessor.GetToken();
					if (LiveIdBasicAuthModule.mailboxProxy)
					{
						token.ExtensionData["UserType"] = this.authInfo.userType.ToString();
					}
					if (this.authInfo.ticket != null)
					{
						token.ExtensionData["CompactTicket"] = this.authInfo.ticket;
					}
					this.UpdateCommonAccessTokenAndHeaders(this.memberName, token);
					this.UpdateAccountValidationContextInHeader(token);
					StringBuilder stringBuilder = new StringBuilder();
					if (this.authInfo.userType == UserType.Federated)
					{
						stringBuilder.Append("<FEDERATED>");
					}
					stringBuilder.Append(string.Format("<UserType:{0}>", this.authInfo.userType));
					if (!flag)
					{
						stringBuilder.Append("<CompactTokenSupplied>");
						if (flag2)
						{
							stringBuilder.Append("AuthenticatedBy:OfflineOrgId.");
						}
					}
					else if (this.receivedAuthCookie && !LiveIdBasicAuthModule.preferClientSideLogonCacheToCookie)
					{
						stringBuilder.Append(string.Format("<CompactTokenSupplied><InvalidCookieSupplied>{0}.<LogonCache>{1}", this.logOfParsingAuthCookie, text4));
					}
					else if (this.authInfo.password == null || this.authInfo.password.Length == 0)
					{
						stringBuilder.Append("<CompactTokenSupplied><LogonCache>" + text4);
					}
					else
					{
						stringBuilder.Append("<LogonCache>" + text4);
					}
					LiveIdBasicAuthModule.UpdateHttpContextItem(this.application.Context, "LiveIdBasicLog", stringBuilder.ToString());
					this.application.Context.Response.AppendToLog(string.Format("&{0}={1}", "LiveIdBasicLog", stringBuilder.ToString()));
					this.FinishAuthentication(0, false, LiveIdAuthResult.Success);
					if (this.authInfo.isValidCookie)
					{
						AuthModulePerformanceCounterHelper.percentageCookieHitLastMinute.AddNumerator(1L);
						return;
					}
				}
				else
				{
					AuthModulePerformanceCounterHelper.counters.RemoteAuthRequests.Increment();
					LiveIdBasicAuthentication liveIdBasicAuthentication = new LiveIdBasicAuthentication();
					liveIdBasicAuthentication.ApplicationName = LiveIdBasicAuthModule.applicationName;
					liveIdBasicAuthentication.UserIpAddress = text3;
					liveIdBasicAuthentication.UserAgent = text2;
					liveIdBasicAuthentication.SyncAD = LiveIdBasicAuthModule.syncAD;
					liveIdBasicAuthentication.SyncADBackEndOnly = LiveIdBasicAuthModule.syncADBackEndOnly;
					liveIdBasicAuthentication.SyncUPN = LiveIdBasicAuthModule.syncUPN;
					liveIdBasicAuthentication.AllowOfflineOrgIdAsPrimeAuth = LiveIdBasicAuthModule.allowOfflineOrgIdAsPrimeAuth;
					if (LiveIdBasicAuthModule.cafeProxy)
					{
						liveIdBasicAuthentication.BeginGetCommonAccessToken(this.authInfo.username, this.authInfo.password, this.organizationContext, this.requestId, new AsyncCallback(this.ContinueOnAuthenticateCallback), liveIdBasicAuthentication);
						return;
					}
					liveIdBasicAuthentication.AllowLiveIDOnlyAuth = LiveIdBasicAuthModule.allowLiveIDOnlyAuth;
					liveIdBasicAuthentication.BeginGetWindowsIdentity(this.authInfo.username, this.authInfo.password, this.organizationContext, this.requestId, new AsyncCallback(this.ContinueOnAuthenticateCallback), liveIdBasicAuthentication);
				}
			}, (object exception) => true, ReportOptions.DoNotCollectDumps | ReportOptions.DeepStackTraceHash);
			this.TraceExitFunction("ContinueOnAuthenticate");
			return this.asyncOp;
		}

		private void ContinueOnAuthenticateCallback(IAsyncResult async)
		{
			LiveIdBasicAuthentication auth = (LiveIdBasicAuthentication)async.AsyncState;
			try
			{
				ExWatson.SendReportOnUnhandledException(delegate()
				{
					int tarpitDelay = 0;
					bool flag = false;
					LiveIdAuthResult liveIdAuthResult = LiveIdAuthResult.AuthFailure;
					bool flag2 = false;
					string text = null;
					WindowsIdentity windowsIdentity = null;
					CommonAccessToken commonAccessToken = null;
					try
					{
						if (LiveIdBasicAuthModule.cafeProxy)
						{
							string text2;
							liveIdAuthResult = auth.EndGetCommonAccessToken(async, out text2);
							if (!string.IsNullOrEmpty(text2))
							{
								commonAccessToken = CommonAccessToken.Deserialize(text2);
								commonAccessToken.ExtensionData.TryGetValue("CompactTicket", out text);
								this.authInfo.puid = commonAccessToken.ExtensionData["Puid"];
							}
						}
						else
						{
							liveIdAuthResult = auth.EndGetWindowsIdentity(async, out windowsIdentity);
						}
						LiveIdBasicAuthModule.UpdateHttpContextItem(this.application.Context, "LiveIdBasicAuthResult", liveIdAuthResult.ToString());
						this.authInfo.authenticatedByOfflineOrgId = auth.AuthenticatedByOfflineAuth;
						this.authInfo.OfflineOrgIdFailureResult = auth.OfflineOrgIdFailureResult;
						string text3 = (this.logMessage ?? "") + auth.LastRequestErrorMessage;
						string text4 = null;
						if (liveIdAuthResult != LiveIdAuthResult.Success || !string.IsNullOrEmpty(text3))
						{
							if (string.IsNullOrEmpty(text3))
							{
								text3 = liveIdAuthResult.ToString();
							}
							if (this.logOfParsingAuthCookie != null)
							{
								text3 = string.Format("<CompactTokenSupplied><InvalidCookieSupplied>{0}{1}", this.logOfParsingAuthCookie, text3);
							}
							LiveIdBasicAuthModule.UpdateHttpContextItem(this.application.Context, "LiveIdBasicLog", text3);
							this.application.Context.Response.AppendToLog(string.Format("&{0}={1}", "LiveIdBasicLog", text3));
							Match match = LiveIdBasicAuthModule.userTypeRegex.Match(text3);
							if (match.Success)
							{
								Enum.TryParse<UserType>(match.Groups[LiveIdBasicAuthModule.userTypeGroupName].Value, out this.authInfo.userType);
							}
							Match match2 = LiveIdBasicAuthModule.credExpiresRegex.Match(text3);
							if (match2.Success)
							{
								this.authInfo.passwordExpirationHint = match2.Groups[1].ToString();
							}
							Match match3 = LiveIdBasicAuthModule.recoveryUrlRegex.Match(text3);
							if (match3.Success)
							{
								text4 = match3.Groups[1].ToString();
								this.application.Response.Headers.Add("X-MS-Credential-Service-Url", text4);
							}
							Match match4 = LiveIdBasicAuthModule.appPasswordRegex.Match(text3);
							if (match4.Success)
							{
								this.authInfo.isAppPassword = true;
							}
						}
						if (liveIdAuthResult == LiveIdAuthResult.Success && ((LiveIdBasicAuthModule.cafeProxy && commonAccessToken != null) || (!LiveIdBasicAuthModule.cafeProxy && windowsIdentity != null)))
						{
							string text5 = string.Empty;
							ExDateTime exDateTime = ExDateTime.UtcNow;
							string s;
							ExDateTime exDateTime2;
							if (commonAccessToken.ExtensionData.TryGetValue("CreateTime", out s) && ExDateTime.TryParse(s, out exDateTime2))
							{
								exDateTime = exDateTime2;
							}
							if (LiveIdBasicAuthModule.allowCookieAuth)
							{
								if (!string.IsNullOrEmpty(text))
								{
									text5 = HttpUtility.UrlEncode(text);
								}
								string text6 = HttpUtility.UrlEncode(this.userKey);
								string text7 = auth.AuthenticatedByOfflineAuth ? (exDateTime + TimeSpan.FromMinutes((double)LiveIdBasicAuthModule.OfflineOrgIdCookieExpirationTimeInMin)).ToBinary().ToString() : (exDateTime + TimeSpan.FromSeconds((double)LiveIdSTSBase.RpsTicketLifetime)).ToBinary().ToString();
								string arg;
								string signedHashFromCookieItem = CookieSignHelper.GetSignedHashFromCookieItem(text5, text6, this.authInfo.puid, text7, auth.AuthenticatedByOfflineAuth, out arg);
								if (signedHashFromCookieItem != null)
								{
									HttpCookie emptyCookie = this.GetEmptyCookie();
									emptyCookie["compactTicket"] = text5;
									emptyCookie["key"] = text6;
									emptyCookie["signature"] = signedHashFromCookieItem;
									emptyCookie["puid"] = this.authInfo.puid;
									emptyCookie["expireTime"] = text7;
									emptyCookie["membername"] = HttpUtility.UrlEncode(this.memberName);
									emptyCookie["flags"] = auth.AuthenticatedByOfflineAuth.ToString();
									emptyCookie["userType"] = this.authInfo.userType.ToString();
									if (this.authInfo.isAppPassword)
									{
										emptyCookie["appPassword"] = this.authInfo.isAppPassword.ToString();
									}
									if (!string.IsNullOrEmpty(this.authInfo.passwordExpirationHint))
									{
										emptyCookie["credentialExpirationHint"] = HttpUtility.UrlEncode(this.authInfo.passwordExpirationHint);
									}
									emptyCookie.Expires = DateTime.UtcNow.AddMinutes((double)LiveIdBasicAuthModule.BasicAuthCookieLifetime);
									this.application.Response.Cookies.Set(emptyCookie);
									IDictionary items;
									object key;
									(items = this.application.Context.Items)[key = "LiveIdBasicLog"] = items[key] + arg;
								}
								else
								{
									IDictionary items2;
									object key2;
									(items2 = this.application.Context.Items)[key2 = "LiveIdBasicLog"] = items2[key2] + arg;
								}
							}
							if (LiveIdBasicAuthModule.enableClientSideLogonCache && LiveIdBasicAuthModule.syncADBackEndOnly)
							{
								string tag = auth.AuthenticatedByOfflineAuth ? "AuthenticatedBy:OfflineOrgId." : string.Empty;
								int lifeTimeInMinutes = auth.AuthenticatedByOfflineAuth ? LiveIdBasicAuthModule.OfflineOrgIdCookieExpirationTimeInMin : LiveIdBasicAuthModule.logonCacheLifetime;
								LiveIdBasicAuthModule.logonCache.Add(this.userKey, exDateTime, this.authInfo.puid, HashExtension.GetPasswordHash(this.authInfo.password), lifeTimeInMinutes, tag, this.GetHashCode(), this.authInfo.passwordExpirationHint, this.authInfo.userType, this.authInfo.isAppPassword, false);
								AuthModulePerformanceCounterHelper.counters.LogonCacheSize.RawValue = (long)LiveIdBasicAuthModule.logonCache.ValidCredsCount;
							}
						}
						if (liveIdAuthResult == LiveIdAuthResult.UserNotFoundInAD)
						{
							ExTraceGlobals.AuthenticationTracer.Information<string>((long)this.GetHashCode(), "user not found failure for user '{0}'", this.memberName);
							if (!SmtpAddress.IsValidSmtpAddress(this.memberName))
							{
								return;
							}
							this.application.Context.Response.StatusCode = LiveIdBasicAuthModule.userNotFoundStatus;
							flag2 = true;
						}
						else if (LiveIdBasicAuthModule.cafeProxy && commonAccessToken != null)
						{
							if (LiveIdBasicAuthModule.syncADBackEndOnly || LiveIdBasicAuthModule.mailboxProxy)
							{
								GenericIdentity identity = new GenericIdentity(this.memberName, "LiveIdBasic");
								this.application.Context.User = new GenericPrincipal(identity, null);
							}
							else
							{
								GenericSidIdentity identity2 = new GenericSidIdentity(this.memberName, "LiveIdBasic", new SecurityIdentifier(commonAccessToken.ExtensionData["UserSid"]), commonAccessToken.ExtensionData["Partition"]);
								this.application.Context.User = new GenericPrincipal(identity2, null);
							}
							this.UpdateCommonAccessTokenAndHeaders(this.memberName, commonAccessToken);
							this.UpdateAccountValidationContextInHeader(commonAccessToken);
						}
						else if (!LiveIdBasicAuthModule.cafeProxy && windowsIdentity != null)
						{
							try
							{
								this.application.Context.User = new WindowsPrincipal(windowsIdentity);
							}
							catch (SystemException ex2)
							{
								ExTraceGlobals.AuthenticationTracer.TraceError<string, SystemException>((long)this.GetHashCode(), "Exception thrown setting HTTPContext.User for user '{0}'.  Exception {1}", this.memberName, ex2);
								LiveIdBasicAuthModule.eventLogger.LogEvent(SecurityEventLogConstants.Tuple_GeneralException, this.memberName, new object[]
								{
									this.memberName,
									ex2.ToString()
								});
								windowsIdentity.Dispose();
								return;
							}
							if (LiveIdBasicAuthModule.disposeIdentities)
							{
								this.lastUser = windowsIdentity;
							}
						}
						else if (liveIdAuthResult == LiveIdAuthResult.ExpiredCreds)
						{
							ExTraceGlobals.AuthenticationTracer.Information<string>((long)this.GetHashCode(), "expired credentials failure for user '{0}'", this.memberName);
							this.application.Response.Headers.Add("X-MS-Credential-Service-CredExpired", "true");
							this.application.Response.Headers.Add("X-MS-Credential-Service-Federated", (this.authInfo.userType == UserType.Federated) ? "true" : "false");
							if (string.IsNullOrEmpty(text4))
							{
								this.application.Response.Headers.Add("X-MS-Credential-Service-Url", this.GenerateRecoveryUrl(text4, this.memberName));
							}
							if (LiveIdBasicAuthModule.passwordExpiredStatus != 401)
							{
								this.application.Context.Response.StatusCode = LiveIdBasicAuthModule.passwordExpiredStatus;
								this.application.Context.Response.StatusDescription = this.GenerateRecoveryUrl(text4, this.memberName);
								flag2 = true;
							}
						}
						else if (liveIdAuthResult == LiveIdAuthResult.FederatedStsUnreachable || liveIdAuthResult == LiveIdAuthResult.LiveServerUnreachable || liveIdAuthResult == LiveIdAuthResult.CommunicationFailure || liveIdAuthResult == LiveIdAuthResult.HRDFailure || liveIdAuthResult == LiveIdAuthResult.FaultException || liveIdAuthResult == LiveIdAuthResult.FederatedStsUrlNotEncrypted)
						{
							if (LiveIdBasicAuthModule.translateWCFException)
							{
								foreach (string value in LiveIdBasicAuthModule.userAgentsToBackOff)
								{
									if (!string.IsNullOrEmpty(value) && auth.UserAgent != null && auth.UserAgent.IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0)
									{
										flag = true;
										break;
									}
								}
							}
							if (flag)
							{
								ExTraceGlobals.AuthenticationTracer.Information<string, LiveIdAuthResult>((long)this.GetHashCode(), "InternalError: {1}. Needs to back off client: '{0}'", this.memberName, liveIdAuthResult);
								this.application.Context.Response.StatusCode = 500;
								this.application.Context.Response.Write(LiveIdBasicAuthModule.BackOffXml);
							}
							else
							{
								ExTraceGlobals.AuthenticationTracer.Information<string>((long)this.GetHashCode(), "Cannot reach IDP failure for user '{0}'", this.memberName);
								this.application.Context.Response.StatusCode = LiveIdBasicAuthModule.idpFailureStatus;
								flag2 = true;
							}
						}
						else if (liveIdAuthResult == LiveIdAuthResult.OperationTimedOut)
						{
							ExTraceGlobals.AuthenticationTracer.Information<string>((long)this.GetHashCode(), "Operation timed out for user '{0}'", this.memberName);
							this.application.Context.Response.StatusCode = LiveIdBasicAuthModule.wcfTimeoutStatus;
							if (!string.IsNullOrEmpty(LiveIdBasicAuthModule.wcfTimeoutText))
							{
								this.application.Context.Response.StatusDescription = LiveIdBasicAuthModule.wcfTimeoutText;
							}
							flag2 = true;
						}
						else if (liveIdAuthResult == LiveIdAuthResult.FederatedStsADFSRulesDenied)
						{
							ExTraceGlobals.AuthenticationTracer.Information<string>((long)this.GetHashCode(), "ADFS rules denied for user '{0}'", this.memberName);
							this.application.Context.Response.StatusCode = LiveIdBasicAuthModule.adfsRulesDeniedStatus;
							flag2 = true;
						}
						else if (liveIdAuthResult == LiveIdAuthResult.Forbidden)
						{
							ExTraceGlobals.AuthenticationTracer.Information<string>((long)this.GetHashCode(), "ADFS denied for user '{0}'", this.memberName);
							this.application.Context.Response.StatusCode = LiveIdBasicAuthModule.adfsRulesDeniedStatus;
							flag2 = true;
						}
						else if (liveIdAuthResult == LiveIdAuthResult.UnfamiliarLocation)
						{
							ExTraceGlobals.AuthenticationTracer.Information<string>((long)this.GetHashCode(), "Unfamiliar location for user '{0}'", this.memberName);
							this.application.Context.Response.StatusCode = LiveIdBasicAuthModule.unfamiliarLocationStatus;
							flag2 = true;
						}
						else if (liveIdAuthResult == LiveIdAuthResult.AccountNotProvisioned)
						{
							ExTraceGlobals.AuthenticationTracer.Information<string>((long)this.GetHashCode(), "The account has not been provisioned for user '{0}'", this.memberName);
							this.application.Context.Response.StatusCode = LiveIdBasicAuthModule.accountNotProvisionedStatus;
							flag2 = true;
						}
						else if (liveIdAuthResult == LiveIdAuthResult.InternalServerError)
						{
							ExTraceGlobals.AuthenticationTracer.Information<string>((long)this.GetHashCode(), "Internal server error occured for user '{0}'", this.memberName);
							this.application.Context.Response.StatusCode = LiveIdBasicAuthModule.internalServerErrorStatus;
							flag2 = true;
						}
						else if (liveIdAuthResult == LiveIdAuthResult.AuthFailure || liveIdAuthResult == LiveIdAuthResult.AppPasswordRequired)
						{
							if (!auth.RecoverableLogonFailure || LiveIdBasicAuthModule.recoverableErrorStatus == 401)
							{
								ExTraceGlobals.AuthenticationTracer.Information<string>((long)this.GetHashCode(), "logon failure for user '{0}'", this.memberName);
							}
							else
							{
								ExTraceGlobals.AuthenticationTracer.Information<string>((long)this.GetHashCode(), "recoverable logon failure for user '{0}'", this.memberName);
								this.application.Context.Response.StatusCode = LiveIdBasicAuthModule.recoverableErrorStatus;
								this.application.Context.Response.StatusDescription = this.GenerateRecoveryUrl(text4, this.memberName);
								flag2 = true;
							}
						}
						if (auth.Tarpit || flag)
						{
							if (liveIdAuthResult == LiveIdAuthResult.FederatedStsUnreachable || liveIdAuthResult == LiveIdAuthResult.LiveServerUnreachable || liveIdAuthResult == LiveIdAuthResult.CommunicationFailure || liveIdAuthResult == LiveIdAuthResult.HRDFailure || liveIdAuthResult == LiveIdAuthResult.OperationTimedOut)
							{
								tarpitDelay = LiveIdBasicAuthModule.tarpitIdpFailure;
							}
							else if (liveIdAuthResult == LiveIdAuthResult.ExpiredCreds)
							{
								tarpitDelay = LiveIdBasicAuthModule.tarpitPasswordExpired;
							}
							else if (auth.RecoverableLogonFailure)
							{
								tarpitDelay = LiveIdBasicAuthModule.tarpitRecoverableError;
							}
							else
							{
								tarpitDelay = LiveIdBasicAuthModule.tarpitAccessDenied;
							}
						}
						if (this.application.Context.Response.StatusCode == 503 && LiveIdBasicAuthModule.retryAfterSec > 0)
						{
							this.application.Context.Response.Headers.Add("Retry-After", LiveIdBasicAuthModule.retryAfterSec.ToString());
						}
					}
					finally
					{
						this.FinishAuthentication(tarpitDelay, flag || flag2, liveIdAuthResult);
					}
				}, delegate(object e)
				{
					ExTraceGlobals.AuthenticationTracer.TraceError<string, object>((long)this.GetHashCode(), "Unexpected exception thrown in ContinueOnAuthenticateCallback() for user '{0}'.  Exception {1}", this.memberName, e);
					LiveIdBasicAuthModule.eventLogger.LogEvent(SecurityEventLogConstants.Tuple_GeneralException, this.memberName, new object[]
					{
						this.memberName,
						e.ToString()
					});
					return true;
				}, ReportOptions.DoNotCollectDumps | ReportOptions.DeepStackTraceHash);
			}
			catch (Exception ex)
			{
				this.lastException = ex;
			}
		}

		private void DelayedResponseCallback(object state)
		{
			ExWatson.SendReportOnUnhandledException(delegate()
			{
				if (this.timer != null)
				{
					this.timer.Dispose();
					this.timer = null;
				}
				this.asyncOp.InvokeCallback();
			}, delegate(object e)
			{
				ExTraceGlobals.AuthenticationTracer.TraceError<string, object>((long)this.GetHashCode(), "Unexpected exception thrown in ContinueOnAuthenticateCallback() for user '{0}'.  Exception {1}", this.memberName, e);
				LiveIdBasicAuthModule.eventLogger.LogEvent(SecurityEventLogConstants.Tuple_GeneralException, this.memberName, new object[]
				{
					this.memberName,
					e.ToString()
				});
				return true;
			}, ReportOptions.DoNotCollectDumps | ReportOptions.DeepStackTraceHash);
		}

		private void EndOnAuthenticate(IAsyncResult asyncResult)
		{
			this.TraceEnterFunction("EndOnAuthenticate");
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			LazyAsyncResult lazyAsyncResult = (LazyAsyncResult)asyncResult;
			if (!lazyAsyncResult.IsCompleted)
			{
				lazyAsyncResult.InternalWaitForCompletion();
			}
			if (!this.continueRequest && this.authInfo.password != null)
			{
				Array.Clear(this.authInfo.password, 0, this.authInfo.password.Length);
			}
			this.asyncOp = null;
			if (this.lastException != null)
			{
				throw this.lastException;
			}
			this.TraceExitFunction("EndOnAuthenticate");
		}

		private void OnPreSendRequestHeaders(object source, EventArgs eventArgs)
		{
			this.TraceEnterFunction("OnPreSendRequestHeaders");
			this.AppendBasicChallenge((HttpApplication)source);
			this.TraceExitFunction("OnPreSendRequestHeaders");
		}

		private void OnEndRequest(object source, EventArgs eventArgs)
		{
			this.TraceEnterFunction("OnEndRequest");
			if (!AuthCommon.IsFrontEnd)
			{
				this.AppendBasicChallenge((HttpApplication)source);
			}
			this.asyncOp = null;
			this.receivedAuthCookie = false;
			this.userKey = null;
			this.authInfo = null;
			this.logOfParsingAuthCookie = null;
			this.TraceExitFunction("OnEndRequest");
		}

		private void AppendBasicChallenge(HttpApplication application)
		{
			if (application.Response.StatusCode == 401)
			{
				application.Response.AppendHeader("WWW-Authenticate", "Basic Realm=\"\"");
			}
		}

		private string GenerateRecoveryUrl(string recoveryUrl, string memberName)
		{
			if (string.IsNullOrEmpty(recoveryUrl))
			{
				try
				{
					recoveryUrl = LiveIdBasicAuthModule.FrontEndOWAUrl;
				}
				catch (Exception ex)
				{
					LiveIdBasicAuthModule.eventLogger.LogEvent(SecurityEventLogConstants.Tuple_GeneralException, this.memberName, new object[]
					{
						this.memberName,
						ex.ToString()
					});
					recoveryUrl = this.application.Context.Request.Url.GetLeftPart(UriPartial.Authority) + "/owa";
				}
				if (SmtpAddress.IsValidSmtpAddress(memberName))
				{
					recoveryUrl = recoveryUrl + "/" + new SmtpAddress(memberName).Domain;
				}
			}
			return recoveryUrl;
		}

		private void TraceEnterFunction(string functionName)
		{
			ExTraceGlobals.AuthenticationTracer.TraceFunction<string>((long)this.GetHashCode(), "Enter Function: LiveIdBasicAuthModule.{0}.", functionName);
		}

		private void TraceExitFunction(string functionName)
		{
			ExTraceGlobals.AuthenticationTracer.TraceFunction<string>((long)this.GetHashCode(), "Exit Function: LiveIdBasicAuthModule.{0}.", functionName);
		}

		private HttpCookie GetEmptyCookie()
		{
			HttpCookie httpCookie = new HttpCookie(LiveIdBasicAuthModule.BasicAuthCookieName);
			string filePath = this.application.Request.FilePath;
			int num = filePath.LastIndexOf('/');
			if (num > 0)
			{
				httpCookie.Path = filePath.Substring(0, num);
			}
			else
			{
				httpCookie.Path = filePath;
			}
			httpCookie.Secure = true;
			httpCookie.HttpOnly = true;
			httpCookie.Expires = DateTime.UtcNow.AddYears(-30);
			return httpCookie;
		}

		private void FinishAuthentication(int tarpitDelay, bool completeRequest, LiveIdAuthResult result)
		{
			if (this.authInfo.password != null)
			{
				Array.Clear(this.authInfo.password, 0, this.authInfo.password.Length);
			}
			LiveIdBasicAuthModule.UpdateHttpContextItem(this.application.Context, "AuthType", "LiveIdBasic");
			LiveIdBasicAuthModule.UpdateHttpContextItem(this.application.Context, "AuthModuleLatency", (DateTime.UtcNow - this.startTime).TotalMilliseconds);
			if (this.authInfo.OfflineOrgIdFailureResult != null && this.authInfo.authenticatedByOfflineOrgId)
			{
				this.application.Context.Response.AppendToLog(string.Format("&{0}={1};&{2}={3}", new object[]
				{
					"LiveIdBasicAuthResult",
					this.authInfo.OfflineOrgIdFailureResult.Value,
					"LiveIdBasicAuthResultLatency",
					(DateTime.UtcNow - this.startTime).TotalMilliseconds + (double)tarpitDelay
				}));
			}
			else
			{
				this.application.Context.Response.AppendToLog(string.Format("&{0}={1};&{2}={3}", new object[]
				{
					"LiveIdBasicAuthResult",
					result,
					"LiveIdBasicAuthResultLatency",
					(DateTime.UtcNow - this.startTime).TotalMilliseconds + (double)tarpitDelay
				}));
			}
			if (result == LiveIdAuthResult.Success)
			{
				this.application.Context.Response.AppendToLog(string.Format("&Puid={0};", this.authInfo.puid));
			}
			if (completeRequest)
			{
				this.application.CompleteRequest();
			}
			if (tarpitDelay == 0)
			{
				this.asyncOp.InvokeCallback();
				return;
			}
			ExTraceGlobals.AuthenticationTracer.Information<byte[], int>((long)this.GetHashCode(), "Tar pitting user '{0}' for '{1}'ms", this.authInfo.username, tarpitDelay);
			this.timer = new Timer(new TimerCallback(this.DelayedResponseCallback), null, TimeSpan.FromMilliseconds((double)tarpitDelay), TimeSpan.Zero);
		}

		private void UpdateCommonAccessTokenAndHeaders(string memberName, CommonAccessToken cat)
		{
			if (!string.IsNullOrEmpty(this.authInfo.passwordExpirationHint))
			{
				this.application.Response.Headers.Add("X-MS-Credentials-Expire", this.authInfo.passwordExpirationHint);
				cat.ExtensionData["PasswordExpiry"] = this.authInfo.passwordExpirationHint;
			}
			if (this.authInfo.userType == UserType.Federated)
			{
				this.application.Response.Headers.Add("X-MS-Credential-Service-Federated", "true");
			}
			if (this.authInfo.isAppPassword)
			{
				cat.ExtensionData["AppPasswordUsed"] = "1";
			}
			if (LiveIdBasicAuthModule.mailboxProxy && ConfigBase<AdDriverConfigSchema>.GetConfig<bool>("AccountValidationEnabled") && !string.IsNullOrEmpty(LiveIdBasicAuthModule.applicationName))
			{
				cat.ExtensionData["AppName"] = LiveIdBasicAuthModule.applicationName;
			}
			if (this.compositeIdentityHelper != null)
			{
				this.compositeIdentityHelper.SetMsaUserHintCookie(this.application.Context, cat, memberName);
				this.compositeIdentityHelper.FixLiveIdBasicIdentity(this.application.Context, cat);
				cat = this.compositeIdentityHelper.GetCompositeIdentityCat(cat);
			}
			LiveIdBasicAuthModule.UpdateHttpContextItem(this.application.Context, "Item-CommonAccessToken", cat);
		}

		private void UpdateAccountValidationContextInHeader(CommonAccessToken cat)
		{
			if (ConfigBase<AdDriverConfigSchema>.GetConfig<bool>("AccountValidationEnabled"))
			{
				LiveIdBasicTokenAccessor liveIdBasicTokenAccessor = LiveIdBasicTokenAccessor.Attach(cat);
				ExDateTime utcNow;
				if (!cat.ExtensionData.ContainsKey("CreateTime") || !ExDateTime.TryParse(cat.ExtensionData["CreateTime"], out utcNow))
				{
					utcNow = ExDateTime.UtcNow;
				}
				IAccountValidationContext value = new AccountValidationContextByPUID(liveIdBasicTokenAccessor.Puid, utcNow, LiveIdBasicAuthModule.applicationName);
				LiveIdBasicAuthModule.UpdateHttpContextItem(this.application.Context, "AccountValidationContext", value);
			}
		}

		internal static bool ParseAuthCookie(HttpContext context, ref LiveIdBasicAuthInfo authInfo, out string errorMsg)
		{
			HttpCookie httpCookie = context.Request.Cookies[LiveIdBasicAuthModule.BasicAuthCookieName];
			errorMsg = null;
			if (httpCookie != null)
			{
				string text = httpCookie["compactTicket"];
				string text2 = httpCookie["key"];
				if (httpCookie["userType"] != null)
				{
					Enum.TryParse<UserType>(httpCookie["userType"], out authInfo.userType);
				}
				authInfo.passwordExpirationHint = httpCookie["credentialExpirationHint"];
				if (authInfo.passwordExpirationHint != null)
				{
					authInfo.passwordExpirationHint = HttpUtility.UrlDecode(authInfo.passwordExpirationHint);
				}
				if (httpCookie["appPassword"] != null)
				{
					bool.TryParse(httpCookie["appPassword"], out authInfo.isAppPassword);
				}
				string text3 = httpCookie["signature"];
				authInfo.puid = httpCookie["puid"];
				string text4 = httpCookie["expireTime"];
				if (httpCookie["flags"] != null)
				{
					bool.TryParse(httpCookie["flags"], out authInfo.authenticatedByOfflineOrgId);
				}
				if (!string.IsNullOrEmpty(authInfo.puid) && !string.IsNullOrEmpty(text3) && !string.IsNullOrEmpty(text2) && !string.IsNullOrEmpty(text4))
				{
					if (CookieSignHelper.VerifySignedHash(text, text2, authInfo.puid, text4, authInfo.authenticatedByOfflineOrgId, text3, out errorMsg))
					{
						try
						{
							DateTime dateTime = DateTime.FromBinary(long.Parse(text4));
							if (dateTime >= DateTime.UtcNow)
							{
								if (!string.IsNullOrEmpty(text))
								{
									authInfo.ticket = HttpUtility.UrlDecode(text);
									if (authInfo.ticket == null)
									{
										errorMsg += "NullTicket.";
									}
								}
								authInfo.key = HttpUtility.UrlDecode(text2);
								authInfo.isValidCookie = true;
								authInfo.isExpired = false;
							}
							else
							{
								errorMsg += string.Format("ticket expired ExpirationTime={0}.", dateTime);
								authInfo.puid = null;
							}
						}
						catch (FormatException ex)
						{
							errorMsg += ex.ToString();
							authInfo.ticket = null;
							authInfo.key = null;
							authInfo.puid = null;
						}
					}
					return true;
				}
			}
			return httpCookie != null;
		}

		internal static string GetCompactTokenFromCookie(HttpContext context)
		{
			HttpCookie httpCookie = context.Request.Cookies[LiveIdBasicAuthModule.BasicAuthCookieName];
			if (httpCookie != null)
			{
				return httpCookie["compactTicket"];
			}
			return null;
		}

		internal static bool ParseCredentials(HttpContext context, string authHeader, bool parsePassword, out byte[] username, out byte[] password)
		{
			username = null;
			password = null;
			if (string.IsNullOrEmpty(authHeader))
			{
				return false;
			}
			if (authHeader.Length <= 6)
			{
				return false;
			}
			if (!authHeader.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}
			string s = authHeader.Substring(6, authHeader.Length - 6).Trim();
			byte[] array = null;
			try
			{
				try
				{
					array = Convert.FromBase64String(s);
				}
				catch (FormatException)
				{
					LiveIdBasicAuthModule.UpdateLiveIdBasicError(context, "cannot decode Base64 auth header");
					context.Response.AppendToLog("&LiveIdBasicError=cannot decode Base64 auth header");
					return false;
				}
				int num = Array.IndexOf<byte>(array, 58);
				if (num <= 0)
				{
					LiveIdBasicAuthModule.UpdateLiveIdBasicError(context, "missing colon separator");
					context.Response.AppendToLog("&LiveIdBasicError=missing colon separator");
					return false;
				}
				int num2 = Array.IndexOf<byte>(array, 92, 0, num);
				num2++;
				username = new byte[num - num2];
				Array.Copy(array, num2, username, 0, username.Length);
				if (parsePassword)
				{
					num++;
					int num3 = array.Length - num;
					password = new byte[num3];
					Array.Copy(array, num, password, 0, num3);
				}
			}
			finally
			{
				if (array != null)
				{
					Array.Clear(array, 0, array.Length);
				}
			}
			return true;
		}

		internal static void UpdateLiveIdBasicError(HttpContext context, string errorMessage)
		{
			LiveIdBasicAuthModule.UpdateHttpContextItem(context, "LiveIdBasicError", errorMessage);
		}

		internal static void UpdateHttpContextItem(HttpContext context, string key, object value)
		{
			if (context != null)
			{
				context.Items[key] = value;
			}
		}

		internal static bool TryParseOrganizationContext(Uri uri, out string organizationContext)
		{
			organizationContext = string.Empty;
			if (uri != null)
			{
				organizationContext = LiveIdBasicAuthModule.GetNameValueCollectionFromUri(uri).Get("organizationcontext");
				if (!string.IsNullOrEmpty(organizationContext) && !SmtpAddress.IsValidDomain(organizationContext))
				{
					return false;
				}
			}
			return true;
		}

		internal static NameValueCollection GetNameValueCollectionFromUri(Uri uri)
		{
			return HttpUtility.ParseQueryString(uri.Query.Replace(';', '&'));
		}

		private static string GetLogonKey(string membername, string endpoint, string hostAddress, string organization)
		{
			return string.Format("{0} {1} {2} {3}", new object[]
			{
				membername,
				endpoint,
				hostAddress,
				organization
			}).ToLowerInvariant();
		}

		private static bool IsHotmailAddress(string userName)
		{
			if (userName.Contains("@outlook."))
			{
				foreach (string value in LiveIdBasicAuthModule.hotmailDomains)
				{
					if (userName.EndsWith(value, StringComparison.OrdinalIgnoreCase))
					{
						return true;
					}
				}
			}
			return false;
		}

		private bool CheckLogonCache(HttpContext context, out string tag)
		{
			tag = null;
			if (this.authInfo.password != null && this.authInfo.password.Length > 0)
			{
				this.authInfo.passwordHash = HashExtension.GetPasswordHash(this.authInfo.password);
			}
			if (this.authInfo.passwordHash != null)
			{
				PuidCredInfo puidCredInfo;
				bool flag = LiveIdBasicAuthModule.logonCache.TryGetEntry(this.userKey, this.authInfo.passwordHash, out puidCredInfo);
				if (flag)
				{
					this.authInfo.puid = puidCredInfo.PUID;
					this.authInfo.passwordExpirationHint = puidCredInfo.passwordExpiry;
					this.authInfo.userType = puidCredInfo.userType;
					this.authInfo.isAppPassword = puidCredInfo.appPassword;
					tag = puidCredInfo.Tag;
					AuthModulePerformanceCounterHelper.percentageLogonCacheHitLastMinute.AddNumerator(1L);
					AuthModulePerformanceCounterHelper.counters.NumberOfCachedRequests.Increment();
					return true;
				}
			}
			return false;
		}

		internal const string URIPropertyOrganization = "organizationcontext";

		private const string HotmailAddressDomainPrefix = "@outlook.";

		private const string wlidBasicAuthRef = "WLID-BasicAuthModule";

		private const string cookieKeyField = "key";

		private const string cookieMemberField = "membername";

		private const string cookieTicketField = "compactTicket";

		private const string cookieUserType = "userType";

		private const string cookiePuid = "puid";

		private const string cookieCredentialNearExpirationHint = "credentialExpirationHint";

		private const string cookieSignature = "signature";

		private const string cookieExpireTime = "expireTime";

		private const string cookieAuthFlags = "flags";

		private const string cookieAppPassword = "appPassword";

		public const string LiveIdBasicAuthResultKey = "LiveIdBasicAuthResult";

		public const string LiveIdBasicLogKey = "LiveIdBasicLog";

		public const string LiveIdBasicAuthResultLatencyKey = "LiveIdBasicAuthResultLatency";

		private WindowsIdentity lastUser;

		private Exception lastException;

		private HttpApplication application;

		private LazyAsyncResult asyncOp;

		private bool continueRequest;

		private Timer timer;

		private DateTime startTime;

		private bool receivedAuthCookie;

		private string memberName;

		private string organizationContext;

		private string userKey;

		private Guid requestId;

		private string logMessage;

		private string logOfParsingAuthCookie;

		private LiveIdBasicAuthInfo authInfo;

		private static readonly ExEventLog eventLogger = new ExEventLog(ExTraceGlobals.AuthenticationTracer.Category, LiveIdBasicAuthentication.LiveIdComponent);

		private static string BasicAuthCookieName = "EXOBasicAuth";

		private static int BasicAuthCookieLifetime = 480;

		private static bool syncADBackEndOnly = false;

		private static bool syncAD = false;

		private static bool syncUPN = false;

		private static bool cafeProxy = true;

		private static bool mailboxProxy = false;

		private static bool isCompositeIdentityHandlingEnabled = false;

		private static bool backendDoesAuth = false;

		private static bool disposeIdentities = true;

		private static bool failRedirects = false;

		private static bool allowLiveIDOnlyAuth = false;

		private static bool allowHotmailRedirect = false;

		private static bool allowCookieAuth = false;

		private static bool allowOfflineOrgIdAsPrimeAuth;

		private static string applicationName = null;

		private static string hotmailRedirectUrl = null;

		private static string[] hotmailDomains = new string[]
		{
			"com"
		};

		private static int tarpitAccessDenied = 5000;

		private static int tarpitIdpFailure = 5000;

		private static int tarpitRecoverableError = 5000;

		private static int tarpitPasswordExpired = 5000;

		private static int recoverableErrorStatus = 401;

		private static int passwordExpiredStatus = 401;

		private static int userNotFoundStatus = 403;

		private static int idpFailureStatus = 401;

		private static int wcfTimeoutStatus = 503;

		private static int adfsRulesDeniedStatus = 403;

		private static int unfamiliarLocationStatus = 403;

		private static int accountNotProvisionedStatus = 403;

		private static int internalServerErrorStatus = 500;

		private static string wcfTimeoutText = "Server Busy";

		private static int retryAfterSec = 30;

		private static bool translateWCFException = false;

		private static string[] userAgentsToBackOff = new string[]
		{
			"MacOutlook"
		};

		private static Regex recoveryUrlRegex = new Regex("<RECOVERY (.*?)>", RegexOptions.Compiled);

		private static Regex userTypeRegex = new Regex("<UserType:(?<userType>[^>]+)>", RegexOptions.Compiled);

		private static string userTypeGroupName = "userType";

		private static Regex credExpiresRegex = new Regex("<CREDEXPIRES (\\d+)>", RegexOptions.Compiled);

		private static Regex appPasswordRegex = new Regex("<APPPASSWORD>", RegexOptions.Compiled);

		private static readonly string BackOffXml = "<?xml version=\"1.0\" encoding=\"utf-8\"?><s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\"><s:Body><s:Fault><faultcode xmlns:a=\"http://schemas.microsoft.com/exchange/services/2006/types\">a:ErrorInternalServerTransientError</faultcode><faultstring xml:lang=\"en-US\">The server cannot service this request right now. Try again later.</faultstring><detail><e:ResponseCode xmlns:e=\"http://schemas.microsoft.com/exchange/services/2006/errors\">ErrorInternalServerTransientError</e:ResponseCode><e:Message xmlns:e=\"http://schemas.microsoft.com/exchange/services/2006/errors\">The server cannot service this request right now. Please wait and try again later.</e:Message><t:MessageXml xmlns:t=\"http://schemas.microsoft.com/exchange/services/2006/types\"></t:MessageXml></detail></s:Fault></s:Body></s:Envelope>";

		private static bool globalVariableInitialized;

		private static FileSearchAssemblyResolver fileSearchAssemblyResolver = new FileSearchAssemblyResolver();

		private static string[] approvedAssemblies = new string[]
		{
			"Microsoft.",
			"System."
		};

		private static LogonCache logonCache;

		private static bool enableClientSideLogonCache = true;

		private static bool preferClientSideLogonCacheToCookie;

		private static object cacheLockObject = new object();

		private static int logonCacheSize = 40000;

		private static int logonCacheLifetime = 480;

		private static int badCredsLifetime = 5;

		private static int badCredsRecoverableLifetime = 2;

		private static int level1BadCredCacheSize = 10000;

		private static int level1BadCredLifetime = 10;

		private static int level2BadCredCacheSize = 10000;

		private static int level2BadCredLifetime = 10;

		private static int level2BadCredListSize = 5;

		private static int OfflineOrgIdCookieExpirationTimeInMin = 30;

		private CompositeIdentityAuthenticationHelper compositeIdentityHelper;

		private static string localFqdn = string.Empty;

		private static string frontEndOWAUrl = string.Empty;
	}
}
