using System;
using System.ServiceModel;
using System.Web;
using Microsoft.Exchange.Clients.Common;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal static class CanaryExtensions
	{
		public static void CheckCanary(this HttpContext context)
		{
			if (context.ShouldCheckCanary15())
			{
				context.CheckCanary15(true, null);
				return;
			}
			string canaryVersion = "14.2";
			bool flag = false;
			CanaryStatus canaryStatus = CanaryStatus.None;
			context.SendCanary(ref canaryStatus, ref flag);
			bool flag2 = true;
			if (context.Request.IsAuthenticated)
			{
				bool flag3 = context.IsWebServiceRequest() || context.IsUploadRequest();
				if (flag3)
				{
					canaryStatus |= CanaryStatus.IsCanaryNeeded;
				}
				if ((context.IsWebServiceRequest() && !context.HasValidCanary(out canaryVersion, ref canaryStatus)) || (context.IsUploadRequest() && !context.HasValidUploadCanary(out canaryVersion, ref canaryStatus)))
				{
					flag2 = false;
					flag = true;
				}
				if (flag3 && flag2)
				{
					canaryStatus |= CanaryStatus.IsCanaryValid;
				}
			}
			if (flag)
			{
				CanaryExtensions.LogEvent(context, canaryVersion, canaryStatus);
			}
			if (!flag2)
			{
				throw new FaultException(Strings.InvalidCanary);
			}
		}

		public static void CheckCanaryForPostBack(this HttpContext context, string canaryName)
		{
			if (context.ShouldCheckCanary15())
			{
				context.CheckCanary15(false, canaryName);
				return;
			}
			CanaryStatus canaryStatus = CanaryStatus.None;
			bool flag = true;
			canaryStatus |= CanaryStatus.IsCanaryNeeded;
			string canaryVersion;
			if (!context.HasValidCanaryInForm(HttpContext.Current.Request.Form[canaryName], out canaryVersion, ref canaryStatus))
			{
				flag = false;
			}
			if (flag)
			{
				canaryStatus |= CanaryStatus.IsCanaryValid;
			}
			CanaryExtensions.LogEvent(context, canaryVersion, canaryStatus);
			if (!flag)
			{
				throw new BadRequestException(new Exception(Strings.InvalidCanary));
			}
		}

		public static HttpCookie GetCanaryCookie(this HttpContext context)
		{
			return context.Request.Cookies[context.GetCanaryName()];
		}

		public static void LogEvent(HttpContext context, string canaryVersion, CanaryStatus canaryStatus)
		{
			string canaryName = context.GetCanaryName();
			string ecpVDirForCanary = EcpUrl.GetEcpVDirForCanary();
			string logonUniqueKey;
			if (context.User is RbacSession)
			{
				logonUniqueKey = context.GetCachedUserUniqueKey();
			}
			else
			{
				logonUniqueKey = string.Empty;
			}
			ActivityContextLogger.Instance.LogEvent(new CanaryLogEvent(canaryVersion, ActivityContext.ActivityId.FormatForLog(), canaryName, ecpVDirForCanary, logonUniqueKey, canaryStatus, DateTime.MinValue, null));
		}

		public static bool ShouldCheckCanary15(this HttpContext context)
		{
			if (CanaryExtensions.alwaysUseE15CanaryConfigValue.Value)
			{
				return true;
			}
			bool flag = context.Request.Headers["X-IsFromCafe"] == "1";
			bool flag2 = context.User is InboundProxySession;
			return flag && !flag2;
		}

		public static void CheckCanary15(this HttpContext context, bool shouldRenew, string canaryName = null)
		{
			if (context.Request.IsAuthenticated && !context.IsLogoffRequest())
			{
				canaryName = (canaryName ?? context.GetCanaryName());
				string ecpVDirForCanary = EcpUrl.GetEcpVDirForCanary();
				string cachedUserUniqueKey = context.GetCachedUserUniqueKey();
				CanaryStatus canaryStatus = CanaryStatus.None;
				Canary15Profile profile = new Canary15Profile(canaryName, ecpVDirForCanary);
				bool flag = true;
				if (context.IsWebServiceRequest() || context.IsUploadRequest())
				{
					Canary15Cookie.CanaryValidationResult canaryValidationResult;
					flag = Canary15Cookie.ValidateCanaryInHeaders(context, cachedUserUniqueKey, profile, out canaryValidationResult);
					canaryStatus |= (CanaryStatus)canaryValidationResult;
					canaryStatus |= CanaryStatus.IsCanaryNeeded;
					canaryStatus |= (flag ? CanaryStatus.IsCanaryValid : CanaryStatus.None);
				}
				Canary15Cookie canary15Cookie = null;
				if (shouldRenew)
				{
					canary15Cookie = Canary15Cookie.TryCreateFromHttpContext(context, cachedUserUniqueKey, profile);
					bool isAboutToExpire = canary15Cookie.IsAboutToExpire;
					canaryStatus |= (canary15Cookie.IsAboutToExpire ? CanaryStatus.IsCanaryAboutToExpire : CanaryStatus.None);
					if (isAboutToExpire)
					{
						canary15Cookie = new Canary15Cookie(cachedUserUniqueKey, profile);
					}
					canaryStatus |= (canary15Cookie.IsRenewed ? CanaryStatus.IsCanaryRenewed : CanaryStatus.None);
					if (canary15Cookie.IsRenewed)
					{
						context.Response.SetCookie(canary15Cookie.HttpCookie);
					}
				}
				ActivityContextLogger.Instance.LogEvent(new CanaryLogEvent("15.1", ActivityContext.ActivityId.FormatForLog(), canaryName, ecpVDirForCanary, cachedUserUniqueKey, canaryStatus, (canary15Cookie == null) ? DateTime.MinValue : canary15Cookie.CreationTime, (canary15Cookie == null) ? null : canary15Cookie.ToLoggerString()));
				if (!flag)
				{
					throw new FaultException(Strings.InvalidCanary);
				}
			}
		}

		public static void SendCanary(this HttpContext context, ref CanaryStatus canaryStatus, ref bool shouldAddLog)
		{
			if (context.Request.IsAuthenticated && !context.IsLogoffRequest())
			{
				bool flag = false;
				string cachedUserUniqueKey = context.GetCachedUserUniqueKey();
				string canaryName = context.GetCanaryName();
				HttpCookie httpCookie = context.Request.Cookies[canaryName];
				if (httpCookie != null && Canary.RestoreCanary(httpCookie.Value, cachedUserUniqueKey) != null)
				{
					flag = true;
				}
				if (!flag)
				{
					if (httpCookie != null)
					{
						EcpEventLogConstants.Tuple_ResetCanaryInCookie.LogEvent(new object[]
						{
							EcpEventLogExtensions.GetUserNameToLog(),
							cachedUserUniqueKey,
							canaryName,
							context.GetRequestUrlForLog(),
							(httpCookie != null) ? httpCookie.Value : string.Empty
						});
					}
					Canary canary = new Canary(Guid.NewGuid(), cachedUserUniqueKey);
					HttpCookie httpCookie2 = new HttpCookie(canaryName, canary.ToString());
					httpCookie2.HttpOnly = false;
					httpCookie2.Path = EcpUrl.GetEcpVDirForCanary();
					context.Response.Cookies.Add(httpCookie2);
					canaryStatus |= CanaryStatus.IsCanaryRenewed;
				}
			}
			shouldAddLog = true;
		}

		public static bool HasValidCanary(this HttpContext context, out string canaryVersion, ref CanaryStatus canaryStatus)
		{
			string canaryName = context.GetCanaryName();
			string canaryInUrl = context.Request.QueryString[canaryName];
			bool flag = context.User is InboundProxySession;
			string canaryInHeader;
			if (flag)
			{
				canaryInHeader = context.Request.Headers["msExchEcpCanary"];
			}
			else
			{
				canaryInHeader = context.Request.Headers[canaryName];
			}
			return context.HasValidCanary(canaryInHeader, null, canaryInUrl, out canaryVersion, ref canaryStatus);
		}

		public static bool HasValidUploadCanary(this HttpContext context, out string canaryVersion, ref CanaryStatus canaryStatus)
		{
			return context.HasValidCanary(null, context.Request.Form[context.GetCanaryName()], null, out canaryVersion, ref canaryStatus);
		}

		public static bool HasValidCanaryInForm(this HttpContext context, string canaryInForm, out string canaryVersion, ref CanaryStatus canaryStatus)
		{
			return context.HasValidCanary(null, canaryInForm, null, out canaryVersion, ref canaryStatus);
		}

		private static bool HasValidCanary(this HttpContext context, string canaryInHeader, string canaryInForm, string canaryInUrl, out string canaryVersion, ref CanaryStatus canaryStatus)
		{
			bool flag = context.User is InboundProxySession;
			bool flag2 = !flag || !string.IsNullOrEmpty(context.Request.Headers["msExchEcpOutboundProxyVersion"]);
			canaryVersion = (flag2 ? "14.2" : "14.1");
			string canaryName = context.GetCanaryName();
			HttpCookie httpCookie = context.Request.Cookies[canaryName];
			string text = (httpCookie == null) ? string.Empty : httpCookie.Value;
			string cachedUserUniqueKey = context.GetCachedUserUniqueKey();
			Canary canary = Canary.RestoreCanary(text, cachedUserUniqueKey);
			bool flag3 = !flag2 || canary != null;
			bool flag4 = StringComparer.Ordinal.Equals(httpCookie.Value, canaryInForm);
			bool flag5 = StringComparer.Ordinal.Equals(httpCookie.Value, canaryInHeader);
			bool flag6 = StringComparer.Ordinal.Equals(httpCookie.Value, canaryInUrl);
			bool flag7 = false;
			if (httpCookie != null && !string.IsNullOrEmpty(httpCookie.Value) && flag3)
			{
				flag7 = (flag5 || flag4 || flag6);
			}
			if (flag7)
			{
				if (flag4)
				{
					canaryStatus |= (CanaryStatus)3;
				}
				if (flag5)
				{
					canaryStatus |= (CanaryStatus)1;
				}
				if (flag6)
				{
					canaryStatus |= (CanaryStatus)2;
				}
			}
			else if (!flag3)
			{
				EcpEventLogConstants.Tuple_InvalidCanaryInCookieDetected.LogPeriodicEvent(EcpEventLogExtensions.GetPeriodicKeyPerUser(), new object[]
				{
					EcpEventLogExtensions.GetUserNameToLog(),
					cachedUserUniqueKey,
					canaryName,
					context.GetRequestUrlForLog(),
					text
				});
			}
			else
			{
				EcpEventLogConstants.Tuple_InvalidCanaryDetected.LogPeriodicEvent(EcpEventLogExtensions.GetPeriodicKeyPerUser(), new object[]
				{
					EcpEventLogExtensions.GetUserNameToLog(),
					context.GetRequestUrlForLog(),
					text,
					string.Format("{0} in header, {1} in form, in URL {2}", canaryInHeader, canaryInForm, canaryInUrl)
				});
			}
			return flag7;
		}

		public static string GetCanaryName(this HttpContext context)
		{
			DelegatedPrincipal delegatedPrincipal = null;
			if (context.User is RbacSession)
			{
				RbacSession rbacSession = (RbacSession)context.User;
				delegatedPrincipal = (rbacSession.Settings.OriginalUser as DelegatedPrincipal);
			}
			if (delegatedPrincipal == null)
			{
				return "msExchEcpCanary";
			}
			return "msExchEcpCanary.UID";
		}

		internal static string GetCachedUserUniqueKey(this HttpContext context)
		{
			return ((RbacSession)context.User).Settings.UserUniqueKeyForCanary;
		}

		private const string CanaryVersion141 = "14.1";

		private const string CanaryVersion142 = "14.2";

		private const string CanaryVersion151 = "15.1";

		private static Lazy<bool> alwaysUseE15CanaryConfigValue = new Lazy<bool>(() => AppConfigLoader.GetConfigBoolValue("AlwaysUseE15Canary", false));
	}
}
