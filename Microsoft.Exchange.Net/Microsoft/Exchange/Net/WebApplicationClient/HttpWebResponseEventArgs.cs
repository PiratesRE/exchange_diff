using System;
using System.Net;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Net.WebApplicationClient
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	public class HttpWebResponseEventArgs : HttpWebRequestEventArgs
	{
		public HttpWebResponseEventArgs(HttpWebRequest request, HttpWebResponse response) : base(request)
		{
			this.Response = response;
		}

		public HttpWebResponse Response { get; set; }
	}
}
