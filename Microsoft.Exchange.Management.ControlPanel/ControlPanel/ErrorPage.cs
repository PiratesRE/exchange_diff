using System;
using System.Globalization;
using System.Web;
using System.Web.UI.WebControls;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Management.ControlPanel;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class ErrorPage : EcpPage
	{
		protected bool ShowDebugInformation
		{
			get
			{
				return !string.IsNullOrEmpty(this.debugInformation);
			}
		}

		protected bool ShowDiagnosticInformation
		{
			get
			{
				return Datacenter.GetExchangeSku() == Datacenter.ExchangeSku.ExchangeDatacenter;
			}
		}

		private protected string SignOutReturnVdir { protected get; private set; }

		private protected bool ShowSignOutLink { protected get; private set; }

		private protected bool ShowSignOutHint { protected get; private set; }

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			this.EnsureChildControls();
			if (HttpContext.Current.Error != null)
			{
				this.debugInformation = HttpContext.Current.Error.ToTraceString();
				HttpContext.Current.ClearError();
			}
			if (HttpContext.Current.Request.ServerVariables["X-ECP-ERROR"] != null)
			{
				HttpContext.Current.Response.AddHeader("X-ECP-ERROR", HttpContext.Current.Request.ServerVariables["X-ECP-ERROR"]);
			}
			string text = base.Request.QueryString["cause"] ?? "unexpected";
			ErrorPageContents contentsForErrorType = ErrorPageContents.GetContentsForErrorType(text);
			string text2 = null;
			if (text == "browsernotsupported")
			{
				string helpId = EACHelpId.BrowserNotSupportedHelp.ToString();
				IThemable themable = this.Page as IThemable;
				if (themable != null && themable.FeatureSet == FeatureSet.Options)
				{
					helpId = OptionsHelpId.OwaOptionsBrowserNotSupportedHelp.ToString();
				}
				text2 = string.Format(contentsForErrorType.ErrorMessageText, HelpUtil.BuildEhcHref(helpId));
			}
			else if (text == "nocookies")
			{
				HttpBrowserCapabilities browser = HttpContext.Current.Request.Browser;
				if (browser != null && browser.IsBrowser("IE"))
				{
					text2 = string.Format(Strings.CookiesDisabledMessageForIE, HelpUtil.BuildEhcHref(EACHelpId.CookiesDisabledMessageForIE.ToString()));
				}
			}
			else if (text == "liveidmismatch")
			{
				string value = HttpContextExtensions.CurrentUserLiveID();
				if (string.IsNullOrEmpty(value))
				{
					contentsForErrorType = ErrorPageContents.GetContentsForErrorType("unexpected");
				}
				else
				{
					string arg = EcpUrl.EcpVDir + "logoff.aspx?src=exch&ru=" + HttpUtility.UrlEncode(HttpContext.Current.GetRequestUrl().OriginalString);
					text2 = string.Format(contentsForErrorType.ErrorMessageText, HttpContextExtensions.CurrentUserLiveID(), arg);
				}
			}
			else if (text == "verificationfailed")
			{
				text2 = string.Format(contentsForErrorType.ErrorMessageText, EcpUrl.EcpVDir);
			}
			else if (text == "verificationprocessingerror")
			{
				text2 = contentsForErrorType.ErrorMessageText;
			}
			else if (text == "noroles")
			{
				this.ShowSignOutHint = true;
				this.ShowSignOutLink = true;
				this.SignOutReturnVdir = "/ecp/";
			}
			else if (text == "cannotaccessoptionswithbeparamorcookie")
			{
				this.ShowSignOutLink = true;
				this.SignOutReturnVdir = "/owa/";
			}
			if (string.IsNullOrEmpty(text2))
			{
				this.msgText.Text = contentsForErrorType.ErrorMessageText;
			}
			else
			{
				this.msgText.Text = text2;
			}
			base.Title = contentsForErrorType.PageTitle;
			this.msgTitle.Text = Strings.ErrorTitle(contentsForErrorType.ErrorMessageTitle);
			this.msgCode.Text = ((int)contentsForErrorType.StatusCode).ToString(CultureInfo.InvariantCulture);
			HttpContext.Current.Response.StatusCode = (int)contentsForErrorType.StatusCode;
			HttpContext.Current.Response.SubStatusCode = contentsForErrorType.SubStatusCode;
			HttpContext.Current.Response.TrySkipIisCustomErrors = true;
			this.causeMarker.Text = "<!-- cause:" + contentsForErrorType.CauseMarker + " -->";
		}

		protected void RenderDebugInformation()
		{
			base.Response.Write(HttpUtility.HtmlEncode(this.debugInformation).Replace("&#13;&#10;", "<br />"));
		}

		protected void RenderDiagnosticInformation()
		{
			try
			{
				base.Response.Write("<div class=\"errLbl\">");
				base.Response.Write(Strings.UserEmailAddress);
				base.Response.Write("</div><div>");
				base.Response.Write(HttpUtility.HtmlEncode(HttpContextExtensions.CurrentUserLiveID()));
				base.Response.Write("</div>");
				string s = ActivityContext.ActivityId.FormatForLog();
				base.Response.Write("<div class=\"errLbl\">");
				base.Response.Write(Strings.CorrelationId);
				base.Response.Write("</div><div>");
				base.Response.Write(HttpUtility.HtmlEncode(s));
				base.Response.Write("</div>");
				ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 230, "RenderDiagnosticInformation", "f:\\15.00.1497\\sources\\dev\\admin\\src\\ecp\\error.aspx.cs");
				Server server = topologyConfigurationSession.FindLocalServer();
				base.Response.Write("<div class=\"errLbl\">");
				base.Response.Write(Strings.ClientAccessServerName);
				base.Response.Write("</div><div>");
				base.Response.Write(HttpUtility.HtmlEncode(server.Fqdn));
				base.Response.Write("</div>");
				base.Response.Write("<div class=\"errLbl\">");
				base.Response.Write(Strings.ClientAccessServerVersion);
				base.Response.Write("</div><div>");
				base.Response.Write(HttpUtility.HtmlEncode(Globals.ApplicationVersion));
				base.Response.Write("</div>");
				base.Response.Write("<div class=\"errLbl\">");
				base.Response.Write(Strings.UTCTime);
				base.Response.Write("</div><div>");
				base.Response.Write(HttpUtility.HtmlEncode(DateTime.UtcNow.ToString("o")));
				base.Response.Write("</div>");
			}
			catch (Exception exception)
			{
				ExTraceGlobals.EventLogTracer.TraceError<EcpTraceFormatter<Exception>>(0, 0L, "Application Error: {0}", exception.GetTraceFormatter());
				EcpPerfCounters.AspNetErrors.Increment();
				throw;
			}
		}

		protected Label msgCode;

		protected Label msgTitle;

		protected Literal msgText;

		protected Literal causeMarker;

		private string debugInformation;
	}
}
