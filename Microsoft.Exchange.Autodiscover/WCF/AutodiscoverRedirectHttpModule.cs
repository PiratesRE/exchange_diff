using System;
using System.Configuration;
using System.Text;
using System.Web;
using Microsoft.Exchange.Diagnostics.Components.Autodiscover;

namespace Microsoft.Exchange.Autodiscover.WCF
{
	public class AutodiscoverRedirectHttpModule : IHttpModule
	{
		public void Dispose()
		{
		}

		public void Init(HttpApplication application)
		{
			bool.TryParse(ConfigurationManager.AppSettings["AutodiscoverRedirectHttpModule.EnableHotmailRedirect"], out AutodiscoverRedirectHttpModule.enableHotmailRedirect);
			AutodiscoverRedirectHttpModule.hotmailRedirectUrl = ConfigurationManager.AppSettings["AutodiscoverRedirectHttpModule.HotmailRedirectUrl"];
			string text = ConfigurationManager.AppSettings["AutodiscoverRedirectHttpModule.HotmailTopLevelDomains"];
			if (!string.IsNullOrWhiteSpace(text))
			{
				string[] array = text.Split(new char[]
				{
					','
				}, StringSplitOptions.RemoveEmptyEntries);
				AutodiscoverRedirectHttpModule.hotmailDomains = new string[array.Length];
				int num = 0;
				foreach (string str in array)
				{
					AutodiscoverRedirectHttpModule.hotmailDomains[num++] = "@outlook." + str;
				}
			}
			else
			{
				AutodiscoverRedirectHttpModule.hotmailDomains = new string[]
				{
					"com"
				};
			}
			application.AuthenticateRequest += this.OnAuthenticate;
		}

		private void OnAuthenticate(object source, EventArgs args)
		{
			ExTraceGlobals.FrameworkTracer.TraceDebug((long)this.GetHashCode(), "AutodiscoverRedirectHttpModule.OnAuthenticate start");
			HttpApplication httpApplication = (HttpApplication)source;
			HttpContext context = httpApplication.Context;
			ExTraceGlobals.FrameworkTracer.TraceDebug<string, string>((long)this.GetHashCode(), "AutodiscoverRedirectHttpModule.OnAuthenticate {0} {1}", context.Request.HttpMethod, context.Request.Url.AbsoluteUri);
			string text = context.Request.Headers["Authorization"];
			string text2 = null;
			ExTraceGlobals.FrameworkTracer.TraceDebug<string>((long)this.GetHashCode(), "AutodiscoverRedirectHttpModule.OnAuthenticate Auth {0}", (text == null) ? "not present" : "present");
			if (this.ParseCredentials(context, text, out text2))
			{
				ExTraceGlobals.FrameworkTracer.TraceDebug<string>((long)this.GetHashCode(), "AutodiscoverRedirectHttpModule.OnAuthenticate userName={0}", text2);
				if (AutodiscoverRedirectHttpModule.enableHotmailRedirect && !string.IsNullOrEmpty(AutodiscoverRedirectHttpModule.hotmailRedirectUrl) && this.IsHotmailAddress(text2))
				{
					ExTraceGlobals.FrameworkTracer.TraceDebug((long)this.GetHashCode(), "AutodiscoverRedirectHttpModule.OnAuthenticate This is a hotmail user");
					if (context.Request.Url.AbsolutePath.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
					{
						ExTraceGlobals.FrameworkTracer.TraceDebug((long)this.GetHashCode(), "AutodiscoverRedirectHttpModule.OnAuthenticate Redirecting to hotmail");
						context.Items["AutodiscoverRedirectLog"] = "Hotmail";
						context.Response.Redirect(AutodiscoverRedirectHttpModule.hotmailRedirectUrl, false);
					}
					else
					{
						ExTraceGlobals.FrameworkTracer.TraceDebug((long)this.GetHashCode(), "AutodiscoverRedirectHttpModule.OnAuthenticate Returning 400");
						context.Items["AutodiscoverRedirectLog"] = "BadRequest";
						context.Response.StatusCode = 400;
					}
					httpApplication.CompleteRequest();
				}
			}
		}

		private bool IsHotmailAddress(string userName)
		{
			if (userName.Contains("@outlook."))
			{
				foreach (string value in AutodiscoverRedirectHttpModule.hotmailDomains)
				{
					if (userName.EndsWith(value, StringComparison.OrdinalIgnoreCase))
					{
						return true;
					}
				}
			}
			return false;
		}

		internal bool ParseCredentials(HttpContext context, string authHeader, out string userName)
		{
			userName = null;
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
					context.Items["AutodiscoverRedirectLog"] = "cannot decode Base64 auth header";
					return false;
				}
				int num = Array.IndexOf<byte>(array, 58);
				if (num <= 0)
				{
					context.Items["AutodiscoverRedirectLog"] = "missing colon separator";
					return false;
				}
				int num2 = Array.IndexOf<byte>(array, 92, 0, num);
				num2++;
				userName = Encoding.UTF8.GetString(array, num2, num - num2).Trim();
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

		public const string AutodiscoverRedirectLogKey = "AutodiscoverRedirectLog";

		private const string HotmailAddressDomainPrefix = "@outlook.";

		private static bool enableHotmailRedirect = false;

		private static string hotmailRedirectUrl = null;

		private static string[] hotmailDomains = new string[]
		{
			"com"
		};
	}
}
