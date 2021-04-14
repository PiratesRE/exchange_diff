using System;
using System.Web;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public class OwaUrl
	{
		private OwaUrl(string path)
		{
			this.path = path;
			this.url = OwaUrl.applicationVRoot + path;
		}

		private static OwaUrl Create(string path)
		{
			return new OwaUrl(path);
		}

		public string GetExplicitUrl(OwaContext owaContext)
		{
			if (owaContext == null)
			{
				throw new ArgumentNullException("owaContext");
			}
			return this.GetExplicitUrl(owaContext.HttpContext.Request);
		}

		public string GetExplicitUrl(HttpRequest request)
		{
			if (request == null)
			{
				throw new ArgumentNullException("request");
			}
			string text = OwaUrl.applicationVRoot;
			string text2 = request.Headers["X-OWA-ExplicitLogonUser"];
			if (!string.IsNullOrEmpty(text2))
			{
				text = text + text2 + "/";
			}
			if (this.path != null)
			{
				text += this.path;
			}
			return text;
		}

		public string ImplicitUrl
		{
			get
			{
				return this.url;
			}
		}

		private static readonly string applicationVRoot = HttpRuntime.AppDomainAppVirtualPath + "/";

		private static readonly string authFolder = "auth";

		private static readonly string oeh = "ev.owa";

		public static OwaUrl ApplicationRoot = OwaUrl.Create(string.Empty);

		public static OwaUrl Default14Page = OwaUrl.Create("owa14.aspx");

		public static OwaUrl Default15Page = OwaUrl.Create("default.aspx");

		public static OwaUrl Oeh = OwaUrl.Create(OwaUrl.oeh);

		public static OwaUrl AttachmentHandler = OwaUrl.Create("attachment.ashx");

		public static OwaUrl AuthFolder = OwaUrl.Create(OwaUrl.authFolder + "/");

		public static OwaUrl ErrorPage = OwaUrl.Create(OwaUrl.authFolder + "/error.aspx");

		public static OwaUrl Error2Page = OwaUrl.Create(OwaUrl.authFolder + "/error2.aspx");

		public static OwaUrl RedirectionPage = OwaUrl.Create("redir.aspx");

		public static OwaUrl ProxyLogon = OwaUrl.Create("proxyLogon.owa");

		public static OwaUrl CasRedirectPage = OwaUrl.Create("casredirect.aspx");

		public static OwaUrl LanguagePage = OwaUrl.Create("languageselection.aspx");

		public static OwaUrl LanguagePost = OwaUrl.Create("lang.owa");

		public static OwaUrl LogonFBA = OwaUrl.Create("auth/logon.aspx");

		public static OwaUrl Logoff = OwaUrl.Create("logoff.owa");

		public static OwaUrl LogoffChangePassword = OwaUrl.Create("logoff.owa?ChgPwd=1");

		public static OwaUrl LogoffPage = OwaUrl.Create(OwaUrl.authFolder + "/logoff.aspx?Cmd=logoff&src=exch");

		public static OwaUrl LogoffChangePasswordPage = OwaUrl.Create(OwaUrl.authFolder + "/logoff.aspx?Cmd=logoff&ChgPwd=1");

		public static OwaUrl InfoFailedToSaveCulture = OwaUrl.Create("info.aspx?Msg=1");

		public static OwaUrl ProxyHandler = OwaUrl.Create(OwaUrl.oeh + "?oeh=1&ns=HttpProxy&ev=ProxyRequest");

		public static OwaUrl ProxyLanguagePost = OwaUrl.Create(OwaUrl.oeh + "?oeh=1&ns=HttpProxy&ev=LanguagePost");

		public static OwaUrl ProxyPing = OwaUrl.Create("ping.owa");

		public static OwaUrl ProxyEws = OwaUrl.Create(OwaUrl.oeh + "?oeh=1&ns=EwsProxy&ev=Proxy");

		public static OwaUrl HealthPing = OwaUrl.Create(OwaUrl.oeh + "?oeh=1&ns=Monitoring&ev=Ping");

		public static OwaUrl KeepAlive = OwaUrl.Create("keepalive.owa");

		public static OwaUrl AuthPost = OwaUrl.Create("auth.owa");

		public static OwaUrl AuthDll = OwaUrl.Create("auth/owaauth.dll");

		public static OwaUrl PublishedCalendar = OwaUrl.Create("calendar.html");

		public static OwaUrl ReachPublishedCalendar = OwaUrl.Create("reachcalendar.html");

		public static OwaUrl PublishedICal = OwaUrl.Create("calendar.ics");

		public static OwaUrl ReachPublishedICal = OwaUrl.Create("reachcalendar.ics");

		public static OwaUrl WebReadyUrl = OwaUrl.Create("WebReadyView");

		private string path;

		private string url;
	}
}
