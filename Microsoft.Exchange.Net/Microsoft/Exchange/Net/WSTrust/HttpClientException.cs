using System;

namespace Microsoft.Exchange.Net.WSTrust
{
	internal class HttpClientException : WSTrustException
	{
		public HttpClientException(Uri endpoint) : base(WSTrustStrings.HttpClientFailedToCommunicate(endpoint.ToString()))
		{
		}

		public HttpClientException(Uri endpoint, Exception innerException) : base(WSTrustStrings.HttpClientFailedToCommunicate(endpoint.ToString()), innerException)
		{
		}
	}
}
