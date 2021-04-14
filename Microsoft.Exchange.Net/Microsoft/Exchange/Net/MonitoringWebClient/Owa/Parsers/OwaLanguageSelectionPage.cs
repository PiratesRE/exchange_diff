using System;
using System.Net;

namespace Microsoft.Exchange.Net.MonitoringWebClient.Owa.Parsers
{
	internal class OwaLanguageSelectionPage
	{
		public Uri FinalUri { get; private set; }

		public string PostTarget { get; private set; }

		public string Destination { get; private set; }

		public static bool TryParse(HttpWebResponseWrapper response, out OwaLanguageSelectionPage result)
		{
			if (response.Body == null || response.Body.IndexOf("ASP.languageselection_aspx", StringComparison.OrdinalIgnoreCase) < 0)
			{
				result = null;
				return false;
			}
			result = new OwaLanguageSelectionPage();
			result.FinalUri = response.Request.RequestUri;
			result.PostTarget = ParsingUtility.ParseFormAction(response);
			result.Destination = ParsingUtility.ParseFormDestination(response);
			return true;
		}

		public static bool ContainsLanguagePageRedirection(HttpWebResponseWrapper response)
		{
			return response.StatusCode == HttpStatusCode.Found && response.Headers["Location"] != null && response.Headers["Location"].IndexOf("languageselection.aspx", StringComparison.OrdinalIgnoreCase) >= 0;
		}

		internal const string PageMarker = "ASP.languageselection_aspx";

		internal const string PageName = "languageselection.aspx";
	}
}
