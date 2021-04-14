using System;
using System.Net;

namespace Microsoft.Exchange.Net.MonitoringWebClient.Owa.Parsers
{
	internal class OwaErrorPage
	{
		public FailureReason FailureReason { get; private set; }

		public string ExceptionType { get; private set; }

		private OwaErrorPage()
		{
		}

		internal OwaErrorPage(FailureReason reason)
		{
			this.FailureReason = reason;
		}

		public static bool TryParse(HttpWebResponseWrapper response, out OwaErrorPage errorPage)
		{
			if (!OwaErrorPage.ContainsErrorPage(response))
			{
				errorPage = null;
				return false;
			}
			FailureReason failureReason = FailureReason.OwaErrorPage;
			string text = response.Headers["X-Mserv-Error"];
			string text2 = string.Empty;
			if (!string.IsNullOrEmpty(text))
			{
				text2 = text;
				failureReason = FailureReason.OwaMServErrorPage;
			}
			else
			{
				text2 = response.Headers["X-OWA-Error"];
				if (string.IsNullOrEmpty(text2))
				{
					text2 = response.Headers["X-Auth-Error"];
				}
				if (!string.IsNullOrEmpty(text2))
				{
					if (text2.StartsWith("Microsoft.Exchange.Net.Mserve", StringComparison.OrdinalIgnoreCase) || text2.StartsWith("Microsoft.Exchange.Data.Directory.SystemConfiguration.InvalidPartnerIdException", StringComparison.OrdinalIgnoreCase))
					{
						failureReason = FailureReason.OwaMServErrorPage;
					}
					else if (text2.StartsWith("Microsoft.Exchange.Data.Storage.IllegalCrossServerConnectionException", StringComparison.OrdinalIgnoreCase) || text2.StartsWith("Microsoft.Exchange.Data.Storage.WrongServerException", StringComparison.OrdinalIgnoreCase))
					{
						failureReason = FailureReason.PassiveDatabase;
					}
					else if (text2.StartsWith("Microsoft.Exchange.Data.Storage", StringComparison.OrdinalIgnoreCase))
					{
						failureReason = FailureReason.OwaMailboxErrorPage;
					}
					else if (text2.StartsWith("Microsoft.Exchange.Data.Directory", StringComparison.OrdinalIgnoreCase) || text2.StartsWith("Microsoft.Exchange.Security.Authorization.AuthzException", StringComparison.OrdinalIgnoreCase) || text2.StartsWith("LoadOrganzationProperties", StringComparison.OrdinalIgnoreCase) || text2.StartsWith("Microsoft.Exchange.Clients.Owa.Core.OwaCreateClientSecurityContextFailedException", StringComparison.OrdinalIgnoreCase))
					{
						failureReason = FailureReason.OwaActiveDirectoryErrorPage;
					}
				}
			}
			errorPage = new OwaErrorPage();
			errorPage.FailureReason = failureReason;
			errorPage.ExceptionType = text2;
			return true;
		}

		private static bool ContainsErrorPage(HttpWebResponseWrapper response)
		{
			return (response.Headers["X-OWA-Error"] != null && (response.StatusCode != HttpStatusCode.Found || (response.StatusCode == HttpStatusCode.Found && response.Headers["Location"].IndexOf("errorfe.aspx", StringComparison.OrdinalIgnoreCase) >= 0))) || (response.Body != null && (response.Body.IndexOf("698798E9-889B-4145-ACFC-474C378C7B4F", StringComparison.OrdinalIgnoreCase) >= 0 || response.Body.IndexOf("FDCB17B5-BE4A-492D-BA37-A74E4E1AF7BF", StringComparison.OrdinalIgnoreCase) >= 0));
		}

		public override string ToString()
		{
			return string.Format("ErrorPageFailureReason: {0}, Exception type: {1}", this.FailureReason, this.ExceptionType);
		}

		internal const string ExceptionHeaderName = "X-OWA-Error";

		internal const string MservExceptionHeaderName = "X-Mserv-Error";

		internal const string OwaErrorPageMarker = "698798E9-889B-4145-ACFC-474C378C7B4F";

		internal const string OwaLiveIdErrorPageMarker = "FDCB17B5-BE4A-492D-BA37-A74E4E1AF7BF";
	}
}
