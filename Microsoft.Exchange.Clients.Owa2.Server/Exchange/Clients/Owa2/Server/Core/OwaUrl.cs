using System;
using System.Web;
using Microsoft.Exchange.Clients.Common;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	public class OwaUrl
	{
		private OwaUrl(string path)
		{
			this.path = path;
			this.url = this.ApplicationVRoot + path;
		}

		public static OwaUrl ApplicationRoot
		{
			get
			{
				return OwaUrl.applicationRoot;
			}
		}

		public static OwaUrl Default14Page
		{
			get
			{
				return OwaUrl.default14Page;
			}
		}

		public static OwaUrl Default15Page
		{
			get
			{
				return OwaUrl.default15Page;
			}
		}

		public static OwaUrl PLT1Page
		{
			get
			{
				return OwaUrl.plt1Page;
			}
		}

		public static OwaUrl SessionDataPage
		{
			get
			{
				return OwaUrl.sessionDataPage;
			}
		}

		public static OwaUrl PreloadSessionDataPage
		{
			get
			{
				return OwaUrl.preloadSessionDataPage;
			}
		}

		public static OwaUrl RemoteNotification
		{
			get
			{
				return OwaUrl.remoteNotification;
			}
		}

		public static OwaUrl GroupSubscription
		{
			get
			{
				return OwaUrl.groupSubscription;
			}
		}

		public static OwaUrl OehUrl
		{
			get
			{
				return OwaUrl.oehUrl;
			}
		}

		public static OwaUrl AttachmentHandler
		{
			get
			{
				return OwaUrl.attachmentHandler;
			}
		}

		public static OwaUrl AuthFolderUrl
		{
			get
			{
				return OwaUrl.authFolderUrl;
			}
		}

		public static OwaUrl ErrorPage
		{
			get
			{
				return OwaUrl.errorPage;
			}
		}

		public static OwaUrl Error2Page
		{
			get
			{
				return OwaUrl.error2Page;
			}
		}

		public static OwaUrl RedirectionPage
		{
			get
			{
				return OwaUrl.redirectionPage;
			}
		}

		public static OwaUrl ProxyLogon
		{
			get
			{
				return OwaUrl.proxyLogon;
			}
		}

		public static OwaUrl LanguagePage
		{
			get
			{
				return OwaUrl.languagePage;
			}
		}

		public static OwaUrl LanguagePost
		{
			get
			{
				return OwaUrl.languagePost;
			}
		}

		public static OwaUrl SignOutPage
		{
			get
			{
				return OwaUrl.signOutPage;
			}
		}

		public static OwaUrl LogonFBA
		{
			get
			{
				return OwaUrl.logonFBA;
			}
		}

		public static OwaUrl LogonFBAOWABlockedByClientAccessRules
		{
			get
			{
				return OwaUrl.logonFBAOWABlockedByClientAccessRules;
			}
		}

		public static OwaUrl LogonFBAEACBlockedByClientAccessRules
		{
			get
			{
				return OwaUrl.logonFBAEACBlockedByClientAccessRules;
			}
		}

		public static OwaUrl LogoffOwa
		{
			get
			{
				return OwaUrl.logoffOwa;
			}
		}

		public static OwaUrl LogoffBlockedByClientAccessRules
		{
			get
			{
				return OwaUrl.logoffBlockedByClientAccessRules;
			}
		}

		public static OwaUrl LogoffChangePassword
		{
			get
			{
				return OwaUrl.logoffChangePassword;
			}
		}

		public static OwaUrl LogoffAspxPage
		{
			get
			{
				return OwaUrl.logoffAspxPage;
			}
		}

		public static OwaUrl LogoffPageBlockedByClientAccessRules
		{
			get
			{
				return OwaUrl.logoffPageBlockedByClientAccessRules;
			}
		}

		public static OwaUrl LogoffChangePasswordPage
		{
			get
			{
				return OwaUrl.logoffChangePasswordPage;
			}
		}

		public static OwaUrl InfoFailedToSaveCulture
		{
			get
			{
				return OwaUrl.infoFailedToSaveCulture;
			}
		}

		public static OwaUrl ProxyPing
		{
			get
			{
				return OwaUrl.proxyPing;
			}
		}

		public static OwaUrl KeepAlive
		{
			get
			{
				return OwaUrl.keepAlive;
			}
		}

		public static OwaUrl AuthPost
		{
			get
			{
				return OwaUrl.authPost;
			}
		}

		public static OwaUrl AuthDll
		{
			get
			{
				return OwaUrl.authDll;
			}
		}

		public static OwaUrl PublishedCalendar
		{
			get
			{
				return OwaUrl.publishedCalendar;
			}
		}

		public static OwaUrl PublishedICal
		{
			get
			{
				return OwaUrl.publishedICal;
			}
		}

		public string ImplicitUrl
		{
			get
			{
				return this.url;
			}
		}

		public static OwaUrl Create(string path)
		{
			return new OwaUrl(path);
		}

		public string GetExplicitUrl(HttpRequest request)
		{
			if (request == null)
			{
				throw new ArgumentNullException("request");
			}
			string text = this.ApplicationVRoot;
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

		public const string AuthFolder = "auth";

		public const string Oeh = "ev.owa2";

		internal const string ReasonParameterName = "reason";

		internal const string BlockedByClientAccessRulesReason = "5";

		internal const string EACBlockedByClientAccessRulesReason = "6";

		internal const string LogOffCARParamName = "CARBlock";

		internal const string LogOffCARParamValue = "1";

		public readonly string ApplicationVRoot = HttpRuntime.AppDomainAppVirtualPath + "/";

		private readonly string path;

		private readonly string url;

		private static OwaUrl applicationRoot = OwaUrl.Create(string.Empty);

		private static OwaUrl default14Page = OwaUrl.Create("owa14.aspx");

		private static OwaUrl default15Page = OwaUrl.Create("default.aspx");

		private static OwaUrl sessionDataPage = OwaUrl.Create("sessiondata.ashx");

		private static OwaUrl preloadSessionDataPage = OwaUrl.Create("preloadsessiondata.ashx");

		private static OwaUrl plt1Page = OwaUrl.Create("plt1.ashx");

		private static OwaUrl remoteNotification = OwaUrl.Create("remotenotification.ashx");

		private static OwaUrl groupSubscription = OwaUrl.Create("groupsubscription.ashx");

		private static OwaUrl oehUrl = OwaUrl.Create("ev.owa2");

		private static OwaUrl attachmentHandler = OwaUrl.Create("attachment.ashx");

		private static OwaUrl authFolderUrl = OwaUrl.Create("auth/");

		private static OwaUrl errorPage = OwaUrl.Create("auth/error.aspx");

		private static OwaUrl error2Page = OwaUrl.Create("auth/error2.aspx");

		private static OwaUrl redirectionPage = OwaUrl.Create("redir.aspx");

		private static OwaUrl proxyLogon = OwaUrl.Create("proxyLogon.owa");

		private static OwaUrl languagePage = OwaUrl.Create("languageselection.aspx");

		private static OwaUrl languagePost = OwaUrl.Create("lang.owa");

		private static OwaUrl logonFBA = OwaUrl.Create("auth/logon.aspx");

		private static OwaUrl signOutPage = OwaUrl.Create(LogOnSettings.SignOutPageUrl);

		private static OwaUrl logonFBAOWABlockedByClientAccessRules = OwaUrl.Create(string.Format("auth/logon.aspx?{0}={1}", "reason", "5"));

		private static OwaUrl logonFBAEACBlockedByClientAccessRules = OwaUrl.Create(string.Format("auth/logon.aspx?{0}={1}&url={{0}}", "reason", "6"));

		private static OwaUrl logoffOwa = OwaUrl.Create("logoff.owa");

		private static OwaUrl logoffBlockedByClientAccessRules = OwaUrl.Create(string.Format("logoff.owa?{0}={1}", "reason", "5"));

		private static OwaUrl logoffChangePassword = OwaUrl.Create("logoff.owa?ChgPwd=1");

		private static OwaUrl logoffAspxPage = OwaUrl.Create("auth/logoff.aspx?Cmd=logoff&src=exch");

		private static OwaUrl logoffPageBlockedByClientAccessRules = OwaUrl.Create("auth" + string.Format("/logoff.aspx?Cmd=logoff&{0}={1}", "CARBlock", "1"));

		private static OwaUrl logoffChangePasswordPage = OwaUrl.Create("auth/logoff.aspx?Cmd=logoff&ChgPwd=1");

		private static OwaUrl infoFailedToSaveCulture = OwaUrl.Create("info.aspx?Msg=1");

		private static OwaUrl proxyPing = OwaUrl.Create("ping.owa");

		private static OwaUrl keepAlive = OwaUrl.Create("keepalive.owa");

		private static OwaUrl authPost = OwaUrl.Create("auth.owa");

		private static OwaUrl authDll = OwaUrl.Create("auth/owaauth.dll");

		private static OwaUrl publishedCalendar = OwaUrl.Create("calendar.html");

		private static OwaUrl publishedICal = OwaUrl.Create("calendar.ics");

		public static OwaUrl SuiteServiceProxyPage = OwaUrl.Create("SuiteServiceProxy.aspx");
	}
}
