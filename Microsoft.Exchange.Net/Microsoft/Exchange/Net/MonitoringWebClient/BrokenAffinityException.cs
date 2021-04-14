using System;

namespace Microsoft.Exchange.Net.MonitoringWebClient
{
	internal class BrokenAffinityException : HttpWebResponseWrapperException
	{
		public BrokenAffinityException(string message, HttpWebRequestWrapper request, HttpWebResponseWrapper response, string hostname, string server1, string server2, Exception innerException) : base(message, request, response, innerException)
		{
			this.hostName = hostname;
			this.server1 = server1;
			this.server2 = server2;
		}

		public override string ExceptionHint
		{
			get
			{
				return string.Format("BrokenAffinityException: {0}, {1}, {2}", this.hostName, this.server1, this.server2);
			}
		}

		private readonly string hostName;

		private readonly string server1;

		private readonly string server2;
	}
}
