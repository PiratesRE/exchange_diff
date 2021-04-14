using System;

namespace Microsoft.Exchange.Net.MonitoringWebClient.Owa.Parsers
{
	internal class Owa14StartPage : OwaStartPage
	{
		protected override string StaticFileName
		{
			get
			{
				return "clear1x1.gif";
			}
		}

		public static bool TryParse(HttpWebResponseWrapper response, out Owa14StartPage result)
		{
			if (response.Body == null || response.Body.IndexOf("forms_premium_startpage_aspx", StringComparison.OrdinalIgnoreCase) < 0)
			{
				result = null;
				return false;
			}
			result = new Owa14StartPage();
			result.StaticFileLocalUri = new Uri(ParsingUtility.ParseFilePath(response, "clear1x1.gif"), UriKind.RelativeOrAbsolute);
			return true;
		}

		internal const string StartPageMarker = "forms_premium_startpage_aspx";

		private const string staticFileName = "clear1x1.gif";
	}
}
