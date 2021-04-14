using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Threading;
using System.Web;
using Microsoft.Exchange.Clients.Owa2.Server.Core;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public class LanguageSelection : OwaPage
	{
		protected string Destination
		{
			get
			{
				return Utilities.GetQueryStringParameter(base.Request, "url", false);
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(base.Request.QueryString.ToString());
			string text = nameValueCollection.Get("url");
			Uri uri;
			if (text != null && !Uri.TryCreate(text, UriKind.Relative, out uri))
			{
				base.Response.Redirect(this.deleteURLParam(nameValueCollection));
			}
		}

		protected override bool UseStrictMode
		{
			get
			{
				return false;
			}
		}

		protected bool ShowAccessibilityOption
		{
			get
			{
				return !Utilities.IsEcpUrl(this.Destination);
			}
		}

		protected string PageTitle
		{
			get
			{
				return LocalizedStrings.GetHtmlEncoded(Utilities.IsEacUrl(this.Destination) ? 1018921346 : -1066333875);
			}
		}

		protected string SignInHeader
		{
			get
			{
				return LocalizedStrings.GetHtmlEncoded(Utilities.IsEacUrl(this.Destination) ? 1018921346 : -740205329);
			}
		}

		protected bool IsEcpDestination
		{
			get
			{
				return Utilities.IsEcpUrl(this.Destination);
			}
		}

		public override string OwaVersion
		{
			get
			{
				return OwaVersionId.Current;
			}
		}

		protected void RenderLanguageSelection()
		{
			string queryStringParameter = Utilities.GetQueryStringParameter(base.Request, "ll-cc", false);
			int lcid;
			if (!int.TryParse(queryStringParameter, out lcid))
			{
				lcid = Thread.CurrentThread.CurrentUICulture.LCID;
			}
			base.Response.Write("<select name=");
			base.Response.Write("lcid");
			base.Response.Write(" class=languageInputText>");
			CultureInfo[] supportedCultures = Microsoft.Exchange.Clients.Owa.Core.Culture.GetSupportedCultures(true);
			for (int i = 0; i < supportedCultures.Length; i++)
			{
				base.Response.Write("<option");
				if (supportedCultures[i].LCID == lcid)
				{
					base.Response.Write(" selected");
				}
				base.Response.Write(" value=\"");
				base.Response.Write(supportedCultures[i].LCID);
				base.Response.Write("\">");
				string s = supportedCultures[i].NativeName;
				if (supportedCultures[i].LCID == LanguageSelection.lcidForSrCyrlCS)
				{
					s = "српски (ћирилица, Србија и Црна Гора (бивша))";
				}
				else if (supportedCultures[i].LCID == LanguageSelection.lcidForSrLatnCS)
				{
					s = "srpski (latinica, Srbija i Crna Gora (bivša))";
				}
				Utilities.RenderDirectionEnhancedValue(base.Response.Output, Utilities.HtmlEncode(s), supportedCultures[i].TextInfo.IsRightToLeft);
				base.Response.Write("</option>");
			}
			base.Response.Write("</select>");
		}

		protected void RenderTimeZoneSelection()
		{
			base.Response.Write("<select id=selTz name=");
			base.Response.Write("tzid");
			base.Response.Write(" class=languageInputText onchange=\"isTimezoneSelectedCheck()\">");
			base.Response.Write(" <option selected=\"selected\">");
			base.Response.Write(LocalizedStrings.GetHtmlEncoded(394323495));
			base.Response.Write("</option> ");
			foreach (ExTimeZone exTimeZone in ExTimeZoneEnumerator.Instance)
			{
				base.Response.Write("<option");
				base.Response.Write(" value=\"");
				Utilities.HtmlEncode(exTimeZone.Id, base.Response.Output);
				base.Response.Write("\">");
				Utilities.RenderDirectionEnhancedValue(base.Response.Output, exTimeZone.LocalizableDisplayName.ToString(Thread.CurrentThread.CurrentUICulture), OwaPage.IsRtl);
			}
			base.Response.Write("</select>");
		}

		protected string deleteURLParam(NameValueCollection queryStringParams)
		{
			queryStringParams.Remove("url");
			string str = string.Empty;
			if (queryStringParams.Count > 0)
			{
				str = "?" + queryStringParams.ToString();
			}
			return base.Request.Url.AbsolutePath + str;
		}

		private const string DestinationParameter = "url";

		private const string LcidParameter = "ll-cc";

		private const string SrLatnCSNativeNameSuffixedWithFormer = "srpski (latinica, Srbija i Crna Gora (bivša))";

		private const string SrCyrlCSNativeNameSuffixedWithFormer = "српски (ћирилица, Србија и Црна Гора (бивша))";

		private static readonly int lcidForSrCyrlCS = new CultureInfo("sr-Cyrl-CS").LCID;

		private static readonly int lcidForSrLatnCS = new CultureInfo("sr-Latn-CS").LCID;
	}
}
