using System;

namespace Microsoft.Exchange.Net.MonitoringWebClient
{
	internal class FbaRedirectPage
	{
		public string FbaLogonPagePathAndQuery { get; set; }

		public static FbaRedirectPage Parse(HttpWebResponseWrapper response)
		{
			FbaRedirectPage fbaRedirectPage = new FbaRedirectPage();
			if (response.Body == null || response.Body.IndexOf("{57A118C6-2DA9-419d-BE9A-F92B0F9A418B}", StringComparison.OrdinalIgnoreCase) < 0)
			{
				throw new MissingKeywordException(MonitoringWebClientStrings.MissingOwaFbaRedirectPage("{57A118C6-2DA9-419d-BE9A-F92B0F9A418B}"), response.Request, response, "{57A118C6-2DA9-419d-BE9A-F92B0F9A418B}");
			}
			fbaRedirectPage.FbaLogonPagePathAndQuery = "/owa/auth/" + ParsingUtility.ParseJavascriptStringVariable(response, "a_sLgn") + ParsingUtility.ParseJavascriptStringVariable(response, "a_sUrl");
			return fbaRedirectPage;
		}

		private const string RedirectPageMarker = "{57A118C6-2DA9-419d-BE9A-F92B0F9A418B}";
	}
}
