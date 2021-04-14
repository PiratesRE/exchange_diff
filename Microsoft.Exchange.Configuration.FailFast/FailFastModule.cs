using System;
using System.Configuration;
using System.Diagnostics;
using System.Web;
using Microsoft.Exchange.Configuration.Core;
using Microsoft.Exchange.Configuration.FailFast.EventLog;
using Microsoft.Exchange.Configuration.FailFast.LocStrings;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics.CmdletInfra;
using Microsoft.Exchange.Diagnostics.Components.FailFast;

namespace Microsoft.Exchange.Configuration.FailFast
{
	public class FailFastModule : IHttpModule
	{
		static FailFastModule()
		{
			FailFastUserCache.IsPrimaryUserCache = true;
			bool flag = false;
			if (!bool.TryParse(ConfigurationManager.AppSettings["FailFastEnabled"], out flag))
			{
				flag = false;
			}
			FailFastUserCache.FailFastEnabled = flag;
			ExTraceGlobals.FailFastModuleTracer.Information<bool>(0L, "FailFast Enabled:", flag);
			Logger.LogEvent(TaskEventLogConstants.Tuple_LogFailFastEnabledFlag, null, new object[]
			{
				flag
			});
			FailFastModule.perfCounter = FailFastModule.CreatePerfCounter();
		}

		internal static RemotePowershellPerformanceCountersInstance RemotePowershellPerfCounter
		{
			get
			{
				return FailFastModule.perfCounter;
			}
		}

		internal static string HeaderKeyToStoreUserToken
		{
			get
			{
				return FailFastModule.headerKeyToStoreUserToken;
			}
		}

		void IHttpModule.Init(HttpApplication application)
		{
			application.BeginRequest += FailFastModule.ApplicationBeginRequest;
			application.EndRequest += FailFastModule.ApplicationEndRequest;
		}

		void IHttpModule.Dispose()
		{
		}

		private static void ApplicationBeginRequest(object sender, EventArgs e)
		{
			if (!FailFastUserCache.FailFastEnabled)
			{
				return;
			}
			Logger.EnterFunction(ExTraceGlobals.FailFastModuleTracer, "FailFastModule.ApplicationBeginRequest");
			HttpApplication httpApplication = (HttpApplication)sender;
			HttpContext context = httpApplication.Context;
			string text = null;
			foreach (IUserTokenParser userTokenParser in FailFastModule.tokenParsers)
			{
				if (userTokenParser.TryParseUserToken(context, out text))
				{
					Logger.TraceInformation(ExTraceGlobals.FailFastModuleTracer, "{0} module parses the user token successfully. User token parsed: {1}.", new object[]
					{
						userTokenParser.GetType(),
						text
					});
					break;
				}
			}
			if (string.IsNullOrEmpty(text))
			{
				Logger.TraceDebug(ExTraceGlobals.FailFastModuleTracer, "No usertoken is parsed, exit directly.", new object[0]);
				Logger.ExitFunction(ExTraceGlobals.FailFastModuleTracer, "FailFastModule.ApplicationBeginRequest");
				return;
			}
			if (context == null || context.Response == null)
			{
				Logger.TraceDebug(ExTraceGlobals.FailFastModuleTracer, "context == null || context.Response == null", new object[0]);
				Logger.ExitFunction(ExTraceGlobals.FailFastModuleTracer, "FailFastModule.ApplicationBeginRequest");
				return;
			}
			context.Response.Headers[FailFastModule.headerKeyToStoreUserToken] = text;
			string userTenant = FailFastModule.GetUserTenant(context, text);
			Logger.TraceDebug(ExTraceGlobals.FailFastModuleTracer, "Tenant for current user {0} is {1} (If not email domain, it is in Delegated scenario).", new object[]
			{
				text,
				userTenant
			});
			string text2;
			FailFastUserCacheValue failFastUserCacheValue;
			BlockedReason blockedReason;
			if (!FailFastUserCache.Instance.IsUserInCache(text, userTenant, out text2, out failFastUserCacheValue, out blockedReason))
			{
				Logger.TraceDebug(ExTraceGlobals.FailFastModuleTracer, "Http request of User {0} is not handled by FailFastModule because it is not in FailFastUserCache.", new object[]
				{
					text
				});
				Logger.ExitFunction(ExTraceGlobals.FailFastModuleTracer, "FailFastModule.ApplicationBeginRequest");
				return;
			}
			if (!ConnectedUserManager.ShouldFailFastUserInCache(text, text2, failFastUserCacheValue, blockedReason))
			{
				Logger.TraceDebug(ExTraceGlobals.FailFastModuleTracer, "Http request of User {0} is not considered to be FailFast case. CacheKey: {1}. CacheValue: {2}. BlockedReason {3}.", new object[]
				{
					text,
					text2,
					failFastUserCacheValue,
					blockedReason
				});
				Logger.ExitFunction(ExTraceGlobals.FailFastModuleTracer, "FailFastModule.ApplicationBeginRequest");
				return;
			}
			Logger.LogEvent(TaskEventLogConstants.Tuple_LogUserRequestIsFailed, text2, new object[]
			{
				text,
				text2,
				failFastUserCacheValue.ToString()
			});
			Logger.TraceError(ExTraceGlobals.FailFastModuleTracer, "Http request of User {0} is terminated in fail-fast module.", new object[]
			{
				text
			});
			FailFastModule.perfCounter.RequestsBeFailFasted.RawValue = FailFastModule.perfCounter.RequestsBeFailFasted.RawValue + 1L;
			HttpLogger.SafeSetLogger(RpsHttpMetadata.FailFast, string.Concat(new object[]
			{
				text,
				"+",
				blockedReason,
				"+",
				(failFastUserCacheValue == null) ? "Null" : failFastUserCacheValue.HitCount.ToString()
			}));
			ConnectedUserManager.RemoveUser(text);
			string responseErrorMessage = FailFastModule.GetResponseErrorMessage(blockedReason);
			context.Response.ContentType = "application/soap+xml;charset=UTF-8";
			context.Response.Write(responseErrorMessage);
			context.Response.StatusCode = 400;
			context.Response.SubStatusCode = 350;
			context.Response.TrySkipIisCustomErrors = true;
			WinRMInfo.SetFailureCategoryInfo(context.Response.Headers, FailureCategory.FailFast, blockedReason.ToString());
			Logger.TraceInformation(ExTraceGlobals.FailFastModuleTracer, "Sending 400.350 to the client.", new object[0]);
			context.Response.End();
			Logger.ExitFunction(ExTraceGlobals.FailFastModuleTracer, "FailFastModule.ApplicationBeginRequest");
		}

