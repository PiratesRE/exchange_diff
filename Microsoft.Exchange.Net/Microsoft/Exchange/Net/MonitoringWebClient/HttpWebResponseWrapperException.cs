using System;
using System.Net;
using System.Text;

namespace Microsoft.Exchange.Net.MonitoringWebClient
{
	internal class HttpWebResponseWrapperException : Exception
	{
		public HttpWebRequestWrapper Request { get; private set; }

		public HttpWebResponseWrapper Response { get; private set; }

		public WebExceptionStatus? Status { get; private set; }

		public virtual string ExceptionHint
		{
			get
			{
				return "HttpWebResponseException: " + this.Status;
			}
		}

		public HttpWebResponseWrapperException(string message, HttpWebRequestWrapper request, HttpWebResponseWrapper response) : base(message)
		{
			this.Request = request;
			this.Response = response;
			this.Status = new WebExceptionStatus?(WebExceptionStatus.Success);
		}

		public HttpWebResponseWrapperException(string message, HttpWebRequestWrapper request, HttpWebResponseWrapper response, WebExceptionStatus status) : this(message, request, response)
		{
			this.Status = new WebExceptionStatus?(status);
		}

		public HttpWebResponseWrapperException(string message, HttpWebRequestWrapper request, HttpWebResponseWrapper response, Exception innerException) : this(message, request, innerException)
		{
			this.Response = response;
		}

		public HttpWebResponseWrapperException(string message, HttpWebRequestWrapper request, Exception innerException) : base(message, innerException)
		{
			this.Request = request;
		}

		public override string Message
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendFormat("{0}: {1}{2}", base.GetType().FullName, base.Message, Environment.NewLine);
				if (this.Status != null)
				{
					stringBuilder.AppendFormat("WebExceptionStatus: {0}{1}", this.Status, Environment.NewLine);
				}
				if (this.Request != null)
				{
					stringBuilder.AppendFormat("{0}{1}{1}", this.Request.ToString(RequestResponseStringFormatOptions.TruncateCookies), Environment.NewLine);
				}
				if (this.Response != null)
				{
					stringBuilder.AppendFormat("{0}{1}{1}", this.Response.ToString(RequestResponseStringFormatOptions.TruncateCookies), Environment.NewLine);
				}
				return stringBuilder.ToString();
			}
		}
	}
}
