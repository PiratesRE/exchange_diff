using System;
using System.Net;

namespace Microsoft.Office.CompliancePolicy
{
	internal class HttpWebResponseEventArgs : HttpWebRequestEventArgs
	{
		public HttpWebResponseEventArgs(HttpWebRequest request, HttpWebResponse response) : base(request)
		{
			this.Response = response;
		}

		public HttpWebResponse Response { get; set; }
	}
}
