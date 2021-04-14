using System;

namespace Microsoft.Exchange.Servicelets.RPCHTTP
{
	internal sealed class WebSiteNotFoundException : Exception
	{
		public WebSiteNotFoundException(string webSiteName)
		{
			this.webSiteName = webSiteName;
		}

		public string WebSiteName
		{
			get
			{
				return this.webSiteName;
			}
		}

		private readonly string webSiteName;
	}
}
