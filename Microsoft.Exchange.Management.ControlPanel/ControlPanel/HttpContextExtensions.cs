using System;
using System.Security.Principal;
using System.Web;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Security.OAuth;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public static class HttpContextExtensions
	{
		public static bool IsLogoffRequest(this HttpContext context)
		{
			return context.Request.FilePath.EndsWith("logoff.aspx", StringComparison.OrdinalIgnoreCase);
		}

		public static bool IsWebServiceRequest(this HttpContext context)
		{
			return context.Request.HttpMethod == "POST" && context.Request.FilePath.Contains(".svc");
		}

		public static bool IsUploadRequest(this HttpContext context)
		{
			return context.Request.HttpMethod == "POST" && context.Request.FilePath.EndsWith("UploadHandler.ashx");
		}

		public static bool IsNarrowPage(this HttpContext context)
		{
			string text = HttpContext.Current.Request.QueryString["isNarrow"];
			return text != null && text == "t";
		}

		public static Exception GetError(this HttpContext context)
		{
			if (context.Error != null)
			{
				return context.Error;
			}
			Exception[] allErrors = context.AllErrors;
			if (allErrors.IsNullOrEmpty())
			{
				return null;
			}
			return allErrors[0];
		}

		public static string GetExplicitUser(this HttpContext context)
		{
			return context.Request.Headers["msExchEcpESOUser"] ?? string.Empty;
		}

		public static string GetOwaNavigationParameter(this HttpContext context)
		{
			return context.Request.QueryString["owaparam"] ?? string.Empty;
		}

		public static bool IsExplicitSignOn(this HttpContext context)
		{
			return !string.IsNullOrEmpty(context.GetExplicitUser());
		}

		public static string GetTargetTenant(this HttpContext context)
		{
			string result;
			if ((result = context.Request.Headers[RequestFilterModule.TargetTenantKey]) == null)
			{
				result = (context.Request.Headers[RequestFilterModule.OrganizationContextKey] ?? string.Empty);
			}
			return result;
		}

		public static bool HasTargetTenant(this HttpContext context)
		{
			return !string.IsNullOrEmpty(context.GetTargetTenant());
		}

		public static bool HasOrganizationContext(this HttpContext context)
		{
			return !string.IsNullOrEmpty(context.Request.Headers[RequestFilterModule.OrganizationContextKey]);
		}

		public static string CurrentUserLiveID()
		{
			string text = string.Empty;
			if (HttpContext.Current.Items.Contains("RPSMemberName"))
			{
				text = (HttpContext.Current.Items["RPSMemberName"] as string);
			}
			else
			{
				text = HttpContext.Current.Request.Headers["RPSMemberName"];
			}
			if (!string.IsNullOrEmpty(text))
			{
				SmtpAddress smtpAddress = new SmtpAddress(text);
				if (!smtpAddress.IsValidAddress)
				{
					text = string.Empty;
				}
			}
			return text;
		}

		public static string GetCurrentLiveIDEnvironment()
		{
			string text = HttpContext.Current.Request.Headers["RPSEnv"];
			if (string.IsNullOrEmpty(text))
			{
				if (HttpContext.Current.Items.Contains("RPSEnv"))
				{
					text = (HttpContext.Current.Items["RPSEnv"] as string);
				}
				else
				{
					text = string.Empty;
				}
			}
			return text;
		}

		public static string GetSessionID(this HttpContext context)
		{
			HttpCookie httpCookie = context.Request.Cookies[RbacModule.SessionStateCookieName];
			if (httpCookie == null)
			{
				httpCookie = context.Response.Cookies[RbacModule.SessionStateCookieName];
				if (httpCookie == null)
				{
					throw new InvalidOperationException("Session cookie hasn't been set.");
				}
			}
			return httpCookie.Value;
		}

		public static Uri GetRequestUrl(this HttpContext context)
		{
			Uri uri = (Uri)context.Items["RequestUrl"];
			if (uri == null || uri.PathAndQuery != context.Request.Url.PathAndQuery)
			{
				string text = context.Request.Headers["msExchProxyUri"];
				if (!string.IsNullOrEmpty(text))
				{
					uri = new Uri(new Uri(text), context.Request.Url.PathAndQuery);
				}
				else
				{
					uri = context.Request.Url;
				}
				context.Items["RequestUrl"] = uri;
			}
			return uri;
		}

		public static string GetRequestUrlPathAndQuery(this HttpContext context)
		{
			return context.Request.Url.PathAndQuery;
		}

		public static string GetRequestUrlAbsolutePath(this HttpContext context)
		{
			return context.Request.Url.AbsolutePath;
		}

		public static string GetRequestUrlForLog(this HttpContext context)
		{
			string text = context.Request.Headers["msExchProxyUri"];
			if (!string.IsNullOrEmpty(text))
			{
				return string.Format("{0}({1})", context.Request.Url, text);
			}
			return context.Request.Url.ToString();
		}

		public static bool TargetServerOrVersionSpecifiedInUrlOrCookie(this HttpContext context)
		{
			return !string.IsNullOrEmpty(context.Request.QueryString["ExchClientVer"]) || !string.IsNullOrEmpty(context.Request.QueryString["TargetServer"]) || context.Request.Cookies["ExchClientVer"] != null || context.Request.Cookies["TargetServer"] != null;
		}

		public static void ThrowIfViewOptionsWithBEParam(this HttpContext context, FeatureSet featureSet)
		{
			if (featureSet == FeatureSet.Options && context.TargetServerOrVersionSpecifiedInUrlOrCookie() && !context.IsExplicitSignOn())
			{
				throw new CannotAccessOptionsWithBEParamOrCookieException();
			}
		}

		public static bool IsAcsOAuthRequest(this HttpContext context)
		{
			if (context.User == null)
			{
				return false;
			}
			RbacSession rbacSession = context.User as RbacSession;
			IIdentity identity;
			if (rbacSession != null)
			{
				identity = rbacSession.Settings.LogonUserIdentity;
			}
			else
			{
				LogoffSession logoffSession = context.User as LogoffSession;
				if (logoffSession != null)
				{
					identity = logoffSession.OriginalIdentity;
				}
				else
				{
					identity = context.User.Identity;
				}
			}
			return identity is OAuthIdentity || identity is SidOAuthIdentity || context.Items["LogonUserIdentity"] is SidOAuthIdentity;
		}

		internal const string LogoffPage = "logoff.aspx";

		internal const string OwaNavigationParameter = "owaparam";

		internal const string LogonUserIdentityKey = "LogonUserIdentity";
	}
}
