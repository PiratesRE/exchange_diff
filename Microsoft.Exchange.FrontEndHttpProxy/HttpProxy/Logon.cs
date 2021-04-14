using System;
using Microsoft.Exchange.Clients;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.HttpProxy
{
	public class Logon : OwaPage
	{
		protected static string UserNameLabel
		{
			get
			{
				if (Datacenter.IsPartnerHostedOnly(false))
				{
					return LocalizedStrings.GetHtmlEncoded(1677919363);
				}
				switch (OwaVdirConfiguration.Instance.LogonFormat)
				{
				case LogonFormats.PrincipalName:
					return LocalizedStrings.GetHtmlEncoded(1677919363);
				case LogonFormats.UserName:
					return LocalizedStrings.GetHtmlEncoded(537815319);
				}
				return LocalizedStrings.GetHtmlEncoded(78658498);
			}
		}

		protected static string UserNamePlaceholder
		{
			get
			{
				if (Datacenter.IsPartnerHostedOnly(false))
				{
					return Strings.GetLocalizedString(1677919363);
				}
				switch (OwaVdirConfiguration.Instance.LogonFormat)
				{
				case LogonFormats.PrincipalName:
					return Strings.GetLocalizedString(-1896713583);
				case LogonFormats.UserName:
					return Strings.GetLocalizedString(-40289791);
				}
				return Strings.GetLocalizedString(609186145);
			}
		}

		protected bool ReplaceCurrent
		{
			get
			{
				string a = base.Request.QueryString["replaceCurrent"];
				return a == "1" || base.IsDownLevelClient;
			}
		}

		protected bool ShowOwaLightOption
		{
			get
			{
				return !UrlUtilities.IsEcpUrl(this.Destination) && OwaVdirConfiguration.Instance.LightSelectionEnabled;
			}
		}

		protected bool ShowPublicPrivateSelection
		{
			get
			{
				return OwaVdirConfiguration.Instance.PublicPrivateSelectionEnabled;
			}
		}

		protected override bool UseStrictMode
		{
			get
			{
				return false;
			}
		}

		protected string LogoffUrl
		{
			get
			{
				return base.Request.Url.Scheme + "://" + base.Request.Url.Authority + OwaUrl.Logoff.GetExplicitUrl(base.Request);
			}
		}

		protected Logon.LogonReason Reason
		{
			get
			{
				string text = base.Request.QueryString["reason"];
				if (text == null)
				{
					return Logon.LogonReason.None;
				}
				string key;
				switch (key = text)
				{
				case "1":
					return Logon.LogonReason.Logoff;
				case "2":
					return Logon.LogonReason.InvalidCredentials;
				case "3":
					return Logon.LogonReason.Timeout;
				case "4":
					return Logon.LogonReason.ChangePasswordLogoff;
				case "5":
					return Logon.LogonReason.BlockedByClientAccessRules;
				case "6":
					return Logon.LogonReason.EACBlockedByClientAccessRules;
				}
				return Logon.LogonReason.None;
			}
		}

		protected string Destination
		{
			get
			{
				string text = base.Request.QueryString["url"];
				if (text == null || string.Equals(text, this.LogoffUrl, StringComparison.Ordinal))
				{
					return base.Request.GetBaseUrl();
				}
				return text;
			}
		}

		protected string CloseWindowUrl
		{
			get
			{
				Uri uri;
				string result;
				if (Uri.TryCreate(this.Destination, UriKind.Absolute, out uri) && uri.AbsolutePath.EndsWith("/closewindow.aspx", StringComparison.OrdinalIgnoreCase))
				{
					result = this.Destination;
				}
				else
				{
					result = this.BaseUrl + "?ae=Dialog&t=CloseWindow&exsvurl=1";
				}
				return result;
			}
		}

		protected string PageTitle
		{
			get
			{
				return LocalizedStrings.GetHtmlEncoded(this.IsEcpDestination ? 1018921346 : -1066333875);
			}
		}

		protected string SignInHeader
		{
			get
			{
				return LocalizedStrings.GetHtmlEncoded(this.IsEcpDestination ? 1018921346 : -740205329);
			}
		}

		protected bool IsEcpDestination
		{
			get
			{
				return UrlUtilities.IsEacUrl(this.Destination);
			}
		}

		protected string LoadFailedMessageValue
		{
			get
			{
				return "logon page loaded";
			}
		}

		private string BaseUrl
		{
			get
			{
				return base.Request.Url.Scheme + "://" + base.Request.Url.Authority + OwaUrl.ApplicationRoot.GetExplicitUrl(base.Request);
			}
		}

		private string Default14Url
		{
			get
			{
				return base.Request.Url.Scheme + "://" + base.Request.Url.Authority + OwaUrl.Default14Page.GetExplicitUrl(base.Request);
			}
		}

		protected void RenderLogonHref()
		{
			base.Response.Write("logon.aspx?replaceCurrent=1");
			if (this.Reason != Logon.LogonReason.None)
			{
				base.Response.Write("&reason=");
				base.Response.Write((int)this.Reason);
			}
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.Response.Headers.Set("X-Frame-Options", "SAMEORIGIN");
			base.OnPreRender(e);
		}

		private const string Option = "<option value=\"{0}\"{1}>{2}</option>";

		private const string DestinationParameter = "url";

		private const string FlagsParameter = "flags";

		private const string LiveIdAuthenticationModuleSaveUrlOnLogoffParameter = "&exsvurl=1";

		private const string EcpCloseWindowUrl = "/closewindow.aspx";

		protected enum LogonReason
		{
			None,
			Logoff,
			InvalidCredentials,
			Timeout,
			ChangePasswordLogoff,
			BlockedByClientAccessRules,
			EACBlockedByClientAccessRules
		}
	}
}
