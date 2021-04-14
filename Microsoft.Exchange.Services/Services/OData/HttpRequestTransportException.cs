using System;

namespace Microsoft.Exchange.Services.OData
{
	internal class HttpRequestTransportException : HttpTransportException
	{
		public HttpRequestTransportException(Exception innerException) : base(innerException)
		{
		}
	}
}
