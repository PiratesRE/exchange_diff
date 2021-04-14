using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using Microsoft.Exchange.Clients;
using Microsoft.Exchange.Clients.Common;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Security;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Net.Protocols;

namespace Microsoft.Exchange.HttpProxy
{
	public class ErrorFE : OwaPage
	{
		protected bool HasErrorDetails
		{
			get
			{
				return this.errorInformation.MessageDetails != null;
			}
		}

		protected bool IsPreviousPageLinkEnabled
		{
			get
			{
				return !string.IsNullOrEmpty(this.errorInformation.PreviousPageUrl);
			}
		}

		protected bool IsExternalLinkPresent
		{
			get
			{
				return !string.IsNullOrEmpty(this.errorInformation.ExternalPageLink);
			}
		}

		protected bool IsOfflineEnabledClient
		{
			get
			{
				if (HttpContext.Current != null && HttpContext.Current.Request != null)
				{
					HttpCookie httpCookie = HttpContext.Current.Request.Cookies.Get("offline");
					if (httpCookie != null && httpCookie.Value == "1")
					{
						return true;
					}
				}
				return false;
			}
		}

		protected bool RenderAddToFavoritesButton
		{
			get
			{
				return !string.IsNullOrEmpty(this.errorInformation.RedirectionUrl) && this.isIE;
			}
		}

		protected Microsoft.Exchange.Clients.Owa.Core.ErrorInformation ErrorInformation
		{
			get
			{
				return this.errorInformation;
			}
		}

		protected bool RenderDiagnosticInfo
		{
			get
			{
				return this.renderDiagnosticInfo;
			}
		}

		protected string DiagnosticInfo
		{
			get
			{
				return this.diagnosticInfo;
			}
		}

		protected string ResourcePath
		{
			get
			{
				if (this.resourcePath == null)
				{
					this.resourcePath = OwaUrl.AuthFolder.ImplicitUrl;
				}
				return this.resourcePath;
			}
		}

