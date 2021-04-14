using System;
using System.Net;

namespace Microsoft.Exchange.Net.WebApplicationClient
{
	internal class WebSessionCookieContainer : CookieContainer
	{
		public WebSessionCookieContainer(WebSession webSession)
		{
			if (webSession == null)
			{
				throw new ArgumentNullException("webSession");
			}
			this.WebSession = webSession;
		}

		public WebSession WebSession { get; private set; }
	}
}
