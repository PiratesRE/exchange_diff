using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Services.OnlineMeetings.Autodiscover
{
	internal class AnonymousAutodiscoverResult
	{
		internal AnonymousAutodiscoverResult()
		{
			this.Redirects = new List<string>();
		}

		public string AuthenticatedServerUri { get; set; }

		public List<string> Redirects { get; set; }

		public Exception Exception { get; set; }

		public bool HasException
		{
			get
			{
				return this.Exception != null;
			}
		}

		public string RequestHeaders { get; set; }

		public string ResponseHeaders { get; set; }

		public string ResponseBody { get; set; }
	}
}
