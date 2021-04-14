using System;
using System.Net;

namespace Microsoft.Exchange.Net.WebApplicationClient
{
	internal class RedirectResponse : TextResponse
	{
		public RedirectResponse()
		{
		}

		public RedirectResponse(HttpWebResponse response)
		{
			this.SetResponse(response);
		}

		public override void SetResponse(HttpWebResponse response)
		{
			base.SetResponse(response);
			this.RedirectUrl = response.Headers[HttpResponseHeader.Location];
		}

		public bool IsRedirect
		{
			get
			{
				return base.StatusCode == HttpStatusCode.Found;
			}
		}

		public string RedirectUrl { get; private set; }
	}
}
