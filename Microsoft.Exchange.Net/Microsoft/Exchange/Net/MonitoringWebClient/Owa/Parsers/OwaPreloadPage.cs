using System;

namespace Microsoft.Exchange.Net.MonitoringWebClient.Owa.Parsers
{
	internal class OwaPreloadPage
	{
		public Uri CdnUri { get; protected set; }

		public static OwaPreloadPage Parse(HttpWebResponseWrapper response)
		{
			if (!response.Body.Contains(OwaPreloadPage.PageMarker))
			{
				throw new MissingKeywordException(MonitoringWebClientStrings.MissingPreloadPage(OwaPreloadPage.PageMarker), response.Request, response, OwaPreloadPage.PageMarker);
			}
			Uri cdnUri;
			if (!Uri.TryCreate(ParsingUtility.ParseJavascriptStringVariable(response, "basePath"), UriKind.Absolute, out cdnUri))
			{
				throw new MissingKeywordException(MonitoringWebClientStrings.BadPreloadPath("basePath"), response.Request, response, "basePath");
			}
			return new OwaPreloadPage
			{
				CdnUri = cdnUri
			};
		}

		private static string PageMarker = "{19A79CE9-889B-4145-2C4C-474C37DC7B4F}";
	}
}
