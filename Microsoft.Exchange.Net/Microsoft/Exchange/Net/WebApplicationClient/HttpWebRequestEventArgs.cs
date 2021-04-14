using System;
using System.Net;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Net.WebApplicationClient
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	public class HttpWebRequestEventArgs : EventArgs
	{
		public HttpWebRequestEventArgs(HttpWebRequest request)
		{
			this.Request = request;
		}

		public HttpWebRequest Request { get; private set; }
	}
}
