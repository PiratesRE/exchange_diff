using System;

namespace Microsoft.Exchange.Net.MonitoringWebClient.Ecp.Parsers
{
	internal class EcpHelpDeskStartPage
	{
		public Uri FinalUri { get; private set; }

		public static EcpHelpDeskStartPage Parse(HttpWebResponseWrapper response)
		{
			if (response.Body == null || response.Body.IndexOf("4818bc65-fef6-4044-bebf-48f21be0a9d3", StringComparison.OrdinalIgnoreCase) < 0)
			{
				throw new MissingKeywordException(MonitoringWebClientStrings.MissingEcpStartPage("4818bc65-fef6-4044-bebf-48f21be0a9d3"), response.Request, response, "4818bc65-fef6-4044-bebf-48f21be0a9d3");
			}
			if (response.Body == null || response.Body.IndexOf("EsoBarMsg", StringComparison.Ordinal) < 0)
			{
				throw new MissingKeywordException(MonitoringWebClientStrings.MissingEcpStartPage("EsoBarMsg"), response.Request, response, "EsoBarMsg");
			}
			return new EcpHelpDeskStartPage
			{
				FinalUri = response.Request.RequestUri
			};
		}

		internal const string StartPageMarker = "4818bc65-fef6-4044-bebf-48f21be0a9d3";

		internal const string EsoMessageDivId = "EsoBarMsg";
	}
}
