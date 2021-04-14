using System;
using System.Net;

namespace Microsoft.Exchange.Services.OnlineMeetings.Autodiscover
{
	internal class AutodiscoverError
	{
		public AutodiscoverError(AutodiscoverStep step, Exception exception)
		{
			this.failureStep = step;
			this.exception = exception;
		}

		public AutodiscoverError(AutodiscoverStep step, Exception exception, WebRequest request, WebResponse response) : this(step, exception)
		{
			if (request != null)
			{
				this.RequestHeaders = request.GetRequestHeadersAsString();
			}
			if (response != null)
			{
				this.ResponseHeaders = response.GetResponseHeadersAsString();
				this.ResponseBody = response.GetResponseBodyAsString();
				this.ResponseFailureReason = response.GetReasonHeader();
			}
		}

		public AutodiscoverStep FailureStep
		{
			get
			{
				return this.failureStep;
			}
		}

		public Exception Exception
		{
			get
			{
				return this.exception;
			}
		}

		public string RequestHeaders { get; set; }

		public string ResponseHeaders { get; set; }

		public string ResponseBody { get; set; }

		public string ResponseFailureReason { get; set; }

		private readonly AutodiscoverStep failureStep;

		private readonly Exception exception;
	}
}
