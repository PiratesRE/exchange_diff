using System;
using System.Net;

namespace Microsoft.Office.CompliancePolicy
{
	internal class HttpWebRequestEventArgs : EventArgs
	{
		public HttpWebRequestEventArgs(HttpWebRequest request)
		{
			this.Request = request;
		}

		public HttpWebRequest Request { get; private set; }
	}
}