		private static void ApplicationEndRequest(object sender, EventArgs e)
		{
			if (!FailFastUserCache.FailFastEnabled)
			{
				return;
			}
			Logger.EnterFunction(ExTraceGlobals.FailFastModuleTracer, "FailFastModule.ApplicationEndRequest");
			HttpApplication httpApplication = (HttpApplication)sender;
			HttpContext context = httpApplication.Context;
			if (context == null || context.Request == null || context.Response == null)
			{
				Logger.TraceDebug(ExTraceGlobals.FailFastModuleTracer, "context == null || context.Request == null || context.Response == null", new object[0]);
				Logger.ExitFunction(ExTraceGlobals.FailFastModuleTracer, "FailFastModule.ApplicationEndRequest");
				return;
			}
			if (!context.Request.IsAuthenticated)
			{
				Logger.TraceDebug(ExTraceGlobals.FailFastModuleTracer, "Return directly for un-authenticated request.", new object[0]);
				Logger.ExitFunction(ExTraceGlobals.FailFastModuleTracer, "FailFastModule.ApplicationEndRequest");
				return;
			}
			string text = context.Response.Headers[FailFastModule.headerKeyToStoreUserToken];
			Logger.TraceDebug(ExTraceGlobals.FailFastModuleTracer, "Current UserToken is {0}.", new object[]
			{
				text
			});
			if (!string.IsNullOrEmpty(text))
			{
				int statusCode = context.Response.StatusCode;
				Logger.TraceDebug(ExTraceGlobals.FailFastModuleTracer, "Response status code: {0}.", new object[]
				{
					statusCode
				});
				if (statusCode == 200)
				{
					ConnectedUserManager.AddUser(text);
				}
				else if (statusCode == 500)
				{
					ConnectedUserManager.RefreshUser(text);
				}
			}
			Logger.ExitFunction(ExTraceGlobals.FailFastModuleTracer, "FailFastModule.ApplicationEndRequest");
		}

		private static bool TryParseEmailDomain(string userToken, out string emailDomain)
		{
			emailDomain = null;
			int num = userToken.IndexOf('@');
			if (num != -1 && num < userToken.Length - 1)
			{
				emailDomain = userToken.Substring(num + 1);
				if (emailDomain.IndexOf('@') == -1)
				{
					return true;
				}
				Logger.TraceError(ExTraceGlobals.FailFastCacheTracer, "Got user token {0} with multiple '@'.", new object[]
				{
					userToken
				});
			}
			else
			{
				Logger.TraceError(ExTraceGlobals.FailFastCacheTracer, "Got user token {0} without '@' or '@' is the last character.", new object[]
				{
					userToken
				});
			}
			return false;
		}

		private static RemotePowershellPerformanceCountersInstance CreatePerfCounter()
		{
			RemotePowershellPerformanceCountersInstance currentPerfCounter = RPSPerfCounterHelper.CurrentPerfCounter;
			using (Process currentProcess = Process.GetCurrentProcess())
			{
				currentPerfCounter.PID.RawValue = (long)currentProcess.Id;
			}
			currentPerfCounter.RequestsBeFailFasted.RawValue = 0L;
			return currentPerfCounter;
		}

		private static string GetUserTenant(HttpContext context, string userToken)
		{
			string text = null;
			if (context.Request != null)
			{
				text = context.Request.Headers["msExchTargetTenant"];
			}
			if (string.IsNullOrEmpty(text))
			{
				FailFastModule.TryParseEmailDomain(userToken, out text);
			}
			return text;
		}

		private static string GetResponseErrorMessage(BlockedReason blockedReason)
		{
			string failedReason;
			switch (blockedReason)
			{
			case BlockedReason.BySelf:
				failedReason = Strings.FailBecauseOfSelf;
				break;
			case BlockedReason.ByTenant:
				failedReason = Strings.FailBecauseOfTenant;
				break;
			case BlockedReason.ByServer:
				failedReason = Strings.FailBecauseOfServer;
				break;
			default:
				throw new InvalidOperationException(string.Format("DEV Code Bug. Unexpected blocked reason {0}", blockedReason));
			}
			return Strings.RequestBeingBlockedInFailFast(failedReason);
		}

		private const string WSManContentType = "application/soap+xml;charset=UTF-8";

		private const char EmailDomainCharacter = '@';

		private const int SubStatusCodeForFailFast = 350;

		private static readonly string headerKeyToStoreUserToken = "MSExchange-FailFast-UserToken";

		private static readonly RemotePowershellPerformanceCountersInstance perfCounter;

		private static readonly IUserTokenParser[] tokenParsers = new IUserTokenParser[]
		{
			BasicLiveIDAuthUserTokenParser.Instance
		};
	}
}
