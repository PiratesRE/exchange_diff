using System;
using System.Net;
using Microsoft.Exchange.Net.WebApplicationClient;

namespace Microsoft.Exchange.Monitoring
{
	internal class ExchangeControlPanelApplication : ExchangeWebApplication
	{
		public ExchangeControlPanelApplication(string virtualDirectory, WebSession webSession) : base(virtualDirectory, webSession)
		{
			webSession.SendingRequest += this.webSession_SendingRequest;
		}

		protected override bool IsLanguageSelectionResponse(RedirectResponse response)
		{
			return response.IsRedirect && response.RedirectUrl.IndexOf("/owa/languageselection.aspx", StringComparison.OrdinalIgnoreCase) >= 0;
		}

		public override bool ValidateLogin()
		{
			return base.ValidateLogin() && null != base.GetCookie("ASP.NET_SessionId");
		}

		private void webSession_SendingRequest(object sender, HttpWebRequestEventArgs e)
		{
			e.Request.Headers.Set("msExchEcpOutboundProxyVersion", "2");
			Cookie cookie = base.GetCookie("msExchEcpCanary");
			if (cookie != null)
			{
				e.Request.Headers.Set("msExchEcpCanary", cookie.Value);
			}
			cookie = base.GetCookie("msExchEcpCanary.UID");
			if (cookie != null)
			{
				e.Request.Headers.Set("msExchEcpCanary.UID", cookie.Value);
			}
		}

		protected Cookie AspNetSessionCookie
		{
			get
			{
				return base.GetCookie("ASP.NET_SessionId");
			}
		}

		public void Ping(Action<ExchangeControlPanelApplication.PingResponse> onStatusAvailable, Action<Exception> onError)
		{
			base.Get<ExchangeControlPanelApplication.PingResponse>("exhealth.check", delegate(ExchangeControlPanelApplication.PingResponse ping)
			{
				onStatusAvailable(ping);
			}, delegate(Exception ex)
			{
				onError(ex);
			});
		}

		public static class EcpPaths
		{
			public const string Main = "";

			public const string Ping = "exhealth.check";
		}

		public static class Headers
		{
			public const string Session = "ASP.NET_SessionId";

			public const string Canary = "msExchEcpCanary";

			public const string CanaryFromUserId = "msExchEcpCanary.UID";

			public const string OutboundProxyVersion = "msExchEcpOutboundProxyVersion";

			public const string CurrentOutboundServerVersion = "2";

			public const string ApplicationVersion = "msExchEcpVersion";
		}

		public class PingResponse : WebApplicationResponse
		{
			public bool IsAlive
			{
				get
				{
					return base.StatusCode == HttpStatusCode.OK;
				}
			}

			public Version ApplicationVersion { get; private set; }

			public override void SetResponse(HttpWebResponse response)
			{
				base.SetResponse(response);
				string text = response.Headers["msExchEcpVersion"];
				this.ApplicationVersion = (string.IsNullOrEmpty(text) ? new Version() : new Version(text));
			}
		}
	}
}
