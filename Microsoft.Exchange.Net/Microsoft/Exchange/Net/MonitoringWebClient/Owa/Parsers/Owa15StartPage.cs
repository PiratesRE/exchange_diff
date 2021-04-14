using System;

namespace Microsoft.Exchange.Net.MonitoringWebClient.Owa.Parsers
{
	internal class Owa15StartPage : OwaStartPage
	{
		protected override string StaticFileName
		{
			get
			{
				return string.Format("{0}, {1}", "microsoft.exchange.clients.owa2.client.core.framework.js", "preboot.js");
			}
		}

		public static bool TryParse(HttpWebResponseWrapper response, out Owa15StartPage result)
		{
			if (response.Body == null)
			{
				result = null;
				return false;
			}
			if (response.Body.IndexOf("Program.main", StringComparison.OrdinalIgnoreCase) < 0 && response.Body.IndexOf("StartPage.start", StringComparison.OrdinalIgnoreCase) < 0)
			{
				result = null;
				return false;
			}
			result = new Owa15StartPage();
			string text = ParsingUtility.ParseFilePath(response, "preboot.js");
			if (string.IsNullOrEmpty(text))
			{
				text = ParsingUtility.ParseFilePath(response, "preboot.0.js");
			}
			if (string.IsNullOrEmpty(text))
			{
				text = ParsingUtility.ParseFilePath(response, "microsoft.exchange.clients.owa2.client.core.framework.js");
			}
			if (string.IsNullOrEmpty(text))
			{
				result.StaticFileLocalUri = null;
			}
			else
			{
				result.StaticFileLocalUri = new Uri(text, UriKind.RelativeOrAbsolute);
			}
			return true;
		}

		internal const string Df8StartPageMarker = "StartPage.start";

		internal const string StartPageMarker = "Program.main";

		private const string gaStaticFileName = "preboot.js";

		private const string gaStaticFileName2 = "preboot.0.js";

		private const string rtmStaticFileName = "microsoft.exchange.clients.owa2.client.core.framework.js";
	}
}
