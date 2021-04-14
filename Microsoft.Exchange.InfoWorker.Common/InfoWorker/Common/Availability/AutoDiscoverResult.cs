using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal sealed class AutoDiscoverResult
	{
		public AutoDiscoverResult(LocalizedException exception)
		{
			this.Exception = exception;
		}

		public AutoDiscoverResult(WebServiceUri webServiceUri, ProxyAuthenticator proxyAuthenticator)
		{
			this.ProxyAuthenticator = proxyAuthenticator;
			this.WebServiceUri = webServiceUri;
		}

		public WebServiceUri WebServiceUri { get; private set; }

		public ProxyAuthenticator ProxyAuthenticator { get; private set; }

		public LocalizedException Exception { get; private set; }
	}
}