		protected string LoadFailedMessageValue
		{
			get
			{
				string text = this.errorInformation.HttpCode.ToString();
				if (this.errorInformation.MessageDetails != null)
				{
					text = text + ":" + this.errorInformation.MessageDetails;
				}
				return text;
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			ErrorFE.FEErrorCodes feerrorCodes = ErrorFE.FEErrorCodes.Unknown;
			string text = null;
			if (HttpContext.Current != null && HttpContext.Current.Items != null)
			{
				if (HttpContext.Current.Items.Contains("CafeError"))
				{
					feerrorCodes = (ErrorFE.FEErrorCodes)HttpContext.Current.Items["CafeError"];
				}
				if (HttpContext.Current.Items.Contains("redirectUrl"))
				{
					text = (HttpContext.Current.Items["redirectUrl"] as string);
				}
			}
			string s;
			int httpCode;
			if (string.IsNullOrEmpty(s = HttpContext.Current.Request.QueryString["httpCode"]) || !int.TryParse(s, out httpCode))
			{
				if (!string.IsNullOrEmpty(text))
				{
					httpCode = 302;
				}
				else
				{
					httpCode = 500;
				}
			}
			bool sharePointApp;
			if (!bool.TryParse(HttpContext.Current.Request.QueryString["sharepointapp"], out sharePointApp))
			{
				sharePointApp = false;
			}
			bool siteMailbox;
			if (!bool.TryParse(HttpContext.Current.Request.QueryString["sm"], out siteMailbox))
			{
				siteMailbox = false;
			}
			ErrorMode value;
			if (!Enum.TryParse<ErrorMode>(HttpContext.Current.Request.QueryString["m"], out value))
			{
				value = ErrorMode.Frowny;
			}
			string groupMailboxDestination = HttpContext.Current.Request.QueryString["gm"];
			this.errorInformation = new Microsoft.Exchange.Clients.Owa.Core.ErrorInformation(httpCode, feerrorCodes.ToString(), sharePointApp);
			this.errorInformation.SiteMailbox = siteMailbox;
			this.errorInformation.GroupMailboxDestination = groupMailboxDestination;
			this.errorInformation.RedirectionUrl = text;
			this.errorInformation.ErrorMode = new ErrorMode?(value);
			this.isIE = (BrowserType.IE == Utilities.GetBrowserType(this.Context.Request.UserAgent));
			this.CompileDiagnosticInfo();
			this.AddDiagnosticsHeaders();
			this.OnInit(e);
		}

		protected void RenderTitle()
		{
			if (this.errorInformation.ErrorMode == ErrorMode.MailboxNotReady)
			{
				base.Response.Write(LocalizedStrings.GetHtmlEncoded(-594631022));
				return;
			}
			base.Response.Write(LocalizedStrings.GetHtmlEncoded(933672694));
		}

		protected void RenderIcon()
		{
			ThemeManager.RenderBaseThemeFileUrl(base.Response.Output, this.errorInformation.Icon, false);
		}

		protected void AddDiagnosticsHeaders()
		{
			if (!string.IsNullOrWhiteSpace(HttpContext.Current.Request.QueryString["owaError"]))
			{
				base.Response.AddHeader("X-OWA-Error", HttpContext.Current.Request.QueryString["owaError"]);
			}
			if (!string.IsNullOrWhiteSpace(HttpContext.Current.Request.QueryString["authError"]))
			{
				base.Response.AddHeader("X-Auth-Error", HttpContext.Current.Request.QueryString["authError"]);
			}
			if (!string.IsNullOrWhiteSpace(HttpContext.Current.Request.QueryString["owaVer"]))
			{
				base.Response.AddHeader("X-OWA-Version", HttpContext.Current.Request.QueryString["owaVer"]);
			}
			if (!string.IsNullOrWhiteSpace(Environment.MachineName))
			{
				base.Response.AddHeader(WellKnownHeader.XFEServer, Environment.MachineName);
			}
			if (!string.IsNullOrWhiteSpace(HttpContext.Current.Request.QueryString["be"]))
			{
				base.Response.AddHeader(WellKnownHeader.XBEServer, HttpContext.Current.Request.QueryString["be"]);
			}
		}

		protected void CompileDiagnosticInfo()
		{
			this.renderDiagnosticInfo = false;
			StringBuilder stringBuilder = new StringBuilder();
			if (HttpContext.Current.Request.Cookies["ClientId"] != null)
			{
				this.renderDiagnosticInfo = true;
				stringBuilder.Append("X-ClientId: ");
				string value = HttpContext.Current.Request.Cookies["ClientId"].Value;
				string text = ClientIdCookie.ParseToPrintableString(value);
				stringBuilder.Append(text.ToUpperInvariant());
				stringBuilder.Append("\n");
			}
			if (!string.IsNullOrWhiteSpace(HttpContext.Current.Request.QueryString["owaError"]))
			{
				this.renderDiagnosticInfo = true;
				stringBuilder.Append("X-OWA-Error: ");
				stringBuilder.Append(HttpContext.Current.Request.QueryString["owaError"]);
				stringBuilder.Append("\n");
			}
			if (!string.IsNullOrWhiteSpace(HttpContext.Current.Request.QueryString["authError"]))
			{
				this.renderDiagnosticInfo = true;
				stringBuilder.Append("X-Auth-Error: ");
				stringBuilder.Append(HttpContext.Current.Request.QueryString["authError"]);
				stringBuilder.Append("\n");
			}
			if (!string.IsNullOrWhiteSpace(HttpContext.Current.Request.QueryString["owaVer"]))
			{
				this.renderDiagnosticInfo = true;
				stringBuilder.Append("X-OWA-Version: ");
				stringBuilder.Append(HttpContext.Current.Request.QueryString["owaVer"]);
				stringBuilder.Append("\n");
			}
			if (!string.IsNullOrWhiteSpace(Environment.MachineName))
			{
				this.renderDiagnosticInfo = true;
				stringBuilder.Append("X-FEServer: ");
				stringBuilder.Append(Environment.MachineName);
				stringBuilder.Append("\n");
			}
			if (!string.IsNullOrWhiteSpace(HttpContext.Current.Request.QueryString["be"]))
			{
				this.renderDiagnosticInfo = true;
				stringBuilder.Append("X-BEServer: ");
				stringBuilder.Append(HttpContext.Current.Request.QueryString["be"]);
				stringBuilder.Append("\n");
			}
			long fileTime;
			if (long.TryParse(HttpContext.Current.Request.QueryString["ts"], out fileTime))
			{
				this.renderDiagnosticInfo = true;
				stringBuilder.Append("Date: ");
				stringBuilder.Append(DateTime.FromFileTimeUtc(fileTime).ToString());
				stringBuilder.Append("\n");
			}
			else if (this.renderDiagnosticInfo)
			{
				stringBuilder.Append("Date: ");
				stringBuilder.Append(DateTime.UtcNow.ToString());
				stringBuilder.Append("\n");
			}
			this.diagnosticInfo = stringBuilder.ToString();
		}

		protected void RenderErrorHeader()
		{
			if (!this.errorInformation.SharePointApp)
			{
				if (this.errorInformation.GroupMailbox)
				{
					return;
				}
				if (this.errorInformation.ErrorMode == ErrorMode.MailboxNotReady)
				{
					return;
				}
				if (this.errorInformation.HttpCode == 404)
				{
					base.Response.Write(LocalizedStrings.GetHtmlEncoded(-392503097));
					return;
				}
				if (this.errorInformation.HttpCode == 302)
				{
					return;
				}
				base.Response.Write(LocalizedStrings.GetHtmlEncoded(629133816));
			}
		}

		protected void RenderErrorSubHeader()
		{
			if (this.errorInformation.SharePointApp)
			{
				base.Response.Write(LocalizedStrings.GetHtmlEncoded(735230835));
				return;
			}
			if (this.errorInformation.GroupMailbox)
			{
				if (this.errorInformation.GroupMailboxDestination == "conv")
				{
					base.Response.Write(LocalizedStrings.GetHtmlEncoded(-526376074));
					return;
				}
				if (this.errorInformation.GroupMailboxDestination == "cal")
				{
					base.Response.Write(LocalizedStrings.GetHtmlEncoded(1147057944));
					return;
				}
			}
			else
			{
				if (this.errorInformation.ErrorMode == ErrorMode.MailboxNotReady)
				{
					base.Response.Write(LocalizedStrings.GetHtmlEncoded(-146632527));
					return;
				}
				if (this.errorInformation.ErrorMode == ErrorMode.MailboxSoftDeleted)
				{
					base.Response.Write(LocalizedStrings.GetHtmlEncoded(-1935911806));
					return;
				}
				if (this.errorInformation.ErrorMode == ErrorMode.AccountDisabled)
				{
					base.Response.Write(LocalizedStrings.GetHtmlEncoded(425733410));
					return;
				}
				if (this.errorInformation.ErrorMode == ErrorMode.SharedMailboxAccountDisabled)
				{
					base.Response.Write(LocalizedStrings.GetHtmlEncoded(-432125413));
					return;
				}
				if (this.errorInformation.HttpCode == 404)
				{
					base.Response.Write(LocalizedStrings.GetHtmlEncoded(1252002283));
					return;
				}
				if (this.errorInformation.HttpCode == 503)
				{
					base.Response.Write(LocalizedStrings.GetHtmlEncoded(1252002321));
					return;
				}
				if (this.errorInformation.HttpCode == 302)
				{
					return;
				}
				base.Response.Write(LocalizedStrings.GetHtmlEncoded(1252002318));
			}
		}

		protected void RenderErrorSubHeader2()
		{
		}

		protected void RenderRefreshButtonText()
		{
			if (this.errorInformation.ErrorMode == ErrorMode.MailboxNotReady)
			{
				base.Response.Write(LocalizedStrings.GetHtmlEncoded(867248262));
				return;
			}
			base.Response.Write(LocalizedStrings.GetHtmlEncoded(1939504838));
		}

		protected void RenderErrorDetails()
		{
			if (!this.errorInformation.GroupMailbox)
			{
				Strings.IDs ds;
				if (HttpContext.Current != null && HttpContext.Current.Request != null && HttpContext.Current.Request.QueryString["msg"] != null && Enum.TryParse<Strings.IDs>(HttpContext.Current.Request.QueryString["msg"], out ds))
				{
					string text = ErrorFE.SafeErrorMessagesNoHtmlEncoding.Contains(ds) ? Strings.GetLocalizedString(ds) : LocalizedStrings.GetHtmlEncoded(ds);
					List<string> list = Microsoft.Exchange.Clients.Common.ErrorInformation.ParseMessageParameters(text, HttpContext.Current.Request);
					if (list != null && list.Count > 0)
					{
						for (int i = 0; i < list.Count; i++)
						{
							list[i] = EncodingUtilities.HtmlEncode(list[i]);
						}
						if (ErrorFE.MessagesToRenderLogoutLinks.Contains(ds) || ErrorFE.MessagesToRenderLoginLinks.Contains(ds))
						{
							ErrorFE.AddSafeLinkToMessageParametersList(ds, HttpContext.Current.Request, ref list);
						}
						base.Response.Write(string.Format(text, list.ToArray()));
						return;
					}
					if (!ErrorFE.MessagesToRenderLogoutLinks.Contains(ds) && !ErrorFE.MessagesToRenderLoginLinks.Contains(ds))
					{
						base.Response.Write(text);
						return;
					}
					list = new List<string>();
					ErrorFE.AddSafeLinkToMessageParametersList(ds, HttpContext.Current.Request, ref list);
					if (list.Count > 0)
					{
						base.Response.Write(string.Format(text, list.ToArray()));
						return;
					}
				}
				else
				{
					if (this.errorInformation.HttpCode == 404)
					{
						base.Response.Write(LocalizedStrings.GetHtmlEncoded(236137810));
						return;
					}
					if (this.errorInformation.HttpCode == 302)
					{
						LegacyRedirectTypeOptions? legacyRedirectTypeOptions = HttpContext.Current.Items["redirectType"] as LegacyRedirectTypeOptions?;
						if (legacyRedirectTypeOptions == null || legacyRedirectTypeOptions != LegacyRedirectTypeOptions.Manual)
						{
							base.Response.Redirect(this.errorInformation.RedirectionUrl);
							return;
						}
						base.Response.Write(LocalizedStrings.GetHtmlEncoded(967320822));
						base.Response.Write("<br/>");
						base.Response.Write(string.Format("<a href=\"{0}\">{0}</a>", this.errorInformation.RedirectionUrl));
						base.Response.Headers.Add("X-OWA-FEError", ErrorFE.FEErrorCodes.CasRedirect.ToString());
						return;
					}
					else
					{
						base.Response.Write(LocalizedStrings.GetHtmlEncoded(236137783));
					}
				}
				return;
			}
			if (this.errorInformation.GroupMailboxDestination == "conv")
			{
				base.Response.Write(LocalizedStrings.GetHtmlEncoded(-364732161));
				return;
			}
			if (this.errorInformation.GroupMailboxDestination == "cal")
			{
				base.Response.Write(LocalizedStrings.GetHtmlEncoded(-292781713));
			}
		}

		protected void RenderOfflineInfo()
		{
			if (!this.IsOfflineEnabledClient)
			{
				base.Response.Write(LocalizedStrings.GetHtmlEncoded(-1051316555));
			}
		}

		protected void RenderOfflineDetails()
		{
			if (!this.IsOfflineEnabledClient)
			{
				string str;
				using (StringWriter stringWriter = new StringWriter())
				{
					ThemeManager.RenderBaseThemeFileUrl(stringWriter, ThemeFileId.OwaSettings, false);
					str = stringWriter.ToString();
				}
				string s = string.Format(LocalizedStrings.GetHtmlEncoded(510910463), "<img src=\"" + OwaUrl.AuthFolder.ImplicitUrl + str + "\"/>");
				base.Response.Write(LocalizedStrings.GetHtmlEncoded(107625936));
				base.Response.Write("<br/>");
				base.Response.Write(s);
				base.Response.Write("<br/>");
				base.Response.Write(LocalizedStrings.GetHtmlEncoded(-1055173478));
				base.Response.Write("<br/>");
				base.Response.Write(LocalizedStrings.GetHtmlEncoded(-295658591));
			}
		}

		protected void RenderExternalLink()
		{
			base.Response.Write(this.errorInformation.ExternalPageLink);
		}

		protected void RenderBackLink()
		{
			base.Response.Write(string.Format(LocalizedStrings.GetHtmlEncoded(161749640), "<a href=\"" + this.errorInformation.PreviousPageUrl + "\">", "</a>"));
		}

		protected void RenderBackground()
		{
			ThemeManager.RenderBaseThemeFileUrl(base.Response.Output, this.errorInformation.Background, false);
		}

		private static string GetLocalizedLiveIdSignoutLinkMessage(HttpRequest request)
		{
			string explicitUrl = OwaUrl.Logoff.GetExplicitUrl(request);
			return "<BR><BR>" + string.Format(CultureInfo.InvariantCulture, Strings.LogonErrorLogoutUrlText, new object[]
			{
				explicitUrl
			});
		}

		private static void AddSafeLinkToMessageParametersList(Strings.IDs messageId, HttpRequest request, ref List<string> messageParameters)
		{
			string item = string.Empty;
			string realm = string.Empty;
			if (ErrorFE.MessagesToRenderLogoutLinks.Contains(messageId))
			{
				item = ErrorFE.GetLocalizedLiveIdSignoutLinkMessage(request);
				messageParameters.Insert(0, item);
				return;
			}
			if (ErrorFE.MessagesToRenderLoginLinks.Contains(messageId))
			{
				string dnsSafeHost = request.Url.DnsSafeHost;
				if (messageParameters != null && messageParameters.Count > 0)
				{
					realm = messageParameters[0];
				}
				item = Utilities.GetAccessURLFromHostnameAndRealm(dnsSafeHost, realm, false);
				messageParameters.Insert(0, item);
				messageParameters.Remove(dnsSafeHost);
			}
		}

		internal const string RedirectUrl = "redirectUrl";

		internal const string RedirectType = "redirectType";

		private const string CafeErrorKey = "CafeError";

		private const string HttpCodeQueryKey = "httpCode";

		private const string ErrorMessageQueryKey = "msg";

		private const string ErrorMessageParameterQueryKey = "msgParam";

		private const string SharePointAppQueryKey = "sharepointapp";

		private const string SiteMailboxQueryKey = "sm";

		private const string GroupMailboxQueryKey = "gm";

		private const string ConversationsDestination = "conv";

		private const string CalendarDestination = "cal";

		private const string OfflineEnabledParameterName = "offline";

		private static readonly Strings.IDs[] SafeErrorMessagesNoHtmlEncoding = new Strings.IDs[]
		{
			1799660809,
			-1420330575,
			-870357301,
			637041586,
			-106213327,
			-2011393914,
			1317300008
		};

		private static readonly Strings.IDs[] MessagesToRenderLogoutLinks = new Strings.IDs[]
		{
			1799660809,
			-1420330575,
			-870357301,
			637041586,
			-106213327,
			-2011393914,
			1753500428
		};

		private static readonly Strings.IDs[] MessagesToRenderLoginLinks = new Strings.IDs[]
		{
			1317300008
		};

		private Microsoft.Exchange.Clients.Owa.Core.ErrorInformation errorInformation;

		private bool renderDiagnosticInfo;

		private string diagnosticInfo = string.Empty;

		private string resourcePath;

		private bool isIE = true;

		internal enum FEErrorCodes
		{
			Unknown,
			SSLCertificateProblem,
			CAS14WithNoWIA,
			NoFbaSSL,
			NoLegacyCAS,
			CasRedirect
		}
	}
}
