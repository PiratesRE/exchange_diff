using System;
using System.Net;

namespace Microsoft.Exchange.Monitoring
{
	public class LyncAutodiscoverRequestState
	{
		public HttpWebRequest Request { get; set; }

		public HttpWebResponse Response { get; set; }

		public string TargetUrl { get; set; }

		public bool IsRedirect { get; set; }
	}
}
