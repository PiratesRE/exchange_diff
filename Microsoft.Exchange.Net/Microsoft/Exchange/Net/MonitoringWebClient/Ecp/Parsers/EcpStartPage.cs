using System;

namespace Microsoft.Exchange.Net.MonitoringWebClient.Ecp.Parsers
{
	internal class EcpStartPage
	{
		public Uri FinalUri { get; private set; }

		public Uri StaticFileUri { get; protected set; }

		protected Uri StaticFileLocalUri { get; set; }

		public static EcpStartPage Parse(HttpWebResponseWrapper response)
		{
			if (response.Body == null || response.Body.IndexOf("4818bc65-fef6-4044-bebf-48f21be0a9d3", StringComparison.OrdinalIgnoreCase) < 0)
			{
				throw new MissingKeywordException(MonitoringWebClientStrings.MissingEcpStartPage("4818bc65-fef6-4044-bebf-48f21be0a9d3"), response.Request, response, "4818bc65-fef6-4044-bebf-48f21be0a9d3");
			}
			EcpStartPage ecpStartPage = new EcpStartPage();
			foreach (string fileName in EcpStartPage.StaticFileNames)
			{
				string text = ParsingUtility.ParseFilePath(response, fileName);
				if (!string.IsNullOrEmpty(text))
				{
					ecpStartPage.StaticFileLocalUri = new Uri(text, UriKind.RelativeOrAbsolute);
					break;
				}
			}
			if (ecpStartPage.StaticFileLocalUri != null)
			{
				if (!ecpStartPage.StaticFileLocalUri.IsAbsoluteUri)
				{
					ecpStartPage.StaticFileUri = new Uri(response.Request.RequestUri, ecpStartPage.StaticFileLocalUri);
				}
				else
				{
					ecpStartPage.StaticFileUri = ecpStartPage.StaticFileLocalUri;
				}
			}
			ecpStartPage.FinalUri = response.Request.RequestUri;
			return ecpStartPage;
		}

		internal const string StartPageMarker = "4818bc65-fef6-4044-bebf-48f21be0a9d3";

		internal static string[] StaticFileNames = new string[]
		{
			"clear1x1.gif",
			"common.js"
		};
	}
}
