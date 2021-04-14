using System;

namespace Microsoft.Exchange.Net.MonitoringWebClient
{
	internal class HttpWebEventArgs : EventArgs
	{
		public HttpWebEventArgs(HttpWebRequestWrapper request, object eventState) : this(request, null, eventState)
		{
		}

		public HttpWebEventArgs(HttpWebRequestWrapper request, HttpWebResponseWrapper response, object eventState)
		{
			this.Request = request;
			this.Response = response;
			this.EventState = eventState;
		}

		public HttpWebRequestWrapper Request { get; set; }

		public HttpWebResponseWrapper Response { get; set; }

		public object EventState { get; set; }
	}
}
