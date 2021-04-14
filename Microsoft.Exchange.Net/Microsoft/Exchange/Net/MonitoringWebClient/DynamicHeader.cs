using System;
using System.Net;

namespace Microsoft.Exchange.Net.MonitoringWebClient
{
	public class DynamicHeader
	{
		public DynamicHeader(string name, Func<Uri, string> valueDelegate)
		{
			this.name = name;
			this.valueDelegate = valueDelegate;
		}

		internal void UpdateHeaders(HttpWebRequestWrapper request)
		{
			string text = this.valueDelegate(request.RequestUri);
			if (text != null)
			{
				request.Headers[this.name] = text;
			}
		}

		internal void UpdateHeaders(HttpWebRequest request)
		{
			Uri arg = new Uri(request.RequestUri.ToString().Replace(request.RequestUri.Host, request.Host));
			string text = this.valueDelegate(arg);
			if (text != null)
			{
				request.Headers[this.name] = text;
			}
		}

		private readonly string name;

		private Func<Uri, string> valueDelegate;
	}
}
