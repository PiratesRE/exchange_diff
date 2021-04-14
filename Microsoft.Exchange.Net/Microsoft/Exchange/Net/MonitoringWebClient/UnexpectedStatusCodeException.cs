using System;
using System.Net;
using System.Text;

namespace Microsoft.Exchange.Net.MonitoringWebClient
{
	internal class UnexpectedStatusCodeException : HttpWebResponseWrapperException
	{
		public HttpStatusCode[] ExpectedStatusCodes { get; private set; }

		public UnexpectedStatusCodeException(string message, HttpWebRequestWrapper request, HttpWebResponseWrapper response, HttpStatusCode[] expectedCodes, HttpStatusCode actualStatusCode) : base(message, request, response)
		{
			this.ExpectedStatusCodes = expectedCodes;
			this.actualStatusCode = actualStatusCode;
		}

		public override string Message
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(base.GetType().FullName + Environment.NewLine);
				StringBuilder stringBuilder2 = new StringBuilder();
				foreach (HttpStatusCode httpStatusCode in this.ExpectedStatusCodes)
				{
					if (stringBuilder2.Length != 0)
					{
						stringBuilder2.Append(", ");
					}
					stringBuilder2.Append(httpStatusCode.ToString());
				}
				stringBuilder.Append(MonitoringWebClientStrings.ExpectedStatusCode(stringBuilder2.ToString()));
				stringBuilder.Append(Environment.NewLine);
				stringBuilder.Append(MonitoringWebClientStrings.ActualStatusCode(this.actualStatusCode.ToString()));
				stringBuilder.Append(Environment.NewLine);
				stringBuilder.Append(Environment.NewLine);
				stringBuilder.Append(base.Message);
				return stringBuilder.ToString();
			}
		}

		public override string ExceptionHint
		{
			get
			{
				return "UnexpectedHttpCode: " + this.actualStatusCode.ToString();
			}
		}

		private HttpStatusCode actualStatusCode;
	}
}
