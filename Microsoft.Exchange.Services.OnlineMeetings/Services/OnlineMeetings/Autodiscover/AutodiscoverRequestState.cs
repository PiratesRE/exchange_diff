using System;
using System.Net;
using System.Threading;

namespace Microsoft.Exchange.Services.OnlineMeetings.Autodiscover
{
	internal class AutodiscoverRequestState
	{
		public AutodiscoverRequestState(HttpWebRequest request)
		{
			if (request == null)
			{
				throw new ArgumentNullException("request");
			}
			this.request = request;
			this.url = request.RequestUri.ToString();
			this.Result = new AnonymousAutodiscoverResult();
			this.Result.Redirects.Add(request.RequestUri.ToString());
			this.manualResetEvent = new ManualResetEvent(false);
		}

		public string Url
		{
			get
			{
				return this.url;
			}
		}

		public ManualResetEvent ManualResetEvent
		{
			get
			{
				return this.manualResetEvent;
			}
		}

		public HttpWebRequest Request
		{
			get
			{
				return this.request;
			}
		}

		public HttpWebResponse Response { get; set; }

		public bool HasException
		{
			get
			{
				return this.Result.HasException;
			}
		}

		public AnonymousAutodiscoverResult Result { get; set; }

		private readonly HttpWebRequest request;

		private readonly ManualResetEvent manualResetEvent;

		private readonly string url;
	}
}
