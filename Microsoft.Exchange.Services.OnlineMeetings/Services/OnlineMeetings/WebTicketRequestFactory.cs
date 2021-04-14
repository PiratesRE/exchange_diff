using System;
using System.Net;

namespace Microsoft.Exchange.Services.OnlineMeetings
{
	internal class WebTicketRequestFactory : UcwaRequestFactory
	{
		public WebTicketRequestFactory(string webTicket)
		{
			this.webTicket = webTicket;
		}

		public override string LandingPageToken
		{
			get
			{
				return "webticket";
			}
		}

		protected override HttpWebRequest CreateHttpWebRequest(string httpMethod, string url)
		{
			HttpWebRequest httpWebRequest = base.CreateHttpWebRequest(httpMethod, url);
			httpWebRequest.Headers.Add("X-MS-WebTicket", this.webTicket);
			return httpWebRequest;
		}

		private readonly string webTicket;
	}
}
