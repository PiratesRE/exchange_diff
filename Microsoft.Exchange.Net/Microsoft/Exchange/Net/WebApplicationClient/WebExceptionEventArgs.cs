using System;
using System.Net;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Net.WebApplicationClient
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	public class WebExceptionEventArgs : HttpWebResponseEventArgs
	{
		public WebExceptionEventArgs(HttpWebRequest request, WebException exception) : base(request, exception.Response as HttpWebResponse)
		{
			this.Exception = exception;
		}

		public WebException Exception { get; private set; }
	}
}
