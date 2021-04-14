using System;
using System.Net;

namespace Microsoft.Exchange.Net.MonitoringWebClient.Rws.Parsers
{
	internal class RwsErrorResponse
	{
		public FailureReason FailureReason { get; private set; }

		public string ExceptionType { get; private set; }

		public HttpStatusCode StatusCode { get; private set; }

		private RwsErrorResponse()
		{
		}

		internal RwsErrorResponse(FailureReason reason)
		{
			this.FailureReason = reason;
		}

		public static bool TryParse(HttpWebResponseWrapper response, out RwsErrorResponse error)
		{
			error = null;
			if (response.StatusCode == HttpStatusCode.OK)
			{
				return false;
			}
			FailureReason failureReason = FailureReason.Unknown;
			string text = response.Headers["X-RWS-Error"];
			if (!string.IsNullOrEmpty(text))
			{
				if (text.StartsWith("Microsoft.Exchange.Data.Directory", StringComparison.OrdinalIgnoreCase))
				{
					failureReason = FailureReason.RwsActiveDirectoryErrorResponse;
				}
				else if (text.StartsWith("Microsoft.Exchange.Management.ReportingTask.ConnectionFailedException", StringComparison.OrdinalIgnoreCase))
				{
					failureReason = FailureReason.RwsDataMartErrorResponse;
				}
				else
				{
					failureReason = FailureReason.RwsError;
				}
			}
			error = new RwsErrorResponse();
			error.FailureReason = failureReason;
			error.ExceptionType = text;
			error.StatusCode = response.StatusCode;
			return true;
		}

		public override string ToString()
		{
			return string.Format("ErrorFailureReason: {0}, Exception type: {1}, Http status code: {2}", this.FailureReason, this.ExceptionType, this.StatusCode);
		}

		internal const string RwsExceptionHeaderName = "X-RWS-Error";
	}
}
