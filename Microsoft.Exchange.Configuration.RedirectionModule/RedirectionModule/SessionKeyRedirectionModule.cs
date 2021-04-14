using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Web;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Configuration.RedirectionModule
{
	public class SessionKeyRedirectionModule : IHttpModule
	{
		void IHttpModule.Init(HttpApplication application)
		{
			application.PostAuthenticateRequest += SessionKeyRedirectionModule.OnPostAuthenticateRequestHandler;
		}

		void IHttpModule.Dispose()
		{
		}

		private static void OnPostAuthenticateRequest(object source, EventArgs args)
		{
			HttpApplication httpApplication = (HttpApplication)source;
			HttpContext context = httpApplication.Context;
			if (!context.Request.IsAuthenticated)
			{
				Logger.LogWarning(SessionKeyRedirectionModule.traceSrc, "OnPostAuthenticateRequest was called on a not Authenticated Request!");
				return;
			}
			string tenantName = SessionKeyRedirectionModule.ResolveTenantName();
			Uri uri = RedirectionHelper.RemovePropertiesFromOriginalUri(context.Request.Url, RedirectionConfig.RedirectionUriFilterProperties);
			if (SessionKeyRedirectionModule.ShouldAddSessionKey(uri, tenantName))
			{
				SessionKeyRedirectionModule.AddSessionKey(ref uri);
				Logger.LogVerbose(SessionKeyRedirectionModule.traceSrc, "Redirecting user to {0}.", new object[]
				{
					uri
				});
				context.Response.Redirect(uri.ToString());
			}
		}

		private static string ResolveTenantName()
		{
			string text = (string)HttpContext.Current.Items["Cert-MemberOrg"];
			if (string.IsNullOrEmpty(text))
			{
				string text2 = (string)HttpContext.Current.Items["WLID-MemberName"];
				if (!string.IsNullOrEmpty(text2) && SmtpAddress.IsValidSmtpAddress(text2))
				{
					text = SmtpAddress.Parse(text2).Domain;
				}
			}
			return text;
		}

		private static void AddSessionKey(ref Uri uri)
		{
			string text = uri.ToString();
			int random = SessionKeyRedirectionModule.GetRandom();
			text = string.Format("{0};{1}={2}", text, "sessionKey", random);
			UriBuilder uriBuilder = new UriBuilder(text);
			uri = uriBuilder.Uri;
		}

		private static bool ShouldAddSessionKey(Uri originalUri, string tenantName)
		{
			if (RedirectionConfig.SessionKeyCreationStatus == RedirectionConfig.SessionKeyCreation.Disable)
			{
				return false;
			}
			if (!HttpContext.Current.Request.IsSecureConnection)
			{
				return false;
			}
			NameValueCollection urlProperties = RedirectionHelper.GetUrlProperties(originalUri);
			return urlProperties["sessionKey"] == null && (RedirectionConfig.SessionKeyCreationStatus != RedirectionConfig.SessionKeyCreation.Partner || string.IsNullOrEmpty(tenantName));
		}

		private static int GetRandom()
		{
			byte[] array = new byte[4];
			SessionKeyRedirectionModule.randomInstance.GetBytes(array);
			return Math.Abs(BitConverter.ToInt32(array, 0)) % 999;
		}

		public const string SessionKeyName = "sessionKey";

		private static readonly EventHandler OnPostAuthenticateRequestHandler = new EventHandler(SessionKeyRedirectionModule.OnPostAuthenticateRequest);

		private static readonly TraceSource traceSrc = new TraceSource("SessionKeyRedirectionModule");

		private static RNGCryptoServiceProvider randomInstance = new RNGCryptoServiceProvider();
	}
}
