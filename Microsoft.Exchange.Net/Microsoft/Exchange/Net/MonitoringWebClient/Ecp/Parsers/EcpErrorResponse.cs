using System;
using System.Text.RegularExpressions;

namespace Microsoft.Exchange.Net.MonitoringWebClient.Ecp.Parsers
{
	internal class EcpErrorResponse
	{
		public FailureReason FailureReason { get; private set; }

		public string ExceptionType { get; private set; }

		private EcpErrorResponse()
		{
		}

		internal EcpErrorResponse(FailureReason reason)
		{
			this.FailureReason = reason;
		}

		public static bool TryParse(HttpWebResponseWrapper response, out EcpErrorResponse errorPage)
		{
			errorPage = null;
			if (response.Body == null)
			{
				return false;
			}
			FailureReason failureReason;
			if (response.Body.IndexOf("6DD23A7E-5C94-4d52-B537-2EA53079B2D5", StringComparison.OrdinalIgnoreCase) >= 0)
			{
				failureReason = FailureReason.EcpErrorPage;
			}
			else if (response.Body.IndexOf("\"ErrorRecords\":[{", StringComparison.OrdinalIgnoreCase) >= 0)
			{
				failureReason = FailureReason.EcpJsonResultErrorResponse;
			}
			else
			{
				if (response.Headers["jsonerror"] == null || !response.Headers["jsonerror"].Equals("true", StringComparison.OrdinalIgnoreCase))
				{
					return false;
				}
				failureReason = FailureReason.EcpJsonErrorResponse;
			}
			string text = response.Headers["X-ECP-Error"];
			if (string.IsNullOrEmpty(text) && failureReason == FailureReason.EcpJsonResultErrorResponse)
			{
				Regex regex = new Regex("\"Type\":\"(?<ExceptionName>[^\"]*)\"", RegexOptions.IgnoreCase);
				if (regex.IsMatch(response.Body))
				{
					Match match = regex.Match(response.Body);
					text = match.Result("${ExceptionName}");
				}
				else
				{
					text = response.Body;
				}
			}
			if (!string.IsNullOrEmpty(text))
			{
				if (text.StartsWith("Microsoft.Exchange.Data.Directory", StringComparison.OrdinalIgnoreCase))
				{
					if (text.EndsWith("OverBudgetException", StringComparison.OrdinalIgnoreCase))
					{
						failureReason = FailureReason.EcpErrorPage;
					}
					else
					{
						failureReason = FailureReason.EcpActiveDirectoryErrorResponse;
					}
				}
				else if (text.StartsWith("Microsoft.Exchange.Data.Storage", StringComparison.OrdinalIgnoreCase))
				{
					failureReason = FailureReason.EcpMailboxErrorResponse;
				}
			}
			errorPage = new EcpErrorResponse();
			errorPage.FailureReason = failureReason;
			errorPage.ExceptionType = text;
			return true;
		}

		public override string ToString()
		{
			return string.Format("ErrorPageFailureReason: {0}, Exception type: {1}", this.FailureReason, this.ExceptionType);
		}

		internal const string EcpExceptionHeaderName = "X-ECP-Error";

		internal const string EcpErrorPageMarker = "6DD23A7E-5C94-4d52-B537-2EA53079B2D5";

		internal const string EcpJsonErrorHeaderName = "jsonerror";

		internal const string EcpJsonResultErrorMarker = "\"ErrorRecords\":[{";
	}
}
