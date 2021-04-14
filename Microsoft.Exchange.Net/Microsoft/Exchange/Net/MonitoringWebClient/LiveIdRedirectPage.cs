using System;
using System.Text.RegularExpressions;

namespace Microsoft.Exchange.Net.MonitoringWebClient
{
	internal class LiveIdRedirectPage
	{
		public string TargetUrl { get; private set; }

		private LiveIdRedirectPage()
		{
		}

		public static bool TryParse(HttpWebResponseWrapper response, out LiveIdRedirectPage errorPage)
		{
			if (response.Body == null || response.Body.IndexOf("onload=\"javascript:rd();\"", StringComparison.OrdinalIgnoreCase) < 0)
			{
				errorPage = null;
				return false;
			}
			Regex regex = new Regex("window.location.replace\\(\"(?<URL>[^\"]*)\"\\)", RegexOptions.IgnoreCase);
			Match match = regex.Match(response.Body);
			if (!match.Success)
			{
				errorPage = null;
				return false;
			}
			string s = match.Result("${URL}");
			errorPage = new LiveIdRedirectPage();
			errorPage.TargetUrl = ParsingUtility.JavascriptDecode(s);
			return true;
		}

		public override string ToString()
		{
			return string.Format("LiveIdRedirectPage. Target URL: {0}", this.TargetUrl);
		}

		internal const string PageMarker = "onload=\"javascript:rd();\"";
	}
}
