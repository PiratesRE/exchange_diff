using System;

namespace Microsoft.Exchange.Net.MonitoringWebClient
{
	internal class NameResolutionException : HttpWebResponseWrapperException
	{
		public NameResolutionException(string message, HttpWebRequestWrapper request, Exception innerException, string hostName) : base(message, request, innerException)
		{
			this.hostName = hostName;
		}

		public override string ExceptionHint
		{
			get
			{
				return "NameResolutionFailure: " + this.hostName;
			}
		}

		private readonly string hostName;
	}
}
