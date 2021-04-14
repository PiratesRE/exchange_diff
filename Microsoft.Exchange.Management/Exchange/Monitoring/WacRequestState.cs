using System;
using System.Net;
using System.Text;

namespace Microsoft.Exchange.Monitoring
{
	public class WacRequestState
	{
		public HttpWebRequest Request { get; set; }

		public HttpWebResponse Response { get; set; }

		public string WacIFrameUrl { get; set; }

		public string WopiUrl { get; set; }

		public bool Error { get; set; }

		public StringBuilder DiagnosticsDetails { get; set; }
	}
}
