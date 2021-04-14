using System;

namespace Microsoft.Exchange.Net.MonitoringWebClient.Owa.Parsers
{
	internal abstract class OwaStartPage
	{
		public Uri FinalUri { get; protected set; }

		public Uri StaticFileUri { get; protected set; }

		protected Uri StaticFileLocalUri { get; set; }

		protected abstract string StaticFileName { get; }

		public static OwaStartPage Parse(HttpWebResponseWrapper response)
		{
			Owa14StartPage owa14StartPage = null;
			Owa15StartPage owa15StartPage = null;
			OwaStartPage owaStartPage;
			if (Owa14StartPage.TryParse(response, out owa14StartPage))
			{
				owaStartPage = owa14StartPage;
			}
			else
			{
				if (!Owa15StartPage.TryParse(response, out owa15StartPage))
				{
					throw new MissingKeywordException(MonitoringWebClientStrings.MissingOwaStartPage(string.Format("{0},{1}", "forms_premium_startpage_aspx", "Program.main")), response.Request, response, "OWA start page");
				}
				owaStartPage = owa15StartPage;
			}
			if (owaStartPage.StaticFileLocalUri == null)
			{
				throw new MissingKeywordException(MonitoringWebClientStrings.MissingStaticFile(owaStartPage.StaticFileName), response.Request, response, "OWA static files");
			}
			owaStartPage.FinalUri = response.Request.RequestUri;
			if (!owaStartPage.StaticFileLocalUri.IsAbsoluteUri)
			{
				owaStartPage.StaticFileUri = new Uri(response.Request.RequestUri, owaStartPage.StaticFileLocalUri);
			}
			else
			{
				owaStartPage.StaticFileUri = owaStartPage.StaticFileLocalUri;
			}
			return owaStartPage;
		}
	}
}
