using System;

namespace Microsoft.Exchange.Net.MonitoringWebClient
{
	internal class SslNegotiationException : HttpWebResponseWrapperException
	{
		public SslNegotiationException(string message, HttpWebRequestWrapper request, SslError sslError, Exception innerException) : base(message, request, innerException)
		{
			this.hostName = request.RequestUri.Host;
			this.sslError = sslError;
		}

		public SslErrorType SslErrorType
		{
			get
			{
				return this.sslError.SslErrorType;
			}
		}

		public override string ExceptionHint
		{
			get
			{
				return "SslNegotiationFailure: " + this.hostName;
			}
		}

		private readonly string hostName;

		private readonly SslError sslError;
	}
}
