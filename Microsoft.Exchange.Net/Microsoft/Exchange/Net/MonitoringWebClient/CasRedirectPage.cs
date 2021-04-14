using System;
using System.Text.RegularExpressions;

namespace Microsoft.Exchange.Net.MonitoringWebClient
{
	internal class CasRedirectPage
	{
		public string TargetUrl { get; private set; }

		private CasRedirectPage()
		{
		}

		public static bool TryParse(HttpWebResponseWrapper response, out CasRedirectPage errorPage)
		{
			if (response.Body == null || response.Body.IndexOf("ASP.casredirect_aspx", StringComparison.OrdinalIgnoreCase) < 0)
			{
				errorPage = null;
				return false;
			}
			Regex regex = new Regex("window.location.href[\\s]*=[\\s]*\"(?<URL>[^\"]*)\"", RegexOptions.IgnoreCase);
			Match match = regex.Match(response.Body);
			string s = match.Result("${URL}");
			errorPage = new CasRedirectPage();
			errorPage.TargetUrl = ParsingUtility.JavascriptDecode(s);
			return true;
		}

		public override string ToString()
		{
			return string.Format("CasRedirectPage. Target URL: {0}", this.TargetUrl);
		}

		internal const string PageMarker = "ASP.casredirect_aspx";
	}
}
